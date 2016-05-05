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
        public Thread moverThread;
        public Thread timersThread;
        public Thread viewVisibleThread;
        public WorldThreads()
        {
            // Init Threads
            moverThread = new Thread(new ThreadStart(MoverThreadProcess));
            timersThread = new Thread(new ThreadStart(TimersThreadProcess));
            viewVisibleThread = new Thread(new ThreadStart(ViewVisibleThread));

            // Start Threads
            moverThread.Start();
            timersThread.Start();
            viewVisibleThread.Start();
        }

    }
}
