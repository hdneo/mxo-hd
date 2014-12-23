
using System;

namespace hds
{
	public class Crc32
	{
		ulong [] table;

		public Crc32(){
			this.table = new ulong[256];
			generateTable();
		}
		
		public byte[] checksumB(byte[]block, int reversed){
			UInt32 temp = checksum(block);
			return NumericalUtils.uint32ToByteArray(temp,reversed);
		}
		
		public UInt32 checksum(byte[] block){
			ulong crc;
   			int i;
			int length = block.Length;

   			crc = 0xFFFFFFFF;
   			for (i = 0; i < length; i++){
      			crc = ((crc >> 8) & 0x00FFFFFF) ^ table[(crc ^ block[i]) & 0xFF];
   			}
   			return (UInt32)(crc ^ 0xFFFFFFFF);
		}
		
		public void generateTable(){
			ulong crc, poly;
   			int i, j;
			
   			poly = 0xEDB88320L;
   			for (i = 0; i < 256; i++){
      			crc = (ulong)i;
      			for (j = 8; j > 0; j--){
	 				if ((crc & 1)==1){
	    				crc = (crc >> 1) ^ poly;
					}
					else{
	    				crc >>= 1;
	 				}
      			}
      			table[i] = crc;
   			}
		}
	}
}
