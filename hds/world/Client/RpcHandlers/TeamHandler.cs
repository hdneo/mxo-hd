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

        // This is for team and crew invites
        public void processInviteAnswer(ref byte[] packet)
        {
            // read the important things
            byte[] maybeType = new byte[2]; 
            byte[] sizeString = new byte[2];
            ArrayUtils.copyTo(packet, 3, maybeType, 0, 2);
            ArrayUtils.copyTo(packet, 7, sizeString, 0, 2);
            UInt16 sizeCharName = NumericalUtils.ByteArrayToUint16(sizeString, 1);
            byte[] characterNameBytes = new byte[sizeCharName];
            ArrayUtils.copyTo(packet, 9, characterNameBytes, 0, sizeCharName);

            string characterName = StringUtils.charBytesToString(characterNameBytes);

            // if it is 0 - then he has accepted the request - otherwise decline and ..we dont care

            switch (NumericalUtils.ByteArrayToUint16(maybeType,1))
            {
                    // Team Invites
                    case 0:
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
                        break;
                     
                    // Crew Invites
                    case 2:
                        // ToDo: add to Crew and maybe to faction (if crew is part of faction)
                        // ToDo: Generate Repsonse for all connected crew mates and the new member
                        // ToDo: add to crew and figure out the responses that are necessary (like crew message , player update etc.) 
                        // ToDo: for this the "2_player_action" logs could be useful.
                        
                        break;
                        
                     
            
                    
            }
        }
    }
}
