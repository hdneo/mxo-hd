using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass304 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute Purity = new Attribute(1, "Purity"); 
		public Attribute AccessLevel = new Attribute(2, "AccessLevel"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 
		public Attribute InfoAmount = new Attribute(2, "InfoAmount"); 


		 public AttributeClass304(string name,UInt16 _goid)
		: base(6, 1, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, -1); 
			AddAttribute(ref HalfExtents, 1, -1); 
			AddAttribute(ref Purity, 2, -1); 
			AddAttribute(ref AccessLevel, 3, -1); 
			AddAttribute(ref Orientation, 4, -1); 
			AddAttribute(ref InfoAmount, 5, 0); 

		}

	}

}