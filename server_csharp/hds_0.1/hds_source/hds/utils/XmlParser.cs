using System;
using System.Xml;

namespace hds
{
	public class XmlParser
	{
		
		private XmlDocument  xDoc;
		
		public XmlParser ()
		{
			xDoc = new XmlDocument ();
		}
		
		public string[] loadDBParams(string fileName){
			xDoc.Load(fileName);
			XmlNodeList name = xDoc.GetElementsByTagName("DBConfig"); // Should be just 1 anyway
			
			XmlElement values = (XmlElement) name[0];
			
			XmlElement sHost = (XmlElement) values.GetElementsByTagName("serverHost")[0];
			XmlElement sPort = (XmlElement) values.GetElementsByTagName("serverPort")[0];
			XmlElement database = (XmlElement) values.GetElementsByTagName("databaseName")[0];
			XmlElement dbUser = (XmlElement) values.GetElementsByTagName("databaseUser")[0];
			XmlElement dbPass = (XmlElement) values.GetElementsByTagName("databasePassword")[0];
			XmlElement motd = (XmlElement) values.GetElementsByTagName("motd")[0];
			
			string[] result = new string[6];
			result[0] = sHost.InnerText;
			result[1] = sPort.InnerText;
			result[2] = database.InnerText;
			result[3] = dbUser.InnerText;
			result[4] = dbPass.InnerText;
			result[5] = motd.InnerText;
			
			return result;
		}
	
		public string[] loadServerConfig(string fileName){
			xDoc.Load(fileName);
			XmlNodeList name = xDoc.GetElementsByTagName("ServerConfig"); // Should be just 1 anyway
			
			XmlElement values = (XmlElement) name[0];
			
			XmlElement replayerOn = (XmlElement) values.GetElementsByTagName("replayerMode")[0];
			XmlElement adminConsoleOn = (XmlElement) values.GetElementsByTagName("adminConsoleEnabled")[0];
			XmlElement replayerFile = (XmlElement) values.GetElementsByTagName("replayerFile")[0];
			
			
			string[] result = new string[3];
			result[0] = replayerOn.InnerText;
			result[1] = adminConsoleOn.InnerText;
			result[2] = replayerFile.InnerText;
			
			
			
			return result;
		}
			

		
		
	}
}

