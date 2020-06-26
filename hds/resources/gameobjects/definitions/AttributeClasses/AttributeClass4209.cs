using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass4209 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute CurrentState = new Attribute(4, "CurrentState"); 
		public Attribute Hidden = new Attribute(1, "Hidden"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 


		 public AttributeClass4209(string name,UInt16 _goid)
		: base(5, 2, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, -1); 
			AddAttribute(ref HalfExtents, 1, -1); 
			AddAttribute(ref CurrentState, 2, 1); 
			AddAttribute(ref Hidden, 3, 0); 
			AddAttribute(ref Orientation, 4, -1); 

		}

	}

}