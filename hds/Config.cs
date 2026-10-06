using System;
using System.Collections.Generic;
using System.Text;

namespace hds{
    
    public class ServerConfig{

        private string filename;

        private DbParams _dbParams;
        private ServerParams _serverParams;

        public DbParams dbParams { get { return _dbParams; } }
        public ServerParams serverParams { get { return _serverParams; } }


        public const string TemplateFileName = "ServerConfig.template.xml";

        public ServerConfig(string _filename){
            filename = _filename;
            EnsureConfigExists();
        }

        // Config.xml holds credentials and is not versioned - create it from the template on first start
        private void EnsureConfigExists(){
            if (!System.IO.File.Exists(filename) && System.IO.File.Exists(TemplateFileName)){
                System.IO.File.Copy(TemplateFileName, filename);
                Output.WriteLine("[Config] Created " + filename + " from " + TemplateFileName + " - please review the database settings.");
            }
        }

        public void LoadDbParams() {
            XmlParser.loadDBParams(filename, out _dbParams);
        }

        public void LoadServerParams() {
            XmlParser.loadServerParams(filename, out _serverParams);
        }
    
    }


    public class DbParams{

        public string Host { get; set; }
        public int Port {get; set;}
        public string DatabaseName {get;set;}
        public string Username {get;set;}
        public string Password {get;set;}
        public string Motd {get;set;}
        public string DbType {get;set;}

    }

    public class ServerParams{
        public bool AdminConsoleEnabled { get; set; }
        public string ServerType { get; set; }
    }
}
