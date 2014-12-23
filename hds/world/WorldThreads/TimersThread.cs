using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace hds
{
    public partial class WorldThreads
    {
        public void TimersThreadProcess()
        {
            Output.WriteLine("[WORLD SERVER]MoverThread started");
            // ToDo: This should update "timers" like Buffs, Skill Execution or something 
            Thread.Sleep(100);
        }
    }
}