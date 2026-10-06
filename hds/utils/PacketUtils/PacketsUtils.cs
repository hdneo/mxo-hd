using System;
using System.Collections;

namespace hds{


	public class PacketsUtils{
		
		public static byte[]getRSIBytes(int[] rsiValues){
			
			Hashtable rsiTable = new Hashtable();
			
			string[] keys = {"sex","body","hat","face","shirt","coat","pants","shoes","gloves","glasses","hair","facialdetail","shirtcolor","pantscolor","coatcolor","shoecolor","glassescolor","haircolor","skintone","tattoo","facialdetailcolor","leggins"};
			
			for (int i = 0;i<keys.Length;i++){
				rsiTable[keys[i]] = rsiValues[i];	
			}
			
			/* ORDER:
			 * sex body	hat face shirt coat	pants shoes	gloves glasses 
			 * hair	facialdetail shirtcolor pantscolor coatcolor shoecolor 
			 * glassescolor haircolor skintone tatto facialdetailcolor leggins */
			
			string unknown1 = "0";
			string unknown2 = "00";
			
			string binaryRSI = "";
			
			// Common part
			
			binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["sex"],1);
			binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["body"],2);
			binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["hat"],6);
			binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["face"],5);
			binaryRSI +=unknown1;
			binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["shirt"],5);
			
			
			if (rsiValues[0]==0){// Male part
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["coat"],6);	
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["pants"],5);	
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["shoes"],6);
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["gloves"],5);
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["glasses"],5);
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["hair"],5);
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["facialdetail"],4);
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["shirtcolor"],6);
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["pantscolor"],5);
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["coatcolor"],5);
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["shoecolor"],4);
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["glassescolor"],4);
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["haircolor"],5);
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["skintone"],5);
				binaryRSI +=unknown2;
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["tattoo"],3);
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["facialdetailcolor"],3);
				binaryRSI +="000000";
			}
			else{// Female part
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["coat"],5);	
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["pants"],5);	
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["shoes"],5);
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["gloves"],6);
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["glasses"],5);
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["hair"],5);
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["leggins"],4);
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["facialdetail"],4);
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["shirtcolor"],6);
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["pantscolor"],5);
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["coatcolor"],5);
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["shoecolor"],4);
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["glassescolor"],4);
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["haircolor"],5);
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["skintone"],5);
				binaryRSI +=unknown2;
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["tattoo"],3); 
				binaryRSI +=NumericalUtils.intToBitsString((int)rsiTable["facialdetailcolor"],3);
				binaryRSI +="000";
			}
			
			
			return NumericalUtils.bigBinaryStringToBytes(binaryRSI);
		}




		
		public static byte[] createTeleportPacket(int x, int y, int z){
			
			// Adjust coords between human view and mxo format
			x = x*100;
			y = y*100;
			z = z*100;
			// End adjust
			
			byte[] xHex = NumericalUtils.floatToByteArray(x+0.0f,1);
			byte[] yHex = NumericalUtils.floatToByteArray(y+0.0f,1);
			byte[] zHex = NumericalUtils.floatToByteArray(z+0.0f,1);
			
			
			byte[] header = StringUtils.hexStringToBytes("0200011c11f7025c00030000");
			byte[] tail = StringUtils.hexStringToBytes("0000");

			DynamicArray din = new DynamicArray();
			
			din.append(header);
			din.append(xHex);
			din.append(yHex);
			din.append(zHex);
			din.append(tail);
			
			return din.getBytes();
		}
		

       

        public static byte[] createSystemMessageWithoutRPC(string message)
        {
            byte[] messageBytes = StringUtils.stringToBytes(message + "\x00");

            
            UInt16 messageSize = (UInt16)(message.Length + 1);

            
            PacketContent packet = new PacketContent();
            packet.AddUShort((UInt16)RPCResponseHeaders.SERVER_CHAT_MESSAGE_RESPONSE);
            packet.AddHexBytes("0700000000000000000024000000000000000000000000000000000000000000000000");
            packet.AddUint16(messageSize, 1);
            packet.AddByteArray(messageBytes);

            return packet.ReturnFinalPacket();
        }

		public static byte[] createSystemMessage(string message, WorldClient client){

			
			byte[] messageBytes = StringUtils.stringToBytes(message+"\x00");
					
			DynamicArray din = new DynamicArray();
					
			UInt16 messageSize = (UInt16) (message.Length+1);			
			byte[] messageSizeHex = NumericalUtils.uint16ToByteArray(messageSize,1);

            byte[] hexContents = StringUtils.hexStringToBytes("0700000000000000000024000000000000000000000000000000000000000000000000");
            
            din.append((byte)RPCResponseHeaders.SERVER_CHAT_MESSAGE_RESPONSE);
			din.append(hexContents);
			din.append(messageSizeHex);
			din.append(messageBytes);
				
			
			return din.getBytes();
		}


        public static byte[] createMessage(string message, string type, WorldClient client){
            
            byte typeByte;
            switch (type){

                case "TEAM":
                    typeByte = 0x05;
                    break;
                case "CREW":
                    typeByte = 0x02;
                    break;

                case "FACTION":
                    typeByte = 0x03;
                    break;
                case "SYSTEM":
                    typeByte = 0x07;
                    break;
                case "MODAL":
                    typeByte = 0x17;
                    break;

                case "FRAMEMODAL":
                    typeByte = 0xd7;
                    break;

                case "BROADCAST":
                    typeByte = 0xc7;
                    break;
                case "AREA":
                    typeByte = 0x10;
                    break;
                default:
                    typeByte = 0x07;
                    break;
            }

            byte[] messageBytes = StringUtils.stringToBytes(message + "\x00");

            DynamicArray din = new DynamicArray();

            UInt16 messageSize = (UInt16)(message.Length + 1);

            byte[] messageSizeHex = NumericalUtils.uint16ToByteArray(messageSize, 1);
            byte[] hexContents = StringUtils.hexStringToBytes("00000000000000000024000000000000000000000000000000000000000000000000");



            din.append((byte)RPCResponseHeaders.SERVER_CHAT_MESSAGE_RESPONSE); // Response header
            din.append(typeByte);
            din.append(hexContents);
            din.append(messageSizeHex);
            din.append(messageBytes);


            return din.getBytes();
        }
		
	}
}

