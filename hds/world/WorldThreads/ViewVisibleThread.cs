using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using hds.shared;

namespace hds
{
    public partial class WorldThreads
    {
        public void ViewVisibleThread()
        {
            
            Output.WriteLine("[WORLD SERVER]View Visible Thread started");
            while (true)
            {
                ArrayList deadPlayers = new ArrayList();
                ArrayList removeEntities = new ArrayList();

                // Clean 
                lock (WorldSocket.Clients.SyncRoot)
                {
                    foreach (string clientKey in WorldSocket.Clients.Keys)
                    {
                        // Collect dead players to arraylist
                        WorldClient thisclient = WorldSocket.Clients[clientKey] as WorldClient;

                        if (thisclient.Alive == false)
                        {
                            // Add dead Player to the List - we need later to clear them
                            deadPlayers.Add(clientKey);

                        }
                    }
                    CleanDeadPlayers(deadPlayers);
                }

                CheckPlayerViews();
                CheckPlayerMobViews();
                //CheckForServerEntites();
                Thread.Sleep(500);
            }
        }

        private static void CheckForServerEntites()
        {
            lock (WorldSocket.gameServerEntities)
            {
                foreach (var serverEntity in WorldSocket.gameServerEntities)
                {

                    if (serverEntity.GetType() == typeof(Mob))
                    {
                        // ToDo: implement
                        continue;
                    }

                    if (serverEntity.GetType() == typeof(PlayerCharacter))
                    {
                        // ToDo: implement
                        continue;
                    }
                }
            }
        }

        private static void CheckPlayerMobViews()
        {
            // Spawn/Update for mobs
            int npcCount = WorldSocket.npcs.Count;
            for (int i = 0; i < npcCount; i++)
            {
                Mob thismob = (Mob) WorldSocket.npcs[i];

                lock (WorldSocket.Clients.SyncRoot)
                {
                    foreach (string clientKey in WorldSocket.Clients.Keys)
                    {
                        // Loop through all clients
                        WorldClient thisclient = WorldSocket.Clients[clientKey] as WorldClient;

                        if (thisclient.Alive == true)
                        {
                            // Check if

                            if (thisclient.playerData.getOnWorld() == true && thisclient.playerData.waitForRPCShutDown == false)
                            {
                                Maths math = new Maths();
                                double playerX = 0;
                                double playerY = 0;
                                double playerZ = 0;
                                NumericalUtils.LtVector3dToDoubles(thisclient.playerInstance.Position.getValue(), ref playerX, ref playerY, ref playerZ);
                                Maths mathUtils = new Maths();
                                bool mobIsInCircle = mathUtils.IsInCircle((float) playerX, (float) playerZ, (float) thismob.getXPos(), (float) thismob.getZPos(), 5000);

                                // Spawn Mob if its in Visibility Range
                                ClientView mobView = thisclient.viewMan.getViewForEntityAndGo(thismob.getEntityId(), NumericalUtils.ByteArrayToUint16(thismob.getGoId(), 1));
                                if (mobView.viewCreated == false && thismob.getDistrict() == thisclient.playerData.getDistrictId() && thisclient.playerData.getOnWorld() && mobIsInCircle)
                                {
                                    #if DEBUG
                                    ServerPackets pak = new ServerPackets();
                                    pak.sendSystemChatMessage(thisclient, "Mob with Name " + thismob.getName() + " with new View ID " + mobView.ViewID + " spawned", "BROADCAST");
                                    #endif
                                    
                                    ServerPackets mobPak = new ServerPackets();
                                    mobPak.SpawnMobView(thisclient, thismob, mobView);
                                    mobView.spawnId = thisclient.playerData.spawnViewUpdateCounter;
                                    mobView.viewCreated = true;
                                    thismob.isUpdateable = true;
                                    thismob.DoMobUpdate(thismob);
                                }

                                // Update Mob with State Data
//                                if (mobView.viewCreated == true && thismob.getDistrict() == thisclient.playerData.getDistrictId() && thisclient.playerData.getOnWorld()){
//                                    // ToDo: We need to involve the Statuslist here and we need to move them finaly
//                                    if (thismob.getIsDead() == false)
//                                    {
//                                        UpdateMob(thisclient, ref thismob, mobView);
//                                    }
//                                }

                                // Delete Mob's View from Client if we are outside
                                if (mobView.viewCreated == true && !mobIsInCircle && thismob.getDistrict() == thisclient.playerData.getDistrictId())
                                {
                                    // ToDo: delete mob
                                    ServerPackets packets = new ServerPackets();
                                    packets.sendDeleteViewPacket(thisclient, mobView.ViewID);
                                    #if DEBUG
                                    packets.sendSystemChatMessage(thisclient, "MobView (" + thismob.getName() + " LVL: " + thismob.getLevel() + " ) with View ID " + mobView.ViewID + " is out of range and is deleted!", "MODAL");
                                    #endif
                                    thisclient.viewMan.removeViewByViewId(mobView.ViewID);
                                    thismob.isUpdateable = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void CheckPlayerViews()
        {
            // Check for Player Views
            lock (WorldSocket.Clients.SyncRoot)
            {
                foreach (string clientKey in WorldSocket.Clients.Keys)
                {
                    // get Current client
                    WorldClient currentClient = WorldSocket.Clients[clientKey] as WorldClient;
                    // Loop all Clients and check if we need to create a view for it
                    foreach (string clientOtherKey in WorldSocket.Clients.Keys)
                    {
                        WorldClient otherClient = WorldSocket.Clients[clientOtherKey] as WorldClient;
                        if (otherClient != currentClient)
                        {
                            ClientView clientView = currentClient.viewMan.getViewForEntityAndGo(
                                otherClient.playerData.getEntityId(),
                                NumericalUtils.ByteArrayToUint16(otherClient.playerInstance.GetGoid(), 1));

                            // Create
                            Maths math = new Maths();
                            double currentPlayerX = 0;
                            double currentPlayerY = 0;
                            double currentPlayerZ = 0;

                            double otherPlayerX = 0;
                            double otherPlayerY = 0;
                            double otherPlayerZ = 0;

                            NumericalUtils.LtVector3dToDoubles(currentClient.playerInstance.Position.getValue(), ref currentPlayerX, ref currentPlayerY, ref currentPlayerZ);
                            NumericalUtils.LtVector3dToDoubles(currentClient.playerInstance.Position.getValue(), ref otherPlayerX, ref otherPlayerY, ref otherPlayerZ);
                            Maths mathUtils = new Maths();
                            bool playerIsInCircle = mathUtils.IsInCircle((float) currentPlayerX, (float) currentPlayerZ, (float) otherPlayerX, (float) otherPlayerZ, 5000);
                            if (clientView.viewCreated == false &&
                                currentClient.playerData.getDistrictId() == otherClient.playerData.getDistrictId() &&
                                otherClient.playerData.getOnWorld() && currentClient.playerData.getOnWorld() &&
                                playerIsInCircle)
                            {
                                // Spawn player
                                ServerPackets pak = new ServerPackets();
                                pak.sendSystemChatMessage(currentClient,
                                    "Player " + otherClient.playerInstance.GetName() + " with new View ID " +
                                    clientView.ViewID + " jacked in", "BROADCAST");
                                pak.sendPlayerSpawn(currentClient, otherClient, clientView.ViewID);
                                clientView.spawnId = currentClient.playerData.spawnViewUpdateCounter;
                                clientView.viewCreated = true;
                            }


                            if (clientView.viewCreated && !playerIsInCircle)
                            {
                                // ToDo: delete mob
                                ServerPackets packets = new ServerPackets();
                                packets.sendSystemChatMessage(currentClient,
                                    "Player " + otherClient.playerInstance.GetName() + " with View ID " + clientView.ViewID +
                                    " jacked out!", "MODAL");
                                packets.sendDeleteViewPacket(currentClient, clientView.ViewID);
                                currentClient.viewMan.removeViewByViewId(clientView.ViewID);
                            }
                        }
                    }
                }
            }
        }

        private static void CleanDeadPlayers(ArrayList deadPlayers)
        {
            foreach (string key in deadPlayers)
            {
                WorldClient deadClient = WorldSocket.Clients[key] as WorldClient;
                foreach (string client in WorldSocket.Clients.Keys)
                {
                    WorldClient otherclient = WorldSocket.Clients[client] as WorldClient;
                    ClientView view = otherclient.viewMan.getViewForEntityAndGo(deadClient.playerData.getEntityId(), NumericalUtils.ByteArrayToUint16(deadClient.playerInstance.GetGoid(), 1));

                    ServerPackets pak = new ServerPackets();
                    pak.sendDeleteViewPacket(otherclient, view.ViewID);
                    Store.margin.removeClientsByCharId(otherclient.playerData.getCharID());
                }

                // Views are now deleted to other players
                // ToDo: Cleanup Missions (kill all running missions the player have)
                // ToDo: Cleanup Teams (if your mission team has more than one player, you need to announce an update for the mission team to your mates)
                // ToDo: Announce friendlists from other users that you are going offline (just collect all players whohave this client in list and send the packet)
                // ToDo: Finally save the current character Data to the Database^^
                Output.WriteLine("Removed inactive Client with Key " + key);
                lock (WorldSocket.Clients.SyncRoot)
                {
                    WorldSocket.Clients.Remove(key);
                }
                
            }
        }


    }
}
