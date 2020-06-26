using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass3955 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute MissionKey = new Attribute(4, "MissionKey"); 
		public Attribute DetectTrapFX = new Attribute(4, "DetectTrapFX"); 
		public Attribute UseCount = new Attribute(1, "UseCount"); 
		public Attribute CurrentState = new Attribute(4, "CurrentState"); 
		public Attribute CurrentTimerState = new Attribute(4, "CurrentTimerState"); 
		public Attribute Hidden = new Attribute(1, "Hidden"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 


		 public AttributeClass3955(string name,UInt16 _goid)
		: base(9, 6, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, -1); 
			AddAttribute(ref HalfExtents, 1, -1); 
			AddAttribute(ref MissionKey, 2, 1); 
			AddAttribute(ref DetectTrapFX, 3, 3); 
			AddAttribute(ref UseCount, 4, 0); 
			AddAttribute(ref CurrentState, 5, 5); 
			AddAttribute(ref CurrentTimerState, 6, 4); 
			AddAttribute(ref Hidden, 7, 2); 
			AddAttribute(ref Orientation, 8, -1); 

		}

	}

}