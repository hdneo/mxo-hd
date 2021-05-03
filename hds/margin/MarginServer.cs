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


namespace hds
{
    public class MarginServer
    {
        List<MarginClient> clientList;
        List<Thread> threadList;

        private TcpListener tcpListener;
        private Thread listenThread;
        private bool mainThreadWorking;

        public MarginServer()
        {
            // Threads storage
            mainThreadWorking = true;
            clientList = new List<MarginClient>();
            threadList = new List<Thread>();
            tcpListener = new TcpListener(IPAddress.Any, 10000);
            Output.WriteLine("Margin server set and ready at port 10000");
            listenThread = new Thread(new ThreadStart(ListenForClients));
        }

        public void StartServer()
        {
            listenThread.Start();
        }

        public void StopServer()
        {
            mainThreadWorking = false;
            for (int i = 0; i < clientList.Count; i++)
            {
                MarginClient temp = (MarginClient) clientList[i];
                temp.forceClose();
            }

            tcpListener.Stop();
            for (int i = 0; i < threadList.Count; i++)
            {
                Thread tempThread = (Thread) threadList[i];
                if (tempThread.IsAlive)
                {
                    Output.WriteLine("Margin thread " + i + " is alive, closing it");
                    tempThread.Interrupt();
                }
            }

            listenThread.Abort();
        }


        private void ListenForClients()
        {
            this.tcpListener.Start();

            while (mainThreadWorking)
            {
                // Create a new object when a client arrives
                TcpClient client = this.tcpListener.AcceptTcpClient();
                MarginClient marClient = new MarginClient(client.GetHashCode());

                // Define a new thread with the handling method as main loop and start it
                Thread clientThread = new Thread(new ParameterizedThreadStart(marClient.HandleClientComm));
                threadList.Add(clientThread);

                clientThread.Start(client);

                marClient.marginThreadId = clientThread.ManagedThreadId;
                // Add it to the Margin clients list
                clientList.Add(marClient);
            }
        }

        // <summary>
        // An external Hole to establish the UDP Session.
        // Will be called by a seperate thread maybe (NOT FINISHED YET)
        // </summary>
        // <param name="charID">the CharID to find the correct thread
        public void SendUDPSessionReply(UInt32 charID)
        {
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

        public bool IsAnotherClientActive(UInt32 charId)
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

        public void RemoveClientsByCharId(UInt32 charId)
        {
            MarginClient removeClient = null;
            foreach (MarginClient resultClient in clientList)
            {
                if (resultClient.getCharID() == charId)
                {
                    RemoveThreadFromThreadList(resultClient.marginThreadId);
                    removeClient = resultClient;
                    break;
                }
            }

            clientList.Remove(removeClient);
        }

        private void RemoveThreadFromThreadList(int ThreadId)
        {
            for (int i = 0; i < threadList.Count; i++)
            {
                Thread tempThread = (Thread) threadList[i];
                if (tempThread.ManagedThreadId == ThreadId)
                {
                    tempThread.Interrupt();
                }
            }
        }
    }
}