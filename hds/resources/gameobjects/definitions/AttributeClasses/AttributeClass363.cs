using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass363 :GameObject
	{
		public Attribute DisarmDifficulty = new Attribute(2, "DisarmDifficulty"); 
		public Attribute IsZionAligned = new Attribute(1, "IsZionAligned"); 
		public Attribute ClosedHalfExtents = new Attribute(12, "ClosedHalfExtents"); 
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


		 public AttributeClass363(string name,UInt16 _goid)
		: base(15, 9, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref DisarmDifficulty, 0, -1); 
			AddAttribute(ref IsZionAligned, 1, 3); 
			AddAttribute(ref ClosedHalfExtents, 2, -1); 
			AddAttribute(ref Orientation, 3, -1); 
			AddAttribute(ref DetectDifficulty, 4, -1); 
			AddAttribute(ref IsMeroAligned, 5, 4); 
			AddAttribute(ref TrapFX, 6, 1); 
			AddAttribute(ref IsGMAligned, 7, 6); 
			AddAttribute(ref Position, 8, -1); 
			AddAttribute(ref IsMachineAligned, 9, 5); 
			AddAttribute(ref TrapVisible, 10, 0); 
			AddAttribute(ref TrapArmed, 11, -1); 
			AddAttribute(ref MissionKey, 12, 2); 
			AddAttribute(ref DetectTrapFX, 13, 7); 
			AddAttribute(ref CurrentState, 14, 8); 

		}

	}

}