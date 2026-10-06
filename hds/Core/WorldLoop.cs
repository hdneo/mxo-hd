using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace hds.Core
{
    /// <summary>
    /// The single game thread. All world state (clients, views, mobs, combat...) is only touched from here:
    /// network threads <see cref="Post"/> incoming packets, game code uses <see cref="Schedule"/> instead of
    /// System.Threading timers. This removes the races between the receive callback, the old worker threads
    /// and timer callbacks.
    /// </summary>
    public sealed class WorldLoop
    {
        public static readonly TimeSpan DefaultTickInterval = TimeSpan.FromMilliseconds(50);

        private readonly ConcurrentQueue<Action> inbox = new ConcurrentQueue<Action>();
        private readonly PriorityQueue<ScheduledTask, long> timers = new PriorityQueue<ScheduledTask, long>();
        private readonly List<(string Name, Action<long> Tick)> tickSystems = new List<(string, Action<long>)>();
        private readonly AutoResetEvent wakeUp = new AutoResetEvent(false);
        private readonly Stopwatch clock = Stopwatch.StartNew();
        private readonly TimeSpan tickInterval;
        private Thread thread;
        private volatile bool running;
        private long sequence;

        public WorldLoop() : this(DefaultTickInterval)
        {
        }

        public WorldLoop(TimeSpan tickInterval)
        {
            this.tickInterval = tickInterval;
        }

        /// <summary>Milliseconds since the loop was created (monotonic).</summary>
        public long NowMs => clock.ElapsedMilliseconds;

        public bool IsWorldThread => Thread.CurrentThread == thread;

        public void Start()
        {
            if (running) return;
            running = true;
            thread = new Thread(Run) {Name = "WorldLoop", IsBackground = true};
            thread.Start();
        }

        public void Stop()
        {
            running = false;
            wakeUp.Set();
            thread?.Join(TimeSpan.FromSeconds(5));
        }

        /// <summary>Queues work for the world thread (thread safe).</summary>
        public void Post(Action action)
        {
            inbox.Enqueue(action ?? throw new ArgumentNullException(nameof(action)));
            wakeUp.Set();
        }

        /// <summary>Runs <paramref name="action"/> once on the world thread after <paramref name="delay"/>.</summary>
        public ScheduledTask Schedule(TimeSpan delay, Action action) => AddTimer(delay, TimeSpan.Zero, action);

        /// <summary>Runs <paramref name="action"/> repeatedly on the world thread until the task is cancelled.</summary>
        public ScheduledTask ScheduleRepeating(TimeSpan interval, Action action) => AddTimer(interval, interval, action);

        /// <summary>Registers a system that runs on every tick (argument: <see cref="NowMs"/>).</summary>
        public void AddTickSystem(string name, Action<long> tick)
        {
            Post(() => tickSystems.Add((name, tick)));
        }

        private ScheduledTask AddTimer(TimeSpan delay, TimeSpan interval, Action action)
        {
            var task = new ScheduledTask(action, (long) interval.TotalMilliseconds);
            var due = NowMs + (long) Math.Max(0, delay.TotalMilliseconds);
            Post(() => Enqueue(task, due));
            return task;
        }

        private void Enqueue(ScheduledTask task, long due)
        {
            task.DueMs = due;
            // sequence keeps FIFO order for timers with the same due time
            timers.Enqueue(task, (due << 20) + (Interlocked.Increment(ref sequence) & 0xFFFFF));
        }

        /// <summary>Processes queued work and due timers once. Used by the loop and by tests.</summary>
        public void RunOnce()
        {
            while (inbox.TryDequeue(out var action))
            {
                Execute("posted action", action);
            }

            var now = NowMs;
            while (timers.TryPeek(out var task, out _) && task.DueMs <= now)
            {
                timers.Dequeue();
                if (task.Cancelled) continue;
                Execute("scheduled task", task.Action);
                if (task.IntervalMs > 0 && !task.Cancelled)
                {
                    Enqueue(task, now + task.IntervalMs);
                }
            }

            foreach (var (name, tick) in tickSystems)
            {
                Execute(name, () => tick(now));
            }
        }

        private static void Execute(string what, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                // One broken handler must not stop the world
                Output.WriteLine($"[WorldLoop] Exception in {what}: {ex}");
            }
        }

        private void Run()
        {
            Output.WriteLine("[WorldLoop] started");
            while (running)
            {
                var start = NowMs;
                RunOnce();
                var elapsed = NowMs - start;
                var wait = (int) Math.Max(1, tickInterval.TotalMilliseconds - elapsed);
                wakeUp.WaitOne(wait);
            }
            Output.WriteLine("[WorldLoop] stopped");
        }
    }

    public sealed class ScheduledTask
    {
        internal ScheduledTask(Action action, long intervalMs)
        {
            Action = action;
            IntervalMs = intervalMs;
        }

        internal Action Action { get; }
        internal long IntervalMs { get; }
        internal long DueMs { get; set; }

        public bool Cancelled { get; private set; }

        public void Cancel() => Cancelled = true;
    }
}
