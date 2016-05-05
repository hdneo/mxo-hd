using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using hds.shared;


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

        public void processTeamInviteAnswer(ref byte[] packet)
        {
            // read the important things
            byte[] unknownUint16_1 = new byte[2];
            byte[] sizeString = new byte[2];
            ArrayUtils.copyTo(packet, 3, unknownUint16_1, 0, 2);
            ArrayUtils.copyTo(packet, 7, sizeString, 0, 2);
            UInt16 sizeCharName = NumericalUtils.ByteArrayToUint16(sizeString, 1);
            byte[] characterNameBytes = new byte[sizeCharName];
            ArrayUtils.copyTo(packet, 9, characterNameBytes, 0, sizeCharName);

            string characterName = StringUtils.charBytesToString(characterNameBytes);

            // if it is 0 - then he has accepted the request - otherwise decline and ..we dont care
            if (NumericalUtils.ByteArrayToUint16(unknownUint16_1,1) == 0)
            {
                lock (WorldSocket.missionTeams)
                {
                    foreach (MissionTeam team in WorldSocket.missionTeams)
                    {
                        if (team.characterMasterName.Equals(characterName))
                        {
                            team.addMember(StringUtils.charBytesToString_NZ(Store.currentClient.playerInstance.CharacterName.getValue()));
                        }
                    }
                }
            }
            

        }
    }
}
