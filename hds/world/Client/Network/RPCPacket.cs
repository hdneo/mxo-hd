using System;
using System.Collections;

using hds.shared;

namespace hds
{
	public class RPCPacket{

		private int addedMsgBlocks;
		private int rpcInside;
        private DynamicArray din { get; set; }
        private WorldClient destClient;


		public RPCPacket (WorldClient _client){
            destClient = _client;
            din = new DynamicArray();
			this.addedMsgBlocks = 0;
			this.rpcInside = 0;
		}
		
		public int getCurrentMsgBlocks(){
			return this.addedMsgBlocks;
		}
		
		public void appendMsgBlock(byte[] block){
			din.append(block);
			addedMsgBlocks++;
		}
		
		public void appendRpc(RPCPacket newRPC){
			din.append(newRPC.getBytes());
			rpcInside++;
		}
		
		
		private void incrementRpcCounter(int quantity){
			UInt16 rpcCounter = destClient.playerData.getRPCCounter();
			rpcCounter+=(UInt16)quantity;
            destClient.playerData.setRPCCounter(rpcCounter);
            
		}
		
		
		private byte[] getRpcBytes(){
			
			UInt16 rpcCounter = destClient.playerData.getRPCCounter();
			byte [] rpc = NumericalUtils.uint16ToByteArray(rpcCounter,0);
			return rpc;
		}
		
		public byte[] getBytes(){
			DynamicArray rpcStructure = new DynamicArray();
			byte [] totalMsgBlocks= NumericalUtils.uint16ToByteArrayShort((UInt16)addedMsgBlocks);
			byte [] currentCounter =  getRpcBytes();
			
			rpcStructure.append(currentCounter);
			rpcStructure.append(totalMsgBlocks);
			rpcStructure.append(din.getBytes());
			
			// Increment RPC Counter
			
			incrementRpcCounter(addedMsgBlocks);
			
			return rpcStructure.getBytes();
		}
		
		
		public byte[] getBytesWithHeader(bool timedRPC){
			DynamicArray rpcStructure = new DynamicArray();
			byte[] noTimedHeader = {0x04};
			byte[] timedHeader = new byte[6];
			byte[] time = TimeUtils.getUnixTime();
			
			timedHeader[0] = 0x82;
					
			ArrayUtils.copy(time,0,timedHeader,1,4);
			timedHeader[5] = 0x04; // This makes it "82aabbccdd04"
			
			if(timedRPC)
				rpcStructure.append(timedHeader);
			else
				rpcStructure.append(noTimedHeader);
			
			// Calculate blocks number
						
			
			if (rpcInside==0){ // Is just 1 group of msgblocks
				rpcInside=1;
				rpcStructure.append(NumericalUtils.uint16ToByteArrayShort((UInt16)rpcInside));
				rpcStructure.append(getBytes()); //Append our own content	
			}
			else{
				rpcStructure.append(NumericalUtils.uint16ToByteArrayShort((UInt16)rpcInside));
				rpcStructure.append(din.getBytes()); //Append our own content	
			}
			
			
			return rpcStructure.getBytes();
		}
	}
}

