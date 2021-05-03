using System;
using System.Collections.Generic;

#nullable disable

namespace hds.databases.Entities
{
    public partial class Crew
    {
        public int Id { get; set; }
        public string CrewName { get; set; }
        public string MasterPlayerHandle { get; set; }
        public uint Money { get; set; }
        public int Org { get; set; }
        public int FactionId { get; set; }
        public int FactionRank { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
