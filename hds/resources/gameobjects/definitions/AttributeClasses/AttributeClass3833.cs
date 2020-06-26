using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass3833 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute ContactName = new Attribute(32, "ContactName"); 
		public Attribute DescriptionID = new Attribute(4, "DescriptionID"); 
		public Attribute ContactID = new Attribute(4, "ContactID"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 


		 public AttributeClass3833(string name,UInt16 _goid)
		: base(6, 0, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, -1); 
			AddAttribute(ref HalfExtents, 1, -1); 
			AddAttribute(ref ContactName, 2, -1); 
			AddAttribute(ref DescriptionID, 3, -1); 
			AddAttribute(ref ContactID, 4, -1); 
			AddAttribute(ref Orientation, 5, -1); 

		}

	}

}