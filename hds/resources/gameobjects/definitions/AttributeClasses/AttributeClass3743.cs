using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass3743 :GameObject
	{
		public Attribute InfoID = new Attribute(4, "InfoID"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 
		public Attribute MoreInfoID = new Attribute(4, "MoreInfoID"); 
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 


		 public AttributeClass3743(string name,UInt16 _goid)
		: base(5, 2, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref InfoID, 0, 1); 
			AddAttribute(ref Orientation, 1, -1); 
			AddAttribute(ref MoreInfoID, 2, 0); 
			AddAttribute(ref Position, 3, -1); 
			AddAttribute(ref HalfExtents, 4, -1); 

		}

	}

}