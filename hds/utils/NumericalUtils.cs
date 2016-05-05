
using System;
using System.Collections;

namespace hds
{
	
	
	public class NumericalUtils{
		
		
		/* This does reverse the original array */
		static public void reverse4ByteArray(byte[] res){
			byte temp;
			temp = res[0];
			res[0]=res[3];res[3] = temp;
			temp = res[1];
			res[1]=res[2];res[2] = temp;
		}
		
		/* This does reverse the original array */
		static public void reverse2ByteArray(byte[] res){
			byte temp;
			temp = res[0];
			res[0]=res[1];
			res[1]=temp;
		}
		
		
		static public byte[] uint32ToByteArray(UInt32 data, int reversed){
			byte [] res = BitConverter.GetBytes(data);
            
			
			if(reversed==1)
				return res;
			else
				reverse4ByteArray(res);
			
			return res;
		}

        static public byte[] int32ToByteArray(Int32 data, int reversed){

            byte[] res = BitConverter.GetBytes(data);

            if (reversed == 1)
                return res;
            else
                reverse4ByteArray(res);

            return res;
        }
		
		static public byte[] uint16ToByteArray(UInt16 data,int reversed){
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
		
		//TODO: change this code to uint8 to byte array
		static public byte[] uint16ToByteArrayShort(UInt16 data){
			byte [] tempRes = BitConverter.GetBytes(data);
			byte [] res = new byte[1];
			
			res[0] = tempRes[0];
			
			return res;
		}
		
		static public UInt16 ByteArrayToUint16(byte[] data, int reversed){
			
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
		
		
		static public UInt32 ByteArrayToUint32(byte[] data, int reversed){
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
		
		static public float byteArrayToFloat(byte[] array, int reversed){
			
			byte[] copy = new byte[4];
			
			if(reversed==1)
			{
				return BitConverter.ToSingle(array,0);
			}
			else{
				copy[0]=array[3];
				copy[1]=array[2];
				copy[2]=array[1];
				copy[3]=array[0];
				return BitConverter.ToSingle(copy,0);
			}
			
			
		}
		
		static public double byteArrayToDouble(byte[] array, int reversed){
			
			byte[] copy = new byte[8];
			
			if(reversed==1)
			{
				return BitConverter.ToDouble(array,0);
			}
			else{
				copy[0]=array[7];
				copy[1]=array[6];
				copy[2]=array[5];
				copy[3]=array[4];
				copy[4]=array[3];
				copy[5]=array[2];
				copy[6]=array[1];
				copy[7]=array[0];
				return BitConverter.ToDouble(copy,0);
			}
			
			
		}
		
		
		//It does reversed by default (Bitconverter)
		static public byte[] floatToByteArray(float n,int reversed){
			
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
		static public byte[] doubleToByteArray(double n,int reversed){
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
		static public string intToBitsString(int n,int bitsSize){
			string temp = Convert.ToString(n,2);
			if(temp.Length==bitsSize)
				return temp;
			else{
				if(temp.Length<bitsSize){
					temp = temp.PadLeft(bitsSize,'0');
					return temp;
				}
				else{
					temp = temp.PadLeft(bitsSize);
					return temp;
				}
				
			}
		
		}
		
		static public int binaryStringToInt(string binaryStr){
			return (int)Convert.ToInt32(binaryStr,2);
		}
		
		static public byte[] bigBinaryStringToBytes(string bigBinaryStr){
			
			byte[] res = new byte[bigBinaryStr.Length/8];
			int pointer = 0;
			
			for(int i = 0;i<bigBinaryStr.Length;i+=8){
				string temp = bigBinaryStr.Substring(i,8);
				res[pointer]=(byte)binaryStringToInt(temp);
				pointer++;
			}
			
			return res;
		}
		
		static public void LtVector3fToFloats(byte[] LtVector3f,ref float x,ref float y, ref float z){
			byte[] tempX = {0x00,0x00,0x00,0x00};
			byte[] tempY = {0x00,0x00,0x00,0x00};
			byte[] tempZ = {0x00,0x00,0x00,0x00};
			for(int i = 0;i<4;i++){
				tempX[i] = LtVector3f[i];
				tempY[i] = LtVector3f[i+4];
				tempZ[i] = LtVector3f[i+8];
			}
			x = NumericalUtils.byteArrayToFloat(tempX,1);
			y = NumericalUtils.byteArrayToFloat(tempY,1);
			z = NumericalUtils.byteArrayToFloat(tempZ,1);
		}
		
		static public void LtVector3dToDoubles(byte[] LtVector3d,ref double x,ref double y, ref double z){
			byte[] tempX = {0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00};
			byte[] tempY = {0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00};
			byte[] tempZ = {0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00};
			for(int i = 0;i<8;i++){
				tempX[i] = LtVector3d[i];
				tempY[i] = LtVector3d[i+8];
				tempZ[i] = LtVector3d[i+16];
			}
			x = NumericalUtils.byteArrayToDouble(tempX,1);
			y = NumericalUtils.byteArrayToDouble(tempY,1);
			z = NumericalUtils.byteArrayToDouble(tempZ,1);
		}
		
		static public byte[] floatsToLtVector3f(float x,float y,float z){
			DynamicArray din = new DynamicArray();
			din.append(NumericalUtils.floatToByteArray(x,1));
			din.append(NumericalUtils.floatToByteArray(y,1));
			din.append(NumericalUtils.floatToByteArray(z,1));
			return din.getBytes();
		}
		
		static public byte[] doublesToLtVector3d(double x,double y,double z){
			DynamicArray din = new DynamicArray();
			din.append(NumericalUtils.doubleToByteArray(x,1));
			din.append(NumericalUtils.doubleToByteArray(y,1));
			din.append(NumericalUtils.doubleToByteArray(z,1));
			return din.getBytes();
		}

        static public BitArray byteToBitArray(byte input)
        {
            BitArray flags = new BitArray(new byte[] { input });
            return flags;
        }

	}
}
