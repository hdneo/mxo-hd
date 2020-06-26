using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass4092 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute MissionKey = new Attribute(4, "MissionKey"); 
		public Attribute CurrentState = new Attribute(4, "CurrentState"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 


		 public AttributeClass4092(string name,UInt16 _goid)
		: base(5, 2, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, -1); 
			AddAttribute(ref HalfExtents, 1, -1); 
			AddAttribute(ref MissionKey, 2, 0); 
			AddAttribute(ref CurrentState, 3, 1); 
			AddAttribute(ref Orientation, 4, -1); 

		}

	}

}