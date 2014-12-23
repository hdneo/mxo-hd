using System;
using System.IO;
using System.Collections;

using hds.shared;

namespace hds{
	
	public class ChatCommandsHelper{
		
		public void parseCommand(string data){
			Output.WriteLine("[Chat Command helper] Chat command is: '"+data+"'");
			string[] commands = data.Split(' ');
			
			string command = commands[0].ToLower();
			byte[] hexData = null;

			try{
				
				if (command.Equals("?fix") && commands.Length>1){
					int maxRPC = int.Parse(commands[1]);
					for(int i = 0;i<maxRPC;i++){
						Store.currentClient.playerData.setRPCCounter((UInt16)i);
                        Store.currentClient.messageQueue.addRpcMessage(PacketsUtils.createSystemMessage("Trying to fix! " + i, Store.currentClient));
					}
					
				}
				
				if (command.Equals("?teleport") && commands.Length==4){
					// parse the coord parameters parameters as int
                    Store.currentClient.messageQueue.addObjectMessage(new PlayerHelper().teleport(int.Parse(commands[1]), int.Parse(commands[2]), int.Parse(commands[3])), false);
                    Store.currentClient.messageQueue.addRpcMessage(PacketsUtils.createSystemMessage("Teleported!", Store.currentClient));
					
				}
				
				if (command.Equals("?rsi") && commands.Length==3){
					//parse the rsi part and value
                    Store.currentClient.messageQueue.addObjectMessage(new PlayerHelper().changeRsi(commands[1], int.Parse(commands[2])), false);
                    Store.currentClient.messageQueue.addRpcMessage(PacketsUtils.createSystemMessage("Rsi changed!", Store.currentClient));
				}

                
                if (command.StartsWith("?message"))
                {
                    Output.WriteLine("[COMMAND HELPER]MESSAGE RECEIVED");
                    byte[] theMessage = PacketsUtils.createSystemMessageWithoutRPC(commands[1]);
                    Store.world.sendRPCToAllPlayers(theMessage);

                }

                if (command.Equals("?playanim"))
                {
                    string animId = commands[1];
                    if (animId.Length == 4)
                    {
                        ServerPackets pak = new ServerPackets();
                        pak.sendPlayerAnimation(Store.currentClient, animId);
                        
                    }
                    
                }

                if (command.StartsWith("?playfx"))
                {
                    string fxHEDID =  commands[1];
                    DynamicArray din = new DynamicArray();


                    byte[] animationId = StringUtils.hexStringToBytes(fxHEDID);
                    byte[] viewID = { 0x02, 0x00 };

                    Random rand = new Random();
                    ushort updateViewCounter = (ushort)rand.Next(3, 200);
                    byte[] updateCount = NumericalUtils.uint16ToByteArrayShort(updateViewCounter);

                    Output.WriteLine("Check if its really one byte or two : " + StringUtils.bytesToString(updateCount));

                    
                    din.append(viewID);
                    din.append(0x02);
                    din.append(0x80);
                    din.append(0x80);
                    din.append(0x80);
                    din.append(0x90);
                    din.append(0xed);
                    din.append(0x00);
                    din.append(0x30);
                    din.append(animationId);
                    din.append(updateCount);

                    Store.currentClient.messageQueue.addObjectMessage(din.getBytes(), false);
                    
                }

                if (command.Contains("?send"))
                {
                    // Sends a packet from a file
                    string filename = "packet.txt";
                    TextReader tr = new StreamReader(filename);
                    string hexContent = tr.ReadToEnd();
                    hexContent = hexContent.Replace(" ", string.Empty);
                    hexContent = hexContent.Replace(" ", Environment.NewLine);
                    tr.Close();
                    
                    if (hexContent.Length > 0)
                    {
                        Store.currentClient.messageQueue.addObjectMessage(StringUtils.hexStringToBytes(hexContent), false);
                        Output.writeToLogForConsole("[SENDPACK FROM FILE] Content : " + hexContent);
                    }
                    
                }

                if (command.Contains("?combat"))
                {
                    byte[] dummypak = new byte[4];
                    TestUnitHandler test = new TestUnitHandler();
                    test.testCloseCombat(ref dummypak);
                }

                if (command.Contains("?mob"))
                {
                    // we use this for a test to see if we can spawn mobs and how we can handle them 
                    // First we just spawn one mob - and after that we test around with some things
                    // it is something like pupeteer but just a simple variant of it

                    byte[] goID = { 0x57, 0x02 }; // For NPC
                    byte[] spawnCounter = { 0x35 }; // well this needs to be incremented but reseted if we receve an ack for it | EDIT: it isnt a really spawn counter ?
                    byte[] seperator = { 0xcd, 0xab };

                    double x = 0; 
                    double y = 0; 
                    double z = 0;

                    byte[] Ltvector3d = Store.currentClient.playerInstance.Position.getValue(); // Get player coords as we want to spawn him there 
                    UInt16 rotation = (UInt16)Store.currentClient.playerInstance.YawInterval.getValue()[0]; // Rotation

                    // Convert to Double 
                    NumericalUtils.LtVector3dToDoubles(Ltvector3d, ref x, ref y, ref z);

                    Output.writeToLogForConsole("SPAWN MOB Position : X " + x.ToString() + " Y : " + y.ToString() + " Z: " + z.ToString());

                    UInt16 dynObjectID = 0x8fff;
                    string name = "HD Protector".PadRight(32,'\x00');
                    byte[] viewID = { 0x08, 0x00 }; // The new ViewID
                    byte[] tail = { 0x00, 0x00, 0x00 }; // The tail at the end 
                    byte[] weapon = {0xc0, 0x01, 0x8B, 0x05};

                    UInt32[] rsiIDs = new UInt32[10];
                    rsiIDs[0] = 0xB7010058;
                    rsiIDs[1] = 0x89090058;
                    rsiIDs[2] = 0xB5010058;
                    rsiIDs[3] = 0x3A030008;
                    rsiIDs[4] = 0x32030008;
                    rsiIDs[5] = 0xD0010058;
                    rsiIDs[6] = 0xD4010058;
                    rsiIDs[7] = 0xB8040004; // Smith
                    rsiIDs[8] = 0x92010058; // Seraph
                    rsiIDs[9] = 0x56050004;

                    Random rand = new Random();
                    int index = rand.Next(0, 9);

                    byte[] rsiID = NumericalUtils.uint32ToByteArray(rsiIDs[index], 0);

                    UInt16 randView = (UInt16)rand.Next(0, 256);

                    DynamicArray din = new DynamicArray();
                    din.append(0x01); // Master View ID 1
                    din.append(0x00); // Master View ID 1
                    din.append(0x0c);
                    din.append(goID); // The goID
                    din.append(spawnCounter);
                    din.append(seperator);
                    din.append(0x12); // Num Attributes
                    din.append(0x8e); // Indicator for name
                    din.append(StringUtils.stringToBytes(name)); // 32 Bytes Name (padded)
                    din.append(StringUtils.hexStringToBytes("0010000022D4F5941C00C601")); // Unknown yet
                                                           //0010000022c4c30180c9
                    din.append(StringUtils.hexStringToBytes("80C1")); // Maybe ObjectID ? Ok maybe the C1 is part of the objectID (uint32) and the 0x58 at the end belongs to new attrib
                    din.append(rsiID);
                    //din.append(NumericalUtils.uint16ToByteArrayShort(rotation)); (must first check what is rotation)
                    din.append(NumericalUtils.doublesToLtVector3d(x, y, z));
                    din.append(weapon); // NOT SURE - ITS FROM MXOSOURCE
                    din.append(StringUtils.hexStringToBytes("0312")); // 0312 - has something to do with the mood of the npc (01 is normal , 12 is similiar to aggresive)
                    din.append(0xA8); // Should this be the level attrib identifier ?
                    din.append(0xff); // Level (should be 255)
                    din.append(StringUtils.hexStringToBytes("9600B209960007080000"));
                    din.append(0x80);
                    din.append(0x01); // something with the options (02 = close combat (unselectable) and details, 01 close combat (unselectable) , details and talk)
                    din.append(0x01);
                    din.append(viewID); // new View ID
                    //din.append(NumericalUtils.uint16ToByteArray(randView, 1)); // Try to assign a random id

                    Store.currentClient.messageQueue.addObjectMessage(din.getBytes(), false);
                }



                if (command.Contains("?sendrpc"))
                {
                    
                    // sends a RPC Packet from a File
                    string filename = "rpcpacket.txt";
                    TextReader tr = new StreamReader(filename);
                    string hexContent = tr.ReadToEnd();
                    hexContent = hexContent.Replace(" ", string.Empty);
                    hexContent = hexContent.Replace(" ", Environment.NewLine);
                    Output.Write("SEND RPC COMMAND : CONTENT : "+ hexContent);
                    tr.Close();

                    if (hexContent.Length > 0)
                    {
                        Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes(hexContent));
                        Output.writeToLogForConsole("[SENDRPC FROM FILE] Content : " + hexContent);
                    }
                }

                if (command.Contains("?checkrpc"))
                {
                    DynamicArray din = new DynamicArray();
                    din.append(StringUtils.hexStringToBytes("2E1000FF7D020024000000310000000000000000000000000000000000000000000000000B0053796E61707A65373737001D004F6E2079656168204920646F2072656D656D62657220796F75203A2900"));
                    Store.currentClient.messageQueue.addRpcMessage(din.getBytes());
                }

                if (command.Contains("?testrpc")){

                    UInt16 maxRPC = 33279;
                    // Just to reference 
                    if (Store.currentClient.playerData.currentTestRPC <= maxRPC){

                        // Only if it is below we send it - we test with a 5 size packet
                        DynamicArray din = new DynamicArray();
                        if (Store.currentClient.playerData.currentTestRPC < 127)
                        {
                            din.append(NumericalUtils.uint16ToByteArrayShort(Store.currentClient.playerData.currentTestRPC));
                        }
                        else
                        {
                            din.append(NumericalUtils.uint16ToByteArray(Store.currentClient.playerData.currentTestRPC, 0));
                        }
                        
                        din.append(0x00);
                        din.append(0x00);
                        din.append(0x00);

                        Store.currentClient.messageQueue.addRpcMessage(din.getBytes());
                        Store.currentClient.messageQueue.addRpcMessage(PacketsUtils.createMessage("Test RPC Header : " + Store.currentClient.playerData.currentTestRPC.ToString(), "MODAL", Store.currentClient));

                        Store.currentClient.playerData.currentTestRPC++;
                    }
                }
				
				if (command.Equals("?save")){
                    new PlayerHelper().savePlayerInfo();

                    Store.currentClient.messageQueue.addRpcMessage(PacketsUtils.saveCharDataMessage(Store.currentClient.playerInstance.getName(), Store.currentClient));
				}
				
			}
			catch(Exception e){
                Store.currentClient.messageQueue.addRpcMessage(PacketsUtils.createSystemMessage("Error parsing command!", Store.currentClient));
				Output.WriteLine("[CHAT COMMAND PARSER] Error parsing request: "+data);
				Output.WriteLine("[CHAT COMMAND PARSER] DEBUG: "+e.Message+"\n"+e.StackTrace);
			}
			
			
		}
		
		
		
	}
}

