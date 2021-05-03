using System;
using System.Collections.Generic;

#nullable disable

namespace hds.databases.Entities
{
    public partial class CharAbility
    {
        public uint Id { get; set; }
        public uint CharId { get; set; }
        public int Slot { get; set; }
        public long AbilityId { get; set; }
        public uint Level { get; set; }
        public bool IsLoaded { get; set; }
        public string AbilityName { get; set; }
        public DateTime Added { get; set; }
    }
}
