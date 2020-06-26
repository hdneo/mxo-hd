using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass3958 :GameObject
	{
		public Attribute InfoID = new Attribute(4, "InfoID"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 
		public Attribute MoreInfoID = new Attribute(4, "MoreInfoID"); 
		public Attribute Class = new Attribute(4, "Class"); 
		public Attribute Quality = new Attribute(4, "Quality"); 
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 


		 public AttributeClass3958(string name,UInt16 _goid)
		: base(7, 4, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref InfoID, 0, 2); 
			AddAttribute(ref Orientation, 1, -1); 
			AddAttribute(ref MoreInfoID, 2, 1); 
			AddAttribute(ref Class, 3, 3); 
			AddAttribute(ref Quality, 4, 0); 
			AddAttribute(ref Position, 5, -1); 
			AddAttribute(ref HalfExtents, 6, -1); 

		}

	}

}