
using System;
using System.Security.Cryptography;

namespace hds
{
	
	public class Md5
	{
		
		MD5CryptoServiceProvider hasher;
		
		public Md5(){
			hasher = new MD5CryptoServiceProvider();
		}
		
		public byte[] digest(byte[] data){
			byte [] result = hasher.ComputeHash(data);
			return result;
		}
	}
}
