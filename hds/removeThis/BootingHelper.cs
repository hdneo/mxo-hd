using System;
using System.Collections;
using System.Collections.Generic;

using hds.shared;

namespace hds{
	
	#region OldHelpers moved here
	
	class BootingHelperRsi
	{
		 
		 
		private PacketsUtils packetUtils;
		public BootingHelperRsi (){
			packetUtils = new PacketsUtils();
		}


        public byte[] generatePlayerSpawnPacket(WorldClient client, UInt16 spawnIdCounter)
        {
            DynamicArray rsiPacket;
            DynamicArray creationPacket;
            playerRSIPacket(out rsiPacket, out creationPacket, client, spawnIdCounter);

            // Same as generate Self without viewData
            rsiPacket.append(creationPacket.getBytes());
            return rsiPacket.getBytes();

        }

		public byte[] generateSelfSpawnPacket(WorldClient client)
        {
            DynamicArray rsiPacket;
            DynamicArray creationPacket;
            client.playerData.selfSpawnIdCounter = client.playerData.spawnViewUpdateCounter;
            playerRSIPacket(out rsiPacket, out creationPacket, client, client.playerData.assignSpawnIdCounter());

            // self
            byte[] selfViewID = {0x02, 0x00 };

            creationPacket.append(selfViewID);
            creationPacket.append(0x00);
            /*
             * Maybe we must remove the first too..needs testing
            creationPacket.append(0x00);
            creationPacket.append(0x00);
            */
			rsiPacket.append(creationPacket.getBytes());
            
			return rsiPacket.getBytes();
			
		}

        private static void playerRSIPacket(out DynamicArray rsiPacket, out DynamicArray creationPacket, WorldClient client, ushort spawnIdCounter)
        {
            //Create the packet for the player

            rsiPacket = new DynamicArray();
            byte[] rsi = PacketsUtils.getRSIBytes(client.playerData.getRsiValues());

            byte[] CurCombatExclusiveAbility = { 0x00, 0x10, 0x00, 0x00 };

            //TODO: fix this 03 01 00
            rsiPacket.append(new byte[] { 0x01, 0x00 });
            /////////////////////////////////////////////
            client.playerInstance.DisableAllAttributes(); //Predisable to just send what we need to spawn

            client.playerInstance.RealFirstName.enable();
            client.playerInstance.RealLastName.enable();
            client.playerInstance.Health.enable();
            client.playerInstance.MaxHealth.enable();
            client.playerInstance.YawInterval.enable(); //ROTATION
            client.playerInstance.OrganizationID.enable();

            client.playerInstance.StealthAwareness.setValue((byte)0x01); // TODO: See what's stealth awareness
            client.playerInstance.InnerStrengthAvailable.enable();
            client.playerInstance.CharacterName.enable();

            client.playerInstance.TitleAbility.enable();
            client.playerInstance.CharacterID.enable(); // It was set when grabbing from database
            client.playerInstance.RSIDescription.setValue(rsi);
            client.playerInstance.InnerStrengthMax.enable();
            client.playerInstance.Position.enable();
            client.playerInstance.Level.enable();
            client.playerInstance.CombatantMode.setValue((byte)0x22); //TODO: see what's combatantmode


            client.playerInstance.CurExclusiveAbility.setValue(CurCombatExclusiveAbility); //TODO: see what's this

            // ok we set all our values - lets get the generated packet for us
            DynamicArray creationPacketWithoutView = Store.world.objMan.GenerateCreationPacket(client.playerInstance, 0x0000,(byte)spawnIdCounter);
            creationPacket = Store.world.objMan.GenerateCreationPacket(client.playerInstance, 0x0000,(byte)spawnIdCounter);
        }
	}
	
	#endregion	

}

