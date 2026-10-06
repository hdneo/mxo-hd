using System;

namespace hds
{
    /// <summary>
    /// A view (03) or RPC (04) message of the reliable stream. It stays in the client's message queue until
    /// the packet that carried it was acked; lost packets put their messages back to "not sent".
    /// </summary>
    public class SequencedMessage
    {
        private static long nextOrder;

        public byte[] content;
        public bool isTimed;

        /// <summary>Queue order - resent messages keep their place before newer ones.</summary>
        public readonly long Order;

        /// <summary>RPC counter, assigned when the RPC is sent the first time and kept for every resend.</summary>
        public int RpcCounter = -1;

        /// <summary>Sequence of the packet that carries the message right now, null = waits for sending.</summary>
        public ushort? InFlightSequence;

        public int SendCount;

        public SequencedMessage(byte[] _content)
        {
            content = _content;
            Order = System.Threading.Interlocked.Increment(ref nextOrder);
        }
    }
}
