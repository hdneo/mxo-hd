using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass3751 :GameObject
	{
		public Attribute Orientation = new Attribute(16, "Orientation"); 
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute CurrentState2 = new Attribute(4, "CurrentState2"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute CurrentState = new Attribute(4, "CurrentState"); 


		 public AttributeClass3751(string name,UInt16 _goid)
		: base(5, 2, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Orientation, 0, -1); 
			AddAttribute(ref Position, 1, -1); 
			AddAttribute(ref CurrentState2, 2, 0); 
			AddAttribute(ref HalfExtents, 3, -1); 
			AddAttribute(ref CurrentState, 4, 1); 

		}

	}

}