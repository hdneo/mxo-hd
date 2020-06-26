using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass137 :GameObject
	{
		public Attribute Defuse3ActiveTracker = new Attribute(1, "Defuse3ActiveTracker"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 
		public Attribute Defuse4ActiveTracker = new Attribute(1, "Defuse4ActiveTracker"); 
		public Attribute Defuse1ActiveTracker = new Attribute(1, "Defuse1ActiveTracker"); 
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute Defuse5ActiveTracker = new Attribute(1, "Defuse5ActiveTracker"); 
		public Attribute TimerCurrentState = new Attribute(4, "TimerCurrentState"); 
		public Attribute Defuse2ActiveTracker = new Attribute(1, "Defuse2ActiveTracker"); 
		public Attribute DefuseCurrentState = new Attribute(4, "DefuseCurrentState"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 


		 public AttributeClass137(string name,UInt16 _goid)
		: base(10, 7, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Defuse3ActiveTracker, 0, 4); 
			AddAttribute(ref Orientation, 1, -1); 
			AddAttribute(ref Defuse4ActiveTracker, 2, 3); 
			AddAttribute(ref Defuse1ActiveTracker, 3, 6); 
			AddAttribute(ref Position, 4, -1); 
			AddAttribute(ref Defuse5ActiveTracker, 5, 2); 
			AddAttribute(ref TimerCurrentState, 6, 0); 
			AddAttribute(ref Defuse2ActiveTracker, 7, 5); 
			AddAttribute(ref DefuseCurrentState, 8, 1); 
			AddAttribute(ref HalfExtents, 9, -1); 

		}

	}

}