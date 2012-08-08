using System;
using System.Collections;

namespace hds
{
	public class PacketsUtils
	{
		private StringUtils su;
		private NumericalUtils nu;
		
		public PacketsUtils ()
		{
			su = new StringUtils();
			nu = new NumericalUtils();
		}
		
		
		
		
		public byte[]getRSIBytes(int[] rsiValues){
			
			Hashtable rsiTable = new Hashtable();
			
			string[] keys = {"sex","body","hat","face","shirt","coat","pants","shoes","gloves","glasses","hair","facialdetail","shirtcolor","pantscolor","coatcolor","shoecolor","glassescolor","haircolor","skintone","tatto","facialdetailcolor","leggins"};
			
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
			
			binaryRSI +=nu.intToBitsString((int)rsiTable["sex"],1);
			binaryRSI +=nu.intToBitsString((int)rsiTable["body"],2);
			binaryRSI +=nu.intToBitsString((int)rsiTable["hat"],6);
			binaryRSI +=nu.intToBitsString((int)rsiTable["face"],5);
			binaryRSI +=unknown1;
			binaryRSI +=nu.intToBitsString((int)rsiTable["shirt"],5);
			
			
			if (rsiValues[0]==0){// Male part
				binaryRSI +=nu.intToBitsString((int)rsiTable["coat"],6);	
				binaryRSI +=nu.intToBitsString((int)rsiTable["pants"],5);	
				binaryRSI +=nu.intToBitsString((int)rsiTable["shoes"],6);
				binaryRSI +=nu.intToBitsString((int)rsiTable["gloves"],5);
				binaryRSI +=nu.intToBitsString((int)rsiTable["glasses"],5);
				binaryRSI +=nu.intToBitsString((int)rsiTable["hair"],5);
				binaryRSI +=nu.intToBitsString((int)rsiTable["facialdetail"],4);
				binaryRSI +=nu.intToBitsString((int)rsiTable["shirtcolor"],6);
				binaryRSI +=nu.intToBitsString((int)rsiTable["pantscolor"],5);
				binaryRSI +=nu.intToBitsString((int)rsiTable["coatcolor"],5);
				binaryRSI +=nu.intToBitsString((int)rsiTable["shoecolor"],4);
				binaryRSI +=nu.intToBitsString((int)rsiTable["glassescolor"],4);
				binaryRSI +=nu.intToBitsString((int)rsiTable["haircolor"],5);
				binaryRSI +=nu.intToBitsString((int)rsiTable["skintone"],5);
				binaryRSI +=unknown2;
				binaryRSI +=nu.intToBitsString((int)rsiTable["tatto"],3);//yup, i know, i knooooow
				binaryRSI +=nu.intToBitsString((int)rsiTable["facialdetailcolor"],3);
				binaryRSI +="000000";
			}
			else{// Female part
				binaryRSI +=nu.intToBitsString((int)rsiTable["coat"],5);	
				binaryRSI +=nu.intToBitsString((int)rsiTable["pants"],5);	
				binaryRSI +=nu.intToBitsString((int)rsiTable["shoes"],5);
				binaryRSI +=nu.intToBitsString((int)rsiTable["gloves"],6);
				binaryRSI +=nu.intToBitsString((int)rsiTable["glasses"],5);
				binaryRSI +=nu.intToBitsString((int)rsiTable["hair"],5);
				binaryRSI +=nu.intToBitsString((int)rsiTable["leggins"],4);
				binaryRSI +=nu.intToBitsString((int)rsiTable["facialdetail"],4);
				binaryRSI +=nu.intToBitsString((int)rsiTable["shirtcolor"],6);
				binaryRSI +=nu.intToBitsString((int)rsiTable["pantscolor"],5);
				binaryRSI +=nu.intToBitsString((int)rsiTable["coatcolor"],5);
				binaryRSI +=nu.intToBitsString((int)rsiTable["shoecolor"],4);
				binaryRSI +=nu.intToBitsString((int)rsiTable["glassescolor"],4);
				binaryRSI +=nu.intToBitsString((int)rsiTable["haircolor"],5);
				binaryRSI +=nu.intToBitsString((int)rsiTable["skintone"],5);
				binaryRSI +=unknown2;
				binaryRSI +=nu.intToBitsString((int)rsiTable["tatto"],3); //yup, i know, i knooooow
				binaryRSI +=nu.intToBitsString((int)rsiTable["facialdetailcolor"],3);
				binaryRSI +="000";
			}
			
			
			return nu.bigBinaryStringToBytes(binaryRSI);
		}
		
		
		public void incrementRpcCounter(ref ClientData cData){
			
			UInt16 rpcCounter = cData.getRPCCounter();
			rpcCounter++;
			cData.setRPCCounter(rpcCounter);
		}
		
		
		public byte[] getRpcBytes(ref ClientData cData){
			
			UInt16 rpcCounter = cData.getRPCCounter();
			byte [] rpc = nu.uint16ToByteArray(rpcCounter,0);
			return rpc;
		}
		
		public byte[] createRpcPacket(ref ClientData cData,byte[] rpcContent){
			
			byte [] currentCounter =  getRpcBytes(ref cData);
			byte [] header = {0x02,0x04,0x01};
			
			DynamicArray din = new DynamicArray();
			
			din.append(header);
			din.append(currentCounter);
			din.append(rpcContent);
			
			incrementRpcCounter(ref cData);
			
			return din.getBytes();
		}
		
		
		public byte[] createVendorPacket(ref ClientData cData,int id){
			//Create
			
			byte[] content = su.hexStringToBytes("014a810d7cadd94300000000a0b7f6c000000000003c9440000000000051c8c020000a0000140080002000800018008000040080002c008000f41180009806800090078000c4068000640680");
			byte[] packet = createRpcPacket(ref cData, content);
					
			return packet;
		}
		
		public byte[] createTeleportPacket(ref ClientData cData,int x, int y, int z){
			//Create
			
			NumericalUtils nu = new NumericalUtils();
			StringUtils su = new StringUtils();
			
			
			// Adjust coords between human view and mxo format
			x = x*100;
			y = y*100;
			z = z*100;
			// End adjust
			
			byte[] xHex = nu.floatToByteArray(x+0.0f,1);
			byte[] yHex = nu.floatToByteArray(y+0.0f,1);
			byte[] zHex = nu.floatToByteArray(z+0.0f,1);
			
			
			byte[] header = su.hexStringToBytes("02030200011c11f7025c00030000");
			byte[] tail = su.hexStringToBytes("000000000000");

			DynamicArray din = new DynamicArray();
			
			din.append(header);
			din.append(xHex);
			din.append(yHex);
			din.append(zHex);
			din.append(tail);
			
			byte[] packet = din.getBytes();
			
			return packet;
		}
		
		public byte[] createSystemMessage(ref ClientData cData,string message){
			StringUtils su = new StringUtils();
			NumericalUtils nu = new NumericalUtils();
			
			byte[] messageBytes = su.stringToBytes(message);
			
			//Increment counter :D
			
			byte [] rpcInside = {0x01};
			
			DynamicArray din = new DynamicArray();
			
			
			din.append(rpcInside);
			
			UInt16 packetSize = (UInt16) (message.Length+1+38);
			UInt16 messageSize = (UInt16) (message.Length+1);
			
			byte[] packetSizeHex = nu.uint16ToByteArrayShort(packetSize);
			byte[] messageSizeHex = nu.uint16ToByteArray(messageSize,1);
			byte[] hexContents = {0x2e,0x07,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x24,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00};
			byte[] tail = {0x00};
			
			din.append(packetSizeHex);
			din.append(hexContents);
			din.append(messageSizeHex);
			din.append(messageBytes);
			din.append(tail);
	
			byte[] finalPacket = createRpcPacket(ref cData, din.getBytes());
			
			return finalPacket;
		}
		
	}
}

