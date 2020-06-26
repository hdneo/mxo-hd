using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass3781 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute DescriptionID = new Attribute(4, "DescriptionID"); 
		public Attribute Identifier = new Attribute(18, "Identifier"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 


		 public AttributeClass3781(string name,UInt16 _goid)
		: base(5, 5, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, 0); 
			AddAttribute(ref HalfExtents, 1, 3); 
			AddAttribute(ref DescriptionID, 2, 4); 
			AddAttribute(ref Identifier, 3, 2); 
			AddAttribute(ref Orientation, 4, 1); 

		}

	}

}