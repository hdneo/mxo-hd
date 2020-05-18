using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using hds.shared;
using hds.world.Structures;

namespace hds
{
    public partial class ServerPackets
    {
        public void sendFactionName(WorldClient client, UInt32 factionID, string factionName)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16) RPCResponseHeaders.SERVER_FACTION_NAME_RESPONSE, 0);
            pak.addUint32(factionID, 1);
            // Add 42 Bytes long faction name 
            pak.addStringWithFixedSized(factionName, 42);
            Store.currentClient.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }

        public void SendCrewMembers(WorldClient client, Crew crew, List<CrewMember> members)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16) RPCResponseHeaders.SERVER_CREW_MEMBERS_LIST, 0);
            pak.addUint32(client.playerData.getCharID(), 1);
            pak.addUint32(crew.crewId, 1);
            if (crew.crewId == 0)
            {
                // Finalize the packet - we have now crew data
                pak.addUintShort(0);
                pak.addUint32(0,1);
                pak.addHexBytes("0000000000000000000000210000000000230000000000");
            }
            else
            {
                pak.addUintShort(crew.org);
                pak.addUint16(33, 1); // CrewName Offset

                UInt32 charIdCaptain = 0;
                UInt32 charIdFM = 0;
                foreach (CrewMember member in members)
                {
                    if (member.isCaptain)
                    {
                        charIdCaptain = member.charId;
                    }

                    if (member.isFirstMate)
                    {
                        charIdFM = member.charId;
                    }
                }

                pak.addUint32(charIdCaptain, 1);
                pak.addUint32(charIdFM, 1);
                pak.addUint32(crew.money, 1);

                UInt16 offsetMemberList =
                    (ushort) (33 + crew.crewName.Length +
                              3); // baseoffset + full crewname size (inkl. size byte and 0 termination)
                pak.addUint16(offsetMemberList, 1);
                pak.addHexBytes("14020000"); // Constant in every log from medanon, sonyblack and afterwhoruneo
                UInt16 calculatedFullSize = (UInt16) (members.Count * 38 + offsetMemberList - 2);
                // pak.addUint16(calculatedFullSize,1);
                
                
                PacketContent memberData = new PacketContent();
                memberData.addSizedTerminatedString(crew.crewName);
                memberData.addUint16((ushort) members.Count,1);
                foreach (CrewMember member in members)
                {
                    memberData.addByte(0x00);
                    memberData.addUint32(member.charId,1);
                    memberData.addStringWithFixedSized(member.handle,32);
                    memberData.addUintShort(member.isOnline);
                }
                
                memberData.addByteArray(new byte[]{0x00, 0x00, 0x00});
                
                UInt16 finalFullSize = (UInt16) (pak.returnFinalPacket().Length + memberData.returnFinalPacket().Length);
                pak.addUint16(calculatedFullSize, 1);
                pak.addByteArray(memberData.returnFinalPacket());

            }

            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }

        public void SendFactionInfo(WorldClient client, Faction faction)
        {
            PacketContent pak = new PacketContent();
            pak.addUintShort((ushort) RPCResponseHeaders.SERVER_FACTION_PLAYER_INFO);
            pak.addUint32(Store.currentClient.playerData.getCharID(), 1);
            pak.addUint32(faction.factionId, 1);
            pak.addHexBytes("010000010F00"); // Currently unknown
            pak.addUint16(52, 1); // 52 size for the "next" part but its constant at this time (as string has no size)
            pak.addUintShort(
                1); // ToDo: Should be Alignment : 1=Zion, 3 Mero, 2 Machine? Logs has only 1 and 3 (needs more testing and setup)
            pak.addStringWithFixedSized(faction.name,
                42); // 32 is more realistic - but after that there is much "dummy" data which differs

            pak.addUint32(faction.masterPlayerCharId, 1);
            pak.addUint32(faction.money, 1);
            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }

        public void SendFactionCrews(WorldClient client, Faction faction)
        {
            PacketContent pak = new PacketContent();
            pak.addUintShort((ushort) RPCResponseHeaders.SERVER_FACTION_PLAYER_INFO);
            pak.addUint32(Store.currentClient.playerData.getCharID(), 1);
            pak.addUint32(faction.factionId, 1);
            pak.addHexBytes("020001020F00"); // Currently unknown but the 02 should tell "hey this are the crew data"
                                    //020000020F00 
            // Add the size for the next Data until end (we know that 81 bytes for each crewData so we you just multiply it)
            pak.addUint16((ushort) (faction.crews.Count * 81), 1);
            foreach (Crew theCrew in faction.crews)
            {
                pak.addUint32(theCrew.crewId, 1);
                pak.addStringWithFixedSized(theCrew.crewName, 38);
                //pak.addHexBytes("0000000000E6DC");
                pak.addUint32(theCrew.masterPlayerCharId, 1);
                pak.addStringWithFixedSized(theCrew.characterMasterName, 31);
                pak.addByte(0x00); // ToDo: Should be an online flag (if leader is online ? )
                pak.addUintShort(theCrew.factionRank); // This is rank
                
            }

            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }

        public void sendCrewInviteToPlayer(string playerHandle, string crewName)
        {
            // ToDo: fix the name display issue ?
            string charname =
                StringUtils.charBytesToString_NZ(Store.currentClient.playerInstance.CharacterName.getValue());
            UInt16 crewOffset = (UInt16) (charname.Length + 7 + 3);
            PacketContent pak = new PacketContent();
            pak.addUintShort((ushort) RPCResponseHeaders.SERVER_CREW_INVITE);
            pak.addUint16(7, 1); // Start Offsset for Charactername
            pak.addUint16(crewOffset, 1);
            pak.addByte(0x01);
            pak.addSizedTerminatedString(
                StringUtils.charBytesToString_NZ(Store.currentClient.playerInstance.CharacterName.getValue()));
            pak.addSizedTerminatedString(crewName);
            Store.world.sendRPCToOnePlayerByHandle(pak.returnFinalPacket(), playerHandle);
        }

        public void SendPlayersFactionInfo()
        {
        }
    }
}