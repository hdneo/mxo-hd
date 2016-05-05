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
            UInt16 typeId = NumericalUtils.ByteArrayToUint16(door.type, 1);
            byte[] masterViewId = { 0x01, 0x00 };
            byte[] seperator = { 0xcd, 0xab };
            Store.currentClient.playerData.newViewIdCounter++; // It is just for a test Later we will change this to have a List with Views and Object IDs

            byte[] disarmDifficultyMaybe = { 0x03, 0x84 };
            byte[] endViewID = { 0x00, 0x00 };

            byte[] spawnCounter = NumericalUtils.uint16ToByteArrayShort(Store.currentClient.playerData.assignSpawnIdCounter());
            Output.WriteLine("[DOOR]POS X : " + door.pos_x.ToString() + " POS Y: " + door.pos_y.ToString() + " POS Z: " + door.pos_z.ToString() + ", TypeId: " + StringUtils.bytesToString_NS(door.type));

            switch (typeId)
            {
                case 417:
                case 419:
                    // ToDo: Packet Format for Elevator
                    // 02 03 01 00 
                    // 08 
                    // a3 01 
                    // 6b 01 f0 3d be 
                    // cd ab 03 88 00 00 00 00 ff ff 7f 3f 00 00 00 00 f3 04 35 33 
                    // 22 00 00 00 00 40 f4 fb 40 00 00 00 00 00 90 75 40 00 00 00 00 00 40 af 40 ff ff ff ff 19 00 00
                    // 11 00 01 00 04 61 97 e1 47 f0 bf 2d 44 de 30 35 45 00 00
                    PacketContent content = new PacketContent();
                    content.addByteArray(masterViewId);
                    content.addByte(0x08);
                    content.addByteArray(door.type);
                    content.addByteArray(NumericalUtils.uint32ToByteArray(door.mxoId, 1));
                    content.addByteArray(spawnCounter); // Spawn Object Counter
                    content.addByteArray(seperator);
                    content.addByte(0x03); // Number of Attributes to parse (3)
                    content.addByte(0x88); // GROUP 1 - more groups ON, Attribute 4 (305,Orientation,LTQuaternion,16) SET (10001000) 
                    content.addByteArray(StringUtils.hexStringToBytes(door.quat)); // ToDo: replace it later with the real LTQuaternion
                    content.addByte(0x22); // GROUP 2 - more groups OFF, Attribute 2,6  SET (00100010) 
                    content.addDoubleLtVector3d(door.pos_x, door.pos_y, door.pos_z);
                    content.addByteArray(new byte[]{ 0xff,0xff,0xff,0xff});
                    content.addByteArray(NumericalUtils.uint16ToByteArray(Store.currentClient.playerData.newViewIdCounter, 1));
                    content.addByteArray(endViewID);
                    content.addByte(0x00);
                    Store.currentClient.messageQueue.addObjectMessage(content.returnFinalPacket(), false);
                    break;
                case 2506:
                    // ToDo: Packet Format for Elevator
                    
                    
                    break;  
                default:

                    PacketContent contentDefault = new PacketContent();
                    contentDefault.addByteArray(masterViewId);
                    contentDefault.addByte(0x08);
                    contentDefault.addByteArray(door.type);
                    contentDefault.addByteArray(NumericalUtils.uint32ToByteArray(door.mxoId, 1));
                    contentDefault.addByteArray(spawnCounter); // Spawn Object Counter
                    contentDefault.addByteArray(seperator);
                    contentDefault.addByteArray(disarmDifficultyMaybe);
                    contentDefault.addByte(0x00); // isZionAligned?
                    contentDefault.addByteArray(StringUtils.hexStringToBytes("0000000000803F000000000000000041")); // ToDo: replace it later with the real values from the object
                    contentDefault.addDoubleLtVector3d(door.pos_x, door.pos_y, door.pos_z);
                    contentDefault.addByteArray(StringUtils.hexStringToBytes("34080000"));
                    contentDefault.addByteArray(NumericalUtils.uint16ToByteArray(Store.currentClient.playerData.newViewIdCounter, 1));
                    contentDefault.addByteArray(endViewID);
                    contentDefault.addByte(0x00);
                    Store.currentClient.messageQueue.addObjectMessage(contentDefault.returnFinalPacket(), false);
                    break;

            }
            

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
            ServerPackets pak = new ServerPackets();

            switch (objectTypeID){

                case (int)objectTypes.DOOR:
                    Output.writeToLogForConsole("[OI HELPER] INTERACT WITH DOOR | Object ID :" + id + " Sector ID : " + numericSectorId);

                    // just a test
                    
                    if (objectValues != null && objectValues.type!=null)
                    {
                        GameObjectItem item = objectLoader.getGameObjectItemById(NumericalUtils.ByteArrayToUint16(objectValues.type,1));
                        Store.currentClient.messageQueue.addRpcMessage(PacketsUtils.createSystemMessage("[OI HELPER] Door StrType: !" + item.getName() + " with Type ID " + StringUtils.bytesToString_NS(objectValues.type), Store.currentClient));
                        this.processOpenDoor(objectValues);
                    }
                    
                    break;

                case (int)objectTypes.HARDLINE_SYNC:
                   
                    pak.sendSystemChatMessage(Store.currentClient, "Hardline Interaction(not done yet)!", "MODAL");
                    break;

                case (int)objectTypes.HARDLINE_UPLOAD:
                    Output.writeToLogForConsole("[OBJECT HELPER] Upload Hardline will be used for combat tests");
                    Store.currentClient.playerData.lastClickedObjectId = numericObjectId;
                    new TestUnitHandler().processTestCombat(ref packet);
                    break;

                case (int)objectTypes.HARDLINE_LAEXIT:
                    // Exit LA
                    pak.sendSystemChatMessage(Store.currentClient, "Exit to LA Dialog should popup", "MODAL");
                    new TeleportHandler().processHardlineExitRequest(ref packet);
                    //new TestUnitHandler().processTestCombat(ref packet);
                    break;

                case (int)objectTypes.HUMAN_NPC:
                    pak.sendSystemChatMessage(Store.currentClient, "NPC Interaction (not done yet)!", "MODAL");
                    this.processVendorOpen(ref objectID);
                    break;

                case (int)objectTypes.COLLECTOR:
                    pak.sendSystemChatMessage(Store.currentClient, "Collector Interaction (not done yet)!", "MODAL");
                    break;

                case (int)objectTypes.PIPE:
                    pak.sendSystemChatMessage(Store.currentClient, "Collector Interaction (not done yet)!", "MODAL");
                    break;
                

                default:
                    pak.sendSystemChatMessage(Store.currentClient, "[OI HELPER] Unknown Object Type : " + objectTypeID.ToString() + "| Object ID :" + id, "MODAL");
                    break;
            }

        }
    }
}
