using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass3664 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute StartTime = new Attribute(4, "StartTime"); 


		 public AttributeClass3664(string name,UInt16 _goid)
		: base(3, 1, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, 0); 
			AddAttribute(ref HalfExtents, 1, -1); 
			AddAttribute(ref StartTime, 2, -1); 

		}

	}

}