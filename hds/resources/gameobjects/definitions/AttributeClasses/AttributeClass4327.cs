using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass4327 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute ArchivistTeleportRadius = new Attribute(4, "ArchivistTeleportRadius"); 
		public Attribute UseCount = new Attribute(4, "UseCount"); 
		public Attribute CurrentState = new Attribute(4, "CurrentState"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 


		 public AttributeClass4327(string name,UInt16 _goid)
		: base(6, 2, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, -1); 
			AddAttribute(ref HalfExtents, 1, -1); 
			AddAttribute(ref ArchivistTeleportRadius, 2, -1); 
			AddAttribute(ref UseCount, 3, 0); 
			AddAttribute(ref CurrentState, 4, 1); 
			AddAttribute(ref Orientation, 5, -1); 

		}

	}

}