using System;
using System.Collections.Generic;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Net;

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

        public static Dictionary<string, WorldClient> Clients { get; set; }

        // Collection to store the spawned npcs 
        public static ArrayList npcs = ArrayList.Synchronized(new ArrayList());
        public static ArrayList missionTeams = ArrayList.Synchronized(new ArrayList());
        public static UInt64 entityIdCounter = 1;
        private byte[] buffer;
        
        public ObjectManager objMan { get; set; }


        public WorldSocket()
        {

            serverport = 10000;
            udplistener = new IPEndPoint(IPAddress.Any, serverport);

            Clients = new Dictionary<string, WorldClient>();

            listenThread = new Thread(new ThreadStart(ListenForAllClients));
            mainThreadWorking = true;

            objMan = new ObjectManager();

            Output.WriteLine("[World Server] Set and ready at UDP port 10000");
        }

        public void startServer()
        {
            listenThread.Start();
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
            lock (Clients)
            {
                foreach (string client in Clients.Keys)
                {
                    if (Clients[client].playerData.getCharID() == charId)
                    {
                        Clients[client].messageQueue.addRpcMessage(packet);
                        Clients[client].flushQueue();
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
            lock(Clients){
                foreach (string client in Clients.Keys)
                {
                    // Populate a message to all players 
                    // WE TEST THIS HERE!


                    if (Clients[client].playerData.getCharID() != myself.getCharID())
                    {

                        // create the RPC Message
                        ArrayList content = new ArrayList();
                        content.Add(data);
                        
                        WorldClient worldClient = Clients[client];
                        worldClient.messageQueue.addRpcMessage(data);
                        worldClient.flushQueue();
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
            lock (Clients)
            {
                foreach (string client in Clients.Keys)
                {
                    Clients[client].messageQueue.addRpcMessage(data);
                    Clients[client].flushQueue();
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
            lock (Clients)
            {
                foreach (string client in Clients.Keys)
                {
                    if (Clients[client].playerData.getCharID() != charId)
                    {
                        Output.Write("[ViewThread] Handle View For all Packet for charId : " + charId.ToString());
                        // Get or generate a View for the GoID 
                        ClientView view = Clients[client].viewMan.getViewForEntityAndGo(entityId, goId);
                        DynamicArray content = new DynamicArray();

                        if (view.viewCreated == false)
                        {
                            Output.WriteDebugLog("Created View Id : " + view.ViewID.ToString());
                            // if view is new , add the viewId at the end (cause its creation)
                            // Remove the 03 01 00 (as it was generate previosly)
                            content.append(data);
                            content.append(NumericalUtils.uint16ToByteArray(view.ViewID, 1));
                            content.append(0x00); // View Zero  

                        }
                        else
                        {
                            // Update View
                            content.append(NumericalUtils.uint16ToByteArray(view.ViewID, 1));
                            content.append(data);
                            content.append(0x00); // View Zero  
                            
                            Output.WriteDebugLog("Update View Id : " + view.ViewID.ToString());
                        }

                        if (view.viewNeedsToBeDeleted == true)
                        {
                            
                            content.append(0x01);
                            content.append(0x00);
                            content.append(0x01); // Comand (Delete)
                            content.append(0x01); // NumViews (1 currently)
                            content.append(0x00);
                            content.append(NumericalUtils.uint16ToByteArray(view.ViewID, 1));
                            content.append(0x00);
                        }

                        
                        // ToDo: handle viewId for packets (For creation it needs to be append to the end, for update at the start )
                        // ToDo: Complete this :)
                        Clients[client].messageQueue.addObjectMessage(content.getBytes(), false);
                        Clients[client].flushQueue();
                        Output.WriteLine("[World View Server] CharId: " + Clients[client].playerData.getCharID().ToString() + " )");
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
            }catch(SocketException ex)
            {

            }
            finally
            {
                byte[] finalMessage = new byte[msgLen];
                ArrayUtils.fastCopy(buffer, finalMessage, msgLen);

                string key = Remote.ToString();


                // TODO CLIENT CHECK AND HANDLING
                WorldClient value;
                if (Clients.TryGetValue(key, out value))
                {
                    Store.currentClient = value; // BEFORE processing
                }
                else
                {
                    objMan.PushClient(key); // Push first, then create it
                    value = new WorldClient(Remote, socket, key);
                    value.playerData.setEntityId(WorldSocket.entityIdCounter++);

                    Clients.Add(key, value);
                    // Once one player enters, clean all 
                    Store.currentClient = value; // BEFORE processing
                }
                value.processPacket(finalMessage);

                // Listening for a new message
                EndPoint newClientEP = new IPEndPoint(IPAddress.Any, 0);
                socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref Remote, finalReceiveFrom, socket);
            }
                
        }

        
        private void ListenForAllClients()
        {
            buffer = new byte[4096];
            socket.Bind(this.udplistener);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)(sender);
            socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref Remote, finalReceiveFrom, socket);
            
        }
        
        /*
        private void ListenForAllClients()
        {

            socket.Bind(this.udplistener);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)(sender);


            while (mainThreadWorking)
            {

                byte[] message = new byte[4096];
                int receivedDataLenght = socket.ReceiveFrom(message, ref Remote);
                byte[] finalMessage = new byte[receivedDataLenght];

                // Use fast copy to do express operations
                ArrayUtils.fastCopy(message, finalMessage, receivedDataLenght);
                string key = Remote.ToString();


                // TODO CLIENT CHECK AND HANDLING
                WorldClient value;
                if (Clients.TryGetValue(key, out value))
                {
                    Store.currentClient = value; // BEFORE processing
                }
                else
                {
                    objMan.PushClient(key); // Push first, then create it
                    value = new WorldClient(Remote, socket, key);
                    value.playerData.setEntityId(WorldSocket.entityIdCounter++);

                    Clients.Add(key, value);
                    // Once one player enters, clean all 
                    Store.currentClient = value; // BEFORE processing
                }
                value.processPacket(finalMessage);
            }
        }*/

    }
}