using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass3678 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 


		 public AttributeClass3678(string name,UInt16 _goid)
		: base(3, 3, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, 0); 
			AddAttribute(ref HalfExtents, 1, 2); 
			AddAttribute(ref Orientation, 2, 1); 

		}

	}

}