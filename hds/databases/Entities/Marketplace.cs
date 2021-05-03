using System;
using System.Collections.Generic;

#nullable disable

namespace hds.databases.Entities
{
    public partial class Marketplace
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public int? ItemId { get; set; }
        public int? Purity { get; set; }
        public int? DelistPrice { get; set; }
        public int? Price { get; set; }
        public int? CharId { get; set; }
        public int? CharRep { get; set; }
        public int? IsSold { get; set; }
        public int? Created { get; set; }
    }
}
