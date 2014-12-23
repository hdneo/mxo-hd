using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hds
{
    public class WorldPacket
    {
        // Holds the ArrayList for the Packets and proove if the size is reached
        public List<SequencedMessage> RPCMessages = new List<SequencedMessage>();
        public List<SequencedMessage> ObjectMessages = new List<SequencedMessage>();
        public UInt16 neededAckSSeq;
        
        public DynamicArray content = new DynamicArray();
        public bool isFinal = false;
        public UInt16 bufferSize = 0;
        private UInt16 maxBuffer = 1300;
        public ClientData playerData;
        public bool timed = false;
        public bool isRPCReset = false;

        public WorldPacket(ClientData _playerData)
        {
            this.playerData = _playerData;
        }

        public void setFinalContent(byte[] content)
        {

        }

        public bool addRPCContent(SequencedMessage rpcMessage)
        {
            bool bufferSize = checkIfBufferIsFull(rpcMessage.content.Length);
            if (bufferSize == false)
            {
                this.RPCMessages.Add(rpcMessage);
                return true;
            }
            return false;
        }

        public bool addObjectContent(SequencedMessage objectMessage)
        {
            bool bufferSize = checkIfBufferIsFull(objectMessage.content.Length);
            if (bufferSize == false)
            {
                this.ObjectMessages.Add(objectMessage);
                return true;
            }
            return false;
        }

        public bool checkIfBufferIsFull(int sizeOfPossibleAddedMessage)
        {
            if (bufferSize + sizeOfPossibleAddedMessage > maxBuffer)
            {
                return true;
            }
            return false;
        }

        public byte[] getFinalData(ClientData playerData)
        {
            // TODO: Sim Time with Client (not real time) on every 4 Local SSEQ we send
            

            playerData.IncrementSseq();
            this.neededAckSSeq = playerData.getSseq();

            if ((playerData.getSseq() - playerData.lastSimTimeSEQ) >= 4)
            {
                timed = true;
            }

            playerData.getRPCShutDown();
            if (timed)
            {

                if (playerData.waitForRPCShutDown==true)
                {
                    content.append((byte)0xc2);
                }
                else
                {
                    content.append((byte)0x82);
                }

                content.append(TimeUtils.getCurrentSimTime(2000));
            }
            else
            {

                if (playerData.waitForRPCShutDown == true)
                {
                    content.append((byte)0x42);
                }
                else
                {
                    content.append((byte)0x02);
                }

            }

            // Merge all Message together and generate the Final Packet Header
            generateObjectMessageData();
            generateRpcMessageData();
            
            return content.getBytes();
            
        }

        public void generateRpcMessageData()
        {
            // Example : 04 01 00 15 01 + submessages
            // Strcut: uint8 04 header + uint8 listcount + uint16 currentRPCCounter + submessageCounter (list1)
            // If you have 2 lists , the data struct is : uint8 submessagecount + submessages

            // TODO!!! FINALIZE!!
            // Here goes code from RPCPacket (As it should be generated on the fly)
            if (RPCMessages.Count > 0)
            {

                content.append((byte)ProtocolHeaders.RPC_PROTOCOL);
                // ToDo: Figure out what the "Lists" Count should handle ...maybe different types?
                // Why not having just one List and adding them all ? Maybe a list has a limit ? 
                content.append(0x01); // List Count for RPC Messages
                content.append(NumericalUtils.uint16ToByteArray(playerData.getRPCCounter(),0));
                content.append(NumericalUtils.uint16ToByteArrayShort((UInt16)RPCMessages.Count));
                foreach (SequencedMessage message in RPCMessages)
                {
                    // Do the loop
                    content.append(message.content);
                }

                // Set new RPC Counter
                Output.writeToLogForConsole("RPC COUNT BEFORE: " + playerData.getRPCCounter().ToString());
                UInt16 incrementRpcCounter = playerData.getRPCCounter();
                incrementRpcCounter += (ushort)RPCMessages.Count;
                playerData.setRPCCounter(incrementRpcCounter);
                Output.writeToLogForConsole("RPC COUNT AFTER: " + playerData.getRPCCounter().ToString());
            }
            
        }

        public void generateObjectMessageData()
        {
            // ToDo: add the SSEQ to the MessageQueue Objects
            if (ObjectMessages.Count > 0)
            {
                content.append((byte)ProtocolHeaders.OBJECT_VIEW_PROTOCOL);
                // Add ViewId 0 to tell our Client that Object Protocol is done now
                foreach (SequencedMessage message in ObjectMessages)
                {
                    message.increaseResendTime();
                    message.setNeededAck(this.neededAckSSeq);
                    content.append(message.content);
                }

                content.append(0x00);
                content.append(0x00);

            }
            
        }



        
    }
}
