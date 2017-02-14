using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace hds{

	public class ConsoleSocket{
		
		private TcpListener tcpListener;	 
    	private Thread listenThread;
		private bool mainThreadWorking;
        private int consoleport = 55557;


        public ConsoleSocket (){
			this.mainThreadWorking = true;
			 
      		this.tcpListener = new TcpListener(IPAddress.Any, consoleport);
			Output.WriteLine("[Console] Server console operating on port " + consoleport);
      		this.listenThread = new Thread(new ThreadStart(ListenForClients));
		}
		
		public void startServer(){
			this.listenThread.Start();
		}
		
		public void stopServer(){
			this.mainThreadWorking = false;
			this.tcpListener.Stop();
		}
		
		private  void ListenForClients(){
  			this.tcpListener.Start();

  			while (mainThreadWorking){
    			// Create a new thread per connected client	
    			TcpClient client = this.tcpListener.AcceptTcpClient();
    		
	  			NetworkStream clientStream = client.GetStream();
				bool working = true;
				
	  			byte[] message = new byte[2048];
	  			int bytesRead;
				
				Output.WriteLine("[Console] Console client Connected.");
	  			
				
				// Receive TCP auth packets from the connected client.
				while (working){
	    			bytesRead = 0;
	
		    		try{ 
						bytesRead = clientStream.Read(message, 0, 2048);
					}
					catch{break;}
		
		    		if (bytesRead == 0)
		            	break;
					
					// Parse the received packet data
					try{
	    				
						byte[] response = parseCommand(message,bytesRead);
						
						if(response.Length == 1 && response[0]==0x00)
							break;
						else
							clientStream.Write(response,0,response.Length);
						
					}catch(Exception e){
						Output.WriteLine("[Console] Server console Error\nDebug:"+e);
						break;
					}
	  			}
	
	  			Output.WriteLine("[Console] Console client closing");
	  			client.Close();
				clientStream.Close();
					
	  		}
		}	
		
		private byte[] parseCommand(byte[] command,int length){
			byte[] temp = new byte[length];
            

			ArrayUtils.fastCopy(command,temp,length); //saves time and so
			string response = "";
			
			string txtCommand = StringUtils.charBytesToString(temp);
			bool responseSet = false;

            //TODO: REMOVE CLIENT CONSOLE

			if (!responseSet){
				return StringUtils.stringToBytes("Unrecognized command: "+txtCommand);
			}
			
			return StringUtils.stringToBytes(response);
		}
		
	}
}

