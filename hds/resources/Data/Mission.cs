using System;
using System.Collections.Generic;

namespace hds
{
    public class Mission
    {
        public string title;
        public string description;
        public int exp = 500; // ToDo: ths is the base amount (but we need to take care of the "Time" and "Diffulty" 
        public int info = 1000;
        public int repZion = 1;
        public int repMachine = 1;
        public int repMero = 1;
        public enum DIFFICULT { EASY, NORMAL, HARD }
        public enum MISSION_LENGTH { SHORT, NORMAL, LONG }

        public DIFFICULT missionDifficult = DIFFICULT.EASY;
        public MISSION_LENGTH missionLength = MISSION_LENGTH.SHORT;
        public List<MissionPhase> phases = new List<MissionPhase>(); 
    }
}