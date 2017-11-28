using System;
using System.IO;
using System.Collections;

using hds.shared;

namespace hds{
	
	public class ChatCommandsHelper{
		
		public void parseCommand(string data){
			string[] commands = data.Split(' ');
			
			string command = commands[0].ToLower();

			try{
				
				if (command.Equals("?fix") && commands.Length>1){
					int maxRPC = int.Parse(commands[1]);
					for(int i = 0;i<maxRPC;i++){
						Store.currentClient.playerData.setRPCCounter((UInt16)i);

					    ServerPackets pak = new ServerPackets();
					    pak.sendSystemChatMessage(Store.currentClient, "Trying to fix!", "BROADCAST");
						
					}
					
				}
				
				if (command.Equals("?teleport") && commands.Length==4){
					// parse the coord parameters parameters as int
                    Store.currentClient.messageQueue.addObjectMessage(new PlayerHelper().teleport(int.Parse(commands[1]), int.Parse(commands[2]), int.Parse(commands[3])), false);

				    ServerPackets pak = new ServerPackets();
				    pak.sendSystemChatMessage(Store.currentClient, "Teleported!", "BROADCAST");
					
				}
				
				if (command.Equals("?rsi") && commands.Length==3){
					//parse the rsi part and value
                    Store.currentClient.messageQueue.addObjectMessage(new PlayerHelper().changeRsi(commands[1], int.Parse(commands[2])), false);

				    ServerPackets pak = new ServerPackets();
				    pak.sendSystemChatMessage(Store.currentClient, "Rsi changed!", "BROADCAST");
				}

				if (command.Contains("?updatersi"))
				{
					int[] current = Store.currentClient.playerData.getRsiValues();
					byte[] rsiData = PacketsUtils.getRSIBytes(current);
					
					ServerPackets packets = new ServerPackets();
					packets.sendAppeareanceUpdate(Store.currentClient, rsiData);
				}

			    if (command.StartsWith("?spawndatanode"))
			    {
			        ServerPackets pak = new ServerPackets();
			        pak.sendSystemChatMessage(Store.currentClient, "Spawn Datanode!", "BROADCAST");
			        pak.spawnDataNodeView(Store.currentClient);
			    }
                
                if (command.StartsWith("?message"))
                {
                    byte[] theMessage = PacketsUtils.createSystemMessageWithoutRPC(commands[1]);
                    Store.world.sendRPCToAllPlayers(theMessage);

                }

				if (command.Contains("?loot"))
				{
					UInt32 objectId = UInt32.Parse(commands[1]);
					
					ServerPackets packets = new ServerPackets();
					packets.sendLootWindowTest(Store.currentClient,objectId);

				}

			    if (command.Contains("?moa"))
			    {
			        string hexMoa = commands[1];
			        byte[] moaRSI = StringUtils.hexStringToBytes(hexMoa);
                    Array.Reverse(moaRSI,0,moaRSI.Length);

			        ServerPackets pak = new ServerPackets();
			        pak.sendSystemChatMessage(Store.currentClient, "Changed MOA to hexMoa d!", "BROADCAST");
			        pak.sendChangeChangeMoaRSI(Store.currentClient,moaRSI);
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

                    double x = 0; double y = 0; double z = 0;
                    byte[] Ltvector3d = Store.currentClient.playerInstance.Position.getValue();
                    NumericalUtils.LtVector3dToDoubles(Ltvector3d, ref x, ref y, ref z);

                    byte[] xPos = NumericalUtils.floatToByteArray((float)x, 1);
                    byte[] yPos = NumericalUtils.floatToByteArray((float)y, 1);
                    byte[] zPos = NumericalUtils.floatToByteArray((float)z, 1);

                    UInt64 currentEntityId = WorldSocket.entityIdCounter;
                    WorldSocket.entityIdCounter++;
                    uint rotation = 0;
                    
                    Mob theMob = new Mob();
                    theMob.setEntityId(currentEntityId);
                    theMob.setDistrict(Convert.ToUInt16(data[0].ToString()));
                    theMob.setDistrictName(Store.currentClient.playerData.getDistrict());
                    theMob.setName("HD Protector");
                    theMob.setLevel(255);
                    theMob.setHealthM(UInt16.Parse(data[4].ToString()));
                    theMob.setHealthC(UInt16.Parse(data[5].ToString()));
                    theMob.setMobId((ushort)rsiIDs[index]);
                    theMob.setRsiHex(StringUtils.bytesToString_NS(NumericalUtils.uint32ToByteArray(rsiIDs[index],1)));
                    theMob.setXPos(x);
                    theMob.setYPos(y);
                    theMob.setZPos(z);
                    theMob.xBase = x;
                    theMob.yBase = y;
                    theMob.zBase = z;
                    theMob.setRotation(rotation);
                    theMob.setIsDead(false);
                    theMob.setIsLootable(false);
	                lock (WorldSocket.npcs)
	                {
		                WorldSocket.npcs.Add(theMob);
	                }

	                lock (WorldSocket.gameServerEntities)
	                {
		                WorldSocket.gameServerEntities.Add(theMob);
	                }

                    // we use this for a test to see if we can spawn mobs and how we can handle them 
                    // We refactor this 
                }



                if (command.Contains("?sendrpc"))
                {
                    
                    // sends a RPC Packet from a File
                    string filename = "rpcpacket.txt";
                    TextReader tr = new StreamReader(filename);
                    string hexContent = tr.ReadToEnd();
                    hexContent = hexContent.Replace(" ", string.Empty);
                    hexContent = hexContent.Replace(" ", Environment.NewLine);
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
						
						#if DEBUG
                        ServerPackets pak = new ServerPackets();
                        pak.sendSystemChatMessage(Store.currentClient, "Test RPC Header : " + Store.currentClient.playerData.currentTestRPC.ToString(),"MODAL");
	                    #endif

                        Store.currentClient.playerData.currentTestRPC++;
                    }
                }
				
				if (command.Equals("?save")){
                    new PlayerHelper().savePlayerInfo(Store.currentClient);

                    ServerPackets pak = new ServerPackets();
                    pak.sendSaveCharDataMessage(Store.currentClient, StringUtils.charBytesToString_NZ(Store.currentClient.playerInstance.CharacterName.getValue()));
				}
				
			}
			catch(Exception e){
                Store.currentClient.messageQueue.addRpcMessage(PacketsUtils.createSystemMessage("Error parsing command!", Store.currentClient));
				#if DEBUG
				Output.WriteLine("[CHAT COMMAND PARSER] Error parsing request: "+data);
				Output.WriteLine("[CHAT COMMAND PARSER] DEBUG: "+e.Message+"\n"+e.StackTrace);
				#endif
			}
			
			
		}
		
		
		
	}
}

