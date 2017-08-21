using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using hds.shared;

namespace hds
{
    class FCHandler
    {
        public void processLoadFactionName(ref byte[] packet)
        {

            PacketReader packetReader = new PacketReader(packet);
            UInt32 factionId = packetReader.readUInt32(1);

            string factionName = Store.dbManager.WorldDbHandler.getFactionNameById(factionId);
            
            ServerPackets pak = new ServerPackets();
            pak.sendFactionName(Store.currentClient, factionId, factionName);
        }


        public void processInvitePlayerToCrew(ref byte[] rpcData)
        {
            // Request Packet: 80 84 19 00 07 00 02 10 00 54 72 69 6e 69 74 79 73 27 73 20 43 72 65 77 00 07 00 54 68 65 4e 65 6f 00
            // Request Packet: 80 84 22 00 07 00 03 19 00 54 68 69 73 20 69 73 20 74 68 65 20 41 77 65 73 6f 6d 65 20 43 72 65 77 00 09 00 54 72 69 6e 69 74 79 73 00

            // Response

            // ToDO: add it to the tempCrews and check if name is reserved on DB (and check if double names are possible)
            // ToDo: Questions 1. should we persist it directly to reserve crewname in the DB ? maybe its better
            PacketReader pakRead = new PacketReader(rpcData);

            UInt16 someUint16 = pakRead.readUInt16(1);
            UInt16 someUint162 = pakRead.readUInt16(1);
            uint orgId = pakRead.readUint8();
            string crewName = pakRead.readSizedZeroTerminatedString().Trim();
            string playerHandle = pakRead.readSizedZeroTerminatedString().Trim();

            bool isCrewNameAvailable = Store.dbManager.WorldDbHandler.isCrewNameAvailable(crewName);

            // ToDo: Just "reserve" the crewName - so if crewName exists and membercount is just one or zero and its older than a day - delete it (this can be done by the "isCrewNameAvailable" too).
            ServerPackets pak = new ServerPackets();
            if (isCrewNameAvailable)
            {
                Store.dbManager.WorldDbHandler.addCrew(crewName,StringUtils.charBytesToString_NZ(Store.currentClient.playerInstance.CharacterName.getValue()));
                pak.sendCrewInviteToPlayer(playerHandle, crewName);
            }
            else
            {
                pak.sendSystemChatMessage(Store.currentClient,"Crewname was already taken - please choose a new one","BROADCAST");
            }

        }
    }
}
