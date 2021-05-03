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
            pak.AddUint16((UInt16)RPCResponseHeaders.SERVER_FRIENDLIST_STATUS_ADD,0);
            pak.AddUint16(8,1); // Unknonw 08 00 (but should be just reponse)
            pak.AddByte(0x3b); // Add to Online
            pak.AddByteArray(new byte[]{ 0x00, 0x00, 0x8e});
            pak.AddSizedTerminatedString(handleWithPrefix);
            client.messageQueue.addRpcMessage(pak.ReturnFinalPacket());
        }
        
        public void SendRemoveFriend(WorldClient client, string handleWithPrefix)
        {
            PacketContent pak = new PacketContent();
            pak.AddUint16((UInt16)RPCResponseHeaders.SERVER_FRIENDLIST_STATUS_DELETE,0);
            pak.AddUint16(8,1); // Unknonw 08 00 (but should be just reponse)
            pak.AddByteArray(new byte[]{ 0x84, 0x00, 0x00, 0x8e});
            pak.AddSizedTerminatedString(handleWithPrefix);
            client.messageQueue.addRpcMessage(pak.ReturnFinalPacket());
        }

        public void SendOnlineStatusFriend(ArrayList friends, string handle, bool isOnline)
        {
            PacketContent pak = new PacketContent();
            if (isOnline)
            {
                pak.AddUint16((UInt16)RPCResponseHeaders.SERVER_FRIEND_ONLINE,0);   
                pak.AddUint16(5,1);
            }
            else
            {
                pak.AddUint16((UInt16)RPCResponseHeaders.SERVER_FRIEND_OFFLINE,0);
                pak.AddUint16(4,1);
            }
            pak.AddByte(0x00);

            string handleWithPrefix = "SOE+MXO+" + Store.worldConfig.serverName + "+" + handle;
            pak.AddSizedTerminatedString(handleWithPrefix);
            Store.world.SendRPCToPlayerList(friends, pak.ReturnFinalPacket());
        }
    }
}