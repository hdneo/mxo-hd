using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass4041 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute ReqAbilityID = new Attribute(4, "ReqAbilityID"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 


		 public AttributeClass4041(string name,UInt16 _goid)
		: base(4, 0, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, -1); 
			AddAttribute(ref HalfExtents, 1, -1); 
			AddAttribute(ref ReqAbilityID, 2, -1); 
			AddAttribute(ref Orientation, 3, -1); 

		}

	}

}