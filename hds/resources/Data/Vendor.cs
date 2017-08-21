using System;
using System.Collections.Generic;
using System.Text;

namespace hds
{
    public class Vendor
    {
        public UInt32[] items;
        public UInt16 metrId;
        public UInt32 vendorStaticID;

        public Vendor(UInt16 metrId, UInt32 vendorStaticID)
        {
            this.metrId = metrId;
            this.vendorStaticID = vendorStaticID;
        }

        public Vendor(UInt16 metrId, UInt32 vendorStaticID, UInt32[] items)
        {
            this.metrId = metrId;
            this.vendorStaticID = vendorStaticID;
            this.items = items;
        }

    }
}
