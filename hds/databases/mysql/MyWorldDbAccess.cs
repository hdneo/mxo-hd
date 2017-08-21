using System;
using System.Collections;
using System.Data;
using MySql.Data.MySqlClient;

using hds.databases.interfaces;
using hds.shared;
using System.Collections.Generic;
using System.IO;

namespace hds.databases{
	
    public class MyWorldDbAccess: IWorldDBHandler{

		private IDbConnection conn;
		private IDbCommand queryExecuter;
		private IDataReader dr;

		public MyWorldDbAccess(){
            var config = Store.config;
			/* Params: Host, port, database, user, password */
            conn = new MySqlConnection("Server=" + config.dbParams.Host + ";" + "Database=" + config.dbParams.DatabaseName + ";" + "User ID=" + config.dbParams.Username + ";" + "Password=" + config.dbParams.Password + ";" + "Pooling=false;");
			conn.Open();
		}
		
		//Handle the closing on end :D
		public void CloseConn (){
			conn.Close();
		}
		
		public UInt32 getUserIdForCharId(byte[] charIdHex){
			 
			UInt32 charId = NumericalUtils.ByteArrayToUint32(charIdHex,1);
			Output.OptWriteLine("[WORLD] Checking from DB:"+charId);
			string sqlQuery="Select userid from characters where charid ='"+charId+"'";
			queryExecuter= conn.CreateCommand();
			queryExecuter.CommandText = sqlQuery;					
			dr= queryExecuter.ExecuteReader();
			
			UInt32 userId = new UInt32();
			
			while(dr.Read()){
				userId = (UInt32)dr.GetDecimal(0);	
			}
			
			dr.Close();
			
			return userId;
			
		}

        public ArrayList fetchFriendList(UInt32 charId)
        {
            ArrayList friends = new ArrayList();
            string query = "SELECT C.handle, C.is_online, B.friendId FROM characters C, buddylist B WHERE B.charId = '" + charId + "' AND B.friendId=C.charId ";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = query;
            dr = queryExecuter.ExecuteReader();

            while (dr.Read())
            {
                Hashtable data = new Hashtable();
                data.Add("handle", dr.GetString(0));
                data.Add("online", dr.GetInt16(1));
                friends.Add(data);
            }

            dr.Close();
            // ToDo: Write query 
            // ToDo2: add online flag to characters and handle this on "connect" and "disconnect"
            return friends;
            
        }

		
		public string getPathForDistrictKey(string key){
			
			string sqlQuery="select path from districts where districts.key = '"+key+"'";
			queryExecuter= conn.CreateCommand();
			queryExecuter.CommandText = sqlQuery;					
			dr= queryExecuter.ExecuteReader();
			
			string path = "";
			
			while(dr.Read()){
				path = (string)dr.GetString(0);	
			}
			
			dr.Close();
			
			return path;
			
		}
		

		public bool fetchWordList(ref WorldList wl){

			// Doesnt exist by default
			wl.setExistance(false);

			string sqlQuery = "select * from users where username='"+wl.getUsername()+"' and passwordmd5='"+wl.getPassword()+"';";
			queryExecuter= conn.CreateCommand();
			queryExecuter.CommandText = sqlQuery;					
			dr= queryExecuter.ExecuteReader();
			
			
			while(dr.Read()){
				// Player is on the DB
				wl.setExistance(true);
				wl.setUserID((int) dr.GetDecimal(0));
				dr.GetBytes(6,0,wl.getPrivateExponent(),0,96);
				dr.GetBytes(5,0,wl.getPublicModulus(),0,96);
				wl.setTimeCreated((int)dr.GetDecimal(7));
			}
			
			dr.Close();
			
			// If doesnt exist... should not do more things
			if (!wl.getExistance()){
                Output.writeToLogForConsole("[WORLD DB ACCESS] fetchWordList : Player not found on DB with #" + wl.getUsername() + "# and #" + wl.getPassword() + "#");
				conn.Close();
				return false;
			}
			return true;
			
		}


        public void updateSourceHlForObjectTracking(UInt16 sourceDistrict, UInt16 sourceHl, UInt32 lastObjectId)
        {
            string sqlQuery = "SELECT id,HardlineId, DistrictId, objectId FROM data_hardlines WHERE DistrictId = '" + sourceDistrict.ToString() + "' AND HardlineId = '" + sourceHl.ToString() + "' LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            dr = queryExecuter.ExecuteReader();

            UInt16 theId = 0;
            UInt16 hardlineId = 0;
            UInt16 districtId = 0;
            UInt32 objectId = 0;

            while (dr.Read())
            {
                theId = (UInt16)dr.GetInt16(0);
                hardlineId = (UInt16)dr.GetInt16(1);
                districtId = (UInt16)dr.GetInt16(2);
                objectId = (UInt32)dr.GetInt32(3);

                
            }

            dr.Close();

            if (objectId == 0 || objectId != lastObjectId)
                {
                    string updateQuery = "UPDATE data_hardlines SET objectId = '" + lastObjectId.ToString() + "' WHERE id = '" + theId.ToString() + "' LIMIT 1";
                    queryExecuter = conn.CreateCommand();
                    queryExecuter.CommandText = updateQuery;
                    queryExecuter.ExecuteNonQuery();
                    Output.WriteLine("[WORLD DB] UPDATE Hardline " + hardlineId.ToString() + " in District " + districtId.ToString() + " with Object ID : "+lastObjectId.ToString());
                }
            
        }


        public void updateLocationByHL(UInt16 district, UInt16 hardline){

            string sqlQuery = "SELECT DH.X,DH.Y,DH.Z,DH.ROT,DIS.key,DH.DistrictId FROM data_hardlines AS DH, districts as DIS WHERE DH.DistrictId = '" + district.ToString() + "' AND DH.HardLineId = '" + hardline.ToString() + "' AND DH.DistrictId=DIS.id ";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            dr = queryExecuter.ExecuteReader();

            while (dr.Read()){
                double x = (double)(dr.GetFloat(0));
                double y = (double)(dr.GetFloat(1));
                double z = (double)(dr.GetFloat(2));
                string disKey = dr.GetString(4);
                Output.WriteLine("USER DIS IS NOW " + disKey);
                Store.currentClient.playerData.setDistrict(disKey);
                Store.currentClient.playerData.setDistrictId((uint)dr.GetInt16(5));
                Store.currentClient.playerInstance.Position.setValue(NumericalUtils.doublesToLtVector3d(x, y, z));
                //Store.currentClient.playerInstance.YawInterval.setValue((byte)dr.GetDecimal(3));
            }
            dr.Close();
            savePlayer(Store.currentClient);
        }

        public void setBackground(string backgroundText)
        {
            UInt32 charID = Store.currentClient.playerData.getCharID();

            string sqlQuery = "UPDATE characters SET background = '" + backgroundText + "' WHERE charId = '" + charID + "' LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            queryExecuter.ExecuteNonQuery();
        }
		
		public void setPlayerValues(){

            UInt32 charID = Store.currentClient.playerData.getCharID();
			string sqlQuery="Select handle,x,y,z,rotation,healthC,healthM,innerStrC,innerStrM,level,profession,alignment,pvpflag,firstName,lastName,exp,cash,district,districtId,factionId,crewId from characters where charId='"+charID+"'";
			queryExecuter= conn.CreateCommand();
			queryExecuter.CommandText = sqlQuery;					
			dr= queryExecuter.ExecuteReader();
			
			while(dr.Read()){
				
				Store.currentClient.playerInstance.CharacterID.setValue(charID);
				Store.currentClient.playerInstance.CharacterName.setValue(dr.GetString(0));

				double x = (double)(dr.GetFloat(1));
				double y = (double)(dr.GetFloat(2));
				double z = (double)(dr.GetFloat(3));

				Store.currentClient.playerInstance.Position.setValue(NumericalUtils.doublesToLtVector3d(x,y,z));
				Store.currentClient.playerInstance.YawInterval.setValue((byte)dr.GetDecimal(4));	
				Store.currentClient.playerInstance.Health.setValue((UInt16)dr.GetDecimal(5));	
				Store.currentClient.playerInstance.MaxHealth.setValue((UInt16)dr.GetDecimal(6));	
				
				Store.currentClient.playerInstance.InnerStrengthAvailable.setValue((UInt16)dr.GetDecimal(7));	
				Store.currentClient.playerInstance.InnerStrengthMax.setValue((UInt16)dr.GetDecimal(8));	
				
				Store.currentClient.playerInstance.Level.setValue((byte)dr.GetDecimal(9));
				
   				Store.currentClient.playerInstance.TitleAbility.setValue((UInt32) dr.GetDecimal(10));
				Store.currentClient.playerInstance.OrganizationID.setValue((byte)dr.GetDecimal(11));

				//data.setPlayerValue("pvpFlag",(int)dr.GetDecimal(12));
				Store.currentClient.playerInstance.RealFirstName.setValue(dr.GetString(13));
				Store.currentClient.playerInstance.RealLastName.setValue(dr.GetString(14));
				
				Store.currentClient.playerData.setExperience((long)dr.GetDecimal(15));
				Store.currentClient.playerData.setInfo((long)dr.GetDecimal(16));
				Store.currentClient.playerData.setDistrict(dr.GetString(17));
                Store.currentClient.playerData.setDistrictId((uint)dr.GetInt16(18));
				UInt32 factionId = (uint) dr.GetInt16(19);
				UInt32 crewId = (uint) dr.GetInt16(20);

				if (factionId > 0)
				{
					Store.currentClient.playerInstance.FactionID.enable();
					Store.currentClient.playerInstance.FactionID.setValue(factionId);
				}

				if (crewId > 0)
				{
					Store.currentClient.playerInstance.CrewID.enable();
					Store.currentClient.playerInstance.CrewID.setValue(crewId);
				}

			}
			
			dr.Close();
			
		}


        public Hashtable getCharInfo(UInt32 charId)
        {

            string sqlQuery = "SELECT firstName,lastName,background, district, repMero, repMachine, repNiobe, repEPN, repCYPH, repGM, repZion, exp, cash FROM characters WHERE charId = '" + charId.ToString() + "' LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;

            dr = queryExecuter.ExecuteReader();

            Hashtable data = new Hashtable();
            while (dr.Read())
            {
                data.Add("firstname", dr.GetString(0));
                data.Add("lastname", dr.GetString(1));
                data.Add("background", dr.GetString(2));
                data.Add("district", dr.GetString(3));
                data.Add("repMero",dr.GetInt16(4));
                data.Add("repMachine", dr.GetInt16(5));
                data.Add("repNiobe", dr.GetInt16(6));
                data.Add("repEPN", dr.GetInt16(7));
                data.Add("repCYPH", dr.GetInt16(8));
                data.Add("repGM", dr.GetInt16(9));
                data.Add("repZion", dr.GetInt16(10));
                data.Add("exp", dr.GetInt32(11));
                data.Add("cash", dr.GetInt32(12));
            }
            dr.Close();

            return data;
        }

		public void setRsiValues(){
			int charID = (int) Store.currentClient.playerData.getCharID();
			string sqlQuery="Select sex,body,hat,face,shirt,coat,pants,shoes,gloves,glasses,hair,facialdetail,shirtcolor,pantscolor,coatcolor,shoecolor,glassescolor,haircolor,skintone,tattoo,facialdetailcolor,leggins from rsivalues where charId='"+charID+"'";
			queryExecuter= conn.CreateCommand();
			queryExecuter.CommandText = sqlQuery;					
			dr= queryExecuter.ExecuteReader();
			int[] rsiValues  = new int[22];
			
			while(dr.Read()){
				//we will read just one row
				for(int i = 0;i<rsiValues.Length;i++){
					int temp =(int) dr.GetDecimal(i); //int to int
					rsiValues[i]=temp;
				}
			}

			Store.currentClient.playerData.setRsiValues(rsiValues);
			dr.Close();
		}


        public void updateRsiPartValue(string part, uint value)
        {
            int charID = (int) Store.currentClient.playerData.getCharID();
            string sqlQuery = "UPDATE rsivalues SET " + part + "= " + value + " WHERE charid= '" + charID.ToString() + "' LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            queryExecuter.ExecuteNonQuery();
        }
		
		

		public void savePlayer(WorldClient client){
			
			UInt32 charID = NumericalUtils.ByteArrayToUint32(client.playerInstance.CharacterID.getValue(),1);
			string handle = StringUtils.charBytesToString_NZ(client.playerInstance.CharacterName.getValue());
			
			int [] rsiValues = client.playerData.getRsiValues();
			
			double x=0;double y=0;double z=0;
			byte[] Ltvector3d = client.playerInstance.Position.getValue();
			NumericalUtils.LtVector3dToDoubles(Ltvector3d,ref x,ref y,ref z);
			
			int rotation =(int)client.playerInstance.YawInterval.getValue()[0];
			
			string sqlQuery="update characters set x = '"+(float)x+"',y='"+(float)y+"',z='"+(float)z+"',rotation='"+rotation+"', districtId='" + client.playerData.getDistrictId()+"' where handle='"+handle+"';";
			queryExecuter= conn.CreateCommand();
			queryExecuter.CommandText = sqlQuery;
			Output.WriteLine(StringUtils.bytesToString(StringUtils.stringToBytes(sqlQuery)));
            Output.writeToLogForConsole(queryExecuter.ExecuteNonQuery() + " rows affecting saving");
						
			string rsiQuery="update rsivalues set sex='"+rsiValues[0]+"',body='"+rsiValues[1]+"',hat='"+rsiValues[2]+"',face='"+rsiValues[3]+"',shirt='"+rsiValues[4]+"',coat='"+rsiValues[5]+"',pants='"+rsiValues[6]+"',shoes='"+rsiValues[7]+"',gloves='"+rsiValues[8]+"',glasses='"+rsiValues[9]+"',hair='"+rsiValues[10]+"',facialdetail='"+rsiValues[11]+"',shirtcolor='"+rsiValues[12]+"',pantscolor='"+rsiValues[13]+"',coatcolor='"+rsiValues[14]+"',shoecolor='"+rsiValues[15]+"',glassescolor='"+rsiValues[16]+"',haircolor='"+rsiValues[17]+"',skintone='"+rsiValues[18]+"',tattoo='"+rsiValues[19]+"',facialdetailcolor='"+rsiValues[20]+"',leggins='"+rsiValues[21]+"' where charId='"+charID+"';";
			queryExecuter= conn.CreateCommand();
			queryExecuter.CommandText = rsiQuery;
            Output.writeToLogForConsole("[WORLD DB ACCESS ]" + rsiQuery);
            queryExecuter.ExecuteNonQuery();
			
		}


        public void updateInventorySlot(UInt16 sourceSlot, UInt16 destSlot)
        {
            UInt32 charID = Store.currentClient.playerData.getCharID();
            string sqlQuery = "UPDATE inventory SET slot = '" + destSlot.ToString() + "' WHERE slot = '" + sourceSlot.ToString() + "' AND charID = '" + charID.ToString() + "' LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            queryExecuter.ExecuteNonQuery();
        }

        public bool isSlotinUseByItem(UInt16 slotId)
        {
            bool isSlotInUse = false;
            UInt32 charID = Store.currentClient.playerData.getCharID();
            string sqlQuery = "SELECT slot FROM inventory WHERE slot = '" + slotId.ToString() + "' AND charID = '" +
                              charID.ToString() + "' LIMIT 1";

            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            dr = queryExecuter.ExecuteReader();

            UInt16 freeSlot = 0;

            if (dr.Read())
            {
                isSlotInUse = true;
            }

            dr.Close();
            return isSlotInUse;

        }

        public UInt16 getCrewMemberCountByCrewName(string crewName)
        {
            string sqlQuery = "SELECT COUNT(cm.id) as count_members FROM crew_members cm LEFT JOIN crews c ON cm.crew_id=c.id WHERE c.crew_name='" + crewName + "' LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            dr = queryExecuter.ExecuteReader();

            UInt16 countCrewMembers = 0;

            if (dr.Read())
            {
                countCrewMembers = (UInt16)dr.GetInt16(0);

            }

            dr.Close();
            return countCrewMembers;
        }

	    public ushort getCrewIdByCrewMasterHandle(string playerHandle)
	    {
		    // ToDo: we need to proove if this can work this way 
		    // ToDo: can only the master invite other players ? i am not sure then we need to change this
		    // ToDo: make DB easier
		    string sqlQuery = "SELECT c.id, c.crew_name FROM crew_members cm LEFT JOIN crews c ON cm.crew_id=c.id WHERE c.master_player_handle='" + playerHandle + "' LIMIT 1";
		    queryExecuter = conn.CreateCommand();
		    queryExecuter.CommandText = sqlQuery;
		    dr = queryExecuter.ExecuteReader();

		    UInt16 countCrewMembers = 0;

		    if (dr.Read())
		    {
			    countCrewMembers = (UInt16)dr.GetInt16(0);

		    }

		    dr.Close();
		    return countCrewMembers;
	    }

	    public string getFactionNameById(uint factionId)
	    {
		    string sqlQuery = "SELECT name FROM factions WHERE id =" + factionId + " LIMIT 1";
		    queryExecuter = conn.CreateCommand();
		    queryExecuter.CommandText = sqlQuery;
		    dr = queryExecuter.ExecuteReader();

		    string factionName = "";

		    if (dr.Read())
		    {

			    factionName = dr.GetString(0);

		    }
		    dr.Close();
		    return factionName;
	    }

	    public bool isCrewNameAvailable(string crewName)
        {
            bool isCrewNameAvailable = true;
	        crewName = crewName.Replace("'", @"\'");

            string sqlQuery = "SELECT id,created_at,deleted_at FROM crews WHERE crew_name='" + crewName + "' AND deleted_at > NOW() LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            dr = queryExecuter.ExecuteReader();

            if (dr.Read())
            {
                UInt16 memberCount = getCrewMemberCountByCrewName(crewName);

                UInt32 crewId = (UInt32)dr.GetInt32(0);
                DateTime createdAt = dr.GetDateTime(1);
                DateTime deletedAt = dr.GetDateTime(2);

                isCrewNameAvailable = false;

	            // Lets check if more than one player is in the crew and if created_at is greater than one day
                if ((DateTime.Now - deletedAt).TotalHours > 24 && memberCount < 2)
                {
                    deleteCrew(crewId);
	                isCrewNameAvailable = true;
                }


            }

            dr.Close();
            return isCrewNameAvailable;
        }

        public void deleteCrew(UInt32 crewId)
        {
	        // Delete the Crew
            string sqlQueryDelete = "UPDATE crews SET deleted_at = NOW() WHERE id=" + crewId + " LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQueryDelete;
            queryExecuter.ExecuteNonQuery();

	        // Delete the Player Crew ID
	        string updatePlayers = "UPDATE characters SET crewId = 0 WHERE crewId = " + crewId;
	        queryExecuter = conn.CreateCommand();
	        queryExecuter.CommandText = updatePlayers;
	        queryExecuter.ExecuteNonQuery();
	        
	        // Delete the Player Crew ID
	        string deleteMembers = "DELETE FROM crew_members WHERE crew_id = " + crewId;
	        queryExecuter = conn.CreateCommand();
	        queryExecuter.CommandText = deleteMembers;
	        queryExecuter.ExecuteNonQuery();
        }

        public void addCrew(string crewName, string masterHandle)
        {
	        crewName = crewName.Replace("'", @"\'");
            string sqlQueryInsert = "INSERT INTO crews SET crew_name= '" + crewName + "', created_at =NOW() ";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQueryInsert;
            queryExecuter.ExecuteNonQuery();
        }

        public UInt16 getFirstNewSlot()
        {
            UInt32 charID = Store.currentClient.playerData.getCharID();

            // We want the next free slot which is not in the "wearing" range (which starts with 97)
            string sqlQuery = "SELECT inv.slot +1 as freeSlot FROM inventory inv WHERE NOT EXISTS (SELECT * FROM inventory inv2 WHERE inv2.slot = inv.slot + 1 AND charID = '" + charID.ToString() + "') AND charID = '" + charID.ToString() + "' AND slot<97 LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            dr = queryExecuter.ExecuteReader();

            UInt16 freeSlot = 0;
            while (dr.Read()){
                freeSlot = (UInt16)dr.GetInt16(0);
            }
            dr.Close();
            return freeSlot;
        }

        public void addItemToInventory(UInt16 slotId, UInt32 itemGoID)
        {
            UInt32 charID = Store.currentClient.playerData.getCharID();

            string sqlQuery = "INSERT INTO inventory SET charId = '" + charID.ToString() + "' , goid = '" + itemGoID.ToString() + "', slot = '" + slotId.ToString() + "', created = NOW() ";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            queryExecuter.ExecuteNonQuery();
        }

        public UInt32 GetItemGOIDAtInventorySlot(UInt16 slotId)
        {
            UInt32 charID = Store.currentClient.playerData.getCharID();

            string sqlQuery = "SELECT goid FROM inventory WHERE charID = '" + charID + "' AND slot = '" + slotId + "' LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            dr = queryExecuter.ExecuteReader();

            UInt32 GoID = 0;
            while (dr.Read()){
                GoID  = (UInt32)dr.GetInt32(0);
            }
            dr.Close();
            return GoID;
        }


        public void updateAbilityLoadOut(List<UInt16> abilitySlots, uint loaded)
        {
            UInt32 charID = Store.currentClient.playerData.getCharID();
            string sqlQuery = "";
            foreach(ushort slot in abilitySlots)
            {
                sqlQuery += "UPDATE char_abilities SET is_loaded = " + loaded.ToString() + " WHERE char_id = " + charID.ToString() + " AND slot = " + slot.ToString() + ";";
            }

            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            queryExecuter.ExecuteNonQuery();

        }
    }
}

