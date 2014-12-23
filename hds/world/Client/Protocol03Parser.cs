using System;
namespace hds
{
	public class Protocol03Parser
	{
		
		private ArrayUtils au;
		
		public Protocol03Parser ()
		{
			au = new ArrayUtils();
		}
		
		public int detect03Plugin(ref byte[] packetData){
			
			//CHAT PACKET: 02 03 02 00 01 02 73 00 00 
			// 04 01 00 54 01 1d 28 10 00 08 00 00 00 00 13 00 3f 74 65 6c 65 70 6f 72 74 20 32 30 20 32 30 20 32 30 00 
			
			byte actionFromClient=packetData[6];
			
			if(actionFromClient==0x73){ //Do the 03->04 bypass to handle chat commands
				if(packetData.Length>12){
				
					Output.WriteLine("[03 Parser] PATCHING 03 PACKET TO SELF CHAT RPC PROCESS");
					byte[] change = new byte[1+packetData.Length-9];
					change[0]=0x02;
					ArrayUtils.copy(packetData,9,change,1,packetData.Length-9);
					packetData = change;
					return (int)PluginList.PLUGIN_CHATSELF;
				}
			}
			
			byte[] selfUpdateHeader = {0x02,0x03,0x02,0x00,0x01};
					
			
			/* DOING A TRICK (03 self chat packet to 04 RPC) */
			byte[] temp = new byte[5];
			ArrayUtils.copy(packetData,0,temp,0,5);
			
			
			if (au.equal(temp,selfUpdateHeader)){
				//02 03 02 02 01 02 73
				byte updateSelector = packetData[5];
				byte possibleUpdateValue = packetData[6];
				
				switch ((int)updateSelector){
					
					case 0x02:
						//DO
						return (int)PluginList.PLUGIN_SELFUPDATE;
						
						
					case 0x03:
						return (int)PluginList.PLUGIN_SELFUPDATE;
					
					case 0x04:
						return (int)PluginList.PLUGIN_SELFUPDATE;
										
					case 0x06:
						return (int)PluginList.PLUGIN_SELFUPDATE;
					
								
					case 0x08:
						return (int)PluginList.PLUGIN_SELFUPDATE;
					
										
					case 0x0a:
						return (int)PluginList.PLUGIN_SELFUPDATE;
					
					
				}
			}
			
			
			return (int)PluginList.PLUGIN_NONE; // Need more work here :P
		}
	}
}

