using System;
using System.Collections;
using hds.shared;

namespace hds
{
    public class BuddylistHandler
    {
        private readonly WorldClient client;

        public BuddylistHandler(WorldClient client)
        {
            this.client = client;
        }


        public void ProcessAddFriend(ref byte[] packet)
        {
            PacketReader reader = new PacketReader(packet);
            UInt16 listType = reader.ReadUInt16(1); // Possible List Type (Online, Ignore etc. need research)
            string playerHandleWithPrefix = reader.ReadSizedZeroTerminatedString();
            string playerHandleClean =
                playerHandleWithPrefix.Replace("SOE+MXO+" + Store.worldConfig.serverName + "+", "");
            Store.dbManager.WorldDbHandler.AddHandleToFriendList(playerHandleClean,
                client.playerData.getCharID());
            ServerPackets packets = new ServerPackets(client);
            packets.SendAddFriend(client, playerHandleWithPrefix);
        }

        public void ProcessRemoveFriend(ref byte[] packet)
        {
            PacketReader reader = new PacketReader(packet);
            UInt16 listType = reader.ReadUInt16(1); // Possible List Type (Online, Ignore etc. need research)
            string playerHandleWithPrefix = reader.ReadSizedZeroTerminatedString();
            string playerHandleClean =
                playerHandleWithPrefix.Replace("SOE+MXO+" + Store.worldConfig.serverName + "+", "");
            
            Store.dbManager.WorldDbHandler.RemoveHandleFromFriendList(playerHandleClean,
                client.playerData.getCharID());
            ServerPackets packets = new ServerPackets(client);
            packets.SendRemoveFriend(client, playerHandleWithPrefix);
        }

        public void ProcessAnnounceFriendsOffline(UInt32 charId, string handle)
        {
            ArrayList friends = Store.dbManager.WorldDbHandler.FetchPlayersWhoAddedMeToBuddylist(charId);
            ServerPackets packets = new ServerPackets(client);
            packets.SendOnlineStatusFriend(friends, handle, false);
        }

        public void ProcessAnnounceFriendOnline(uint charId, string handle)
        {
            ArrayList friends = Store.dbManager.WorldDbHandler.FetchPlayersWhoAddedMeToBuddylist(charId);
            ServerPackets packets = new ServerPackets(client);
            packets.SendOnlineStatusFriend(friends, handle, true);
        }
    }
}