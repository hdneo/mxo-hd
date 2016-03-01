using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace hds
{
    // This Class holds all Queue Submessages
    public class MessageQueue
    {
        public ArrayList RPCMessagesQueue;
        public ArrayList ObjectMessagesQueue;
        public ArrayList rawMessages;
        public int ackOnlyCount = 0;

        public MessageQueue()
        {
            RPCMessagesQueue = new ArrayList();
            ObjectMessagesQueue = new ArrayList();
            rawMessages = new ArrayList();
        }

        
        /*
         * Add a RPC Message with size byte automatically to the queue
         */
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

            SequencedMessage message = new SequencedMessage(content.getBytes());
            RPCMessagesQueue.Add(message);
        }

        public void addObjectMessage(byte[]messageBlock,bool isTimed)
        {
            SequencedMessage message = new SequencedMessage(messageBlock);
            message.isTimed = isTimed;
            lock (ObjectMessagesQueue.SyncRoot)
            {
                ObjectMessagesQueue.Add(message);
            }
            
        }

        public void addRawMessage(byte[] messageBlock)
        {
            rawMessages.Add(messageBlock);
        }

    }
}
