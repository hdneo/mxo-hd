using System;
using System.Collections;
using System.Linq;
using System.Text;
using hds.databases;
using hds.shared;

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
            // Packet 3080D708003C00008E2600534F452B4D584F2B566563746F722D486F7374696C652B52756E6E696E6757696C6433303500
            // 2A80D708003C00008E2000534F452B4D584F2B566563746F722D486F7374696C652B656E74696C73617200 <- Entilsar (has charId 2f d8 01 in Margin)
            /*
            PacketContent pakTest = new PacketContent();
            pakTest.addUint16((UInt16)RPCResponseHeaders.SERVER_FRIENDLIST_STATUS, 0);
            pakTest.addHexBytes("0800");
            pakTest.addByte(0x3b); // Online Flag: 0x3c is offline, 0x3b is online
            pakTest.addHexBytes("0000");
            pakTest.addByte(0x8e); // Another unknown flag ...mxosource made it wrong lol
            pakTest.addSizedTerminatedString("SOE+MXO+MyFriend");

            client.messageQueue.addRpcMessage(pakTest.returnFinalPacket());
            client.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80D708003C00008a2600534F452B4D584F2B566563746F722D486F7374696C652B52756E6E696E6757696C6433303500"));
             */
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
