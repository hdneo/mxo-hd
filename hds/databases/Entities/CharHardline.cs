using System;
using System.Collections.Generic;

#nullable disable

namespace hds.databases.Entities
{
    public partial class CharHardline
    {
        public uint Id { get; set; }
        public uint CharId { get; set; }
        public uint HlId { get; set; }
        public uint DistrictId { get; set; }
        public DateTime Added { get; set; }
    }
}
