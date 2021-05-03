using System;
using System.Collections.Generic;

#nullable disable

namespace hds.databases.Entities
{
    public partial class Character
    {
        public ulong CharId { get; set; }
        public uint UserId { get; set; }
        public ushort WorldId { get; set; }
        public byte Status { get; set; }
        public string Handle { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Background { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public int Rotation { get; set; }
        public int HealthC { get; set; }
        public int HealthM { get; set; }
        public int InnerStrC { get; set; }
        public int InnerStrM { get; set; }
        public int Level { get; set; }
        public short Profession { get; set; }
        public short Alignment { get; set; }
        public short Pvpflag { get; set; }
        public int ConquestPoints { get; set; }
        public int Exp { get; set; }
        public int Cash { get; set; }
        public int RepMero { get; set; }
        public int RepMachine { get; set; }
        public int RepNiobe { get; set; }
        public int RepEpn { get; set; }
        public int RepCyph { get; set; }
        public int RepGm { get; set; }
        public int RepZion { get; set; }
        public string District { get; set; }
        public byte DistrictId { get; set; }
        public uint FactionId { get; set; }
        public uint CrewId { get; set; }
        public int IsDeleted { get; set; }
        public sbyte IsOnline { get; set; }
        public DateTime Created { get; set; }
    }
}
