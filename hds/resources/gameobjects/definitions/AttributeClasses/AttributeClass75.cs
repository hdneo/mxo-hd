using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass75 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute UseCount = new Attribute(4, "UseCount"); 
		public Attribute ArchivistOrganization = new Attribute(1, "ArchivistOrganization"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 


		 public AttributeClass75(string name,UInt16 _goid)
		: base(5, 1, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, -1); 
			AddAttribute(ref HalfExtents, 1, -1); 
			AddAttribute(ref UseCount, 2, 0); 
			AddAttribute(ref ArchivistOrganization, 3, -1); 
			AddAttribute(ref Orientation, 4, -1); 

		}

	}

}