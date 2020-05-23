using System;
using System.Collections;
using hds.shared;

namespace hds
{
    public class BuddylistHandler
    {

        public void ProcessAddFriend(ref byte[] packet)
        {
            PacketReader reader = new PacketReader(packet);
            UInt16 listType = reader.readUInt16(1); // Possible List Type (Online, Ignore etc. need research)
            string playerHandleWithPrefix = reader.readSizedZeroTerminatedString();
            string playerHandleClean =
                playerHandleWithPrefix.Replace("SOE+MXO+" + Store.worldConfig.serverName + "+", "");
            Store.dbManager.WorldDbHandler.AddHandleToFriendList(playerHandleClean,
                Store.currentClient.playerData.getCharID());
            ServerPackets packets = new ServerPackets();
            packets.SendAddFriend(Store.currentClient, playerHandleWithPrefix);
        }

        public void ProcessRemoveFriend(ref byte[] packet)
        {
            PacketReader reader = new PacketReader(packet);
            UInt16 listType = reader.readUInt16(1); // Possible List Type (Online, Ignore etc. need research)
            string playerHandleWithPrefix = reader.readSizedZeroTerminatedString();
            string playerHandleClean =
                playerHandleWithPrefix.Replace("SOE+MXO+" + Store.worldConfig.serverName + "+", "");
            
            Store.dbManager.WorldDbHandler.RemoveHandleFromFriendList(playerHandleClean,
                Store.currentClient.playerData.getCharID());
            ServerPackets packets = new ServerPackets();
            packets.SendRemoveFriend(Store.currentClient, playerHandleWithPrefix);
        }

        public void ProcessAnnounceFriendsOffline(UInt32 charId, string handle)
        {
            ArrayList friends = Store.dbManager.WorldDbHandler.FetchPlayersWhoAddedMeToBuddylist(charId);
            ServerPackets packets = new ServerPackets();
            packets.SendOnlineStatusFriend(friends, handle, false);
        }

        public void ProcessAnnounceFriendOnline(uint charId, string handle)
        {
            ArrayList friends = Store.dbManager.WorldDbHandler.FetchPlayersWhoAddedMeToBuddylist(charId);
            ServerPackets packets = new ServerPackets();
            packets.SendOnlineStatusFriend(friends, handle, true);
        }
    }
}