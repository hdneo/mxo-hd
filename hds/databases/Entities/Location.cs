using System;
using System.Collections.Generic;

#nullable disable

namespace hds.databases.Entities
{
    public partial class Location
    {
        public uint Id { get; set; }
        public string Command { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public byte District { get; set; }
    }
}
