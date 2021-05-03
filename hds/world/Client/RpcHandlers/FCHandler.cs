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
        public void ProcessLoadFactionName(ref byte[] packet)
        {

            PacketReader packetReader = new PacketReader(packet);
            UInt32 factionId = packetReader.ReadUInt32(1);

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
            pakRead.IncrementOffsetByValue(1);

            UInt16 offsetHandle = pakRead.ReadUInt16(1);
            UInt16 offsetCrewName = pakRead.ReadUInt16(1);
            string crewName = pakRead.ReadSizedZeroTerminatedString().Trim();
            string playerHandle = pakRead.ReadSizedZeroTerminatedString().Trim();
            string crewCaptainHandle =
                StringUtils.charBytesToString_NZ(Store.currentClient.playerInstance.CharacterName.getValue());

            UInt32 crewId = NumericalUtils.ByteArrayToUint32(Store.currentClient.playerInstance.CrewID.getValue(), 1);

            bool isCrewNameAvailable = Store.dbManager.WorldDbHandler.IsCrewNameAvailable(crewName);
            bool isNewCrew = false;
            if (crewId == 0 && isCrewNameAvailable)
            {
                // Create The Crew
                Store.dbManager.WorldDbHandler.AddCrew(crewName,crewCaptainHandle);
                crewId = Store.dbManager.WorldDbHandler.GetCrewIdByCrewMasterHandle(crewCaptainHandle);
                // We add ourself as captain to the crew
                Store.dbManager.WorldDbHandler.AddMemberToCrew(Store.currentClient.playerData.getCharID(), crewId, 0, 1, 0  );
                isNewCrew = true;
            }
            
            ServerPackets pak = new ServerPackets();
            if (isNewCrew || crewId != 0)
            {
                pak.SendCrewInviteToPlayer(playerHandle, crewName);
            }
            else
            {
                pak.sendSystemChatMessage(Store.currentClient,"Crewname was already taken - please choose a new one","BROADCAST");
            }

        }

        public void ProcessInvitePlayerToNewFaction(ref byte[] packet)
        {
            PacketReader reader = new PacketReader(packet);
            // We read the offset but we would not really need it (as we just read a sized terminated string)
            UInt16 offsetHandleToInvite = reader.ReadUInt16(1);
            UInt16 offsetFactionName = reader.ReadUInt16(1);
            string factionName = reader.ReadSizedZeroTerminatedString();
            string handleToInvite = reader.ReadSizedZeroTerminatedString();

            string masterHandle =
                StringUtils.charBytesToString_NZ(Store.currentClient.playerInstance.CharacterName.getValue());
            
            if (Store.dbManager.WorldDbHandler.IsFactionnameAvailable(factionName) && Store.dbManager.WorldDbHandler.IsHandleCaptainOfACrew(handleToInvite) && Store.dbManager.WorldDbHandler.IsHandleCaptainOfACrew(masterHandle))
            {
                // We should now create the faction and add the crews to it (and don't forget to update the members)
                Crew crewMaster = Store.dbManager.WorldDbHandler.GetCrewData(Store.dbManager.WorldDbHandler.GetCrewIdByCrewMasterHandle(masterHandle));
                Crew crewSecondCaptaint = Store.dbManager.WorldDbHandler.GetCrewData(Store.dbManager.WorldDbHandler.GetCrewIdByCrewMasterHandle(handleToInvite));
                
                UInt32 factionId = Store.dbManager.WorldDbHandler.createFaction(factionName, crewMaster, crewSecondCaptaint);
                ServerPackets packets = new ServerPackets();
                
                if (factionId > 0)
                {
                    crewMaster.factionId = factionId;
                    crewSecondCaptaint.factionId = factionId;
                    // Update Faction Id
                    var crewMembers = from crewMasterClients in WorldServer.Clients
                        where NumericalUtils.ByteArrayToUint32(crewMasterClients.Value.playerInstance.CrewID.getValue(),
                                  1) ==
                              crewMaster.crewId ||
                              NumericalUtils.ByteArrayToUint32(crewMasterClients.Value.playerInstance.CrewID.getValue(),
                                  1) == crewSecondCaptaint.crewId
                        select crewMasterClients;

                    foreach (var client in crewMembers)
                    {
                        client.Value.playerInstance.FactionID.setValue(factionId);
                    }
                    
                    packets.SendJoinedGroup(1, factionId, crewMaster.crewId, crewMaster.characterMasterName);
                    packets.SendJoinedGroup(1, factionId, crewSecondCaptaint.crewId, crewSecondCaptaint.characterMasterName);
                    new FCHandler().ProcessFactionInfoUpdate(true);
                    
                    // Now selfView Updates
                    foreach (var client in crewMembers)
                    {

                        List<Attribute> updateAttributes = new List<Attribute>();
                        updateAttributes.Add(client.Value.playerInstance.FactionID);

                        PacketContent viewStateData = new PacketContent();
                        PacketContent myselfStateData = new PacketContent();
                        viewStateData.AddByteArray(client.Value.playerInstance.GetUpdateAttributes(updateAttributes));
                        myselfStateData.AddByteArray(client.Value.playerInstance.GetSelfUpdateAttributes(updateAttributes, true));
                        
                        // OtherView
                        Store.world.SendViewPacketToAllPlayers(viewStateData.ReturnFinalPacket(), client.Value.playerData.getCharID(), NumericalUtils.ByteArrayToUint16(client.Value.playerInstance.GetGoid(),1), client.Value.playerData.getEntityId());
                       
                        // SelfView
                        client.Value.messageQueue.addObjectMessage(myselfStateData.ReturnFinalPacket(), false);
                    }
                }
                else
                {
                    packets.SendFactionCreationError(Store.currentClient);    
                }
            }
        }

        public void ProcessDisbandFaction(ref byte[] packet)
        {
            // ToDo: Check if i am the leader, disband the faction and tell that to all other players
        }
        
        public void ProcessLeaveGroup(ref byte[] rpcData)
        {
            PacketReader reader = new PacketReader(rpcData);
            uint groupType = reader.ReadUint8();

            ServerPackets pak = new ServerPackets();
            // ToDo: when we leave we don't sent it to ourself (but maybe to the crew?)
            switch (groupType)
            {
                case 1:
                    // Leave Faction (remove crew from faction, tell it all others faction members that are online)
                    UInt32 factionId =
                        NumericalUtils.ByteArrayToUint32(Store.currentClient.playerInstance.FactionID.getValue(), 1);
                    pak.SendLeaveGroup(groupType, Store.currentClient.playerData.getCharID(), factionId);
                    break;
                
                case 2:
                    // Leave Group (announce it to all crew members)
                    UInt32 crewId =
                        NumericalUtils.ByteArrayToUint32(Store.currentClient.playerInstance.CrewID.getValue(), 1);
                    Store.dbManager.WorldDbHandler.RemoveMemberFromCrew(Store.currentClient.playerData.getCharID(), crewId);
                    pak.SendLeaveGroup(groupType, Store.currentClient.playerData.getCharID(), crewId);
                    break;
                case 3:
                    // ToDo: Leave Team can be done if we handle Teams :) 
                    UInt32 teamId =
                        NumericalUtils.ByteArrayToUint32(Store.currentClient.playerInstance.MissionTeamID.getValue(), 1);
                    pak.SendLeaveGroup(groupType, Store.currentClient.playerData.getCharID(), teamId);
                    break;
            }
        }

        public void ProcessDepositMoney(ref byte[] rpcData)
        {
            PacketReader reader = new PacketReader(rpcData);
            uint type = reader.ReadUint8();
            UInt32 moneyToDepositOrTake = reader.ReadUInt32(1);
            uint IsGivingMoney = reader.ReadUint8();

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
        
        public void ProcessFactionInfoUpdate(bool sendToAllMembers)
        {
            // ToDo: maybe we can remove this as we update money on the other way (but we need to check if this is the case)
            UInt32 factionId = NumericalUtils.ByteArrayToUint32(Store.currentClient.playerInstance.FactionID.getValue(), 1);
            
            Faction faction = Store.dbManager.WorldDbHandler.FetchFaction(factionId);
            faction.masterPlayerCharId =
                Store.dbManager.WorldDbHandler.GetCharIdByHandle(faction.masterPlayerHandle);
            
            ServerPackets packet = new ServerPackets();
            
            packet.SendFactionInfo(Store.currentClient, faction, sendToAllMembers);
            packet.SendFactionCrews(Store.currentClient, faction, sendToAllMembers);
        }
        
        
    }
}
