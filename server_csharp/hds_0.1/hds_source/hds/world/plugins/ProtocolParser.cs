using System;
namespace hds
{
	public class ProtocolParser
	{
		private ArrayUtils au;
		private StringUtils su;
		
		/* We keep a list of the same plugins here */
		private const int PLUGIN_INITIAL_LOADER = 0; // Plugin ID 0
		private const int PLUGIN_VENDOR=1; // Plugin for vendors
		private const int PLUGIN_CHATSELF=2; // Plugin for self chat
		private const int PLUGIN_NONE = 3; // This is the "just say 02" plugin
		private const int PLUGIN_ECHO = 4; // This is the "just-echo" plugin
		
		/* End of the list */
		
		public ProtocolParser(){
			au = new ArrayUtils();
			su = new StringUtils();
		}
		
		public int detectNeededPlugin (ref byte[] packetData)
		{
			// Here we need to get if it's one 03 or 04 protocol
						
			if (packetData.Length<=5){
				if (packetData[0]==0x00){
					return PLUGIN_ECHO; // 0 will mean not encrypted, so echo it
				}
				
				if (packetData[0]==0x42){
					Console.WriteLine("RPC counter reset DETECTED");
				}
				return PLUGIN_NONE; // If too short, it's better to ack for now
			}
			
			byte pos1 = packetData[0];
			byte pos2 = packetData[1];
			byte pos3 = packetData[5];
			
			// 03 protocol handling here
			
			if ((pos1==0x02 && pos2 == 0x03) || (pos1==0x82 && pos3 == 0x03))
			{
				return detect03Plugin(ref packetData);
			}
			
			// 04 protocol handling here
			
			if (pos1==0x02 && pos2 == 0x04){
				return detect04Plugin(ref packetData,2); //Offset is 2
			}else {
				if(pos1==0x82 && pos3 == 0x04)
					return detect04Plugin(ref packetData,6); //Offset is 6
			}
			
			return PLUGIN_NONE; // In case that either plugin detects anything
		}
		
		
		
		private int detect03Plugin(ref byte[] packetData){
			
			//CHAT PACKET: 02 03 02 00 01 02 73 00 00 
			// 04 01 00 54 01 1d 28 10 00 08 00 00 00 00 13 00 3f 74 65 6c 65 70 6f 72 74 20 32 30 20 32 30 20 32 30 00 
			
			/* DOING A TRICK (03 self chat packet to 04 RPC) */
				byte[] temp = new byte[7];
				au.copy(temp,0,packetData,0,7);
				byte[] chatHeader = {0x02,0x03,0x02,0x00,0x01,0x02,0x73};
			
				if(au.equal(temp,chatHeader) && packetData.Length>12){ // if it's a self chat packet DUDE!
					Console.WriteLine("[03 Parser] PATCHING 03 PACKET TO SELF CHAT RPC PROCESS");
					byte[] change = new byte[1+packetData.Length-9];
					change[0]=0x02;
					au.copy(change,1,packetData,9,packetData.Length-9);
					packetData = change;
					return PLUGIN_CHATSELF;
				}
				
			/* TRICK DONE */
			
			return PLUGIN_NONE; // Need more work here :P
		}
		
		private int detect04Plugin(ref byte[] packetData,int headerOffset){
			
			// Here we supose that client didnt packet more than 1 RPC request (ideal thing of course)
			
			
			byte headerA = packetData[headerOffset+5];	
			byte headerB = packetData[headerOffset+6];
			
			if (headerA==0x80 && headerB==0xc8){ //80c8 -> vendor
				byte[] objectClickId = new byte[4];
				au.copy(objectClickId,0,packetData,headerOffset+7,4);
				string id = su.bytesToString_NS(objectClickId);				
				Console.WriteLine("Clicked object {0}",id);
				
				byte typeA = packetData[headerOffset+11];	
				byte typeB = packetData[headerOffset+12];
				
				if (typeA==0x02 && typeB ==0x00){
					return PLUGIN_VENDOR;
				}
			}
			
			if(headerA==0x28 && headerB==0x10){ // 2810 -> chat 
				return PLUGIN_CHATSELF;
			}
					
			return PLUGIN_NONE; // Need more work here too hehe.
		}
	}
}

