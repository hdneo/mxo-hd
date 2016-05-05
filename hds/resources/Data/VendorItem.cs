using System;
using System.Text;

namespace hds
{
    class VendorItem
    {
        private UInt16 AbilityID;
        private Int32 GOID;
        private UInt16 price;
        private UInt32 vendorStaticID;
      

        public VendorItem()
        {

        }

        public void setPrice(UInt16 ID)
        {
            this.price = ID;
        }

        public UInt16 getPrice()
        {
            return price;
        }

        public void setGOID(Int32 GOID)
        {
            this.GOID = GOID;
        }

        public Int32 getGOID()
        {
            return this.GOID;
        }

        public void setVendorStaticID(UInt32 staticID)
        {
            this.vendorStaticID = staticID;
        }

        public UInt32 getVendorStaticID()
        {
            return this.vendorStaticID;
        }
    }
}
