using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using hds.shared;
using hds.world.Structures;


namespace hds
{
    public class TeamHandler
    {
        // Checks and create a mission Team if you are not on a mission
        public void checkAndCreateMissionTeam(WorldClient client)
        {
            if (client.playerData.getMissionTeam() == null)
            {
                string missionTeamName = StringUtils.charBytesToString_NZ(client.playerInstance.CharacterName.getValue()) + "'s Mission Team";
                ServerPackets packets = new ServerPackets();
                packets.sendTeamCreation(client, missionTeamName);

                // Add the Team to our global Team List
                MissionTeam team = new MissionTeam(missionTeamName, StringUtils.charBytesToString_NZ(client.playerInstance.CharacterName.getValue()));
                lock (WorldSocket.missionTeams)
                {
                    WorldSocket.missionTeams.Add(team);
                }
            }
        }

        // This is for team and crew invites
        public void processInviteAnswer(ref byte[] packet)
        {
            // read the important things
            PacketReader reader = new PacketReader(packet);
            uint type = reader.readUint8();
            UInt16 offsetInviterHandle = reader.readUInt16(1);
            UInt32 unknownUint32 = reader.readUInt32(1);
            
            reader.setOffsetOverrideValue(offsetInviterHandle-1);
            string inviterCharacterName = reader.readSizedZeroTerminatedString();

            // if it is 0 - then he has accepted the request - otherwise decline and ..we dont care

            ServerPackets packets = new ServerPackets();
            switch (type)
            {
                    // Team Invites
                    // ToDo: shouldnt it be 3 ? As Faction 1, Crew 2, Mission 3
                    case 0:
                        lock (WorldSocket.missionTeams)
                        {
                            foreach (MissionTeam team in WorldSocket.missionTeams)
                            {
                                if (team.characterMasterName.Equals(inviterCharacterName))
                                {
                                    team.addMember(StringUtils.charBytesToString_NZ(Store.currentClient.playerInstance.CharacterName.getValue()));
                                }
                            }
                        }
                        break;
                     
                    case 1:
                        // This is factionInvite - but we first need to test this what data is send from client
                        break;
                    // Crew Invites
                    case 2:
                        // ToDo: Send CrewMember "added" message to all crew members (i think faction members is not necessary)
                        // ToDo: Send Faction Id and CrewId (if necessary) View Update to all connected Faction Members
                        UInt32 crewId = Store.dbManager.WorldDbHandler.GetCrewIdByInviterHandle(inviterCharacterName);
                        Crew crewData = Store.dbManager.WorldDbHandler.GetCrewData(crewId);
                        UInt32 characterId =
                            NumericalUtils.ByteArrayToUint32(Store.currentClient.playerInstance.CharacterID.getValue(),
                                1);
                        if (crewId > 0)
                        {
                            Store.dbManager.WorldDbHandler.AddMemberToCrew(characterId, crewId, crewData.factionId,0,0 );
                            Store.currentClient.playerInstance.CrewID.setValue(crewId);
                            new FCHandler().ProcessCrewInfoUpdate();
                        }

                        if (crewData.factionId > 0)
                        {
                            Store.currentClient.playerInstance.FactionID.setValue(crewData.factionId);
                            new FCHandler().ProcessFactionInfoUpdate(false);
                        }

                        string characterName =
                            StringUtils.charBytesToString_NZ(
                                Store.currentClient.playerInstance.CharacterName.getValue());
                        packets.SendJoinedGroup(type, characterId, crewId, characterName);
                        break;
            }
        }
    }
}
