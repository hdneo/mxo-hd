using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace hds
{
	public class WorldDbAccess
	{
		IDbConnection conn;
		IDbCommand queryExecuter;
		IDataReader dr;
		private XmlParser xmlParser;
		
		public WorldDbAccess()
		{
			xmlParser = new XmlParser();
			string [] dbaParams = xmlParser.loadDBParams("Config.xml");
			
			/* Params: Host, port, database, user, password */
			conn = new MySqlConnection("Server="+dbaParams[0]+";" + "Database="+dbaParams[2]+";" +"User ID="+dbaParams[3]+";" + "Password="+dbaParams[4]+";" + "Pooling=false;");
			conn.Open();
		}
		
		public UInt32 getUserIdForCharId(byte[] charIdHex){
			NumericalUtils nu = new NumericalUtils();
			UInt32 charId = nu.ByteArrayToUint32(charIdHex,1);
			Console.WriteLine("Check charid:"+charId);
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
		
		public void setPlayerValues(ref ClientData data, UInt16 charID){
			string sqlQuery="Select handle,x,y,z,rotation,healthC,healthM,innerStrC,innerStrM,level,profession,alignment,pvpflag,firstName,lastName,exp,cash,district from characters where charId='"+charID+"'";
			queryExecuter= conn.CreateCommand();
			queryExecuter.CommandText = sqlQuery;					
			dr= queryExecuter.ExecuteReader();
			while(dr.Read()){
				data.setPlayerValue("handle",dr.GetString(0));	
				data.setPlayerValue("x",(float)dr.GetFloat(1));	
				data.setPlayerValue("y",(float)dr.GetFloat(2));	
				data.setPlayerValue("z",(float)dr.GetFloat(3));	
            	data.setPlayerValue("rotation",(int)dr.GetDecimal(4));	
            	data.setPlayerValue("healthC",(int)dr.GetDecimal(5));	
				data.setPlayerValue("healthM",(int)dr.GetDecimal(6));	
				data.setPlayerValue("innerStrC",(int)dr.GetDecimal(7));	
				data.setPlayerValue("innerStrM",(int)dr.GetDecimal(8));	
   				data.setPlayerValue("level",(int)dr.GetDecimal(9));
				data.setPlayerValue("profession",(int)dr.GetDecimal(10));
				data.setPlayerValue("alignment",(int)dr.GetDecimal(11));
				data.setPlayerValue("pvpFlag",(int)dr.GetDecimal(12));
				data.setPlayerValue("firstName",dr.GetString(13));
				data.setPlayerValue("lastName",dr.GetString(14));
				data.setPlayerValue("exp",(long)dr.GetDecimal(15));
				data.setPlayerValue("cash",(long)dr.GetDecimal(16));
				data.setPlayerValue("district",dr.GetString(17));
			}
			
			dr.Close();
			
		}
		

		public void setRsiValues(ref ClientData cData,UInt16 charID){
			
			string sqlQuery="Select sex,body,hat,face,shirt,coat,pants,shoes,gloves,glasses,hair,facialdetail,shirtcolor,pantscolor,coatcolor,shoecolor,glassescolor,haircolor,skintone,tatto,facialdetailcolor,leggins from rsivalues where charId='"+charID+"'";
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
			
			
			cData.setRsiValues(rsiValues);
			dr.Close();
			
		}
		
		public bool fetchWordList(WorldList wl){
			
			
			
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
				Console.WriteLine("Player not found on DB with #{0}# and #{1}#",wl.getUsername(),wl.getPassword());
				conn.Close();
				return false;
			}
			return true;
			
		}
	}
}

