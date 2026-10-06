using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using hds.shared;

namespace hds
{
    public class ChatCommandsHelper
    {
        private readonly WorldClient client;

        public ChatCommandsHelper(WorldClient client)
        {
            this.client = client;
        }

        public void parseCommand(string data)
        {
            string[] commands = data.Split(' ');

            string command = commands[0].ToLower();

            try
            {
                if (command.Equals("?spawnobject") && commands.Length == 2)
                {
                    // This works ! 
                    // parse the coord parameters parameters as int
                    UInt16 GoId = UInt16.Parse(commands[1]);
                    GameObjectDefinitions def = new GameObjectDefinitions();
                    FieldInfo info = typeof(GameObjectDefinitions).GetField("Object" + GoId);
                    var instance = (GameObject) info.GetValue(def);
                    instance.DisableAllAttributes();

                    bool hasFieldsEnabled = false;
                    foreach (var propertyInfo in instance.GetType().GetFields())
                    {
                        Attribute theAttribute = (Attribute) propertyInfo.GetValue(instance);
                        switch (theAttribute.getName())
                        {
                            case "Position":
                                hasFieldsEnabled = true;
                                theAttribute.enable();
                                theAttribute.setValue(client.playerInstance.Position.getValue());
                                break;

                            case "Orientation":
                                hasFieldsEnabled = true;
                                theAttribute.enable();
                                theAttribute.setValue(client.playerInstance.YawInterval.getValue());
                                break;
                        }
                    }

                    ServerPackets packets = new ServerPackets(client);

                    if (hasFieldsEnabled)
                    {
                        UInt64 currentEntityId = WorldServer.entityIdCounter;
                        WorldServer.entityIdCounter++;
                        WorldServer.gameServerEntities.Add(instance);
                        packets.SendSpawnGameObject(client, instance, currentEntityId);
                    }
                }

                if (command.Equals("?org") && commands.Length == 2)
                {
                    int orgId = 0;
                    try
                    {
                        orgId = Int16.Parse(commands[1].ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                    if (orgId < 6)
                    {
                        Store.dbManager.WorldDbHandler.SetOrgId(client.playerData.getCharID(), orgId);
                        List<Attribute> updateAttributes = new List<Attribute>();
                        client.playerInstance.OrganizationID.setValue(orgId);
                        updateAttributes.Add(client.playerInstance.OrganizationID);

                        PacketContent myselfStateData = new PacketContent();
                        myselfStateData.AddByteArray(
                            client.playerInstance.GetSelfUpdateAttributes(updateAttributes, true));
                        client.messageQueue.addObjectMessage(myselfStateData.ReturnFinalPacket(), false);
                    }
                }

                if (command.Equals("?rep") && commands.Length == 3)
                {
                    int reputation = 0;
                    try
                    {
                        reputation = Int16.Parse(commands[2].ToString());
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }

                    List<Attribute> updateAttributes = new List<Attribute>();

                    bool changedReputation = false;
                    switch (commands[1].ToString())
                    {
                        case "zion":
                            changedReputation = true;
                            Store.dbManager.WorldDbHandler.SetReputation(client.playerData.getCharID(),
                                "repZion", reputation);
                            client.playerInstance.ReputationZionMilitary.setValue(reputation);
                            updateAttributes.Add(client.playerInstance.ReputationZionMilitary);
                            break;
                        case "machine":
                            changedReputation = true;
                            Store.dbManager.WorldDbHandler.SetReputation(client.playerData.getCharID(),
                                "repMachine", reputation);
                            client.playerInstance.ReputationMachines.setValue(reputation);
                            updateAttributes.Add(client.playerInstance.ReputationMachines);
                            break;
                        case "mero":
                            changedReputation = true;
                            Store.dbManager.WorldDbHandler.SetReputation(client.playerData.getCharID(),
                                "repMero", reputation);
                            client.playerInstance.ReputationMerovingian.setValue(reputation);
                            updateAttributes.Add(client.playerInstance.ReputationMerovingian);
                            break;
                        default:
                            break;
                    }

                    if (changedReputation)
                    {
                        PacketContent myselfStateData = new PacketContent();
                        myselfStateData.AddByteArray(
                            client.playerInstance.GetSelfUpdateAttributes(updateAttributes, true));
                        client.messageQueue.addObjectMessage(myselfStateData.ReturnFinalPacket(), false);
                    }
                }

                if (command.Equals("?gotopos") && commands.Length == 4)
                {
                    // parse the coord parameters parameters as int
                    client.messageQueue.addObjectMessage(
                        new PlayerHelper(client).teleport(int.Parse(commands[1]), int.Parse(commands[2]),
                            int.Parse(commands[3])), false);

                    ServerPackets pak = new ServerPackets(client);
                    pak.sendSystemChatMessage(client, "Teleported!", "BROADCAST");
                }

                if (command.Equals("?rsi") && commands.Length == 3)
                {
                    //parse the rsi part and value
                    client.messageQueue.addObjectMessage(
                        new PlayerHelper(client).changeRsi(commands[1], int.Parse(commands[2])), false);

                    ServerPackets pak = new ServerPackets(client);
                    pak.sendSystemChatMessage(client, "Rsi changed!", "BROADCAST");
                }

                if (command.Contains("?updatersi"))
                {
                    int[] current = client.playerData.getRsiValues();
                    byte[] rsiData = PacketsUtils.getRSIBytes(current);

                    ServerPackets packets = new ServerPackets(client);
                    packets.sendAppeareanceUpdate(client, rsiData);
                }

                if (command.StartsWith("?spawndatanode"))
                {
                    ServerPackets pak = new ServerPackets(client);
                    pak.sendSystemChatMessage(client, "Spawn Datanode!", "BROADCAST");
                    pak.spawnDataNodeView(client);
                }

                if (command.StartsWith("?message"))
                {
                    byte[] theMessage = PacketsUtils.createSystemMessageWithoutRPC(commands[1]);
                    Store.world.sendRPCToAllPlayers(theMessage);
                }

                if (command.Contains("?moa"))
                {
                    string hexMoa = commands[1];
                    byte[] moaRSI = StringUtils.hexStringToBytes(hexMoa);
                    Array.Reverse(moaRSI, 0, moaRSI.Length);

                    ServerPackets pak = new ServerPackets(client);
                    pak.sendSystemChatMessage(client, "Changed MOA to hexMoa d!", "BROADCAST");
                    pak.sendChangeChangeMoaRSI(client, moaRSI);
                }

                if (command.Equals("?playanim"))
                {
                    string animId = commands[1];
                    if (animId.Length == 4)
                    {
                        ServerPackets pak = new ServerPackets(client);
                        pak.sendPlayerAnimation(client, animId);
                    }
                }

                if (command.Equals("?playmove"))
                {
                    string animIdString = commands[1];

                    // Should just one byte
                    if (animIdString.Length == 2)
                    {
                        byte animId = Byte.Parse(animIdString);
                        ServerPackets pak = new ServerPackets(client);
                        pak.sendPlayerMoveAnim(client, animId);
                    }
                }

                if (command.StartsWith("?playfx"))
                {
                    string fxHEDID = commands[1];
                    DynamicArray din = new DynamicArray();


                    byte[] animationId = StringUtils.hexStringToBytes(fxHEDID);
                    byte[] viewID = {0x02, 0x00};

                    Random rand = new Random();
                    ushort updateViewCounter = (ushort) rand.Next(3, 200);
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

                    client.messageQueue.addObjectMessage(din.getBytes(), false);
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
                        client.messageQueue.addObjectMessage(StringUtils.hexStringToBytes(hexContent),
                            false);
                        Output.writeToLogForConsole("[SENDPACK FROM FILE] Content : " + hexContent);
                    }
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

                    double x = 0;
                    double y = 0;
                    double z = 0;
                    byte[] Ltvector3d = client.playerInstance.Position.getValue();
                    NumericalUtils.LtVector3dToDoubles(Ltvector3d, ref x, ref y, ref z);

                    byte[] xPos = NumericalUtils.floatToByteArray((float) x, 1);
                    byte[] yPos = NumericalUtils.floatToByteArray((float) y, 1);
                    byte[] zPos = NumericalUtils.floatToByteArray((float) z, 1);

                    UInt64 currentEntityId = WorldServer.entityIdCounter;
                    WorldServer.entityIdCounter++;
                    uint rotation = 0;

                    Mob theMob = new Mob();
                    theMob.setEntityId(currentEntityId);
                    theMob.setDistrict(Convert.ToUInt16(data[0].ToString()));
                    theMob.setDistrictName(client.playerData.getDistrict());
                    theMob.setName("HD Protector");
                    theMob.setLevel(255);
                    theMob.setHealthM(UInt16.Parse(data[4].ToString()));
                    theMob.setHealthC(UInt16.Parse(data[5].ToString()));
                    theMob.setMobId((ushort) rsiIDs[index]);
                    theMob.setRsiHex(StringUtils.bytesToString_NS(NumericalUtils.uint32ToByteArray(rsiIDs[index], 1)));
                    theMob.setXPos(x);
                    theMob.setYPos(y);
                    theMob.setZPos(z);
                    theMob.xBase = x;
                    theMob.yBase = y;
                    theMob.zBase = z;
                    theMob.setRotation(rotation);
                    theMob.setIsDead(false);
                    theMob.setIsLootable(false);
                    lock (WorldServer.mobs)
                    {
                        WorldServer.mobs.Add(theMob);
                    }

                    lock (WorldServer.gameServerEntities)
                    {
                        WorldServer.gameServerEntities.Add(theMob);
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
                        client.messageQueue.addRpcMessage(StringUtils.hexStringToBytes(hexContent));
                        Output.writeToLogForConsole("[SENDRPC FROM FILE] Content : " + hexContent);
                    }
                }

                if (command.Contains("?checkrpc"))
                {
                    DynamicArray din = new DynamicArray();
                    din.append(StringUtils.hexStringToBytes(
                        "2E1000FF7D020024000000310000000000000000000000000000000000000000000000000B0053796E61707A65373737001D004F6E2079656168204920646F2072656D656D62657220796F75203A2900"));
                    client.messageQueue.addRpcMessage(din.getBytes());
                }

                if (command.Contains("?testrpc"))
                {
                    UInt16 maxRPC = 255;
                    // Just to reference 
                    if (client.playerData.currentTestRPC <= maxRPC)
                    {
                        // Only if it is below we send it - we test with a 5 size packet
                        DynamicArray din = new DynamicArray();
                        if (client.playerData.currentTestRPC < 256)
                        {
                            din.append(0x80);
                            din.append(NumericalUtils.uint16ToByteArrayShort(client.playerData
                                .currentTestRPC));
                        }
                        else
                        {
                            din.append(0x81);
                            din.append(NumericalUtils.uint16ToByteArray(client.playerData.currentTestRPC,
                                0));
                        }

                        // Test Faction Invites
                        din.append(StringUtils.hexStringToBytes(
                            "0C001C000000000000000E00416674657257686F72754E656F001D00416674657257686F72754E656F2773204D697373696F6E205465616D00"));

                        client.messageQueue.addRpcMessage(din.getBytes());

#if DEBUG
                        ServerPackets pak = new ServerPackets(client);
                        pak.sendSystemChatMessage(client,
                            "Test RPC Header : " + client.playerData.currentTestRPC.ToString(), "MODAL");
#endif

                        client.playerData.currentTestRPC++;
                    }
                }

                if (command.Equals("?save"))
                {
                    new PlayerHelper(client).SavePlayerInfo(client);

                    ServerPackets pak = new ServerPackets(client);
                    pak.sendSaveCharDataMessage(client,
                        StringUtils.charBytesToString_NZ(client.playerInstance.CharacterName.getValue()));
                }
            }
            catch (Exception e)
            {
                client.messageQueue.addRpcMessage(
                    PacketsUtils.createSystemMessage("Error parsing command!", client));
#if DEBUG
                Output.WriteLine("[CHAT COMMAND PARSER] Error parsing request: " + data);
                Output.WriteLine("[CHAT COMMAND PARSER] DEBUG: " + e.Message + "\n" + e.StackTrace);
#endif
            }
        }
    }
}