using System;
using System.Collections;

using hds.shared;

namespace hds
{
    class TeleportHandler{

        // ToDo: ALL!
        public void processHardlineStatusRequest(ref byte[] packet){
            
            // NEED TO BE DONE CORRECTLY
        }

        public void processTeleportReset(ref byte[] packet){

            //1. Reset 
            /*
            WorldAnswer RPCReset;
            RPCReset = new WorldAnswer();
            RPCReset.encrypted = true;
            RPCReset.isAckOnly = true;
            RPCReset.timed = false;
            RPCReset.isRPCReset = true;
            Console.WriteLine("TELEPORT : 0x08 Reset Acked by Client - We just ack this again!");
             */
            Store.currentClient.playerData.setRPCShutDown(true);

            // We want to reset
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_LOAD_WORLD, 0);
            Store.currentClient.messageQueue.addRpcMessage(pak.returnFinalPacket());


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
            destHardline[0]   = packet[8];
            destHardline[1]   = packet[9];

            byte[] destDistrict = new byte[2];
            destDistrict[0]   = packet[12];
            destDistrict[1]   = packet[13];

            UInt16 sourceHL   = NumericalUtils.ByteArrayToUint16(sourceHardline,1);
            UInt16 sourceDIS = NumericalUtils.ByteArrayToUint16(sourceDistrict,1);

            UInt16 destHL = NumericalUtils.ByteArrayToUint16(destHardline, 1);
            UInt16 destDIS = NumericalUtils.ByteArrayToUint16(destDistrict,1);

            // This should do the magic - we just catch
            Store.dbManager.WorldDbHandler.updateLocationByHL(destDIS, destHL);
            Store.dbManager.WorldDbHandler.updateSourceHlForObjectTracking(sourceDIS, sourceHL, Store.currentClient.playerData.lastClickedObjectId);


            Output.writeToLogForConsole("User wants teleport from : HL ID: " + sourceHL.ToString() + " (DIS: " + sourceDIS.ToString() + " ) TO HL ID: " + destHL.ToString() + " (DIS: " + destDIS.ToString() + ") ");

            // Tell client we want to unload the World
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_LOAD_RPC_RESET, 0);


            Store.currentClient.messageQueue.addRpcMessage(pak.returnFinalPacket());

        }

        public void processHardlineExitConfirm(ref byte[] packet)
        {
            if (packet[0] == 0x01)
            {
                Store.currentClient.messageQueue.addRpcMessage(PacketsUtils.createMessage("Exit to LA init...", "MODAL", Store.currentClient));
                // Tell client we want to reset
                byte[] response = { 0x81, 0x07 };

                Store.currentClient.messageQueue.addRpcMessage(response);
            }

        }

        public void processHardlineExitRequest(ref byte[] packet)
        {
            DynamicArray din = new DynamicArray();

            byte[] header = { 0x80, 0xfb };

            din.append(header);
            din.append(0x95);
            din.append(0x00);
            din.append(0x00);
            din.append(0x00);
            din.append(0x01);

            Store.currentClient.messageQueue.addRpcMessage(din.getBytes());

        }


    }
}
