using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using hds.resources.gameobjects;
using hds.shared;

namespace hds
{
    
    public class WorldServer
    {

        private IPEndPoint udplistener;
        private Thread listenThread;
        private int serverport;
        private bool mainThreadWorking;    
        private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
        public const int SIO_UDP_CONNRESET = -1744830452;

        public static Dictionary<string, WorldClient> Clients { get; set; }

        // Collection to store the spawned npcs 
        public static ArrayList mobs = ArrayList.Synchronized(new ArrayList());
        public static ArrayList subways = ArrayList.Synchronized(new ArrayList());
        public static ArrayList missionTeams = ArrayList.Synchronized(new ArrayList());
        public static UInt64 entityIdCounter = 1;
        public static ArrayList gameServerEntities = ArrayList.Synchronized(new ArrayList()); // should hold all GameObject Entities where a view can be created (static, dynamic etc.)
        private byte[] buffer;
        
        public ObjectManager objMan { get; set; }


        public WorldServer()
        {

            Clients = new Dictionary<string, WorldClient>();
            objMan = new ObjectManager();

            listenThread = new Thread(ListenForAllClients);
            mainThreadWorking = true;
            Output.WriteLine("[World Server] Set and ready at UDP port 10000");
        }

        public void StartServer()
        {
            listenThread.Start();
            mainThreadWorking = true;
        }

        public void StopServer()
        {
            mainThreadWorking = false;
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
                lock (Clients)
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
        

        /// <summary>
        /// Send a Message to One Player (like private Message)
        /// </summary>
        /// <param name="charId">CharId to send the Packet</param>
        /// <param name="packet">Packet Stream</param>
        public void SendRPCToOnePlayer(UInt32 charId, byte[] packet)
        {
            lock (Clients)
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
        public void SendRPCToOnePlayerByHandle(byte[] packet, string playerHandle)
        {
            lock (Clients)
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
        public void SendRPCToAllOtherPlayers(ClientData myself, byte[] data)
        {
            // Send a global message to all connected Players (like shut down Server announce or something)
            lock(Clients){
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

        public void SendRPCToCrewMembers(UInt32 groupId, WorldClient myself, byte[] data, bool ShouldSendToMyself)
        {
            // Send a global message to all connected Players (like shut down Server announce or something)
            lock(Clients){
                foreach (string clientKey in Clients.Keys)
                {
                    // Populate a message to all players to my crew
                    WorldClient client = Clients[clientKey] as WorldClient;
                    if (NumericalUtils.ByteArrayToUint32(client.playerInstance.CrewID.getValue(),1) == groupId)
                    {
                        if (client.playerData.getCharID() != myself.playerData.getCharID() || ShouldSendToMyself)
                        {
                            // create the RPC Message
                            client.messageQueue.addRpcMessage(data);
                            client.FlushQueue();    
                        }
                    }

                }
            }
        }
        
        public void SendRPCToFactionMembers(UInt32 groupId, WorldClient myself, byte[] data, bool ShouldSendToMyself)
        {
            // Send a global message to all connected Players (like shut down Server announce or something)
            lock (Clients)
            {
                var clients = from clientCollection in Clients
                    where NumericalUtils.ByteArrayToUint32(clientCollection.Value.playerInstance.FactionID.getValue(),1) == groupId
                    select clientCollection;

                foreach (KeyValuePair<string, WorldClient> theClient in clients)
                {
                    WorldClient client = theClient.Value;

                    if (client.playerData.getCharID() != myself.playerData.getCharID() || ShouldSendToMyself)
                    {
                        // create the RPC Message
                        client.messageQueue.addRpcMessage(data);
                        client.FlushQueue();
                    }
                }
            }
        }
        
        public void SendRPCToMissionTeamMembers(UInt32 groupId, WorldClient myself, byte[] data, bool ShouldSendToMyself)
        {
            // Send a global message to all connected Players (like shut down Server announce or something)
            lock (Clients)
            {
                var clients = from clientCollection in Clients
                    where NumericalUtils.ByteArrayToUint32(
                        clientCollection.Value.playerInstance.MissionTeamID.getValue(), 1) == groupId
                    select clientCollection;

                foreach (KeyValuePair<string, WorldClient> theClient in clients)
                {
                    WorldClient client = theClient.Value;
                    // Populate a message to all players to my crew

                    if (client.playerData.getCharID() != myself.playerData.getCharID() || ShouldSendToMyself)
                    {
                        // create the RPC Message
                        client.messageQueue.addRpcMessage(data);
                        client.FlushQueue();
                    }
                }
            }
        }

        public void SendRPCToPlayerList(ArrayList players, byte[] data)
        {
            List<string> handles = new List<string>();
            foreach (Hashtable friend in players)
            {
                handles.Add(friend["handle"].ToString());
            }

            lock (Clients)
            {
                foreach (string clientKey in Clients.Keys)
                {
                    // Populate a message to all players to my crew
                    WorldClient client = Clients[clientKey] as WorldClient;

                    string handleRecipient =
                        StringUtils.charBytesToString_NZ(client.playerInstance.CharacterName.getValue());
                    if (handles.Contains(handleRecipient))
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
            lock (Clients)
            {
                foreach (string clientKey in Clients.Keys)
                {
                    WorldClient client = Clients[clientKey];
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
        public void SendViewPacketToAllPlayers(byte[] data,UInt32 charId, UInt32 goId, UInt64 entityId)
        {
            // Send a global message to all connected Players (like shut down Server announce or something)
            lock (Clients)
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
                        ClientView view = client.viewMan.GetViewForEntityAndGo(entityId, goId);
                        PacketContent viewPacket = new PacketContent();

                        if (view.viewCreated == false)
                        {
                            // if view is new , add the viewId at the end (cause its creation)
                            // Remove the 03 01 00 (as it was generate previosly)
                            viewPacket.AddByteArray(data);
                            viewPacket.AddUint16(view.ViewID,1);
                            viewPacket.AddByte(0x00);

                        }
                        else
                        {
                            // Update View
                            viewPacket.AddUint16(view.ViewID,1);
                            viewPacket.AddByteArray(data);
                            viewPacket.AddByte(0x00);  
                        }

                        if (view.viewNeedsToBeDeleted)
                        {
                            // Delete one View
                            viewPacket.AddByte(0x01);
                            viewPacket.AddByte(0x00);
                            viewPacket.AddByte(0x01); // Comand (Delete)
                            viewPacket.AddUint16(1,1); // NumViews to Delete
                            viewPacket.AddUint16(view.ViewID,1);
                            viewPacket.AddByte(0x00);
                        }

                        
                        client.messageQueue.addObjectMessage(viewPacket.ReturnFinalPacket(), false);
                        client.FlushQueue();
                    }
                }
            }
        }

        public void SendSelfViewUpdate(PacketContent pak, UInt16 viewId, WorldClient client, bool isTimed)
        {
            ServerPackets serverPackets = new ServerPackets();
            serverPackets.SendUpdateViewStatePacket(client, viewId, pak.ReturnFinalPacket());
        }

        public void SendSelfViewUpdateToTarget(PacketContent pak, UInt16 targetViewId, WorldClient currentClient)
        {

            ClientView theView = currentClient.viewMan.getViewById(targetViewId);
            if (theView == null)
            {
                return;
            }
            
            if (theView.GoID != 12)
            {
                // it is no player - so we do nothing
                return;
            }

            lock (Clients)
            {
                
                foreach (string clientKey in Clients.Keys)
                {
                    WorldClient target = Clients[clientKey];
                    if (target.playerData.getEntityId() == theView.entityId)
                    {
                        Output.WriteDebugLog("Update SelfViewState from Ability FX on OtherState Views from " + StringUtils.charBytesToString(currentClient.playerInstance.CharacterName.getValue()) + " to  " + StringUtils.charBytesToString(target.playerInstance.CharacterName.getValue()) + " HEX: " + StringUtils.bytesToString_NS(pak.ReturnFinalPacket()));
                        target.messageQueue.addObjectMessage(pak.ReturnFinalPacket(), false);
                        target.FlushQueue();
                        return;
                    }
                }
            }
        }

        public void SendViewUpdateToClientWhoHasStaticObjectSpawned(PacketContent packet, StaticWorldObject worldObject, string debugMessage)
        {
            lock (Clients)
            {
                foreach (string clientKey in Clients.Keys)
                {
                    String entityHackString = "" + worldObject.metrId + "" + worldObject.mxoStaticId;
                    UInt64 entityStaticId = UInt64.Parse(entityHackString);
                    
                    WorldClient client = Clients[clientKey] as WorldClient;
                    ClientView objectView = client.viewMan.GetViewForEntityAndGo(entityStaticId, NumericalUtils.ByteArrayToUint16(worldObject.type, 1));
                    if (objectView.viewCreated && worldObject.metrId == client.playerData.getDistrictId() && client.playerData.getOnWorld())
                    {
                        ServerPackets pak = new ServerPackets();
                        pak.SendUpdateViewStatePacket(client, objectView.ViewID, packet.ReturnFinalPacket());
                        pak.sendSystemChatMessage(client, debugMessage, "BROADCAST");
                    }
                }
            }
        }

        public void SendViewUpdateToClientsWhoHasMobSpawned(PacketContent packet, Mob mob)
        {
            lock (Clients)
            {
                foreach (string clientKey in Clients.Keys)
                {
                    WorldClient client = Clients[clientKey] as WorldClient;
                    ClientView mobView = client.viewMan.GetViewForEntityAndGo(mob.getEntityId(), NumericalUtils.ByteArrayToUint16(mob.getGoId(), 1));
                    if (mobView.viewCreated == true && mob.getDistrict() == client.playerData.getDistrictId() && client.playerData.getOnWorld())
                    {
                        ServerPackets pak = new ServerPackets();
                        pak.SendNpcUpdateData(mobView.ViewID, client, packet.ReturnFinalPacket());
                    }
                }
            }
        }


    }
}