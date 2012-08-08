using System;
using System.Collections;
using MySql.Data.MySqlClient;

namespace hds
{
	public abstract class Plugin
	{
		
		protected ClientData cData;
		protected WorldDbAccess databaseHandler;
		protected PacketsUtils utils;
		
		public abstract ArrayList process(byte[] packetData);
		public abstract bool endedProcess();
		
	}
}

