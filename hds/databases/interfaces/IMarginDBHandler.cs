using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace hds.databases.interfaces{
    public interface IMarginDBHandler{

        Hashtable getCharInfo (int charId);
        List<MarginInventoryItem> loadInventory(int charId);
        List<MarginAbilityItem> loadAbilities(int charId);
        string loadAllHardlines();
        UInt32 getNewCharnameID(string handle, UInt32 userId);
        UInt32 createNewCharacter(string handle, UInt32 userid, UInt32 worldId);
        void updateRSIValue(string field, string value, UInt32 charID);

        // NEW
        void addAbility(Int32 abilityID, UInt16 slotID, UInt32 charID, UInt16 level, UInt16 is_loaded);
        void deleteCharacter(UInt64 charId);
    }
}