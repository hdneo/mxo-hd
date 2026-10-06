using System.Diagnostics;

namespace hds.Core
{
    /// <summary>
    /// Simulation time shared with the clients. The original server sent seconds since its start as float
    /// ("time adjustment" 82 xx xx xx xx, e.g. 5478.2 then 5480.1 a few packets later) and used the same clock
    /// in milliseconds for u32 time attributes (e.g. ILCombatHandler.StartTime, sim time of the self view).
    /// </summary>
    public static class ServerClock
    {
        /// <summary>Start value so the clock is never close to zero.</summary>
        private const double StartSeconds = 1000.0;

        private static readonly Stopwatch clock = Stopwatch.StartNew();

        public static double SimSeconds => StartSeconds + clock.Elapsed.TotalSeconds;

        public static float SimSecondsFloat => (float) SimSeconds;

        public static uint SimMilliseconds => (uint) (SimSeconds * 1000.0);
    }
}
