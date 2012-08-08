
using System;
using System.IO;

namespace hds
{
	
	
	public class DynamicArray
	{
		MemoryStream storage;
		
		public DynamicArray(){
			storage = new MemoryStream();
		}
		
		public byte[] getBytes(){
			return storage.ToArray();
		}
		
		public int getSize(){
			return (int)storage.Length;
		}
		
		public void append(byte [] newData){
			storage.Write(newData,0,newData.Length);
			
		}
		
	}
}
	

