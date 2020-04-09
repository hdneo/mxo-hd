using System;
using hds.auth;
using hds.shared;
using hds.world.scripting;


namespace hds{
	
	public static class MainClass{

		public static void Main(string[] args){
			var customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
			customCulture.NumberFormat.NumberDecimalSeparator = ".";
			System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            HealthCheck hc = new HealthCheck();
	
			if (hc.doTests()){
				
				Output.WriteLine("\nHealth checks OK. Proceeding.\n");
                

                // Create
                Store.auth = new AuthSocket();
                Store.margin = new MarginSocket();
                Store.world = new WorldSocket();
                Store.worldThreads = new WorldThreads();

                Store.config = new ServerConfig("Config.xml");
                Store.config.LoadDbParams();
                Store.config.LoadServerParams();
				
                /* Load Game Data */
                DataLoader.getInstance();

                /* Initialize DB Stuff */

                Store.dbManager = new databases.DatabaseManager();

                if (Store.config.dbParams.DbType == "mysql"){
                    Store.dbManager.AuthDbHandler = new databases.MyAuthDBAccess();
                    Store.dbManager.MarginDbHandler = new databases.MyMarginDBAccess();
                    Store.dbManager.WorldDbHandler = new databases.MyWorldDbAccess();
                }                
				
                /* Initialize the MPM object */

                Store.Mpm = new MultiProtocolManager();

                /* Initialize the scripting server */
                Store.rpcScriptManager = new ScriptManager();
                var scrLoader = new ScriptLoader();
                scrLoader.LoadScripts(); //<<-- This does


                /* External console */
				if(Store.config.serverParams.AdminConsoleEnabled){
					ConsoleSocket adminConsole = new ConsoleSocket();
					adminConsole.startServer();	
				}


                // Now everything should be loaded - START THE ENGINES!!!
                Store.auth.startServer();
                Store.margin.startServer();
                Store.world.startServer();

                // Check if execution keeps going after starting
                Output.WriteLine("Im'running :D");
				

				// Capture Ctrl C key to clean and then end the program
				Console.CancelKeyPress += delegate
				{
					Output.WriteLine("Closing Auth server and threads");
					Store.auth.stopServer();
					
					Output.WriteLine("Closing Margin server and threads");
                    Store.margin.stopServer();
					
					Output.WriteLine("Closing World server and threads");
                    Store.world.stopServer();
					
					Output.WriteLine("Server exited");
                };

			}
			else{
				Output.WriteLine("\nHealth checks not passed. Aborting launch.");
				Output.WriteLine("Please check the errors above or press Enter to close the window.");
				Console.ReadLine();
			}
		}
	}
}