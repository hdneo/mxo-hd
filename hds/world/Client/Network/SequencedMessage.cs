using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hds
{
    public class SequencedMessage
    {
        public byte[] content;
        public UInt16 sendCounter;
        private int resendTime;
        private int lastSentTime;
        private UInt16 neededAck;
        public bool isTimed;
        public bool isSent = false;

        public SequencedMessage(byte[] _content)
        {
            content = new byte[_content.Length];
            content = _content;
            sendCounter = 0;
            resendTime = 0;
            neededAck = 0;
            isTimed = false;
        }

        

        public void setNeededAck(UInt16 ackSSeq)
        {
            neededAck = ackSSeq;
        }

        public UInt16 getNeededAck()
        {
            return neededAck;
        }

        public void IncreaseSentCounter()
        {
            sendCounter++;
        }
        
        public void increaseResendTime()
        {
            resendTime = resendTime + 4000;
        }

        public int getResendTime()
        {
            return resendTime;
        }
    }
}
