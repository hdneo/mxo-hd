using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass155 :GameObject
	{
		public Attribute DisarmDifficulty = new Attribute(2, "DisarmDifficulty"); 
		public Attribute UseCount = new Attribute(1, "UseCount"); 
		public Attribute RewardRule = new Attribute(64, "RewardRule"); 
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


		 public AttributeClass155(string name,UInt16 _goid)
		: base(14, 6, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref DisarmDifficulty, 0, -1); 
			AddAttribute(ref UseCount, 1, 0); 
			AddAttribute(ref RewardRule, 2, -1); 
			AddAttribute(ref Orientation, 3, -1); 
			AddAttribute(ref DetectDifficulty, 4, -1); 
			AddAttribute(ref RewardRuleArgs, 5, -1); 
			AddAttribute(ref TrapFX, 6, 2); 
			AddAttribute(ref Position, 7, -1); 
			AddAttribute(ref TrapVisible, 8, 1); 
			AddAttribute(ref TrapArmed, 9, -1); 
			AddAttribute(ref MissionKey, 10, 3); 
			AddAttribute(ref HalfExtents, 11, -1); 
			AddAttribute(ref DetectTrapFX, 12, 4); 
			AddAttribute(ref CurrentState, 13, 5); 

		}

	}

}