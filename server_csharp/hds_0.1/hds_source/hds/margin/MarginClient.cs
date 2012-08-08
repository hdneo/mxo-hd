/// <summary>
/// Hardline Dreams Team - 2010
/// Created by Neo
/// Modified by Morpheus
/// </summary>
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace hds
{
	public class MarginClient{
		
		private int hashID;
		private TcpClient tcpClient;
		private bool working;
		private MarginEncryption crypto;
		private StringUtils su;
		private ArrayUtils au;
		private NumericalUtils nu;
		private NetworkStream clientStream;
		private bool waitingForWorld;
		private UInt32 userID;
		
		public MarginClient(int hashID){
			this.hashID = hashID;
			working = true;
			waitingForWorld = false;
			
			crypto = new MarginEncryption();
			su = new StringUtils();
			au = new ArrayUtils();
			nu = new NumericalUtils();
		}
		
		public int getID(){
			return hashID;
		}
		
		public bool isWorking(){
			return working;
		}
		
		public void passiveClose(){
			this.working = false;
		}
		
		public void forceClose(){
			this.tcpClient.Close();
			this.clientStream.Close();
		}
		
		public bool isYourClientWaiting(UInt32 waitingID){
			//byte[] waitingID = nu.uint32ToByteArray(userId,1);
			
			if (this.userID==waitingID){
				this.waitingForWorld = false;
				return true;
			}
			return false;
		}
			
		public void HandleClientComm(object client){
  			tcpClient = (TcpClient)client;
  			clientStream= tcpClient.GetStream();
			
			// Define an auth server processor per thread
						
  			byte[] message = new byte[2048];
  			int bytesRead;
			
			Console.WriteLine("Margin client Connected.");
  			
	
			// Receive TCP auth packets from the connected client.
			while (working){
    			bytesRead = 0;

	    		try{ 
					bytesRead = clientStream.Read(message, 0, 2048);
				}
				catch{ break; }
	
	    		if (bytesRead == 0){
	            	Console.WriteLine("Margin: 0 Bytes received");
					break;
				}
				
				// Parse the received packet data
				try{
					
					byte[] packet = new byte[bytesRead];
					au.copy(packet,0,message,0,bytesRead);
    				packetHandler(packet,clientStream);
									
				}catch(MarginException marEx){
					Console.WriteLine(marEx);
					break;
				}
				
  			}
			working = false;
  			Console.WriteLine("Margin thread closing");
  			tcpClient.Close();
			clientStream.Close();
		}	
		
		
		public void packetHandler(byte[] packet, NetworkStream client )
        {
            bool encrypted = true;
            byte opcode = packet[2];
            byte[] data = { };
			byte pointer = packet[0];
			
            if (opcode == 0x01)
            {
                // Packet is unencrypted
                encrypted = false;
                data = packet;
            }
			
			// This overrides the above IF for packets where 3rd position is a "01"
			// Happened someday, and screwed login... yeah, really
			if(pointer!=0x81){
				encrypted=true;
			}

            if (encrypted == true)
            {
                byte[] encryptedPacket = { };
                if (packet[0] >= 0x80)
                {
                    // try to readjust the packet for one byte less if the lenght is too long
                    // just a crappy way
                    encryptedPacket = new byte[packet.Length-1];
                    au.copy(encryptedPacket,0,packet,1,packet.Length-1);
                }
                else
                {
                    encryptedPacket = packet;
                }
                // Get the IV from encrypted Packet and set it in cipherman for encryption
                byte[] decrypted = crypto.decryptMargin(encryptedPacket);
                // Just 2 zero bytes for opcode handling later (As we use third byte for both state, encrypted or not
                byte[] spacer = { 0x00, 0x00 };

                DynamicArray din = new DynamicArray();
				din.append(spacer);
				din.append(decrypted);
				
                data = din.getBytes(); 

            }else{
                data = packet;
            }

            opcode = data[2];
            showPacket(data, "received ");
			
			switch (opcode)
            {
                case 0x01:
					
                    certConnectRequest(packet, client);
					
                break;

                case 0x03:
                    certConnectReply(packet, client);
				
                break;

                case 0x06:
                    connectChallenge(packet, client);
				
                break;

                case 0x08:
                    ConnectChallengeResponse(packet, client);
                break;

                case 0x0f:
                    loadCharacter(packet, client);
                break;
            }
        }

        public void loadCharacter(byte[] packet, NetworkStream client)
        {
            
            byte[] response1 = su.hexStringToBytes("100000000028A9020000000100010000");
            byte[] encryptedResponse1 = crypto.encrypt(response1);
            sendTCPVariableLenPacket(encryptedResponse1, client);
            System.Threading.Thread.Sleep(50);

            byte[] response2 = su.hexStringToBytes("100000000028A90200000002000210009E0409000000650000006F0000006900000005000000000000006600000002000000300100004C617572656E7A0000000000000000000000000000000000000000000000000046697368626F726E65000000000000000000000000000000000000000000000057656E6E2064752064696520526F74652050696C6C65206E696D6D7374202C20626C656962737420647520696D2057756E6465726C616E64202C20756E64206963682066FC687265206469636820696E2064696520746965667374656E2074696566656E20646573204B616E696E6368656E6261756D732E0A0D0A0D416E64207468652050617468206F66204D6F72706865757A20697320626F726E2E2E2E0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000286BEEE7CBC01201000000000000000000000000000000000000000000000101010100000000034F0000000200520000000100540000000100");
            byte[] encryptedResponse2 = crypto.encrypt(response2);
            sendTCPVariableLenPacket(encryptedResponse2, client);
            System.Threading.Thread.Sleep(50);

            byte[] response3 = su.hexStringToBytes("100000000028A90200000003000310000000");
            byte[] encryptedResponse3 = crypto.encrypt(response3);
            sendTCPVariableLenPacket(encryptedResponse3, client);
            System.Threading.Thread.Sleep(50);

            byte[] response4 = su.hexStringToBytes("100000000028A90200000004000410000000");
            byte[] encryptedResponse4 = crypto.encrypt(response4);
            sendTCPVariableLenPacket(encryptedResponse4, client);
            System.Threading.Thread.Sleep(50);

            byte[] response5 = su.hexStringToBytes("100000000028A90200000005000810001E00013000000001310000000180000000018100000001980000000199000000");
            byte[] encryptedResponse5 = crypto.encrypt(response5);
            sendTCPVariableLenPacket(encryptedResponse5, client);
            System.Threading.Thread.Sleep(50);

            byte[] response6 = su.hexStringToBytes("100000000028A90200000006000910000000");
            byte[] encryptedResponse6 = crypto.encrypt(response6);
            sendTCPVariableLenPacket(encryptedResponse6, client);
            System.Threading.Thread.Sleep(50);

            byte[] response7 = su.hexStringToBytes("100000000028A90200000007000A10000000");
            byte[] encryptedResponse7 = crypto.encrypt(response7);
            sendTCPVariableLenPacket(encryptedResponse7, client);
            System.Threading.Thread.Sleep(50);

            byte[] response8 = su.hexStringToBytes("100000000028A90200000008000B10000000");
            byte[] encryptedResponse8 = crypto.encrypt(response8);
            sendTCPVariableLenPacket(encryptedResponse8, client);
            System.Threading.Thread.Sleep(50);

            byte[] response9 = su.hexStringToBytes("100000000028A90200000009000510007E0000130200000000001C0177A700000000001C02EC0400000000001C03890300000000001404050A00000000001405F704000000000024064295000000000014075F0900000000000009A6010000000000440A080A00000000003863DB0400000000001C64471A00000000001C66F41800000000001C68EA04000000000014");
            byte[] encryptedResponse9 = crypto.encrypt(response9);
            sendTCPVariableLenPacket(encryptedResponse9, client);
            System.Threading.Thread.Sleep(50);

            byte[] response10 = su.hexStringToBytes("100000000028A9020000000A000610000000");
            byte[] encryptedResponse10 = crypto.encrypt(response10);
            sendTCPVariableLenPacket(encryptedResponse10, client);
            System.Threading.Thread.Sleep(50);

            byte[] response11 = su.hexStringToBytes("100000000028A9020000000B00071000DE00000005080080010005C40180020000E40280030000080280040000140380050000180380060005B40080070000E400800800003401800900059400800A00006C00800B0000E002800C00006403800D00009000800E0080530E840F0080550E84100000A40180110001D001801200003C02801300004402801400012C00801500012800801600012C1180170001301180180001341180190005B000801A0000A000801B0000A400801C0001D000801D0000C800801E0000D400801F0000C400802000001C0580210000520E84220005B80F80230005C00F80240000550E84");
            byte[] encryptedResponse11 = crypto.encrypt(response11);
            sendTCPVariableLenPacket(encryptedResponse11, client);
            System.Threading.Thread.Sleep(50);

            byte[] response12 = su.hexStringToBytes("100000000028A9020000000C000E10000000");
            byte[] encryptedResponse12 = crypto.encrypt(response12);
            sendTCPVariableLenPacket(encryptedResponse12, client);
            System.Threading.Thread.Sleep(50);

            byte[] response13 = su.hexStringToBytes("100000000028A9020000000D000F10000000");
            byte[] encryptedResponse13 = crypto.encrypt(response13);
            sendTCPVariableLenPacket(encryptedResponse13, client);
            System.Threading.Thread.Sleep(50);

			/* This is the server MOTD announcement */
			
			DynamicArray announcement = new DynamicArray();
			byte[] header = {0x10,0x00,0x00,0x00,0x00,0x28,0xA9,0x02,0x00,0x10,0x27,0x0E,0x01,0x0D,0x10,0x00};
			TimeUtils tim = new TimeUtils();
			byte[] currentTime = tim.getUnixTime();
			//Load motd from file
			XmlParser xmlP = new XmlParser();
			string[] data=xmlP.loadDBParams("Config.xml");
			string motd=data[5];
			
			if (!motd.Equals("")){ // if MOTD not empty
				byte[] text = su.stringToBytes(motd);
				byte[] size = nu.uint16ToByteArray((UInt16)(currentTime.Length+text.Length),1);
				
				announcement.append(header);
				announcement.append(size);
				announcement.append(currentTime);
				announcement.append(text);
				          
	            byte[] encryptedResponse14 = crypto.encrypt(announcement.getBytes());
	            sendTCPVariableLenPacket(encryptedResponse14, client);
			}
			/* End of the MOTD */
			
            Console.WriteLine("[MARGIN] UserID "+this.userID+" waiting for session reply from world");
			waitForWorldReply();
			Console.WriteLine("[MARGIN] World Session Reply OK for UserID "+this.userID);
			
            EstablishUDPSessionReply(packet, client);
            
        }
		
		public void waitForWorldReply(){
			this.waitingForWorld = true;
			
			while(this.waitingForWorld){
				System.Threading.Thread.Sleep(25);
			}
			
		}

        public void EstablishUDPSessionReply(byte[] packet, NetworkStream client)
        {
            byte[] response = { 0x11, 0x00, 0x00, 0x00, 0x00 };
            byte[] encryptedResponse = crypto.encrypt(response);
            sendTCPVariableLenPacket(encryptedResponse, client);
        }

        public void ConnectChallengeResponse(byte[] packet, NetworkStream client)
        {
            // just accept the packet everytime
            // it seems that it have a 16 byte blob which could be encrypted but dunno if we will ever need it again
            byte[] response = {0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3e, 0x17, 0x26, 0x70, 0x0f, 0x00, 0x03, 0x00, 0x0c, 0x00, 0x07, 0x00, 0xa9, 0x00 };
            byte[] encryptedResponse = crypto.encrypt(response);
            sendTCPVariableLenPacket(encryptedResponse, client);
        }

        public void connectChallenge(byte[] packet, NetworkStream client)
        {

            byte[] response = { 0x07, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00, 0x00, 0x01, 0x00 };
            byte[] encryptedResponse = crypto.encrypt(response);
            sendTCPVariableLenPacket(encryptedResponse, client);
        }

        public void certConnectReply(byte[] packet, NetworkStream client)
        {
            // Just send the response
            byte[] response = { 0x04, 0x00, 0x00, 0x00, 0x00 };
            byte[] encryptedResponse = crypto.encrypt(response);
            sendTCPVariableLenPacket(encryptedResponse, client);

        }

        public void certConnectRequest(byte[] packet, NetworkStream client)
        {
            int firstnum = BitConverter.ToInt16(packet, 3);
            int authstart = BitConverter.ToInt16(packet, 5);
            if (firstnum != 3)
            {
                showMarginDebug("FirstNum is not 3, it is" + firstnum);
                
            }

            if (authstart != 310)
            {
                showMarginDebug("AuthStart is not 0x3601");
                
            }

			
			
            // Get the Signature
            byte[] signatur = new byte[128];
			
			au.copy(signatur,0,packet,7,128);
			            
			// ToDO MD5 Signature and verify it with RSA to check if everything is correct

            // Get the signedData
            byte[] signedData = new byte[packet.Length - 135];
            au.copy(signedData,0,packet,135,packet.Length-135);
			int userid = BitConverter.ToInt32(signedData, 1);
			this.userID = (UInt32)userid;

			showMarginDebug("UserID is :" + userid);
            // Stripout Exponent and Modulus

            byte[] exponent = {0x00, 0x00, 0x00, 0x11};
            byte[] modulus = new byte[96];
            
			au.copy(modulus,0,signedData,82,96);
            
            showPacket(exponent, "Users Exponent");
            showPacket(modulus, "Users Modulus");
            // Init our encryptor with users modulus and exponent
            crypto.setUserPubKey(exponent, modulus);
            // build the final packet, it consists of 1 byte 0x00 + twofish key for world and margin + encryptIV

            byte[] encryptMeResponse = new byte[33];
            encryptMeResponse[0] = 0x00;
			
			au.copy(encryptMeResponse,1,crypto.getTFKey(),0,16);
			au.copy(encryptMeResponse,17,crypto.getIV(),0,16);
            	

            showPacket(encryptMeResponse, "RSA PAcket before encryption");
            byte[] encryptedShit = { };
            encryptedShit = crypto.encryptUsersPublic(encryptMeResponse);

            byte[] blobSize = nu.uint16ToByteArray((UInt16)encryptedShit.Length,1);

            byte[] header = { 0x02, 0x03, 0x00 };
            // Write final packet
			
			DynamicArray din = new DynamicArray();
            din.append(header);
			din.append(blobSize);
			din.append(encryptedShit);
            
			sendTCPVariableLenPacket(din.getBytes(), client);
			
        }

        public void sendTCPVariableLenPacket(byte[] packet, NetworkStream client)
        {


            byte[] size = { };
            byte[] bytesize = { };

            if (packet.Length > 127)
            {
				
				// TODO: Check this for shorter code
                showMarginDebug("Size is long");
                int packetsize = packet.Length + 0x8000;
                size = BitConverter.GetBytes(packetsize);
                bytesize = new byte[2];
				au.copy(bytesize,0,size,0,2);
                au.reverse(bytesize);
                
                showPacket(bytesize, "Long size");
                

            }
            else
            {
				// TODO: Check this for shorter code
                showMarginDebug("Size is small");
                size = BitConverter.GetBytes(packet.Length);
                bytesize = new byte[1];
                au.copy(bytesize, 0, size, 0, 1);
                
            }
            showMarginDebug("Leave size if");
            byte[] finalPacket = new byte[packet.Length+size.Length];

            DynamicArray din = new DynamicArray();
			din.append(bytesize);
			din.append(packet);
			            
            finalPacket = din.getBytes();
            
            showPacket(finalPacket, "Response");
            
            client.Write(finalPacket, 0, finalPacket.Length);
            client.Flush();
        }

        private void showMarginDebug(string message)
        {
           // Console.WriteLine("[Margin]" + message);
        }
		
		private void showPacket(byte[] packet, string type)
        {
           // Console.WriteLine("Show " + type + " Packet :\n" + su.bytesToString(packet));
        }
		
		
	}
	
}
