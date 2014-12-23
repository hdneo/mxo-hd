using System;
namespace hds
{
	public class Protocol04Manager
	{
		private byte[] buffer;
		private int offset;
		private RpcManager rpcManager;
		
		
		public Protocol04Manager (){
			rpcManager = new RpcManager();
		}
		
		
		public int parse(int _offset, ref byte[] packetData){
			
			buffer = packetData;
			offset = _offset;
			// See what we are dealing with:
			int rpcBlocks = (int)  BufferHandler.readByte(ref buffer,ref offset);
			
			for (int i = 0;i<rpcBlocks;i++){
				parseRpcBlock();	
			}
			
			
			return offset;
		}

        private void parseRpcBlock(){

            // Read current Client RPC Counter
            BufferHandler.readBytes(ref buffer, ref offset, 2);
            int rpcInside = (int)BufferHandler.readByte(ref buffer, ref offset);
            for (int i = 0; i < rpcInside; i++){
                parseRpc();
            }
        }

        private void parseRpc(){
            int length = BufferHandler.readByte(ref buffer, ref offset);
            if (length >= 0x80){ //2 bytes as size
                length = (length - 0x80) << 8;
                length = length + (int)BufferHandler.readByte(ref buffer, ref offset);
            }

            int header = BufferHandler.readByte(ref buffer, ref offset);
            length--; //discount 1 byte for the header
            if (header >= 0x80){ //2 bytes as size
                header = (header - 0x80) << 8;
                header = header + (int)BufferHandler.readByte(ref buffer, ref offset);
                length--; // discount another byte for the header (2nd byte)
            }
            byte[] rpcValues = BufferHandler.readBytes(ref buffer, ref offset, length);
            Output.WriteLine("RPC (Header " + header + " | Content:" + StringUtils.bytesToString(rpcValues));
            rpcManager.HandleRpc(header, ref rpcValues);


        }

	}
}

