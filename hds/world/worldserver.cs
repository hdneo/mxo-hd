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

        public const int WorldPort = 10000;
        private IPEndPoint udplistener;
        private int serverport;
        private readonly Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
        public const int SIO_UDP_CONNRESET = -1744830452;

        /// <summary>Connected clients by remote endpoint. Only accessed from the world thread.</summary>
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
        }

        public void StartServer()
        {
            ListenForAllClients();
            Output.WriteLine("[World Server] Set and ready at UDP port " + WorldPort);
        }

        public void StopServer()
        {
            socket.Close();
        }
        
        
        /// <summary>Network thread: copy the datagram and hand it to the world thread.</summary>
        private void finalReceiveFrom(IAsyncResult iar)
        {
            EndPoint remote = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                int msgLen = socket.EndReceiveFrom(iar, ref remote);
                byte[] message = new byte[msgLen];
                Buffer.BlockCopy(buffer, 0, message, 0, msgLen);
                EndPoint sender = remote;
                Store.Loop.Post(() => HandleDatagram(sender, message));
            }
            catch (ObjectDisposedException)
            {
                return; // socket closed on shutdown
            }
            catch (SocketException ex)
            {
                Output.WriteLine("Exception thrown by socket " + ex.SocketErrorCode + " " + ex.Message);
            }

            BeginReceive();
        }

        private void BeginReceive()
        {
            EndPoint any = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref any, finalReceiveFrom, socket);
            }
            catch (Exception ex)
            {
                Output.WriteDebugLog("Exception thrown in BeginReceive " + ex.Message);
            }
        }

        /// <summary>World thread: find or create the session of the sender and process the packet.</summary>
        private void HandleDatagram(EndPoint remote, byte[] message)
        {
            string key = remote.ToString();
            if (!Clients.TryGetValue(key, out WorldClient client))
            {
                objMan.PushClient(key); // Push first, then create it
                client = new WorldClient(remote, socket, key);
                gameServerEntities.Add(objMan.GetAssignedObject(key));
                client.playerData.setEntityId(entityIdCounter++);
                Clients.Add(key, client);
            }

            // Legacy code still reads the "current" client - only valid on the world thread
            Store.currentClient = client;
            client.processPacket(message);
        }

        private void ListenForAllClients()
        {
            if (OperatingSystem.IsWindows())
            {
                // Ignore ICMP port unreachable, otherwise one disconnected client breaks the receive loop
                byte[] byteTrue = new byte[4];
                byteTrue[byteTrue.Length - 1] = 1;
                socket.IOControl(SIO_UDP_CONNRESET, byteTrue, null);
            }

            serverport = WorldPort;
            udplistener = new IPEndPoint(IPAddress.Any, serverport);
            buffer = new byte[4096];
            socket.Bind(udplistener);
            BeginReceive();
        }

        /// <summary>
        /// Sends a view update of <paramref name="subject"/> (component mask + data, without view id) to every
        /// other client that currently has a view of it.
        /// </summary>
        public void SendViewUpdateToObservers(WorldClient subject, byte[] updateData)
        {
            ulong entityId = subject.playerData.getEntityId();
            foreach (WorldClient observer in Clients.Values)
            {
                if (observer == subject) continue;
                ClientView view = observer.viewMan.FindViewForEntity(entityId);
                if (view == null || !view.viewCreated) continue;

                var packet = new hds.Core.Wire.WireWriter(updateData.Length + 2);
                packet.WriteUInt16(view.ViewID);
                packet.WriteBytes(updateData);
                observer.messageQueue.addObjectMessage(packet.ToArray(), false);
                observer.FlushQueue();
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
        /// Sends a view update (component mask + data, without view id) of the given player entity to all
        /// other players that have a view of it. Views are created by the view visibility system.
        /// </summary>
        public void SendViewPacketToAllPlayers(byte[] data, UInt32 charId, UInt32 goId, UInt64 entityId)
        {
            foreach (WorldClient observer in Clients.Values)
            {
                if (observer.playerData.getCharID() == charId) continue;
                ClientView view = observer.viewMan.FindViewForEntity(entityId);
                if (view == null || !view.viewCreated) continue;

                PacketContent viewPacket = new PacketContent();
                viewPacket.AddUint16(view.ViewID, 1);
                viewPacket.AddByteArray(data);
                observer.messageQueue.addObjectMessage(viewPacket.ReturnFinalPacket(), false);
                observer.FlushQueue();
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
            SendViewUpdateToClientsWhoHasMobSpawned(packet.ReturnFinalPacket(), mob);
        }

        /// <summary>Sends a view update (without view id) to every client that has a view of the mob.</summary>
        public void SendViewUpdateToClientsWhoHasMobSpawned(byte[] update, Mob mob)
        {
            lock (Clients)
            {
                foreach (string clientKey in Clients.Keys)
                {
                    WorldClient client = Clients[clientKey] as WorldClient;
                    // only look up: GetViewForEntityAndGo would create views for clients that never saw the mob
                    ClientView mobView = client?.viewMan.FindViewForEntity(mob.getEntityId());
                    if (mobView != null && mobView.viewCreated && mob.getDistrict() == client.playerData.getDistrictId() &&
                        client.playerData.getOnWorld())
                    {
                        new ServerPackets().SendNpcUpdateData(mobView.ViewID, client, update);
                    }
                }
            }
        }

    }
}