using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using hds.shared;

namespace hds
{
    public class MissionHandler
    {
        private readonly WorldClient client;

        public MissionHandler(WorldClient client)
        {
            this.client = client;
        }

        public void ProcessMissionaccept(ref byte[] packet)
        {
            // ToDo: figure out if time and diffulty is in the packet
            byte[] contactBytes = { packet[0], packet[1] };
            UInt16 contactId = NumericalUtils.ByteArrayToUint16(contactBytes, 1);
            ushort missionId = packet[2];

            #if DEBUG
            Output.WriteRpcLog("Mission Accept Data:" + StringUtils.bytesToString(packet));
            #endif
            ServerPackets pak = new ServerPackets(client);

            new TeamHandler(client).checkAndCreateMissionTeam(client);
            
            pak.sendMissionAccept(client, contactId, missionId);
            
            pak.SendSetMissionObjective(1, 0, "Follow the white Rabbit!", client);
            pak.SendSetMissionObjective(2, 0, "Wake UP!", client);
            pak.SendSetMissionObjective(3, 0, "Watch Matrix Resurrections", client);
            client.FlushQueue();
            
            // pak.SendSetMissionObjective(2, 0, "Success", client);
            // pak.SendSetMissionObjective(3, 1, "Talk to Morpheus", client);
            // pak.SendSetMissionObjective(4, 1, "Talk to Niobe", client);
            // pak.SendSetMissionObjective(5, 2, "Failed Remain", client);
            // pak.SendSetMissionObjective(6, 0, "Failed Clear", client);
        }

        public void processMissionList(ref byte[] packet)
        {
            
            new TeamHandler(client).checkAndCreateMissionTeam(client);
            PacketReader reader = new PacketReader(packet);
            
            UInt16 contactId = reader.ReadUInt16(1);
            uint orgID = reader.ReadUint8();
            // ToDo: Load the possible missions from the given contactId

            /*
            byte[] contactBytes = { packet[0], packet[1] };
            UInt16 contactId = NumericalUtils.ByteArrayToUint16(contactBytes,1);
            uint orgID = packet[2];
            */
            ServerPackets pak = new ServerPackets(client);
            
            client.playerData.currentMissionList = DataLoader.getInstance().FindMissions(contactId, orgID); 
            pak.sendMissionList(contactId, orgID, client.playerData.currentMissionList, client);
        }

        public void processInvitePlayerToMissionTeam(ref byte[] packet)
        {
            // Read the Data
            PacketReader reader = new PacketReader(packet);
            UInt32 unknownUint = reader.ReadUInt32(1); // Maybe its just an offset of uint8 - we dont care and know lol
            reader.IncrementOffsetByValue(1);
            String handleToInvite = reader.ReadSizedZeroTerminatedString();
            // ToDo: implement the right response for the player who get the invite
        }

        public void processLoadMissionInfo(ref byte[] packet)
        {
            PacketReader reader = new PacketReader(packet);
            UInt16 incrementMissionRequest = reader.ReadUInt16(1);
            // ToDo: we need to capture the mission index from list (which means we need to store the list in the session somewhere?)
            UInt16 missionIndexFromList = reader.ReadUInt16(1);

            Mission theMission = client.playerData.currentMissionList[missionIndexFromList];
            
            uint missionTime = reader.ReadUint8();
            uint difficulty = reader.ReadUint8();
            
            uint unknownLastByte = reader.ReadUint8();
            
            ServerPackets pak = new ServerPackets(client);
            pak.sendMissionInfo(theMission, client);
        }

        public void processAbortMission(ref byte[] packet)
        {
            ServerPackets pak = new ServerPackets(client);
            pak.SendMissionAbort(client);
        }
    }
}
