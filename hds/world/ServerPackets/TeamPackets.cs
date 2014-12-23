using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using hds.shared;
using System.Collections;
namespace hds
{
    public partial class ServerPackets
    {
        // ToDo: if other clients invite this player to a team
        public void sendTeamInvitation(WorldClient client, string inviterCharname, string missionTeamName)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_TEAM_INVITE_MEMBER, 0);
            pak.addHexBytes("0C001C00000000000000"); // Unknown 
            pak.addSizedTerminatedString(inviterCharname);
            pak.addSizedTerminatedString(missionTeamName);
            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }

        public void sendTeamCreation(WorldClient client, string missionTeamName)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_TEAM_CREATE, 0);
            pak.addHexBytes("2A0000020E00040000000000"); // Unknown mostly
            pak.addSizedTerminatedString(missionTeamName);
            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }
    }
}