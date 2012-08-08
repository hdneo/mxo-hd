
using System;

namespace hds
{
	
	
	public class NumericalUtils
	{
		
		public NumericalUtils()
		{
			
		}
		
		
		/* This does reverse the original array */
		public void reverse4ByteArray(byte[] res){
			byte temp;
			temp = res[0];
			res[0]=res[3];res[3] = temp;
			temp = res[1];
			res[1]=res[2];res[2] = temp;
		}
		
		/* This does reverse the original array */
		public void reverse2ByteArray(byte[] res){
			byte temp;
			temp = res[0];
			res[0]=res[1];
			res[1]=temp;
		}
		
		
		public byte[] uint32ToByteArray(UInt32 data, int reversed){
			byte [] res = BitConverter.GetBytes(data);
			
			if(reversed==1)
				return res;
			else
				reverse4ByteArray(res);
			
			return res;
		}
		
		public byte[] uint16ToByteArray(UInt16 data,int reversed){
			byte [] tempRes = BitConverter.GetBytes(data);
			byte [] res = new byte[2];
			
			res[0] = tempRes[0];
			res[1] = tempRes[1];
			
			if(reversed==1)
				return res;
			else
				reverse2ByteArray(res);
			
			return res;
		}
		
		public byte[] uint16ToByteArrayShort(UInt16 data){
			byte [] tempRes = BitConverter.GetBytes(data);
			byte [] res = new byte[1];
			
			res[0] = tempRes[0];
			
			return res;
		}
		
		public UInt16 ByteArrayToUint16(byte[] data, int reversed){
			
			UInt16 result;
			byte[] buffer = new byte[2];
			
			if(reversed==1){
				buffer[0] = data[1];
				buffer[1] = data[0];
			}
			else{
				buffer[0] = data[0];
				buffer[1] = data[1];
			}
			
			result = (UInt16) ((buffer[0] << 8) + buffer[1]);
			
			return result;
			
		}
		
		public UInt32 ByteArrayToUint32(byte[] data, int reversed){
			UInt32 result;
			byte[] buffer = new byte[4];
			
			if(reversed==1){
				buffer[0] = data[3];
				buffer[1] = data[2];
				buffer[2] = data[1];
				buffer[3] = data[0];
			}
			else{
				
				buffer[0] = data[0];
				buffer[1] = data[1];
				buffer[2] = data[2];
				buffer[3] = data[3];
			}
			
			result = (UInt32) ((buffer[0] << 24) + (buffer[1] << 16) + (buffer[2] << 8)
			+ buffer[3]);
			
			return result;
			
		}
		
		//It does reversed by default (Bitconverter)
		public byte[] floatToByteArray(float n,int reversed){
			
			byte[] buffer =  BitConverter.GetBytes(n);
			
			if(reversed==1)
				return buffer;
			else{
				byte[] rev = new byte[4];
				rev[0] = buffer[3];
				rev[1] = buffer[2];
				rev[2] = buffer[1];
				rev[3] = buffer[0];
				return rev;
			}
		}
		
		//It does reversed by default (Bitconverter)
		public byte[] doubleToByteArray(double n,int reversed){
			byte[] buffer =  BitConverter.GetBytes(n);
			
			if(reversed==1)
				return buffer;
			else{
				byte[] rev = new byte[8];
				rev[0] = buffer[7];
				rev[1] = buffer[6];
				rev[2] = buffer[5];
				rev[3] = buffer[4];
				rev[4] = buffer[3];
				rev[5] = buffer[2];
				rev[6] = buffer[1];
				rev[7] = buffer[0];
				return rev;
			}
		}
		
		
		// Returns N transformed to bits, taking bitsSize bits
		public string intToBitsString(int n,int bitsSize){
			string temp = Convert.ToString(n,2);
			if(temp.Length==bitsSize)
				return temp;
			else{
				if(temp.Length<bitsSize){
					temp = temp.PadLeft(bitsSize,'0');
					return temp;
				}
				else{
					//int index = 1;
					temp = temp.PadLeft(bitsSize);
					/*while(temp.Length>bitsSize){
						temp = temp.Substring(index);
						index++;
					}*/
					return temp;
				}
				
			}
		
		}
		
		public int binaryStringToInt(string binaryStr){
			return (int)Convert.ToInt32(binaryStr,2);
		}
		
		public byte[] bigBinaryStringToBytes(string bigBinaryStr){
			
			byte[] res = new byte[bigBinaryStr.Length/8];
			int pointer = 0;
			
			for(int i = 0;i<bigBinaryStr.Length;i+=8){
				string temp = bigBinaryStr.Substring(i,8);
				res[pointer]=(byte)binaryStringToInt(temp);
				pointer++;
			}
			
			return res;
		}

	}
}
