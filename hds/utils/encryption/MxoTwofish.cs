
using System;
using System.Security.Cryptography;
using ManyMonkeys.Cryptography;


namespace hds
{
	
	public class MxoTwofish
	{
		Twofish tf;
		ICryptoTransform tfEncryptor;
		ICryptoTransform tfDecryptor;
		
				
		public MxoTwofish(){
			tf = new Twofish();
			tf.Mode = CipherMode.CBC;
			tf.KeySize = 128;
			tf.BlockSize = 128;
			
		}
		
		public void setIV(byte[] iv){
			tf.IV = iv;
		}
		
		public void setKey(byte[] key){
			tf.Key = key;
		}
		
		public void encrypt(byte[] plainData, byte[] destination){
			tfEncryptor = tf.CreateEncryptor(tf.Key,tf.IV);
			for (int i = 0;i<plainData.Length/16;i++)
				tfEncryptor.TransformBlock(plainData,i*16,16,destination,i*16);
		}
		
		public void decrypt(byte[] criptedData, byte[] destination){
			tfDecryptor = tf.CreateDecryptor(tf.Key,tf.IV);
			for (int i = 0;i<criptedData.Length/16;i++)
				tfDecryptor.TransformBlock(criptedData,i*16,16,destination,i*16);
		}
		
		
	}
}
