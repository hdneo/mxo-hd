using System;
using System.Collections;

namespace hds
{
    /// <summary>
    /// Unacknowledged messages of one client: waiting ones and the ones in flight (see <see cref="OutgoingStream"/>).
    /// </summary>
    public class MessageQueue
    {
        public ArrayList RPCMessagesQueue;
        public ArrayList ObjectMessagesQueue;
        public ArrayList rawMessages;

        public MessageQueue()
        {
            RPCMessagesQueue = new ArrayList();
            ObjectMessagesQueue = new ArrayList();
            rawMessages = new ArrayList();
        }

        /// <summary>Adds an RPC message (opcode + payload); the size prefix is added here.</summary>
        public void addRpcMessage(byte[] messageBlock)
        {
            byte[] rpcSizeByte;
            if (messageBlock.Length > 127)
            {
                rpcSizeByte = NumericalUtils.uint16ToByteArray((UInt16)(messageBlock.Length + 0x8000), 0);
            }
            else
            {
                rpcSizeByte = NumericalUtils.uint16ToByteArrayShort((UInt16)messageBlock.Length);
            }
            DynamicArray content = new DynamicArray();
            content.append(rpcSizeByte);
            content.append(messageBlock);

            RPCMessagesQueue.Add(new SequencedMessage(content.getBytes()));
        }

        /// <summary>Adds a view message (u16 view id + update, or an admin view command).</summary>
        public void addObjectMessage(byte[] messageBlock, bool isTimed)
        {
            ObjectMessagesQueue.Add(new SequencedMessage(messageBlock) {isTimed = isTimed});
        }

        public void addRawMessage(byte[] messageBlock)
        {
            rawMessages.Add(messageBlock);
        }

        /// <summary>Drops everything - used when the channel is reset for a world change.</summary>
        public void Clear()
        {
            RPCMessagesQueue.Clear();
            ObjectMessagesQueue.Clear();
        }
    }
}
