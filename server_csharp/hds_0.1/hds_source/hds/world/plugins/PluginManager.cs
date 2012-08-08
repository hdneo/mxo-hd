using System;
using System.Collections;

namespace hds
{
	public class PluginManager
	{
		
		private Plugin[] pluginList = new Plugin[100];
		private ProtocolParser protocolParser;
		private ClientData cData;
		private WorldDbAccess databaseHandler;
		private bool replayerModeEnabled;
		
				
		/* DEFINE STATUS HERE */
		
		private const int IN_INITIAL_LOADING = 0;
		private const int IN_WORLD = 1;
		private const int IN_COMBAT = 2;
		private const int IN_DUEL = 3;
		private const int IN_REPLAYER = 4;
		
		private int clientStatus;
		private int nextStatus;
		
		/* END DEFINE STATUS */
		
		
		/* DEFINE PLUGINS FOR MXO HERE (same order than loadPlugins method) */
		
		private const int PLUGIN_INITIAL_LOADER = 0; // Plugin ID 0
		private const int PLUGIN_VENDOR=1; // Plugin for vendors
		private const int PLUGIN_CHATSELF=2; // Plugin for self chat
		private const int PLUGIN_NONE = 3; // This is the "just say 02" plugin
		private const int PLUGIN_ECHO = 4; // This is the "just-echo" plugin
		private const int PLUGIN_REPLAY = 5; // This is the "just-replay" plugin
		
		/* END DEFINE PLUGINS */
		
		public PluginManager (ref ClientData data,ref WorldDbAccess databaseHandler,bool replayerModeEnabled)
		{
			this.cData = data;
			this.databaseHandler = databaseHandler;
			this.replayerModeEnabled = replayerModeEnabled;
			
			if(this.replayerModeEnabled) //If replayer is on, we dont need to do anything
				clientStatus = IN_REPLAYER;
			else
				clientStatus = IN_INITIAL_LOADING;
			protocolParser = new ProtocolParser();
			loadPlugins();
		}
		
		// Same order than the "define plugin" list
		private void loadPlugins(){
			pluginList[0]=(new InitialPacketsLoader(cData,databaseHandler)); // Plugin ID 0
			pluginList[1]=(new Vendor(cData,databaseHandler));
			pluginList[2]=(new ChatSelf(cData,databaseHandler));
			pluginList[3]=(new None(cData,databaseHandler));
			pluginList[4]=(new Echo(cData,databaseHandler));
			pluginList[5]=(new Replay(cData,databaseHandler));
		} // Remove the last "end of string"
		
		
		public ArrayList process(byte[] packetData){
			
			Console.WriteLine("[PLUGIN MANAGER] Received packet");
			
			ArrayList response;
			
			int SELECTED_PLUGIN = -1;
			
			switch(clientStatus){
			
			case IN_INITIAL_LOADING:
					SELECTED_PLUGIN = PLUGIN_INITIAL_LOADER;
					nextStatus = IN_WORLD;
				break;
				
			case IN_WORLD:
					
					//We need to check if packet is a 03,04 or none of them
					SELECTED_PLUGIN = protocolParser.detectNeededPlugin(ref packetData);
					
					nextStatus = IN_WORLD; //Kinda recursive, if you let me say so
				break;
			
			case IN_REPLAYER:
					SELECTED_PLUGIN = PLUGIN_REPLAY;
					
					nextStatus = IN_REPLAYER; // Recursivity again :D.
			break;
				
				
			default:
				SELECTED_PLUGIN = PLUGIN_NONE;
				break;
			}
			
			// Ignore type of plugin, polymorphism
			response = pluginList[SELECTED_PLUGIN].process(packetData);
			Console.WriteLine("[PLUGIN MANAGER] Plugin retorned {0} packets",response.Count);
			
			//If we ended processing, we just say go to the next status
			if (pluginList[SELECTED_PLUGIN].endedProcess()) 
				clientStatus = nextStatus;
			return response;
		}
	}
}

