using System.Collections.Generic;

namespace hds
{
    public class MissionPhase
    {
        public List<MissionDialog> dialogs = new List<MissionDialog>();
        public List<Mob> npcs = new List<Mob>();
        public List<GameObject> gameObjects = new List<GameObject>();
        public List<MissionObjective> missionObjectives = new List<MissionObjective>();
    }
}