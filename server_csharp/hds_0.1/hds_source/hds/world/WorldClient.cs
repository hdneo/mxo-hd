
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.IO;
using System.Threading;


namespace hds
{


	public class WorldClient
	{

		private EndPoint Remote;
		private Socket socket;
		private WorldEncryption cypher;
		
		
		private ArrayUtils au;
		private StringUtils su;
		private TimeUtils timer;
		
		private PluginManager manager;
		private ClientData clData;
		private WorldDbAccess databaseHandler;
		private XmlParser configParser;
		
		
		private bool replayerModeOn;
		private bool alive;
		private UInt32 lastUsedTime;
		private int deadSignals;
		
		private int pssIndex;
		
		public WorldClient (EndPoint Remote, Socket socket,string key, WorldDbAccess databaseHandler)
		{
			this.Remote = Remote;
			this.socket = socket;
			this.databaseHandler = databaseHandler;
			this.cypher = new WorldEncryption();
			this.au = new ArrayUtils();
			this.su = new StringUtils();
			this.timer = new TimeUtils();
			this.replayerModeOn = false;
			
			this.clData = new ClientData();
			this.clData.setUniqueKey(key);
			
			this.pssIndex = 0;
			
			this.alive= true;
			this.deadSignals=0;
			
			this.configParser = new XmlParser();
			
			string []data = configParser.loadServerConfig("Config.xml");
			if (data[0].Equals("ON") || data[0].Equals("on") || data[0].Equals("On"))
				replayerModeOn=true;
			
			clData.setRPCCounter((UInt16)87); //HARDCODED
			

			this.manager = new PluginManager(ref clData,ref this.databaseHandler,replayerModeOn); // Link plugin manager (and all its plugins) with our data
		}
		
		// This method makes the client connection die inmediately
		public void die(){
			//Instashutdown!
			sendPacket(su.hexStringToBytes("02040100000101a1"));
		}
		
		// This method is just an external hole to send data (event manager or consoles)
		public void externalSend(byte[] data,bool encryption){
			clData.IncrementSseq();
			if(encryption){
				byte[] encryptedData = cypher.encrypt(data,data.Length,clData.getPss(),clData.getCseq(),clData.getSseq());
				sendPacket(encryptedData);
			}
			else
				sendPacket(data);
		}
		
		
		// This method sends one packet at a time to the game client 
		private void sendPacket(byte[] data){
			if(this.deadSignals==0){
				socket.SendTo(data, Remote);
			}
		}
		
		
		// This method process a list of 0 to N packets and sends it with the right options
		private void sendPacketList(ArrayList packetList){
			WorldPacket temp;
			for (int i = 0;i<packetList.Count;i++){
				temp = (WorldPacket)packetList[i];
				
				if(temp.getSleepTime()==0){ //Not a sleep packet
				
					if (!temp.getEncrypted()){
						sendPacket(temp.getContent());
						//Sending non-encrypted packet to the client
					}
					else{
						//Encrypted packet
						clData.IncrementSseq();
						byte[] encryptedData = cypher.encrypt(temp.getContent(),temp.getContent().Length,clData.getPss(),clData.getCseq(),clData.getSseq());
						sendPacket(encryptedData);
						//Sending an encrypted packet to the client
					}
				}
				else{ // Sometimes we need a sleep between packets. We fake it by using a timer on the packet with empty content
					Thread.Sleep(temp.getSleepTime());
				}
			}

		}
		
		
		public byte[] decryptReceivedPacket(byte[] packet){
			byte [] processedPacket;
			
			ArrayList decValues;
					
			// Here we check if the first byte is "1" which means that the packet is encrypted or not
			if (packet[0] == 0x01){
				processedPacket= new byte[packet.Length-1];
				au.copy(processedPacket,0,packet,1,packet.Length-1);
				decValues = new ArrayList();
				decValues = cypher.decrypt(packet,packet.Length);
				processedPacket = (byte[])decValues[0];
				clData.setPss((UInt16) decValues[1]);
				clData.setCseq ((UInt16)decValues[2]); 
				clData.setACK((UInt16) decValues[3]);
			}
			
			else {
				 processedPacket = packet; // Packet is a non-encrypted packet
			}
			return processedPacket;
		}
		
		public void processPacket(byte[] packet){
			
			// Decryption start

			byte [] processedPacket = decryptReceivedPacket(packet);
			
			Console.WriteLine("Packet Seq, PSS = {0} , Cseq = {1}, AckSSeq = {2}",clData.getPss(),clData.getCseq(),clData.getACK());
			Console.WriteLine("Received Packet: "+su.bytesToString(processedPacket));
			

			/* WORLD CLIENT DYING PATTERN HANDLING */

			this.lastUsedTime = timer.getUnixTimeUint32();
			
			byte[] deadNotice = {0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x04};
			
			if(au.equal(deadNotice,processedPacket)){
				Console.WriteLine("Advice");
				this.deadSignals++;
				if(deadSignals==4)
					this.alive=false;
			}
			/* END OF WORLD CLIENT DYING PATTERN */
			
			else{
				//PROCESS HERE (manager would take care of processing the packets)
				sendPacketList(manager.process(processedPacket));
				//PROCESS HERE (manager already did its work)
			}
		}
		
		public bool isAlive(){
			UInt32 now = timer.getUnixTimeUint32();
			if (!this.alive || (now-lastUsedTime>30))
				return false;
			
			return true;
		}
		
		public ClientData getClientData(){
			return this.clData;
		}
		
	}
}
