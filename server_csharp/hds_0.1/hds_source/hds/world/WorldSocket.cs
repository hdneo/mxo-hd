using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;
using System.Security.Cryptography;

namespace hds
{
	public class WorldSocket
	{
		private IPEndPoint udplistener;
        private Thread listenThread;
        private int serverport;
		private bool mainThreadWorking;
		
		private ArrayUtils au;
		private Dictionary<string, WorldClient> clients;
        private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		private MarginSocket marginRef;
		
		private WorldDbAccess databaseHandler;
		

		public WorldSocket (ref MarginSocket marginRef)
		{

			serverport = 10000;
			clients =  new Dictionary<string, WorldClient>();
            udplistener = new IPEndPoint(IPAddress.Any, serverport);
			au = new ArrayUtils();
			databaseHandler = new WorldDbAccess();
			this.marginRef = marginRef;
			
            listenThread = new Thread(new ThreadStart(ListenForAllClients));
			this.mainThreadWorking = true;
			Console.WriteLine("World Server set and ready at UDP port 10000");
		}
		
		public void startServer(){
			listenThread.Start();
		}
		
		public void stopServer(){
			this.mainThreadWorking = false;
		}
			
		
		private void ListenForAllClients()
        {

            socket.Bind(this.udplistener);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)(sender);
			
			
			while (mainThreadWorking)
            {
                // check if an input is done
                //string stringInput = Console.ReadLine();

                byte[] message = new byte[4096];
                // Debug, show received packet in hex
				
				//Clean the house first.
				cleanDeadPlayers();
				
				//Now, it's cleaned.
				
                int receivedDataLenght = socket.ReceiveFrom(message, ref Remote);

                byte[] finalMessage = new byte[receivedDataLenght];

				
				// Use fast copy to do express operations
				au.fastCopy(message,finalMessage,receivedDataLenght);
                
                if (receivedDataLenght > 0)
                {
                    Console.WriteLine("Datasize is :" + receivedDataLenght);
                }

				
				
				string key = Remote.ToString();
				
				Console.WriteLine("Packet coming from "+key);
				
				/* Check if key it's on the list or is a new client */
				WorldClient value;
				if (clients.TryGetValue(key, out value)){
					clients[key].processPacket(finalMessage);
				}
				else{
					// It's the first packet, always non encrypted. Taking the charID now 
					
					byte[] charID = new byte[4];
					charID[0] = finalMessage[11];
					charID[1] = finalMessage[12];
					charID[2] = finalMessage[13];
					charID[3] = finalMessage[14];
					
					
					
					UInt32 userID = databaseHandler.getUserIdForCharId(charID);
					// Now we notify the margin server about the connected user 
					
					Console.WriteLine("[WORLD] Session Reply sent to Margin server for userID: "+userID);
					
					marginRef.unlockWaitingUser(userID);
					
					// And we can keep processing it here
					
					WorldClient newClient = new WorldClient(Remote,socket,key,databaseHandler);
					clients.Add(key,newClient);
					// We save one key hashing by using the created client this time
					newClient.processPacket(finalMessage);
				}
				
                
            }
			
		}
		
		
		private void cleanDeadPlayers(){
			
			ArrayList deadPlayers = new ArrayList();
			foreach (KeyValuePair<string, WorldClient> client in clients)
			{		
				if(!client.Value.isAlive()){
					deadPlayers.Add(client.Key);
				}
			}
			
			Console.WriteLine("Found {0} dead players",deadPlayers.Count);
			for(int i = 0;i<deadPlayers.Count;i++){
				clients.Remove((string)deadPlayers[i]);
			}
		}
		
		/* FOLLOWING METHODS ARE EXCLUSIVE TO SERVER OPERATION CONSOLE */
		
		public int getConnectedClients(){
			return clients.Count;
		}
		
		public string[] getConnectedClientsNames(){
			string [] names = new string[getConnectedClients()];
			
			int i = 0;
			foreach (KeyValuePair<string, WorldClient> client in clients)
			{
				
				ClientData temp = client.Value.getClientData();
				names[i] = (string)temp.getPlayerValue("handle");
				i++;
			}
			
			
			return names;
		}
		
		public void sendPacket(string handle, string packet,bool encryption){
			foreach (KeyValuePair<string, WorldClient> client in clients)
			{
				
				ClientData temp = client.Value.getClientData();
				string tempHandle= (string)temp.getPlayerValue("handle");
				if(tempHandle.Equals(handle)){
					StringUtils su = new StringUtils();
					byte[] data = su.hexStringToBytes(packet.Trim());
					client.Value.externalSend(data,encryption);
					break;
				}
			}
		}
		
		
		
		public void kickPlayer(string handle){
			foreach (KeyValuePair<string, WorldClient> client in clients)
			{
				
				ClientData temp = client.Value.getClientData();
				string tempHandle= (string)temp.getPlayerValue("handle");
				if(tempHandle.Equals(handle)){
					
					client.Value.die(); //brave you, killing in distance...
					break;
				}
			}
		}
		/* END OF SERVER OPERATION CONSOLE METHODS*/
		
		
		
	}
}
