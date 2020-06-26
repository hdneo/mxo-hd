using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass3758 :GameObject
	{
		public Attribute Orientation = new Attribute(16, "Orientation"); 
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute CurrentState = new Attribute(4, "CurrentState"); 


		 public AttributeClass3758(string name,UInt16 _goid)
		: base(4, 1, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Orientation, 0, -1); 
			AddAttribute(ref Position, 1, -1); 
			AddAttribute(ref HalfExtents, 2, -1); 
			AddAttribute(ref CurrentState, 3, 0); 

		}

	}

}