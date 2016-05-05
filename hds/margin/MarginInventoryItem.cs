using System;
using System.Collections;
using System.Text;

namespace hds
{
    public class MarginInventoryItem
    {

        private UInt32 itemID;
        private UInt16 amount;
        private UInt16 purity;
        private UInt16 slot;
        public MarginInventoryItem()
        {

        }

        public void setItemID(UInt32 itemID)
        {
            this.itemID = itemID;
        }

        public UInt32 getItemID()
        {
            return this.itemID;
        }

        public void setAmount(UInt16 count)
        {
            this.amount = count;
        }

        public UInt16 getAmount()
        {
            return this.amount;
        }

        public void setPurity(UInt16 purity)
        {
            this.purity = purity;
        }

        public UInt16 getPurity()
        {
            return this.purity;
        }

        public void setSlot(UInt16 slot)
        {
            this.slot = slot;
        }

        public UInt16 getSlot()
        {
            return this.slot;
        }
    }
}
