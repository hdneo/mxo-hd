using System;
using System.Collections.Generic;

#nullable disable

namespace hds.databases.Entities
{
    public partial class CrewMember
    {
        public int Id { get; set; }
        public int CrewId { get; set; }
        public int CharId { get; set; }
        public int IsCaptain { get; set; }
        public int IsFirstMate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
