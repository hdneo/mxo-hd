using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using hds.databases.Entities;

namespace hds.databases.interfaces{
    public interface IMarginDBHandler{

        Character GetCharInfo (int charId);
        List<MarginInventoryItem> LoadInventory(int charId);
        List<MarginAbilityItem> LoadAbilities(int charId);
        string LoadAllHardlines();
        UInt32 getNewCharnameID(string handle, UInt32 userId);
        UInt32 CreateNewCharacter(string handle, UInt32 userid, UInt32 worldId);
        void updateRSIValue(string field, string value, UInt32 charID);
        void updateCharacter(string firstName, string lastName, string background, UInt32 charID);

        // NEW
        void addAbility(Int32 abilityID, UInt16 slotID, UInt32 charID, UInt16 level, UInt16 is_loaded);

        void AddItemToSlot(UInt32 itemId, UInt16 slotID, UInt32 charID);
        void deleteCharacter(UInt64 charId);
    }
}