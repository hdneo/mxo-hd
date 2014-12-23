using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Xml;
using System.Linq;
using System.Data.SQLite;

using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;
using hds.shared;

namespace hds
{
    class DataLoader
    {
        private static DataLoader Instance;
        
        // Data Storage - maybe we need to change this to something better (like a AbilityQuery Object)
        public List<AbilityItem> AbilityDB = new List<AbilityItem>(); // holds our Ability DB
        public List<GameObjectItem> GODB = new List<GameObjectItem>(); // Holds the GameObjects
        public List<ClothingItem> ClothingRSIDB = new List<ClothingItem>();
        public List<StaticWorldObject> WorldObjectsDB = new List<StaticWorldObject>();
        public SQLiteConnection mxoSqliteDb = null;


        private DataLoader()
        {
            this.loadGODB("data\\gameobjects.csv");
            this.loadMobs("data\\mob.csv");
            this.loadAbilityDB("data\\abilityIDs.csv");
            this.loadClothingDB("data\\mxoClothing.csv");
            this.loadWorldObjectsDb("data\\staticObjects.csv");
            // Init our DB
            /*
            try
            {
                this.mxoSqliteDb = new SQLiteConnection("Data Source=data\\mxo_data.s3db");
                this.mxoSqliteDb.Open();
            }catch(SQLiteException ex){
                Output.WriteLine("Error opening SQLite DB " + ex.Message);
            }*/
            
        }


        public void loadMobs(string path)
        {
            // This is just an example of loading CSV    
            //return dataTable;
            Output.Write("Loading Hostile Mobs ..");
            ArrayList goDB = loadCSV(path, ';');
            int linecount = 1;
            foreach (string[] data in goDB)
            {

                //Output.WriteLine("Show Colums for Line : " + linecount.ToString() + " GOID:  " + data[1].ToString() + " Name " + data[0].ToString());
                if (linecount > 1)
                {
                    UInt64 currentEntityId = WorldSocket.entityIdCounter;
                    WorldSocket.entityIdCounter++;
                    uint rotation = 0;
                    if (data[10].ToString().Length > 0)
                    {
                        rotation = uint.Parse(data[10].ToString());
                    }
                    npc theMob = new npc();
                    theMob.setEntityId(currentEntityId);
                    theMob.setDistrict(Convert.ToUInt16(data[0].ToString()));
                    theMob.setDistrictName(data[1]);
                    theMob.setName(data[2].ToString());
                    theMob.setLevel(ushort.Parse(data[3].ToString()));
                    theMob.setHealthM(UInt16.Parse(data[4].ToString()));
                    theMob.setHealthC(UInt16.Parse(data[5].ToString()));
                    theMob.setMobId((ushort)linecount);
                    theMob.setRsiHex(data[6].ToString());
                    theMob.setXPos(double.Parse(data[7].ToString()));
                    theMob.setYPos(double.Parse(data[8].ToString()));
                    theMob.setZPos(double.Parse(data[9].ToString()));
                    theMob.xBase = double.Parse(data[7].ToString());
                    theMob.yBase = double.Parse(data[8].ToString());
                    theMob.zBase = double.Parse(data[9].ToString());
                    theMob.setRotation(rotation);
                    theMob.setIsDead(bool.Parse(data[11].ToString()));
                    theMob.setIsLootable(bool.Parse(data[12].ToString()));
                    WorldSocket.npcs.Add(theMob);
                }
                linecount++;
            }
        }

        public ArrayList loadCSV(string path, char seperator)
        {
            ArrayList rows = new ArrayList();
            StreamReader sr = new StreamReader(path);

            string line;
            int i = 0;
            while ((line = sr.ReadLine()) != null)
            {
                if (line.Length == 0) continue;
                string[] Colum = line.Split(new char[] { seperator });
                rows.Add(Colum);
                //Console.Write("\r{0}%   ", i);
                i++;
            }
            Output.Write(i.ToString() + " Entrys added !\n");
            // lets test a little bit to see what data is there
            
             
            
            return rows;
        }

        public void loadGODB(string path)
        {

            // This is just an example of loading CSV    
            //return dataTable;
            Output.Write("Loading GameObjects ..");
            ArrayList goDB = loadCSV(path,',');
            int linecount = 1;
            foreach (string[] data in goDB)
            {

                //Output.WriteLine("Show Colums for Line : " + linecount.ToString() + " GOID:  " + data[1].ToString() + " Name " + data[0].ToString());

                GameObjectItem goItem = new GameObjectItem();
                goItem.setGOID(Convert.ToInt32(data[1].ToString()));
                goItem.setName(data[0].ToString());
                this.GODB.Add(goItem);
                linecount++;
            }
            
        }

        public void loadAbilityDB(string path)
        {
            Output.Write("Loading Abilities..");

            // load the CSV into an ArrayList collection
            ArrayList abilityDB = loadCSV(path,',');

            int linecount = 1;

            // Create a new List 
            


            foreach (string[] data in abilityDB)
            {

                //Output.WriteLine("Show Colums for Line : " + linecount.ToString() + " Ability ID:  " + data[0].ToString() + " GOID " + data[1].ToString() + " Ability Name : " + data[2].ToString());
                AbilityItem ability = new AbilityItem();
                // we want to skip the first line as it should have the Names
                if (linecount > 1)
                {
                    ability.setAbilityID(Convert.ToUInt16(data[0].ToString()));
                    ability.setGOID(Convert.ToInt32(data[1].ToString()));
                    ability.setAbilityName(data[2].ToString());
                    this.AbilityDB.Add(ability);
                }
                

                linecount++;
            }
            
            
        }

        public void loadClothingDB(string path)
        {
            Output.Write("Loading Clothing..");

            ArrayList itemRSIMap = loadCSV(path,';');

            int linecount = 1;
            foreach (string[] data in itemRSIMap)
            {

                if (linecount > 1)
                {
                    //Output.WriteLine("Show Colums for Line : " + linecount.ToString() + " GOID:  " + data[1].ToString() + " Name " + data[0].ToString());

                    ClothingItem clothItem = new ClothingItem();

                    clothItem.setGoidDecimal(Convert.ToUInt16(data[0].ToString()));

                    // As loading the Endians doesnt work well and as we dont need them really just ignored it!
                    //clothItem.setGoidLittleEndian(Convert.ToByte(data[1].ToString()));
                    //clothItem.setGoidBigEndian(Convert.ToByte(data[2].ToString()));
                    clothItem.setClothesType(data[3].ToString());
                    clothItem.setShortName(data[4].ToString());
                    clothItem.setItemName(data[5].ToString());
                    clothItem.setModelId(Convert.ToUInt16(data[6].ToString()));
                    clothItem.setColorId(Convert.ToUInt16(data[7].ToString()));
                    this.ClothingRSIDB.Add(clothItem);

                }
                
                linecount++;
            }
        }

        public void loadWorldObjectsDb(string path)
        {
            Output.Write("Loading Static Objects for Slums,It and Downtown (could take a while)..");

            ArrayList staticWorldObjects = loadCSV(path, ',');
            StaticWorldObject worldObject = null;
            int linecount = 1;
            foreach (string[] data in staticWorldObjects)
            {

                if (linecount > 1)
                {
                    //Output.WriteLine("Show Colums for Line : " + linecount.ToString() + " GOID:  " + data[1].ToString() + " Name " + data[0].ToString());

                    worldObject = new StaticWorldObject();

                    worldObject.metrId = Convert.ToUInt16(data[0].ToString());
                    worldObject.sectorID = Convert.ToUInt16(data[1].ToString());
                    worldObject.sectorPath = data[2].ToString();

                    worldObject.mxoId = NumericalUtils.ByteArrayToUint32(StringUtils.hexStringToBytes(data[3].ToString()), 1);
                    worldObject.staticId = NumericalUtils.ByteArrayToUint32(StringUtils.hexStringToBytes(data[4].ToString()), 1);
                    worldObject.type = StringUtils.hexStringToBytes(data[5].ToString().Substring(0, 4));
                    worldObject.strType = data[6].ToString();
                    worldObject.exterior = Convert.ToBoolean(data[7].ToString());
                    worldObject.pos_x = double.Parse(data[8].ToString(), CultureInfo.InvariantCulture);
                    worldObject.pos_y = double.Parse(data[9].ToString(), CultureInfo.InvariantCulture);
                    worldObject.pos_z = double.Parse(data[10].ToString(), CultureInfo.InvariantCulture);
                    worldObject.rot = double.Parse(data[11].ToString(), CultureInfo.InvariantCulture);
                    worldObject.quat = data[12].ToString();

                    if (data[3].ToString() == "01003039")
                    {
                        Output.writeToLogForConsole("[DEMO DOOR] 01003039, X: " + worldObject.pos_x.ToString() + ", Y: " + worldObject.pos_y.ToString() + ", Z: " + worldObject.pos_z.ToString() + ", ROT: " + worldObject.rot.ToString() + ", TypeId: " + StringUtils.bytesToString_NS(worldObject.type));
                    }


                    this.WorldObjectsDB.Add(worldObject);
                    worldObject = null;

                }

                linecount++;
            }
        }


        public void loadVendorItems()
        {

        }


        public StaticWorldObject getObjectValues(UInt32 objectId)
        {
            Output.WriteLine("REQUEST OBJECT WITH ID :" + StringUtils.bytesToString_NS(NumericalUtils.uint32ToByteArray(objectId, 0)));
            SQLiteCommand cmd = new SQLiteCommand("select * from world_objects where mxoId='" + StringUtils.bytesToString_NS(NumericalUtils.uint32ToByteArray(objectId,0)) + "' LIMIT 1", this.mxoSqliteDb);
            SQLiteDataReader reader = cmd.ExecuteReader();
            StaticWorldObject worldObject = null;

            if (reader.HasRows)
                while (reader.Read())
                {
                    worldObject = new StaticWorldObject();

                    worldObject.metrId = (UInt16)reader.GetInt32(reader.GetOrdinal("metr_id"));
                    worldObject.sectorID = (UInt16)reader.GetInt32(reader.GetOrdinal("sector_id"));
                    worldObject.sectorPath = reader.GetString(reader.GetOrdinal("path"));

                    worldObject.mxoId = NumericalUtils.ByteArrayToUint32(StringUtils.hexStringToBytes(reader.GetString(reader.GetOrdinal("mxoId"))), 1);
                    worldObject.staticId = NumericalUtils.ByteArrayToUint32(StringUtils.hexStringToBytes(reader.GetString(reader.GetOrdinal("staticId"))), 1);
                    worldObject.type = StringUtils.hexStringToBytes(reader.GetString(reader.GetOrdinal("type")).Substring(0, 4));
                    worldObject.strType = reader.GetString(reader.GetOrdinal("strType"));
                    worldObject.exterior = reader.GetBoolean(reader.GetOrdinal("exterior"));
                    worldObject.pos_x = reader.GetDouble(reader.GetOrdinal("pos_x"));
                    worldObject.pos_y = reader.GetDouble(reader.GetOrdinal("pos_y"));
                    worldObject.pos_z = reader.GetDouble(reader.GetOrdinal("pos_z"));
                    worldObject.rot = reader.GetDouble(reader.GetOrdinal("rotation"));
                    worldObject.quat = reader.GetString(reader.GetOrdinal("quat"));
                }

            /*
            StaticWorldObject theObject = WorldObjectsDB.Find(delegate(StaticWorldObject temp) { return temp.mxoId == objectId; });
            if (theObject == null)
            {
                StaticWorldObject nullObject = new StaticWorldObject();
                return nullObject;

            }*/

            return worldObject;
        }

        public string getAbilityNameByID(UInt16 id)
        {
            AbilityItem abTemp = AbilityDB.Find(delegate(AbilityItem a) { return a.getAbilityID() == id; });
            return abTemp.getAbilityName();
           
        }

        public ClothingItem getItemValues(UInt16 itemID)
        {
            ClothingItem clothingTemp = ClothingRSIDB.Find(delegate(ClothingItem temp) { return temp.getGoidDecimal() == itemID; });
            if (clothingTemp == null)
            {
                ClothingItem emptyItem = new ClothingItem();
                return emptyItem;

            }
            return clothingTemp;
        }

        // SingleTon Pattern - as we only want to load the Data once at startup and manage 

        public static DataLoader getInstance()
        {
            if (Instance == null)
            {
                Instance = new DataLoader();
            }
            return Instance;
        }
    }

}
