using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using hds.shared;
using hds.world.Structures;

namespace hds
{
    class FCHandler
    {
        public void processLoadFactionName(ref byte[] packet)
        {

            PacketReader packetReader = new PacketReader(packet);
            UInt32 factionId = packetReader.readUInt32(1);

            string factionName = Store.dbManager.WorldDbHandler.GetFactionNameById(factionId);
            
            ServerPackets pak = new ServerPackets();
            pak.SendFactionName(Store.currentClient, factionId, factionName);
        }


        public void processInvitePlayerToCrew(ref byte[] rpcData)
        {
            // Request Packet: 80 84 19 00 07 00 02 10 00 54 72 69 6e 69 74 79 73 27 73 20 43 72 65 77 00 07 00 54 68 65 4e 65 6f 00
            // Request Packet: 80 84 22 00 07 00 03 19 00 54 68 69 73 20 69 73 20 74 68 65 20 41 77 65 73 6f 6d 65 20 43 72 65 77 00 09 00 54 72 69 6e 69 74 79 73 00

            // Response

            // ToDO: add it to the tempCrews and check if name is reserved on DB (and check if double names are possible)
            // ToDo: Questions 1. should we persist it directly to reserve crewname in the DB ? maybe its better
            PacketReader pakRead = new PacketReader(rpcData);

            UInt16 offsetHandle = pakRead.readUInt16(1);
            UInt16 offsetCrewName = pakRead.readUInt16(1);
            uint orgId = pakRead.readUint8();
            string crewName = pakRead.readSizedZeroTerminatedString().Trim();
            string playerHandle = pakRead.readSizedZeroTerminatedString().Trim();

            bool isCrewNameAvailable = Store.dbManager.WorldDbHandler.IsCrewNameAvailable(crewName);

            // ToDo: Just "reserve" the crewName - so if crewName exists and membercount is just one or zero and its older than a day - delete it (this can be done by the "isCrewNameAvailable" too).
            ServerPackets pak = new ServerPackets();
            if (isCrewNameAvailable)
            {
                Store.dbManager.WorldDbHandler.AddCrew(crewName,StringUtils.charBytesToString_NZ(Store.currentClient.playerInstance.CharacterName.getValue()));
                pak.SendCrewInviteToPlayer(playerHandle, crewName);
            }
            else
            {
                pak.sendSystemChatMessage(Store.currentClient,"Crewname was already taken - please choose a new one","BROADCAST");
            }

        }

        public void ProcessDisbandFaction(ref byte[] packet)
        {
            // ToDo: Check if i am the leader, disband the faction and tell that to all other players
        }

        public void ProcessDepositMoney(ref byte[] rpcData)
        {
            PacketReader reader = new PacketReader(rpcData);
            uint type = reader.readUint8();
            UInt32 moneyToDepositOrTake = reader.readUInt32(1);
            uint IsGivingMoney = reader.readUint8();

            ServerPackets packet = new ServerPackets();
            switch (type)
            {
                case 1:
                    // This is too faction
                    if (IsGivingMoney==1)
                    {
                        UInt32 newMoneyAmount = (UInt32) Store.currentClient.playerData.getInfo() - moneyToDepositOrTake;
                        // Update Info
                        Store.dbManager.WorldDbHandler.IncreaseFactionMoney(NumericalUtils.ByteArrayToUint16(Store.currentClient.playerInstance.FactionID.getValue(),1), moneyToDepositOrTake);
                        Store.dbManager.WorldDbHandler.SaveInfo(Store.currentClient,newMoneyAmount);
                        Store.currentClient.playerData.setInfo(newMoneyAmount);
                    }
                    else
                    {
                        UInt32 newMoneyAmount = (UInt32) Store.currentClient.playerData.getInfo() + moneyToDepositOrTake;
                        // Update Info
                        Store.dbManager.WorldDbHandler.DecreaseFactionMoney(NumericalUtils.ByteArrayToUint16(Store.currentClient.playerInstance.FactionID.getValue(),1), moneyToDepositOrTake);
                        Store.dbManager.WorldDbHandler.SaveInfo(Store.currentClient,newMoneyAmount);
                        Store.currentClient.playerData.setInfo(newMoneyAmount);
                    }

                    //ProcessFactionInfoUpdate();
                    break;
                case 2:
                    // This is to crew
                    if (IsGivingMoney==1)
                    {
                        UInt32 newMoneyAmount = (UInt32) Store.currentClient.playerData.getInfo() - moneyToDepositOrTake;
                        // Update Info
                        Store.dbManager.WorldDbHandler.IncreaseCrewMoney(NumericalUtils.ByteArrayToUint16(Store.currentClient.playerInstance.CrewID.getValue(),1), moneyToDepositOrTake);
                        Store.dbManager.WorldDbHandler.SaveInfo(Store.currentClient,newMoneyAmount);
                        Store.currentClient.playerData.setInfo(newMoneyAmount);
                    }
                    else
                    {
                        UInt32 newMoneyAmount = (UInt32) Store.currentClient.playerData.getInfo() + moneyToDepositOrTake;
                        // Update Info
                        Store.dbManager.WorldDbHandler.DecreaseCrewMoney(NumericalUtils.ByteArrayToUint16(Store.currentClient.playerInstance.CrewID.getValue(),1), moneyToDepositOrTake);
                        Store.dbManager.WorldDbHandler.SaveInfo(Store.currentClient,newMoneyAmount);
                        Store.currentClient.playerData.setInfo(newMoneyAmount);
                    }
                    //ProcessCrewInfoUpdate();
                    break;
            }

            packet.SendMoneyUpdateFactionCrew(Store.currentClient, (ushort) type, moneyToDepositOrTake, (ushort) IsGivingMoney);
            packet.SendInfoCurrent(Store.currentClient, (UInt32)Store.currentClient.playerData.getInfo());
            
        }

        public void ProcessCrewInfoUpdate()
        {
            // ToDo: maybe we can remove this as we update money on the other way (but we need to check if this is the case)
            UInt32 crewId = NumericalUtils.ByteArrayToUint32(Store.currentClient.playerInstance.CrewID.getValue(), 1);
            Crew crewData = Store.dbManager.WorldDbHandler.GetCrewData(crewId);
            List<CrewMember> members = Store.dbManager.WorldDbHandler.GetCrewMembersForCrewId(crewId);
            ServerPackets packet = new ServerPackets();
            packet.SendCrewInfo(Store.currentClient, crewData, members);
        }
        
        public void ProcessFactionInfoUpdate()
        {
            // ToDo: maybe we can remove this as we update money on the other way (but we need to check if this is the case)
            UInt32 factionId = NumericalUtils.ByteArrayToUint32(Store.currentClient.playerInstance.FactionID.getValue(), 1);
            
            Faction faction = Store.dbManager.WorldDbHandler.fetchFaction(factionId);
            faction.masterPlayerCharId =
                Store.dbManager.WorldDbHandler.getCharIdByHandle(faction.masterPlayerHandle);
            
            ServerPackets packet = new ServerPackets();
            packet.SendFactionInfo(Store.currentClient, faction);
            packet.SendFactionCrews(Store.currentClient, faction);
        }
    }
}
