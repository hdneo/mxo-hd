using System;
using System.IO;
namespace hds
{
	public class InitialPacketCreator
	{
		private PacketsUtils packetUtils;
		private WorldDbAccess databaseHandler;
		private DynamicArray din;
		private StringUtils su;
		private NumericalUtils nu;
		
		public InitialPacketCreator (WorldDbAccess databaseHandler, PacketsUtils packetUtils)
		{
			this.packetUtils = packetUtils;
			this.databaseHandler = databaseHandler;
			din = new DynamicArray();
			su = new StringUtils();
			nu = new NumericalUtils();
		}
		
		private byte[] createWorldPacket1(ref ClientData cData){
			TimeUtils tu = new TimeUtils();
			
			//Generate location header
			
			byte[] timeHeader = su.hexStringToBytes("82");

			DynamicArray packet1 = new DynamicArray();
			packet1.append(timeHeader);
			packet1.append(tu.getUnixTime());
			packet1.append(su.stringToBytes("\x04\x01\x00\x00\x3b"));	//Append 04 header (3b RPCS)
			packet1.append(generateLocationHeader(ref cData));
			
			packet1.append(su.hexStringToBytes("0a80e5"));
			long exp = (long)cData.getPlayerValue("exp");
			
			packet1.append(nu.uint32ToByteArray((UInt32)exp,1));
			packet1.append(su.hexStringToBytes("000000000a80e4"));
			long cash = (long)cData.getPlayerValue("cash");
			packet1.append(nu.uint32ToByteArray((UInt32)cash,1));
			
			//THIS PART SHOULD NOT BE HARDCODED
			string packetContent ="000000000880b24e00080008020880b25200100008020880b25400090008020880b24f000b0008020880b251000c0008020880b21100110008021680bc45031100000200000011001100000000000000001680bc450002000002000000cc000400000000000000001680bc45000300000b00000037023300000000000000001680bc45000400002d00000023040300000000000000001680bc45000500002d00000020040300000000000000001680bc45000600002d00000019040100000000000000001680bc45000700002d00000042040100000000000000001680bc45000800003400000083024400000000000000001680bc45000900004c00000015040400000000000000001680bc45000a00004c00000006040400000000000000001680bc45000b00004c00000019040200000000000000001680bc45000c00005300000015040100000000000000001680bc45000d00005300000019040100000000000000000880b23a04000008021680bc45033a0400530000003a040000000000000000001680bc45000e00005300000006040100000000000000001680bc45000f00005f0000007c040500000000000000001680bc45001000005f00000007040500000000000000001680bc45001100006200000006040100000000000000000880b23504000008021680bc45033504006200000035040000000000000000001680bc45001200006200000015040100000000000000001680bc45001300003602000037020a00000000000000001680bc5600140000531d0000b4000500000000000000001680bc5600150000531d000074040100000000000000001680bc5600160000531d000009040500000000000000001680bc5600170000531d000078040100000000000000001680bc5600180000531d0000b5000500000000000000001680bc5600190000689a0000b4000a00000000000000001680bc56001a0000689a00009d000a00000000000000001680bc56001b0000689a00007b040100000000000000001680bc56001c0000689a0000b5000a00000000000000001680bc56001d00002ea1000021040100000000000000001680bc56001e00002ea1000009040a00000000000000001680bc56001f00002ea1000022040100000000000000001680bc56002000002ea100009d000a00000000000000001680bc56002100002ea10000b5000a00000000000000001680bc5600220000d3260000b4000a00000000000000001680bc5600230000d326000009040a00000000000000001680bc5600240000d326000022040100000000000000001680bc5600250000d3260000c3020100000000000000001680bc5600260000d326000076040100000000000000001680bc5600270000d3260000b5000a00000000000000001680bc5600280000b3a90000b4000900000000000000001680bc5600290000b3a9000011040100000000000000001680bc56002a0000b3a9000009040900000000000000001680bc56002b0000b3a9000006040100000000000000001680bc56002c0000b3a90000b5000900000000000000001680bc56002d0000936c000009041000000000000000001680bc56002e0000936c00009d00100000000000000000";
			
			packet1.append(su.hexStringToBytes(packetContent));
			//THIS PART SHOULD NOT BE HARDCODED(END)

					
			return packet1.getBytes();
		}
		
		private byte[] generateLocationHeader(ref ClientData cData){
			
			string districtName = (string)cData.getPlayerValue("district");
			string path;
			
			
			string atlasByte;
			
			if (districtName=="downtown"){
	            path ="downtown/dt_world.metr";
	            atlasByte="02";
			}else{
				if (districtName=="international"){
            		path ="international/it.metr";
            		atlasByte="03";
				}
        		else{
            		path = "slums_barrens_full.metr";
            		atlasByte = "01";
				}
			}

					
			//We need to use atlasByte later too
			
			
			path = "resource/worlds/final_world/"+path+"\x00"; //c-string
			
			int length = path.Length+1;
			int length2 = path.Length+17;
			
			byte[] pathLength = nu.uint16ToByteArray((UInt16)length,1);
			byte[] pathLength2 = nu.uint16ToByteArray((UInt16)length2,1);
			
			string middle = "\x06\x0e\x00\x01\x00\x00\x00\xd8\x68\xc8\x47\x01";
			string environment = "\x08\x00\x53\x61\x74\x69\x53\x6b\x79\x00";
			
			
			string pack = middle + "AAAA" +path+environment; //"AAAA" are just to fill 4bytes syze
			
			int packetLength = pack.Length;
			byte[] packetLengthHex = nu.uint16ToByteArrayShort((UInt16)packetLength);
			
			
			
			din.append(packetLengthHex);	//Append packetLength
			
			din.append(su.stringToBytes(middle));
			din.append(pathLength2);
			din.append(pathLength);
			din.append(su.stringToBytes(path));
			din.append(su.stringToBytes(environment));
			
		
			return din.getBytes();
		}
		

		private byte[] createWorldPacket2(ref ClientData cData){
			// Load just the packet itself
			return su.hexStringToBytes("0204010045123080d708003c00008e2600534f452b4d584f2b566563746f722d486f7374696c652b52756e6e696e6757696c64333035002b80d708003c00008e2100534f452b4d584f2b566563746f722d486f7374696c652b537461726c61696e65002880d708003c00008e1e00534f452b4d584f2b566563746f722d486f7374696c652b4b6164616e61002880d708003c00008e1e00534f452b4d584f2b566563746f722d486f7374696c652b416e74687258002980d708003c00008e1f00534f452b4d584f2b566563746f722d486f7374696c652b4d6564616e6961002c80d708003c00008e2200534f452b4d584f2b566563746f722d486f7374696c652b43616e647953616e6479002880d708003c00008e1e00534f452b4d584f2b566563746f722d486f7374696c652b537465766542002b80d708003c00008e2100534f452b4d584f2b566563746f722d486f7374696c652b41726d696e61746f72002780d708003c00008e1d00534f452b4d584f2b566563746f722d486f7374696c652b466f6e646f002880d708003c00008e1e00534f452b4d584f2b566563746f722d486f7374696c652b72597567656e002880d708003c00008e1e00534f452b4d584f2b566563746f722d486f7374696c652b546175726f6e002f80d708003c00008e2500534f452b4d584f2b566563746f722d486f7374696c652b546865536f6c6964536e616b65002d80d708003c00008e2300534f452b4d584f2b566563746f722d486f7374696c652b57697463686275726e6572002780d708003c00008e1d00534f452b4d584f2b566563746f722d486f7374696c652b53796d6269002a80d708003c00008e2000534f452b4d584f2b566563746f722d486f7374696c652b546f646433343635002980d708003c00008e1f00534f452b4d584f2b566563746f722d486f7374696c652b4d6564616e6f6e002f80d708003c00008e2500534f452b4d584f2b566563746f722d486f7374696c652b5468654176656e4e6967657261002a80d708003c00008e2000534f452b4d584f2b566563746f722d486f7374696c652b656e74696c73617200");
		}
		
		private byte[] createWorldPacket3(ref ClientData cData){
			// Load just the packet itself
			//return su.hexStringToBytes("020403003b061680bc15002f0000f703000008020000000000000000001680bc1500300000f70300000702ecffffff00000000001680bc1500310000f703000050040000000000000000001680bc1500320000f7030000f4030000000000000000001680bc1500330000f70300005104f6ffffff00000000001680bc1500340000f703000052040f00000000000000000041022f47000004010000000000000000000000000000001a0006000000010000000001010000000000800000000000000000242e0700000000000000000000005900002e0000000000000000000000000000000000000000430225808675d40100000000000000000000000000000000000000002100000000002300000000004881a900000700053f00687474703a2f2f7468656d61747269786f6e6c696e652e73746174696f6e2e736f6e642e636f6d2f70726f63657373466c617368547261666669632e766d00");
			return su.hexStringToBytes("020403003b061680bc15002f0000f703000008020000000000000000001680bc1500300000f70300000702ecffffff00000000001680bc1500310000f703000050040000000000000000001680bc1500320000f7030000f4030000000000000000001680bc1500330000f70300005104f6ffffff00000000001680bc1500340000f703000052040f00000000000000000041022f47000004010000000000000000000000000000001a0006000000010000000001010000000000800000000000000000242e0700000000000000000000005900002e0000000000000000000000000000000000000000430225808628A90200000000000000000000000000000000000000002100000000002300000000004881a900000700053f00687474703a2f2f7468656d61747269786f6e6c696e652e73746174696f6e2e736f6e642e636f6d2f70726f63657373466c617368547261666669632e766d00");
		}
		
		private byte[] createWorldPacket4(ref ClientData cData){
			//Create the packet for the player
			DynamicArray rsiPacket = new DynamicArray();
			byte[] rsi = packetUtils.getRSIBytes(cData.getRsiValues());
			
			string firstName = (string)cData.getPlayerValue("firstName");
			firstName = firstName.PadRight(32,'\x00');
			string lastName = (string)cData.getPlayerValue("lastName");
			lastName = lastName.PadRight(32,'\x00');
			string handle = (string)cData.getPlayerValue("handle");
			handle = handle.PadRight(32,'\x00');
			
			double x = Convert.ToDouble((float)cData.getPlayerValue("x"));
			double y = Convert.ToDouble((float)cData.getPlayerValue("y"));
			double z = Convert.ToDouble((float)cData.getPlayerValue("z"));
			
			UInt16 healthC = Convert.ToUInt16((int)cData.getPlayerValue("healthC"));
			UInt16 healthM = Convert.ToUInt16((int)cData.getPlayerValue("healthM"));
			UInt16 innerStrC = Convert.ToUInt16((int)cData.getPlayerValue("innerStrC"));
			UInt16 innerStrM = Convert.ToUInt16((int)cData.getPlayerValue("innerStrM"));
			
			UInt16 profession = Convert.ToUInt16((int)cData.getPlayerValue("profession"));
			UInt16 level = Convert.ToUInt16((int)cData.getPlayerValue("level"));
			
			UInt16 alignment = Convert.ToUInt16((int)cData.getPlayerValue("alignment"));
			
			// Packet composition
			rsiPacket.append(su.hexStringToBytes("82bd0613490301000c0c002fcdab188becff05000000"));
			rsiPacket.append(su.stringToBytes(firstName));
			rsiPacket.append(su.hexStringToBytes("90"));
			rsiPacket.append(su.stringToBytes(lastName));
			rsiPacket.append(su.hexStringToBytes("8098"));
			rsiPacket.append(nu.uint16ToByteArray(healthC,1));
			rsiPacket.append(su.hexStringToBytes("04868cff"));
			rsiPacket.append(nu.uint16ToByteArray(innerStrC,1));
			rsiPacket.append(su.hexStringToBytes("c6c5ff"));
			rsiPacket.append(su.stringToBytes(handle));
			rsiPacket.append(nu.uint16ToByteArray(healthM,1));
			rsiPacket.append(su.hexStringToBytes("ed"));
			
			byte[] profBytes = new byte[1];
			profBytes[0] = nu.uint16ToByteArray(profession,1)[0];
			
			rsiPacket.append(profBytes); //1 byte only
			//rsiPacket.append(su.hexStringToBytes("000000c5ff75d40100"));
			rsiPacket.append(su.hexStringToBytes("000000c5ff28A90200"));
			rsiPacket.append(rsi);
			rsiPacket.append(su.hexStringToBytes("0000"));
			rsiPacket.append(nu.uint16ToByteArray(innerStrM,1));
			rsiPacket.append(su.hexStringToBytes("9d"));
			rsiPacket.append(nu.doubleToByteArray(x,1));
			rsiPacket.append(nu.doubleToByteArray(y,1));
			rsiPacket.append(nu.doubleToByteArray(z,1));
			rsiPacket.append(su.hexStringToBytes("8cff"));
			
			byte[] levelBytes = new byte[1];
			levelBytes[0] = nu.uint16ToByteArray(level,1)[0];
			
			rsiPacket.append(levelBytes); //1 byte only
			rsiPacket.append(su.hexStringToBytes("228088171c"));
			
			byte[] alignmentBytes = new byte[1];
			alignmentBytes[0] = nu.uint16ToByteArray(alignment,1)[0];
			rsiPacket.append(alignmentBytes); //1 byte only
			rsiPacket.append(su.hexStringToBytes("00100000c5ff0200000000"));
			
			////
			string debug="";
			int[] values = cData.getRsiValues();
			for (int i = 0;i<(values.Length);i++){
				debug=debug+values[i]+";";
			}
			
			
			return rsiPacket.getBytes();
			
		}
		
		public byte[] createWorldPacket(ref ClientData cData,int num){
			byte[] result;
			switch(num){
				
				case 0:
					result=createWorldPacket1(ref cData);
					break;
				case 1:
					result=createWorldPacket2(ref cData);
					break;
				case 2:
					result=createWorldPacket3(ref cData);
					break;
				case 3:
					result=createWorldPacket4(ref cData);
					break;
				default:
					result=createWorldPacket1(ref cData);
					break;
			}
			return result;
		}
	}
}

