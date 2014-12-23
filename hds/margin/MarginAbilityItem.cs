using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace hds
{
    public class MarginAbilityItem
    {

        public Int32 AbilityID;
        public UInt16 level;
        public UInt16 slot;
        public bool isLoaded;

        public MarginAbilityItem()
        {

        }

        public void setAbilityID(Int32 AbilityID)
        {
            this.AbilityID = AbilityID;
        }

        public Int32 getAbilityID()
        {
            return this.AbilityID;
        }

        public void setLevel(UInt16 level)
        {
            this.level = level;
        }

        public UInt16 getLevel()
        {
            return this.level;
        }

        public void setSlot(UInt16 slot)
        {
            this.slot = slot;
        }

        public UInt16 getSlot()
        {
            return this.slot;
        }

        public void setLoaded(bool isLoaded)
        {
            this.isLoaded = isLoaded;
        }

        public bool getLoaded()
        {
            return this.isLoaded;
        }



    }
}
