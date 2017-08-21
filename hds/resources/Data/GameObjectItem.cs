using System;
using System.Collections.Generic;

using System.Text;

namespace hds
{
    public class GameObjectItem
    {
        private Int32 GOID;
        private string Name;

        public GameObjectItem()
        {

        }

        public void setGOID(Int32 GOID)
        {
            this.GOID = GOID;
        }

        public Int32 getGOID()
        {
            return this.GOID;
        }

        public void setName(string name)
        {
            this.Name = name;
        }

        public string getName()
        {
            return this.Name;
        }
    }
}
