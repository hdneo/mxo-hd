using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass4239 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute DescriptionID = new Attribute(4, "DescriptionID"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 


		 public AttributeClass4239(string name,UInt16 _goid)
		: base(4, 0, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, -1); 
			AddAttribute(ref HalfExtents, 1, -1); 
			AddAttribute(ref DescriptionID, 2, -1); 
			AddAttribute(ref Orientation, 3, -1); 

		}

	}

}