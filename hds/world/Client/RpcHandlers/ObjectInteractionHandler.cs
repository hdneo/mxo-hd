using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using hds.shared;

namespace hds{

    public class ObjectInteractionHandler{

        private enum objectTypesStatic : int
        {
            COLLECTOR       = 0x01,
            HUMAN_NPC       = 0x02,
            DOOR            = 0x03,
            HARDLINE_UPLOAD = 0x04,
            HARDLINE_LAEXIT = 0x05,
            HARDLINE_SYNC   = 0x08, // Request Hardline Sync
            ENVIROMENT            = 0x00, // Bench too
        }

        private enum objectTypesDynamic : int
        {
            LOOT = 0x05
            
        }

        public void processTarget(ref byte[] packet)
        {
            byte[] viewBytes = { packet[0], packet[1] };
            UInt16 viewID = NumericalUtils.ByteArrayToUint16(viewBytes, 1);
            
        }

        public void processVendorOpen(ref byte[] objectID){
            // ToDo: Dynamic Shop Vendors
            PacketContent pak = new PacketContent();

            Vendor vendor = DataLoader.getInstance().getVendorByGoIDandMetrId(NumericalUtils.ByteArrayToUint32(objectID,1), 1);

            if (vendor != null)
            {
                // ToDo: send the Packet (and give it the items ?)
                ServerPackets serverPacket = new ServerPackets();
                serverPacket.sendVendorWindow(Store.currentClient, vendor);
            }

            //pak.addHexBytes("810D7CADD943000000A07EB1D1400000008000BEA940000000E0E74CA7402000140000140080002000800018008000040080002C008000F4118000540680004C088000BC06800000068000AC058000700D8000740D8000900D8000940D8000980D80009C0D8000240080003C0080001C0080");
            //Store.currentClient.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }

        public void processObjectInteraction(StaticWorldObject staticWorldObject, GameObjectItem item)
        {
            WorldSocket.gameServerEntities.Add(staticWorldObject);
            UInt16 typeId = NumericalUtils.ByteArrayToUint16(staticWorldObject.type, 1);

            Store.currentClient.playerData.newViewIdCounter++; // It is just for a test Later we will change this to have a List with Views and Object IDs

            NumericalUtils.uint16ToByteArrayShort(Store.currentClient.playerData.assignSpawnIdCounter());
            ServerPackets packets = new ServerPackets();
            packets.sendSystemChatMessage(Store.currentClient,"Door ID " + staticWorldObject.mxoId + " Type ID " + typeId.ToString() +" POS X:" + staticWorldObject.pos_x.ToString() + " Y:" + staticWorldObject.pos_y.ToString() + " Z:" + staticWorldObject.pos_z.ToString() + typeId,"BROADCAST");


            switch (typeId)
            {

                case 343:
                case 346:
                case 359:
                case 365:
                case 414:
                case 415:
                case 416:
                case 576:
                case 6958:
                case 6965:
                case 6963:
                case 6964:
                case 6972:
                    // ObjectAttribute364
                    ObjectAttributes364 door364 = new ObjectAttributes364("DOOR364",typeId,staticWorldObject.mxoId);
                    door364.DisableAllAttributes();
                    door364.Orientation.enable();
                    door364.Position.enable();
                    door364.CurrentState.enable();
                    // Set Values
                    door364.Position.setValue(NumericalUtils.doublesToLtVector3d(staticWorldObject.pos_x, staticWorldObject.pos_y, staticWorldObject.pos_z));
                    door364.CurrentState.setValue(StringUtils.hexStringToBytes("34080000"));
                    door364.Orientation.setValue(StringUtils.hexStringToBytes(staticWorldObject.quat));
                    //door364.Orientation.setValue(StringUtils.hexStringToBytes("000000000000803F0000000000000000")); // ToDo: Replace it with staticWorldObject.quat when it is okay

                    // ToDo: We make a little Entity "Hack" so that we have a unique id : metrid + fullmxostatic_id is entity
                    String entityMxOHackString = "" + staticWorldObject.metrId + "" + staticWorldObject.mxoId;
                    UInt64 entityId = UInt64.Parse(entityMxOHackString);

                    packets.sendSpawnStaticObject(Store.currentClient,door364,entityId);

                    break;

                 case 6601:
                 case 6994:
                 case 341:
                 case 417:
                 case 419:

                     ObjectAttributes363 door363 = new ObjectAttributes363("DOOR363",typeId,staticWorldObject.mxoId);
                     door363.DisableAllAttributes();
                     door363.Orientation.enable();
                     door363.Position.enable();
                     door363.CurrentState.enable();
                     // Set Values
                     door363.Position.setValue(NumericalUtils.doublesToLtVector3d(staticWorldObject.pos_x, staticWorldObject.pos_y, staticWorldObject.pos_z));
                     door363.Orientation.setValue(StringUtils.hexStringToBytes(staticWorldObject.quat));
                     //door363.Orientation.setValue(StringUtils.hexStringToBytes("000000000000803F0000000000000000")); // ToDo: Replace it with staticWorldObject.quat when it is okay
                     door363.CurrentState.setValue(StringUtils.hexStringToBytes("34080000"));

                     // ToDo: We make a little Entity "Hack" so that we have a unique id : metrid + fullmxostatic_id is entity
                     String entity363MxOHackString = "" + staticWorldObject.metrId + "" + staticWorldObject.mxoId;
                     UInt64 entity363Id = UInt64.Parse(entity363MxOHackString);

                     packets.sendSpawnStaticObject(Store.currentClient,door363,entity363Id);
                     break;
                  case 592:
                      new TeleportHandler().processHardlineExitRequest();
                      break;
                      
                  default:
                      new ServerPackets().sendSystemChatMessage(Store.currentClient, "Unknown Object Interaction with Object Type " + staticWorldObject.type + " and Name " + item.getName(), "MODAL");
                      // We take 364 as fallback
//                      ObjectAttributes364 fallbackDoor = new ObjectAttributes364("DOOR364",typeId,staticWorldObject.mxoId);
//                      fallbackDoor.DisableAllAttributes();
//                      fallbackDoor.Orientation.enable();
//                      fallbackDoor.Position.enable();
//                      fallbackDoor.CurrentState.enable();
//                      // Set Values
//                      fallbackDoor.Position.setValue(NumericalUtils.doublesToLtVector3d(staticWorldObject.pos_x, staticWorldObject.pos_y, staticWorldObject.pos_z));
//                      fallbackDoor.CurrentState.setValue(StringUtils.hexStringToBytes("34080000"));
//                      //fallbackDoor.Orientation.setValue(StringUtils.hexStringToBytes("000000000000803F0000000000000000")); // ToDo: Replace it with staticWorldObject.quat when it is okay
//                      fallbackDoor.Orientation.setValue(StringUtils.hexStringToBytes(staticWorldObject.quat));
//
//                      // ToDo: We make a little Entity "Hack" so that we have a unique id : metrid + fullmxostatic_id is entity
//                      String entityFallbackMxOHackString = "" + staticWorldObject.metrId + "" + staticWorldObject.mxoId;
//                      UInt64 entityFallbackId = UInt64.Parse(entityFallbackMxOHackString);
//                      packets.sendSpawnStaticObject(Store.currentClient,fallbackDoor,entityFallbackId);

                      break;

            }

            

        }

        public void processObjectStatic(ref byte[] packet){

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

                // Case 03 is not only a staticWorldObject ... 
                case (int)objectTypesStatic.DOOR:

                    Output.writeToLogForConsole("[OI HELPER] INTERACT WITH DOOR | Object ID :" + id + " Sector ID : " + numericSectorId);

                    // just a test
                    
                    if (objectValues != null && objectValues.type!=null)
                    {
                        
                        GameObjectItem item = objectLoader.getGameObjectItemById(NumericalUtils.ByteArrayToUint16(objectValues.type,1));
                        //Store.currentClient.messageQueue.addRpcMessage(PacketsUtils.createSystemMessage("[OI HELPER] Door StrType: !" + item.getName() + " with Type ID " + StringUtils.bytesToString_NS(objectValues.type), Store.currentClient));
                        this.processObjectInteraction(objectValues, item);
                    }
                    
                    break;

                case (int)objectTypesStatic.HARDLINE_SYNC:
                   
                    pak.sendSystemChatMessage(Store.currentClient, "Hardline Interaction(not done yet)!", "MODAL");
                    break;

                case (int)objectTypesStatic.HARDLINE_UPLOAD:
                    Output.writeToLogForConsole("[OBJECT HELPER] Upload Hardline will be used for combat tests");
                    Store.currentClient.playerData.lastClickedObjectId = numericObjectId;
                    new TestUnitHandler().processTestCombat(ref packet);
                    break;

                case (int)objectTypesStatic.HARDLINE_LAEXIT:
                    // Exit LA
                    pak.sendSystemChatMessage(Store.currentClient, "Exit to LA Dialog should popup", "MODAL");
                    new TeleportHandler().processHardlineExitRequest();
                    //new TestUnitHandler().processTestCombat(ref packet);
                    break;

                case (int)objectTypesStatic.HUMAN_NPC:
                    pak.sendSystemChatMessage(Store.currentClient, "Open Vendor Dialog for Object ID" + StringUtils.bytesToString_NS(objectID), "MODAL");
                    this.processVendorOpen(ref objectID);
                    break;

                case (int)objectTypesStatic.COLLECTOR:
                    pak.sendSystemChatMessage(Store.currentClient, "Collector Interaction (not done yet)!", "MODAL");
                    break;

                case (int)objectTypesStatic.ENVIROMENT:
                    GameObjectItem enviromentItem = objectLoader.getGameObjectItemById(NumericalUtils.ByteArrayToUint16(objectValues.type,1));
                    UInt16 enviromentTypeID = NumericalUtils.ByteArrayToUint16(objectValues.type, 1);
                    switch (enviromentTypeID)
                    {
                            case 6952:
                                // ToDo: implement Elevator Panel
                                pak.sendElevatorPanel(Store.currentClient, objectValues);
                                break;
                            default:
                                pak.sendSystemChatMessage(Store.currentClient, "Enviroment Type ID " + objectValues.type + " name " + enviromentItem.getName(), "MODAL");
                                break;
                    }
                    pak.sendSystemChatMessage(Store.currentClient, "Enviroment Type ID " + objectValues.type + " name " + enviromentItem.getName(), "MODAL");
                    break;
                

                default:
                    pak.sendSystemChatMessage(Store.currentClient, "[OI HELPER] Unknown Object Type : " + objectTypeID.ToString() + "| Object ID :" + id, "MODAL");
                    break;
            }

        }

        public void processObjectDynamic(ref byte[] packet)
        {

            PacketReader reader = new PacketReader(packet);

            byte[] objectID = new byte[4];
            byte[] sectorID = new byte[2];
            ArrayUtils.copyTo(packet, 0, objectID, 0, 4);
            ArrayUtils.copyTo(objectID, 2, sectorID, 0, 2);

            UInt32 numericObjectId = NumericalUtils.ByteArrayToUint32(objectID, 1);


            // Ok sector Bytes are something like 30 39 (reversed but the result must be 39 03)
            UInt16 numericSectorId = NumericalUtils.ByteArrayToUint16(sectorID, 1);
            // strip out object id
            string id = StringUtils.bytesToString_NS(objectID);


            // get the type 
            byte[] objectType = new byte[1];
            ArrayUtils.copy(packet, 4, objectType, 0, 1);
            int objectTypeID = packet[4];


            // create a new System message but fill it in the switch block
            ServerPackets pak = new ServerPackets();
            pak.sendSystemChatMessage(Store.currentClient,"Object Type ID IS " + objectTypeID.ToString() + " Dynamic Object RPC : " + StringUtils.bytesToString_NS(packet), "BROADCAST");
            switch (objectTypeID)
            {

                case (int)objectTypesDynamic.LOOT:
                    UInt32[] theTestArrayLoots = new UInt32[2];
                    theTestArrayLoots[0] = NumericalUtils.ByteArrayToUint32(StringUtils.hexStringToBytes("8e220000"), 1);
                    pak.sendLootWindow(5000,Store.currentClient,theTestArrayLoots);
                    break;


                default:
                    pak.sendSystemChatMessage(Store.currentClient, "[OI HELPER] Unknown Object Type : " + objectTypeID.ToString() + "| Object ID :" + id, "MODAL");
                    break;
            }

        }
    }
}
