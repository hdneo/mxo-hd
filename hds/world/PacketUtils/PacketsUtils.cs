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



        public static byte[] createRpcPacket(ArrayList contents, bool timedRPC, WorldClient client)
        {
			
			RPCPacket rpcp = new RPCPacket(client);
			
			for (int i = 0;i<contents.Count;i++){
				rpcp.appendMsgBlock((byte[])contents[i]);
			}
					
			return rpcp.getBytesWithHeader(timedRPC);
		}
		
		public static byte[] createBigRpcPacket(ArrayList[] contents,bool timedRPC,WorldClient client){
			RPCPacket rpcp = new RPCPacket(client);
			
			// Max size is 4
			
			for (int i = 0; i<contents.Length;i++){
				RPCPacket temp = new RPCPacket(client);
				for(int j = 0;j<contents[i].Count;j++){
					temp.appendMsgBlock((byte[])contents[i][j]);
				}
				rpcp.appendRpc(temp);
			}
			
			return rpcp.getBytesWithHeader(timedRPC);
			
		}

        public static byte[] createVendorPacket(int id, WorldClient client)
        {
			//ToDo: Near check as it should contain the size if i am right
			
			ArrayList content = new ArrayList();
			content.Add(StringUtils.hexStringToBytes("4a810d7cadd94300000000a0b7f6c000000000003c9440000000000051c8c020000a0000140080002000800018008000040080002c008000f41180009806800090078000c4068000640680"));
            byte[] packet = createRpcPacket(content, false, client);
					
			return packet;
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
            packet.addUintShort((UInt16)RPCResponseHeaders.SERVER_CHAT_MESSAGE_RESPONSE);
            packet.addHexBytes("0700000000000000000024000000000000000000000000000000000000000000000000");
            packet.addUint16(messageSize, 1);
            packet.addByteArray(messageBytes);

            return packet.returnFinalPacket();
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

