using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass3605 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute AnimationID0 = new Attribute(4, "AnimationID0"); 


		 public AttributeClass3605(string name,UInt16 _goid)
		: base(2, 1, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, -1); 
			AddAttribute(ref AnimationID0, 1, 0); 

		}

	}

}