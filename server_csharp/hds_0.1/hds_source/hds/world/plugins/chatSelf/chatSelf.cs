using System;
using System.Collections;

namespace hds
{
	public class ChatSelf: Plugin
	{
		
		private ChatCommands chatCommands;
		
		public ChatSelf (ClientData cData, WorldDbAccess databaseHandler)
		{
			this.cData = cData;
			this.chatCommands = new ChatCommands(this.cData);
		}
		
		public override ArrayList process(byte[] packetData){ // Override the abstract method
			NumericalUtils nu = new NumericalUtils();
			StringUtils su = new StringUtils();
			ArrayUtils au = new ArrayUtils();
			
			ArrayList packetList = new ArrayList();
			int offset=0;
			
			Console.WriteLine("[CHAT PLUGIN] PROCESSING");
			
			if(packetData[0]==0x02){
				offset=15;
			}else{
				offset=19;
			}
			
			// Get the length in hex values
			byte[] length=new byte[2];
			length[0]=packetData[offset];
			length[1]=packetData[offset+1];
			
			int lengthI = (int)nu.ByteArrayToUint16(length,1);
			lengthI--; // Remove the last "end of string"
			
			offset+=2;
			
			byte [] contentHex = new byte[lengthI];
			au.copy(contentHex,0,packetData,offset,lengthI);
			
			string chatContent = su.charBytesToString(contentHex);
			
			if (chatContent[0]=='?'){ //Maybe a parameter
				
				return chatCommands.parseCommand(chatContent); // Parse for commands
			}
			
			// If it's not a command, we just return an empty arrayList
			return packetList;
		}
		
		//This plugins always end with 1 packet
		public override bool endedProcess(){
			return true;
		}
	}
}

