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
            int npcCount = WorldSocket.mobs.Count;
            lock (WorldSocket.mobs.SyncRoot)
            {
                for (int i = 0; i < npcCount; i++)
                {
                    Mob thismob = (Mob)WorldSocket.mobs[i];
                    // Check if Client has a view for this mob

                    if (thismob.getIsSpawned() == true)
                    {
                        thismob.DoMobUpdate(thismob);
                    }

                }
            }


            // ToDo: This is the Mover Update Thread to update objects like NPCs every interval
            Thread.Sleep(1000);
        }
    }
}