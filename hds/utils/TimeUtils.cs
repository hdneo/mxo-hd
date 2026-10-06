
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

        /// <summary>Simulation time in seconds as float (see <see cref="hds.Core.ServerClock"/>).</summary>
        public static byte[] getCurrentSimTime()
        {
            // was: unix seconds / 1000, which made the client clock advance 1 ms per real second
            return NumericalUtils.floatToByteArray(hds.Core.ServerClock.SimSecondsFloat, 1);
        }

        public static UInt32 getCurrentSimTimeMs()
        {
            return hds.Core.ServerClock.SimMilliseconds;
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
