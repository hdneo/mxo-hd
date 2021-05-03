using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using hds.shared;

namespace hds
{
    public class MissionHandler
    {
        public void ProcessMissionaccept(ref byte[] packet)
        {
            // ToDo: figure out if time and diffulty is in the packet
            byte[] contactBytes = { packet[0], packet[1] };
            UInt16 contactId = NumericalUtils.ByteArrayToUint16(contactBytes, 1);
            ushort missionId = packet[2];

            #if DEBUG
            Output.WriteRpcLog("Mission Accept Data:" + StringUtils.bytesToString(packet));
            #endif
            ServerPackets pak = new ServerPackets();

            new TeamHandler().checkAndCreateMissionTeam(Store.currentClient);
            
            pak.sendMissionAccept(Store.currentClient, contactId, missionId);
            pak.SendSetMissionObjective(1, 1, "This is the test Mission, mate", Store.currentClient);
            pak.SendSetMissionObjective(2, 0, "Success", Store.currentClient);
            pak.SendSetMissionObjective(3, 1, "Talk to Morpheus", Store.currentClient);
            pak.SendSetMissionObjective(4, 1, "Talk to Niobe", Store.currentClient);
            pak.SendSetMissionObjective(5, 2, "Failed Remain", Store.currentClient);
            pak.SendSetMissionObjective(6, 0, "Failed Clear", Store.currentClient);
        }

        public void processMissionList(ref byte[] packet)
        {
            
            new TeamHandler().checkAndCreateMissionTeam(Store.currentClient);
            PacketReader reader = new PacketReader(packet);
            
            UInt16 contactId = reader.ReadUInt16(1);
            uint orgID = reader.ReadUint8();
            // ToDo: Load the possible missions from the given contactId

            /*
            byte[] contactBytes = { packet[0], packet[1] };
            UInt16 contactId = NumericalUtils.ByteArrayToUint16(contactBytes,1);
            uint orgID = packet[2];
            */
            ServerPackets pak = new ServerPackets();
            
            Store.currentClient.playerData.currentMissionList = DataLoader.getInstance().FindMissions(contactId, orgID); 
            pak.sendMissionList(contactId, orgID, Store.currentClient.playerData.currentMissionList, Store.currentClient);
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

            Mission theMission = Store.currentClient.playerData.currentMissionList[missionIndexFromList];
            
            uint missionTime = reader.ReadUint8();
            uint difficulty = reader.ReadUint8();
            
            uint unknownLastByte = reader.ReadUint8();
            
            ServerPackets pak = new ServerPackets();
            pak.sendMissionInfo(theMission, Store.currentClient);
        }

        public void processAbortMission(ref byte[] packet)
        {
            ServerPackets pak = new ServerPackets();
            pak.SendMissionAbort(Store.currentClient);
        }
    }
}
