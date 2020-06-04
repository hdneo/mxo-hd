using System;

namespace hds
{
    public class ObjectAttributes364 : GameObject
    {
        public Attribute[] attributesSelfView;
        public int selfViewGroups;

        public Attribute DisarmDifficulty = new Attribute(2,"DisarmDifficulty");
        public Attribute IsZionAligned = new Attribute(1,"IsZionAligned");
        public Attribute Orientation = new Attribute(16,"Orientation");
        public Attribute DetectDifficulty = new Attribute(2,"DetectDifficulty");
        public Attribute IsMeroAligned = new Attribute(1,"IsMeroAligned");
        public Attribute TrapFX = new Attribute(4,"TrapFX");
        public Attribute IsGMAligned = new Attribute(1,"IsGMAligned");
        public Attribute Position = new Attribute(24,"Position");
        public Attribute IsMachineAligned = new Attribute(1,"IsMachineAligned");
        public Attribute TrapVisible = new Attribute(1,"TrapVisible");
        public Attribute TrapArmed = new Attribute(1,"TrapArmed");
        public Attribute MissionKey = new Attribute(4,"MissionKey");
        public Attribute DetectTrapFX = new Attribute(4,"DetectTrapFX");
        public Attribute CurrentState = new Attribute(4,"CurrentState");

        public ObjectAttributes364(string name,UInt16 _goid,UInt32 _relatedStaticObjId)
            : base(14, 1, name, _goid, _relatedStaticObjId)
        {
            this.AddAttribute(ref DisarmDifficulty,0,-1);
            this.AddAttribute(ref IsZionAligned,1,-1);
            this.AddAttribute(ref Orientation,2,-1);
            this.AddAttribute(ref DetectDifficulty,3,-1);
            this.AddAttribute(ref IsMeroAligned,4,-1);
            this.AddAttribute(ref TrapFX,5,-1);
            this.AddAttribute(ref IsGMAligned,6,-1);
            this.AddAttribute(ref Position,7,-1);
            this.AddAttribute(ref IsMachineAligned,8,-1);
            this.AddAttribute(ref TrapVisible,9,-1);
            this.AddAttribute(ref TrapArmed,10,-1);
            this.AddAttribute(ref MissionKey,11,-1);
            this.AddAttribute(ref DetectTrapFX,12,-1);
            this.AddAttribute(ref CurrentState,13,-1);
        }


    }
}
