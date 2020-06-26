using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass305 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute MissionKey = new Attribute(4, "MissionKey"); 
		public Attribute UseCount = new Attribute(1, "UseCount"); 
		public Attribute CurrentState = new Attribute(4, "CurrentState"); 
		public Attribute CurrentTimerState = new Attribute(4, "CurrentTimerState"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 


		 public AttributeClass305(string name,UInt16 _goid)
		: base(7, 4, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, -1); 
			AddAttribute(ref HalfExtents, 1, -1); 
			AddAttribute(ref MissionKey, 2, 1); 
			AddAttribute(ref UseCount, 3, 0); 
			AddAttribute(ref CurrentState, 4, 3); 
			AddAttribute(ref CurrentTimerState, 5, 2); 
			AddAttribute(ref Orientation, 6, -1); 

		}

	}

}