using System;
using System.Collections.Generic;
using System.Text;

using hds.databases;
using hds.world.scripting;

namespace hds.shared{
    public class Store{

        /* Configuration */

        public static ServerConfig config { get; set; }

        /* Servers */
        public static AuthSocket auth {get;set;}
        public static MarginSocket margin {get;set;}
        public static WorldSocket world {get;set;}

        /* Threading */
        public static WorldThreads worldThreads { get; set; }

        /* Database Handling */
        public static DatabaseManager dbManager { get; set; }

        /* Protocol Handling */
        public static WorldClient currentClient { get; set; }
        public static MultiProtocolManager Mpm { get; set; }

        /* Scripting Handling */

        public static ScriptManager rpcScriptManager { get; set; }

    }
}
