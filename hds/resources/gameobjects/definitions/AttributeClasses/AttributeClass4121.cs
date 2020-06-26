using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass4121 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute Demeanor = new Attribute(1, "Demeanor"); 
		public Attribute Action = new Attribute(1, "Action"); 
		public Attribute StartPosition = new Attribute(24, "StartPosition"); 
		public Attribute Description = new Attribute(4, "Description"); 
		public Attribute StartOrientation = new Attribute(16, "StartOrientation"); 
		public Attribute Stance = new Attribute(1, "Stance"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 


		 public AttributeClass4121(string name,UInt16 _goid)
		: base(9, 1, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, -1); 
			AddAttribute(ref HalfExtents, 1, -1); 
			AddAttribute(ref Demeanor, 2, -1); 
			AddAttribute(ref Action, 3, -1); 
			AddAttribute(ref StartPosition, 4, -1); 
			AddAttribute(ref Description, 5, 0); 
			AddAttribute(ref StartOrientation, 6, -1); 
			AddAttribute(ref Stance, 7, -1); 
			AddAttribute(ref Orientation, 8, -1); 

		}

	}

}