using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass337 :GameObject
	{
		public Attribute Class = new Attribute(4, "Class"); 
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute MoreInfoID = new Attribute(4, "MoreInfoID"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute ReqAbilityID = new Attribute(4, "ReqAbilityID"); 
		public Attribute InfoID = new Attribute(4, "InfoID"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 


		 public AttributeClass337(string name,UInt16 _goid)
		: base(7, 3, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Class, 0, 2); 
			AddAttribute(ref Position, 1, -1); 
			AddAttribute(ref MoreInfoID, 2, 0); 
			AddAttribute(ref HalfExtents, 3, -1); 
			AddAttribute(ref ReqAbilityID, 4, -1); 
			AddAttribute(ref InfoID, 5, 1); 
			AddAttribute(ref Orientation, 6, -1); 

		}

	}

}