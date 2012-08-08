using System;
using System.Collections;
using System.IO;

namespace hds
{
	public class ChatCommands
	{
		public ChatCommands ()
		{
			
		}
		
		public ArrayList parseCommand(){
			ArrayList result = new ArrayList();
			
			//Go to batchSend
			
			//Go to teleport
			
			return result;
		}
		
		public ArrayList batchSend(string fileName,int delay){
			ArrayList result = new ArrayList();
			StringUtils su = new StringUtils();

			
			StreamReader SR;
			string S;
			SR=File.OpenText(fileName);
			S=SR.ReadLine();
			while(S!=null)
			{

				if (S!=""){
					S= S.Trim(); //Remove spaces
					byte[] data = su.hexStringToBytes(S);
					
					// Create a new packet from the line (All encrypted for now)
					
					result.Add(new WorldPacket(data,true,0));
					
					if(delay!=0){
						result.Add(new WorldPacket(null,false,delay));
					}
				}
				S=SR.ReadLine();
			}
			SR.Close();
			
			// Close file
			
			return result;
		}
		
		
		public ArrayList teleport(int x, int y, int z){
			ArrayList result = new ArrayList();
			NumericalUtils nu = new NumericalUtils();
			
			
			// Adjust coords between human view and mxo format
			x = x*100;
			y = y*100;
			z = z*100;
			// End adjust
			
			byte[] xHex = nu.floatToByteArray(x+0.0f);
			byte[] yHex = nu.floatToByteArray(y+0.0f);
			byte[] zHex = nu.floatToByteArray(z+0.0f);
			
			
			// Construct teleport packet
			
			// Send teleport packet
			
			return result;
		}
		
		
		/*public override ArrayList process(byte[] packetData){ // Override the abstract method
			Console.WriteLine("[ECHO PLUGIN] ECHOING NON-ENCRYPTED PACKET");
			ArrayList packetList = new ArrayList();

			packetList.Add(new WorldPacket(packetData,false,0)); // Not encrypted!

			return packetList;
		}
		
		//This plugins always end with 1 packet
		public override bool endedProcess(){
			return true;
		}*/
	}
}

