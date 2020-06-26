using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass295 :GameObject
	{
		public Attribute ActiveTracker2 = new Attribute(1, "ActiveTracker2"); 
		public Attribute Defuse3ActiveTracker = new Attribute(1, "Defuse3ActiveTracker"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 
		public Attribute Defuse4ActiveTracker = new Attribute(1, "Defuse4ActiveTracker"); 
		public Attribute Defuse1ActiveTracker = new Attribute(1, "Defuse1ActiveTracker"); 
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute Defuse5ActiveTracker = new Attribute(1, "Defuse5ActiveTracker"); 
		public Attribute SpawnSequence = new Attribute(2, "SpawnSequence"); 
		public Attribute Defuse2ActiveTracker = new Attribute(1, "Defuse2ActiveTracker"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute CurrentState = new Attribute(4, "CurrentState"); 


		 public AttributeClass295(string name,UInt16 _goid)
		: base(11, 7, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref ActiveTracker2, 0, 6); 
			AddAttribute(ref Defuse3ActiveTracker, 1, 2); 
			AddAttribute(ref Orientation, 2, -1); 
			AddAttribute(ref Defuse4ActiveTracker, 3, 1); 
			AddAttribute(ref Defuse1ActiveTracker, 4, 4); 
			AddAttribute(ref Position, 5, -1); 
			AddAttribute(ref Defuse5ActiveTracker, 6, 0); 
			AddAttribute(ref SpawnSequence, 7, -1); 
			AddAttribute(ref Defuse2ActiveTracker, 8, 3); 
			AddAttribute(ref HalfExtents, 9, -1); 
			AddAttribute(ref CurrentState, 10, 5); 

		}

	}

}