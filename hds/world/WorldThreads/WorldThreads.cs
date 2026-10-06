using System;
using hds.Core;

namespace hds
{
    /// <summary>
    /// Periodic world systems. They used to run on their own threads; now they are scheduled on the
    /// world loop so they never race with packet processing.
    /// </summary>
    public partial class WorldThreads
    {
        public static readonly TimeSpan ViewVisibilityInterval = TimeSpan.FromMilliseconds(500);
        public static readonly TimeSpan TimersInterval = TimeSpan.FromSeconds(2);
        public static readonly TimeSpan ResendInterval = TimeSpan.FromMilliseconds(250);

        public void Register(WorldLoop loop)
        {
            loop.ScheduleRepeating(ViewVisibilityInterval, ViewVisibleTick);
            loop.ScheduleRepeating(TimersInterval, TimersTick);
            loop.ScheduleRepeating(ResendInterval, ResendTick);
            Output.WriteLine("[WORLD SERVER] view visibility and timer systems scheduled");
        }
    }
}
