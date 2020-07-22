using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using hds.shared;

namespace hds
{
    public class MissionHandler
    {
        public void processMissionaccept(ref byte[] packet)
        {
            byte[] contactBytes = { packet[0], packet[1] };
            UInt16 contactId = NumericalUtils.ByteArrayToUint16(contactBytes, 1);
            ushort missionId = packet[2];

            #if DEBUG
            Output.WriteRpcLog("Mission Accept Data:" + StringUtils.bytesToString(packet));
            #endif
            ServerPackets pak = new ServerPackets();

            pak.sendMissionAccept(Store.currentClient, contactId, missionId);
            pak.sendSetMissionObjective(1, 1, "This is the test Mission, mate", Store.currentClient);
            pak.sendSetMissionObjective(2, 0, "Success", Store.currentClient);
            pak.sendSetMissionObjective(3, 1, "Talk to Morpheus", Store.currentClient);
            pak.sendSetMissionObjective(4, 1, "Talk to Niobe", Store.currentClient);
            pak.sendSetMissionObjective(5, 2, "Failed Remain", Store.currentClient);
            pak.sendSetMissionObjective(6, 0, "Failed Clear", Store.currentClient);
        }

        public void processMissionList(ref byte[] packet)
        {
            new TeamHandler().checkAndCreateMissionTeam(Store.currentClient);
            byte[] contactBytes = { packet[0], packet[1] };
            UInt16 contactId = NumericalUtils.ByteArrayToUint16(contactBytes,1);
            uint orgID = packet[2];
            ServerPackets pak = new ServerPackets();
            pak.sendMissionList(contactId, orgID, Store.currentClient);
        }

        public void processInvitePlayerToMissionTeam(ref byte[] packet)
        {
            // Read the Data
            PacketReader reader = new PacketReader(packet);
            UInt32 unknownUint = reader.readUInt32(1); // Maybe its just an offset of uint8 - we dont care and know lol
            reader.incrementOffsetByValue(1);
            String handleToInvite = reader.readSizedZeroTerminatedString();
            // ToDo: implement the right response for the player who get the invite
        }

        public void processLoadMissionInfo(ref byte[] packet)
        {
            ServerPackets pak = new ServerPackets();
            pak.sendMissionInfo(Store.currentClient);
        }

        public void processAbortMission(ref byte[] packet)
        {
            ServerPackets pak = new ServerPackets();
            pak.sendMissionAbort(Store.currentClient);
        }
    }
}
