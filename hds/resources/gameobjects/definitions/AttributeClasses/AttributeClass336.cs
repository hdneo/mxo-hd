using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass336 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 


		 public AttributeClass336(string name,UInt16 _goid)
		: base(2, 2, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, 0); 
			AddAttribute(ref Orientation, 1, 1); 

		}

	}

}