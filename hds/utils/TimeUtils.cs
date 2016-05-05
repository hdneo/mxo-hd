
using System;

namespace hds{

	public class TimeUtils{
		
		public static byte[] getUnixTime(){
			TimeSpan ts = (DateTime.UtcNow - new DateTime(1970,1,1,0,0,0));
			UInt32 unixTime = (UInt32)ts.TotalSeconds;
			byte[] result = NumericalUtils.uint32ToByteArray(unixTime,1);
			return result;
		}
		
		public static UInt32 getUnixTimeUint32(){
			TimeSpan ts = (DateTime.UtcNow - new DateTime(1970,1,1,0,0,0));
			UInt32 unixTime = (UInt32)ts.TotalSeconds;
			return unixTime;
		}
		
		public static byte[] getCurrentTime(){
			TimeSpan ts = (DateTime.UtcNow - new DateTime(1970,1,1,0,0,0));
			UInt32 unixTime = (UInt32)ts.TotalSeconds;
			byte[] result = NumericalUtils.uint32ToByteArray(unixTime,1);
			return result;
		}

        public static byte[] getCurrentSimTime()
        {
            
            
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
             
            //long timeSim = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            //UInt32 simTimeTicks = (UInt32)timeSim;
            float timeSimFloat = (UInt32)ts.TotalSeconds * (1.0f / 1000.0f);
            byte[] result = NumericalUtils.floatToByteArray(timeSimFloat, 1);
            return result;
        }

		
		public static byte[] getUnixTime(int seconds){
			TimeSpan ts = (DateTime.UtcNow - new DateTime(1970,1,1,0,0,0));
			UInt32 unixTime = (UInt32)(ts.TotalSeconds+seconds);
			byte[] result = NumericalUtils.uint32ToByteArray(unixTime,1);
			return result;
		}

        public static string getCurrentDateTime(){
            string currentTime ;
            DateTime value = new DateTime();
            value = DateTime.Today;
            currentTime = value.ToString();
            return currentTime;

        }
	}
}
