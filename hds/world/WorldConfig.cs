using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace hds
{
    public class WorldConfig
    {
        private string filename;
        
        public string serverName;
        public string weather = "bluesky2";
        public bool IsPvpServer = false;
        public UInt16 PvpMaxSafeLevel = 16;
        public UInt16 FixedBinkIDOverride = 0;
        
        public Hashtable events = new Hashtable();

        public WorldConfig(string filename)
        {
            this.filename = filename;
            var xDoc = new XmlDocument();
            xDoc.Load(filename);
            
            serverName = xDoc.GetElementsByTagName("serverName")[0].InnerText;
            IsPvpServer = Boolean.Parse(xDoc.GetElementsByTagName("IsPvPServer")[0].InnerText);
            PvpMaxSafeLevel = UInt16.Parse(xDoc.GetElementsByTagName("PvPMaxSafeLevel")[0].InnerText);
            FixedBinkIDOverride = UInt16.Parse(xDoc.GetElementsByTagName("FixedBinkIDOverride")[0].InnerText);

            XmlNodeList eventList = xDoc.GetElementsByTagName("WorldEvents");
            foreach (XmlNode eventNode in eventList[0].ChildNodes)
            {
                if (eventNode.NodeType != XmlNodeType.Comment)
                {
                    events.Add(eventNode.Name, eventNode.InnerText);    
                }
                
            }

        }
        
    }
}