using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass3722 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute CurrentState = new Attribute(4, "CurrentState"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 


		 public AttributeClass3722(string name,UInt16 _goid)
		: base(4, 1, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, -1); 
			AddAttribute(ref HalfExtents, 1, -1); 
			AddAttribute(ref CurrentState, 2, 0); 
			AddAttribute(ref Orientation, 3, -1); 

		}

	}

}