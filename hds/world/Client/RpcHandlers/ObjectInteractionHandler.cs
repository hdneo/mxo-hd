using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using hds.shared;

namespace hds{

    public class ObjectInteractionHandler{

        private enum objectTypes : int
        {
            COLLECTOR       = 0x01,
            HUMAN_NPC       = 0x02,
            DOOR            = 0x03,
            HARDLINE_UPLOAD = 0x04,
            HARDLINE_LAEXIT = 0x05,
            HARDLINE_SYNC   = 0x08, // Request Hardline Sync
            PIPE            = 0x00, // Bench too
        }

        public void processTarget(ref byte[] packet)
        {
            byte[] viewBytes = { packet[0], packet[1] };
            UInt16 viewID = NumericalUtils.ByteArrayToUint16(viewBytes, 1);
            
        }

        public void processVendorOpen(ref byte[] objectID){
            PacketContent pak = new PacketContent();
            pak.addHexBytes("810D7CADD943000000A07EB1D1400000008000BEA940000000E0E74CA7402000140000140080002000800018008000040080002C008000F4118000540680004C088000BC06800000068000AC058000700D8000740D8000900D8000940D8000980D80009C0D8000240080003C0080001C0080");
            Store.currentClient.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }

        public void processOpenDoor(StaticWorldObject door)
        {
            
            /* Example Packet
             *  02 
                03 
                01 00 
                08 a0 01 
                08 00 f0 34 
                be 
                cd ab 
                03 84 00 
                00 00 00 f3 04 35 bf 00 
                00 00 00 f3 04 35 3f 41 
                00 00 00 00 80 a6 f0 c0 
                00 00 00 00 00 20 62 40 
                00 00 00 00 00 b3 d0 40 
                34 08 00 00 12 00 00 00 00 
             */
            Store.currentClient.playerData.newViewIdCounter++; // It is just for a test Later we will change this to have a List with Views and Object IDs
            byte[] masterViewId = { 0x01, 0x00 };
            
            byte[] typeId = StringUtils.hexStringToBytes("8502");
            byte[] seperator = { 0xcd, 0xab };
            byte[] disarmDifficultyMaybe = { 0x03, 0x84 };
            byte[] newViewId = {0x22, 0x00};
            byte[] endViewID = { 0x00, 0x00 };


            Random rand = new Random();
            UInt16 randSpawncounter = (UInt16)rand.Next(15, 254);

            byte[] spawnCounter = NumericalUtils.uint16ToByteArrayShort(randSpawncounter);


            PacketContent content = new PacketContent();
            content.addByteArray(masterViewId);
            content.addByte(0x08);
            content.addByteArray(door.type);
            content.addByteArray(NumericalUtils.uint32ToByteArray(door.mxoId, 1));
            content.addByteArray(spawnCounter); // Spawn Object Counter
            content.addByteArray(seperator);
            content.addByteArray(disarmDifficultyMaybe);
            content.addByte(0x00); // isZionAligned?
            //content.addByteArray(StringUtils.hexStringToBytes(door.quat));
            content.addByteArray(StringUtils.hexStringToBytes("0000000000803F000000000000000041"));
            //content.addByteArray(NumericalUtils.doubleToByteArray(door.rot,1));
            content.addDoubleLtVector3d(door.pos_x,door.pos_y,door.pos_z);
            content.addByteArray(StringUtils.hexStringToBytes("34080000"));
            content.addByteArray(NumericalUtils.uint16ToByteArray(Store.currentClient.playerData.newViewIdCounter,1));
            content.addByteArray(endViewID);
            content.addByte(0x00);

            Output.WriteLine("[DOOR]POS X : " + door.pos_x.ToString() + " POS Y: " + door.pos_y.ToString() + " POS Z: " + door.pos_z.ToString() + ", TypeId: " + StringUtils.bytesToString_NS(door.type) );
            Store.currentClient.messageQueue.addObjectMessage(content.returnFinalPacket(), false);
            

        }

        public void processObject(ref byte[] packet){

            byte[] objectID = new byte[4];
            byte[] sectorID = new byte[2];
            ArrayUtils.copyTo(packet, 0, objectID, 0, 4);
            ArrayUtils.copyTo(objectID, 2, sectorID, 0, 2);


            UInt32 numericObjectId = NumericalUtils.ByteArrayToUint32(objectID,1);

            // Ok sector Bytes are something like 30 39 (reversed but the result must be 39 03)
            UInt16 numericSectorId = NumericalUtils.ByteArrayToUint16(sectorID,1);
            // strip out object id
            string id = StringUtils.bytesToString_NS(objectID);
            

            // get the type 
            byte[] objectType = new byte[1];
            ArrayUtils.copy(packet, 4, objectType, 0, 1);
            int objectTypeID = packet[4];

            DataLoader objectLoader = DataLoader.getInstance();
            StaticWorldObject objectValues = objectLoader.getObjectValues(NumericalUtils.ByteArrayToUint32(objectID, 1));

            // create a new System message but fill it in the switch block

            switch(objectTypeID){

                case (int)objectTypes.DOOR:
                    Output.writeToLogForConsole("[OI HELPER] INTERACT WITH DOOR | Object ID :" + id + " Sector ID : " + numericSectorId);

                    // just a test
                    if (objectValues != null)
                    {
                        Store.currentClient.messageQueue.addRpcMessage(PacketsUtils.createSystemMessage("[OI HELPER] Door StrType: !" + objectValues.strType + " with Type ID " + StringUtils.bytesToString_NS(objectValues.type), Store.currentClient));
                        this.processOpenDoor(objectValues);
                    }
                    
                    break;

                case (int)objectTypes.HARDLINE_SYNC:
                    Output.writeToLogForConsole("[OI HELPER] INTERACT WITH HARDLINE | Object ID :" + id);
                    Store.currentClient.messageQueue.addRpcMessage(PacketsUtils.createMessage("Hardline Interaction (not done yet)!", "MODAL", Store.currentClient));
                    break;

                case (int)objectTypes.HARDLINE_UPLOAD:
                    Output.writeToLogForConsole("[OBJECT HELPER] Upload Hardline will be used for combat tests");
                    Store.currentClient.playerData.lastClickedObjectId = numericObjectId;
                    new TestUnitHandler().processTestCombat(ref packet);
                    break;

                case (int)objectTypes.HARDLINE_LAEXIT:
                    // Exit LA
                    Store.currentClient.messageQueue.addRpcMessage(PacketsUtils.createMessage("Exit to LA Dialog should popup", "MODAL", Store.currentClient));
                    new TeleportHandler().processHardlineExitRequest(ref packet);
                    //new TestUnitHandler().processTestCombat(ref packet);
                    break;

                case (int)objectTypes.HUMAN_NPC:
                    Output.writeToLogForConsole("[OI HELPER] INTERACT WITH HUMAN NPC | Object ID :" + id);
                    Store.currentClient.messageQueue.addRpcMessage(PacketsUtils.createMessage("NPC Interaction (not done yet)!", "MODAL", Store.currentClient));
                    this.processVendorOpen(ref objectID);
                    break;

                case (int)objectTypes.COLLECTOR:
                    Output.writeToLogForConsole("[OI HELPER] INTERACT WITH COLLECTOR | Object ID :" + id);
                    Store.currentClient.messageQueue.addRpcMessage(PacketsUtils.createMessage("Collector Interaction (not done yet)!", "MODAL", Store.currentClient));                    
                    break;

                case (int)objectTypes.PIPE:
                    Output.writeToLogForConsole("[OI HELPER] INTERACT WITH PIPE/BENCH | Object ID :" + id);
                    Store.currentClient.messageQueue.addRpcMessage(PacketsUtils.createMessage("PIPE/BENCH/others Interaction (not done yet)!", "MODAL", Store.currentClient));
                    break;
                

                default:
                    Output.writeToLogForConsole("[OI HELPER] Unknown Object Type : " + objectTypeID.ToString() + "| Object ID :" + id);
                    Store.currentClient.messageQueue.addRpcMessage(PacketsUtils.createMessage("[OI HELPER] Unknown Object Type : " + objectTypeID.ToString() + "| Object ID :" + id, "MODAL", Store.currentClient));
                    break;
            }

        }
    }
}
