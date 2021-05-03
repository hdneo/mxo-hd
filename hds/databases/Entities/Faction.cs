using System;
using System.Collections.Generic;

#nullable disable

namespace hds.databases.Entities
{
    public partial class Faction
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MasterPlayerHandle { get; set; }
        public int Money { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
