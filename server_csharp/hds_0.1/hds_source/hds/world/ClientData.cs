using System;
using System.Collections;
namespace hds
{
	public class ClientData
	{
		
		
		// Mxo coords
		
		private Hashtable playerValues;
		
		private int[] rsiValues;
		
		// MXO Client values (server data)
		private string uniqueKey;
		private UInt16 pss;
		private UInt16 cseq;
		private UInt16 sseq;
		private UInt16 sseqACKBYCLIENT;
		private UInt16 RPCCounter;
		private UInt16 charID;
		private UInt16 objectID;
		
		public ClientData ()
		{
			playerValues = new Hashtable();
		
			rsiValues = new int[22];
			RPCCounter = 0;
			initPlayerValues();
		}
		
		public void initPlayerValues(){
			playerValues["handle"]="";
            playerValues["x"]=0.0f;
            playerValues["y"]=0.0f;
            playerValues["z"]=0.0f;
            playerValues["rotation"]=0;
            playerValues["healthC"]=0.0f;
            playerValues["healthM"]=0.0f;
            playerValues["innerStrC"]=0.0f;
            playerValues["innerStrM"]=0.0f;
            playerValues["level"]=0;
            playerValues["profession"]=0;
            playerValues["alignment"]=0;
            playerValues["pvpFlag"]=0;
            playerValues["firstName"]="";
            playerValues["lastName"]="";
            playerValues["exp"]=0;
            playerValues["cash"]=0;
            playerValues["district"]="";
		}
		
		public Object getPlayerValue(string key){
			try{
				return playerValues[key];
			}	
			catch(Exception e){
				Console.WriteLine("Accessing non existent player value ID:"+key);
				Console.WriteLine("DEBUG: "+e.Message);
				return null;
			}
		}
		
		public void setPlayerValue(string key, Object value){
			playerValues[key]=value;
		}
		
		public void setRsiValues(int[] newRSI){
			this.rsiValues = newRSI;
		}
		
		public int[] getRsiValues(){
			return this.rsiValues;
		}
		
				
		public void IncrementSseq(){
			this.sseq+=1;
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
		
		public UInt16 getCharID(){
			return this.charID;
		}
		
		public void setCharID(UInt16 charID){
			this.charID = charID;
		}
		
		public UInt16 getObjectID(){
			return this.objectID;
		}
		
		public void setObjectID(UInt16 objectID){
			this.objectID = objectID;
		}
		
		public void setUniqueKey(string key){
			this.uniqueKey = key;
		}
		
		public string getUniqueKey(string key){
			return this.uniqueKey;
		}
	}
}

