using System;
namespace hds
{
	public class Attribute
	{
		
		byte[] innerValue;
		int size;
		string name;
		bool active;
				
		
		public Attribute (int _size,string _name)
		{
			size = _size;
			innerValue = new byte[_size];
			name = _name;
			active = false;
		}
		
		public string getName(){
			return name;
		}
		
		public byte[] getValue(){
			return innerValue;
		}
		
		public bool isActive(){
			return active;
		}
		
		public void enable(){
			active = true;
		}
		
		public void disable(){
			active = false;
		}
		
		public int getSize(){
			return size;
		}
		
		public void setValue(byte[] newValue){
			
			for(int i = 0;i<size;i++){
				if (i<newValue.Length){
					innerValue[i] = newValue[i];
				}
				else{
					innerValue[i] = 0x00;
				}
			}
					
			enable();
		}
		
				
		#region interfaces for values other-than-byte
		public void setValue(bool newValue){
			byte[] t = {0x00};
			if(newValue)
				t[0] = 0x01;
			setValue(t);
		}
		
		public void setValue(byte newValue){
			byte[] t = {0x00};
			t[0] = (byte) newValue;
			setValue(t);
		}
		
		public void setValue(UInt16 newValue){
			byte[] t= NumericalUtils.uint16ToByteArray(newValue,1);
			setValue(t);
		}
		
		public void setValue(UInt32 newValue){
			byte[] t= NumericalUtils.uint32ToByteArray(newValue,1);
			setValue(t);
		}
		
		public void setValue(float newValue){
			byte[] t= NumericalUtils.floatToByteArray(newValue,1);
			setValue(t);
		}
		
		public void setValue(double newValue){
			byte[] t= NumericalUtils.doubleToByteArray(newValue,1);
			setValue(t);
		}
		
		public void setValue(string newValue){
			byte[] t = StringUtils.stringToBytes(newValue);
			setValue(t);
		}
		
		#endregion
		
	}
}

