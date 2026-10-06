using System;
using hds.shared;

namespace hds
{
    class TeleportHandler
    {
        private readonly WorldClient client;

        public TeleportHandler(WorldClient client)
        {
            this.client = client;
        }

        // ToDo: ALL!
        public void processHardlineStatusRequest(ref byte[] packet)
        {
            // NEED TO BE DONE CORRECTLY
        }

        /// <summary>
        /// Client RPC 0x108: it unloaded the world after our 0x107. Now the channel is reset; the world is
        /// loaded again (LOAD_WORLD) as first message of the new stream.
        /// </summary>
        public void processTeleportReset(ref byte[] packet)
        {
            client.BeginRccReset();
        }

        public void processHardlineTeleport(ref byte[] packet)
        {
            // we dont care where the journey goes
            // just want to see IF the journey will do :)
            // for this just ack and send 0x42 packet
            byte[] sourceHardline = new byte[2];
            sourceHardline[0] = packet[0];
            sourceHardline[1] = packet[1];

            byte[] sourceDistrict = new byte[2];
            sourceDistrict[0] = packet[4];
            sourceDistrict[1] = packet[5];

            byte[] destHardline = new byte[2];
            destHardline[0] = packet[8];
            destHardline[1] = packet[9];

            byte[] destDistrict = new byte[2];
            destDistrict[0] = packet[12];
            destDistrict[1] = packet[13];

            UInt16 sourceHL = NumericalUtils.ByteArrayToUint16(sourceHardline, 1);
            UInt16 sourceDIS = NumericalUtils.ByteArrayToUint16(sourceDistrict, 1);

            UInt16 destHL = NumericalUtils.ByteArrayToUint16(destHardline, 1);
            UInt16 destDIS = NumericalUtils.ByteArrayToUint16(destDistrict, 1);

            // This should do the magic - we just catch
            Store.dbManager.WorldDbHandler.updateLocationByHL(destDIS, destHL);
            Store.dbManager.WorldDbHandler.updateSourceHlForObjectTracking(sourceDIS, sourceHL,
                client.playerData.lastClickedObjectId);

#if DEBUG
            ServerPackets serverPak = new ServerPackets(client);
            serverPak.sendSystemChatMessage(client,
                "User wants teleport from : HL ID: " + sourceHL.ToString() + " (DIS: " + sourceDIS.ToString() +
                " ) TO HL ID: " + destHL.ToString() + " (DIS: " + destDIS.ToString() + ") ", "MODAL");
#endif

            // Tell client we want to unload the World
            PacketContent pak = new PacketContent();
            pak.AddUint16((UInt16) RPCResponseHeaders.SERVER_LOAD_RPC_RESET, 0);
            client.messageQueue.addRpcMessage(pak.ReturnFinalPacket());
            client.FlushQueue();
        }

        public void processHardlineExitConfirm(ref byte[] packet)
        {
            ServerPackets packets = new ServerPackets(client);
            switch (packet[0])
            {
                case 0x01:
                    ServerPackets serverPak = new ServerPackets(client);
#if DEBUG
                    serverPak.sendSystemChatMessage(client, "Exit to LA init...", "MODAL");
#endif

                    // Tell client we want to reset
                    byte[] response = {0x81, 0x07};
                    client.messageQueue.addRpcMessage(response);
                    break;

                case 0x02:
                    client.playerData.isJackoutInProgress = true;
                    client.playerData.jackoutStartTime = TimeUtils.getUnixTimeUint32();
                    packets.sendJackoutEffect(client);
                    break;
            }
        }

        public void processHardlineExitRequest()
        {
            ServerPackets packets = new ServerPackets(client);
            packets.sendJackoutEffect(client);
        }

        public void processGameFinish()
        {
            Store.dbManager.WorldDbHandler.SetOnlineStatus(client.playerData.getCharID(), 0);
            ServerPackets packets = new ServerPackets(client);
            packets.sendExitGame(client);
        }
    }
}