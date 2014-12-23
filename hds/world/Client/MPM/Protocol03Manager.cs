using System;

using hds.shared;

namespace hds{

	public class Protocol03Manager{

		private byte[] buffer;
		private int offset;
		
			
		public int parse(int _offset, ref byte[] packetData){
			offset = _offset;
			buffer = packetData;
			    
			int viewId = (int)BufferHandler.readByte(ref packetData,ref offset);
			viewId += (int) (BufferHandler.readByte(ref packetData,ref offset)<<8);
			
			while (viewId!=0x00 && offset < packetData.Length){
                
				if (viewId==0x02){
					Output.WriteLine("[MPM] Parsing selfview");
                    
					offset = Store.currentClient.playerInstance.parseAutoView(ref packetData,offset);
				}
				
				//Keep reading
                
                viewId = (int)BufferHandler.readByte(ref packetData, ref offset);

                
                // Prevent the crash issue
                if (offset < packetData.Length)
                {
                    viewId += (int)(BufferHandler.readByte(ref packetData, ref offset) << 8);
                    //Output.WriteLine("[MPM] Parsing view: " + viewId);
                }
                
                
                
				
			}
			return offset;
		}

      
		
	}
}

