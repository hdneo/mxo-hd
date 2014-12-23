using System;
using System.Collections;
using System.Collections.Generic;

namespace hds
{
	public class ObjectManager
	{
		
		private int currentIdcounter = 0x01;
		private Dictionary<string,PlayerCharacter> currentObjects;
				
				
		public ObjectManager (){
			currentObjects = new Dictionary<string, PlayerCharacter>();
			Output.WriteLine("[SHEEVA] Object Manager Operative. Welcome back");
		}
		
		private byte assignIdCounter(){
			byte temp = (byte) currentIdcounter;
			currentIdcounter++;
			if (currentIdcounter==256)
				currentIdcounter = 1;
			return temp;
		}
		
		public void PushClient(string key){
			// Create a new gameobject for the unique key
			currentObjects[key] = new PlayerCharacter();
		}
		
		public PlayerCharacter GetAssignedObject(string key){
			// Return gameobject structure
			return currentObjects[key];
		}
		
		public void PopClient(string key){
			// Delete key for unused player
			currentObjects.Remove(key);
		}
		
		public DynamicArray generateCreationPacket(GameObject go,UInt16 viewID){
            
			DynamicArray din = new DynamicArray();
            byte FullyDynamicFlag=0x08;
			byte[] goid = go.getGoid();
            bool fullyDynamic = (go.getRelatedStaticObjId()==0xFFFFFFFF);
			if (fullyDynamic){
				FullyDynamicFlag=(byte)0x0c;
			}
			
			din.append(FullyDynamicFlag);
			din.append(goid);
			if (!fullyDynamic){
				din.append(NumericalUtils.uint32ToByteArray(go.getRelatedStaticObjId(),1));
			}
						
			din.append(assignIdCounter());
			byte[] separator = {0xcd,0xab};
			din.append(separator);
			
			din.append(go.getCreationAttributes());
			
			if(goid[0] == 0x0c && goid[1] == 0x00){
				viewID = 0x0002;
			}
			
            /*
			din.append(NumericalUtils.uint16ToByteArray(viewID,1));
			
			din.append(0x00);
			din.append(0x00);
			din.append(0x00);
			*/
			
			return din;
		}
	}
}

