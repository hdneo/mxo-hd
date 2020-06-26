using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass4094 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute MissionKey = new Attribute(4, "MissionKey"); 
		public Attribute CurrentState = new Attribute(4, "CurrentState"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 


		 public AttributeClass4094(string name,UInt16 _goid)
		: base(4, 2, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, -1); 
			AddAttribute(ref MissionKey, 1, 0); 
			AddAttribute(ref CurrentState, 2, 1); 
			AddAttribute(ref Orientation, 3, -1); 

		}

	}

}