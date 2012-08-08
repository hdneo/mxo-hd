/// <summary>
/// Hardline Dreams Team - 2010
/// Created by Neo
/// Modified by Morpheus
/// </summary>

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;


namespace hds
{
	
	public class MarginSocket
	{
		List <MarginClient> clientList;
		List <Thread> threadList;
		
		private TcpListener tcpListener;
    	private Thread listenThread;
		private bool mainThreadWorking;
		

    	public MarginSocket(){
			
			// Threads storage
			this.mainThreadWorking = true;
			this.clientList = new List<MarginClient>();
			this.threadList = new List<Thread>();
			
      		this.tcpListener = new TcpListener(IPAddress.Any, 10000);
			Console.WriteLine("Margin server set and ready at port 10000");
      		this.listenThread = new Thread(new ThreadStart(ListenForClients));
			
    	}
		
		public void startServer(){
			this.listenThread.Start();
		}
		
		public void stopServer(){
			this.mainThreadWorking = false;
			for (int i = 0;i<clientList.Count;i++){
				MarginClient temp = (MarginClient) clientList[i];
				temp.forceClose();

			}
			
			this.tcpListener.Stop();
			for (int i = 0;i<threadList.Count;i++){
				Thread tempThread = (Thread) threadList[i];
				if (tempThread.IsAlive){
					Console.WriteLine("Margin thread {0} is alive, closing it",i);
					tempThread.Abort();
				}
					
			}
			
			this.listenThread.Abort();
		}
	
		public void unlockWaitingUser(UInt32 userId){
			for (int i=0;i<clientList.Count;i++){
				MarginClient temp = (MarginClient) clientList[i];
				if(temp.isYourClientWaiting(userId))
					break;
			}
		}
		
		private void ListenForClients(){
  			this.tcpListener.Start();

  			while (mainThreadWorking){
    			// Create a new object when a client arrives
    			TcpClient client = this.tcpListener.AcceptTcpClient();
				MarginClient marClient = new MarginClient(client.GetHashCode());
				
				// Add it to the Margin clients list
				clientList.Add(marClient);
				
				// Define a new thread with the handling method as main loop and start it
    			Thread clientThread = new Thread(new ParameterizedThreadStart(marClient.HandleClientComm));
				threadList.Add(clientThread);
				
    			clientThread.Start(client);
  			}
		}	
		
		
		
	}
}
