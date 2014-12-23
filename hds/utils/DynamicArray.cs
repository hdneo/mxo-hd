
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
		
		public void append(byte data){
			byte[] d = {data};
			append(d);
		}
		
		public void append(byte [] newData){
			storage.Write(newData,0,newData.Length);
		}
		
		public void insertBefore(byte data){
			byte[] d = {data};
			insertBefore(d);
		}
		
		public void insertBefore(byte [] newData){
			byte[] oldData = storage.ToArray();
			storage = null;
			storage = new MemoryStream();
			storage.Write(newData,0,newData.Length);
			storage.Write(oldData,0,oldData.Length);
			
			
		}
		
	}
}
	

