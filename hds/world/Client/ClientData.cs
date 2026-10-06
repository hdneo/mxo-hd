using hds.world.Skill;
using System;
using System.Collections.Generic;

namespace hds
{
	public class ClientData
	{
		
		// Mxo coords
		private int[] rsiValues;
		
		// MXO Client values (server data)
		private string uniqueKey;
		private UInt32 charID;
		private UInt16 objectID;
        private UInt64 entityId;
		// Character Player Values
		private long exp;
		private long cash;
		private string district;
        private uint districtId;
		private bool onWorld;
		private string missionTeam;
		
		public List<BuffSkill> currentBuffs;
		public List<Mission> currentMissionList;
		
		private float clientSimTime;
        private bool UDPSessionEstablished;

        public UInt16 currentSelectedTargetViewId = 2;
        public ushort currentSelectedTargetSpawnId = 1; // we need both for combat

        // This vars needs to be refaktored / deleted (they are more for testing)
        public UInt16 currentTestRPC = 55;  // 0-127 = DONE, all 0x81XX -> done ToDo: 127-256 and 80XX(didnt found higher) Needs to be removed (hell the only way to store the current value) Start Point is : 32769 / Brek Point 1 : 32827 | Break Point 2: 33056 (need later for Subway)
        public UInt32 lastClickedObjectId = 0; // Temp for Hardline ObjectID Tracking - call upload hardline before teleporting so we know the last clicked object ID and save it (if not isset) to the Hardline list to assing it
        public UInt16 spawnViewUpdateCounter = 1; // maybe the wording is wrong - need to change this later
        private UInt16 jumpID = 1;
        public bool isJumping = false;

		public UInt32 jackoutStartTime;
		public bool isJackoutInProgress;
		
        public bool waitForRPCShutDown;
        internal ushort selfSpawnIdCounter;
	    public UInt32 lastSaveTime;

	    public ClientData ()
		{
			rsiValues = new int[22];
			onWorld = false;
			SetupPlayerData();

		}
	    
	    public bool DistrictIsLA(){
		    return (this.districtId == (int)MxOLocations.LA);
	    }

        public byte assignSpawnIdCounter()
        {
            byte temp = (byte)spawnViewUpdateCounter;
            spawnViewUpdateCounter++;
            if (spawnViewUpdateCounter == 256)
                spawnViewUpdateCounter = 1;
            return temp;
        }

        public void SetupPlayerData()
        {
            // This init buffs, current skills, missions etc.
            currentBuffs = new List<BuffSkill>();

        }
		
		
		public void setClientSimTime(float _newTime){
			clientSimTime = _newTime;
		}

        public void setJumpID(UInt16 jumpID)
        {
            this.jumpID = jumpID;
        }

        public UInt16 getJumpID()
        {
            return jumpID;
        }

        public void incrementJumpID()
        {
            jumpID++;
            //Output.WriteLine("JUMP ID IS NOW " + this.jumpID.ToString());
			if (jumpID==65534){ //Safe way
                jumpID = 1;
			}
        }
		
		public float getClientSimTime(){
			return clientSimTime;
		}
		
		
		public void setRsiValues(int[] newRSI){
			rsiValues = newRSI;
		}
		
		public int[] getRsiValues(){
			return rsiValues;
		}
		
		public void setOnWorld(bool newValue){
			onWorld = newValue;
		}		
		
		public bool getOnWorld(){
			return onWorld;
		}

        public void setMissionTeam(string newValue)
        {
            missionTeam = newValue;
            
        }

        public string getMissionTeam()
        {
            return missionTeam;
        }

        public void setUDPSessionEstablished(bool value)
        {
            UDPSessionEstablished = value;
        }

        public bool getUDPSEssionEstablished()
        {
            return UDPSessionEstablished;
        }

		public UInt32 getCharID(){
			return charID;
		}
		
		public void setCharID(UInt32 charID){
			this.charID = charID;
		}
		
		public UInt16 getObjectID(){
			return objectID;
		}
		
		public void setObjectID(UInt16 objectID){
			this.objectID = objectID;
		}

        public UInt64 getEntityId()
        {
            return entityId;
        }

        public void setEntityId(UInt64 _entityId)
        {
            entityId = _entityId;
        }
		
		public void setUniqueKey(string key){
			uniqueKey = key;
		}
		
		public string getUniqueKey(string key){
			return uniqueKey;
		}
		
		
		public void setExperience(long _exp){
			exp = _exp;
		}
		public long getExperience(){
			return exp;
		}
		
		public void setInfo(long _cash){
			cash = _cash;
		}
		
		public long getInfo(){
			return cash;
		}
		
		public void setDistrict(string _dist){
			district = _dist;
		}
		
		public string getDistrict(){
			return district;
		}

        public void setDistrictId(uint _districtId)
        {
            districtId = _districtId;
        }

        public uint getDistrictId()
        {
            return districtId;
        }
		

        public bool getRPCShutDown()
        {
            return waitForRPCShutDown;
        }

        public void setRPCShutDown(bool state)
        {
            waitForRPCShutDown = state;
        }
	}
}

