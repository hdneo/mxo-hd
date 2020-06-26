using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass4309 :GameObject
	{
		public Attribute MoreInfoID = new Attribute(4, "MoreInfoID"); 


		 public AttributeClass4309(string name,UInt16 _goid)
		: base(1, 1, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref MoreInfoID, 0, 0); 

		}

	}

}