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
        public void SendFactionName(WorldClient client, UInt32 factionID, string factionName)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16) RPCResponseHeaders.SERVER_FACTION_NAME_RESPONSE, 0);
            pak.addUint32(factionID, 1);
            // Add 42 Bytes long faction name 
            pak.addStringWithFixedSized(factionName, 42);
            Store.currentClient.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }

        public void SendCrewInfo(WorldClient client, Crew crew, List<CrewMember> members)
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
                // pak.addUint16(calculatedFullSize,1);
                
                
                PacketContent memberData = new PacketContent();
                memberData.addSizedTerminatedString(crew.crewName);
                memberData.addUint16((ushort) members.Count,1);
                foreach (CrewMember member in members)
                {
                    memberData.addByte(0x00);
                    memberData.addUint32(member.charId,1);
                    memberData.addStringWithFixedSized(member.handle,31);
                    memberData.addUintShort(member.isOnline);
                }
                
                memberData.addByteArray(new byte[]{0x00, 0x00, 0x00});
                
                UInt16 finalFullSize = (UInt16) (pak.returnFinalPacket().Length + memberData.returnFinalPacket().Length);
                pak.addUint16(finalFullSize, 1);
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
                pak.addUintShort(theCrew.masterIsOnline); // ToDo: Should be an online flag (if leader is online ? )
                pak.addUintShort(theCrew.factionRank); // This is rank
                
            }

            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }

        public void SendMoneyUpdateFactionCrew(WorldClient client, ushort type, UInt32 moneyAmount, ushort IsMoneyGiving)
        {
            // We found only ONE Example of this reponse in the 020_mxoemu_2_persons_actions_with_afterwhoruneo
            PacketContent pak = new PacketContent();
            pak.addUintShort((ushort) RPCResponseHeaders.SERVER_FACTION_UPDATE_MONEY);
            pak.addUintShort(type);
            pak.addUint32(moneyAmount,1);
            pak.addUint32(Store.currentClient.playerData.getCharID(),1);
            pak.addUintShort(IsMoneyGiving);
            
            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }

        public void SendCrewInviteToPlayer(string playerHandle, string crewName)
        {
            // ToDo: fix the name display issue ?
            string charname =
                StringUtils.charBytesToString_NZ(Store.currentClient.playerInstance.CharacterName.getValue());
            UInt16 crewOffset = (UInt16) (charname.Length + 7 + 3);
            PacketContent pak = new PacketContent();
            pak.addUintShort((ushort) RPCResponseHeaders.SERVER_CREW_INVITE);
            pak.addUint16(7, 1); // Start Offset for Charactername
            pak.addUint16(crewOffset, 1);
            pak.addByte(0x01);
            pak.addSizedTerminatedString(
                StringUtils.charBytesToString_NZ(Store.currentClient.playerInstance.CharacterName.getValue()));
            pak.addSizedTerminatedString(crewName);
            Store.world.SendRPCToOnePlayerByHandle(pak.returnFinalPacket(), playerHandle);
        }

        public void SendLeaveGroup(uint type, UInt32 charId, UInt32 groupId)
        {
            PacketContent pak = new PacketContent();
            pak.addUintShort((ushort) RPCResponseHeaders.SERVER_LEAVE_GROUP);
            pak.addUintShort((ushort) type);
            pak.addUint32(charId,1);

            PacketContent viewResetStateData = new PacketContent();
            switch (type)
            {
                case 1:
                    // This removes the faction flag (but as it is from the crew packet it may set faction and crew to zero)
                    // This is just a simple ViewStateUpdate on GRoup 5 (4 times 80 skipped) and set CrewId to 0
                    viewResetStateData.addHexBytes("0280804000000000");
                    Store.world.sendRPCToFactionMembers(groupId, Store.currentClient, pak.returnFinalPacket(), false);
                    break;
                
                case 2:
                    // This removes the faction flag (but as it is from the crew packet it may set faction and crew to zero)
                    // This is just a simple ViewStateUpdate on GRoup 5 (4 times 80 skipped) and set CrewId to 0
                    viewResetStateData.addHexBytes("02808080808200000000");
                    Store.world.SendRPCToCrewMembers(groupId, Store.currentClient, pak.returnFinalPacket(), false);
                    break;
                    
                case 3:
                    // ToDo: we MAYBE not send it to ourself (needs testing)
                    viewResetStateData.addHexBytes("024000000000");
                    Store.world.sendRPCToMissionTeamMembers(groupId, Store.currentClient, pak.returnFinalPacket(), false);
                    break;
            }
            
            //viewResetId.addUint16(2,1); // ViewID
            
            Store.world.sendViewPacketToAllPlayers(viewResetStateData.returnFinalPacket(), Store.currentClient.playerData.getCharID(), NumericalUtils.ByteArrayToUint16(Store.currentClient.playerInstance.GetGoid(), 1), Store.currentClient.playerData.getEntityId());

            
            PacketContent myselfStateData = new PacketContent();
            myselfStateData.addUint16(2,1);
            myselfStateData.addByteArray(viewResetStateData.returnFinalPacket());
            
            Store.currentClient.messageQueue.addObjectMessage(myselfStateData.returnFinalPacket(),false);
            
        }
    }
}