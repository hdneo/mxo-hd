using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass4067 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute MoreInfoID = new Attribute(4, "MoreInfoID"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 


		 public AttributeClass4067(string name,UInt16 _goid)
		: base(3, 1, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, -1); 
			AddAttribute(ref MoreInfoID, 1, 0); 
			AddAttribute(ref Orientation, 2, -1); 

		}

	}

}