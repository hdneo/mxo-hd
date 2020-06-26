using System;

namespace hds
{
    public class ObjectAttributes8400 : GameObject
    {
        public Attribute[] attributesSelfView;
        public int selfViewGroups;

        public Attribute Position = new Attribute(24, "Position");
        public Attribute HalfExtents = new Attribute(12, "HalfExtents");
        public Attribute SignpostNameString = new Attribute(64, "SignpostNameString");
        public Attribute AnimationID0 = new Attribute(4, "REZ_ID");
        public Attribute SignpostOrgID = new Attribute(1, "Organization::MissionOrg");
        public Attribute DescriptionID = new Attribute(4, "REZ_ID");
        public Attribute SignpostReqReputation = new Attribute(2, "int16");
        public Attribute SignpostReqLevel = new Attribute(1, "uint8");
        public Attribute Orientation = new Attribute(16, "LTQuaternion");

        public ObjectAttributes8400(string name, UInt16 _goid)
            : base(9, 1, name, _goid, 0xFFFFFFFF)
        {
            this.AddAttribute(ref Position,0,-1);
            this.AddAttribute(ref HalfExtents,1,-1);
            this.AddAttribute(ref SignpostNameString,2,-1);
            this.AddAttribute(ref AnimationID0,3,-1);
            this.AddAttribute(ref SignpostOrgID,4,-1);
            this.AddAttribute(ref DescriptionID,5,-1);
            this.AddAttribute(ref SignpostReqReputation,6,-1);
            this.AddAttribute(ref SignpostReqLevel,7,-1);
            this.AddAttribute(ref Orientation,8,-1);
        }
    }
}


