using System;
using hds.Core.Wire;
using hds.Game.Players;
using hds.Protocol.Mpm;
using hds.Protocol.Rpc;
using hds.shared;

namespace hds
{
    /// <summary>
    /// Handles decrypted MPM packets of one client on the world thread (replaces the old MultiProtocolManager).
    /// Sequences, acks and the channel reset are handled before by <see cref="WorldClient.processPacket"/>.
    /// </summary>
    public sealed class ClientPacketProcessor
    {
        private readonly RpcDispatcher dispatcher;

        public ClientPacketProcessor(RpcDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public void Process(WorldClient client, bool encrypted, byte[] data)
        {
            if (!encrypted)
            {
                ProcessPlain(client, data);
                return;
            }

            ClientFrame frame;
            try
            {
                frame = ClientFrame.Parse(data);
            }
            catch (WireFormatException ex)
            {
                Output.WriteClientViewRequestLog("[MPM] Could not parse client packet: " + ex.Message + "\r\n" +
                                                 StringUtils.bytesToString(data) + "\r\n");
                return;
            }

            if (!frame.HasFlag(ClientFrame.FlagAck))
            {
                Output.WriteDebugLog("[WARNING] Packet has ack not set and is encrypted");
            }

            if (frame.HasFlag(ClientFrame.FlagSimTime))
            {
                client.playerData.setClientSimTime(frame.SimTime);
            }

            foreach (var (_, update) in frame.Views)
            {
                SelfViewUpdates.Apply(client, update);
            }

            DispatchRpcs(client, frame);
        }

        private void DispatchRpcs(WorldClient client, ClientFrame frame)
        {
            if (frame.Rpcs.Count == 0) return;

            // Messages of one list carry consecutive counters starting at the list counter.
            // Lists the client resends because our ack got lost must not be executed twice.
            ushort currentList = 0;
            var indexInList = 0;
            var first = true;
            foreach (var message in frame.Rpcs)
            {
                if (first || message.ListCounter != currentList)
                {
                    currentList = message.ListCounter;
                    indexInList = 0;
                    first = false;
                }
                var counter = (ushort) (currentList + indexInList++);
                if (!client.AcceptIncomingRpc(counter))
                {
                    Output.WriteDebugLog($"[RPC] Skipping duplicate RPC 0x{message.Opcode:x} (counter {counter})");
                    continue;
                }

                try
                {
                    if (!dispatcher.Dispatch(client, message))
                    {
                        Output.WriteRpcLog($"RPCMAIN : Unknown Header 0x{message.Opcode:x} \n Content:\n {StringUtils.bytesToString_NS(message.Payload)}");
                    }
                }
                catch (Exception ex)
                {
                    Output.WriteLine($"[RPC] Handler for 0x{message.Opcode:x} failed: {ex}");
                }
            }
        }

        private static void ProcessPlain(WorldClient client, byte[] data)
        {
            // The first unencrypted packet opens the UDP session
            if (!client.playerData.getUDPSEssionEstablished() && data.Length >= 15)
            {
                var handler = new PlayerHandler(client);
                handler.processInitUDPSession(ref data);
                Output.WriteUnencryptedPacketLog(data, "CLIENT");
                client.playerData.setUDPSessionEstablished(true);
                handler.processPlayerSetup();
                handler.processAttributes();
                return;
            }

            Output.WriteUnencryptedPacketLog(data, "CLIENT");
            client.messageQueue.addRawMessage(data);
        }
    }
}
