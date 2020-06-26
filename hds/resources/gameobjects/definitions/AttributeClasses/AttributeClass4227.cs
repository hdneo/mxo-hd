using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass4227 :GameObject
	{
		public Attribute Class = new Attribute(4, "Class"); 
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute MoreInfoID = new Attribute(4, "MoreInfoID"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute ReqAbilityID = new Attribute(4, "ReqAbilityID"); 
		public Attribute Quality = new Attribute(4, "Quality"); 
		public Attribute InfoID = new Attribute(4, "InfoID"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 


		 public AttributeClass4227(string name,UInt16 _goid)
		: base(8, 4, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Class, 0, 3); 
			AddAttribute(ref Position, 1, -1); 
			AddAttribute(ref MoreInfoID, 2, 1); 
			AddAttribute(ref HalfExtents, 3, -1); 
			AddAttribute(ref ReqAbilityID, 4, -1); 
			AddAttribute(ref Quality, 5, 0); 
			AddAttribute(ref InfoID, 6, 2); 
			AddAttribute(ref Orientation, 7, -1); 

		}

	}

}