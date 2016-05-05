using System;
using System.Xml;

namespace hds{
	
    public class XmlParser{
	
		public static void loadDBParams(string fileName, out DbParams _params){

            _params = new DbParams();

            var xDoc = new XmlDocument();
            xDoc.Load(fileName);
			XmlNodeList name = xDoc.GetElementsByTagName("DBConfig"); // Should be just 1 anyway
			
			XmlElement values = (XmlElement) name[0];
			
			XmlElement sHost = (XmlElement) values.GetElementsByTagName("serverHost")[0];
			XmlElement sPort = (XmlElement) values.GetElementsByTagName("serverPort")[0];
			XmlElement database = (XmlElement) values.GetElementsByTagName("databaseName")[0];
			XmlElement dbUser = (XmlElement) values.GetElementsByTagName("databaseUser")[0];
			XmlElement dbPass = (XmlElement) values.GetElementsByTagName("databasePassword")[0];
			XmlElement motd = (XmlElement) values.GetElementsByTagName("motd")[0];
            XmlElement dbType = (XmlElement)values.GetElementsByTagName("dbType")[0];
			
            _params.Host = sHost.InnerText;
            _params.Port = int.Parse(sPort.InnerText);
            _params.DatabaseName = database.InnerText;
            _params.Username = dbUser.InnerText;
            _params.Password = dbPass.InnerText;
			_params.Motd = motd.InnerText;
            _params.DbType = dbType.InnerText.ToLower();

		}
	
		public static void loadServerParams(string fileName, out ServerParams _params){
            _params = new ServerParams();
            
            var xDoc = new XmlDocument();
            xDoc.Load(fileName);
			XmlNodeList name = xDoc.GetElementsByTagName("ServerConfig"); // Should be just 1 anyway			
			XmlElement values = (XmlElement) name[0];
			XmlElement adminConsoleOn = (XmlElement) values.GetElementsByTagName("adminConsoleEnabled")[0];
            XmlElement serverType = (XmlElement)values.GetElementsByTagName("ServerType")[0];
			
			_params.AdminConsoleEnabled = adminConsoleOn.InnerText.ToLower().Equals("on");
            _params.ServerType = serverType.InnerText.ToLower();
		}

	}
}

