using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hds
{
    public class Object6568 : GameObject
    {
        public int SelfViewGroups;

        public Attribute Position = new Attribute(24, "Position");
        public Attribute HalfExtents = new Attribute(12, "HalfExtents");
        public Attribute CurrentState = new Attribute(4, "CurrentState");
        public Attribute Hidden = new Attribute(1, "Hidden");
        public Attribute Orientation = new Attribute(16, "Orientation");

        public Object6568(string name, UInt16 _goid, UInt32 _relatedStaticObjId) : base(5, 2, 
            name, _goid, _relatedStaticObjId)
        {
            this.AddAttribute(ref Position, 0, -1);
            this.AddAttribute(ref HalfExtents, 1, -1);
            this.AddAttribute(ref CurrentState, 2, -1);
            this.AddAttribute(ref Hidden, 3, -1);
            this.AddAttribute(ref Orientation, 4, -1);
        }
    }
}