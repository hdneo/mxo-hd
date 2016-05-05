using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace hds
{
    public partial class WorldThreads
    {
        public void MoverThreadProcess()
        {
            Output.WriteLine("[WORLD SERVER]MoverThread started");

            // Moove the Mobs a little bit around 
            int npcCount = WorldSocket.npcs.Count;
            lock (WorldSocket.npcs.SyncRoot)
            {
                for (int i = 0; i < npcCount; i++)
                {
                    npc thismob = (npc)WorldSocket.npcs[i];
                    // Check if Client has a view for this mob

                    if (thismob.getIsSpawned() == true)
                    {
                        thismob.doTick();
                    }

                }
            }
            

            // ToDo: This is the Mover Update Thread to update objects like NPCs every interval
            Thread.Sleep(1000);
        }
    }
}