using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass254 :GameObject
	{
		public Attribute Orientation = new Attribute(16, "Orientation"); 
		public Attribute FactionID = new Attribute(4, "FactionID"); 
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute CurrentState = new Attribute(4, "CurrentState"); 


		 public AttributeClass254(string name,UInt16 _goid)
		: base(5, 2, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Orientation, 0, -1); 
			AddAttribute(ref FactionID, 1, 0); 
			AddAttribute(ref Position, 2, -1); 
			AddAttribute(ref HalfExtents, 3, -1); 
			AddAttribute(ref CurrentState, 4, 1); 

		}

	}

}