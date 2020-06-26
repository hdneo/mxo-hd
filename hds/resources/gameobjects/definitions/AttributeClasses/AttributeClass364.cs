using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass364 :GameObject
	{
		public Attribute DisarmDifficulty = new Attribute(2, "DisarmDifficulty"); 
		public Attribute IsZionAligned = new Attribute(1, "IsZionAligned"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 
		public Attribute DetectDifficulty = new Attribute(2, "DetectDifficulty"); 
		public Attribute IsMeroAligned = new Attribute(1, "IsMeroAligned"); 
		public Attribute TrapFX = new Attribute(4, "TrapFX"); 
		public Attribute IsGMAligned = new Attribute(1, "IsGMAligned"); 
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute IsMachineAligned = new Attribute(1, "IsMachineAligned"); 
		public Attribute TrapVisible = new Attribute(1, "TrapVisible"); 
		public Attribute TrapArmed = new Attribute(1, "TrapArmed"); 
		public Attribute MissionKey = new Attribute(4, "MissionKey"); 
		public Attribute DetectTrapFX = new Attribute(4, "DetectTrapFX"); 
		public Attribute CurrentState = new Attribute(4, "CurrentState"); 


		 public AttributeClass364(string name,UInt16 _goid)
		: base(14, 9, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref DisarmDifficulty, 0, -1); 
			AddAttribute(ref IsZionAligned, 1, 3); 
			AddAttribute(ref Orientation, 2, -1); 
			AddAttribute(ref DetectDifficulty, 3, -1); 
			AddAttribute(ref IsMeroAligned, 4, 4); 
			AddAttribute(ref TrapFX, 5, 1); 
			AddAttribute(ref IsGMAligned, 6, 6); 
			AddAttribute(ref Position, 7, -1); 
			AddAttribute(ref IsMachineAligned, 8, 5); 
			AddAttribute(ref TrapVisible, 9, 0); 
			AddAttribute(ref TrapArmed, 10, -1); 
			AddAttribute(ref MissionKey, 11, 2); 
			AddAttribute(ref DetectTrapFX, 12, 7); 
			AddAttribute(ref CurrentState, 13, 8); 

		}

	}

}