using System;
namespace hds
{
	public class WorldPacket
	{
		
		private byte[] content;
		private bool encrypted;
		private int sleepTime;
		
		public WorldPacket (byte[] content,bool encrypted,int sleepTime)
		{
			setContent(content);	
			setEncrypted(encrypted);
			setSleepTime(sleepTime);
		}
		
		public void setContent(byte[] content){
			this.content = content;
		}
		
		public byte[] getContent(){
			return this.content;
		}
		
		public void setEncrypted(bool encrypted){
			this.encrypted = encrypted;
		}
		
		public bool getEncrypted(){
			return this.encrypted;
		}
		
		public void setSleepTime(int sleepTime){
			this.sleepTime = sleepTime;
		}
		
		public int getSleepTime(){
			return this.sleepTime;
		}
	}
}

