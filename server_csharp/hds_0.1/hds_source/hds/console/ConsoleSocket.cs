using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace hds
{
	public class ConsoleSocket
	{
		
		private TcpListener tcpListener;
		private ArrayUtils au;
		private StringUtils su;
    	private Thread listenThread;
		private bool mainThreadWorking;
		private WorldSocket world;
		
		public ConsoleSocket (ref WorldSocket world)
		{
			this.mainThreadWorking = true;
			this.world = world;
			this.au = new ArrayUtils();
			this.su = new StringUtils();
      		this.tcpListener = new TcpListener(IPAddress.Any, 55555);
			Console.WriteLine("Server console operating on port 55555");
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
				
				Console.WriteLine("Console client Connected.");
	  			
				
				// Receive TCP auth packets from the connected client.
				while (working){
	    			bytesRead = 0;
	
		    		try{ 
						bytesRead = clientStream.Read(message, 0, 2048);
					}
					catch{ break; }
		
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
						Console.WriteLine("Console server Error\nDebug:"+e);
						break;
					}
					
	  			}
	
	  			Console.WriteLine("Console client closing");
	  			client.Close();
				clientStream.Close();
					
	  		}
		}	
		
		private byte[] parseCommand(byte[] command,int length){
			byte[] temp = new byte[length];
			au.fastCopy(command,temp,length); //saves time and so
			string response = "";
			
			string txtCommand = su.charBytesToString(temp);
			bool responseSet = false;
			
			if (txtCommand.Equals("number")){
				response="Current connected players:"+world.getConnectedClients();
				responseSet=true;
			}
			
			if(txtCommand.Equals("names")){
				string[] names = world.getConnectedClientsNames();
				response ="Connected players handles:\n";
				for(int i = 0;i<names.Length;i++){
					response = response+names[i]+"\n";
				}
				responseSet=true;
			}
			
			if(txtCommand.StartsWith("send")){
				string data=txtCommand.Split(' ')[1];
				string handle=data.Split(',')[0];
				string packet=data.Split(',')[1];
				
				world.sendPacket(handle,packet,false);
				response="packet sent to "+handle;
				responseSet = true;	
			}
			
			if(txtCommand.StartsWith("esend")){
				string data=txtCommand.Split(' ')[1];
				string handle=data.Split(',')[0];
				string packet=data.Split(',')[1];
				
				world.sendPacket(handle,packet,true);
				response="packet sent to "+handle;
				responseSet = true;	
			}
			
			if(txtCommand.StartsWith("kick")){
				string handle=txtCommand.Split(' ')[1];
				
				world.kickPlayer(handle);
				response="kicked "+handle;
				responseSet = true;	
			}
			
			if (!responseSet){
				return su.stringToBytes("Unrecognized command: "+txtCommand);
			}
			return su.stringToBytes(response);
		}
		
	}
}

