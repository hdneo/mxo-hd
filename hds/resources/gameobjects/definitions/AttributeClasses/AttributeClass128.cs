using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass128 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute ActiveTracker = new Attribute(1, "ActiveTracker"); 
		public Attribute MissionKey = new Attribute(4, "MissionKey"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 
		public Attribute FXID = new Attribute(4, "FXID"); 


		 public AttributeClass128(string name,UInt16 _goid)
		: base(6, 3, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, -1); 
			AddAttribute(ref HalfExtents, 1, -1); 
			AddAttribute(ref ActiveTracker, 2, 2); 
			AddAttribute(ref MissionKey, 3, 0); 
			AddAttribute(ref Orientation, 4, -1); 
			AddAttribute(ref FXID, 5, 1); 

		}

	}

}