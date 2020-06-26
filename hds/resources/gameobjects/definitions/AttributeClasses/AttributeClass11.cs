using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass11 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute DescriptionID = new Attribute(4, "DescriptionID"); 
		public Attribute AmbientFX = new Attribute(4, "AmbientFX"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 


		 public AttributeClass11(string name,UInt16 _goid)
		: base(5, 0, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, -1); 
			AddAttribute(ref HalfExtents, 1, -1); 
			AddAttribute(ref DescriptionID, 2, -1); 
			AddAttribute(ref AmbientFX, 3, -1); 
			AddAttribute(ref Orientation, 4, -1); 

		}

	}

}