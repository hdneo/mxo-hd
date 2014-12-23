using System;
using System.Text;

namespace hds
{
    class AbilityItem
    {
        private UInt16 AbilityID;
        private Int32 GOID;
        private string AbilityName;

        public AbilityItem()
        {

        }

        public void setAbilityID(UInt16 ID)
        {
            this.AbilityID = ID;
        }

        public UInt16 getAbilityID()
        {
            return AbilityID;
        }

        public void setGOID(Int32 GOID)
        {
            this.GOID = GOID;
        }

        public Int32 getGOID()
        {
            return this.GOID;
        }

        public void setAbilityName(string name)
        {
            this.AbilityName = name;
        }

        public string getAbilityName()
        {
            return this.AbilityName;
        }
    }
}
