using System;
using System.Collections.Generic;

#nullable disable

namespace hds.databases.Entities
{
    public partial class Inventory
    {
        public uint InvId { get; set; }
        public ulong CharId { get; set; }
        public long Goid { get; set; }
        public byte Slot { get; set; }
        public int Count { get; set; }
        public int Purity { get; set; }
        public DateTime Updated { get; set; }
        public DateTime Created { get; set; }
    }
}
