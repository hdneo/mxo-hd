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
using System.Collections;


namespace hds{
	
	public class MarginSocket{
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
			Output.WriteLine("Margin server set and ready at port 10000");
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
					Output.WriteLine("Margin thread "+i+" is alive, closing it");
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

        // <summary>
        // An external Hole to establish the UDP Session.
        // Will be called by a seperate thread maybe (NOT FINISHED YET)
        // </summary>
        // <param name="charID">the CharID to find the correct thread
        public void sendUDPSessionReply(UInt32 charID){
            // Search the Margin Client
            foreach (MarginClient resultClient in clientList)
            {
                if (resultClient.getCharID() == charID)
                {
                    resultClient.EstablishUDPSessionReplyExternal();
                    return;
                }
            }

        }

        public bool isAnotherClientActive(UInt32 charId)
        {
            bool isActive = false;

            foreach (MarginClient resultClient in clientList)
            {
                if (resultClient.getCharID() == charId)
                {
                    isActive = true;
                    break;
                }
            }
            return isActive;
        }

        public void removeClientsByCharId(UInt32 charId)
        {
            MarginClient removeClient = null;
            foreach (MarginClient resultClient in clientList)
            {
                if (resultClient.getCharID() == charId)
                {
                    removeClient = resultClient;
                    break;
                }
            }
            clientList.Remove(removeClient);
        }

	}
}
