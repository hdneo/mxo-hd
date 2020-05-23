using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using hds.world.Structures;

namespace hds.databases.interfaces{
    
    public interface IWorldDBHandler
    {
        UInt32 getCharIdByHandle(string handle);
        Hashtable getCharInfo (UInt32 charId);
        Hashtable getCharInfoByHandle(string handle);
        UInt32 getUserIdForCharId(byte[] charIdHex);
        string getPathForDistrictKey(string key);
        void AddHandleToFriendList(string handleToAdd, UInt32 charId);
        void RemoveHandleFromFriendList(string handleToRemove, UInt32 charId);
        ArrayList FetchPlayersWhoAddedMeToBuddylist(UInt32 charId);
        ArrayList fetchFriendList(UInt32 charId);
        Faction fetchFaction(UInt32 factionId);
        void IncreaseCrewMoney(UInt32 crewId, UInt32 amount);
        void DecreaseCrewMoney(UInt32 crewId, UInt32 amount);
        void IncreaseFactionMoney(UInt32 crewId, UInt32 amount);
        void DecreaseFactionMoney(UInt32 crewId, UInt32 amount);
        Crew GetCrewData(UInt32 crewId);
        List<CrewMember> GetCrewMembersForCrewId(UInt32 crewId);
        void updateLocationByHL(UInt16 district, UInt16 hardline);
        void updateSourceHlForObjectTracking(UInt16 sourceDistrict, UInt16 sourceHl, UInt32 lastObjectId);
        void setPlayerValues();
        void setRsiValues();
        void setOnlineStatus(UInt32 charId, ushort isOnline);
        void ResetOnlineStatus();
        void savePlayer(WorldClient client);
        void SaveExperience(WorldClient client, long exp);
        void SaveInfo(WorldClient client, long exp);

        void updateAbilityLoadOut(List<UInt16> abilitySlots, uint loaded);
        
        //NEW
        void updateInventorySlot(UInt16 sourceSlot, UInt16 destSlot);
        UInt16 GetFirstNewSlot();
        void addItemToInventory(UInt16 slotId, UInt32 itemGoID);
        void setBackground(string backgroundText);
        UInt32 GetItemGOIDAtInventorySlot(UInt16 slotId);
        void updateRsiPartValue(string part, uint value);
        bool isSlotinUseByItem(UInt16 slotId);

        bool IsCrewNameAvailable(string crewName);
        void AddCrew(string crewName, string masterHandle);
        UInt16 GetCrewMemberCountByCrewName(string crewName);
        UInt16 GetCrewIdByCrewMasterHandle(string playerHandle);

        string GetFactionNameById(UInt32 factionId);
    }
}