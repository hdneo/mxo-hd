using System;
using System.Collections;
using System.Collections.Generic;

using hds.shared;

namespace hds{
	
	#region OldHelpers moved here
	
	class LAMEBootingHelperWorld{

        /*
        public byte[] generateLocationHeader(){
			
            // Prepare the dict
            Dictionary<uint, string> locs = new Dictionary<uint, string>();
            locs.Add((uint)MxOLocations.TUTORIAL, "resource/worlds/final_world/tutorial_v2/tutorial_v2.metr");
            locs.Add((uint)MxOLocations.SLUMS, "resourceresource/worlds/final_world/international/it.metr");
            locs.Add((uint)MxOLocations.DOWNTOWN, "resource/worlds/final_world/downtown/dt_world.metr");
            locs.Add((uint)MxOLocations.ARCHIVE01, "resource/worlds/final_world/constructs/archive/archive01/archive01.metr");
            locs.Add((uint)MxOLocations.ARCHIVE02, "resource/worlds/final_world/constructs/archive/archive02/archive02.metr");
            locs.Add((uint)MxOLocations.ASHENCOURT, "resource/worlds/final_world/constructs/archive/archive_ashencourte/archive_ashencourte.metr");
            locs.Add((uint)MxOLocations.DATAMINE, "resource/worlds/final_world/constructs/archive/archive_datamine/datamine.metr");
            locs.Add((uint)MxOLocations.SAKURA, "resource/worlds/final_world/constructs/archive/archive_sakura/archive_sakura.metr");
            locs.Add((uint)MxOLocations.SATI, "resource/worlds/final_world/constructs/archive/archive_sati/sati.metr");
            locs.Add((uint)MxOLocations.WIDOWSMOOR, "resource/worlds/final_world/constructs/archive/archive_widowsmoor/archive_widowsmoor.metr");
            locs.Add((uint)MxOLocations.YUKI, "resource/worlds/final_world/constructs/archive/archive_yuki/archive_yuki.metr");
            locs.Add((uint)MxOLocations.LARGE01, "resource/worlds/final_world/constructs/large/large01/large01.metr");
            locs.Add((uint)MxOLocations.LARGE02, "resource/worlds/final_world/constructs/large/large02/large02.metr");
            locs.Add((uint)MxOLocations.MEDIUM01, "resource/worlds/final_world/constructs/medium/medium01/medium01.metr");
            locs.Add((uint)MxOLocations.MEDIUM02, "resource/worlds/final_world/constructs/medium/medium02/medium02.metr");
            locs.Add((uint)MxOLocations.MEDIUM03, "resource/worlds/final_world/constructs/medium/medium03/medium03.metr");
            
            
            
            
            
            locs[SMALL03] = "resource/worlds/final_world/constructs/small/small03/small03.metr";
            locs[CAVES] = "resource/worlds/final_world/zion_caves.metr";

            string districtName = Store.currentClient.playerData.getDistrict();
			
            DynamicArray din = new DynamicArray();
			
            //We need to use atlasByte for atlas showing right ;)
            string atlasByte;

            switch (districtName){

                case "downtown":
                    atlasByte = "\x02";
                    break;

                case "international":
                    atlasByte = "\x03";
                    break;

                case "slums":
                    atlasByte = "\x01";
                    break;

                default:
                    atlasByte = "\x00";
                    break;
            }
            //TODO: need to add 0 for tutorialV2 map

            //Get path for world
            string path = Store.dbManager.WorldDbHandler.getPathForDistrictKey(districtName)+"\x00"; //c-string
						
            int length = path.Length+1;
            int length2 = path.Length+17;
			
            byte[] pathLength = NumericalUtils.uint16ToByteArray((UInt16)length,1);
            byte[] pathLength2 = NumericalUtils.uint16ToByteArray((UInt16)length2,1);
			
            string middle = "\x06\x0e\x00"+atlasByte+"\x00\x00\x00\xd8\x68\xc8\x47\x01";

            // Massive
            //string environment = "\x08\x00\x4D\x61\x73\x73\x69\x76\x65\x00"; // Massive
            string environment = "\x08\x00\x53\x61\x74\x69\x53\x6b\x79\x00"; // Sati Sky

            string pack = middle + "####" +path+environment; //"####" are just to fill 4bytes syze
			
            int packetLength = pack.Length;
            byte[] packetLengthHex = NumericalUtils.uint16ToByteArrayShort((UInt16)packetLength);
			
			
            din.append(StringUtils.stringToBytes(middle));
            din.append(pathLength2);
            din.append(pathLength);
            din.append(StringUtils.stringToBytes(path));
            din.append(StringUtils.stringToBytes(environment));
            return din.getBytes();
        }*/
    }
	
	
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
            Store.currentClient.playerInstance.FactionID.disable();
            //Store.currentClient.playerInstance.FactionID.setValue(61954); // in hex f202 or 02f2
            Store.currentClient.playerInstance.CombatantMode.setValue((byte)0x22); //TODO: see what's combatantmode


            Store.currentClient.playerInstance.CurExclusiveAbility.setValue(CurCombatExclusiveAbility); //TODO: see what's this

            // ok we set all our values - lets get the generated packet for us
            DynamicArray creationPacketWithoutView = Store.world.objMan.generateCreationPacket(Store.currentClient.playerInstance, 0x0000);
            creationPacket = Store.world.objMan.generateCreationPacket(Store.currentClient.playerInstance, 0x0000);
        }
	}
	
	#endregion	

}

