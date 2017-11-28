/// <summary>
/// Hardline Dreams Team - 2010
/// Created by Neo
/// Modified by Morpheus
/// </summary>
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using hds.shared;

namespace hds{

	public class MarginClient{
		
		private int hashID;
		private TcpClient tcpClient;
		private bool working;
		private bool waitingForWorld;
		private UInt32 userID;
        private UInt32 newCharID;
        private UInt32 loadCharCounter = 0;
        private bool isNewCreatedChar = false;
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
            return this.newCharID;
        }
		
		public bool isWorking(){
			return working;
		}
		
		public void passiveClose(){
			this.working = false;
		}
		
		public void forceClose(){
			this.tcpClient.Close();
			this.clientStream.Close();
		}
		
		public bool isYourClientWaiting(UInt32 waitingID){
			if (this.userID==waitingID){
				this.waitingForWorld = false;
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
  			Output.WriteLine("Margin thread closing");
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

            if (encrypted == true)
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



        private void sendMarginCharData(byte[] data, byte opcode, NetworkStream client)
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

            byte[] header = { 0x10, 0x00, 0x00, 0x00 };
            // Raise one up
            loadCharCounter++;

            // Add the counter
            byte[] counterByte = new byte[4];
            counterByte = NumericalUtils.uint32ToByteArray(loadCharCounter,1);

            // get the datasize for the packet 
            byte[] dataSize = NumericalUtils.uint16ToByteArray((UInt16)data.Length,1);

            // charId
            byte[] charID = NumericalUtils.uint32ToByteArray(this.newCharID, 0);

            // viewData code 
            byte[] responseCode = {0x00, opcode };

            string pakData = StringUtils.bytesToString_NS(header) +
                                StringUtils.bytesToString_NS(charID) +
                                StringUtils.bytesToString_NS(counterByte) +
                                StringUtils.bytesToString_NS(responseCode);

            string hasData;

            // this needs improvement...
            if (data.Length == 0 && opcode==0x01)
            {
                hasData = "0000";
            }
            else
            {
                hasData = "1000" + StringUtils.bytesToString_NS(dataSize) + StringUtils.bytesToString_NS(data);
            }

            pakData += hasData;
            PacketContent pak = new PacketContent();
            
            // Dump the Packet Data to see whats in
            //Console.WriteLine("Response Load Pak : " + pakData);

            // Do the viewData 
            byte[] response = StringUtils.hexStringToBytes(pakData);
            Output.WritePacketLog(response, "MARGINSERVER", "0", "0", "0");
            Output.WriteDebugLog("[MARGIN SERVER RESPONSE] for OPCODE " + opcode + " : " + StringUtils.bytesToString(response)); 
            byte[] encryptedResponse = marginEncr.encrypt(response);
            sendTCPVariableLenPacket(encryptedResponse, client);
            System.Threading.Thread.Sleep(50);
        }

		private void sendMarginData(string data,NetworkStream client){
			byte[] response = StringUtils.hexStringToBytes(data);
            byte[] encryptedResponse = marginEncr.encrypt(response);
            sendTCPVariableLenPacket(encryptedResponse, client);
            System.Threading.Thread.Sleep(50);
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
	        reader.incrementOffsetByValue(3);

            
	        UInt16 body = 0;
	        UInt16 gender = 0;

	        // ToDo: if you know struct - replace the "setOffsetOverrideValues" 
	        UInt16 skintone = reader.readUInt16(1);
	        reader.setOffsetOverrideValue(7);
	        UInt16 bodyTypeId = reader.readUInt16(1);
	        reader.setOffsetOverrideValue(15);
	        UInt16 hairId = reader.readUInt16(1);
	        reader.setOffsetOverrideValue(19);
	        UInt16 haircolor = reader.readUInt16(1);
	        reader.setOffsetOverrideValue(23);
	        UInt16 tattoo = reader.readUInt16(1);
	        reader.setOffsetOverrideValue(27);
	        UInt16 headId = reader.readUInt16(1);
	        reader.setOffsetOverrideValue(31);
	        UInt16 facialDetail = reader.readUInt16(1);
	        reader.setOffsetOverrideValue(35);
	        UInt16 facialDetailColor = reader.readUInt16(1);
	        reader.setOffsetOverrideValue(67);
	        UInt16 profession = reader.readUInt16(1);
	        
	        
            // lets read the values
            // the IDs for the Appeareance is always uint16 goID
	        // Extra Hint: there are no leggins in Char Creation Process
	        reader.setOffsetOverrideValue(35);
	        UInt16 hatId = reader.readUInt16(1);
	        reader.setOffsetOverrideValue(39);
	        UInt16 eyewearId = reader.readUInt16(1);
	        reader.setOffsetOverrideValue(43);
	        UInt16 shirtId = reader.readUInt16(1);
	        reader.setOffsetOverrideValue(47);
	        UInt16 glovesId = reader.readUInt16(1);
	        reader.setOffsetOverrideValue(51);
	        UInt16 outerwearId = reader.readUInt16(1);
	        reader.setOffsetOverrideValue(55);
	        UInt16 pantsId = reader.readUInt16(1);
	        reader.setOffsetOverrideValue(63);
	        UInt16 footwearId = reader.readUInt16(1);


            // Get Values by "NewRSI" IDs
            NewRSIItem hairItem = itemLoader.getNewRSIItemByTypeAndID("HAIR", hairId);
            NewRSIItem bodyItem = itemLoader.getNewRSIItemByTypeAndID("BODY", (ushort)bodyTypeId);
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
            loadCharacter(packet, client, this.newCharID);
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
			
			int size = (int) NumericalUtils.ByteArrayToUint16(handleSize,1);
			
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
				nameRequestResponse += StringUtils.bytesToString_NS(NumericalUtils.uint32ToByteArray((UInt32)dbResult,1));
				nameRequestResponse += "0000000000";
				nameRequestResponse += StringUtils.bytesToString_NS(handleSize);
				nameRequestResponse += handleBStr+"00";
			
			}

            Output.writeToLogForConsole(nameRequestResponse);
			sendMarginData(nameRequestResponse,client);
			
		}

        private byte[] loadBackgroundInfo(int charID){

            MarginCharacter marginCharacter = Store.dbManager.MarginDbHandler.getCharInfo(charID);


            PacketContent pak = new PacketContent();
            if (!isNewCreatedChar)
            {
                pak.addByteArray(StringUtils.hexStringToBytes("01000000640000007B0000006500000000000000000000006C000000000000002B010000"));
            }
            pak.addStringWithFixedSized(marginCharacter.firstname,32);
            pak.addStringWithFixedSized(marginCharacter.lastname,32);
            pak.addStringWithFixedSized(marginCharacter.background,1024);


            // ToDo: Analyse it and (Rep zion => 82, Rep Machine 2, RepMero 81
            //pak.addHexBytes("000000000000000000178604E40008AF2F0175020000A39F714A81FF81FF81FF670067006700000003000301310000B402320000B403380000B4044E0000000200510000001600520000001900540000001300");
            pak.addUint32(marginCharacter.exp,1);
            pak.addUint32(marginCharacter.cash,1);
            pak.addUintShort(0x01); // Flag - always 1
            pak.addUint32(200,1); // CQ Points
            pak.addByteArray(StringUtils.hexStringToBytes("875D714A")); // Last Changed Timestamp - current its static - needs to be dynamic

            // Rputation Bytes (int16[7])
            pak.addInt16(-116,1);
            pak.addInt16(-117, 1);
            pak.addInt16(-20, 1);
            pak.addInt16(-59, 1);
            pak.addInt16(-58, 1);
            pak.addInt16(-57, 1);
            pak.addInt16(2, 1);
            pak.addByte(0x03);
            
            // 01 = onWorld , 00 = LA . In World you have to take care to not spawn the Character in LA State
            if (marginCharacter.districtId != 0)
            {
                pak.addByte(0x01);
            }
            else
            {
                pak.addByte(0x00);
            }

            pak.addHexBytes("0301310000b402320000b403380000b403510000000400520000000b00540000000100"); // The Last Part

	        #if DEBUG
            if (isNewCreatedChar == true)
            {
                Output.WriteLine("Load Background Reply for Created Char:\n" + pak.returnFinalPacket());
            }
			#endif

            return pak.returnFinalPacket();
            
        }

        public byte[] loadInventory(int charID)
        {
            List<MarginInventoryItem> Inventory = new List<MarginInventoryItem>();
            Inventory = Store.dbManager.MarginDbHandler.loadInventory(charID);

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
            this.Abilities = Store.dbManager.MarginDbHandler.loadAbilities(charId);
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
                if (ability.getLoaded() == true)
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
                uint charId = (uint)NumericalUtils.ByteArrayToUint32(charIDB, 1);
                // Remove current instances for this character
                if (Store.margin.isAnotherClientActive(charId) == true)
                {
                    CharacterAlreadyInUseReply(client);
                    return;
                }
                this.newCharID = charId;
            }


            charIDB = NumericalUtils.uint32ToByteArray(this.newCharID, 1);
            
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
            string hardlineData = "0207000000021f000000022300000002240000000225000000022600000002270000000229000000022b000000022f00000002300000000231000000023200000002340000000235000000023600000002370000000238000000023b000000023f000000024000000002410000000243000000024500000002480000000249000000024a000000024b000000024d000000024e000000024f000000025000000002510000000252000000025400000002550000000256000000025700000002580000000259000000025a000000025b000000025c000000025d000000025e000000025f000000026000000002610000000262000000026300000002640000000265000000026600000002670000000269000000026a000000026b000000026c000000026d000000026e000000026f000000027000000002710000000130000000013100000001370000000141000000014500000001480000000149000000014a000000014b000000014c000000014e000000014f0000000150000000015100000001520000000163000000016400000001650000000166000000016700000001680000000169000000016a000000016b000000016d000000016f00000001700000000171000000017200000001730000000174000000017700000001780000000179000000017a000000017b000000017c000000017d000000017e000000017f000000018000000001810000000182000000018300000001840000000185000000018600000001870000000188000000018a000000018b000000018c000000018d000000018e000000019000000001910000000192000000019300000001940000000195000000019600000001970000000198000000019900000003020000000303000000030400000003050000000306000000030700000003080000000309000000030a000000030b000000030c000000030d000000030e000000030f000000031000000003110000000312000000031300000003140000000315000000031600000003170000000318000000";
            //string hardlineData = Store.dbManager.MarginDbHandler.loadAllHardlines();
            byte[] knownHardlines = StringUtils.hexStringToBytes(hardlineData);


            // Contacts
            string missionContacts = "0700000001000800000000000a00000003000d00000000001c0000000100";
            byte[] knownContacts = StringUtils.hexStringToBytes(missionContacts);

            byte[] codeArchiveTest = StringUtils.hexStringToBytes("04000000A7010000AC010000CC010000D1010000D60100001302000019020000650200006602000068020000B4020000B5020000B6020000BB020000BC020000C2020000C4020000D5020000D6020000DE020000E2020000E3020000E6020000E7020000E90200003F03000086030000A0030000AA030000E3030000FC030000FE030000030400000604000008040000090400000A040000270400004C0400004E040000BB040000C2040000CE040000EB040000EC040000EE040000F5040000F6040000F7040000F9040000540900005F09000082090000BE090000BF090000C1090000C4090000C7090000C8090000C9090000040A0000080A0000B0120000");

            byte[] empty = new byte[0];

            // New Margin Method to send Data

            // Pre-Load Abilities so that we just need to filter the result later
            loadAbilities((int)this.newCharID);

            sendMarginCharData(empty, 0x01, client);
            sendMarginCharData(loadBackgroundInfo((int)this.newCharID), 0x02, client);
            sendMarginCharData(empty, 0x03, client);                            // BuddyList (but old one)
            sendMarginCharData(empty, 0x04, client);                            // Unknown - no one has data there so ignore it
            sendMarginCharData(loadInventory((int)this.newCharID), 0x05, client);            // Inventory
            //sendMarginCharData(StringUtils.hexStringToBytes(itemData), 0x05, client);            // Inventory CR1
            sendMarginCharData(loadEquippedAbilities(), 0x06, client);          // Loaded Abilitys
            //sendMarginCharData(empty, 0x06, client);                          // Loaded Abilitys CR1
            sendMarginCharData(loadKnownAbilities(), 0x07, client);             // Known Abilities
            sendMarginCharData(knownHardlines, 0x08, client);                   // Hardlines
            sendMarginCharData(empty, 0x09, client);                            // Access Nodes?
            //sendMarginCharData(codeArchiveTest, 0x0a, client);                // Code Storage
            sendMarginCharData(empty, 0x0a, client);                            // Code Storage CR1
            sendMarginCharData(knownContacts, 0x0b, client);                    // Contacts
            sendMarginCharData(empty, 0x0e, client);

            // ModT (has a seperate format)
            sendMarginData("1000000000" + StringUtils.bytesToString_NS(charIDB) + "10270d010f10000000", client);
			
			/* This is the server MOTD announcement (its disabled now) */
			
			/*DynamicArray announcement = new DynamicArray();
			byte[] header = {0x10,0x00,0x00,0x00,0x00,0xac,0x53,0x02,0x00,0x10,0x27,0x0E,0x01,0x0D,0x10,0x00};
			TimeUtils tim = new TimeUtils();
			byte[] currentTime = tim.getUnixTime();
			//Load motd from file
			XmlParser xmlP = new XmlParser();
			string[] data=xmlP.loadDBParams("Config.xml");
			string motd=data[5];
			
			if (!motd.Equals("")){ // if MOTD not empty
				byte[] text = StringUtils.stringToBytes(motd);
				byte[] size = NumericalUtils.uint16ToByteArray((UInt16)(currentTime.Length+text.Length),1);
				
				announcement.append(header);
				announcement.append(size);
				announcement.append(currentTime);
				announcement.append(text);
				          
	            byte[] encryptedResponse14 = marginEncr.encrypt(announcement.getBytes());
	           sendTCPVariableLenPacket(encryptedResponse14, client);
			}*/
			/* End of the MOTD */
			
            //Output.WriteLine("[MARGIN] UserID "+this.userID+" waiting for session reply from world");
			//waitForWorldReply(); //TODO: enabling this outputted the "no usage" bug
			//Output.WriteLine("[MARGIN] World Session Reply OK for UserID "+this.userID);
			
			//System.Threading.Thread.Sleep(1000);
            //EstablishUDPSessionReply(packet, client);
            
        }
		
		public void waitForWorldReply(){
			this.waitingForWorld = true;
			
			while(this.waitingForWorld){
				Thread.Sleep(25);
			}
			
		}

        public void EstablishUdpSessionReply(byte[] packet, NetworkStream client){
            Console.WriteLine("Establish UDP Session Reply for CharID:" + newCharID);
            byte[] response = { 0x11, 0x00, 0x00, 0x00, 0x00 };
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
            byte[] response = { 0x11, 0x00, 0x00, 0x00, 0x00 };
            byte[] encryptedResponse = marginEncr.encrypt(response);
            sendTCPVariableLenPacket(encryptedResponse, this.clientStream);
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
			this.userID = (UInt32)userid;

			
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
                Console.WriteLine("ERROR ex: " + ex.ToString());
                throw;
            }
            client.Flush();
        }

		
	}
	
}
