
using System;

namespace hds
{
	
	public class ArrayUtils
	{
		
		public ArrayUtils()	{}
		
		// Reverse it on memory
		// Much faster than Array.reverse method
		public void reverse(byte[] param){
			byte temp;
			int i=0;
			int j=param.Length-1;
			
			while (j>i){
				temp = param[i];
				param[i] = param[j];
				param[j] = temp;
				i++;
				j--;
			}
		}
		
		// Compares two arrays of same length
		public static bool equal(byte[] arrayA,byte[] arrayB){
			if(arrayA.Length!=arrayB.Length)
				return false;
			for(int i = 0;i<arrayA.Length;i++){
				if(arrayA[i]!=arrayB[i])
					return false;
			}
			return true;
		}
		
		// Copies N bytes from source offset to destination array offset
		static public void copy(byte[] sourceArray, int offsetB,byte[] destinationArray,int offsetA, int nBytes){
			for (int i = 0;i<nBytes;i++){
				if(offsetA+i < destinationArray.Length && offsetB+i < sourceArray.Length)
					destinationArray[offsetA+i] = sourceArray[offsetB+i];
				else
					break; // We keep limits safe if > length
			}	
		}

        // Copies N bytes from source offset to destination array offset
        public static void copyTo(byte[] sourceArray, int offsetB, byte[] destinationArray, int offsetA, int nBytes)
        {
            for (int i = 0; i < nBytes; i++)
            {
                if (offsetA + i < destinationArray.Length)
                    destinationArray[offsetA + i] = sourceArray[offsetB + i];
                else
                    break; // We keep limits safe if > length
            }
        }
		
		
		/* Does a fast copy of array contents */
		public static void fastCopy(byte[] source, byte[]destination, int length){
			int pointer = 0;
			
			// Copy 4 bytes to 4 bytes
			for (int i = 0;(i+4)<length;i+=4){
				destination[i] = source[i];
				destination[i+1] = source[i+1];
				destination[i+2] = source[i+2];
				destination[i+3] = source[i+3];
				pointer+=4;
			}
			
			// Copy the rest, if any
			if (pointer!=length){
				for (int i = pointer;i<length;i++){
					destination[i] = source[i];
				}
			}
			
		}
	
		
		
	}
}
