using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

using hds.databases.interfaces;
using hds.shared;

namespace hds.databases{

    public class MyMarginDBAccess : IMarginDBHandler{
		
		private IDbConnection conn;
		private IDbCommand queryExecuter;
		private IDataReader dr;
		private XmlParser xmlParser;
		
		public MyMarginDBAccess(){

            var config = Store.config;
			/* Params: Host, port, database, user, password */
            conn = new MySqlConnection("Server=" + config.dbParams.Host + ";" + "Database=" + config.dbParams.DatabaseName + ";" + "User ID=" + config.dbParams.Username + ";" + "Password=" + config.dbParams.Password + ";" + "Pooling=false;");
			
		}

        public MarginCharacter getCharInfo(int charId)
        {
            conn.Open();
            string sqlQuery = "SELECT firstName,lastName,background, district, districtId, repMero, repMachine, repNiobe, repEPN, repCYPH, repGM, repZion, exp, cash,created FROM characters WHERE charId = '" + charId.ToString() + "' LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;

            dr = queryExecuter.ExecuteReader();

            MarginCharacter marginCharacter = null;
            if (dr.Read())
            {
                marginCharacter = new MarginCharacter();
                marginCharacter.firstname = dr.GetString(0);
                marginCharacter.lastname = dr.GetString(1);
                marginCharacter.background = dr.GetString(2);
                marginCharacter.district = dr.GetString(3);
                marginCharacter.districtId = (UInt16)dr.GetInt16(4);
                marginCharacter.repMero = (UInt16)dr.GetInt16(5);
                marginCharacter.repMachine = (UInt16)dr.GetInt16(6);
                marginCharacter.repNiobe = (UInt16)dr.GetInt16(7);
                marginCharacter.repEPN = (UInt16)dr.GetInt16(8);
                marginCharacter.repCYPH = (UInt16)dr.GetInt16(9);
                marginCharacter.repGM = (UInt16)dr.GetInt16(10);
                marginCharacter.repZion = (UInt16)dr.GetInt16(11);
                marginCharacter.exp = (UInt32)dr.GetInt32(12);
                marginCharacter.cash = (UInt32)dr.GetInt32(13);

            }
            dr.Close();
            conn.Close();

            return marginCharacter;
        }

        public List<MarginInventoryItem> loadInventory(int charId)
        {
            conn.Open();
            string sqlQuery = "SELECT slot, goid, count, purity FROM inventory WHERE charId = '" + charId.ToString() + "' ORDER BY slot ASC ";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;

            dr = queryExecuter.ExecuteReader();


            List<MarginInventoryItem> Inventory = new List<MarginInventoryItem>();
            while (dr.Read())
            {
                MarginInventoryItem item = new MarginInventoryItem();
                item.setItemID((UInt32)dr.GetInt64(1));
                item.setPurity((UInt16)dr.GetInt16(3));
                item.setAmount((UInt16)dr.GetInt16(2));
                item.setSlot((UInt16)dr.GetInt16(0));
                Inventory.Add(item);
                
            }
            conn.Close();
            return Inventory;
        }

        public string loadAllHardlines()
        {
            conn.Open();
            string sqlQuery = "SELECT DistrictId, HardLineId FROM data_hardlines ORDER BY DistrictId ASC ";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;

            dr = queryExecuter.ExecuteReader();

            string hexpacket = "";

            while (dr.Read())
            {

                string districtHex =  StringUtils.bytesToString_NS(NumericalUtils.uint16ToByteArrayShort((UInt16)dr.GetInt16(0)));
                string hardlineHex = StringUtils.bytesToString_NS(NumericalUtils.uint16ToByteArray((UInt16)dr.GetInt16(1),1));
                hexpacket = hexpacket + districtHex + hardlineHex + "0000";
            }
            conn.Close();
            return hexpacket;
        }

        public List<MarginAbilityItem> loadAbilities(int charId)
        {
            conn.Open();
            string sqlQuery = "SELECT slot, ability_id,level, is_loaded FROM char_abilities WHERE char_id = '" + charId.ToString() + "' ORDER BY slot ASC ";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;

            dr = queryExecuter.ExecuteReader();

            List<MarginAbilityItem> Abilities = new List<MarginAbilityItem>();

            while (dr.Read())
            {
                MarginAbilityItem ability = new MarginAbilityItem();
                ability.setSlot((UInt16)dr.GetInt16(0));
                ability.setAbilityID((Int32)dr.GetInt64(1));
                ability.setLevel((UInt16)dr.GetInt16(2));
                ability.setLoaded(dr.GetBoolean(3));
                Abilities.Add(ability);
            }
            conn.Close();
            return Abilities;
        }


        public UInt32 getNewCharnameID(string handle, UInt32 userId)
        {
			
			conn.Open();

            string sqlQuery = "SELECT count( * ) AS number FROM characters WHERE LOWER( handle ) = LOWER( '" + handle + "' ) AND is_deleted='0' ;";
			queryExecuter= conn.CreateCommand();
			queryExecuter.CommandText = sqlQuery;					
			dr= queryExecuter.ExecuteReader();
				
			int number=0;
			
			while(dr.Read()){
				number = (int) dr.GetDecimal(0);
			}
			
			dr.Close();
			
			// It does exist... hence... invent a new one
			if(number!=0){
				conn.Close();
				return 0;
			}
            conn.Close();
            UInt32 charId = createNewCharacter(handle, userId,1);
            return charId;
			
		}
		
		public UInt32 createNewCharacter(string handle, UInt32 userid, UInt32 worldId){
			
			conn.Open();
            UInt32 charId = 0;
			//TODO: Complete with real data from a hashtable (or something to do it faster);
			//TODO: find values for uria starting place
			string sqlInsertQuery="INSERT INTO characters SET userid = '" + userid.ToString() + "', worldid='" + worldId.ToString() + "', status='0', handle = '" + handle + "', created=NOW() ";
			queryExecuter= conn.CreateCommand();
            queryExecuter.CommandText = sqlInsertQuery;					
			queryExecuter.ExecuteNonQuery();


            //As i didnt find a solution for "last_insert_id" in C# we must fetch the last row by a normal query
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
