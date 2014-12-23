using System;
namespace hds
{
	public class Protocol04Parser
	{
		
	
		public Protocol04Parser ()
		{

		}
		
		public int detect04Plugin(ref byte[] packetData){
			
			// Here we supose that client didnt pack more than 1 RPC request (ideal thing of course)
			
			//02 04 01 00 0c 01 02 71 03 
			int headerA = (int)packetData[7];	
			int headerB = (int)packetData[8];
			
			int rpcHeader = 0;
			
			
			if(headerA<0x80){
				rpcHeader = headerA;
			}else{
				rpcHeader = headerB;
				if (headerA==0x81){
					rpcHeader+=0x0100;
				}
			}
			
			Output.WriteLine("[*] Detected 04 header: "+rpcHeader+" decimal");
			
			switch (rpcHeader){
				case (int)KnownRPCHeaders.JUMP:
					return (int)PluginList.PLUGIN_NONE; //None for now
			
				
				//02 04 01 00 49 01 08 80 c8 f0 01 30 39 02 00 
				case (int)KnownRPCHeaders.OBJECTINTERACTION:
					return (int)PluginList.PLUGIN_OBJECTINTERACTION;
				
				case (int)KnownRPCHeaders.CHATSELF:
					return (int)PluginList.PLUGIN_CHATSELF; //None for now
				
				case (int)KnownRPCHeaders.TARGET:
					return (int)PluginList.PLUGIN_TARGET;
				
				case (int)KnownRPCHeaders.MISSION_REQUEST:
					return (int)PluginList.PLUGIN_MISSION;
				
				case (int)KnownRPCHeaders.PARTY_LEAVE:
					return (int)PluginList.PLUGIN_MISSION;
				
				default:	
					return (int)PluginList.PLUGIN_NONE;
			}
		}
	}
}

