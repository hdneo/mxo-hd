using System;
using System.Collections.Generic;

#nullable disable

namespace hds.databases.Entities
{
    public partial class DataHardline
    {
        public uint Id { get; set; }
        public ushort HardLineId { get; set; }
        public long ObjectId { get; set; }
        public string HardlineName { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Rot { get; set; }
        public ushort DistrictId { get; set; }
    }
}
