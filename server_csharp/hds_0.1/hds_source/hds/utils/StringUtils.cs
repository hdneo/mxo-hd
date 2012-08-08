
using System;
using System.Text;

namespace hds
{
	
	
	public class StringUtils
	{
		
		public StringUtils()
		{}
		
		// If we want just N bytes from the array
		public string bytesToString(byte[] data,int length){
			string ret = "";
			for (int i = 0; i <length; i++)
				ret += data[i].ToString("x2")+" ";
			return ret;
		}
		
		// If we want all bytes from the array
		public string bytesToString(byte[] data){
			string ret = "";
			for (int i = 0; i < data.Length; i++)
				ret += data[i].ToString("x2")+" ";
			return ret;
		}
		
		// If we want all bytes from the array but no spaces
		public string bytesToString_NS(byte[] data){
			string ret = "";
			for (int i = 0; i < data.Length; i++)
				ret += data[i].ToString("x2");
			return ret;
		}
		
		
		// If we want an array of characters (from bytes) to be a string
		public string charBytesToString(byte[] data){
			string ret = "";
			for (int i = 0; i < data.Length; i++)
				ret += (char)data[i];
			return ret;
		}
		
		public byte[] stringToBytes(string data){
			return Encoding.ASCII.GetBytes(data);
		}
		
		public byte[] hexStringToBytes(string hexString){
		
            int NumberChars = hexString.Length;

            byte[] bytes = new byte[NumberChars / 2];

            for (int i = 0; i < NumberChars; i += 2)
            {

            	bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);

            }

            return bytes;
			
	

		}
		
	}
}
