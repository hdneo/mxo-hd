using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Text;

using hds.shared;
using hds.world.Structures;

namespace hds
{
    class PlayerHandler
    {

        // Important: We musts load this after the setup state is set to 0x0f (so before that it wouldnt work properly)
        public void processAttributes()
        {
            ServerPackets server = new ServerPackets();
            server.sendPlayerAttributes(Store.currentClient);
            server.SendPlayerFriendList(Store.currentClient);
            //Store.currentClient.playerData.setPss(0x7f);
        }

        public void processMood(ref byte[] packet)
        {
            byte moodByte = packet[0];
            ServerPackets server = new ServerPackets();
            //ToDo: Announce to other Players (and find packet for it) and save this in playerObject for new players
            server.sendMoodChange(Store.currentClient, moodByte);
        }

        public void processEmote(ref byte[] packet)
        {
            byte[] emoteBytes = new byte[4];
            emoteBytes[0] = packet[0];
            emoteBytes[1] = packet[1];
            emoteBytes[2] = packet[2];
            emoteBytes[3] = packet[3];

            byte emoteByte = packet[0];
            UInt32 emoteKey = NumericalUtils.ByteArrayToUint32(emoteBytes, 0);
            
            ServerPackets server = new ServerPackets();
            server.sendEmotePerform(Store.currentClient, emoteKey);
        }

        public void processSpawn()
        {

            if (Store.currentClient.playerData.getDistrictId()> 0)
            {
                // REFACTOR!!!!
                byte[] rsiObject = new BootingHelperRsi().generateSelfSpawnPacket(Store.currentClient);
                //Store.world.sendViewPacketToAllPlayers(new BootingHelperRsi().generatePlayerSpawnPacket(Store.currentClient), Store.currentClient.playerData.getCharID(), NumericalUtils.ByteArrayToUint16(Store.currentClient.playerInstance.getGoid(), 1), Store.currentClient.playerData.getEntityId());

                Store.currentClient.messageQueue.addObjectMessage(rsiObject, false);
                Store.currentClient.playerData.setOnWorld(true);
            }
            

        }

        public void processPlayerSetup()
        {
            // REFACTOR: Move to PlayerHandler and PlayerPackets (and location header to Server Packets)	

            ServerPackets packets = new ServerPackets();

            packets.sendWorldCMD(Store.currentClient, Store.currentClient.playerData.getDistrictId(), Store.worldConfig.weather); // Genereal Summer Sky
            //packets.sendWorldCMD(Store.currentClient, Store.currentClient.playerData.getDistrictId(),"Massive");
            //packets.sendWorldCMD(Store.currentClient, Store.currentClient.playerData.getDistrictId(),"WinterSky3"); // Winter is coming to MxO
            // packets.SendServerSettingString(Store.currentClient,"WR_RezEvents","bluesky2");

            // This is more a test
            packets.SendWorldSetup(Store.currentClient);

            packets.sendEXPCurrent(Store.currentClient, (UInt32)Store.currentClient.playerData.getExperience());
            packets.SendInfoCurrent(Store.currentClient, (UInt32)Store.currentClient.playerData.getInfo());

            /*
            long exp = Store.currentClient.playerData.getExperience();
            long cash = Store.currentClient.playerData.getInfo();
            string expStr = "80e1" + StringUtils.bytesToString_NS(NumericalUtils.uint32ToByteArray((UInt32)exp, 1)) + "00000000";
            string cashStr = "80df" + StringUtils.bytesToString_NS(NumericalUtils.uint32ToByteArray((UInt32)cash, 1)) + "00000000";
            */

            // There are for testing - need to change this later to load from ClientObject / DB 
            UInt16 focus = 24;
            UInt16 belief = 25;
            UInt16 vitality = 26;
            UInt16 perception = 27;
            UInt16 reason = 28;

            
            packets.sendAttribute(Store.currentClient, focus, 0x4e);
            packets.sendAttribute(Store.currentClient, perception, 0x4f);
            packets.sendAttribute(Store.currentClient, reason, 0x51);
            packets.sendAttribute(Store.currentClient, belief, 0x52);
            packets.sendAttribute(Store.currentClient, vitality, 0x54);
            
            /*
            string focusRPC = "80ad4e" + StringUtils.bytesToString_NS(NumericalUtils.uint16ToByteArray(focus, 0)) + "000802";
            string beliefRPC = "80ad52" + StringUtils.bytesToString_NS(NumericalUtils.uint16ToByteArray(belief, 0)) + "000802";
            string vitalityRPC = "80ad54" + StringUtils.bytesToString_NS(NumericalUtils.uint16ToByteArray(vitality, 0)) + "000802";
            string perceptionRPC = "80ad4f" + StringUtils.bytesToString_NS(NumericalUtils.uint16ToByteArray(perception, 0)) + "000802";
            string reasonRPC = "80ad51" + StringUtils.bytesToString_NS(NumericalUtils.uint16ToByteArray(reason, 0)) + "000802";

            
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes(focusRPC)); // Focus 
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes(beliefRPC)); // Belief
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes(vitalityRPC)); // Vitality
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes(perceptionRPC)); // Perception
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes(reasonRPC)); // Reason
            */
            //Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("808615A0070000000000000000000000000000000000000000210000000000230000000000")); // Cew and Faction Window ena
            // Disable later

            //8167170020001C2200C60111000000000000002900000807006D786F656D750007006D786F656D750002000200000000000000
            //Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80b2110011000802")); // What is this ? Check it later
            /*
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc4503110000020000001100110000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc450002000002000000cc00040000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45000300000b0000003702330000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45000400002d0000002304030000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45000500002d0000002004030000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45000600002d0000001904010000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45000700002d0000004204010000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc4500080000340000008302440000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45000900004c0000001504040000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45000a00004c0000000604040000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45000b00004c0000001904020000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45000c0000530000001504010000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45000d0000530000001904010000000000000000"));
            */
            //Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80b23a0400000802"));
            /*
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45033a0400530000003a04000000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45000e0000530000000604010000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45000f00005f0000007c04050000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc45001000005f0000000704050000000000000000"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc4500110000620000000604010000000000000000"));
            */
            //Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80b2350400000802"));

            // Buddylist
            BuddylistHandler buddylistHandler = new BuddylistHandler();
            buddylistHandler.ProcessAnnounceFriendOnline(Store.currentClient.playerData.getCharID(), StringUtils.charBytesToString_NZ(Store.currentClient.playerInstance.CharacterName.getValue()));
            ;

            ServerPackets pak = new ServerPackets();

            UInt32 factionId =
                NumericalUtils.ByteArrayToUint32(Store.currentClient.playerInstance.FactionID.getValue(), 1);
            if (factionId > 0)
            {
                Faction faction = Store.dbManager.WorldDbHandler.FetchFaction(factionId);
                faction.masterPlayerCharId =
                    Store.dbManager.WorldDbHandler.GetCharIdByHandle(faction.masterPlayerHandle);
                pak.SendFactionInfo(Store.currentClient, faction, false);
                pak.SendFactionCrews(Store.currentClient, faction, false);

            }
            
            // Test icon + bonus EDIT DOESNT WORK IN THIS STATE
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bc1500450000f70300000702ecffffff0000000000"));
            
            UInt32 crewId = NumericalUtils.ByteArrayToUint32(Store.currentClient.playerInstance.CrewID.getValue(), 1);
            Crew crewData = Store.dbManager.WorldDbHandler.GetCrewData(crewId);
            List<CrewMember> members = Store.dbManager.WorldDbHandler.GetCrewMembersForCrewId(crewId);
            pak.SendCrewInfo(Store.currentClient, crewData, members);
            pak.sendCrewAndFactionEnableWindow(Store.currentClient);
        }

        public void processInitUDPSession(ref byte[] packetData)
        {
            PacketReader reader = new PacketReader(packetData);
            reader.SetOffsetOverrideValue(11);

            Store.currentClient.playerData.setCharID(reader.ReadUInt32(1));

            Store.dbManager.WorldDbHandler.SetPlayerValues();
            Store.dbManager.WorldDbHandler.SetRsiValues();
            Store.dbManager.WorldDbHandler.SetOnlineStatus(Store.currentClient.playerData.getCharID(), 1);

            // send the init UDP packet * 5
            byte[] response = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05 };
            for (int i = 0; i < 5; i++)
            {
                Store.currentClient.messageQueue.addRawMessage(response);    
            }
            
            Store.currentClient.FlushQueue();
            Store.margin.SendUDPSessionReply(Store.currentClient.playerData.getCharID());
        }

        public void processGetBackgroundRequest(ref byte[] packetData)
        {
            ServerPackets packet = new ServerPackets();
            packet.sendGetBackgroundMessage(Store.currentClient);
        }

        public void IncrementPlayerExp(UInt32 expToIncrement)
        {
            long newExperienceValue = Store.currentClient.playerData.getExperience() + expToIncrement;
            Store.currentClient.playerData.setExperience(newExperienceValue);
            Store.dbManager.WorldDbHandler.SaveExperience(Store.currentClient, newExperienceValue);

            ServerPackets packets = new ServerPackets();
            packets.sendEXPCurrent(Store.currentClient, (UInt32)newExperienceValue);
        }

        public void ProcessSetBackgroundRequest(ref byte[] packetData)
        {
            UInt16 backgroundSize = NumericalUtils.ByteArrayToUint16(new byte[] {packetData[3], packetData[4]},1);

            byte[] backgroundBytes = new byte[backgroundSize-1];
            ArrayUtils.copy(packetData,5,backgroundBytes,0,backgroundSize-1);

            string backgroundText = StringUtils.charBytesToString(backgroundBytes);
            Store.dbManager.WorldDbHandler.setBackground(backgroundText);

        }

        public void ProcessLootAccepted()
        {
             
            ClientView F = Store.currentClient.viewMan.getViewById(Store.currentClient.playerData.currentSelectedTargetViewId);

            // ToDo: we currently not know what items and money we should have - we give everytime 5000 currently
            UInt32 value = 5000; 
            UInt32 newMoneyAmount = (UInt32) Store.currentClient.playerData.getInfo() + value;
            // Update Info
            Store.dbManager.WorldDbHandler.SaveInfo(Store.currentClient,newMoneyAmount);
            Store.currentClient.playerData.setInfo(newMoneyAmount);
                        
            ServerPackets packet = new ServerPackets();
            packet.SendInfoCurrent(Store.currentClient, (UInt32)Store.currentClient.playerData.getInfo());
            packet.SendLootAccepted(Store.currentClient);
            // ToDo: send "loot disabled" 
        }

        public void ProcessPlayerGetDetails(ref byte[] rpcData)
        {
            PacketReader reader = new PacketReader(rpcData);
            UInt32 unknownAlwaysZeroUint32 = reader.ReadUInt32(1);
            UInt16 alwaysEightUint16 = reader.ReadUInt16(1);
            string handle = reader.ReadSizedString();
            
            // Load Details from Database about the Handle
            Hashtable charInfo = Store.dbManager.WorldDbHandler.GetCharInfoByHandle(handle);
            if (charInfo.Count > 0)
            {
                ServerPackets packets = new ServerPackets();
                packets.SendPlayerGetDetails(Store.currentClient, charInfo);
                packets.SendPlayerBackground(Store.currentClient, charInfo);
            }
            
        }
        
        public void ProcessTargetChange(ref byte[] rpcData, WorldClient currentClient)
        {
            UInt16 viewId = NumericalUtils.ByteArrayToUint16(new byte[] {rpcData[0], rpcData[1]}, 1);
            ushort spawnId = rpcData[2];
            // ToDo: add this to the ClientData

            if (viewId == 0)
            {
                viewId = 2;
            }
            currentClient.playerData.currentSelectedTargetViewId = viewId;
            currentClient.playerData.currentSelectedTargetSpawnId = spawnId;
            ServerPackets pak = new ServerPackets();
            pak.sendSystemChatMessage(Store.currentClient,
                "TARGET CHANGE For ViewID " + viewId.ToString() + " AND SPAWN ID : " + spawnId.ToString(), "MODAL");
        }
        
        public void ProcessLoadAbility(ref byte[] packet)
        {
            // read the values from the packet
            PacketReader reader = new PacketReader(packet);
            UInt32 staticObjectID = reader.ReadUInt32(1);
            UInt16 unloadFlag = reader.ReadUInt16(1);
            UInt16 loadFlag = reader.ReadUInt16(1);
            UInt16 countAbilities = reader.ReadUInt16(1);

            int pointer = 11; // Start at index 11
            List<UInt16> abilitySlots = new List<UInt16>();

            for (int i = 1; i <= countAbilities; i++)
            {
                // This must be looped 
                byte[] slotByteID = new byte[2];
                ArrayUtils.copyTo(packet, pointer, slotByteID, 0, 2);
                pointer = pointer + 2;

                byte[] abilityByteID = new byte[2];
                ArrayUtils.copyTo(packet, pointer, abilityByteID, 0, 2);
                pointer = pointer + 4;

                byte[] abilityByteLevel = new byte[2];
                ArrayUtils.copyTo(packet, pointer, abilityByteLevel, 0, 2);
                pointer = pointer + 3;


                UInt16 slotID = NumericalUtils.ByteArrayToUint16(slotByteID, 1);
                UInt16 AbilityID = NumericalUtils.ByteArrayToUint16(abilityByteID, 1);
                UInt16 AbilityLevel = NumericalUtils.ByteArrayToUint16(abilityByteLevel, 1);

                PacketContent pak = new PacketContent();
                if (unloadFlag > 0)
                {
                    pak.AddUint16((UInt16) RPCResponseHeaders.SERVER_ABILITY_UNLOAD, 0);
                    pak.AddByteArray(abilityByteID);
                }
                else
                {
                    pak.AddUint16((UInt16) RPCResponseHeaders.SERVER_ABILITY_LOAD, 0);
                    pak.AddByteArray(abilityByteID);
                    pak.AddByteArray(abilityByteLevel);
                    pak.AddByteArray(slotByteID);
                }
                abilitySlots.Add(slotID);
                Store.currentClient.messageQueue.addRpcMessage(pak.ReturnFinalPacket());
            }


            if (unloadFlag > 0)
            {
                Store.dbManager.WorldDbHandler.UpdateAbilityLoadOut(abilitySlots, 0);
            }
            else
            {
                Store.dbManager.WorldDbHandler.UpdateAbilityLoadOut(abilitySlots, 1);
            }
        }
    }
}
