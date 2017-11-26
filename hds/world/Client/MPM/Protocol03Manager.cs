using System;

using hds.shared;

namespace hds{

	public class Protocol03Manager
	{

		private PacketReader reader;
			
		public int parse(int _offset, ref byte[] packetData){
			
			PacketReader reader = new PacketReader(packetData);
			reader.setOffsetOverrideValue(_offset);

			UInt16 viewId = reader.readUInt16(1);

			while (viewId!=0 && _offset < packetData.Length){
                
				Output.WriteLine("Parsing View ID " + viewId);
				if (viewId==2){
					Output.WriteLine("[MPM] Parsing selfview (offset:" + _offset + " PackData " + StringUtils.bytesToString(packetData) + " )");
					_offset = Store.currentClient.playerInstance.parseAutoView(ref packetData,_offset);
				}
				
				//Keep reading ViewID - if viewId is greater 2 it is not okay (this can happen as we dont handle all ViewUpdateRequests maybe)
				viewId = reader.readUInt16(1);
				if (viewId > 2)
				{
					viewId = 0;
				}      
				
			}
			
			return _offset;
		}

      
		
	}
}

