using System;
using System.Collections.Generic;

namespace hds.world.Structures
{
    public class Faction
    {
        public UInt32 factionId;
        public string name;
        public UInt32 masterPlayerCharId;
        public string masterPlayerHandle;
        public UInt32 money;
        public List<Crew> crews = new List<Crew>();
    }
}