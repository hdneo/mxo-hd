using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass3720 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute MoreInfoID = new Attribute(4, "MoreInfoID"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 


		 public AttributeClass3720(string name,UInt16 _goid)
		: base(4, 1, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, -1); 
			AddAttribute(ref MoreInfoID, 1, 0); 
			AddAttribute(ref HalfExtents, 2, -1); 
			AddAttribute(ref Orientation, 3, -1); 

		}

	}

}