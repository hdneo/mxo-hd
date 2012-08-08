
using System;

namespace hds
{
	
	
	public class TimeUtils
	{
		NumericalUtils utils;
		
		public TimeUtils()
		{
			this.utils = new NumericalUtils();
		}
		
		public byte[] getUnixTime(){
			TimeSpan ts = (DateTime.UtcNow - new DateTime(1970,1,1,0,0,0));
			UInt32 unixTime = (UInt32)ts.TotalSeconds;
			byte[] result = utils.uint32ToByteArray(unixTime,1);
			return result;
		}
		
		public UInt32 getUnixTimeUint32(){
			TimeSpan ts = (DateTime.UtcNow - new DateTime(1970,1,1,0,0,0));
			UInt32 unixTime = (UInt32)ts.TotalSeconds;
			return unixTime;
		}
		
		public byte[] getUnixTime(int seconds){
			TimeSpan ts = (DateTime.UtcNow - new DateTime(1970,1,1,0,0,0));
			UInt32 unixTime = (UInt32)(ts.TotalSeconds+seconds);
			byte[] result = utils.uint32ToByteArray(unixTime,1);
			return result;
		}
	}
}
