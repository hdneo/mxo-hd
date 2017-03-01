using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;

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
        public List<EmoteItem> Emotes = new List<EmoteItem>();
        public List<NewRSIItem> newRSIItemsDb = new List<NewRSIItem>();
        public SQLiteConnection mxoSqliteDb = null;

        public bool useSQLiteDatabase = false;


        private DataLoader()
        {
            loadEmotes();
            loadNewRSIIDs("data\\newrsiIDs.csv");
            loadGODB("data\\gameobjects.csv");
            
            
            loadMobs("data\\mob.csv");
            loadMobs("data\\mob_parsed.csv");
            
            
            loadMobs("data\\mob_just_one.csv");
            loadAbilityDB("data\\abilityIDs.csv");
            loadClothingDB("data\\mxoClothing.csv");
            
            
            // Disabled for Debugging
            /*
            this.loadWorldObjectsDb("data\\staticObjects_slums.csv");
            this.loadWorldObjectsDb("data\\staticObjects_it.csv");
            this.loadWorldObjectsDb("data\\staticObjects_dt.csv");
            */
            
            
        }

        public void loadNewRSIIDs(string path)
        {
            Output.Write("Loading RSI IDs for Character Creation ..");
            ArrayList newRSIDB = loadCSV(path, ',');
            int linecount = 1;
            foreach (string[] data in newRSIDB) { 
                if (linecount > 1)
                {
                    NewRSIItem item = new NewRSIItem();
                    item.name = data[0];
                    item.newRSIID = UInt16.Parse(data[1]);
                    item.internalId = UInt16.Parse(data[2]);
                    item.type = data[3];
                    item.gender = ushort.Parse(data[4]);
                    newRSIItemsDb.Add(item);
                }
                linecount++;
            }
        }

        public NewRSIItem getNewRSIItemByTypeAndID(string type, UInt16 newRSIID)
        {
            NewRSIItem newrsiItemTemp = newRSIItemsDb.Find(delegate (NewRSIItem temp) { return temp.newRSIID == newRSIID && temp.type==type; });
            if (newrsiItemTemp == null)
            {
                NewRSIItem emptyItem = new NewRSIItem();
                return emptyItem;

            }
            return newrsiItemTemp;
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
                    if (data[10].Length > 0)
                    {
                        rotation = uint.Parse(data[10]);
                    }

                  
                    if (data[4].Length > 0 && data[5].Length > 0)
                    {
                        npc theMob = new npc();
                        theMob.setEntityId(currentEntityId);
                        theMob.setDistrict(Convert.ToUInt16(data[0]));
                        theMob.setDistrictName(data[1]);
                        theMob.setName(data[2]);
                        theMob.setLevel(ushort.Parse(data[3]));
                        theMob.setHealthM(UInt16.Parse(data[4]));
                        theMob.setHealthC(UInt16.Parse(data[5]));
                        theMob.setMobId((ushort)linecount);
                        theMob.setRsiHex(data[6]);
                        theMob.setXPos(double.Parse(data[7]));
                        theMob.setYPos(double.Parse(data[8]));
                        theMob.setZPos(double.Parse(data[9]));
                        theMob.xBase = double.Parse(data[7]);
                        theMob.yBase = double.Parse(data[8]);
                        theMob.zBase = double.Parse(data[9]);
                        theMob.setRotation(rotation);
                        theMob.setIsDead(bool.Parse(data[11]));
                        theMob.setIsLootable(bool.Parse(data[12]));
                        WorldSocket.npcs.Add(theMob);
                        WorldSocket.gameServerEntities.Add(theMob);
                    }
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
                string[] Colum = line.Split(seperator);
                rows.Add(Colum);
                //Console.Write("\r{0}%   ", i);
                i++;
            }
            Output.Write(i + " Entrys added !\n");
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
                goItem.setGOID(Convert.ToInt32(data[1]));
                goItem.setName(data[0]);
                GODB.Add(goItem);
                linecount++;
            }
            
        }

        public void loadAbilityDB(string path)
        {
            Output.Write("Loading Abilities..");

            // load the CSV into an ArrayList collection
            ArrayList abilityDB = loadCSV(path,';');

            int linecount = 1;

            // Create a new List 
            


            foreach (string[] data in abilityDB)
            {

                //Output.WriteLine("Show Colums for Line : " + linecount.ToString() + " Ability ID:  " + data[0].ToString() + " GOID " + data[1].ToString() + " Ability Name : " + data[2].ToString());
                AbilityItem ability = new AbilityItem();
                // we want to skip the first line as it should have the Names
                if (linecount > 1)
                {
                    ability.setAbilityID(Convert.ToUInt16(data[0]));
                    ability.setGOID(Convert.ToInt32(data[1]));
                    ability.setAbilityName(data[2]);
                    if (data[3].Length > 0)
                    {
                        ability.setIsCastable(bool.Parse(data[3]));
                    }

                    if (data.Length >= 5)
                    {
                        if (data[4].Length > 0)
                        {
                            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                            ci.NumberFormat.CurrencyDecimalSeparator = ".";
                            ability.setCastingTime(float.Parse(data[4], NumberStyles.Any, ci));
                        }
                    }

                    if (data.Length >= 6)
                    {
                        if (data[5].Length > 2)
                        {
                            ability.setCastAnimStart(StringUtils.hexStringToBytes(data[5]));
                        }
                    }

                    if (data.Length >= 7)
                    {
                        if (data[6].Length > 2)
                        {
                            ability.setCastAnimMid(StringUtils.hexStringToBytes((data[6])));
                        }
                    }

                    if (data.Length >= 8)
                    {
                        if (data[7].Length > 2)
                        {
                            ability.setCastAnimEnd(StringUtils.hexStringToBytes(data[7]));
                        }
                    }

                    if (data.Length >= 9)
                    {
                        if (data[8].Length > 0)
                        {
                            ability.setActivationFX(UInt32.Parse(data[8]));
                        }
                    }

                    if (data.Length >= 10)
                    {
                        if (data[9].Length > 0)
                        {
                            ability.setValueFrom(Int16.Parse(data[9]));
                        }
                    }

                    if (data.Length >= 11)
                    {
                        if (data[10].Length > 0)
                        {
                            ability.setValueTo(UInt16.Parse(data[10]));
                        }
                    }

                    if (data.Length >= 12)
                    {
                        if (data[11].Length > 0)
                        {
                            ability.setIsBuff(bool.Parse(data[11]));
                        }
                    }

                    if (data.Length >= 13)
                    {
                        if (data[12].Length > 0)
                        {
                            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                            ci.NumberFormat.CurrencyDecimalSeparator = ".";
                            ability.setBuffTime(float.Parse(data[12], NumberStyles.Any, ci));
                        }
                    }

                    if (data.Length >= 14)
                    {
                        if (data[13].Length > 0)
                        {
                            ability.setAbilityExecutionFX(UInt32.Parse(data[13]));
                        }
                    }

                    AbilityDB.Add(ability);
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

                    clothItem.setGoidDecimal(Convert.ToUInt16(data[0]));

                    // As loading the Endians doesnt work well and as we dont need them really just ignored it!
                    //clothItem.setGoidLittleEndian(Convert.ToByte(data[1].ToString()));
                    //clothItem.setGoidBigEndian(Convert.ToByte(data[2].ToString()));
                    clothItem.setClothesType(data[3]);
                    clothItem.setShortName(data[4]);
                    clothItem.setItemName(data[5]);
                    clothItem.setModelId(Convert.ToUInt16(data[6]));
                    clothItem.setColorId(Convert.ToUInt16(data[7]));
                    ClothingRSIDB.Add(clothItem);

                }
                
                linecount++;
            }
        }

        public void loadWorldObjectsDb(string path)
        {
            Output.Write("Loading Static Objects from " + path + " - please wait...");

            ArrayList staticWorldObjects = loadCSV(path, ',');
            StaticWorldObject worldObject = null;
            int linecount = 1;
            foreach (string[] data in staticWorldObjects)
            {

                if (linecount > 1)
                {
                    //Output.WriteLine("Show Colums for Line : " + linecount.ToString() + " GOID:  " + data[1].ToString() + " Name " + data[0].ToString());

                    worldObject = new StaticWorldObject();

                    worldObject.metrId = Convert.ToUInt16(data[0]);
                    worldObject.sectorID = Convert.ToUInt16(data[1]);
                    

                    worldObject.mxoId = NumericalUtils.ByteArrayToUint32(StringUtils.hexStringToBytes(data[2]), 1);
                    worldObject.staticId = NumericalUtils.ByteArrayToUint32(StringUtils.hexStringToBytes(data[3]), 1);
                    worldObject.type = StringUtils.hexStringToBytes(data[4].Substring(0, 4));
                    worldObject.exterior = Convert.ToBoolean(data[5]);
                    worldObject.pos_x = double.Parse(data[6], CultureInfo.InvariantCulture);
                    worldObject.pos_y = double.Parse(data[7], CultureInfo.InvariantCulture);
                    worldObject.pos_z = double.Parse(data[8], CultureInfo.InvariantCulture);
                    worldObject.rot = double.Parse(data[9], CultureInfo.InvariantCulture);
                    worldObject.quat = data[10];

                    if (data[3] == "01003039")
                    {
                        Output.writeToLogForConsole("[DEMO DOOR] 01003039, X: " + worldObject.pos_x + ", Y: " + worldObject.pos_y + ", Z: " + worldObject.pos_z + ", ROT: " + worldObject.rot + ", TypeId: " + StringUtils.bytesToString_NS(worldObject.type));
                    }


                    WorldObjectsDB.Add(worldObject);
                    worldObject = null;

                }

                linecount++;
            }
        }


        public void loadVendorItems()
        {

        }

        public void loadEmotes()
        {
            Emotes.Add(new EmoteItem(0xe6020058, 0x01)); // /beckon
            Emotes.Add(new EmoteItem(0x9b010058, 0x02)); // /bigwave
            Emotes.Add(new EmoteItem(0xe7020058, 0x03)); // /bow
            Emotes.Add(new EmoteItem(0x0d00003a, 0x04)); // /clap
            Emotes.Add(new EmoteItem(0x1000003a, 0x05)); // /crossarms
            Emotes.Add(new EmoteItem(0x1100003a, 0x06)); // /nod
            Emotes.Add(new EmoteItem(0x570d0058, 0x07)); // /agree
            Emotes.Add(new EmoteItem(0x590d0058, 0x08)); // /yes
            Emotes.Add(new EmoteItem(0x1600003a, 0x09)); // /orangutan
            Emotes.Add(new EmoteItem(0x10050004, 0x0a)); // /point
            Emotes.Add(new EmoteItem(0xd1020058, 0x0b)); // /pointback
            Emotes.Add(new EmoteItem(0x1300003a, 0x0c)); // /pointleft
            Emotes.Add(new EmoteItem(0x1400003a, 0x0d)); // /pointright
            Emotes.Add(new EmoteItem(0xf50c0058, 0x0e)); // /pointup
            Emotes.Add(new EmoteItem(0xfa0c0058, 0x0f)); // /pointdown
            Emotes.Add(new EmoteItem(0x0e00003a, 0x10)); // /salute
            Emotes.Add(new EmoteItem(0x1200003a, 0x11)); // /shakehead
            Emotes.Add(new EmoteItem(0x580d0058, 0x12)); // /disagree
            Emotes.Add(new EmoteItem(0x0d00003e, 0x13)); // /no
            Emotes.Add(new EmoteItem(0xe9020058, 0x14)); // /stomp
            Emotes.Add(new EmoteItem(0x0f00003a, 0x15)); // /tapfoot
            Emotes.Add(new EmoteItem(0x0c00003a, 0x16)); // /wave
            Emotes.Add(new EmoteItem(0xef0c0058, 0x17)); // /dangerarea
            Emotes.Add(new EmoteItem(0x130e0058, 0x18)); // /comeforward
            Emotes.Add(new EmoteItem(0xf00c0058, 0x19)); // /enemyinsight
            Emotes.Add(new EmoteItem(0xf00c0058, 0x1a)); // /enemy
            Emotes.Add(new EmoteItem(0xf20c0058, 0x1b)); // /disperse
            Emotes.Add(new EmoteItem(0x1500003a, 0x1c)); // /lookaround
            Emotes.Add(new EmoteItem(0xf60c0058, 0x1d)); // /takecover
            Emotes.Add(new EmoteItem(0xf70c0058, 0x1e)); // /cover
            Emotes.Add(new EmoteItem(0xf30c0058, 0x1f)); // /mapcheck
            Emotes.Add(new EmoteItem(0xf80c0058, 0x20)); // /onehandedhandstand
            Emotes.Add(new EmoteItem(0xfb0c0058, 0x21)); // /giggle
            Emotes.Add(new EmoteItem(0xf40c0058, 0x22)); // /handstand
            Emotes.Add(new EmoteItem(0xfc0c0058, 0x23)); // /hearnoevil
            Emotes.Add(new EmoteItem(0xfd0c0058, 0x24)); // /seenoevil
            Emotes.Add(new EmoteItem(0xfe0c0058, 0x25)); // /speaknoevil
            Emotes.Add(new EmoteItem(0x000d0058, 0x26)); // /coverears
            Emotes.Add(new EmoteItem(0x010d0058, 0x27)); // /covermouth
            Emotes.Add(new EmoteItem(0xff0c0058, 0x28)); // /covereyes
            Emotes.Add(new EmoteItem(0x130d0058, 0x29)); // /blowkiss
            Emotes.Add(new EmoteItem(0x1f0d0058, 0x2a)); // /blush
            Emotes.Add(new EmoteItem(0x270d0058, 0x2b)); // /cheer
            Emotes.Add(new EmoteItem(0x650d0058, 0x2c)); // /crackknuckles
            Emotes.Add(new EmoteItem(0x650d0058, 0x2d)); // /crackknuckles
            Emotes.Add(new EmoteItem(0x7a0d0058, 0x2e)); // /cry
            Emotes.Add(new EmoteItem(0x7b0d0058, 0x2f)); // /curtsey
            Emotes.Add(new EmoteItem(0x7c0d0058, 0x30)); // /formalbow
            Emotes.Add(new EmoteItem(0x7d0d0058, 0x31)); // /formalcurtsey
            Emotes.Add(new EmoteItem(0x810d0058, 0x32)); // /bowhead
            Emotes.Add(new EmoteItem(0x810d0058, 0x33)); // /bowhead
            Emotes.Add(new EmoteItem(0x7e0d0058, 0x34)); // /insult
            Emotes.Add(new EmoteItem(0xa9150058, 0x35)); // /scream
            Emotes.Add(new EmoteItem(0xa5150058, 0x36)); // /anguish
            Emotes.Add(new EmoteItem(0x820d0058, 0x37)); // /karatepower
            Emotes.Add(new EmoteItem(0xb90d0058, 0x38)); // /karatepower2
            Emotes.Add(new EmoteItem(0xba0d0058, 0x39)); // /karatepower3
            Emotes.Add(new EmoteItem(0x830d0058, 0x3a)); // /karatespeed
            Emotes.Add(new EmoteItem(0x840d0058, 0x3b)); // /karatespeed2
            Emotes.Add(new EmoteItem(0x850d0058, 0x3c)); // /karatespeed3
            Emotes.Add(new EmoteItem(0xbb0d0058, 0x3d)); // /karatedefense
            Emotes.Add(new EmoteItem(0xbc0d0058, 0x3e)); // /karatedefense2
            Emotes.Add(new EmoteItem(0xbd0d0058, 0x3f)); // /karatedefense3
            Emotes.Add(new EmoteItem(0x870d0058, 0x40)); // /kneel
            Emotes.Add(new EmoteItem(0x880d0058, 0x41)); // /takeaknee
            Emotes.Add(new EmoteItem(0x890d0058, 0x42)); // /kungfu
            Emotes.Add(new EmoteItem(0x890d0058, 0x43)); // /kungfu
            Emotes.Add(new EmoteItem(0x890d0058, 0x44)); // /kungfu
            Emotes.Add(new EmoteItem(0x890d0058, 0x45)); // /kungfu
            Emotes.Add(new EmoteItem(0x890d0058, 0x46)); // /kungfu
            Emotes.Add(new EmoteItem(0x890d0058, 0x47)); // /kungfu
            Emotes.Add(new EmoteItem(0x890d0058, 0x48)); // /kungfu
            Emotes.Add(new EmoteItem(0x890d0058, 0x49)); // /kungfu
            Emotes.Add(new EmoteItem(0x890d0058, 0x4a)); // /kungfu
            Emotes.Add(new EmoteItem(0xbe0d0058, 0x4b)); // /aikido
            Emotes.Add(new EmoteItem(0xbe0d0058, 0x4c)); // /aikido
            Emotes.Add(new EmoteItem(0xbe0d0058, 0x4d)); // /aikido
            Emotes.Add(new EmoteItem(0xbe0d0058, 0x4e)); // /aikido
            Emotes.Add(new EmoteItem(0xbe0d0058, 0x4f)); // /aikido
            Emotes.Add(new EmoteItem(0xbe0d0058, 0x50)); // /aikido
            Emotes.Add(new EmoteItem(0xbe0d0058, 0x51)); // /aikido
            Emotes.Add(new EmoteItem(0xbe0d0058, 0x52)); // /aikido
            Emotes.Add(new EmoteItem(0xbe0d0058, 0x53)); // /aikido
            Emotes.Add(new EmoteItem(0x920d0058, 0x54)); // /laugh
            Emotes.Add(new EmoteItem(0x7f0d0058, 0x55)); // /rude
            Emotes.Add(new EmoteItem(0xa40d0058, 0x56)); // /loser
            Emotes.Add(new EmoteItem(0x010000b4, 0x57)); // /bigtrouble
            Emotes.Add(new EmoteItem(0x940d0058, 0x58)); // /okay
            Emotes.Add(new EmoteItem(0x940d0058, 0x59)); // /ok
            Emotes.Add(new EmoteItem(0x960d0058, 0x5a)); // /peace
            Emotes.Add(new EmoteItem(0x770d0058, 0x5b)); // /pullhair
            Emotes.Add(new EmoteItem(0x970d0058, 0x5c)); // /rolldice
            Emotes.Add(new EmoteItem(0x780d0058, 0x5d)); // /sarcasticclap
            Emotes.Add(new EmoteItem(0x790d0058, 0x5e)); // /golfclap
            Emotes.Add(new EmoteItem(0x980d0058, 0x5f)); // /scratchhead
            Emotes.Add(new EmoteItem(0x990d0058, 0x60)); // /shrug
            Emotes.Add(new EmoteItem(0x9a0d0058, 0x62)); // /stretch
            Emotes.Add(new EmoteItem(0xf90c0058, 0x63)); // /suckitdown
            Emotes.Add(new EmoteItem(0x9b0d0058, 0x64)); // /surrender
            Emotes.Add(new EmoteItem(0x9c0d0058, 0x65)); // /thumbsup
            Emotes.Add(new EmoteItem(0x1200003a, 0x66)); // /shakehead
            Emotes.Add(new EmoteItem(0xa20d0058, 0x67)); // /shameshame
            Emotes.Add(new EmoteItem(0x9d0d0058, 0x68)); // /twothumbsup
            Emotes.Add(new EmoteItem(0x9f0d0058, 0x69)); // /puke
            Emotes.Add(new EmoteItem(0x9e0d0058, 0x6a)); // /vomit
            Emotes.Add(new EmoteItem(0xa00d0058, 0x6b)); // /whistle
            Emotes.Add(new EmoteItem(0xa60d0058, 0x6c)); // /grovel
            Emotes.Add(new EmoteItem(0xe1020058, 0x6d)); // /yawn
            Emotes.Add(new EmoteItem(0xa70d0058, 0x6e)); // /plead
            Emotes.Add(new EmoteItem(0xa50d0058, 0x6f)); // /shakefist
            Emotes.Add(new EmoteItem(0x4e0200b4, 0x70)); // /cool
            Emotes.Add(new EmoteItem(0xb70d0058, 0x71)); // /crackneck
            Emotes.Add(new EmoteItem(0x150e0058, 0x72)); // /assemble
            Emotes.Add(new EmoteItem(0x880200b4, 0x73)); // /mockcry
            Emotes.Add(new EmoteItem(0xa70200b4, 0x74)); // /throat
            Emotes.Add(new EmoteItem(0x9b0200b4, 0x75)); // /powerpose
            Emotes.Add(new EmoteItem(0xbf0200b4, 0x76)); // /thumbsdown
            Emotes.Add(new EmoteItem(0xcc0200b4, 0x77)); // /twothumbsdown
            Emotes.Add(new EmoteItem(0x250e0058, 0x78)); // /taunt
            Emotes.Add(new EmoteItem(0x1f0e0058, 0x79)); // /moveout
            Emotes.Add(new EmoteItem(0x1f0e0058, 0x7a)); // /move
            Emotes.Add(new EmoteItem(0x1d0e0058, 0x7b)); // /iamready
            Emotes.Add(new EmoteItem(0x1b0e0058, 0x7c)); // /rdy
            Emotes.Add(new EmoteItem(0x190e0058, 0x7d)); // /ready
            Emotes.Add(new EmoteItem(0x0d000080, 0x7e)); // /stop
            Emotes.Add(new EmoteItem(0x0d0e0058, 0x7f)); // /bigcheer
            Emotes.Add(new EmoteItem(0xd80200b4, 0x80)); // /whoa
            Emotes.Add(new EmoteItem(0xd5140058, 0x81)); // /talkrelieved
            Emotes.Add(new EmoteItem(0x17140058, 0x82)); // /talk1
            Emotes.Add(new EmoteItem(0x19140058, 0x83)); // /talk2
            Emotes.Add(new EmoteItem(0x1b140058, 0x84)); // /talk3
            Emotes.Add(new EmoteItem(0x1d140058, 0x85)); // /talkangry
            Emotes.Add(new EmoteItem(0x21140058, 0x86)); // /talkforceful
            Emotes.Add(new EmoteItem(0x1f140058, 0x87)); // /talkexcited
            Emotes.Add(new EmoteItem(0x23140058, 0x88)); // /talkscared
            Emotes.Add(new EmoteItem(0xd1140058, 0x89)); // /talkchuckle
            Emotes.Add(new EmoteItem(0xd3140058, 0x8a)); // /talkhurt
            Emotes.Add(new EmoteItem(0xd5140058, 0x8b)); // /talkrelieved
            Emotes.Add(new EmoteItem(0xe3150058, 0x8c)); // /talknegative
            Emotes.Add(new EmoteItem(0xdd150058, 0x8d)); // /talkpuzzled
            Emotes.Add(new EmoteItem(0xd9140058, 0x8e)); // /talkwhisperobvious
            Emotes.Add(new EmoteItem(0xdb150058, 0x8f)); // /talkgroup
            Emotes.Add(new EmoteItem(0xd7150058, 0x90)); // /talkflirtatious
            Emotes.Add(new EmoteItem(0xd9150058, 0x91)); // /talkaffirmative
            Emotes.Add(new EmoteItem(0xe1140058, 0x92)); // /overheat
            Emotes.Add(new EmoteItem(0xdd140058, 0x93)); // /thewave
            Emotes.Add(new EmoteItem(0xdb140058, 0x94)); // /snake
            Emotes.Add(new EmoteItem(0xdf140058, 0x95)); // /tsuj
            Emotes.Add(new EmoteItem(0x27140058, 0x96)); // /touchearpiece
            Emotes.Add(new EmoteItem(0xe1140058, 0x97)); // /overheat
            Emotes.Add(new EmoteItem(0x51160058, 0x98)); // /backflop
            Emotes.Add(new EmoteItem(0x55160058, 0x99)); // /backflop1
            Emotes.Add(new EmoteItem(0x53160058, 0x9a)); // /backflop2
            Emotes.Add(new EmoteItem(0x57160058, 0x9b)); // /ballet
            Emotes.Add(new EmoteItem(0x59160058, 0x9c)); // /bang
            Emotes.Add(new EmoteItem(0x5b160058, 0x9d)); // /cutitout
            Emotes.Add(new EmoteItem(0x5d160058, 0x9e)); // /giddyup
            Emotes.Add(new EmoteItem(0x5f160058, 0x9f)); // /horns
            Emotes.Add(new EmoteItem(0x61160058, 0xa0)); // /mimewall
            Emotes.Add(new EmoteItem(0x63160058, 0xa1)); // /mimeelbow
            Emotes.Add(new EmoteItem(0x67160058, 0xa2)); // /mimerope
            Emotes.Add(new EmoteItem(0x65160058, 0xa3)); // /picknose
            Emotes.Add(new EmoteItem(0x6b160058, 0xa4)); // /duh
            Emotes.Add(new EmoteItem(0x6d160058, 0xa5)); // /timeout
            Emotes.Add(new EmoteItem(0x6f160058, 0xa6)); // /whichway
            Emotes.Add(new EmoteItem(0x820200b4, 0xa9)); // /kickdoor
            Emotes.Add(new EmoteItem(0x710200b4, 0xaa)); // /examine
            Emotes.Add(new EmoteItem(0x8e0200b4, 0xac)); // /pickup
            Emotes.Add(new EmoteItem(0xb30200b4, 0xad)); // /takepill
            Emotes.Add(new EmoteItem(0x0b0000e0, 0xaf)); // /cough
            Emotes.Add(new EmoteItem(0xa10200b4, 0xb0)); // /righton
            Emotes.Add(new EmoteItem(0x0c0000e0, 0xb1)); // /sleep
            Emotes.Add(new EmoteItem(0xea020058, 0xb2)); // /tiphat
            Emotes.Add(new EmoteItem(0xa10000b4, 0xb3)); // /confused
            Emotes.Add(new EmoteItem(0x000d0058, 0xb4)); // /coverears
            Emotes.Add(new EmoteItem(0x760200b4, 0xb5)); // /eyedrops
            Emotes.Add(new EmoteItem(0x8e0200b4, 0xb6)); // /pickup
            Emotes.Add(new EmoteItem(0x0e050004, 0xba)); // /talkdepressed
            Emotes.Add(new EmoteItem(0x0d050004, 0xbb)); // /throw
            Emotes.Add(new EmoteItem(0xc50200b4, 0xbc)); // /toss
            Emotes.Add(new EmoteItem(0x4b00003a, 0xbe)); // /shakehands
            Emotes.Add(new EmoteItem(0xe4020058, 0xc1)); // /slap
            Emotes.Add(new EmoteItem(0xab0d0058, 0xc7)); // /dogsniff
            Emotes.Add(new EmoteItem(0xfd0d0058, 0xca)); // /hug
            Emotes.Add(new EmoteItem(0xca0d0058, 0xcd)); // /weddingkiss
            Emotes.Add(new EmoteItem(0xff0d0058, 0xd0)); // /holdbothhands
            Emotes.Add(new EmoteItem(0xfb0d0058, 0xd3)); // /kissthering
            Emotes.Add(new EmoteItem(0xf50d0058, 0xd6)); // /manhug
            Emotes.Add(new EmoteItem(0x050e0058, 0xd9)); // /pound
            Emotes.Add(new EmoteItem(0x030e0058, 0xdc)); // /weddingring
            Emotes.Add(new EmoteItem(0x010e0058, 0xdf)); // /propose
            Emotes.Add(new EmoteItem(0xc5140058, 0xe2)); // /dap
            Emotes.Add(new EmoteItem(0xfb0d0058, 0xe5)); // /kiss
            Emotes.Add(new EmoteItem(0x05160058, 0xe8)); // /weddingcake
            Emotes.Add(new EmoteItem(0x59160058, 0xee)); // /bangbang
            Emotes.Add(new EmoteItem(0x050000e0, 0xa8)); // /leanwallloop
        }

        public byte findEmoteByLongId(UInt32 _emoteLongId)
        {
            byte emoteIdShort = 0x00;
            EmoteItem emoteItem = Emotes.Find(delegate (EmoteItem temp) { return temp.emoteIDLong == _emoteLongId; });
            if(emoteItem!= null)
            {
                emoteIdShort = emoteItem.emoteShortID;
            }
            return emoteIdShort;
        }


        public StaticWorldObject getObjectValues(UInt32 objectId)
        {
            Output.WriteLine("REQUEST OBJECT WITH ID :" + StringUtils.bytesToString_NS(NumericalUtils.uint32ToByteArray(objectId, 0)));
            StaticWorldObject worldObject = null;

            worldObject = WorldObjectsDB.Find(delegate(StaticWorldObject temp) { return temp.mxoId == objectId; });
            if (worldObject == null)
            {
                worldObject = new StaticWorldObject();
            }

            return worldObject;
        }

        public AbilityItem getAbilityByID(UInt16 id)
        {
            AbilityItem abTemp = AbilityDB.Find(delegate(AbilityItem a) { return a.getAbilityID() == id; });
            return abTemp;
           
        }

        public GameObjectItem getGameObjectItemById(UInt16 id)
        {
            GameObjectItem goItem =  GODB.Find(delegate(GameObjectItem a) { return a.getGOID() == id; });
            return goItem;

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
