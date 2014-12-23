using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using hds.shared;
using hds;

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
                lock (WorldSocket.Clients)
                {
                    foreach (string client in WorldSocket.Clients.Keys)
                    {
                        // Collect dead players to arraylist
                        WorldClient thisclient = WorldSocket.Clients[client];

                        if (thisclient.Alive == false)
                        {
                            // Add dead Player to the List - we need later to clear them
                            deadPlayers.Add(client);

                        }
                    }
                    cleanDeadPlayers(deadPlayers);
                }
                


                // Spawn/Update for mobs
                int npcCount = WorldSocket.npcs.Count;
                for (int i = 0; i < npcCount; i++)
                {
                    npc thismob = (npc)WorldSocket.npcs[i];
 
                    lock (WorldSocket.Clients)
                    {
                        foreach (string client in WorldSocket.Clients.Keys)
                        {
                            // Loop through all clients
                            WorldClient thisclient = WorldSocket.Clients[client];

                            if (thisclient.Alive == true)
                            {
                                // Check if 
                                
                                if (thisclient.playerData.getOnWorld() == true)
                                {
                                    ClientView mobView = thisclient.viewMan.getViewForEntityAndGo(thismob.getEntityId(), NumericalUtils.ByteArrayToUint16(thismob.getGoId(), 1));
                                    Maths math = new Maths();
                                    double playerX = 0;
                                    double playerY = 0;
                                    double playerZ = 0;
                                     NumericalUtils.LtVector3dToDoubles(thisclient.playerInstance.Position.getValue(), ref playerX, ref playerY, ref playerZ);
                                    Maths mathUtils = new Maths();
                                    bool mobIsInCircle = mathUtils.IsInCircle((float)playerX,(float)playerZ,(float)thismob.getXPos(),(float)thismob.getZPos(),500);
                                    
                                    // ToDo: Check if mob is in circle of player (radian some value that is in a middle range for example 300m)

                                    // Create
                                    if (mobView.viewCreated == false && thismob.getDistrictName() == thisclient.playerData.getDistrict() && thisclient.playerData.getOnWorld() && mobIsInCircle)
                                    {

                                        ServerPackets mobPak = new ServerPackets();
                                        mobPak.spawnMobView(thisclient, thismob, mobView);
                                    }

                                    // Update Mob
                                    if (mobView.viewCreated == true && thismob.getDistrictName() == thisclient.playerData.getDistrict() && thisclient.playerData.getOnWorld())
                                    {
                                        // ToDo: We need to involve the Statuslist here and we need to move them finaly
                                        updateMob(thisclient, ref thismob, mobView);
                                    }

                                    // Mob moves outside - should delete it
                                    if (mobView.viewCreated == true && !mobIsInCircle && thismob.getDistrictName() == thisclient.playerData.getDistrict())
                                    {
                                        // ToDo: delete mob
                                    }
                                }

                            }
                        }
                    }
                    thismob.updateClient=false;
                    

                }
                Thread.Sleep(500);
            }
        }

        private static void cleanDeadPlayers(ArrayList deadPlayers)
        {
            foreach (string key in deadPlayers)
            {
                WorldClient deadClient = WorldSocket.Clients[key];
                foreach (string client in WorldSocket.Clients.Keys)
                {
                    WorldClient otherclient = WorldSocket.Clients[client];
                    ClientView view = otherclient.viewMan.getViewForEntityAndGo(deadClient.playerData.getEntityId(), NumericalUtils.ByteArrayToUint16(deadClient.playerInstance.getGoid(), 1));

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
                WorldSocket.Clients.Remove(key);
            }
        }

        private static void updateMob(WorldClient client, ref npc thismob, ClientView mobView)
        {
            bool clientUpdate = thismob.doTick();
            if (clientUpdate == true)
            {
                // Needs to send the packet to every spawned client who has this view 
                byte[] updateData = thismob.getAndResetUpdateData();
                ServerPackets pak = new ServerPackets();
                pak.sendNPCUpdateData(mobView.ViewID, client, updateData);
                
            }
        }

    }
}
