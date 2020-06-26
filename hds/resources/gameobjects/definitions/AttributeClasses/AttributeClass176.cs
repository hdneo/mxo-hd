using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass176 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute IsStackable = new Attribute(1, "IsStackable"); 
		public Attribute FragmentID = new Attribute(1, "FragmentID"); 
		public Attribute MissionKey = new Attribute(4, "MissionKey"); 
		public Attribute AbilityLevel = new Attribute(2, "AbilityLevel"); 
		public Attribute IsFragment = new Attribute(1, "IsFragment"); 
		public Attribute AbilityID = new Attribute(2, "AbilityID"); 
		public Attribute IsItem = new Attribute(1, "IsItem"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 
		public Attribute IsEncrypted = new Attribute(1, "IsEncrypted"); 


		 public AttributeClass176(string name,UInt16 _goid)
		: base(11, 1, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, -1); 
			AddAttribute(ref HalfExtents, 1, -1); 
			AddAttribute(ref IsStackable, 2, -1); 
			AddAttribute(ref FragmentID, 3, -1); 
			AddAttribute(ref MissionKey, 4, 0); 
			AddAttribute(ref AbilityLevel, 5, -1); 
			AddAttribute(ref IsFragment, 6, -1); 
			AddAttribute(ref AbilityID, 7, -1); 
			AddAttribute(ref IsItem, 8, -1); 
			AddAttribute(ref Orientation, 9, -1); 
			AddAttribute(ref IsEncrypted, 10, -1); 

		}

	}

}