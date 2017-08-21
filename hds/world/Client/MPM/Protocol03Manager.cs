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
                
				Output.WriteLine("Parsing View ID " + viewId);
				if (viewId==0x02){
					Output.WriteLine("[MPM] Parsing selfview");
					offset = Store.currentClient.playerInstance.parseAutoView(ref packetData,offset);
				}
				
				//Keep reading  
				// ToDo: This could cause a crash or bug (and there will no more be viewData but 
                viewId = (int)BufferHandler.readByte(ref packetData, ref offset);

                
                // Prevent the crash issue
                if (offset < packetData.Length)
                {
                    viewId += (int)(BufferHandler.readByte(ref packetData, ref offset) << 8);
                    //Output.WriteLine("[MPM] Parsing view: " + viewId);
                }
//                
				
			}
			return offset;
		}

      
		
	}
}

