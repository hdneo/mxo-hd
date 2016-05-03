using System;
using System.Collections;
using System.Collections.Generic;
namespace hds
{
	public class ClientData
	{
		
		// Mxo coords
		
		private int[] rsiValues;
		
		// MXO Client values (server data)
		private string uniqueKey;
		private UInt16 pss;
		private UInt16 cseq;
		private UInt16 sseq = 0;
		private UInt16 sseqACKBYCLIENT;
		private UInt16 RPCCounter;
		private UInt32 charID;
		private UInt16 objectID;
        private UInt64 entityId;
		private long exp;
		private long cash;
		private string district;
        private uint districtId = 0;
		private bool onWorld;
		private string missionTeam;
        public List<BuffSkill> currentBuffs;
		private float clientSimTime;
        public int lastSimTimeSEQ = 0; // Add timed packet every 4 SEQ automatically
        public long lastSimTimeUpdate = 0; // Timer
        private bool UDPSessionEstablished = false;

        public UInt16 currentSelectedTargetViewId = 2;
        public ushort currentSelectedTargetSpawnId = 1; // we need both for combat

        // This vars needs to be refaktored / deleted (they are more for testing)
        public UInt16 currentTestRPC = 33015;  // 0-127 = DONE, all 0x81XX -> done ToDo: 127-256 and 80XX(didnt found higher) Needs to be removed (hell the only way to store the current value) Start Point is : 32769 / Brek Point 1 : 32827 | Break Point 2: 33056 (need later for Subway)
        public UInt32 lastClickedObjectId = 0; // Temp for Hardline ObjectID Tracking - call upload hardline before teleporting so we know the last clicked object ID and save it (if not isset) to the Hardline list to assing it
        public UInt16 newViewIdCounter = 7;
        public UInt16 spawnViewUpdateCounter = 1; // maybe the wording is wrong - need to change this later
        private UInt16 jumpID = 36352;



        public bool waitForRPCShutDown = false;
        internal ushort selfSpawnIdCounter;

        public ClientData ()
		{
			rsiValues = new int[22];
			RPCCounter = 0;
			
			pss = 0x00;
			onWorld = false;


		}

        public byte assignSpawnIdCounter()
        {
            byte temp = (byte)spawnViewUpdateCounter;
            spawnViewUpdateCounter++;
            if (spawnViewUpdateCounter == 256)
                spawnViewUpdateCounter = 1;
            return temp;
        }

        public void setupPlayerData()
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
            return this.jumpID;
        }

        public void incrementJumpID()
        {
            this.jumpID++;
            //Output.WriteLine("JUMP ID IS NOW " + this.jumpID.ToString());
			if (this.jumpID==65534){ //Safe way
                this.jumpID = 1;
			}
        }
		
		public float getClientSimTime(){
			return clientSimTime;
		}
		
		
		public void setRsiValues(int[] newRSI){
			this.rsiValues = newRSI;
		}
		
		public int[] getRsiValues(){
			return this.rsiValues;
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
            this.UDPSessionEstablished = value;
        }

        public bool getUDPSEssionEstablished()
        {
            return this.UDPSessionEstablished;
        }

        public void DecrementSseq()
        {
            // For the MessageQueue if we need 
            if (this.sseq > 0 && this.sseq < 4096)
            {
                this.sseq--;
            }
        }

        public UInt16 calculateNextPossibleSseq()
        {
            UInt16 futureSseq = this.sseq;
            futureSseq++;
            if(futureSseq==4096)
            {
               futureSseq=0;
            }
            return futureSseq;
        }

		public void IncrementSseq(){
			this.sseq++;
			if (this.sseq==4096){ //Safe way
				this.sseq = 0;
			}
		}
		
		public UInt16 getPss(){
			return this.pss;
		}
		
		public UInt16 getCseq(){
			return this.cseq;
		}
		
		public UInt16 getSseq(){
			return this.sseq;
		}
		
		public UInt16 getACK(){
			return this.sseqACKBYCLIENT;
		}
		
		public void setPss(UInt16 pss){
			this.pss = pss;
		}
		
		public void setCseq(UInt16 cseq){
			this.cseq = cseq;
		}
		
		public void setSseq(UInt16 sseq){
			this.sseq = sseq;
		}
		
		public void setACK(UInt16 ack){
			this.sseqACKBYCLIENT = ack;
		}
		
		public void setRPCCounter(UInt16 rpc){
			this.RPCCounter = rpc;
		}
		
		public UInt16 getRPCCounter(){
			return this.RPCCounter;
		}
		
		public UInt32 getCharID(){
			return this.charID;
		}
		
		public void setCharID(UInt32 charID){
			this.charID = charID;
		}
		
		public UInt16 getObjectID(){
			return this.objectID;
		}
		
		public void setObjectID(UInt16 objectID){
			this.objectID = objectID;
		}

        public UInt64 getEntityId()
        {
            return this.entityId;
        }

        public void setEntityId(UInt64 _entityId)
        {
            this.entityId = _entityId;
        }
		
		public void setUniqueKey(string key){
			this.uniqueKey = key;
		}
		
		public string getUniqueKey(string key){
			return this.uniqueKey;
		}
		
		
		public void setExperience(long _exp){
			this.exp = _exp;
		}
		public long getExperience(){
			return this.exp;
		}
		
		public void setInfo(long _cash){
			this.cash = _cash;
		}
		
		public long getInfo(){
			return this.cash;
		}
		
		public void setDistrict(string _dist){
			this.district = _dist;
		}
		
		public string getDistrict(){
			return this.district;
		}

        public void setDistrictId(uint _districtId)
        {
            this.districtId = _districtId;
        }

        public uint getDistrictId()
        {
            return this.districtId;
        }
		

        public bool getRPCShutDown()
        {
            return this.waitForRPCShutDown;
        }

        public void setRPCShutDown(bool state)
        {
            this.waitForRPCShutDown = state;
        }
	}
}

