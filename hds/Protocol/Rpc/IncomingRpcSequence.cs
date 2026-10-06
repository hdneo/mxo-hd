namespace hds.Protocol.Rpc
{
    /// <summary>
    /// Filters RPCs that were already executed. The client resends packets until they are acked, so the same
    /// RPC list can arrive more than once. Every list carries the counter of its first message and the
    /// messages count upwards from there (verified with the packet logs).
    /// Counter 0 starts a new sequence: the client restarts its counter when it loads a world.
    /// </summary>
    public sealed class IncomingRpcSequence
    {
        private ushort next;
        private bool started;

        public bool Accept(ushort counter)
        {
            if (!started || counter == 0)
            {
                started = true;
                next = (ushort) (counter + 1);
                return true;
            }

            var distance = (short) (counter - next);
            if (distance < 0)
            {
                return false;
            }
            next = (ushort) (counter + 1);
            return true;
        }

        public void Reset() => started = false;
    }
}
