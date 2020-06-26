using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass3825 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute MissionKey = new Attribute(4, "MissionKey"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 


		 public AttributeClass3825(string name,UInt16 _goid)
		: base(4, 1, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, -1); 
			AddAttribute(ref HalfExtents, 1, -1); 
			AddAttribute(ref MissionKey, 2, 0); 
			AddAttribute(ref Orientation, 3, -1); 

		}

	}

}