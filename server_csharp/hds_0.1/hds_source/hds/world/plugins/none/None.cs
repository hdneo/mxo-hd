using System;
using System.Collections;

namespace hds
{
	public class None: Plugin
	{
		public None (ClientData cData, WorldDbAccess databaseHandler)
		{
			// For "02" repeating we dont need it. Is just to share the same plugin structure
		}
		
		public override ArrayList process(byte[] packetData){ // Override the abstract method
			Console.WriteLine("[NONE PLUGIN] ACKNOWLEDGED CLIENT");
			ArrayList packetList = new ArrayList();

			byte[] data = {0x02};
			packetList.Add(new WorldPacket(data,true,0));

			return packetList;
		}
		
		//This plugins always end with 1 packet
		public override bool endedProcess(){
			return true;
		}
	}
}

