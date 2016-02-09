using System;
using System.Collections;
using System.Linq;
using System.Text;
using hds.databases;
using hds.shared;
using hds.world.Structures;

namespace hds
{
    public partial class ServerPackets 
    {
        

        public void sendDeleteViewPacket(WorldClient client, UInt16 viewIdToDelete)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16(1, 1); // Master View
            pak.addByte(0x01);
            pak.addUint16(1,1); // Num Views to delete - current just this one (maybe later more)
            pak.addUint16(viewIdToDelete, 1);
            pak.addByte(0x00);

            client.messageQueue.addObjectMessage(pak.returnFinalPacket(), false);
            client.flushQueue();

        }

        public void sendEmotePerform(WorldClient client, UInt32 emoteID)
        {
            double x = 0; double y = 0; double z = 0;
            byte[] Ltvector3d = Store.currentClient.playerInstance.Position.getValue();
            NumericalUtils.LtVector3dToDoubles(Ltvector3d, ref x, ref y, ref z);
            int rotation = (int)Store.currentClient.playerInstance.YawInterval.getValue()[0];
            float xPos = (float)x;
            float yPos = (float)y;
            float zPos = (float)z;

            /*
            byte sampleEmoteMsg[] =
            {
                0x03, 0x02, 0x00, 0x01, 0x28, 0xAA, 0x40, 0x00, 0x25, 0x01, 0x00, 0x00, 0x10, 0xBB, 0xBB, 0xBB,
                0xBB, 0xCC, 0xCC, 0xCC, 0xCC, 0xDD, 0xDD, 0xDD, 0xDD, 0x2A, 0x9F, 0x1E, 0x20, 0x00, 0x00,
            };*/

            // Example REsponse for /rolldice  02 03 02 00 01 28 01 40 00 25 <ID> 00 00 10 cd a7 65 c7 00 00 be 42 33 ff 72 46 b9 51 32 22 00 00
            // Example REsponse for /rolldice  02 03 02 00 01 28 01 40 00 25 5c 00 00 10 cd a7 65 c7 00 00 be 42 33 ff 72 46 b9 51 32 22 00 00
            // Example Response for ClapEmote: 02 03 02 00 01 28 06 40 00 25 04 00 00 10 34 49 84 c7 00 00 be 42 27 a4 7f 46 b3 a6 5e 18 00 00 
            // Python Example self.emotePacket="\x02\x03\x02\x00\x01\x28<emoteNum>\x40\x00\x25<emoteID>\x00\x00\x10<coordX><coordY><coordZ>\x2a\x9f\x1e\x20\x00\x00"
            //                                 02 03 02 00 01 28 01 40 00 25 01 00 00 ea 33 47 00 00 be 42 80 55 bd c7 b9 51 32 22 00 00 


            // ToDo: parse list and get the fucking id from uint
            DataLoader objectLoader = DataLoader.getInstance();
            byte emoteByte = new byte();
            emoteByte = objectLoader.findEmoteByLongId(emoteID);
            

            if(emoteByte>0)
            {
                byte[] emotePak = { 0x01, 0x28, 0x01, 0x40, 0x00, 0x25, emoteByte, 0x00, 0x00, 0x10 };
                PacketContent pak = new PacketContent();
                pak.addUint16(2, 1);
                pak.addByteArray(emotePak);
                pak.addFloatLtVector3f(xPos, yPos, zPos);
                pak.addHexBytes("b9513222"); // We dont know what they are - maybe rotation ?
                pak.addByte(0x00);
                //pak.addByteArray(client.playerInstance.Position.getValue());
                client.messageQueue.addObjectMessage(pak.returnFinalPacket(), false);
                client.flushQueue();
            }
            

        }

        public void sendMoodChange(WorldClient client, byte moodByte)
        {
            byte[] moodPak = { 0x02, 0x00, 0x01, 0x01, 0x00, moodByte, 0x00, 0x00 };
            PacketContent pak = new PacketContent();
            pak.addUint16(2, 1);
            pak.addByte(0x02);
            pak.addByteArray(moodPak);
            client.messageQueue.addObjectMessage(pak.returnFinalPacket(), false);
            client.flushQueue();

        }

        public void sendAttribute(WorldClient client, UInt16 attributeValue, byte type)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_PLAYER_ATTRIBUTE, 0);
            pak.addByte(type);
            pak.addUint16(attributeValue, 0);
            pak.addHexBytes("000802");
            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }

        public void sendEXPCurrent(WorldClient client, UInt32 exp)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_PLAYER_EXP, 0);
            pak.addUint32(exp, 1);
            pak.addHexBytes("00000000");
            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }

        public void sendInfoCurrent(WorldClient client, UInt32 info)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_PLAYER_INFO, 0);
            pak.addUint32(info, 1);
            pak.addHexBytes("00000000");
            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }

        // Updates the Inner Strenght Value
        public void sendISCurrent(WorldClient client, UInt16 innerStrengthValue )
        {
            PacketContent pak = new PacketContent();
            pak.addUint16(2, 1);
            pak.addByte(0x02);
            pak.addByteArray(new byte[]{0x80,0x80,0x80});
            pak.addByte(0x10);
            pak.addUint16(innerStrengthValue,1);
            client.messageQueue.addObjectMessage(pak.returnFinalPacket(), false);
            client.flushQueue();

        }

        public void sendPlayerFriendList(WorldClient client)
        {
            ArrayList friends =  Store.dbManager.WorldDbHandler.fetchFriendList(Store.currentClient.playerData.getCharID());

            if (friends.Count > 0)
            {
                foreach (Hashtable friend in friends)
                {
                    PacketContent pak = new PacketContent();
                    pak.addUint16((UInt16)RPCResponseHeaders.SERVER_FRIENDLIST_STATUS, 0);
                    pak.addHexBytes("0800");
                    if ((Int16)friend["online"] ==1)
                    {
                        pak.addByte(0x3c);
                    }
                    else
                    {
                        pak.addByte(0x3b);
                    }

                    pak.addHexBytes("0000");
                    pak.addByte(0x8e); // Another unknown flag ...mxosource made it wrong lol
                    pak.addSizedTerminatedString("SOE+MXO+" + friend["handle"]);
                    client.messageQueue.addRpcMessage(pak.returnFinalPacket());
                }
            }
        }

        public void sendPlayerAnimation(WorldClient client, String hexAnimation)
        {
            // See animations.txt - hex animation is the first Id
            // Some samples:
            /*
             * 7312 1273 = Stand_Abil_ProgLauSelectivePhage2s_A
               7412 1274 = Stand_Abil_ProgLauSelectivePhage4s_A
               7512 1275 = Stand_Abil_ProgLauSelectivePhage6s_A
               7612 1276 = Stand_Abil_ProgLauSelectivePhage8s_A
             */
            Random rand = new Random();
            byte[] Ltvector3d = Store.currentClient.playerInstance.Position.getValue();
            double x = 0; 
            double y = 0; 
            double z = 0;
            NumericalUtils.LtVector3dToDoubles(Ltvector3d, ref x, ref y, ref z);
            float xPos = (float)x;
            float yPos = (float)y;
            float zPos = (float)z;

            PacketContent pak = new PacketContent();
            pak.addUint16(2, 1);
            pak.addByteArray(new byte[]{0x01,0x28});
            pak.addUintShort((ushort)rand.Next(0, 255));
            pak.addByteArray(new byte[] { 0x40, 00 });
            pak.addByte(0x29);
            pak.addHexBytes(hexAnimation);
            pak.addByteArray(new byte[] { 0x00, 0x01 });
            pak.addFloat(xPos,1);
            pak.addFloat(yPos,1);
            pak.addFloat(zPos,1);
            pak.addByteArray(new byte[] {0x20, 0x9f, 0x1e, 0x20});
            pak.addUint16(0, 1);
            client.messageQueue.addObjectMessage(pak.returnFinalPacket(), false);
            client.flushQueue();
        }

        public void sendPlayerAttributes(WorldClient client)
        {
            // ToDo: Load dynamic if we know what to load
            
            //client.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc15002f0000f70300000802000000000000000000"));
            //client.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc15002f0000f70300000802000000000000000000"));
            //client.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc1500300000f70300000702ecffffff0000000000"));
            //client.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc1500310000f70300005004000000000000000000"));
            //client.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc1500320000f7030000f403000000000000000000"));
            //client.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc1500330000f70300005104f6ffffff0000000000"));
            //client.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc56002a0000b3a900000904090000000100000000"));
            //client.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80b23a0400000802"));
            //client.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80b2110011000802"));
            
            //client.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("81A500000700052300687474703A2F2F6D786F656D752E696E666F2F666F72756D2F696E6465782E70687000")); // Has forum url - but is not flash traffic - or ? 
            client.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc55005100000b0000003702330000000000000000")); // this adds super jump points dude
            //client.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("2E070000000000000000002400000000000000000000000000000000000000000000000011005768617427732075702062726F736B6900"));
            createFlashTraffic(client, "http://mxo.hardlinedreams.com");
        }


        
    }
}
