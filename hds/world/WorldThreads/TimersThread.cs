using System.Threading;

namespace hds
{
    public partial class WorldThreads
    {
        public void TimersThreadProcess()
        {
            Output.WriteLine("[WORLD SERVER]TimersThread started");
            while (true)
            {
                // ToDo: This should update "timers" like Buffs, Skill Execution or something
                Thread.Sleep(1000);
                // Update Client Data (Buffs ?)
                lock (WorldSocket.Clients.SyncRoot)
                {
                    foreach(string clientKey in WorldSocket.Clients.Keys){
                        WorldClient thisclient = WorldSocket.Clients[clientKey] as WorldClient;
                        if (thisclient != null && thisclient.playerData.lastSaveTime == 0)
                        {
                            thisclient.playerData.lastSaveTime = TimeUtils.getUnixTimeUint32();
                        }

                        if (thisclient != null && (TimeUtils.getUnixTimeUint32() - thisclient.playerData.lastSaveTime) > 20)
                        {
                            thisclient.playerData.lastSaveTime = TimeUtils.getUnixTimeUint32();
                            ServerPackets pak = new ServerPackets();
                            pak.sendSaveCharDataMessage(thisclient, StringUtils.charBytesToString_NZ(thisclient.playerInstance.CharacterName.getValue()));
                        }

                    }
                }
            }
        }
    }
}