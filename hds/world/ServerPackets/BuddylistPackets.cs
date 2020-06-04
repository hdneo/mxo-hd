using System;
using System.Collections;
using hds.shared;

namespace hds
{
    public partial class ServerPackets
    {

        public void SendAddFriend(WorldClient client, string handleWithPrefix)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_FRIENDLIST_STATUS_ADD,0);
            pak.addUint16(8,1); // Unknonw 08 00 (but should be just reponse)
            pak.addByte(0x3b); // Add to Online
            pak.addByteArray(new byte[]{ 0x00, 0x00, 0x8e});
            pak.addSizedTerminatedString(handleWithPrefix);
            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }
        
        public void SendRemoveFriend(WorldClient client, string handleWithPrefix)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_FRIENDLIST_STATUS_DELETE,0);
            pak.addUint16(8,1); // Unknonw 08 00 (but should be just reponse)
            pak.addByteArray(new byte[]{ 0x84, 0x00, 0x00, 0x8e});
            pak.addSizedTerminatedString(handleWithPrefix);
            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }

        public void SendOnlineStatusFriend(ArrayList friends, string handle, bool isOnline)
        {
            PacketContent pak = new PacketContent();
            if (isOnline)
            {
                pak.addUint16((UInt16)RPCResponseHeaders.SERVER_FRIEND_ONLINE,0);   
                pak.addUint16(5,1);
            }
            else
            {
                pak.addUint16((UInt16)RPCResponseHeaders.SERVER_FRIEND_OFFLINE,0);
                pak.addUint16(4,1);
            }
            pak.addByte(0x00);

            string handleWithPrefix = "SOE+MXO+" + Store.worldConfig.serverName + "+" + handle;
            pak.addSizedTerminatedString(handleWithPrefix);
            Store.world.SendRPCToPlayerList(friends, pak.returnFinalPacket());
        }
    }
}