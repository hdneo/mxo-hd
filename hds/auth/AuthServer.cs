using System;
using hds.shared;

namespace hds.auth{
	
	public class AuthServer{
		
		private MxoRSA rsa;
		private MxoTwofish tf;
		private Md5 md5;
		
		int status;
		
		const int AS_GetPublicKey_Request = 6;
		const int AS_HandleAuth_Request = 8;
		const int AS_HandleAuthChallenge_Response = 10;
		
		byte [] response;

		byte [] challenge;
		byte [] blankIV;
		byte [] md5edChallenge;
		
		
		WorldList wl;
		
		
		public AuthServer(){

			status = 0;
			rsa = new MxoRSA();
			tf = new MxoTwofish();
			md5 = new Md5();
			
			wl = new WorldList();
			
			blankIV = new byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00};
		}
		
		public int getStatus(){
			return status;
		}
		
		private int getOpCode(byte[] data){
			int opCode = -1;
			if ((int)data[0]<0x80)
				opCode = (int)data[1];
			else
				opCode = (int)data[2];
			
			return opCode;
		}
		
		public byte[] processPacket(byte[] data,int receivedBytes) {
			
			status = getOpCode(data);
			switch (status){
				case AS_GetPublicKey_Request:	
					response = processGetPublicKey_Request(data);
				break;
				
				case AS_HandleAuth_Request:
					response = processHandleAuth_Request(data);
				break;
				
				case AS_HandleAuthChallenge_Response:
					response = processHandleAuthChallenge_Response(data,receivedBytes);
				break;
				
				default:
					Output.WriteLine("Received: "+StringUtils.bytesToString(data,receivedBytes));
					throw new AuthException("OPcode not developped");
			}
			return response;
		}

		
		
		public byte[] processGetPublicKey_Request(byte[] data){
			byte AS_GetPublicKeyReply= 0x07;
			byte[] currentTime = TimeUtils.getUnixTime();
					
			// implicit RSA version = 0x00000004
			byte[] temp ={0x12,0xff,0x00,0x00,0x00,0x00,0xff,0xff,0xff,0xff,0x04,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00};
					
			// Set the values and not being FF
			temp[1] = AS_GetPublicKeyReply;
			temp[6] = currentTime[0];
			temp[7] = currentTime[1];
			temp[8] = currentTime[2];
			temp[9] = currentTime[3];
			
			
			return temp;
		}
		
		
		public byte[] processHandleAuth_Request(byte[] data){
			// RSA version check
			byte[] neededRSAV = {0x04,0x00,0x00,0x00};
			byte[] packetRSAV = new byte[4];
			
			// In this packet, RSA starts at offset 3
			ArrayUtils.copy(data,3,packetRSAV,0,4);
			
			if (!ArrayUtils.equal(neededRSAV,packetRSAV)){
				throw new AuthException("Invalid RSA version");
			}

			// Get RSA encrypted blob from the data
			byte [] blob = new byte[128];
			
			ArrayUtils.copy(data,44,blob,0,128);
						
			Output.OptWriteLine("-> Encrypted blob received.");
			
			// Get RSA decrypted blob
			byte[] decryptedBlob = rsa.decryptWithPrivkey(blob);
			
					
			Output.OptWriteLine("-> Blob decoded.");
			
			
			
			// Copy the Auth TF key from decoded blob.
			
			byte [] tfKey = new byte[16];
			ArrayUtils.copy(decryptedBlob,7,tfKey,0,16);
			
			tf.setIV(blankIV);
			tf.setKey(tfKey);

			// Create the challenge
			
			challenge = new byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00};
						
			byte[] criptedChallenge= new byte[16];
			tf.encrypt(challenge,criptedChallenge);
			
			md5edChallenge = md5.digest(challenge); // Rajko said this
			
			Output.OptWriteLine(" --> Client TF key: "+StringUtils.bytesToString(tfKey));
			Output.OptWriteLine(" --> MD5(challenge): "+StringUtils.bytesToString(md5edChallenge));
			Output.OptWriteLine(" --> Twofished(challenge): "+StringUtils.bytesToString(criptedChallenge));
			
			// Copy the encrypted thing to the global variable "Challenge"
			ArrayUtils.copy(criptedChallenge,0,challenge,0,16);

			int nameLength = decryptedBlob.Length - 30;
			byte[] nameB = new byte[nameLength];
			ArrayUtils.copy(decryptedBlob,29,nameB,0,nameLength);
			
			// Set WorldList username
			wl.setUsername(StringUtils.charBytesToString(nameB));
			
			Output.OptWriteLine("-> User login: "+wl.getUsername());

			byte[] a= new byte [18];
			a[0] = 0x11; // Size
			a[1] = 0x09; // Answer opcode
			ArrayUtils.copy(criptedChallenge,0,a,2,16);

			return a;
		}
		
		public byte[] processHandleAuthChallenge_Response(byte[] data,int maxBytes){
			
			int blobSize = (int) data[4];
			
			byte[] encryptedBlob = new byte[blobSize];
			byte[] decryptedBlob = new byte[blobSize];
			
			ArrayUtils.copy(data,6,encryptedBlob,0,blobSize);
			
			// Reset IV to 0
			tf.setIV(blankIV);
			tf.decrypt(encryptedBlob,decryptedBlob);
			
			byte[] receivedMD5 = new byte[16];
			ArrayUtils.copy (decryptedBlob,1,receivedMD5,0,16);
			
						
			// Security says that must be the same
            if (!ArrayUtils.equal(receivedMD5, md5edChallenge)){
				Output.WriteLine("The Md5 from client and Our Md5 are not same, aborting");
				Output.WriteLine("Decrypted (TF) blob:"+StringUtils.bytesToString(decryptedBlob));
				Output.WriteLine("Stored MD5ed Challenge:"+StringUtils.bytesToString(md5edChallenge));
				throw new AuthException("Md5 challenge differs");
			}
			
			// We take the pass from the decrypted Blob and subtract 1 byte, the ending "0"
		 	int passSize = ((int) decryptedBlob[23] )-1;
			
			byte[] passwordB = new byte[passSize];
			ArrayUtils.copy(decryptedBlob,25,passwordB,0,passSize);
			
			Output.OptWriteLine("-> Password decrypted, size:"+passSize);
			
			// Set WorldList password
			wl.setPassword(StringUtils.bytesToString_NS(md5.digest(passwordB)));

            if (Store.dbManager.AuthDbHandler.fetchWordList(ref wl)){
				
				// Do calculations magic
				byte[] hexUserID = NumericalUtils.uint32ToByteArray((UInt32)wl.getUserID(),1);
				
				byte[] nameH = new byte[33];
				
				string name = wl.getUsername();
				for (int i = 0;i<name.Length;i++){
					nameH[i] = (byte)name[i];
				}
				
				byte[] padding = new byte[4]; // 4 empty byte ==> [0x00,0x00,0x00,0x00]
				
				// +10 mins from now, so 60secs per min
				byte[] expiredTime = TimeUtils.getUnixTime(60*10);
											
				byte[] padding2 = new byte[32]; // 32 empty byte ==> [0x00,0x00,0x00,0x00,...]
				byte[] pubModulusH = wl.getPublicModulus();
				byte[] createdTime = NumericalUtils.uint32ToByteArray((UInt32)wl.getTimeCreated(),1);
				
					
				// Create signed data
				DynamicArray signedData = new DynamicArray();
				signedData.append(new byte[]{0x01});
				signedData.append(hexUserID);
				signedData.append(nameH);
				signedData.append(new byte[]{0x00,0x01});
				signedData.append(padding);
				signedData.append(expiredTime);
				signedData.append(padding2);
				signedData.append(new byte[]{0x00,0x11});
				signedData.append(pubModulusH);
				signedData.append(createdTime);
				
				byte[] md5FromStructure=md5.digest(signedData.getBytes());
				
				// Do MD5 to the signed data and RSA encrypt the result
				byte[] signature = rsa.signWithMD5(md5FromStructure);
				
								
				Output.OptWriteLine("-> Signing with RSA");
				
				// Do: privExp = Twofish(privExp)
				byte[] privExp = wl.getPrivateExponent();
				byte[] buffer = new byte[privExp.Length];
				
				// We keep the Key from before this part
				// Set the challenge we saved before as IV
				tf.setIV(challenge);
				tf.encrypt(privExp,buffer);
				
				privExp = buffer;
				
				byte []privExpSize = NumericalUtils.uint16ToByteArray((UInt16)privExp.Length,1);
				
				// Calculate offsets
				int offsetAuthData = 33+wl.getCharPack().getPackLength() + wl.getWorldPack().getTotalSize();
				int offsetEncryptedData = offsetAuthData+signature.Length+signedData.getSize()+2;
				int offsetCharData = 33;
				int offsetServerData = 33+wl.getCharPack().getPackLength();
				int offsetUsernameLast = offsetEncryptedData + 2 + privExp.Length;
			
				// Create the auth header
				
				DynamicArray authHeader = new DynamicArray();
				authHeader.append(new byte[]{0x0b,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00});
				authHeader.append(NumericalUtils.uint16ToByteArray((UInt16)offsetAuthData,1));
				authHeader.append(NumericalUtils.uint16ToByteArray((UInt16)offsetEncryptedData,1));
				authHeader.append(new byte[]{0x1f,0x00,0x00,0x00});
				authHeader.append(NumericalUtils.uint16ToByteArray((UInt16)offsetCharData,1));
				authHeader.append(new byte[]{0x6E,0xD1,0x00,0x00});
				authHeader.append(NumericalUtils.uint32ToByteArray((UInt32)offsetServerData,1));
				authHeader.append(NumericalUtils.uint32ToByteArray((UInt32)offsetUsernameLast,1));
				
				byte [] usernameSize = NumericalUtils.uint16ToByteArray((UInt16)(name.Length+1),1);
				
				// Create the semiResponse (full data, except the total size)
				DynamicArray semiResponse = new DynamicArray();
				semiResponse.append(authHeader.getBytes());
				semiResponse.append(wl.getCharPack().getByteContents());
				semiResponse.append(wl.getWorldPack().getByteContents());
				semiResponse.append(new byte[]{0x36,0x01});
				semiResponse.append(signature);
				semiResponse.append(signedData.getBytes());
				semiResponse.append(privExpSize);
				semiResponse.append(privExp);
				semiResponse.append(usernameSize);
				
				
				byte []tempName = new byte[name.Length];
				
				for (int i = 0;i<name.Length;i++){
					tempName[i] = (byte)name[i];
				}
				
				semiResponse.append(tempName);
				semiResponse.append(new byte[]{0x00});
				
				int bigSize = semiResponse.getSize();
				bigSize +=0x8000; // Add TCP Len Var
				
				byte []finalSize = NumericalUtils.uint16ToByteArray((UInt16)bigSize,0);
				
				// Create the finalResponse (full data, plus total size)
				DynamicArray finalResponse = new DynamicArray();
				finalResponse.append(finalSize);
				finalResponse.append(semiResponse.getBytes());
				
				
				Output.OptWriteLine("Sending world list.");
				status = -1;
				
				return finalResponse.getBytes();
			}
			
			byte []worldListPacket = {0x00,0x00};
			
			status=-1; // trick
			return worldListPacket;
			
		}
		
	}
}
