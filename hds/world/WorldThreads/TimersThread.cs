using System.Collections.Generic;
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
                Thread.Sleep(2000);
                lock (WorldSocket.Clients)
                {
                    CheckAndResendClients();
                    SavePlayers();
                }
            }
        }
        
        private static void CheckAndResendClients()
        {
            foreach (string client in WorldSocket.Clients.Keys)
            {
                WorldClient otherclient = WorldSocket.Clients[client] as WorldClient;
                otherclient.CheckAndResend();
            }
        }

        private static void SavePlayers()
        {
            foreach (string clientKey in WorldSocket.Clients.Keys)
            {
                WorldClient thisclient = WorldSocket.Clients[clientKey] as WorldClient;
                if (thisclient != null && thisclient.playerData.lastSaveTime == 0)
                {
                    thisclient.playerData.lastSaveTime = TimeUtils.getUnixTimeUint32();
                }

                CheckToSaveClientData(thisclient);                
                CheckForFinalJackout(thisclient);
                CheckForStaticViews(thisclient);
            }
        }

        private static void CheckForFinalJackout(WorldClient thisclient)
        {
            if (thisclient.playerData.isJackoutInProgress == true &&
                (thisclient.playerData.jackoutStartTime - TimeUtils.getUnixTimeUint32()) > 5)
            {
                ServerPackets packets = new ServerPackets();
                packets.sendExitGame(thisclient);
            }
        }

        private static void CheckToSaveClientData(WorldClient thisclient)
        {
            if (thisclient != null && (TimeUtils.getUnixTimeUint32() - thisclient.playerData.lastSaveTime) > 20)
            {
                thisclient.playerData.lastSaveTime = TimeUtils.getUnixTimeUint32();
                // Save Player
                new PlayerHelper().savePlayerInfo(thisclient);
                // Notify Player about save
                ServerPackets pak = new ServerPackets();
                pak.sendSaveCharDataMessage(thisclient,
                    StringUtils.charBytesToString_NZ(thisclient.playerInstance.CharacterName.getValue()));
            }
        }

        private static void CheckForStaticViews(WorldClient thisclient)
        {
            //List<StaticWorldObject> staticWorldObjects = DataLoader.getInstance().findObjectsInSectorAndWorld(thisclient.playerData.getDistrictId(), 1);
        }
    }
}