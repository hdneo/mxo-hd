using System;
using System.Data;
using MySql.Data.MySqlClient;

using hds.databases.interfaces;
using hds.shared;

namespace hds.databases{

	public class MyAuthDBAccess: IAuthDBHandler {
		
		private IDbConnection conn;
		private IDbCommand queryExecuter;
		private IDataReader dr;
		
		public MyAuthDBAccess(){

            var config = Store.config;
			
			/* Params: Host, port, database, user, password */
            conn = new MySqlConnection("Server=" + config.dbParams.Host + ";" + "Database=" + config.dbParams.DatabaseName + ";" + "User ID=" + config.dbParams.Username + ";" + "Password=" + config.dbParams.Password + ";" + "Pooling=false;");
			
		}
		
		public bool fetchWordList(ref WorldList wl){

			// Doesnt exist by default
			wl.setExistance(false);
			
			conn.Open();
			
			string sqlQuery = "SELECT * FROM users WHERE username='"+wl.getUsername()+"' AND passwordmd5='"+wl.getPassword()+"' LIMIT 1;";
			queryExecuter= conn.CreateCommand();
			queryExecuter.CommandText = sqlQuery;					
			dr= queryExecuter.ExecuteReader();
				
			
			
			while(dr.Read()){
				// Player is on the DB
				wl.setExistance(true);
				wl.setUserID((int) dr.GetDecimal(0));
				dr.GetBytes(5,0,wl.getPublicModulus(),0,96);
				dr.GetBytes(6,0,wl.getPrivateExponent(),0,96);
				wl.setTimeCreated((int)dr.GetDecimal(7));
			}
			
			
			
			dr.Close();
			
			// If doesnt exist... should not do more things
			if (!wl.getExistance()){
				String msg = "Player not found on DB with #"+wl.getUsername()+"# and #"+wl.getPassword()+"#";
				Output.WriteLine(msg);
				conn.Close();
				return false;
			}
			
			// If exist, get the player values 
			
			
			// Count the values first
			
			int totalChars = 0;
            string sqlCount = "SELECT charId FROM characters WHERE userId='" + wl.getUserID() + "' AND is_deleted='0' ";
			queryExecuter= conn.CreateCommand();
			queryExecuter.CommandText = sqlCount;
			dr= queryExecuter.ExecuteReader();
			
			while (dr.Read()){
				totalChars++;
			}
			
			dr.Close();
			wl.getCharPack().setTotalChars(totalChars);
			
			// Prepare to read characters

            string sqlQueryForChars = "SELECT * FROM characters WHERE userId='" + wl.getUserID() + "' AND is_deleted='0' ";
			queryExecuter= conn.CreateCommand();
			queryExecuter.CommandText = sqlQueryForChars;
			dr= queryExecuter.ExecuteReader();
						
			// Read characters
			while(dr.Read()){
				
				//totalChars = (int) dr.GetDecimal(8);
				string charName = dr.GetString(4);
				int charId = (int)dr.GetDecimal(0);
				int status = (int)dr.GetDecimal(3);
				int worldId = (int)dr.GetDecimal(2);
				
				wl.getCharPack().addCharacter(charName,charId,status,worldId);

			}
			
			dr.Close();
			
			
			// Read worlds
			string sqlQueryForWorlds = "SELECT * FROM worlds ORDER BY worldId ASC";
			queryExecuter.CommandText = sqlQueryForWorlds;					
			dr= queryExecuter.ExecuteReader();
			
			while (dr.Read()){
				
				string worldName = dr.GetString(1);
				
				int worldId = (int) dr.GetDecimal(0);
				int worldType = (int) dr.GetDecimal(2);
				int worldStatus = (int) dr.GetDecimal(3);
				int worldPopulation = (int) dr.GetDecimal(4);
				wl.getWorldPack().addWorld(worldName,worldId,worldStatus,worldType,worldPopulation);
				
			}
			
			dr.Close();
			
			conn.Close();
			
			return true;
		}
	}
}
