using System;
using System.Collections.Generic;
using hds.shared;

namespace hds
{
    public class PlayerCharacter : Object12
	{
		// Client self view updates are handled by hds.Game.Players.SelfViewUpdates

		/// <summary>
		/// Self view (view 2) update with the given attributes. With <paramref name="includeComponentMask"/>
		/// the component mask (attribute table) is written first, otherwise the caller writes it (e.g. when it
		/// also sends character movement).
		/// </summary>
		public byte[] GetSelfUpdateAttributes(List<Attribute> attributesNeedsUpdate, bool includeComponentMask){
			var writer = new hds.Core.Wire.WireWriter();
			if (includeComponentMask){
				writer.WriteByte(TableComponentBit(true));
			}
			WriteLegacyTable(writer, attributesSelfView, attribute => attribute.isActive() && Contains(attributesNeedsUpdate, attribute));
			return writer.ToArray();
		}
		
	}
}
