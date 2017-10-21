using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace hds
{

	/* This inner class is a calling to C++ methods for faster handling of encryption */
	class EnigmaLib 

	{
		[DllImport("enigmaLib.dll", EntryPoint="worldEncrypt")] 
		public static extern int worldEncrypt(byte[] plain, byte[] buffer,int length, UInt16 pss, UInt16 cseq,	UInt16 sseq);
		
		[DllImport("enigmaLib.dll", EntryPoint="worldDecrypt")] 
		public static extern int worldDecrypt(byte[] cryptedData,byte[] plainText, int length,ref UInt16 pss, ref UInt16 cseq,	ref UInt16 sseq);
	}
	
	
	public class WorldEncryptionEnigma : IWorldEncryption
	{
		
		private byte[] innerBuffer;		
		private int lastOperationResultSize;
		
		public WorldEncryptionEnigma (){
			innerBuffer = new byte [2048];
		}
		

		// Asuring just inner buffer usage
		public byte[] encrypt(byte[] plainData, int length, UInt16 pss, UInt16 cseq, UInt16 sseq ){

			this.lastOperationResultSize = 	EnigmaLib.worldEncrypt(plainData,innerBuffer,length,pss,cseq,sseq);
			byte[] data = new byte[this.lastOperationResultSize];
			ArrayUtils.copy(innerBuffer,0,data,0,this.lastOperationResultSize);
			return data;
		}
		
		// Return all the values in an ArrayList, not just the decrypted packet
		public ArrayList decrypt(byte[] encryptedData, int length){
			ArrayList response = new ArrayList();
			
			UInt16 pss=0;
			UInt16 cseq=0;
			UInt16 sseq=0;
			
			int decryptedSize = EnigmaLib.worldDecrypt(encryptedData,innerBuffer,length,ref pss,ref cseq,ref sseq);
            
			/*
			 * If decrypted size is -1337 then it was a CRC in packet <> CRC of packet Exception
			 * */
			
			if (decryptedSize==-1337){
				Output.WriteLine("Oh oh, CRC didnt matched in this packet");
			}
			
            // This hexsize throws exceptions ...so we will use the decryptedSize - 10 (same value)
			byte[] hexSize = new byte[2];
			hexSize [0] = innerBuffer[4];
			hexSize [1] = innerBuffer[5];
			UInt16 size = NumericalUtils.ByteArrayToUint16(hexSize,1);
			
			size-=4; // remove the 4 seq data bytes from size
			
			byte[] data = new byte[size];
			ArrayUtils.copy(innerBuffer,14,data,0,decryptedSize-10); // Should do a fast copy method for this, if not done already
			response.Add(data);
			response.Add(pss);
			response.Add(cseq);
			response.Add(sseq);
			
			return response;
		}
	}
}

