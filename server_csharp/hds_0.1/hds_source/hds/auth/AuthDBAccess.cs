
using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace hds
{

	public class AuthDBAccess
	{
		
		IDbConnection conn;
		IDbCommand queryExecuter;
		IDataReader dr;
		private XmlParser xmlParser;
		
		public AuthDBAccess()
		{
			xmlParser = new XmlParser();
			string [] dbaParams = xmlParser.loadDBParams("Config.xml");
			
			/* Params: Host, port, database, user, password */
			conn = new MySqlConnection("Server="+dbaParams[0]+";" + "Database="+dbaParams[2]+";" +"User ID="+dbaParams[3]+";" + "Password="+dbaParams[4]+";" + "Pooling=false;");
			
		}
		
		public bool fetchWordList(WorldList wl){
			
			// Doesnt exist by default
			wl.setExistance(false);
			
			conn.Open();
			
			string sqlQuery = "select * from users where username='"+wl.getUsername()+"' and passwordmd5='"+wl.getPassword()+"';";
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
				Console.WriteLine("Player not found on DB with #{0}# and #{1}#",wl.getUsername(),wl.getPassword());
				conn.Close();
				return false;
			}
			
			// If exist, get the player values 
			
			
			// Count the values first
			
			int totalChars = 0;			
			string sqlCount = "select charId from characters where userId="+wl.getUserID()+"";
			queryExecuter= conn.CreateCommand();
			queryExecuter.CommandText = sqlCount;
			dr= queryExecuter.ExecuteReader();
			
			while (dr.Read()){
				totalChars++;
			}
			
			dr.Close();
			wl.getCharPack().setTotalChars(totalChars);
			
			// Prepare to read characters
			
			string sqlQueryForChars = "select * from characters where userId="+wl.getUserID()+"";
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
			string sqlQueryForWorlds = "select * from worlds";
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
