using System;
using System.Collections;

namespace hds
{

	/* 
		This is the initial packets loader plugin. Handles the first data needed for the client.
	*/
	public class InitialPacketsLoader : Plugin
	{
		
		private int packetsReceived;
		private StringUtils su;
		private NumericalUtils nu;
		private PacketsUtils packetsUtils;
		
		
		/* Hardcoded data init */
	
		private InitialPacketCreator creator;
		/* Hardcoded data end */
		 
		 
		
		public InitialPacketsLoader(ClientData data, WorldDbAccess databaseHandler){
			cData = data; // cData is part of the Plugin class
			packetsUtils = new PacketsUtils();
			
			this.databaseHandler = databaseHandler;
			this.creator = new InitialPacketCreator(this.databaseHandler,this.packetsUtils);
			this.su = new StringUtils();
			this.nu = new NumericalUtils();
			packetsReceived = 0;
			
		}
		
				
		 public override ArrayList process(byte[] packetData){ // Override the abstract method
			
			
			/* TODO HERE: we must recode the case 2 and 3 
			 * 
			 * They make the client have different sessionID.
			 * 
			 * Coded is 28A90200
			 * This packets harcoded had: 75d40100
			 * 
			 * */		
			
			// Here we trick the game, as we send harcoded data
			
			Console.WriteLine("[INITIAL PACKETS PLUGIN] RECEIVED DATA. PacketsReceived now: {0}",packetsReceived);
			Console.WriteLine("[INITIAL PACKETS PLUGIN] Current SeqData: PSS {0} CSeq {1} Sseq {2}",cData.getPss(),cData.getCseq(),cData.getSseq());
			ArrayList packetList = new ArrayList();
			
			switch(packetsReceived){
				
				case 0:
					//Must get the player data from Database before anything
				
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
				
					// Create the first packet in the world

					packetList.Add(new WorldPacket(creator.createWorldPacket(ref cData,0),true,0));
				
				break;
				
				case 1:
					
					packetList.Add(new WorldPacket(creator.createWorldPacket(ref cData,3),true,0));
				break;
				
				/*case 2:
				
					packetList.Add(new WorldPacket(creator.createWorldPacket(ref cData,packetsReceived),true,0));
				break;
				
				case 3:
					packetList.Add(new WorldPacket(creator.createWorldPacket(ref cData,packetsReceived),true,0));
				
				break;*/
				
				default:
					Console.WriteLine("[INITIAL PACKETS PLUGIN] RECEIVED MORE THAN 4 PACKETS. STATUS CHANGE NEEDED.",packetsReceived);
				break;
			}
			
			this.packetsReceived++;
			
			return packetList;
		}
		
		// Determines if we received 7 packets already. In other case... we will be still processing
		public override bool endedProcess(){
			if (this.packetsReceived==4){
				return true;
			}
			return false;
		}
	}
}
