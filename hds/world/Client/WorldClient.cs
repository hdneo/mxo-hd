using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using hds.Protocol.Mpm;
using hds.shared;

namespace hds
{
    public class WorldClient
    {
        private EndPoint Remote;
        private Socket socket;
        private IWorldEncryption cypher;

        private string key;

        private UInt32 lastUsedTime;
        private int deadSignals;
        private bool alive;
        private bool ackOwed;
        private readonly OutgoingStream outgoing = new OutgoingStream();

        /// <summary>Sequence / ack state of the reliable channel.</summary>
        public ReliableChannel Channel { get; } = new ReliableChannel();

        public ClientData playerData { get; set; }
        public MessageQueue messageQueue;
        public ViewManager viewMan { get; set; }
        public PlayerCharacter playerInstance { get; set; }
        public hds.Game.Abilities.PlayerAbilities Abilities { get; } = new hds.Game.Abilities.PlayerAbilities();
        /// <summary>Interlock tactic the player selected (RPC 0x42).</summary>
        public hds.Game.Combat.CombatTactic CombatTactic { get; set; } = hds.Game.Combat.CombatTactic.Speed;

        public WorldClient(EndPoint _Remote, Socket _socket, string _key)
        {
            Remote = _Remote;
            socket = _socket;
            key = _key;
            cypher = new WorldEncryption();

            messageQueue = new MessageQueue();

            // Create the mpm structure for all world usage

            playerData = new ClientData();
            viewMan = new ViewManager();

            playerData.setUniqueKey(key);
            playerInstance = Store.world.objMan.GetAssignedObject(key);


            alive = true;
            deadSignals = 0;
        }


        // This method sends one packet at a time to the game client 
        private void sendPacket(byte[] data)
        {
            if (this.deadSignals == 0)
            {
                //Output.WriteLine("[SEND PACKET] PSS: " + playerData.getPss().ToString() + " SSEQ : " + playerData.getSseq().ToString() + " CSEQ: " + playerData.getCseq().ToString());
                try
                {
                    socket.BeginSendTo(data, 0, data.Length, SocketFlags.None, Remote, new AsyncCallback(FinalSendTo), Remote);
                }
                catch (Exception ex)
                {
                    #if DEBUG
                    Output.WriteDebugLog("Socket Error " + ex.Message);
                    #endif
                    alive = false;
                }
            }
        }

        private void FinalSendTo(IAsyncResult asyncResult)
        {
            try
            {
                socket.EndSend(asyncResult);
            }
            catch (Exception ex)
            {
                Output.WriteDebugLog("SendData Error: " + ex.Message);
            }
        }


        /// <summary>Called by the world loop: resends messages of packets that were not acked in time.</summary>
        public void CheckAndResend()
        {
            if (Channel.Resetting)
            {
                // keep asking until the client answers the reset
                SendReliable(outgoing.BuildEmptyPacket(Channel, TimeUtils.getCurrentSimTimeMs(), hds.Core.ServerClock.SimSecondsFloat));
                return;
            }
            if (outgoing.ResendTimedOut(TimeUtils.getCurrentSimTimeMs()) > 0)
            {
                FlushQueue();
            }
        }

        /// <summary>
        /// Sends all waiting messages. If nothing is waiting but the client sent us something, an ack only
        /// packet is sent (the original server answered every client packet).
        /// </summary>
        public void FlushQueue()
        {
            SendRawMessages();

            var now = TimeUtils.getCurrentSimTimeMs();
            var simTime = hds.Core.ServerClock.SimSecondsFloat;
            var packets = outgoing.BuildPackets(messageQueue, Channel, now, simTime);
            if (packets.Count == 0 && ackOwed)
            {
                packets.Add(outgoing.BuildEmptyPacket(Channel, now, simTime));
            }
            ackOwed = false;

            foreach (var packet in packets)
            {
                SendReliable(packet);
            }
        }

        private void SendReliable(byte[] finalData)
        {
            var stopWatch = Stopwatch.StartNew();
            byte[] encryptedData = cypher.encrypt(finalData, finalData.Length, Channel.ReceivedBits,
                Channel.RemoteSequence, Channel.LocalSequence);
            sendPacket(encryptedData);
            stopWatch.Stop();

            Output.WritePacketLog(this, finalData, "SERVER",
                Channel.ReceivedBits.ToString("x2"), Channel.RemoteSequence.ToString(),
                Channel.LocalSequence.ToString(), stopWatch.Elapsed.TotalMilliseconds.ToString(), "ENCRYPT");
        }

        private void SendRawMessages()
        {
            if (messageQueue.rawMessages.Count == 0) return;
            foreach (byte[] message in messageQueue.rawMessages)
            {
                Output.WriteUnencryptedPacketLog(message, "SERVER");
                sendPacket(message);
            }
            messageQueue.rawMessages.Clear();
        }

        /// <summary>
        /// Starts a world change (client sent RPC 0x108 after our 0x107): the current stream ends, every packet
        /// asks the client to reset until it confirms, see <see cref="hds.Protocol.Mpm.ReliableChannel"/>.
        /// </summary>
        public void BeginRccReset()
        {
            if (Channel.Resetting) return;
            Output.WriteDebugLog("[RCC] world change: requesting channel reset");
            Store.CloseCombat?.OnClientRemoved(this);
            playerData.setRPCShutDown(true);
            playerData.setOnWorld(false);
            Channel.BeginReset();
            messageQueue.Clear();
            outgoing.Clear();
            ackOwed = true;
            FlushQueue();
        }

        /// <summary>The client confirmed the reset: restart both streams and load the world again.</summary>
        private void CompleteRccReset()
        {
            Output.WriteDebugLog("[RCC] client confirmed the reset - loading the world");
            Channel.CompleteReset();
            messageQueue.Clear();
            outgoing.Clear();
            ResetIncomingRpcCounter();
            viewMan = new ViewManager();
            Abilities.ForgetSentBonuses();
            playerData.setRPCShutDown(false);
            playerData.setOnWorld(false);

            var handler = new PlayerHandler(this);
            handler.processPlayerSetup();
            handler.processAttributes();
        }

        /// <summary>Decrypts a packet: returns the payload and the header (ack bits, ack, sequence).</summary>
        private byte[] DecryptReceivedPacket(byte[] packet, out byte ackBits, out ushort sequence, out ushort ack)
        {
            ArrayList decValues = cypher.decrypt(packet, packet.Length);
            ackBits = (byte) (UInt16) decValues[1];
            sequence = (UInt16) decValues[2];
            ack = (UInt16) decValues[3];
            return (byte[]) decValues[0];
        }

        public void processPacket(byte[] packet)
        {
            lastUsedTime = TimeUtils.getUnixTimeUint32();
            if (packet.Length == 0) return;

            if (packet[0] == 0x00)
            {
                // plain packet (session setup and pings)
                Output.WritePacketLog(packet, "CLIENT", Channel.ReceivedBits.ToString("x2"),
                    Channel.RemoteSequence.ToString(), Channel.LocalSequence.ToString());
                Store.PacketProcessor.Process(this, false, packet);
                FlushQueue();
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            var payload = DecryptReceivedPacket(packet, out var ackBits, out var sequence, out var ack);
            stopwatch.Stop();
            Output.WritePacketLog(this, payload, "CLIENT", ackBits.ToString("x2"), sequence.ToString(), ack.ToString(),
                stopwatch.Elapsed.TotalMilliseconds.ToString(), "DECRYPT");
            if (payload.Length == 0) return;

            var status = payload[0];
            if (Channel.Resetting)
            {
                // 0x40 = the client echoes our reset request, 0x20 = it already sends its new stream
                if ((status & (ReliableChannel.StatusResetRcc | ReliableChannel.StatusClientResetDone)) == 0)
                {
                    ackOwed = true;
                    FlushQueue();
                    return;
                }
                CompleteRccReset();
                if ((status & ReliableChannel.StatusClientResetDone) != 0) Channel.RegisterIncoming(sequence);
                FlushQueue();
                return;
            }

            if ((status & ReliableChannel.StatusResetRcc) != 0)
            {
                // late echo of a reset that is already done
                return;
            }

            outgoing.OnAck(messageQueue, ack, ackBits);
            ackOwed = true;
            if (Channel.RegisterIncoming(sequence))
            {
                Store.PacketProcessor.Process(this, true, payload);
            }
            FlushQueue();
        }

        private readonly hds.Protocol.Rpc.IncomingRpcSequence incomingRpcs = new hds.Protocol.Rpc.IncomingRpcSequence();

        /// <summary>False for RPCs that were already executed (see <see cref="hds.Protocol.Rpc.IncomingRpcSequence"/>).</summary>
        public bool AcceptIncomingRpc(ushort counter) => incomingRpcs.Accept(counter);

        public void ResetIncomingRpcCounter() => incomingRpcs.Reset();

        /// <summary>Queues a server RPC (opcode without the 0x80 marker, e.g. 0xB2 for the old 0x80B2 header).</summary>
        public void SendRpc(ushort opcode, byte[] payload)
        {
            var writer = new hds.Core.Wire.WireWriter(payload.Length + 2);
            writer.WriteVarSize(opcode);
            writer.WriteBytes(payload);
            messageQueue.addRpcMessage(writer.ToArray());
        }

        public bool Alive
        {
            get
            {
                UInt32 now = TimeUtils.getUnixTimeUint32();
                if (!this.alive || (now - lastUsedTime > 30))
                {
                    return false;
                }
                return true;
            }
            set { alive = value; }
        }
    }
}