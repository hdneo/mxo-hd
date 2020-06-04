using System;
using System.Collections;
using System.Data;

using hds.databases.interfaces;
using hds.shared;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using hds.world.Structures;
using MySql.Data.MySqlClient;

namespace hds.databases{
	
    public class MyWorldDbAccess: IWorldDBHandler{

		private IDbConnection conn;
		private IDbCommand queryExecuter;
		private IDataReader dr;

		public MyWorldDbAccess(){
            var config = Store.config;
			/* Params: Host, port, database, user, password */
            conn = new MySqlConnection("Server=" + config.dbParams.Host + ";" + "Database=" + config.dbParams.DatabaseName + ";" + "User ID=" + config.dbParams.Username + ";" + "Password=" + config.dbParams.Password + ";" + "Pooling=false;");
		}

		public void OpenConnection()
		{
			if (conn.State == ConnectionState.Closed)
			{
				conn.Open();
			}
		}

		public void CloseConnection()
		{
			if (conn.State != ConnectionState.Closed)
			{
				conn.Close();
			}   
		}
		
		public void ExecuteNonResultQuery(string query)
		{
			OpenConnection();
			string updateQuery = query;
			queryExecuter = conn.CreateCommand();
			queryExecuter.CommandText = updateQuery;
			queryExecuter.ExecuteNonQuery();
			CloseConnection();
		}
		
		
		public UInt32 getUserIdForCharId(byte[] charIdHex){
			OpenConnection();	 
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
			CloseConnection();
			return userId;
			
		}

		public void AddHandleToFriendList(string handleToAdd, UInt32 charId)
		{
			UInt32 friendId = getCharIdByHandle(handleToAdd);
			ExecuteNonResultQuery("INSERT INTO buddylist SET charId='" + charId +
			                      "', friendId='" + friendId + "', is_ignored=0 ");
			
		}

		public void RemoveHandleFromFriendList(string handleToRemove, UInt32 charId)
		{
			UInt32 friendId = getCharIdByHandle(handleToRemove);
			ExecuteNonResultQuery(
				"DELETE FROM buddylist WHERE charId='" + charId + "' AND friendId='" + friendId + "' ");
			
		}

		public ArrayList FetchPlayersWhoAddedMeToBuddylist(UInt32 charId)
		{
			OpenConnection();
			ArrayList friends = new ArrayList();
			string query = "SELECT C.handle, C.charId, C.is_online, B.friendId FROM buddylist B LEFT JOIN characters C ON B.charId=C.charId WHERE B.friendId = '" + charId + "' ";
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
			CloseConnection();
			return friends;
		}
		
        public ArrayList fetchFriendList(UInt32 charId)
        {
	        OpenConnection();
            ArrayList friends = new ArrayList();
            string query = "SELECT C.handle, C.is_online, B.friendId FROM buddylist B LEFT JOIN characters C ON B.friendId=C.charId WHERE B.charId = '" + charId + "' ";
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
	        CloseConnection();
	        return friends;
            
        }


        public Faction fetchFaction(uint factionId)
        {
	        Faction factionData = new Faction();
	        OpenConnection();
	        string query = "SELECT f.id, f.name, f.master_player_handle, f.money FROM factions f LEFT JOIN characters c ON f.id=c.factionId WHERE f.id= '" + factionId + "' LIMIT 1";
	        queryExecuter = conn.CreateCommand();
	        queryExecuter.CommandText = query;
	        dr = queryExecuter.ExecuteReader();

	        while (dr.Read())
	        {
		        factionData.factionId = (UInt32)dr.GetInt32(0);
		        factionData.name = dr.GetString(1);
		        factionData.masterPlayerHandle = dr.GetString(2);
		        factionData.money = (UInt32) dr.GetInt32(3);
	        }

	        dr.Close();
	        CloseConnection();
	        
	        factionData.crews = GetCrewsForFaction(factionData.factionId);
	        return factionData;
        }

        public void IncreaseCrewMoney(UInt32 crewId, UInt32 amount)
        {
	        ExecuteNonResultQuery("UPDATE crews SET money = money + '" + amount + "' WHERE id = '" + crewId.ToString() + "' LIMIT 1");
	       
        }
        
        public void DecreaseCrewMoney(UInt32 crewId, UInt32 amount)
        {
	        ExecuteNonResultQuery("UPDATE crews SET money = money - '" + amount + "' WHERE id = '" + crewId.ToString() +
	                              "' LIMIT 1");
	        
        }
        
        public void IncreaseFactionMoney(UInt32 crewId, UInt32 amount)
        {
	        ExecuteNonResultQuery("UPDATE factions SET money = money + '" + amount + "' WHERE id = '" +
	                              crewId.ToString() + "' LIMIT 1");
        }
        
        public void DecreaseFactionMoney(UInt32 crewId, UInt32 amount)
        {
	        ExecuteNonResultQuery("UPDATE factions SET money = money - '" + amount + "' WHERE id = '" +
	                              crewId.ToString() + "' LIMIT 1");
        }

        public Crew GetCrewData(UInt32 crewId)
        {
	        Crew theCrew = new Crew();
	        OpenConnection();
	        string query = "SELECT id, crew_name, master_player_handle, money, org, faction_id, faction_rank FROM crews WHERE id='" +
	                       crewId + "' LIMIT 1";
	        queryExecuter = conn.CreateCommand();
	        queryExecuter.CommandText = query;
	        dr = queryExecuter.ExecuteReader();

	        while (dr.Read())
	        {
		        theCrew.crewId = (UInt32) dr.GetInt32(0);
		        theCrew.crewName = dr.GetString(1);
		        theCrew.characterMasterName = dr.GetString(2);
		        theCrew.money = (UInt32) dr.GetInt32(3);
		        theCrew.org = (ushort) dr.GetInt16(4);
		        theCrew.factionId = (UInt32) dr.GetInt32(5);
		        theCrew.factionRank = (ushort) dr.GetInt32(6);
	        }

	        dr.Close();
	        CloseConnection();
	        return theCrew;
        }

        public List<Crew> GetCrewsForFaction(UInt32 factionID)
        {
	        List<Crew> tmpCrews = new List<Crew>();
			
	        OpenConnection();
	        string query =
		        "SELECT cr.id, cr.crew_name, cr.master_player_handle, cr.faction_id, cr.faction_rank, c.is_online FROM crews cr LEFT JOIN characters c ON cr.master_player_handle=c.handle WHERE cr.faction_id = '" +
		        factionID + "' ";
	        
	        queryExecuter = conn.CreateCommand();
	        queryExecuter.CommandText = query;
	        dr = queryExecuter.ExecuteReader();
	        while (dr.Read())
	        {
		        Crew theCrew = new Crew();
		        theCrew.crewId = (UInt32)dr.GetInt32(0);
		        theCrew.crewName = dr.GetString(1);
		        theCrew.characterMasterName = dr.GetString(2);
		        theCrew.factionId  = (UInt32) dr.GetInt32(3);
		        theCrew.factionRank = (ushort) dr.GetInt16(4);
		        theCrew.masterIsOnline = (ushort) dr.GetInt16(5);
		        tmpCrews.Add(theCrew);
	        }
	        dr.Close();
	        CloseConnection();

	        List<Crew> crews = new List<Crew>();
	        foreach (Crew theCrew in tmpCrews)
	        {
		        theCrew.masterPlayerCharId = getCharIdByHandle(theCrew.characterMasterName);
		        crews.Add(theCrew);
	        }
	        
	        return crews;
        }

        public List<CrewMember> GetCrewMembersForCrewId(UInt32 crewId)
        {
	        List<CrewMember> crewMembers = new List<CrewMember>();
	        OpenConnection();
	        string query =
		        "SELECT cm.char_id, cm.is_captain, cm.is_first_mate, c.handle, c.is_online FROM crew_members cm LEFT JOIN characters c ON cm.char_id=c.charId WHERE cm.crew_id = '" + crewId + "' ";
	        
	        queryExecuter = conn.CreateCommand();
	        queryExecuter.CommandText = query;
	        dr = queryExecuter.ExecuteReader();
	        while (dr.Read())
	        {
		        UInt32 charId = (UInt32)dr.GetInt32(0);
		        bool isCaptain = dr.GetBoolean(1);
		        bool isFirstMate = dr.GetBoolean(2);
		        string handle = dr.GetString(3);
		        ushort isOnline = (ushort) dr.GetInt16(4);
		        
		        CrewMember member = new CrewMember();
		        member.charId = charId;
		        member.handle = handle;
		        member.isCaptain = isCaptain;
		        member.isFirstMate = isFirstMate;
		        member.isOnline = isOnline;
		        crewMembers.Add(member);
	        }
	        CloseConnection();

	        return crewMembers;
        }

        public void AddMemberToCrew(UInt32 charId, UInt32 crewId, UInt32 factionId, ushort isCaptain, ushort isFirstMate)
        {
	        ExecuteNonResultQuery("INSERT INTO crew_members SET char_id='" + charId + "', crew_id='" + crewId + "', is_first_mate=" + isFirstMate + ", is_captain=" + isCaptain + ", created_at=NOW() ");
	        ExecuteNonResultQuery("UPDATE characters SET crewId='" + crewId + "', factionId='" + factionId + "' WHERE charId='" + charId + "' ");
        }

        public void RemoveMemberFromCrew(uint charId, uint crewId)
        {
	        ExecuteNonResultQuery("DELETE FROM crew_members WHERE crew_id = '" + crewId + "' AND char_id ='" +charId+ "' LIMIT 1");
	        ExecuteNonResultQuery("UPDATE characters SET crewId = 0, factionId = 0 WHERE charId='" + charId + "' ");
        }

        public void AddCrewToFaction(uint factionId, uint crewId)
        {
	        ExecuteNonResultQuery("UPDATE crews SET faction_id = '" + factionId + "' WHERE id='" + crewId + "' ");
        }

        public void RemoveCrewFromFaction(uint factionId, uint crewId)
        {
	        ExecuteNonResultQuery("UPDATE crews SET faction_id = '0' WHERE id='" + crewId + "' AND faction_id = '" + factionId + "' ");
        }

        public void CreateFaction(Crew crew1, Crew crew2, string factionName)
        {
	        throw new NotImplementedException();
        }


        public string getPathForDistrictKey(string key){
	        OpenConnection();
			string sqlQuery="select path from districts where districts.key = '"+key+"'";
			queryExecuter= conn.CreateCommand();
			queryExecuter.CommandText = sqlQuery;					
			dr= queryExecuter.ExecuteReader();
			
			string path = "";
			
			while(dr.Read()){
				path = (string)dr.GetString(0);	
			}
			
			dr.Close();
			CloseConnection();
			return path;
			
		}


        public void updateSourceHlForObjectTracking(UInt16 sourceDistrict, UInt16 sourceHl, UInt32 lastObjectId)
        {
	        OpenConnection();
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
            CloseConnection();

            if (objectId == 0 || objectId != lastObjectId)
            {
	            ExecuteNonResultQuery("UPDATE data_hardlines SET objectId = '" + lastObjectId.ToString() +
	                                  "' WHERE id = '" + theId.ToString() + "' LIMIT 1");
				#if DEBUG
				Output.WriteLine("[WORLD DB] UPDATE Hardline " + hardlineId.ToString() + " in District " + districtId.ToString() + " with Object ID : "+lastObjectId.ToString());
				#endif
			}
	        
            
        }

        public void updateLocationByHL(UInt16 district, UInt16 hardline)
        {
	        OpenConnection();
            string sqlQuery = "SELECT DH.X,DH.Y,DH.Z,DH.ROT,DIS.key,DH.DistrictId FROM data_hardlines AS DH, districts as DIS WHERE DH.DistrictId = '" + district.ToString() + "' AND DH.HardLineId = '" + hardline.ToString() + "' AND DH.DistrictId=DIS.id ";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            dr = queryExecuter.ExecuteReader();

            while (dr.Read()){
                double x = (double)(dr.GetFloat(0));
                double y = (double)(dr.GetFloat(1));
                double z = (double)(dr.GetFloat(2));
                string disKey = dr.GetString(4);
                Store.currentClient.playerData.setDistrict(disKey);
                Store.currentClient.playerData.setDistrictId((uint)dr.GetInt16(5));
                Store.currentClient.playerInstance.Position.setValue(NumericalUtils.doublesToLtVector3d(x, y, z));
                //Store.currentClient.playerInstance.YawInterval.setValue((byte)dr.GetDecimal(3));
            }
            dr.Close();
	        CloseConnection();
            SavePlayer(Store.currentClient);
        }

        public void setBackground(string backgroundText)
        {
	        UInt32 charID = Store.currentClient.playerData.getCharID();

            ExecuteNonResultQuery("UPDATE characters SET background = '" + backgroundText + "' WHERE charId = '" +
                                  charID + "' LIMIT 1");
            
        }
		
		public void setPlayerValues()
		{
			OpenConnection();
            UInt32 charID = Store.currentClient.playerData.getCharID();
			string sqlQuery="Select handle,x,y,z,rotation,healthC,healthM,innerStrC,innerStrM,level,profession,alignment,pvpflag,firstName,lastName,exp,cash,district,districtId,factionId,crewId from characters where charId='"+charID+"'";
			queryExecuter= conn.CreateCommand();
			queryExecuter.CommandText = sqlQuery;					
			dr= queryExecuter.ExecuteReader();
			
			while(dr.Read()){
				
				Store.currentClient.playerInstance.CharacterID.setValue(charID);
				Store.currentClient.playerInstance.CharacterName.setValue(dr.GetString(0));

				double x = dr.GetDouble(1);
				double y = dr.GetDouble(2);
				double z = dr.GetDouble(3);

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
			CloseConnection();
			
		}

		public UInt32 getCharIdByHandle(string handle)
		{
			OpenConnection();
			string sqlQuery = "SELECT charId FROM characters WHERE handle = '"+handle.Trim()+"' LIMIT 1";
			queryExecuter = conn.CreateCommand();
			queryExecuter.CommandText = sqlQuery;

			dr = queryExecuter.ExecuteReader();

			UInt32 charId = 0;
			while (dr.Read())
			{
				charId = (UInt32) dr.GetInt32(0);
			}
	        
			dr.Close();
			CloseConnection();

			return charId;
		}
		
        public Hashtable getCharInfo(UInt32 charId)
        {
	        OpenConnection();
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
	        CloseConnection();

            return data;
        }
        
        public Hashtable getCharInfoByHandle(string handle)
        {
	        OpenConnection();
	        handle = handle.Substring(0, handle.Length - 1);
	        string sqlQuery = "SELECT c.charId, c.firstName, c.lastName, c.background, c.alignment, c.conquest_points, f.name as faction_name, fc.crew_name as crew_name FROM characters c LEFT JOIN crews fc ON c.crewId=fc.id LEFT JOIN factions f ON c.factionId=f.id WHERE c.handle = '"+handle.Trim()+"' LIMIT 1";
	        queryExecuter = conn.CreateCommand();
	        queryExecuter.CommandText = sqlQuery;

	        dr = queryExecuter.ExecuteReader();

	        Hashtable data = new Hashtable();
	        
	        while (dr.Read())
	        {
		        data.Add("charId", (UInt32)dr.GetInt32(0));
		        data.Add("firstname", dr.GetString(1));
		        data.Add("lastname", dr.GetString(2));
		        data.Add("background", dr.GetString(3));
		        data.Add("alignment",(ushort)dr.GetInt16(4));
		        data.Add("conquest_points", (UInt32)dr.GetInt32(5));
		        if (!dr.IsDBNull(6))
		        {
			        data.Add("faction_name", dr.GetString(6));    
		        }
		        
		        if (!dr.IsDBNull(7))
		        {
			        data.Add("crew_name", dr.GetString(7));    
		        }

		        data.Add("handle", handle);
	        }
	        
	        dr.Close();
	        CloseConnection();

	        return data;
        }

		public void setRsiValues(){
			OpenConnection();
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
			CloseConnection();
		}

		public void SetOnlineStatus(uint charId, ushort isOnline)
		{
			ExecuteNonResultQuery("UPDATE characters SET is_online = '" + isOnline + "' WHERE charid= '" + charId +
			                      "' LIMIT 1");
			
		}

		public void ResetOnlineStatus()
		{
			ExecuteNonResultQuery("UPDATE characters SET is_online = '0' ");
		}


		public void UpdateRsiPartValue(string part, uint value, UInt32 charId)
        {
	        ExecuteNonResultQuery("UPDATE rsivalues SET " + part + "=" + value + " WHERE charid= '" + charId + "' LIMIT 1");
            
        }
		
		public void SavePlayer(WorldClient client){
			System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            UInt32 charID = NumericalUtils.ByteArrayToUint32(client.playerInstance.CharacterID.getValue(),1);
			string handle = StringUtils.charBytesToString_NZ(client.playerInstance.CharacterName.getValue());
			
			int [] rsiValues = client.playerData.getRsiValues();
			
			double x=0;double y=0;double z=0;
			byte[] Ltvector3d = client.playerInstance.Position.getValue();
			NumericalUtils.LtVector3dToDoubles(Ltvector3d,ref x,ref y,ref z);
			
			int rotation =client.playerInstance.YawInterval.getValue()[0];

			string positionQuery = "update characters set x =" + x + " ,y=" + y + " ,z=" + z + " , rotation=" +
			                       rotation + ", districtId=" + client.playerData.getDistrictId() + " where handle='" +
			                       handle + "' ";
			ExecuteNonResultQuery(positionQuery);
			Output.WriteLine(StringUtils.bytesToString(StringUtils.stringToBytes(positionQuery)));

			string rsiQuery = "update rsivalues set sex='" + rsiValues[0] + "',body='" + rsiValues[1] + "',hat='" +
			                  rsiValues[2] + "',face='" + rsiValues[3] + "',shirt='" + rsiValues[4] + "',coat='" +
			                  rsiValues[5] + "',pants='" + rsiValues[6] + "',shoes='" + rsiValues[7] +
			                  "',gloves='" + rsiValues[8] + "',glasses='" + rsiValues[9] + "',hair='" +
			                  rsiValues[10] + "',facialdetail='" + rsiValues[11] + "',shirtcolor='" +
			                  rsiValues[12] + "',pantscolor='" + rsiValues[13] + "',coatcolor='" + rsiValues[14] +
			                  "',shoecolor='" + rsiValues[15] + "',glassescolor='" + rsiValues[16] +
			                  "',haircolor='" + rsiValues[17] + "',skintone='" + rsiValues[18] + "',tattoo='" +
			                  rsiValues[19] + "',facialdetailcolor='" + rsiValues[20] + "',leggins='" +
			                  rsiValues[21] + "' where charId='" + charID + "';";
            ExecuteNonResultQuery(rsiQuery);
		
            Output.writeToLogForConsole("[WORLD DB ACCESS ]" + rsiQuery);
        
			
		}


        public void UpdateInventorySlot(UInt16 sourceSlot, UInt16 destSlot, UInt32 charId)
        {
	        ExecuteNonResultQuery("UPDATE inventory SET slot = '" + destSlot.ToString() + "' WHERE slot = '" + sourceSlot.ToString() + "' AND charID = '" + charId + "' LIMIT 1");
        }

        public bool isSlotinUseByItem(UInt16 slotId)
        {
	        OpenConnection();
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
	        CloseConnection();
            return isSlotInUse;

        }

        public UInt16 GetCrewMemberCountByCrewName(string crewName)
        {
	        OpenConnection();
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
	        CloseConnection();
            return countCrewMembers;
        }

	    public UInt32 GetCrewIdByCrewMasterHandle(string playerHandle)
	    {
		    // ToDo: we need to proove if this can work this way 
		    // ToDo: can only the master invite other players ? i am not sure then we need to change this
		    // ToDo: make DB easier
		    OpenConnection();
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
		    CloseConnection();
		    return countCrewMembers;
	    }

	    public uint GetCrewIdByInviterHandle(string playerHandle)
	    {
		    UInt32 crewId = 0;
		    OpenConnection();
		    string sqlQuery = "SELECT crewId FROM characters WHERE handle ='" + playerHandle + "' LIMIT 1";
		    queryExecuter = conn.CreateCommand();
		    queryExecuter.CommandText = sqlQuery;
		    dr = queryExecuter.ExecuteReader();

		    string factionName = "";

		    if (dr.Read())
		    {

			    crewId = (UInt32) dr.GetInt32(0);

		    }
		    dr.Close();
		    CloseConnection();
		    return crewId;
	    }

	    public string GetFactionNameById(uint factionId)
	    {
		    OpenConnection();
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
		    CloseConnection();
		    return factionName;
	    }

	    public bool IsCrewNameAvailable(string crewName)
        {
	        OpenConnection();
            bool isCrewNameAvailable = true;
	        crewName = crewName.Replace("'", @"\'");

            string sqlQuery = "SELECT id,created_at,deleted_at FROM crews WHERE crew_name='" + crewName + "' AND deleted_at > NOW() LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            dr = queryExecuter.ExecuteReader();

            if (dr.Read())
            {
                UInt16 memberCount = GetCrewMemberCountByCrewName(crewName);

                UInt32 crewId = (UInt32)dr.GetInt32(0);
                DateTime createdAt = dr.GetDateTime(1);
                DateTime deletedAt = dr.GetDateTime(2);

                isCrewNameAvailable = false;

	            // Lets check if more than one player is in the crew and if created_at is greater than one day
                if ((DateTime.Now - deletedAt).TotalHours > 24 && memberCount < 2)
                {
                    DeleteCrew(crewId);
	                isCrewNameAvailable = true;
                }


            }

            dr.Close();
	        CloseConnection();
            return isCrewNameAvailable;
        }

        public void DeleteCrew(UInt32 crewId)
        {
	        // Delete the Crew
            ExecuteNonResultQuery("UPDATE crews SET deleted_at = NOW() WHERE id=" + crewId + " LIMIT 1");
            
	        // Delete the Player Crew ID
	        ExecuteNonResultQuery("UPDATE characters SET crewId = 0 WHERE crewId = " + crewId);
	        
	        // Delete the Player Crew ID
	        ExecuteNonResultQuery("DELETE FROM crew_members WHERE crew_id = " + crewId);
        }

        public void AddCrew(string crewName, string masterHandle)
        {
	        crewName = crewName.Replace("'", @"\'");
            ExecuteNonResultQuery("INSERT INTO crews SET crew_name= '" + crewName + "', created_at =NOW() ");
        }

        public UInt16 GetFirstNewSlot()
        {
	        OpenConnection();
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
	        CloseConnection();
            return freeSlot;
        }

        public void addItemToInventory (UInt16 slotId, UInt32 itemGoID)
        {
	        UInt32 charID = Store.currentClient.playerData.getCharID();
            ExecuteNonResultQuery("INSERT INTO inventory SET charId = '" + charID.ToString() + "' , goid = '" +
                                  itemGoID + "', slot = '" + slotId.ToString() + "', created = NOW() ");
        }

        public UInt32 GetItemGOIDAtInventorySlot(UInt16 slotId)
        {
	        OpenConnection();
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
	        CloseConnection();
            return GoID;
        }


	    public void SaveInfo(WorldClient client, long cash)
	    {
		    ExecuteNonResultQuery("UPDATE characters SET cash =" + cash + " WHERE charId= " +
		                          client.playerData.getCharID().ToString() + " LIMIT 1");
		    
	    }

	    public void UpdateAbilityLoadOut(List<UInt16> abilitySlots, uint loaded)
        {
	        UInt32 charID = Store.currentClient.playerData.getCharID();
            string sqlQuery = "";
            foreach(ushort slot in abilitySlots)
            {
                sqlQuery += "UPDATE char_abilities SET is_loaded = " + loaded.ToString() + " WHERE char_id = " + charID.ToString() + " AND slot = " + slot.ToString() + ";";
            }

			ExecuteNonResultQuery(sqlQuery);
        }

        public void SaveExperience(WorldClient client, long exp)
        {
	        ExecuteNonResultQuery("UPDATE characters SET exp =" + exp + " WHERE charId= " + client.playerData.getCharID().ToString() + " LIMIT 1");
        }
    }
}

