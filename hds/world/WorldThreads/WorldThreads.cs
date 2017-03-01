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
            //moverThread = new Thread(MoverThreadProcess);
            timersThread = new Thread(TimersThreadProcess);
            viewVisibleThread = new Thread(ViewVisibleThread);

            // Start Threads
            //moverThread.Start();
            timersThread.Start();
            viewVisibleThread.Start();
        }

    }
}
