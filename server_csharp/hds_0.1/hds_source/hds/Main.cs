using System;

namespace hds
{
	
	class MainClass
	{
		
		public static void Main(string[] args)
		{
			
			HealthCheck hc = new HealthCheck();

			if (hc.doTests()){
				
				Console.WriteLine("\nHealth checks OK. Proceeding.\n");
				AuthSocket auth = new AuthSocket();
				MarginSocket margin = new MarginSocket();
				
				// Send a reference to communicate world -> margin later
				WorldSocket world = new WorldSocket(ref margin); 
				
				
				auth.startServer();
				margin.startServer();
				world.startServer();
				
				XmlParser configParser = new XmlParser();
				string[] config = configParser.loadServerConfig("Config.xml");
				
				if(config[1].Equals("ON") || config[1].Equals("on") || config[1].Equals("On")){
					ConsoleSocket adminConsole = new ConsoleSocket(ref world);
					adminConsole.startServer();	
				}
				
				
				// Check if execution keeps going after starting
				Console.WriteLine("Im'running :D");
				

				// Capture Ctrl C key to clean and then end the program
				Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e) {
					Console.WriteLine("Closing Auth server and threads");
					auth.stopServer();
					
					Console.WriteLine("Closing Margin server and threads");
					margin.stopServer();
					
					Console.WriteLine("Closing World server and threads");
					world.stopServer();
					
					Console.WriteLine("Server exited");
                };

			}
			else{
				Console.WriteLine("\nHealth checks not passed. Aborting launch.");
			}
			
		}
	}
}