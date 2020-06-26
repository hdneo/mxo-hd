using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass15 :GameObject
	{
		public Attribute FXStartOnActivate = new Attribute(1, "FXStartOnActivate"); 
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute FXLooping = new Attribute(1, "FXLooping"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 
		public Attribute FXID = new Attribute(4, "FXID"); 


		 public AttributeClass15(string name,UInt16 _goid)
		: base(6, 2, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref FXStartOnActivate, 0, -1); 
			AddAttribute(ref Position, 1, 0); 
			AddAttribute(ref HalfExtents, 2, -1); 
			AddAttribute(ref FXLooping, 3, -1); 
			AddAttribute(ref Orientation, 4, 1); 
			AddAttribute(ref FXID, 5, -1); 

		}

	}

}