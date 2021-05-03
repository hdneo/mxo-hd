using System;
using System.Collections.Generic;

#nullable disable

namespace hds.databases.Entities
{
    public partial class User
    {
        public uint UserId { get; set; }
        public string Username { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordHash { get; set; }
        public ushort PublicExponent { get; set; }
        public byte[] PublicModulus { get; set; }
        public byte[] PrivateExponent { get; set; }
        public DateTime TimeCreated { get; set; }
        public int AccountStatus { get; set; }
        public string Sessionid { get; set; }
        public string Passwordmd5 { get; set; }
        public string EmailAdress { get; set; }
    }
}
