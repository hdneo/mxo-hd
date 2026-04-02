using System;
namespace hds
{
	public class BufferHandler
	{
		public BufferHandler ()
		{}

		static public byte readByte(ref byte[] data,ref int offset){
            // This causes sometimes an error ...
            byte t = data[offset];
			offset++;
			return t;
		}
		
		static public byte[] readBytes(ref byte[] data,ref int offset,int bytesNum){
			if (bytesNum>data.Length || (offset+bytesNum>data.Length)){
				return null; // You cannot read further than the limit
			}else{
				byte[] readValue = new byte[bytesNum];
				ArrayUtils.copy(data,offset,readValue,0,bytesNum);
				offset+=bytesNum;
				return readValue;
			}
		}
		
	}
}

