using System;
using System.Collections;

namespace hds.resources.gameobjects
{
	public class ObjectManager
	{

		private readonly Hashtable _currentObjects;
				
				
		public ObjectManager (){
			_currentObjects = new Hashtable();
			Output.WriteLine("[SHEEVA] Object Manager Operative. Welcome back");
		}
		
		
		public void PushClient(string key){
            // Create a new gameobject for the unique key
            if (!_currentObjects.ContainsKey(key))
            {
                _currentObjects.Add(key, new PlayerCharacter());
            }
            
		}
		
		public PlayerCharacter GetAssignedObject(string key){
			// Return gameobject structure
			return _currentObjects[key] as PlayerCharacter;
		}
		
		public void PopClient(string key){
			// Delete key for unused player
			_currentObjects.Remove(key);
		}
		
		public DynamicArray GenerateCreationPacket(GameObject go,UInt16 viewID, byte idCounter){
            
			DynamicArray din = new DynamicArray();
            byte fullyDynamicFlag=0x08;
			byte[] goid = go.GetGoid();
            bool fullyDynamic = (go.GetRelatedStaticObjId()==0xFFFFFFFF);
			if (fullyDynamic){
				fullyDynamicFlag=(byte)0x0c;
			}

			din.append(fullyDynamicFlag);
			din.append(goid);
			if (!fullyDynamic){
				din.append(NumericalUtils.uint32ToByteArray(go.GetRelatedStaticObjId(),1));
			}
						
			din.append(idCounter);
			byte[] separator = {0xcd,0xab};
			din.append(separator);
			
			din.append(go.GetCreationAttributes());
			
			if(goid[0] == 0x0c && goid[1] == 0x00){
				viewID = 0x0002;
			}

			return din;
		}
	}
}

