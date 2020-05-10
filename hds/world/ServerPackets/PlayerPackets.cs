﻿using System;
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
            client.FlushQueue();

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
                client.FlushQueue();
            }
            

        }

        public void sendMoodChange(WorldClient client, byte moodByte)
        {
            // ToDo: should be more dynamic ? How to announce to another players ?
            byte[] moodPak = { 0x02, 0x00, 0x01, 0x01, 0x00, moodByte, 0x00, 0x00 };
            PacketContent pak = new PacketContent();
            pak.addUint16(2, 1);
            pak.addByte(0x02);
            pak.addByteArray(moodPak);
            client.messageQueue.addObjectMessage(pak.returnFinalPacket(), false);
            client.FlushQueue();
        }

        public void sendAppeareanceUpdate(WorldClient client, byte[] rsibytes)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16(2,1);
            pak.addByteArray(new byte[] {  0x02, 0x80, 0x89 });
            pak.addByteArray(rsibytes);

            client.messageQueue.addObjectMessage(pak.returnFinalPacket(), false);
            client.FlushQueue();
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
            
            PacketContent pakAnimation = new PacketContent();
            pakAnimation.addUint16((UInt16)RPCResponseHeaders.SERVER_PLAYER_EXP_ANIM, 0);
            pakAnimation.addUint32(exp, 1);
            pakAnimation.addByte(0x01); // Gain Type
            pakAnimation.addHexBytes("000000");
            client.messageQueue.addRpcMessage(pakAnimation.returnFinalPacket());
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
            client.FlushQueue();

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

        public void sendPlayerMoveAnim(WorldClient client, byte animationId)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16(2, 1);
            pak.addByte(0x02);
            pak.addByteArray(new byte[] { 0x01, 0x02 });
            pak.addByte(animationId);
            client.messageQueue.addObjectMessage(pak.returnFinalPacket(), false);
            client.FlushQueue();
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
            client.FlushQueue();
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

        public void SendPlayerSpawn(WorldClient receiverClient, WorldClient otherClient, UInt16 viewId)
        {
            PacketContent pak = new PacketContent();
            byte[] spawnPaket = new BootingHelperRsi().generatePlayerSpawnPacket(otherClient, receiverClient.playerData.assignSpawnIdCounter());
            pak.addByteArray(spawnPaket);
            pak.addUint16(viewId,1);
            pak.addByte(0x00);
            receiverClient.messageQueue.addObjectMessage(pak.returnFinalPacket(),false);
            receiverClient.FlushQueue();
        }

        public void sendSaveCharDataMessage(WorldClient client, string handle)
        {

            PacketContent pak = new PacketContent();
            pak.addByte((byte)RPCResponseHeaders.SERVER_CHAT_MESSAGE_RESPONSE);
            pak.addHexBytes("0700000000000000000000006400002E00240000000000000000000000000000000000");
            pak.addSizedTerminatedString(handle);
            client.messageQueue.addRpcMessage(pak.returnFinalPacket());

        }

        public void sendGetBackgroundMessage(WorldClient client)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_PLAYER_GET_BACKGROUND,0);
            pak.addByte(0x05);
            pak.addUint16(0,1);

            // Get Data from DB and save
            Hashtable characterData = Store.dbManager.WorldDbHandler.getCharInfo(client.playerData.getCharID());
            String backgroundTextt = characterData["background"].ToString();
            pak.addSizedTerminatedString(backgroundTextt);
            client.messageQueue.addRpcMessage(pak.returnFinalPacket());

        }

        public void SendPlayerGetDetails(WorldClient client, Hashtable charData)
        {
            // ToDo: Research real packet and replace the content with it 
            UInt16 handleOffset = 29;
            UInt16 firstNameOffset = (ushort) (handleOffset + (charData["handle"].ToString().Length + 3));
            UInt16 lastNameOffset = (ushort) (firstNameOffset + (charData["firstname"].ToString().Length + 3));
            
            UInt16 crewNameOffset = 0;
            if (charData["crew_name"] != null)
            {
                crewNameOffset = (ushort) (lastNameOffset + (charData["lastname"].ToString().Length + 3));
            }
            
            UInt16 factionNameOffset = 0;
            if (charData["faction_name"] != null)
            {
                if (crewNameOffset > 0)
                {
                    factionNameOffset = (ushort) (crewNameOffset + (charData["faction_name"].ToString().Length + 3));    
                }
                else
                {
                    factionNameOffset = (ushort) (lastNameOffset + (charData["lastname"].ToString().Length + 3));
                }
            }

            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16) RPCResponseHeaders.SERVER_PLAYER_GET_DETAILS, 0);
            pak.addUint32((UInt32)charData["charId"],1);
            pak.addUint16(handleOffset,1); // Offset to Handle which should ALWAYS be 29
            pak.addUint32(0,1); // Unknown UInt32 Zero
            pak.addUint16(firstNameOffset,1 );
            pak.addUint16(lastNameOffset,1 );
            pak.addUint32(300,1); // Character Trait ?
            pak.addUintShort((ushort)charData["alignment"]);
            pak.addUint16(crewNameOffset,1);
            pak.addUint16(factionNameOffset,1);
            pak.addUint32((UInt32)charData["conquest_points"],1);

            pak.addSizedTerminatedString(charData["handle"].ToString());
            pak.addSizedTerminatedString(charData["firstname"].ToString());
            pak.addSizedTerminatedString(charData["lastname"].ToString());
            if (crewNameOffset > 0)
            {
                pak.addSizedTerminatedString(charData["crew_name"].ToString());
            }
            
            if (factionNameOffset > 0)
            {
                pak.addSizedTerminatedString(charData["faction_name"].ToString());
            }
            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }

        public void SendPlayerBackground(WorldClient client, Hashtable charData)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16) RPCResponseHeaders.SERVER_PLAYER_HANDLE_BACKGROUND,0 );
            pak.addUint16(5,1);
            pak.addUintShort(1);
            if (charData["background"].ToString().Length > 0)
            {
                pak.addSizedTerminatedString(charData["background"].ToString());
            }
            else
            {
                pak.addUint16(1,0);
                pak.addUintShort(0);
            }
            
            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }

        // ToDo: Move it to player Packets and make a ?moa command for it
        public void sendChangeChangeMoaRSI(WorldClient client, byte[] rsi)
        {
            // ToDo: proove to remove
            PacketContent pak = new PacketContent();
            pak.addUint16(2,1);
            pak.addHexBytes("0281008080808004");
            pak.addByteArray(rsi);
            pak.addByte(0x41);
            pak.addByte(0x00);
            client.messageQueue.addObjectMessage(pak.returnFinalPacket(),false);

        }

        public void sendJackoutEffect(WorldClient client)
        {
            PacketContent pak = new PacketContent();
            double x = 0; double y = 0; double z = 0;
            byte[] Ltvector3d = client.playerInstance.Position.getValue();
            NumericalUtils.LtVector3dToDoubles(Ltvector3d, ref x, ref y, ref z);
            int rotation = (int)client.playerInstance.YawInterval.getValue()[0];
            float xPos = (float)x;
            float yPos = (float)y;
            float zPos = (float)z;
            
            pak.addUint16(2,1); // ToDo: we should change this for other views ?
            pak.addHexBytes("032802C000740010");
            pak.addFloatLtVector3f(xPos,yPos,zPos);
            pak.addHexBytes("E93C991E8080808080801001000000"); // ToDo: analyze it more ?
            client.playerData.isJackoutInProgress = true;
            client.messageQueue.addObjectMessage(pak.returnFinalPacket(),false);
        }
        

        public void sendExitGame(WorldClient client)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_JACKOUT_FINISH,0);
            pak.addHexBytes("000000000000");
            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }



    }
}
