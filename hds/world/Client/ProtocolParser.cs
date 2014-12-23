using System;
namespace hds
{
	public class ProtocolParser
	{
		private Protocol03Parser p03;
		private Protocol04Parser p04;

		public ProtocolParser(){
			p03 = new Protocol03Parser();
			p04 = new Protocol04Parser();
		}
		
		public int detectNeededPlugin (ref byte[] packetData)
		{
			// Here we need to get if it's one 03 or 04 protocol
						
			if (packetData.Length<=5){
				if (packetData[0]==0x00){
					return (int)PluginList.PLUGIN_ECHO; // 0 will mean not encrypted, so echo it
				}
				
				if (packetData[0]==0x42){
					Output.WriteLine("[PROTOCOL PARSER] RPC counter reset DETECTED");
				}
				
				return (int)PluginList.PLUGIN_NONE; // If too short, it's better to ack for now
			}
			
			
			
			
			byte pos1 = packetData[0];
			byte pos2 = packetData[1];
						
			// 03 protocol handling here
			
			if ((pos1==0x02 && pos2 == 0x03))
			{
				return p03.detect03Plugin(ref packetData);
			}
			
			// 04 protocol handling here
			
			if (pos1==0x02 && pos2 == 0x04){
				return p04.detect04Plugin(ref packetData); //Offset is 2
			}
			
			//same, for >5 length but start with 0x00
			//if(pos1==0x00){
			//	return (int)PluginList.PLUGIN_ECHO; // 0 will mean not encrypted, so echo it
			//}
			
			return (int)PluginList.PLUGIN_NONE; // In case that either plugin detects anything
		}

		
	}
}

