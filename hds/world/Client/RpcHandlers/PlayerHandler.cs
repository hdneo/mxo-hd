using System;
using System.Collections.Generic;
using System.Text;

using hds.shared;

namespace hds
{
    class PlayerHandler
    {

        // Important: We musts load this after the setup state is set to 0x0f (so before that it wouldnt work properly)
        public void processAttributes()
        {
            ServerPackets server = new ServerPackets();
            server.sendPlayerAttributes(Store.currentClient);
            server.sendPlayerFriendList(Store.currentClient);
            //Store.currentClient.playerData.setPss(0x7f);
        }

        public void processMood(ref byte[] packet)
        {
            byte moodByte = packet[0];
            ServerPackets server = new ServerPackets();
            //ToDo: Announce to other Players (and find packet for it) and save this in playerObject for new players
            server.sendMoodChange(Store.currentClient, moodByte);
        }

        public void processEmote(ref byte[] packet)
        {
            byte[] emoteBytes = new byte[4];
            emoteBytes[0] = packet[0];
            emoteBytes[1] = packet[1];
            emoteBytes[2] = packet[2];
            emoteBytes[3] = packet[3];

            byte emoteByte = packet[0];
            UInt32 emoteKey = NumericalUtils.ByteArrayToUint32(emoteBytes, 0);
            
            ServerPackets server = new ServerPackets();
            server.sendEmotePerform(Store.currentClient, emoteKey);
        }

        public void processSpawn()
        {

            if (Store.currentClient.playerData.getDistrict()!="la")
            {
                // REFACTOR!!!!
                byte[] rsiObject = new BootingHelperRsi().generateSelfSpawnPacket(Store.currentClient);
                //Store.world.sendViewPacketToAllPlayers(new BootingHelperRsi().generatePlayerSpawnPacket(Store.currentClient), Store.currentClient.playerData.getCharID(), NumericalUtils.ByteArrayToUint16(Store.currentClient.playerInstance.getGoid(), 1), Store.currentClient.playerData.getEntityId());

                Store.currentClient.messageQueue.addObjectMessage(rsiObject, false);
                Store.currentClient.playerData.setOnWorld(true);
            }
            

        }

        public void processPlayerSetup()
        {
            // REFACTOR: Move to PlayerHandler and PlayerPackets (and location header to Server Packets)	

            ServerPackets packets = new ServerPackets();

            packets.sendWorldCMD(Store.currentClient, Store.currentClient.playerData.getDistrictId(), "bluesky1"); // Test our skies
            //packets.sendWorldCMD(Store.currentClient, Store.currentClient.playerData.getDistrictId(),"Massive");
            //packets.sendWorldCMD(Store.currentClient, Store.currentClient.playerData.getDistrictId(),"Massive,WinterSky3");
            //packets.sendWorldCMD(Store.currentClient, Store.currentClient.playerData.getDistrictId(), "bluesky2");

            packets.sendEXPCurrent(Store.currentClient, (UInt32)Store.currentClient.playerData.getExperience());
            packets.sendInfoCurrent(Store.currentClient, (UInt32)Store.currentClient.playerData.getInfo());

            /*
            long exp = Store.currentClient.playerData.getExperience();
            long cash = Store.currentClient.playerData.getInfo();
            string expStr = "80e1" + StringUtils.bytesToString_NS(NumericalUtils.uint32ToByteArray((UInt32)exp, 1)) + "00000000";
            string cashStr = "80df" + StringUtils.bytesToString_NS(NumericalUtils.uint32ToByteArray((UInt32)cash, 1)) + "00000000";
            */

            // There are for testing - need to change this later to load from ClientObject / DB 
            UInt16 focus = 24;
            UInt16 belief = 25;
            UInt16 vitality = 26;
            UInt16 perception = 27;
            UInt16 reason = 28;

            
            packets.sendAttribute(Store.currentClient, focus, 0x4e);
            packets.sendAttribute(Store.currentClient, perception, 0x4f);
            packets.sendAttribute(Store.currentClient, reason, 0x51);
            packets.sendAttribute(Store.currentClient, belief, 0x52);
            packets.sendAttribute(Store.currentClient, vitality, 0x54);
            
            /*
            string focusRPC = "80ad4e" + StringUtils.bytesToString_NS(NumericalUtils.uint16ToByteArray(focus, 0)) + "000802";
            string beliefRPC = "80ad52" + StringUtils.bytesToString_NS(NumericalUtils.uint16ToByteArray(belief, 0)) + "000802";
            string vitalityRPC = "80ad54" + StringUtils.bytesToString_NS(NumericalUtils.uint16ToByteArray(vitality, 0)) + "000802";
            string perceptionRPC = "80ad4f" + StringUtils.bytesToString_NS(NumericalUtils.uint16ToByteArray(perception, 0)) + "000802";
            string reasonRPC = "80ad51" + StringUtils.bytesToString_NS(NumericalUtils.uint16ToByteArray(reason, 0)) + "000802";

            
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes(focusRPC)); // Focus 
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes(beliefRPC)); // Belief
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes(vitalityRPC)); // Vitality
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes(perceptionRPC)); // Perception
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes(reasonRPC)); // Reason
            */
            //Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("808615A0070000000000000000000000000000000000000000210000000000230000000000"));
            // Disable later
            /*
             * 8167170020001C2200C60111000000000000002900000807006D786F656D750007006D786F656D750002000200000000000000
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80b2110011000802")); // What is this ? Check it later

            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc4503110000020000001100110000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc450002000002000000cc00040000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45000300000b0000003702330000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45000400002d0000002304030000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45000500002d0000002004030000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45000600002d0000001904010000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45000700002d0000004204010000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc4500080000340000008302440000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45000900004c0000001504040000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45000a00004c0000000604040000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45000b00004c0000001904020000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45000c0000530000001504010000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45000d0000530000001904010000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80b23a0400000802"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45033a0400530000003a04000000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45000e0000530000000604010000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45000f00005f0000007c04050000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45001000005f0000000704050000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc4500110000620000000604010000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80b2350400000802"));
            */
            // Test icon + bonus EDIT DOESNT WORK IN THIS STATE
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc1500450000f70300000702ecffffff0000000000"));

        }

        public void processInitUDPSession(ref byte[] packetData)
        {

            byte[] cIDHex = new byte[4];
            cIDHex[0] = packetData[11];
            cIDHex[1] = packetData[12];
            cIDHex[2] = packetData[13];
            cIDHex[3] = packetData[14];

            Store.currentClient.playerData.setCharID(NumericalUtils.ByteArrayToUint32(cIDHex, 1));
            //_playerData.setObjectID((UInt16)(_playerData.getCharID() + (UInt16)8000));

            Store.dbManager.WorldDbHandler.setPlayerValues();
            Store.dbManager.WorldDbHandler.setRsiValues();

            // send the init UDP packet * 5
            byte[] response = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05 };
            Store.currentClient.messageQueue.addRawMessage(response);
            Store.currentClient.messageQueue.addRawMessage(response);
            Store.currentClient.messageQueue.addRawMessage(response);
            Store.currentClient.messageQueue.addRawMessage(response);
            Store.currentClient.messageQueue.addRawMessage(response);
            Store.currentClient.flushQueue();
            Store.margin.sendUDPSessionReply(Store.currentClient.playerData.getCharID());
            

        }
    }
}
