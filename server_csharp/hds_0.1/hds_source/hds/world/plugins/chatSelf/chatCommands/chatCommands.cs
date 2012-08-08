using System;
using System.Collections;
using System.IO;

namespace hds
{
	public class ChatCommands
	{
		private PacketsUtils pu;
		private ClientData cData;
		
		public ChatCommands (ClientData cData){
			this.cData = cData;
			this.pu = new PacketsUtils();
		}
		
		public ArrayList parseCommand(string data){
			ArrayList result = new ArrayList();
			
			
			string[] commands = data.Split(' ');
			
			string command = commands[0];
			
			
			try{
				if (command.Equals("?teleport") && commands.Length==4){
					// parse the coord parameters parameters as int
					result = teleport(int.Parse(commands[1]),int.Parse(commands[2]),int.Parse(commands[3]));
					result.Add(new WorldPacket(pu.createSystemMessage(ref cData,"Teleported!"),true,0));
				}
				
				if (command.Equals("?read") && commands.Length==3){
					// parse the file and delay
					result = batchSend(commands[1],int.Parse(commands[2]));
				}
				
				if (command.Equals("?rsi") && commands.Length==3){
					//parse the rsi part and value
					result = changeRsi(commands[1],int.Parse(commands[2]));
					result.Add(new WorldPacket(pu.createSystemMessage(ref cData,"Rsi changed!"),true,0));
				}
				
			}
			catch(Exception e){
				result.Add(new WorldPacket(pu.createSystemMessage(ref cData,"Error parsing command"),true,0));
				Console.WriteLine("[CHAT COMMAND PARSER] Error parsing request: "+data);
				Console.WriteLine("DEBUG: "+e.Message+"\n"+e.StackTrace);
			}
			return result;
		}
		
		private ArrayList batchSend(string fileName,int delay){
			ArrayList result = new ArrayList();
			StringUtils su = new StringUtils();

			
			StreamReader SR;
			string S;
			SR=File.OpenText(fileName);
			S=SR.ReadLine();
			while(S!=null)
			{

				if (S.Length>=2){
					
					S=S.Trim();
					
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
		
		
		private ArrayList teleport(int x, int y, int z){
			
			ArrayList result = new ArrayList();
			
			byte[] packet = pu.createTeleportPacket(ref cData,x,y,z);

			result.Add(new WorldPacket(packet,true,0));
			

			return result;
		}
		
		
		private ArrayList changeRsi(string part, int value){
			ArrayList result = new ArrayList();
			
			string[] keys = {"sex","body","hat","face","shirt","coat","pants","shoes","gloves","glasses","hair","facialdetail","shirtcolor","pantscolor","coatcolor","shoecolor","glassescolor","haircolor","skintone","tattoo","facialdetailcolor","leggins"};
			
			int pos = -1;
			
			for (int i = 0;i<keys.Length;i++){
				if (part.Equals(keys[i].ToLower())){
					pos = i;
					break;
				}	
			}
			
			if (pos>=0){
				int[] current = cData.getRsiValues();
				current[pos] = value;
				cData.setRsiValues(current);
				byte[] rsiData = pu.getRSIBytes(current);
				
				DynamicArray din = new DynamicArray();
				byte [] rsiChangeHeader = {0x02,0x03,0x02,0x00,0x02,0x80,0x89};
				din.append(rsiChangeHeader);
				din.append(rsiData);
				
				result.Add(new WorldPacket(din.getBytes(),true,0));
				
			}
			else{ 
				throw new FormatException("body part or clothes not found");
			}
			
			return result;
		}
		
		
	}
}

