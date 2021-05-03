using System;
using System.Collections.Generic;

#nullable disable

namespace hds.databases.Entities
{
    public partial class World
    {
        public ushort WorldId { get; set; }
        public string Name { get; set; }
        public byte Type { get; set; }
        public byte Status { get; set; }
        public byte Load { get; set; }
        public uint NumPlayers { get; set; }
    }
}
