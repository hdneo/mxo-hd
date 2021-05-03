using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using hds.databases.Entities;
using MySql.Data.MySqlClient;

using hds.databases.interfaces;
using hds.shared;

namespace hds.databases{

    public class MyMarginDBAccess : IMarginDBHandler{
		
		private IDbConnection conn;
		private IDbCommand queryExecuter;
		private IDataReader dr;
		private XmlParser xmlParser;
		
		private MatrixDbContext dbContext;
		
		public MyMarginDBAccess(){

            var config = Store.config;
            dbContext = new MatrixDbContext(config.dbParams);
			/* Params: Host, port, database, user, password */
            conn = new MySqlConnection("Server=" + config.dbParams.Host + ";" + "Database=" + config.dbParams.DatabaseName + ";" + "User ID=" + config.dbParams.Username + ";" + "Password=" + config.dbParams.Password + ";" + "Pooling=false;");
			
		}

        public Character GetCharInfo(int charId)
        {
	        return dbContext.Characters.SingleOrDefault(c => c.CharId == (ulong) charId);
        }

        public List<MarginInventoryItem> LoadInventory(int charId)
        {
	        var items = dbContext.Inventories.Where(i => i.CharId == (ulong) charId);
	        List<MarginInventoryItem> Inventory = new List<MarginInventoryItem>();
            foreach (var inventoryItem in items)
            {
	            MarginInventoryItem item = new MarginInventoryItem();
	            item.setItemID((uint) inventoryItem.Goid);
	            item.setPurity((UInt16)inventoryItem.Purity);
	            item.setAmount((UInt16)inventoryItem.Count);
	            item.setSlot(inventoryItem.Slot);
	            Inventory.Add(item);
            }
            
            return Inventory;
        }

        public string LoadAllHardlines()
        {
	        var hardLines = dbContext.DataHardlines.ToList();

            string hexpacket = "";
            foreach (var hardline in hardLines)
            {
	            string districtHex =  StringUtils.bytesToString_NS(NumericalUtils.uint16ToByteArrayShort((UInt16)hardline.DistrictId));
	            string hardlineHex = StringUtils.bytesToString_NS(NumericalUtils.uint16ToByteArray((UInt16)hardline.HardLineId,1));
	            hexpacket = hexpacket + districtHex + hardlineHex + "0000";
            }
            
            return hexpacket;
        }

        public List<MarginAbilityItem> LoadAbilities(int charId)
        {
	        List<CharAbility> charAbilities =
		        dbContext.CharAbilities.Where(ca => ca.CharId == charId).OrderBy(ca => ca.Slot).ToList();

	        List<MarginAbilityItem> abilities = new List<MarginAbilityItem>();
	        foreach (var charAbility in charAbilities)
	        {
		        MarginAbilityItem ability = new MarginAbilityItem();
		        ability.setSlot((ushort) charAbility.Slot);
		        ability.setAbilityID((Int32)charAbility.AbilityId);
		        ability.setLevel((UInt16)charAbility.Level);
		        ability.setLoaded(charAbility.IsLoaded);
		        abilities.Add(ability);
	        }
	        
            return abilities;
        }


        public UInt32 getNewCharnameID(string handle, UInt32 userId)
        {
	        var character =  dbContext.Characters.FirstOrDefault(c => c.Handle == handle && c.IsDeleted == 0);

			int number=0;
			if (character != null)
			{
				number = (int) character.CharId;
			}

			// It does exist... hence... invent a new one
			if(number!=0){
				return 0;
			}
            
            UInt32 charId = CreateNewCharacter(handle, userId,1);
            return charId;
			
		}
		
		public UInt32 CreateNewCharacter(string handle, UInt32 userid, UInt32 worldId){
			
			
            UInt32 charId = 0;
			//TODO: Complete with real data from a hashtable (or something to do it faster);
			//TODO: find values for uria starting place
			var character = new Character {Handle = handle, UserId = userid, WorldId = (ushort) worldId};
			dbContext.Characters.Add(character);


			//As i didnt find a solution for "last_insert_id" in C# we must fetch the last row by a normal query
			conn.Open();
            string sqlQuery = "SELECT charId FROM characters WHERE userId='" + userid.ToString() + "' AND worldId='" + worldId.ToString() + "' AND is_deleted='0' ORDER BY charId DESC LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            dr = queryExecuter.ExecuteReader();

            while (dr.Read())
            {
                charId = (UInt32)dr.GetDecimal(0);
            }

            conn.Close();

	        // Create RSI Entry
            conn.Open();
            string sqlRSIQuery = "INSERT INTO rsivalues SET charid='" + charId.ToString() + "' ";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlRSIQuery;
            queryExecuter.ExecuteNonQuery();

			conn.Close();

            return charId;
			
		}

        public void updateCharacter(string firstName, string lastName, string background, UInt32 charID)
        {
            string theQuery = "UPDATE characters SET firstName = '" + firstName + "', lastName='" + lastName + "', background='" + background + "' WHERE charid='" + charID.ToString() + "' ";
            conn.Open();
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = theQuery;
            queryExecuter.ExecuteNonQuery();
            conn.Close();
        }


        public void updateRSIValue(string field, string value, UInt32 charID)
        {
            string theQuery = "UPDATE rsivalues SET " + field + "='" + value + "' WHERE charid='" + charID.ToString() + "' ";
            conn.Open();
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = theQuery;
            queryExecuter.ExecuteNonQuery();
            conn.Close();
        }

        public void addAbility(Int32 abilityID, UInt16 slotID, UInt32 charID, UInt16 level, UInt16 is_loaded)
        {
            string theQuery = "INSERT INTO char_abilities SET char_id='" + charID.ToString()  + "', slot='" + slotID.ToString() + "', ability_id='" + abilityID.ToString() + "', level='" + level.ToString() + "', is_loaded='" + is_loaded.ToString() + "', added=NOW() ";
            conn.Open();
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = theQuery;
            queryExecuter.ExecuteNonQuery();
            conn.Close();
        }


	    public void AddItemToSlot(UInt32 itemGoId, ushort slotId, UInt32 charId)
	    {

		    // Faster way instead of checking on every item type 
		    if (itemGoId != 0 && slotId != 0 && charId != 0)
		    {
			    conn.Open();
			    string sqlQuery = "INSERT INTO inventory SET charId = '" + charId.ToString() + "' , goid = '" + itemGoId.ToString() + "', slot = '" + slotId.ToString() + "', created = NOW() ";
			    queryExecuter = conn.CreateCommand();
			    queryExecuter.CommandText = sqlQuery;
			    queryExecuter.ExecuteNonQuery();   
			    conn.Close();
		    }
	    }

	    public void deleteCharacter(UInt64 charId)
        {
            // This is a "soft-delete" Method. The Data wouldnt be deleted but will be invisible to the users.
            conn.Open();
            string sqlSoftDeleteChar = "UPDATE characters SET is_deleted='1' WHERE charId = '" + charId.ToString() + "' ";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlSoftDeleteChar;
            queryExecuter.ExecuteNonQuery();
            conn.Close();
        }
	}
}
