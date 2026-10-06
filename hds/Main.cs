using System;
using hds.auth;
using hds.databases.Entities;
using hds.shared;


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
                Store.Loop = new hds.Core.WorldLoop();
                Store.auth = new AuthServer();
                Store.margin = new MarginServer();
                Store.world = new WorldServer();
                Store.worldThreads = new WorldThreads();

                Store.config = new ServerConfig("Config.xml");
                Store.config.LoadDbParams();
                Store.config.LoadServerParams();
                
                Store.worldConfig = new WorldConfig("WorldConfig.xml");

                /* Load Game Data */
                DataLoader.getInstance();

                /* Initialize DB Stuff */

                Store.dbManager = new databases.DatabaseManager();
                Store.matrixDbContext = new MatrixDbContext(Store.config.dbParams);

                if (Store.config.dbParams.DbType == "mysql"){
                    Store.dbManager.AuthDbHandler = new databases.MyAuthDBAccess();
                    Store.dbManager.MarginDbHandler = new databases.MyMarginDBAccess();
                    Store.dbManager.WorldDbHandler = new databases.MyWorldDbAccess();
                }                
				
                /* Protocol handling: RPC routes and the packet processor used by every client session */
				Store.dbManager.WorldDbHandler.ResetOnlineStatus();
                Store.RpcDispatcher = RpcRoutes.CreateDispatcher();
                Store.PacketProcessor = new ClientPacketProcessor(Store.RpcDispatcher);
                Output.WriteLine("[World Server] " + hds.Game.Objects.GoSchemaRegistry.Instance.Count + " game object types, " +
                                 Store.RpcDispatcher.Opcodes.Count + " client RPCs registered");


                /* External console */
				if(Store.config.serverParams.AdminConsoleEnabled){
					ConsoleSocket adminConsole = new ConsoleSocket();
					adminConsole.startServer();	
				}


                // Now everything should be loaded - START THE ENGINES!!!
                Store.worldThreads.Register(Store.Loop);
                Store.Loop.Start();
                Store.auth.startServer();
                Store.margin.StartServer();
                Store.world.StartServer();

                // Check if execution keeps going after starting
                Output.WriteLine("Im'running :D");
				

				// Real crashes must leave a trace (the orderly shutdown below only runs on Ctrl+C / Ctrl+Break)
				AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
				{
					var text = "[CRASH] " + DateTime.Now + " unhandled exception: " + eventArgs.ExceptionObject;
					Output.WriteLine(text);
					try { System.IO.File.AppendAllText("crash.log", text + Environment.NewLine); } catch { /* nothing left to do */ }
				};

				// Ctrl+C / Ctrl+Break stop the server - but only when pressed twice within 5 seconds,
				// a single Ctrl+C (e.g. copying log text from the console) must not end it
				var shutdown = new System.Threading.ManualResetEventSlim(false);
				var lastStopRequest = DateTime.MinValue;
				Console.CancelKeyPress += (sender, eventArgs) =>
				{
					eventArgs.Cancel = true; // shut down in an orderly way below
					var now = DateTime.Now;
					if (now - lastStopRequest > TimeSpan.FromSeconds(5))
					{
						lastStopRequest = now;
						Output.WriteLine("[Shutdown] " + eventArgs.SpecialKey + " received at " + now.ToString("HH:mm:ss") +
						                 " - press it again within 5 seconds to stop the server");
						return;
					}
					Output.WriteLine("[Shutdown] " + eventArgs.SpecialKey + " confirmed at " + now.ToString("HH:mm:ss"));
					shutdown.Set();
				};
				shutdown.Wait();

				Output.WriteLine("Closing Auth server and threads");
				Store.auth.stopServer();

				Output.WriteLine("Closing Margin server and threads");
				Store.margin.StopServer();

				Output.WriteLine("Closing World server and threads");
				Store.world.StopServer();
				Store.Loop.Stop();

				Output.WriteLine("Server exited");
			}
			else{
				Output.WriteLine("\nHealth checks not passed. Aborting launch.");
				Output.WriteLine("Please check the errors above or press Enter to close the window.");
				Console.ReadLine();
			}
		}
	}
}