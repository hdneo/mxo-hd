/// <summary>
/// Hardline Dreams Team - 2010
/// Created by Neo
/// Modified by Morpheus
/// </summary>

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using hds.databases.Entities;
using hds.shared;

namespace hds{

	public class MarginClient{
		
		private int hashID;
		private TcpClient tcpClient;
		private bool working;
		private bool waitingForWorld;
		private UInt32 userID;
        private UInt32 newCharID;
        private ushort numCharacterReplies;
        private bool isNewCreatedChar;
        public List<MarginAbilityItem> Abilities;
		
		private MarginEncryption marginEncr;
		  
		private NetworkStream clientStream;
        public int marginThreadId;

        public MarginClient(int hashID){
			
			this.hashID = hashID;
			
			working = true;
			waitingForWorld = false;
			
			marginEncr = new MarginEncryption();
			 
		}


		public int getID(){
			return hashID;
		}

        public UInt32 getCharID()
        {
            return newCharID;
        }
		
		public bool isWorking(){
			return working;
		}
		
		public void passiveClose(){
			working = false;
		}
		
		public void forceClose(){
			tcpClient.Close();
			clientStream.Close();
		}
		
		public bool isYourClientWaiting(UInt32 waitingID){
			if (userID==waitingID){
				waitingForWorld = false;
				return true;
			}
			return false;
		}
			
		public void HandleClientComm(object client){
  			tcpClient = (TcpClient)client;
  			clientStream= tcpClient.GetStream();
			
			// Define an auth server processor per thread
						
  			byte[] message = new byte[2048];
  			int bytesRead;
			
			Output.WriteLine("Margin client Connected.");
  			
	
			// Receive TCP auth packets from the connected client.
			while (working){
    			bytesRead = 0;

	    		try{ 
					bytesRead = clientStream.Read(message, 0, 2048);
				}
				catch{ break; }
	
	    		if (bytesRead == 0){
	            	Output.OptWriteLine("Margin: 0 Bytes received");
					break;
				}
				
				// Parse the received packet data
				try{
					
					byte[] packet = new byte[bytesRead];
					ArrayUtils.copy(message,0,packet,0,bytesRead);
    				packetHandler(packet,clientStream);
									
				}catch(MarginException marEx){
					Output.WriteLine(marEx);
					break;
				}
				
  			}
			working = false;
  			Output.WriteLine("Margin Client Disconnected");
  			tcpClient.Close();
			clientStream.Close();
		}	
		
		
		public void packetHandler(byte[] packet, NetworkStream client )
        {
            bool encrypted = true;
            byte opcode = packet[2];
            byte[] data = { };
			byte pointer = packet[0];
			
            if (opcode == 0x01){
                // Packet is unencrypted
                encrypted = false;
                data = packet;
            }
			
			// This overrides the above IF for packets where 3rd position is a "01"
			// Happened someday, and screwed login... yeah, really
			if(pointer!=0x81){
				encrypted=true;
			}

            if (encrypted)
            {
                byte[] encryptedPacket = { };
                if (packet[0] >= 0x80)
                {
                    // try to readjust the packet for one byte less if the lenght is too long
                    // just a crappy way
                    encryptedPacket = new byte[packet.Length-1];
                    ArrayUtils.copy(packet,1,encryptedPacket,0,packet.Length-1);
                }
                else{
                    encryptedPacket = packet;
                }
                // Get the IV from encrypted Packet and set it in encryptor/decryptor
                byte[] decrypted = marginEncr.decryptMargin(encryptedPacket);
	            if (decrypted == null)
	            {
		            // This shouldnt happen - but can happen
		            tcpClient.Close();
		            clientStream.Close();
                    return;
	            }
                // Just 2 zero bytes for opcode handling later (As we use third byte for both state, encrypted or not
                byte[] spacer = { 0x00, 0x00 };

                DynamicArray din = new DynamicArray();
				din.append(spacer);
				din.append(decrypted);
				
                data = din.getBytes(); 

            }else{
                data = packet;
            }
            Output.WritePacketLog(data, "MARGINCLIENT", "0", "0", "0");

	        if (data.Length >= 3)
	        {
				opcode = data[2];
				
				//TODO: check if this needs "packet" or "data"
				switch (opcode)
				{
					case 0x01:
						certConnectRequest(packet, client);
						
					break;
	
					case 0x03:
						certConnectReply(packet, client);
					
					break;
	
					case 0x06:
						connectChallenge(packet, client);
					
					break;
	
					case 0x08:
						ConnectChallengeResponse(packet, client);
					break;
					
					case 0x0a:
						//CharNameRequest
						charNameRequest(data,client);
					break;
					
					case 0x0c:
						//TODO: this is creation. must be done someday
						Output.writeToLogForConsole("CREATECHAR RSI VALUES:"+ StringUtils.bytesToString(data));
						//loadCharacter(data, client);
						createCharacterRSI(data, client);
						// Add the first abilitys
						// AbilityID : 2147485696 (Awakened) Level 2
						// AbilityID : 
						
					break;
	
					case 0x0d:
						// Delete Charname Request
						deleteCharName(data, client);
					break;
					
					case 0x0f:
						loadCharacter(data, client, 0);
					break;
				}
	        }
        }



        private void SendMarginCharData(byte[] data, byte opcode, NetworkStream client, UInt16 shortAfterId, bool isLast)
        {
            // This Method will be used to send the real Data to the client

            // Format for general LoadCharacterReply Responses:
            // 10 00 00 00 <- everytime the same
            // 00 f9 76 04 <- uint32 Char ID 
            // 00 00 00 09 <- uint32 number of Character Replys (just raise one up)
            // 00 05 <- viewData opcode (what data is inside) uint16 
            // 10 00 <- if it has Data it is 10 00 if not is just 00 00 (and no Data can follow up and packet ends here)
            // e1 00 <- short / uint16 data size
            // DATA
            numCharacterReplies++;
            
            PacketContent pak = new PacketContent();
            pak.AddByte(0x10);
            pak.AddUint32(0,1);
            pak.AddUint32(newCharID, 1);
            pak.AddUint16(shortAfterId,1);
            pak.AddUShort(numCharacterReplies);
            if (isLast)
            {
	            pak.AddUShort(1);
            }
            else
            {
	            pak.AddUShort(0);
            }
            pak.AddUShort(opcode);
            if (data.Length > 0)
            {
	            pak.AddByteArray(new byte[]{0x10, 0x00});
	            pak.AddUint16((UInt16)data.Length,1);
	            pak.AddByteArray(data);
            }
            else
            {
	            pak.AddByteArray(new byte[]{0x00, 0x00});
            }

            Output.WriteDebugLog("[MARGIN SERVER RESPONSE] for OPCODE " + opcode + " : " + StringUtils.bytesToString(pak.ReturnFinalPacket())); 
            byte[] encryptedResponse = marginEncr.encrypt(pak.ReturnFinalPacket());
            sendTCPVariableLenPacket(encryptedResponse, client);
        }

		private void sendMarginData(string data,NetworkStream client){
			byte[] response = StringUtils.hexStringToBytes(data);
            byte[] encryptedResponse = marginEncr.encrypt(response);
            sendTCPVariableLenPacket(encryptedResponse, client);
		}

        private void deleteCharName(byte[] packet, NetworkStream client)
        {
            // Response should something like : 0e 44 87 1f 00 00 00 00 00 00 00 00 00
            
            // Get the charId from the packet
            byte[] charIDB = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            ArrayUtils.copy(packet, 5, charIDB, 0, 8); //Offset for charID is 5 in request
            
            // Need for SQL Delete
            UInt64 charID = NumericalUtils.ByteArrayToUint32(charIDB, 1);
            Store.dbManager.MarginDbHandler.deleteCharacter(charID);

            // Create viewData
            string deleteRequestResponse = "0e" + StringUtils.bytesToString_NS(charIDB) + "0000000000000000";
            sendMarginData(deleteRequestResponse, client);
        }
		

        private void createCharacterRSI(byte[] packet, NetworkStream client){
            // Instance the Data Loader
            DataLoader itemLoader = DataLoader.getInstance();
            isNewCreatedChar = true;
            string debugHexPacket = StringUtils.bytesToString_NS(packet);

            // ToDo: Replace all with Packet Reader Instance
            PacketReader reader = new PacketReader(packet);
	        reader.IncrementOffsetByValue(3);

            
	        UInt16 body = 0;
	        UInt16 gender = 0;

	        
	        UInt16 skintone = reader.ReadUInt16(1);
	        reader.SetOffsetOverrideValue(7);
	        UInt16 bodyTypeId = reader.ReadUInt16(1);
	        reader.SetOffsetOverrideValue(15);
	        UInt16 hairId = reader.ReadUInt16(1);
	        reader.SetOffsetOverrideValue(19);
	        UInt16 haircolor = reader.ReadUInt16(1);
	        reader.SetOffsetOverrideValue(23);
	        UInt16 tattoo = reader.ReadUInt16(1);
	        reader.SetOffsetOverrideValue(27);
	        UInt16 headId = reader.ReadUInt16(1);
	        reader.SetOffsetOverrideValue(31);
	        UInt16 facialDetail = reader.ReadUInt16(1);
	        reader.SetOffsetOverrideValue(35);
	        // ToDo: it has a bug - figure out correct position
	        UInt16 facialDetailColor = reader.ReadUInt16(1);
	        // ToDo: Remove this when facialDetailColor is parsed properly
	        facialDetailColor = 0;
	        reader.SetOffsetOverrideValue(67);
	        UInt16 profession = reader.ReadUInt16(1);
	        
	        
            // lets read the values
            // the IDs for the Appeareance is always uint16 goID
	        // Extra Hint: there are no leggins in Char Creation Process
	        reader.SetOffsetOverrideValue(35);
	        UInt16 hatId = reader.ReadUInt16(1);
	        reader.SetOffsetOverrideValue(39);
	        UInt16 eyewearId = reader.ReadUInt16(1);
	        reader.SetOffsetOverrideValue(43);
	        UInt16 shirtId = reader.ReadUInt16(1);
	        reader.SetOffsetOverrideValue(47);
	        UInt16 glovesId = reader.ReadUInt16(1);
	        reader.SetOffsetOverrideValue(51);
	        UInt16 outerwearId = reader.ReadUInt16(1);
	        reader.SetOffsetOverrideValue(55);
	        UInt16 pantsId = reader.ReadUInt16(1);
	        reader.SetOffsetOverrideValue(63);
	        UInt16 footwearId = reader.ReadUInt16(1);


            // Get Values by "NewRSI" IDs
            NewRSIItem hairItem = itemLoader.getNewRSIItemByTypeAndID("HAIR", hairId);
            NewRSIItem bodyItem = itemLoader.getNewRSIItemByTypeAndID("BODY", bodyTypeId);
            NewRSIItem headItem = itemLoader.getNewRSIItemByTypeAndID("HEAD", headId);

            Store.dbManager.MarginDbHandler.updateRSIValue("body", bodyItem.internalId.ToString(), newCharID);
            Store.dbManager.MarginDbHandler.updateRSIValue("sex", bodyItem.gender.ToString(), newCharID);
            Store.dbManager.MarginDbHandler.updateRSIValue("face", headItem.internalId.ToString(), newCharID);              // ToDo: 
            Store.dbManager.MarginDbHandler.updateRSIValue("hair", hairItem.internalId.ToString(), newCharID); 
            Store.dbManager.MarginDbHandler.updateRSIValue("haircolor", haircolor.ToString(), newCharID);                   // ToDo: check if it is correct
            Store.dbManager.MarginDbHandler.updateRSIValue("tattoo", tattoo.ToString(), newCharID);                         // ToDo:
	        Store.dbManager.MarginDbHandler.updateRSIValue("facialdetail", facialDetail.ToString(), newCharID);   			// ToDo:
            Store.dbManager.MarginDbHandler.updateRSIValue("facialdetailcolor", facialDetailColor.ToString(), newCharID);   // ToDo:
            Store.dbManager.MarginDbHandler.updateRSIValue("skintone", skintone.ToString(), newCharID);
            
            
			// Clothing Items
            ClothingItem shirt      = itemLoader.getItemValues(shirtId);
            ClothingItem pants      = itemLoader.getItemValues(pantsId);
            ClothingItem outerwear  = itemLoader.getItemValues(outerwearId);
            ClothingItem hat        = itemLoader.getItemValues(hatId);
            ClothingItem eyewear    = itemLoader.getItemValues(eyewearId);
            ClothingItem footwear   = itemLoader.getItemValues(footwearId);
            ClothingItem gloves     = itemLoader.getItemValues(glovesId);

            Store.dbManager.MarginDbHandler.updateRSIValue("hat", hat.getModelId().ToString(), newCharID);
            Store.dbManager.MarginDbHandler.updateRSIValue("shirt", shirt.getModelId().ToString(), newCharID);
            Store.dbManager.MarginDbHandler.updateRSIValue("shirtcolor", shirt.getColorId().ToString(), newCharID);
            Store.dbManager.MarginDbHandler.updateRSIValue("coat", outerwear.getModelId().ToString(), newCharID);
            Store.dbManager.MarginDbHandler.updateRSIValue("coatcolor", outerwear.getColorId().ToString(), newCharID);
            Store.dbManager.MarginDbHandler.updateRSIValue("pants", pants.getModelId().ToString(), newCharID);
            Store.dbManager.MarginDbHandler.updateRSIValue("pantscolor", pants.getColorId().ToString(), newCharID);
            Store.dbManager.MarginDbHandler.updateRSIValue("shoes", footwear.getModelId().ToString(), newCharID);
            Store.dbManager.MarginDbHandler.updateRSIValue("shoecolor", footwear.getColorId().ToString(), newCharID);
            Store.dbManager.MarginDbHandler.updateRSIValue("glasses", eyewear.getModelId().ToString(), newCharID);
            Store.dbManager.MarginDbHandler.updateRSIValue("glassescolor", eyewear.getColorId().ToString(), newCharID);
            Store.dbManager.MarginDbHandler.updateRSIValue("gloves", gloves.getModelId().ToString(), newCharID);

	        Store.dbManager.MarginDbHandler.AddItemToSlot(hatId,0x61,newCharID);
	        Store.dbManager.MarginDbHandler.AddItemToSlot(eyewearId,0x62,newCharID);
	        Store.dbManager.MarginDbHandler.AddItemToSlot(shirtId,0x63,newCharID);
	        Store.dbManager.MarginDbHandler.AddItemToSlot(glovesId,0x64,newCharID);
	        Store.dbManager.MarginDbHandler.AddItemToSlot(outerwearId,0x65,newCharID);
	        Store.dbManager.MarginDbHandler.AddItemToSlot(pantsId,0x66,newCharID);
	        Store.dbManager.MarginDbHandler.AddItemToSlot(footwearId,0x68,newCharID);
	        

            // FirstName
            UInt16 currentOffset = 79;
            byte[] firstNameLenBytes = new byte[2];
            ArrayUtils.copy(packet, currentOffset, firstNameLenBytes, 0,2);
            UInt16 firstNameLen = NumericalUtils.ByteArrayToUint16(firstNameLenBytes, 1);
            currentOffset += 2;

            byte[] firstNameBytes = new byte[NumericalUtils.ByteArrayToUint16(firstNameLenBytes,1) - 1];
            ArrayUtils.copy(packet, currentOffset, firstNameBytes, 0, firstNameLen - 1);
            string firstNameString = StringUtils.charBytesToString(firstNameBytes);
            currentOffset += firstNameLen;

            // LastName
            byte[] lastNameLenBytes = new byte[2];
            ArrayUtils.copy(packet, currentOffset, lastNameLenBytes, 0, 2);
            UInt16 lastNameLen = NumericalUtils.ByteArrayToUint16(lastNameLenBytes, 1);
            currentOffset += 2;

            byte[] lastNameBytes = new byte[NumericalUtils.ByteArrayToUint16(lastNameLenBytes, 1) - 1];
            ArrayUtils.copy(packet, currentOffset, lastNameBytes, 0, lastNameLen - 1);
            string lastNameString = StringUtils.charBytesToString(lastNameBytes);
            currentOffset += lastNameLen;

            // Description
            byte[] descriptionLenBytes = new byte[2];
            ArrayUtils.copy(packet, currentOffset, descriptionLenBytes, 0, 2);
            UInt16 descriptionLen = NumericalUtils.ByteArrayToUint16(descriptionLenBytes, 1);
            currentOffset += 2;

            byte[] descriptionBytes = new byte[NumericalUtils.ByteArrayToUint16(descriptionLenBytes, 1) - 1];
            ArrayUtils.copy(packet, currentOffset, descriptionBytes, 0, descriptionLen - 1);
            string descriptionString = StringUtils.charBytesToString(descriptionBytes);
            currentOffset += lastNameLen;

            // Update Characters values
            Store.dbManager.MarginDbHandler.updateCharacter(firstNameString, lastNameString, descriptionString,newCharID);
            
            // Add the Basic Abilitys...
            addStartAbilitys(newCharID);

            // we have all created - lets load the charData 
            loadCharacter(packet, client, newCharID);
        }

		


        private void addStartAbilitys(UInt32 charID){
            Store.dbManager.MarginDbHandler.addAbility(-2147481600, 0, charID, 1, 1);
            Store.dbManager.MarginDbHandler.addAbility(-2147367936, 1, charID, 1, 1);
            Store.dbManager.MarginDbHandler.addAbility(-2147294208, 2, charID, 0, 1);
            Store.dbManager.MarginDbHandler.addAbility(-2147281920, 3, charID, 0, 0);
            Store.dbManager.MarginDbHandler.addAbility(-2147280896, 4, charID, 0, 0);
            Store.dbManager.MarginDbHandler.addAbility(-2147437568, 5, charID, 1, 1);
            Store.dbManager.MarginDbHandler.addAbility(-2147425280, 6, charID, 0, 0);
            Store.dbManager.MarginDbHandler.addAbility(-2147404800, 7, charID, 0, 0);
            Store.dbManager.MarginDbHandler.addAbility(-2147445760, 8, charID, 0, 0);
            Store.dbManager.MarginDbHandler.addAbility(-2146493440, 9, charID, 0, 0);
            Store.dbManager.MarginDbHandler.addAbility(-2146453504, 10, charID, 1, 0);
            Store.dbManager.MarginDbHandler.addAbility(-2147472384, 11, charID, 20, 1); // Hyperjump
            Store.dbManager.MarginDbHandler.addAbility(-2147295232, 12, charID, 20, 1); // Hyperspeed
            Store.dbManager.MarginDbHandler.addAbility(-2147295232,13,charID,20,1); // HyperSprint
        }
		
		private void charNameRequest(byte[] packet,NetworkStream client){
			
			// Get the handle text
			byte[] handleSize = new byte[2];
			handleSize[0] =packet[5];
			handleSize[1] =packet[6];
			
			int size = NumericalUtils.ByteArrayToUint16(handleSize,1);
			
			byte[] handleB = new byte[size-1];
			ArrayUtils.copy(packet,7,handleB,0,size-1);
			string handleBStr = StringUtils.bytesToString_NS(handleB); // Handle as "414141"
			string handleStr = StringUtils.charBytesToString(handleB); // Handle as "AAA"
			
			// Process DB to answer the client - we directly create the character
            UInt32 dbResult = Store.dbManager.MarginDbHandler.getNewCharnameID(handleStr, userID);

            // Add the new charId so that we can work with it

			// Create the answer
			string nameRequestResponse="";
			
			if(dbResult==0){
				nameRequestResponse += "0b0f00010000110000000000000000";
				nameRequestResponse += StringUtils.bytesToString_NS(handleSize);
				nameRequestResponse += handleBStr+"00";
			}else{
				
				newCharID = dbResult;
				nameRequestResponse += "0b0f0000000000";
				nameRequestResponse += StringUtils.bytesToString_NS(NumericalUtils.uint32ToByteArray(dbResult,1));
				nameRequestResponse += "0000000000";
				nameRequestResponse += StringUtils.bytesToString_NS(handleSize);
				nameRequestResponse += handleBStr+"00";
			
			}

            Output.writeToLogForConsole(nameRequestResponse);
			sendMarginData(nameRequestResponse,client);
			
		}

        private byte[] loadBackgroundInfo(int charID){
	        Character marginCharacter = Store.dbManager.MarginDbHandler.GetCharInfo(charID);


            PacketContent pak = new PacketContent();
            if (!isNewCreatedChar)
            {
                pak.AddByteArray(StringUtils.hexStringToBytes("01000000640000007B0000006500000000000000000000006C000000000000002B010000"));
            }
            pak.AddStringWithFixedSized(marginCharacter.FirstName,31);
            pak.AddStringWithFixedSized(marginCharacter.LastName,31);
            pak.AddStringWithFixedSized(marginCharacter.Background,1023);


            // ToDo: Analyse it and (Rep zion => 82, Rep Machine 2, RepMero 81
            //pak.addHexBytes("000000000000000000178604E40008AF2F0175020000A39F714A81FF81FF81FF670067006700000003000301310000B402320000B403380000B4044E0000000200510000001600520000001900540000001300");
            pak.AddUint32((uint) marginCharacter.Exp,1);
            pak.AddUint32((uint) marginCharacter.Cash,1);
            pak.AddUShort(0x01); // Flag - always 1
            pak.AddUint32(200,1); // CQ Points
            
            pak.AddByteArray(TimeUtils.getCurrentTime());
            //pak.addByteArray(StringUtils.hexStringToBytes("875D714A")); // Last Changed Timestamp - current its static - needs to be dynamic

            // Rputation Bytes (int16[7])
            pak.AddInt16(-116,1);
            pak.AddInt16(-117, 1);
            pak.AddInt16(-20, 1);
            pak.AddInt16(-59, 1);
            pak.AddInt16(-58, 1);
            pak.AddInt16(-57, 1);
            pak.AddInt16(2, 1);
            pak.AddByte(0x03);
            
            // 01 = onWorld , 00 = LA . In World you have to take care to not spawn the Character in LA State
            if (marginCharacter.DistrictId != (uint)MxOLocations.LA)
            {
                pak.AddByte(0x01);
            }
            else
            {
                pak.AddByte(0x00);
            }

            pak.AddHexBytes("0301310000b402320000b403380000b403510000000400520000000b00540000000100"); // The Last Part

	        #if DEBUG
            if (isNewCreatedChar)
            {
                Output.WriteLine("Load Background Reply for Created Char:\n" + pak.ReturnFinalPacket());
            }
			#endif

            return pak.ReturnFinalPacket();
            
        }

        public byte[] loadInventory(int charID)
        {
            List<MarginInventoryItem> Inventory = new List<MarginInventoryItem>();
            Inventory = Store.dbManager.MarginDbHandler.LoadInventory(charID);

            DynamicArray din = new DynamicArray();
            foreach (MarginInventoryItem item in Inventory)
            {
                din.append(NumericalUtils.uint16ToByteArrayShort(item.getSlot())); // slot
                din.append(NumericalUtils.uint32ToByteArray(item.getItemID(),1));
                
                din.append(0x00);
                din.append(0x00);
                din.append(0x00); // ToDo : Figure out the bit flags shit thing for amount
                din.append(0x0c); // ToDo : same for purity
                
            }
            return din.getBytes();
        }

        
        public void loadAbilities(int charId)
        {
            Abilities = Store.dbManager.MarginDbHandler.LoadAbilities(charId);
        }


        public byte[] loadKnownAbilities()
        {

            DynamicArray din = new DynamicArray();

            foreach (MarginAbilityItem ability in Abilities)
            {   
                Int32 finalAblityID = ability.getAbilityID() + ability.getLevel();
             
                string debugAbility = StringUtils.bytesToString(NumericalUtils.int32ToByteArray(finalAblityID, 1));
                din.append(NumericalUtils.uint16ToByteArray(ability.getSlot(),1)); // slot
                din.append(NumericalUtils.int32ToByteArray(finalAblityID, 1));
            }

            return din.getBytes();
        }

        public byte[] loadEquippedAbilities()
        {
            DynamicArray din = new DynamicArray();

            //AbilityItem abTemp = AbilityDB.Find(delegate(AbilityItem a) { return a.getAbilityID() == id; });
            

            foreach (MarginAbilityItem ability in Abilities)
            {
                if (ability.getLoaded())
                {
                    Int32 finalAblityID = ability.getAbilityID() + ability.getLevel();
                    din.append(NumericalUtils.uint16ToByteArray(ability.getSlot(), 1)); // slot
                    din.append(NumericalUtils.int32ToByteArray(finalAblityID, 1));
                }
                
            }
            return din.getBytes();
        }

		
        private void loadCharacter(byte[] packet, NetworkStream client, UInt32 theCharId)
        {
            byte[] charIDB = new byte[4];

            if (theCharId == 0)
            {    
                ArrayUtils.copy(packet, 3, charIDB, 0, 4); //Offset for charID is 3 in request
                uint charId = NumericalUtils.ByteArrayToUint32(charIDB, 1);
                // Remove current instances for this character
                if (Store.margin.IsAnotherClientActive(charId))
                {
                    CharacterAlreadyInUseReply(client);
                    return;
                }
                newCharID = charId;
            }


            charIDB = NumericalUtils.uint32ToByteArray(newCharID, 1);
            
			//New harcoded sessionID is: ac 53 02 00
			//Old is: 28 A9 02 00
			string [] marginValues = new string[0x0e];
			marginValues[8]     ="000b10001e000700000001000800000000000a00000003000d00000000001c0000000100";
            marginValues[0xc]   ="000e10000000";
            marginValues[0xd]   ="010f10000000";			
            
            // Some Items
            string itemData = "0170A7000000000010" + "020B8F00000000001803B80200000000001804EE9900000000000005F399000000000000065F0900000000100007C80900000000000008D59D000000000C0009EA040000000000480AD61E0000000000380BF21800000000001C0C66900000000000140DB30400000000001C0E04A80000000018000F3743000000000028108D2200000000002411441A00000000004415808C00000000004462EB0300000000001C63CC5A00000000004464ECB2000000000048651CB3000000000038663E9500000000006868EA0400000000001C";
            byte[] inventory = StringUtils.hexStringToBytes(itemData);

            // Loaded Abilities
            string loadedAbs = "000032080080070000E400800B0000E002800C00006403800E0000340180110032B400801600322C0080290032D404802B0000DC04802F00322800803000323000803E0001E404804200322005805D0000E800805E0032D000805F00327C01806B00008801806F00001C0580990001280580A40000A40080A60000AC0080AC0000EC0080B00000000180B10000040180B80032240180BB0032300180BC00004C0180C50000A00080D80001EC0480DD00012C0580EA0001E80480F400013005800D01013405801D0101680780200132740580280100E004802C01013805803601018C05803801017805803A01325C0780490100840180520100D8088056010124";
            byte[] loadAbilitys = StringUtils.hexStringToBytes(loadedAbs);

            // Known Abilities
            string abilityData = "0000320800800100014800800200017C0080030000080280040000140380050001300480060001B40080070000E4008008000018038009000F9400800A0000E002800B0000E002800C00006403800D00009000800E00003401800F00018C00801000006C0080110032B40080120000E402801300001003801400016400801500322004801600322C0080170001AC09801800012C008019000F9400801A0032C401801B00010800801C00018400801D0000F803801E001A3802801F00010400802000006C0280210001240580220032B40980230032640680240001680080250087DF11842600000400802700001C0080280032540480290032D404802A003210";
            byte[] knownAbilitiys = StringUtils.hexStringToBytes(abilityData);


            // Known Hardlines
            //string hardlineData = "0207000000021f000000022300000002240000000225000000022600000002270000000229000000022b000000022f00000002300000000231000000023200000002340000000235000000023600000002370000000238000000023b000000023f000000024000000002410000000243000000024500000002480000000249000000024a000000024b000000024d000000024e000000024f000000025000000002510000000252000000025400000002550000000256000000025700000002580000000259000000025a000000025b000000025c000000025d000000025e000000025f000000026000000002610000000262000000026300000002640000000265000000026600000002670000000269000000026a000000026b000000026c000000026d000000026e000000026f000000027000000002710000000130000000013100000001370000000141000000014500000001480000000149000000014a000000014b000000014c000000014e000000014f0000000150000000015100000001520000000163000000016400000001650000000166000000016700000001680000000169000000016a000000016b000000016d000000016f00000001700000000171000000017200000001730000000174000000017700000001780000000179000000017a000000017b000000017c000000017d000000017e000000017f000000018000000001810000000182000000018300000001840000000185000000018600000001870000000188000000018a000000018b000000018c000000018d000000018e000000019000000001910000000192000000019300000001940000000195000000019600000001970000000198000000019900000003020000000303000000030400000003050000000306000000030700000003080000000309000000030a000000030b000000030c000000030d000000030e000000030f000000031000000003110000000312000000031300000003140000000315000000031600000003170000000318000000";
            //string hardlineData = Store.dbManager.MarginDbHandler.loadAllHardlines();
            string hardlineData = "0130000000013100000001370000000141000000014500000001480000000149000000014A000000014B000000014C000000014E000000014F00000001500000000151000000015200000001530000000163000000016400000001650000000166000000016700000001680000000169000000016A000000016B000000016D000000016F00000001700000000171000000017200000001730000000174000000017700000001780000000179000000017A000000017B000000017C000000017D000000017E000000017F000000018000000001810000000182000000018300000001840000000185000000018600000001870000000188000000018A000000018B000000018C000000018D000000018E000000018F00000001900000000191000000019200000001930000000194000000019500000001960000000197000000019800000001990000000207000000021F000000022300000002240000000225000000022600000002270000000229000000022B000000022F00000002300000000231000000023200000002340000000235000000023600000002370000000238000000023B000000023F000000024000000002410000000243000000024500000002480000000249000000024A000000024B000000024D000000024E000000024F000000025000000002510000000252000000025400000002550000000256000000025700000002580000000259000000025A000000025B000000025C000000025D000000025E000000025F000000026000000002610000000262000000026300000002640000000265000000026600000002670000000269000000026A000000026B000000026C000000026D000000026E000000026F0000000270000000027100000003020000000303000000030400000003050000000306000000030700000003080000000309000000030A000000030B000000030C000000030D000000030E000000030F00000003100000000311000000031200000003130000000314000000031500000003160000000317000000031800000011010000000E010000000F0100000010020000000C010000000D01000000";
            byte[] knownHardlines = StringUtils.hexStringToBytes(hardlineData);


            // Contacts
            string missionContacts = "0700000001000800000000000a00000003000d00000000001c0000000100";
            byte[] knownContacts = StringUtils.hexStringToBytes(missionContacts);

            byte[] codeArchiveTest = StringUtils.hexStringToBytes("04000000A7010000AC010000CC010000D1010000D60100001302000019020000650200006602000068020000B4020000B5020000B6020000BB020000BC020000C2020000C4020000D5020000D6020000DE020000E2020000E3020000E6020000E7020000E90200003F03000086030000A0030000AA030000E3030000FC030000FE030000030400000604000008040000090400000A040000270400004C0400004E040000BB040000C2040000CE040000EB040000EC040000EE040000F5040000F6040000F7040000F9040000540900005F09000082090000BE090000BF090000C1090000C4090000C7090000C8090000C9090000040A0000080A0000B0120000");
            byte[] codeRecipesFromDecompiledItems = StringUtils.hexStringToBytes("04000000a7010000ac010000cc010000d1010000d60100001302000019020000650200006602000068020000b4020000b5020000b6020000bb020000bc020000c2020000c4020000d5020000d6020000de020000e2020000e3020000e6020000e7020000e90200003f03000086030000a0030000aa030000e3030000fc030000fe030000030400000604000008040000090400000a040000270400004c0400004e040000bb040000c2040000ce040000eb040000ec040000ee040000f5040000f6040000f7040000f9040000540900005f09000082090000be090000bf090000c1090000c4090000c7090000c8090000c9090000040a0000080a0000b0120000fa12000009130000691400008614000095140000e9140000ef140000f11400001c1500002d1500005b150000631500007815000080150000ab15000000160000011600001f1600002416000033160000611600006e160000731600008216000084160000871600009416000000170000241700002517000038170000451700005e1700007317000074170000f4170000f6170000fc1700004f1800005c1800001a1900002d1900003a1900005a1900005b1900006519000066190000121a0000181a0000281a0000341a0000451a0000471a0000621a00006e1a00007b1a00007c1a00007d1a0000e51a0000eb1a0000ec1a0000ed1a0000ee1a0000f01a0000f11a00000c1b00002d1c0000521c0000c31c00003a1d00003f1d0000501d0000521d0000551d0000671d0000d81d0000011e0000041e0000061e0000071e0000091e00000d1e00003b1e0000521e0000751e0000ae1e0000b41e0000c11e0000c21e0000c31e0000c41e0000c51e0000c61e0000c71e0000c81e0000c91e0000ca1e0000cb1e0000cc1e0000cd1e0000ce1e0000cf1e0000d01e0000d11e0000d21e0000d31e0000d41e0000d51e0000d61e0000d71e0000d81e0000da1e0000dd1e0000e61e0000e91e0000f51e0000fc1e0000fd1e0000001f0000031f00007a1f00007c1f0000821f0000a61f0000b91f0000302000003d200000492000005820000082200000832000008420000085200000a5200000be200000c4200000ce200000db200000e7200000eb200000152100007f2100008d210000972100009f210000a7210000b8210000e3210000022200000f2200001222000016220000582200005b2200007022000075220000782200007d2200008922000099220000a4220000ab220000b2220000be220000c9220000d5220000d6220000d72200001b2300001e2300001f23000023230000392300005a230000a7240000c1240000ea240000f924000008250000092500000a25000029250000a9250000cf250000e1250000662600006e260000d2260000b02800005131000070310000f6330000823400004c36000030390000743900009e390000a0390000f63b0000813d0000203e00004b3e0000cd3e0000f73e0000033f00000d3f00006f3f0000164000005b40000069400000f2410000d2420000eb4200002c4300004b430000584300005b4300005e4300009c4300009e4300009f430000b7430000d4430000094400001344000019440000c3440000994d0000804f0000b44f0000b3510000b5510000fb51000005520000b75300009e550000b2550000de5500000156000021560000fc5600006f5700007057000087570000dd570000fc570000105800002658000063580000dd5800000a5900001759000063590000925b0000ea5b0000fe5c000086600000a2640000136b0000306b00004f6f000023700000cc700000ba7100000473000057730000ee7300006e740000a8740000117500009e750000407600006a8800007a8800009388000015890000fc890000a58a0000b38a0000b68a0000b78a0000978b0000ac8b00003b8c0000758c0000a78c0000db8c0000138d0000728d0000548e0000a38e0000e68e0000128f0000f28f0000f88f0000fa8f0000109400005c9600005d960000ee990000ef990000f0990000f1990000f2990000f3990000f4990000f5990000f6990000689a00006a9a00006c9a00006e9a00006f9a00001e9d00004e9d00007b9d00000a9e00007d9e0000c49e0000f49e0000289f00008d9f0000919f0000b29f00001da000003fa0000073a0000097a0000009a100002ea1000045a1000074a10000cea10000d6a10000fba10000fca1000046a2000047a2000049a2000050a20000aca20000bca2000009a3000045a3000054a300005aa30000aba30000b5a3000020a4000021a4000025a4000027a400002ea40000bda500000ea6000011a600002da6000066a60000c1b100007fb20000beb20000cfb20000");
            byte[] empty = new byte[0];

            // New Margin Method to send Data

            // Pre-Load Abilities so that we just need to filter the result later
            loadAbilities((int)newCharID);
            
            SendMarginCharData(empty, 0x01, client, 0, false);
            SendMarginCharData(loadBackgroundInfo((int)newCharID), 0x02, client,0,false);
            SendMarginCharData(empty, 0x03, client,0, false);                            // BuddyList (but old one)
            SendMarginCharData(empty, 0x04, client,0, false);                            // Unknown - no one has data there so ignore it
            SendMarginCharData(loadInventory((int)newCharID), 0x05, client,0, false);            // Inventory
            //sendMarginCharData(StringUtils.hexStringToBytes(itemData), 0x05, client);            // Inventory CR1
            SendMarginCharData(loadEquippedAbilities(), 0x06, client,0, false);          // Loaded Abilitys
            //sendMarginCharData(empty, 0x06, client);                          // Loaded Abilitys CR1
            SendMarginCharData(loadKnownAbilities(), 0x07, client,0, false);             // Known Abilities
            SendMarginCharData(knownHardlines, 0x08, client,0, false);                   // Hardlines
            SendMarginCharData(empty, 0x09, client,0, false);                            // Access Nodes?
            //sendMarginCharData(codeArchiveTest, 0x0a, client);                // Code Storage
            SendMarginCharData(codeRecipesFromDecompiledItems, 0x0a, client,0, false);   // Code Recipes
            SendMarginCharData(knownContacts, 0x0b, client,0, false);                    // Contacts
            SendMarginCharData(empty, 0x0e, client,0, false);

            // MotD has a special handling
			DbParams _dbParams;
			XmlParser.loadDBParams("Config.xml", out _dbParams);
			string motd = _dbParams.Motd;

			if (!motd.Equals("")){ // if MOTD not empty
				DynamicArray announcement = new DynamicArray();
				byte[] currentTime = TimeUtils.getUnixTime();
				//Load motd from file
				byte[] text = StringUtils.stringToBytes(motd);
				byte[] size = NumericalUtils.uint16ToByteArrayShort((UInt16)(currentTime.Length+text.Length));
				
				//announcement.append(size);
				announcement.append(currentTime);
				announcement.append(text);
				
				SendMarginCharData(announcement.getBytes(), 0x0d, client, 10000, true);
			}
			/* End of the MOTD */

        }
		
		public void waitForWorldReply(){
			waitingForWorld = true;
			
			while(waitingForWorld){
				Thread.Sleep(25);
			}
			
		}

        public void EstablishUdpSessionReply(byte[] packet, NetworkStream client){
            Console.WriteLine("Establish UDP Session Reply for CharID:" + newCharID);
            byte[] response = { 0x11, 0x00, 0x00, 0x00, 0x00, 0x01 };
            byte[] encryptedResponse = marginEncr.encrypt(response);
            sendTCPVariableLenPacket(encryptedResponse, client);
        }

        public void CharacterAlreadyInUseReply(NetworkStream client)
        {
            byte[] response = {0x10, 0x25, 0x00, 0x00, 0x0b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00 };
            byte[] encryptedResponse = marginEncr.encrypt(response);
            sendTCPVariableLenPacket(encryptedResponse, client);
        }

        public void EstablishUDPSessionReplyExternal(){
            Console.WriteLine("Establish UDP Session Reply for CharID:" + newCharID);
            byte[] response = { 0x11, 0x00, 0x00, 0x00, 0x00, 0x01 };
            byte[] encryptedResponse = marginEncr.encrypt(response);
            sendTCPVariableLenPacket(encryptedResponse, clientStream);
        }

        public void ConnectChallengeResponse(byte[] packet, NetworkStream client)
        {
            // just accept the packet everytime
            // it seems that it has a 16 byte blob which could be encrypted but dunno if we will ever need it again
            byte[] response = {0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3e, 0x17, 0x26, 0x70, 0x0f, 0x00, 0x03, 0x00, 0x0c, 0x00, 0x07, 0x00, 0xa9, 0x00 };
            byte[] encryptedResponse = marginEncr.encrypt(response);
            sendTCPVariableLenPacket(encryptedResponse, client);
        }

        public void connectChallenge(byte[] packet, NetworkStream client)
        {

            byte[] response = { 0x07, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00, 0x00, 0x01, 0x00 };
            byte[] encryptedResponse = marginEncr.encrypt(response);
            sendTCPVariableLenPacket(encryptedResponse, client);
        }

        public void certConnectReply(byte[] packet, NetworkStream client)
        {
            // Just send the viewData
            byte[] response = { 0x04, 0x00, 0x00, 0x00, 0x00 };
            byte[] encryptedResponse = marginEncr.encrypt(response);
            sendTCPVariableLenPacket(encryptedResponse, client);

        }

        public void certConnectRequest(byte[] packet, NetworkStream client)
        {
            int firstnum = BitConverter.ToInt16(packet, 3);
            int authstart = BitConverter.ToInt16(packet, 5);
            if (firstnum != 3)
            {
                //showMarginDebug("FirstNum is not 3, it is" + firstnum);
            }

            if (authstart != 310)
            {
                //showMarginDebug("AuthStart is not 0x3601");

            }

            // Get the Signature
            byte[] signature= new byte[128];
			
			ArrayUtils.copy(packet,7,signature,0,128);
			            
			// TODO MD5 Signature and verify it with RSA to check if everything is correct

            // Get the signedData
            byte[] signedData = new byte[packet.Length - 135];
            ArrayUtils.copy(packet,135,signedData,0,packet.Length-135);
			int userid = BitConverter.ToInt32(signedData, 1);
			userID = (UInt32)userid;

			
            // Stripout Exponent and Modulus

            byte[] exponent = {0x00, 0x00, 0x00, 0x11};
            byte[] modulus = new byte[96];
            
			ArrayUtils.copy(signedData,82,modulus,0,96);
            
            // Init our encryptor with users modulus and exponent
            marginEncr.setUserPubKey(exponent, modulus);
            // build the final packet, it consists of 1 byte 0x00 + twofish key for world and margin + encryptIV

            byte[] encryptMeResponse = new byte[33];
            encryptMeResponse[0] = 0x00;
			
			ArrayUtils.copy(marginEncr.getTFKey(),0,encryptMeResponse,1,16);
			ArrayUtils.copy(marginEncr.getIV(),0,encryptMeResponse,17,16);
            
            byte[] encryptedShit = { };
            encryptedShit = marginEncr.encryptUsersPublic(encryptMeResponse);

            byte[] blobSize = NumericalUtils.uint16ToByteArray((UInt16)encryptedShit.Length,1);

            byte[] header = { 0x02, 0x03, 0x00 };
            // Write final packet
			
			DynamicArray din = new DynamicArray();
            din.append(header);
			din.append(blobSize);
			din.append(encryptedShit);
            
			sendTCPVariableLenPacket(din.getBytes(), client);
			
        }

        public void sendTCPVariableLenPacket(byte[] packet, NetworkStream client)
        {


            byte[] size = { };
            byte[] bytesize = { };

            if (packet.Length > 127){
				//Do NOT reverse here :P
				bytesize = NumericalUtils.uint16ToByteArray((UInt16)(packet.Length + 0x8000),0);
            }
            else{
				bytesize = NumericalUtils.uint16ToByteArrayShort((ushort)packet.Length);
            }
           
            byte[] finalPacket = new byte[packet.Length+bytesize.Length];

            DynamicArray din = new DynamicArray();
			din.append(bytesize);
			din.append(packet);
			            
            finalPacket = din.getBytes();

            try
            {
                client.Write(finalPacket, 0, finalPacket.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR ex: " + ex);
                throw;
            }
            client.Flush();
        }

		
	}
	
}
