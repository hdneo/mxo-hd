using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass4187 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 
		public Attribute SiteInUse = new Attribute(1, "SiteInUse"); 


		 public AttributeClass4187(string name,UInt16 _goid)
		: base(3, 1, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, -1); 
			AddAttribute(ref Orientation, 1, -1); 
			AddAttribute(ref SiteInUse, 2, 0); 

		}

	}

}