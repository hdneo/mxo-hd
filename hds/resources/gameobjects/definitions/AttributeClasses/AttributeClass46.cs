using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass46 :GameObject
	{
		public Attribute DisarmDifficulty = new Attribute(2, "DisarmDifficulty"); 
		public Attribute UseCount = new Attribute(1, "UseCount"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 
		public Attribute DetectDifficulty = new Attribute(2, "DetectDifficulty"); 
		public Attribute RewardRuleArgs = new Attribute(64, "RewardRuleArgs"); 
		public Attribute TrapFX = new Attribute(4, "TrapFX"); 
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute TrapVisible = new Attribute(1, "TrapVisible"); 
		public Attribute TrapArmed = new Attribute(1, "TrapArmed"); 
		public Attribute MissionKey = new Attribute(4, "MissionKey"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute DetectTrapFX = new Attribute(4, "DetectTrapFX"); 
		public Attribute CurrentState = new Attribute(4, "CurrentState"); 


		 public AttributeClass46(string name,UInt16 _goid)
		: base(13, 6, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref DisarmDifficulty, 0, -1); 
			AddAttribute(ref UseCount, 1, 0); 
			AddAttribute(ref Orientation, 2, -1); 
			AddAttribute(ref DetectDifficulty, 3, -1); 
			AddAttribute(ref RewardRuleArgs, 4, -1); 
			AddAttribute(ref TrapFX, 5, 2); 
			AddAttribute(ref Position, 6, -1); 
			AddAttribute(ref TrapVisible, 7, 1); 
			AddAttribute(ref TrapArmed, 8, -1); 
			AddAttribute(ref MissionKey, 9, 3); 
			AddAttribute(ref HalfExtents, 10, -1); 
			AddAttribute(ref DetectTrapFX, 11, 4); 
			AddAttribute(ref CurrentState, 12, 5); 

		}

	}

}