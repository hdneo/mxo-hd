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


        public byte[] generatePlayerSpawnPacket()
        {
            DynamicArray rsiPacket;
            DynamicArray creationPacket;
            playerRSIPacket(out rsiPacket, out creationPacket);

            // Same as generate Self without viewData
            rsiPacket.append(creationPacket.getBytes());
            return rsiPacket.getBytes();

        }

		public byte[] generateSelfSpawnPacket(){
            DynamicArray rsiPacket;
            DynamicArray creationPacket;
            playerRSIPacket(out rsiPacket, out creationPacket);

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

        private static void playerRSIPacket(out DynamicArray rsiPacket, out DynamicArray creationPacket)
        {
            //Create the packet for the player

            rsiPacket = new DynamicArray();
            byte[] rsi = PacketsUtils.getRSIBytes(Store.currentClient.playerData.getRsiValues());

            byte[] CurCombatExclusiveAbility = { 0x00, 0x10, 0x00, 0x00 };

            //TODO: fix this 03 01 00
            rsiPacket.append(new byte[] { 0x01, 0x00 });
            /////////////////////////////////////////////
            Store.currentClient.playerInstance.disableAllAttributes(); //Predisable to just send what we need to spawn

            Store.currentClient.playerInstance.RealFirstName.enable();
            Store.currentClient.playerInstance.RealLastName.enable();
            Store.currentClient.playerInstance.Health.enable();
            Store.currentClient.playerInstance.MaxHealth.enable();
            Store.currentClient.playerInstance.YawInterval.enable(); //ROTATION
            Store.currentClient.playerInstance.OrganizationID.enable();

            Store.currentClient.playerInstance.StealthAwareness.setValue((byte)0x01); // TODO: See what's stealth awareness
            Store.currentClient.playerInstance.InnerStrengthAvailable.enable();
            Store.currentClient.playerInstance.CharacterName.enable();

            Store.currentClient.playerInstance.TitleAbility.enable();
            Store.currentClient.playerInstance.CharacterID.enable(); // It was set when grabbing from database
            Store.currentClient.playerInstance.RSIDescription.setValue(rsi);
            Store.currentClient.playerInstance.InnerStrengthMax.enable();
            Store.currentClient.playerInstance.Position.enable();
            Store.currentClient.playerInstance.Level.enable();
            Store.currentClient.playerInstance.CombatantMode.setValue((byte)0x22); //TODO: see what's combatantmode


            Store.currentClient.playerInstance.CurExclusiveAbility.setValue(CurCombatExclusiveAbility); //TODO: see what's this

            // ok we set all our values - lets get the generated packet for us
            DynamicArray creationPacketWithoutView = Store.world.objMan.generateCreationPacket(Store.currentClient.playerInstance, 0x0000);
            creationPacket = Store.world.objMan.generateCreationPacket(Store.currentClient.playerInstance, 0x0000);
        }
	}
	
	#endregion	

}

