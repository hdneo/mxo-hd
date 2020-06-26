using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass3930 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute DefuseCurrentState = new Attribute(4, "DefuseCurrentState"); 
		public Attribute Defuse1ActiveTracker = new Attribute(1, "Defuse1ActiveTracker"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 


		 public AttributeClass3930(string name,UInt16 _goid)
		: base(5, 2, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, -1); 
			AddAttribute(ref HalfExtents, 1, -1); 
			AddAttribute(ref DefuseCurrentState, 2, 0); 
			AddAttribute(ref Defuse1ActiveTracker, 3, 1); 
			AddAttribute(ref Orientation, 4, -1); 

		}

	}

}