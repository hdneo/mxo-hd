using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Threading;

using hds.shared;

namespace hds{

	public class WorldClient{

		private EndPoint Remote;
		private Socket socket;
		private WorldEncryption cypher;
        
        private string key;
		
		private UInt32 lastUsedTime;
		private int deadSignals;
        private bool alive;
        private bool flushingQueueInProgress = false;

        public ClientData playerData { get; set; }
        public MessageQueue messageQueue;
        public ViewManager viewMan { get; set; }
        public PlayerCharacter playerInstance { get; set; }
		
		public WorldClient (EndPoint _Remote, Socket _socket,string _key){
			Remote = _Remote;
			socket = _socket;
			key = _key;
			cypher = new WorldEncryption();

            messageQueue = new MessageQueue();
			
			// Create the mpm structure for all world usage

            playerData = new ClientData();
            viewMan = new ViewManager();
			
			playerData.setUniqueKey(key);
			playerData.setRPCCounter((UInt16)0); //not hardcoded, RPC server system its AWESOME now!
            playerInstance = Store.world.objMan.GetAssignedObject(key);
			
			
			alive = true;
			deadSignals=0;


		}
		
				
		// This method sends one packet at a time to the game client 
		private void sendPacket(byte[] data){
			if(this.deadSignals==0){
                Output.WriteLine("[SEND PACKET] PSS: " + playerData.getPss().ToString() + " SSEQ : " + playerData.getSseq().ToString() + " CSEQ: " + playerData.getCseq().ToString());
				socket.SendTo(data, Remote);
			}
		}

        // Flush the MessageQueue Lists to the Client
        public void flushQueue()
        {
            // This resend the MessageQueue - should be called after parsing or after sending something out
            // or in a timed interval (keep alive for example)

            // Sends RAW MEssages
            sendRawMessages();
            Output.WriteLine("[CLIENT] In Queue Messages : " + messageQueue.ObjectMessagesQueue.Count.ToString() + " Object and " + messageQueue.RPCMessagesQueue.Count.ToString() + " RPC Messages");
            ArrayList worldPackets = new ArrayList();

            WorldPacket packet = new WorldPacket(playerData);

            // Init encrypted Packet if we have MPM Messages
            if (messageQueue.RPCMessagesQueue.Count > 0 || messageQueue.ObjectMessagesQueue.Count > 0 && flushingQueueInProgress==false)
            {
                flushingQueueInProgress = true;
                
                
                // we currently dont know if we send something out so we need to proove that in a way
                if (messageQueue.ObjectMessagesQueue.Count > 0)
                {
                    UInt16 sendingSSeq = playerData.calculateNextPossibleSseq();
                    lock (messageQueue.ObjectMessagesQueue.SyncRoot)
                    {
                        foreach (SequencedMessage messageObjects in messageQueue.ObjectMessagesQueue)
                        {
                            // Just append each message...
                            if (messageObjects.getResendTime() >= TimeUtils.getUnixTimeUint32() || messageObjects.getResendTime() == 0)
                            {
                                // Check if this really save the resendtime or not
                                messageObjects.increaseResendTime();
                                bool canAddThePak = packet.addObjectContent(messageObjects);
                                if (canAddThePak == false)
                                {
                                    packet.isFinal = true;
                                    // if one sub has a timed param set, we just set it here too
                                    worldPackets.Add(packet);
                                    // Start new packet and add it to the queue 
                                    packet = new WorldPacket(playerData);
                                    packet.addRPCContent(messageObjects);
                                    
                                }
                                if (messageObjects.isTimed == true)
                                {
                                    packet.timed = true;
                                }
                            }
                        }
                    }
                }

                // Workaround: complete the packet so we dont mix up
                if (packet.ObjectMessages.Count > 0)
                {
                    packet.isFinal = true;
                    worldPackets.Add(packet);
                    packet = new WorldPacket(playerData);
                    packet.timed = false;
                }
                
                if (messageQueue.RPCMessagesQueue.Count > 0)
                {
                    lock (messageQueue.RPCMessagesQueue.SyncRoot)
                    {
                        foreach (SequencedMessage messageRPC in messageQueue.RPCMessagesQueue)
                        {
                            // First do stuff on messages
                            // Just append each message...
                            if (messageRPC.getResendTime() >= TimeUtils.getUnixTimeUint32() || messageRPC.getResendTime() == 0)
                            {
                                // Check if this really save the resendtime or not
                                messageRPC.increaseResendTime();
                                if (packet.addRPCContent(messageRPC) == false)
                                {
                                    packet.isFinal = true;
                                    worldPackets.Add(packet);
                                    // Start new packet and add it to the queue 
                                    packet = new WorldPacket(playerData);
                                    packet.addRPCContent(messageRPC);

                                }

                                if (messageRPC.isTimed == true)
                                {
                                    packet.timed = true;
                                }
                            }

                        }
                    }
                    
                }

                // Check if the current not finalize packet has content and needs to be send
                if ((packet.isFinal == false) && (packet.ObjectMessages.Count > 0 || packet.RPCMessages.Count > 0))
                {                    
                    worldPackets.Add(packet);
                }
            }

            // We have nothing - but we should really ack this
            if (messageQueue.ackOnlyCount>0)
            {
                for (int i = 0; i < messageQueue.ackOnlyCount; i++)
                {
                    WorldPacket ackPacket = new WorldPacket(playerData);
                    packet.isFinal = true;
                    packet.timed = false;
                    worldPackets.Add(ackPacket);
                }
                messageQueue.ackOnlyCount = 0;
                
            }
            

            // We have now PacketObjects - time to send them
            if (worldPackets.Count > 0)
            {
                Output.WriteLine("[CLIENT] Flush the final Queue with " + worldPackets.Count.ToString() + " Packets");
                foreach (WorldPacket thePacket in worldPackets)
                {
                    playerData.IncrementSseq();
                    
                    byte[] finalData = thePacket.getFinalData(playerData);
                    Output.WritePacketLog(StringUtils.bytesToString(finalData), "SERVER", playerData.getPss().ToString(), playerData.getCseq().ToString(), playerData.getSseq().ToString());
                    byte[] encryptedData = cypher.encrypt(finalData, finalData.Length, playerData.getPss(), playerData.getCseq(), playerData.getSseq());
                    sendPacket(encryptedData);

                }
            }

            flushingQueueInProgress = false;

            
        }

        private void sendRawMessages()
        {
            lock (messageQueue.rawMessages.SyncRoot)
            {
                if (messageQueue.rawMessages.Count > 0)
                {
                    foreach (byte[] message in messageQueue.rawMessages)
                    {
                        sendPacket(message);
                    }
                }
                messageQueue.rawMessages.Clear();
            }
        }

        public void deletedAckedPackets(UInt16 ackSeq)
        {
            // First identify which messages needs to be deleted from queue
            ArrayList ackedObjectMessages = new ArrayList();
            lock (messageQueue.ObjectMessagesQueue.SyncRoot)
            {
                foreach (SequencedMessage message in messageQueue.ObjectMessagesQueue)
                {
                    // Check first reality then packetstation for the correct way we handle this
                    if (message.getNeededAck() <= ackSeq)
                    {
                        ackedObjectMessages.Add(message);
                    }
                }
            }
            

            ArrayList ackedRPCMessages = new ArrayList();
            lock (messageQueue.RPCMessagesQueue)
            {

                foreach (SequencedMessage message in messageQueue.RPCMessagesQueue)
                {
                    // Check first reality then packetstation for the correct way we handle this
                    if (message.getNeededAck() <= ackSeq)
                    {
                        ackedRPCMessages.Add(message);
                    }
                }
            }

            // Now delete them finally
            foreach (SequencedMessage deletedAckObject in ackedObjectMessages)
            {
                lock (messageQueue.ObjectMessagesQueue.SyncRoot)
                {
                    messageQueue.ObjectMessagesQueue.Remove(deletedAckObject);
                }
            }

            foreach (SequencedMessage deletedAckRPC in ackedRPCMessages)
            {
                lock (messageQueue.RPCMessagesQueue.SyncRoot)
                {
                    messageQueue.RPCMessagesQueue.Remove(deletedAckRPC);
                }
                
            }
            Output.WriteLine("[CLIENT] Removed " + messageQueue.ObjectMessagesQueue.Count + messageQueue.RPCMessagesQueue.Count + " Acked Messages by SSEQ : " + ackSeq);
        }

		
		public byte[] decryptReceivedPacket(byte[] packet){
			byte [] processedPacket;
			byte [] temp;
			
			ArrayList decValues;
					
			// Here we check if the first byte is "1" which means that the packet is encrypted or not
			
			processedPacket= new byte[packet.Length-1];
			ArrayUtils.copy(packet,1,processedPacket,0,packet.Length-1);
			decValues = new ArrayList();
			decValues = cypher.decrypt(packet,packet.Length);
			processedPacket = (byte[])decValues[0];
			
			
			if (processedPacket[0]==0x82) //It's a timed data packet
			{
				Output.WriteLine("[WORLD] TIMED UDP packet adjusted to normal header");
				temp = new byte[processedPacket.Length-4];
				temp[0] = (byte)0x02;
				ArrayUtils.copy(processedPacket,5,temp,1,processedPacket.Length-4);
				processedPacket = temp;
			}

            



			//clData.setPss((UInt16) decValues[1]);
            playerData.setPss((UInt16)decValues[1]);
			playerData.setCseq ((UInt16)decValues[2]); 
			playerData.setACK((UInt16) decValues[3]);

            this.deletedAckedPackets((UInt16)decValues[3]); // REMOVING FROM ACK QUEUE
			return processedPacket;
		}
		
		public void processPacket(byte[] packet){

			// Update the last time we are called
			lastUsedTime = TimeUtils.getUnixTimeUint32();
			
			// Decryption start
			bool encrypted = false;
			
			byte[] processedPacket =null;
            if (packet.Length > 0)
            {
                if (packet[0] == 0x00)
                { // Plain text packet
                    processedPacket = packet;
                }
                else
                {
                    encrypted = true;
                    processedPacket = decryptReceivedPacket(packet);
                }

                Output.WriteLine("\n" + key + " PSS = " + playerData.getPss() + ", Cseq = " + playerData.getCseq() + ", AckSSeq = " + playerData.getACK());
                Output.WriteLine("D: " + StringUtils.bytesToString(processedPacket));
                Output.WritePacketLog(StringUtils.bytesToString(processedPacket), "CLIENT", playerData.getPss().ToString(), playerData.getCseq().ToString(), playerData.getACK().ToString());

                Store.Mpm.Parse(encrypted, processedPacket);
                flushQueue();
            }

		}
		
		
		public bool Alive {
            get{
			    UInt32 now = TimeUtils.getUnixTimeUint32();
			    if (!this.alive || (now-lastUsedTime>30)){
				    return false;
			    }
			    return true;
            }
            set {
                alive = value;
            }
		}	
		
	}
}
