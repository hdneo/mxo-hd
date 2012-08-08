using System;
using System.Collections;

namespace hds
{
	public class Replay: Plugin
	{
		
		private int packetsReceived;
		private ReplayClient rpclient;
		private NumericalUtils nu;
		private StringUtils su;
		private ArrayUtils au;
		private ArrayList replayData;
		
		private int pssIndex;
		
		public Replay (ClientData cData, WorldDbAccess databaseHandler)
		{
			this.packetsReceived = 0;
			this.pssIndex = 0;
			this.cData = cData;
			this.databaseHandler = databaseHandler;
			
			//Load replay server data
			XmlParser parser = new XmlParser();
			string[] result = parser.loadServerConfig("Config.xml");
			this.rpclient = new ReplayClient(result[2]);

			this.nu = new NumericalUtils();
			this.su = new StringUtils();
			this.au = new ArrayUtils();

			this.replayData = new ArrayList();
		
			// fetch data and /quit boys!
			this.replayData = this.rpclient.getData();
		}
		
		public override ArrayList process(byte[] packetData){ // Override the abstract method
			ArrayList packetList = new ArrayList();

		
			switch (packetsReceived){
				
				case 0:
					byte[] cIDHex = new byte[4];
					cIDHex[0] = packetData[11];
					cIDHex[1] = packetData[12];
					cIDHex[2] = packetData[13];
					cIDHex[3] = packetData[14];
				
					cData.setCharID(nu.ByteArrayToUint16(cIDHex,1));
					cData.setObjectID((UInt16)(cData.getCharID() + (UInt16)8000));
					
					databaseHandler.setPlayerValues(ref cData, cData.getCharID());
					databaseHandler.setRsiValues(ref cData, cData.getCharID());
				
					// send the init UDP packet * 5
					byte[] response = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05 };
					packetList.Add(new WorldPacket(response,false,0));
					packetList.Add(new WorldPacket(response,false,0));
					packetList.Add(new WorldPacket(response,false,0));
					packetList.Add(new WorldPacket(response,false,0));
					packetList.Add(new WorldPacket(response,false,0));
					
					// Lets add some sleep time packet
					packetList.Add(new WorldPacket(null,false,1000));
					byte[] data = su.hexStringToBytes((string)replayData[0]);
					packetList.Add(new WorldPacket(data,true,0));
					packetList.Add(new WorldPacket(null,false,1000));
				
				break;
			
				case 1:
					data = su.hexStringToBytes((string)replayData[1]);
					packetList.Add(new WorldPacket(data,true,0));
					cData.setPss((UInt16)0x03);
					break;
					
				case 2:
					data = su.hexStringToBytes((string)replayData[2]);
					packetList.Add(new WorldPacket(data,true,0));
					data = su.hexStringToBytes((string)replayData[3]);
					packetList.Add(new WorldPacket(data,true,0));
					cData.setPss((UInt16)0x15);
					break;
				
				case 3: //ROFL MAX SENDING!
					cData.setPss((UInt16)0x7f);
					cData.setPss((UInt16)0x7f);
				
					for (int i = 4;i<replayData.Count;i++){
						data = su.hexStringToBytes((string)replayData[i]);
						packetList.Add(new WorldPacket(data,true,0));
						packetList.Add(new WorldPacket(null,false,150));
					}
					
				break;

				default:
					//just ACK
					packetList.Add(new WorldPacket(su.stringToBytes("02"),true,0));	
				break;
			}
			
				
			
			packetsReceived++;

			return packetList;
		}
		
		
		private void fetchReplays(ref ArrayList pssList, ref ArrayList dataList){
			
			/*rpclient.send(su.stringToBytes("init"));
			bool reading=true;
			
			byte[] buffer = new byte[2054];
			byte[] temp;
			
			//while(reading){
				
				int receivedBytes = rpclient.receive(ref buffer);
				
				temp= new byte[2];
				au.fastCopy(buffer,temp,2);
				
				byte[] ending={0x99,0x99};
				if(au.equal(temp,ending)){
					break;
				}
				
				//PSS HANDLING
				UInt16 newPss;
				newPss = nu.ByteArrayToUint16(temp,1);
				pssList.Add(newPss);
			
				//DATA HANDLING
				receivedBytes = rpclient.receive(ref buffer);
				temp = new byte[receivedBytes];
				au.fastCopy(buffer,temp,receivedBytes);
				dataList.Add(new WorldPacket(temp,true,0));
			
			//}*/
		}
		

		public override bool endedProcess(){
			if(this.packetsReceived==99999999)
				return true;
			return false;
		}
	}
}

