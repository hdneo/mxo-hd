using System;
namespace hds
{
	public class GameObject
	{
		
		private Attribute[] attributesCreation;
		private Attribute[] attributesUpdate;
		private int creationGroups;
		private int updateGroups;
		private string name;
		private UInt16 goid;
		private UInt32 relatedStaticObjId;
		
		public GameObject(int _attributeNumCreation,int _attributeNumUpdate,int _creationGroups, int _updateGroups,string _name,UInt16 _goid,UInt32 _relatedStaticObjId){
			attributesCreation = new Attribute[_attributeNumCreation];
			
			if (_attributeNumUpdate>0){ 
				attributesUpdate = new Attribute[_attributeNumUpdate];
			}else{
				attributesUpdate=null;
			}
			creationGroups = _creationGroups;
			updateGroups = _updateGroups;
			name = _name;
			goid = _goid;
			relatedStaticObjId=_relatedStaticObjId;
		}
		
		public string GetName(){
			return name;
		}
		
		public byte[] GetGoid(){
			return NumericalUtils.uint16ToByteArray(goid,1);
		}
		
		public UInt32 GetRelatedStaticObjId(){
			return relatedStaticObjId;
		}
		
		public void SetRelatedStaticObjId(UInt32 _relatedStaticObjId){
			relatedStaticObjId= _relatedStaticObjId;
		}
		
		public void UnsetRelatedStaticObjId(){
			SetRelatedStaticObjId(0xFFFFFFFF);
		}

		public void DisableAllAttributes(){
			for (int i = 0;i<attributesCreation.Length;i++){
				attributesCreation[i].disable();
			}
		}
		
		public void AddAttribute(ref Attribute newAttribute, int indexCreation, int indexUpdate){
			attributesCreation[indexCreation]=newAttribute;
			if (indexUpdate!=-1){ // if present only
				attributesUpdate[indexUpdate]=newAttribute;	
			}
		}
		
		public void SetAttributeValue(byte[] content, int indexCreation){
			attributesCreation[indexCreation].setValue(content);
		}
		
		public byte[] GetCreationAttributes(){
			DynamicArray din = new DynamicArray();
			bool lastGroupEmpty = true;
			int attribCounter = 0;
			
			for (int i = (creationGroups-1);i>=0;i--){
				int attribOffset = i*7;
				bool anyAttribEnabled = false;
				byte tempHeader = 0x00;
				
				for (int j = 6;j>=0;j--){
					int position = attribOffset+j;
					// This verifies that the attribute is in a group but groups has not 7 attributes
					if (position<attributesCreation.Length){ 
						Attribute temp = attributesCreation[attribOffset+j];
						if (temp.isActive()){
							anyAttribEnabled = true;
							attribCounter++;
							tempHeader = (byte) (tempHeader + (1<<j)); // Displacement
							din.insertBefore(temp.getValue());
						}
					}
				}

				// Updating last attribute group, set it as 0b0XXXXXXX
				if (i == (creationGroups-1)){
					if (anyAttribEnabled){
						din.insertBefore(tempHeader);
						lastGroupEmpty = false;
					}
				}
				// Updating other than last attribute group
				else{
					if (!lastGroupEmpty){
						tempHeader = (byte) (tempHeader+0x80);
						din.insertBefore(tempHeader);
					}else{
						if(anyAttribEnabled){
							din.insertBefore(tempHeader);
							lastGroupEmpty = false;
						}
					}				
				}

			}
			
			//add the counter of attributes sent
			din.insertBefore((byte)attribCounter);
			return din.getBytes();
		}
		
		public void Debug(){
			Console.WriteLine("Creation statuses");
			for(int i = 0;i<attributesCreation.Length;i++){
				if (attributesCreation[i].isActive()){
					Console.WriteLine("\t"+i+ " ON");
				}else{
					Console.WriteLine("\t"+i+ " OFF");
				}
			}
			
			
			Console.WriteLine("Update statuses");
			for(int i = 0;i<attributesUpdate.Length;i++){
				if (attributesUpdate[i].isActive()){
					Console.WriteLine("\t"+i+ " ON");
				}else{
					Console.WriteLine("\t"+i+ " OFF");
				}
			}
			
		}

	}
}

