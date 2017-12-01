using System;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using hds.resources.gameobjects;
using hds.shared;

namespace hds
{

    public class WorldSocket
    {

        private IPEndPoint udplistener;
        private Thread listenThread;
        private int serverport;
        private bool mainThreadWorking;
        private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
        public const int SIO_UDP_CONNRESET = -1744830452;

        public static Hashtable Clients { get; set; }

        // Collection to store the spawned npcs 
        public static ArrayList npcs = ArrayList.Synchronized(new ArrayList());
        public static ArrayList missionTeams = ArrayList.Synchronized(new ArrayList());
        public static ArrayList TempCrews = ArrayList.Synchronized(new ArrayList());
        public static UInt64 entityIdCounter = 1;
        public static ArrayList gameServerEntities = ArrayList.Synchronized(new ArrayList()); // should hold all GameObject Entities where a view can be created (static, dynamic etc.)
        private byte[] buffer;
        
        public ObjectManager objMan { get; set; }


        public WorldSocket()
        {

            Clients = new Hashtable();
            objMan = new ObjectManager();

            listenThread = new Thread(ListenForAllClients);
            mainThreadWorking = true;
            Output.WriteLine("[World Server] Set and ready at UDP port 10000");
        }

        public void startServer()
        {
            listenThread.Start();
            mainThreadWorking = true;
        }

        public void stopServer()
        {
            this.mainThreadWorking = false;
        }

        /// <summary>
        /// Send a Message to One Player (like private Message)
        /// </summary>
        /// <param name="charId">CharId to send the Packet</param>
        /// <param name="packet">Packet Stream</param>
        public void sendRPCToOnePlayer(UInt32 charId, byte[] packet)
        {
            lock (Clients.SyncRoot)
            {
                foreach (string clientKey in Clients.Keys)
                {
                    WorldClient client = Clients[clientKey] as WorldClient;
                    if (client.playerData.getCharID() == charId)
                    {
                        client.messageQueue.addRpcMessage(packet);
                        client.FlushQueue();
                    }
                }
            }
        }

        /// <summary>
        /// Send a Message to One Player (like private Message by Handle)
        /// </summary>
        /// <param name="packet">Packet Stream</param>
        /// <param name="playerHandle">Player Handle to send the Packet</param>
        public void sendRPCToOnePlayerByHandle(byte[] packet, string playerHandle)
        {
            lock (Clients.SyncRoot)
            {

                foreach (string clientKey in Clients.Keys)
                {
                    WorldClient client = Clients[clientKey] as WorldClient;
                    string charname = StringUtils.charBytesToString_NZ(client.playerInstance.CharacterName.getValue());
                    Output.writeToLogForConsole("Charname |" + charname + "| PlayerHandle |" + playerHandle + "|");
                    int lenCharname = charname.Length;
                    int playerHandleLen = playerHandle.Length;
                    if (charname == playerHandle)
                    {
                        client.messageQueue.addRpcMessage(packet);
                        client.FlushQueue();
                    }
                }
            }

        }


        /// <summary>
        /// Sends a RPC Message to all Players but not myself
        /// NEEDS TESTING
        /// </summary>
        /// <param name="myself"></param>
        /// <param name="data"></param>
        public void sendRPCToAllOtherPlayers(ClientData myself, byte[] data)
        {
            // Send a global message to all connected Players (like shut down Server announce or something)
            lock(Clients.SyncRoot){
                foreach (string clientKey in Clients.Keys)
                {
                    // Populate a message to all players 
                    // WE TEST THIS HERE!
                    WorldClient client = Clients[clientKey] as WorldClient;
                    if (client.playerData.getCharID() != myself.getCharID())
                    {

                        // create the RPC Message
                        client.messageQueue.addRpcMessage(data);
                        client.FlushQueue();
                    }

                }
            }
        }

        public void sendRPCToCrewMembers(WorldClient myself, byte[] data)
        {
            // Send a global message to all connected Players (like shut down Server announce or something)
            lock(Clients.SyncRoot){
                foreach (string clientKey in Clients.Keys)
                {
                    // Populate a message to all players to my crew
                    WorldClient client = Clients[clientKey] as WorldClient;
                    if (client.playerData.getCharID() != myself.playerData.getCharID() && client.playerInstance.CrewID == myself.playerInstance.CrewID)
                    {
                        // create the RPC Message
                        client.messageQueue.addRpcMessage(data);
                        client.FlushQueue();
                    }

                }
            }
        }
        
        public void sendRPCToFactionMembers(WorldClient myself, byte[] data)
        {
            // Send a global message to all connected Players (like shut down Server announce or something)
            lock(Clients.SyncRoot){
                foreach (string clientKey in Clients.Keys)
                {
                    // Populate a message to all players to my crew
                    WorldClient client = Clients[clientKey] as WorldClient;
                    if (client.playerData.getCharID() != myself.playerData.getCharID() && client.playerInstance.FactionID == myself.playerInstance.FactionID)
                    {
                        // create the RPC Message
                        client.messageQueue.addRpcMessage(data);
                        client.FlushQueue();
                    }

                }
            }
        }
        
        public void sendRPCToMissionTeamMembers(WorldClient myself, byte[] data)
        {
            // Send a global message to all connected Players (like shut down Server announce or something)
            lock(Clients.SyncRoot){
                foreach (string clientKey in Clients.Keys)
                {
                    // Populate a message to all players to my crew
                    WorldClient client = Clients[clientKey] as WorldClient;
                    if (client.playerData.getCharID() != myself.playerData.getCharID() && client.playerInstance.MissionTeamID == myself.playerInstance.MissionTeamID)
                    {
                        // create the RPC Message
                        client.messageQueue.addRpcMessage(data);
                        client.FlushQueue();
                    }

                }
            }
        }
        

        /// <summary>
        /// Sends a RPC Message to all connected players including the sender
        /// </summary>
        /// <param name="data"></param>
        public void sendRPCToAllPlayers(byte[] data)
        {
            // Send a global message to all connected Players (like shut down Server announce or something)
            lock (Clients.SyncRoot)
            {
                foreach (string clientKey in Clients.Keys)
                {
                    WorldClient client = Clients[clientKey] as WorldClient;
                    client.messageQueue.addRpcMessage(data);
                    client.FlushQueue();
                }
            }
        }

        /// <summary>
        /// This sends a View Create/Update/Delete Packet to all Players 
        /// </summary>
        /// <param name="data">Packet Stream without ViewID</param>
        /// <param name="charId">from charId</param>
        /// <param name="goId">from GoId</param>
        public void sendViewPacketToAllPlayers(byte[] data,UInt32 charId, UInt32 goId, UInt64 entityId)
        {
            // Send a global message to all connected Players (like shut down Server announce or something)
            lock (Clients.SyncRoot)
            {
                foreach (string clientKey in Clients.Keys)
                {
                    WorldClient client = Clients[clientKey] as WorldClient;
                    if (client.playerData.getCharID() != charId)
                    {
                        #if DEBUG
                        Output.Write("[ViewThread] Handle View For all Packet for charId : " + charId.ToString());
                        #endif
                        // Get or generate a View for the GoID 
                        ClientView view = client.viewMan.getViewForEntityAndGo(entityId, goId);
                        PacketContent viewPacket = new PacketContent();

                        if (view.viewCreated == false)
                        {
                            // if view is new , add the viewId at the end (cause its creation)
                            // Remove the 03 01 00 (as it was generate previosly)
                            viewPacket.addByteArray(data);
                            viewPacket.addUint16(view.ViewID,1);
                            viewPacket.addByte(0x00);

                        }
                        else
                        {
                            // Update View
                            viewPacket.addUint16(view.ViewID,1);
                            viewPacket.addByteArray(data);
                            viewPacket.addByte(0x00);  
                        }

                        if (view.viewNeedsToBeDeleted == true)
                        {
                            // Delete one View
                            viewPacket.addByte(0x01);
                            viewPacket.addByte(0x00);
                            viewPacket.addByte(0x01); // Comand (Delete)
                            viewPacket.addUint16(1,1); // NumViews to Delete
                            viewPacket.addUint16(view.ViewID,1);
                            viewPacket.addByte(0x00);
                        }

                        
                        client.messageQueue.addObjectMessage(viewPacket.returnFinalPacket(), false);
                        client.FlushQueue();
                    }
                }
            }
        }

        public void SendViewUpdateToClientsWhoHasSpawnedView(PacketContent packet, Mob mob)
        {
            lock (Clients.SyncRoot)
            {
                foreach (string clientKey in Clients.Keys)
                {
                    WorldClient client = Clients[clientKey] as WorldClient;
                    ClientView mobView = client.viewMan.getViewForEntityAndGo(mob.getEntityId(), NumericalUtils.ByteArrayToUint16(mob.getGoId(), 1));
                    if (mobView.viewCreated == true && mob.getDistrict() == client.playerData.getDistrictId() && client.playerData.getOnWorld())
                    {
                        ServerPackets pak = new ServerPackets();
                        pak.SendNpcUpdateData(mobView.ViewID, client, packet.returnFinalPacket());
                    }
                }
            }
        }

        
        private void finalReceiveFrom(IAsyncResult iar)
        {
            
            Socket recvSocket = (Socket)iar.AsyncState;
            EndPoint Remote = new IPEndPoint(IPAddress.Any, 0);
            
            int msgLen = 0;

            try
            {
                msgLen = recvSocket.EndReceiveFrom(iar, ref Remote);
                byte[] finalMessage = new byte[msgLen];
                ArrayUtils.fastCopy(buffer, finalMessage, msgLen);

                // TODO CLIENT CHECK AND HANDLING
                lock (Clients.SyncRoot)
                {
                    WorldClient value;
                    if (Clients.ContainsKey(Remote.ToString()))
                    {
                        value = Clients[Remote.ToString()] as WorldClient;
                        Store.currentClient = Clients[Remote.ToString()] as WorldClient;
                    }
                    else
                    {

                        objMan.PushClient(Remote.ToString()); // Push first, then create it
                        value = new WorldClient(Remote, socket, Remote.ToString());
                        gameServerEntities.Add(objMan.GetAssignedObject(Remote.ToString()));
                        value.playerData.setEntityId(entityIdCounter++);

                        Clients.Add(Remote.ToString(), value);
                    }
                    // Once one player enters, clean all 
                    Store.currentClient = value; // BEFORE processing

                    value.processPacket(finalMessage);
                }
                // Listening for a new message
                EndPoint newClientEP = new IPEndPoint(IPAddress.Any, 0);
                try
                {
                    socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref Remote, finalReceiveFrom, socket);
                }
                catch (SocketException ex)
                {
                    // if we get exception - remove the client
                    // ToDo:
                    if (ex.ErrorCode == 10054)
                    {
                        // Client got removed by timeout
                        Store.currentClient.Alive = false;
                    }

                }

            }
            catch (SocketException ex)
            {
                Output.WriteLine("Exception thrown by socket " + ex.SocketErrorCode + " " + ex.Message);
                if (ex.ErrorCode == 10054)
                {
                    // Client got removed by timeout
                    Store.currentClient.Alive = false;
                }
            }
                
        }

        
        private void ListenForAllClients()
        {
            byte[] byteTrue = new byte[4];
            byteTrue[byteTrue.Length - 1] = 1;
            socket.IOControl(SIO_UDP_CONNRESET, byteTrue, null);

            serverport = 10000;
            udplistener = new IPEndPoint(IPAddress.Any, serverport);
            buffer = new byte[4096];
            socket.Bind(udplistener);
    
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = sender;
            try
            {
                socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref Remote, new AsyncCallback(finalReceiveFrom), socket);
            }catch(Exception ex)
            {
                Output.WriteDebugLog("Exception Thrown ListenForAllClients " + ex.Message);
            }
            
            
        }
        

    }
}