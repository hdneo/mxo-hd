using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass3633 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute Hidden = new Attribute(1, "Hidden"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 


		 public AttributeClass3633(string name,UInt16 _goid)
		: base(4, 1, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, -1); 
			AddAttribute(ref HalfExtents, 1, -1); 
			AddAttribute(ref Hidden, 2, 0); 
			AddAttribute(ref Orientation, 3, -1); 

		}

	}

}