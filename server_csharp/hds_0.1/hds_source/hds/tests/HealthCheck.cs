using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace hds
{
	
	
	public class HealthCheck
	{
		
		StringUtils su;
		ArrayUtils au;
		
		public HealthCheck()
		{
			su = new StringUtils();
			au = new ArrayUtils();
		}
		
		private int xmlParserCheck(){
			
			try{
				XmlParser xmlParser = new XmlParser();
				string [] dbaParams = xmlParser.loadDBParams("Config.xml");
				dbaParams[0]="";
							
				Console.Write("OK\n");
				return 1;
			}
			catch(Exception){
				Console.Write("Failed\n");
				return 0;
			}
			
		}
		
		private int mysqlCheck(){
			try{
				XmlParser xmlParser = new XmlParser();
				string [] dbaParams = xmlParser.loadDBParams("Config.xml");
			
				/* Params: Host, port, database, user, password */
				IDbConnection conn = new MySqlConnection("Server="+dbaParams[0]+";" + "Database="+dbaParams[2]+";" +"User ID="+dbaParams[3]+";" + "Password="+dbaParams[4]+";" + "Pooling=false;");
				conn.Open();
				IDbCommand queryExecuter;
				IDataReader dr;
				
				queryExecuter= conn.CreateCommand();
				string sqlQueryForWorlds = "select * from worlds";
				queryExecuter.CommandText = sqlQueryForWorlds;					
				dr= queryExecuter.ExecuteReader();
				
				
				
				Console.Write("OK\n");
				return 1;
			}
			catch(Exception e){
				Console.Write("Failed. Debug error:{0}\n",e.Message);
				return 0;
			}
		}
		
		private int md5Check(){
			Md5 hasher = new Md5();
			byte[] knownResult = {0x5b,0x79,0x3c,0x41,0xd0,0xf5,0x6b,0xa2,0x2f,0x8b,0x84,0x3b,0x32,0x89,0xc1,0x32};
			string testString = "yummy!";
			byte[] testArray = su.stringToBytes(testString);
			byte[] md5 = hasher.digest(testArray);
			
			if (!au.equal(knownResult,md5)){
				Console.Write("Failed\n");
				return 0;
			}
			Console.Write("OK\n");
			return 1;
			
		}
		
		private int crcCheck(){
			Crc32 crc32 = new Crc32();
			byte[] knownResult={0x0d,0x1e,0xe7,0xea};
			string testString = "this is a test";
			byte[] testArray = su.stringToBytes(testString);
			// Test is done with a non reversed array
			byte [] result = crc32.checksumB(testArray,0);
			
			if (!au.equal(knownResult,result)){
				Console.Write("Failed\n");
				return 0;
			}
			Console.Write("OK\n");
			return 1;
		}
		
		private int rsaDecryptCheck(){
			MxoRSA rsa = new MxoRSA();
			// 128 bytes as blob, preExistent
			byte[] blob={0x53,0x18,0x02,0x10,0x4f,0x69,0xba,0x41,0x57,0x80,0x27,0xa3,0x87,0x88,0xb4,0x5f,0xc8,0xd0,0xad,0x39,0x9e,0x7d,0x92,0x57,0x4e,0x94,0x48,0x95,0x3c,0x66,0x34,0x26,0x34,0xb5,0x94,0xa4,0xa1,0xd0,0x0b,0xac,0xb1,0x0b,0xc3,0x23,0xb1,0x98,0xa2,0xc8,0x21,0xd8,0x5f,0x83,0xda,0x00,0x63,0x6f,0x99,0xeb,0x2b,0x34,0x77,0x2a,0x44,0x57,0x13,0xd9,0x0e,0x00,0x4c,0x71,0x07,0x6a,0x9b,0xb3,0xac,0x0c,0x7f,0x9b,0xfe,0x28,0xe4,0x8e,0x7e,0xa6,0xac,0x67,0x7f,0xb6,0xc2,0xba,0xeb,0x95,0x70,0x16,0x09,0x23,0x63,0xc5,0x1e,0x8d,0xea,0xed,0x90,0x8e,0xc7,0xfc,0x3e,0xdb,0x14,0xc0,0xa6,0xab,0x8a,0xc3,0x45,0xbd,0x98,0x07,0x7a,0x15,0x7e,0xc5,0x64,0x45,0xea,0xaa,0x19,0xf6};
			byte[] knownResult={0x00,0x04,0x00,0x00,0x00,0x1b,0x00,0x44,0x2c,0x3c,0x13,0x3e,0xb0,0xbf,0x66,0x81,0x5c,0x2b,0x5f,0x43,0x11,0x38,0xe1,0x27,0xe0,0x0e,0x4c,0x08,0x00,0x6c,0x6f,0x6c,0x75,0x73,0x65,0x72,0x00};
			byte[] result = rsa.decryptWithPrivkey(blob);
			
			if (!au.equal(knownResult,result)){
				Console.Write("Failed\n");
				return 0;
			}
			Console.Write("OK\n");
			return 1;
		}
		
		private int twofishEncrypt(){
			byte[] key = { 0x6C, 0xAB, 0x8E, 0xCC, 0xE7, 0x3C, 0x22, 0x47, 0xDB, 0xEB, 0xDE, 0x1A, 0xA8, 0xE7, 0x5F, 0xB8 };
			byte[] iv = { 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30 };
			byte[] plainText = {0x00,0x01,0x02,0x03,0x04,0x05,0x06,0x07, 0x08 ,0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f};
			byte[] knownResult = {0x77,0x7f,0x54,0xc6,0xc9,0x9e,0x35,0x56,0x26,0x4b,0xc0,0x17,0x96,0x33,0x79,0xe8};
			byte[] result = new byte[16];
			MxoTwofish tf = new MxoTwofish();
			tf.setIV(iv);
			tf.setKey(key);
			tf.encrypt(plainText,result);
		
			if (!au.equal(knownResult,result)){
				Console.Write("Failed\n");
				Console.WriteLine(su.bytesToString(knownResult));
				Console.WriteLine(su.bytesToString(result));
				return 0;
			}
			Console.Write("OK\n");
			return 1;
		}
		
		private int twofishDecrypt(){
			byte[] key = { 0x6C, 0xAB, 0x8E, 0xCC, 0xE7, 0x3C, 0x22, 0x47, 0xDB, 0xEB, 0xDE, 0x1A, 0xA8, 0xE7, 0x5F, 0xB8 };
			byte[] iv = { 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30 };
			byte[] cipherText = {0x77,0x7f,0x54,0xc6,0xc9,0x9e,0x35,0x56,0x26,0x4b,0xc0,0x17,0x96,0x33,0x79,0xe8};
			
			byte[] knownResult = {0x00,0x01,0x02,0x03,0x04,0x05,0x06,0x07, 0x08 ,0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f};
			byte[] result = new byte[16];
			
			MxoTwofish tf = new MxoTwofish();
			tf.setIV(iv);
			tf.setKey(key);
			tf.decrypt(cipherText,result);
			
			if (!au.equal(knownResult,result)){
				Console.Write("Failed\n");
				return 0;
			}
			Console.Write("OK\n");
			return 1;
		}
		
		public int enigmaLibEncryption(){
			try{
				WorldEncryption we = new WorldEncryption();
				byte[] p = new byte[16];
				we.encrypt(p,16,15,15,15);
			
			}
			catch(Exception e){
				Console.Write("Failed\n");
				Console.WriteLine("Debug: "+e);
				return 0;
			}
			Console.Write("OK\n");
			return 1;
		}
		
		
		public bool doTests(){
			int totalPoints = 0;
			int totalTests = 8;
			
			Console.WriteLine("########################");
			Console.WriteLine("HEALTH CHECKS _ STARTING");
			Console.WriteLine("########################\n");
			
			Console.Write("Md5 check ... ");
			totalPoints+= md5Check();
			
			Console.Write("CRC32 check ... ");
			totalPoints+= crcCheck();
			
			Console.Write("RSA (Decryption) check ... ");
			totalPoints+= rsaDecryptCheck();
			
			Console.Write("Twofish (Encryption) check ... ");
			totalPoints+= twofishEncrypt();
			
			Console.Write("Twofish (Decryption) check ... ");
			totalPoints+= twofishDecrypt();
			
			Console.Write("Xml Parser And config file check ... ");
			totalPoints+= xmlParserCheck();
			
			Console.Write("Mysql DB connection check ... ");
			totalPoints+= mysqlCheck();
			
			Console.Write("Enigmalib encryption check ... ");
			totalPoints+= enigmaLibEncryption();
			
			if (totalPoints!=totalTests)
				return false;
			return true;
		}
	}
}
