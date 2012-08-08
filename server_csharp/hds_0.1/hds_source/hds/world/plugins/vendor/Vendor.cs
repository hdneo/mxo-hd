using System;
using System.Collections;

namespace hds
{
	public class Vendor: Plugin
	{
		
		public Vendor (ClientData cData, WorldDbAccess databaseHandler)
		{
			this.cData = cData;
			this.databaseHandler = databaseHandler;
			this.utils = new PacketsUtils();
		}
		
		public override ArrayList process(byte[] packetData){ // Override the abstract method
			Console.WriteLine("[VENDOR PLUGIN] SENDING WINDOW");
			ArrayList packetList = new ArrayList();
			
			
			byte[] data = utils.createVendorPacket(ref cData,3333333); //Static id for now
			packetList.Add(new WorldPacket(data,true,0));
			return packetList;
		}
		
		//This plugins always end with 1 packet
		public override bool endedProcess(){
			return true;
		}
		
	}
}

