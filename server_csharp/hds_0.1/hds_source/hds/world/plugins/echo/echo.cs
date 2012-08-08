using System;
using System.Collections;

namespace hds
{
	public class Echo: Plugin
	{
		public Echo (ClientData cData, WorldDbAccess databaseHandler)
		{
			// For "Echo" we dont need it. Is just to share the same plugin structure
		}
		
		public override ArrayList process(byte[] packetData){ // Override the abstract method
			Console.WriteLine("[ECHO PLUGIN] ECHOING NON-ENCRYPTED PACKET");
			ArrayList packetList = new ArrayList();

			packetList.Add(new WorldPacket(packetData,false,0)); // Not encrypted!

			return packetList;
		}
		
		//This plugins always end with 1 packet
		public override bool endedProcess(){
			return true;
		}
	}
}

