using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace hds.databases.interfaces{
    
    public interface IWorldDBHandler{
        Hashtable getCharInfo (UInt32 charId);
        UInt32 getUserIdForCharId(byte[] charIdHex);
        string getPathForDistrictKey(string key);
        bool fetchWordList(ref WorldList wl);
        ArrayList fetchFriendList(UInt32 charId);
        void updateLocationByHL(UInt16 district, UInt16 hardline);
        void updateSourceHlForObjectTracking(UInt16 sourceDistrict, UInt16 sourceHl, UInt32 lastObjectId);
        void setPlayerValues();
        void setRsiValues();
        void savePlayer(WorldClient client);

        void updateAbilityLoadOut(List<UInt16> abilitySlots, uint loaded);
        
        //NEW
        void updateInventorySlot(UInt16 sourceSlot, UInt16 destSlot);
        UInt16 getFirstNewSlot();
        void addItemToInventory(UInt16 slotId, UInt32 itemGoID);
        void setBackground(string backgroundText);
    }
}