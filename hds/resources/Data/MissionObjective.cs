using System;
using System.Collections.Generic;

namespace hds
{
    public class MissionObjective
    {
        public string description;
        public string dialog;
        public string command; // Maybe ENUM But let us stay more dynamic ?
        public byte[] itemId;
        public LtVector3f missionArea;
    }
}