using System;
using System.Collections.Generic;
using System.Text;
using hds.auth;
using hds.databases;
using hds.databases.Entities;

namespace hds.shared{
    public class Store{

        /* Configuration */

        public static ServerConfig config { get; set; }
        public static WorldConfig worldConfig { get; set; }

        /* Servers */
        public static AuthServer auth {get;set;}
        public static MarginServer margin {get;set;}
        public static WorldServer world {get;set;}

        /* Threading */
        public static WorldThreads worldThreads { get; set; }

        /* Database Handling */
        public static DatabaseManager dbManager { get; set; }
        public static MatrixDbContext matrixDbContext { get; set; }

        /* World thread - all game state is only modified from here */
        public static hds.Core.WorldLoop Loop { get; set; }

        /* Protocol Handling */
        /// <summary>
        /// Client whose packet is currently processed on the world thread. Legacy code only - new code gets the
        /// client passed explicitly.
        /// </summary>
        public static WorldClient currentClient { get; set; }
        public static ClientPacketProcessor PacketProcessor { get; set; }
        public static hds.Protocol.Rpc.RpcDispatcher RpcDispatcher { get; set; }

        /* Game systems */
        public static hds.Game.Combat.CloseCombatService CloseCombat { get; set; }

    }
}
