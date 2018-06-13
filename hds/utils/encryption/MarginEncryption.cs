
using System;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;


namespace hds
{
	
	
	public class MarginEncryption{
		
		private byte[] TF_Key = { 0x6C, 0xAB, 0x8E, 0xCC, 0xE7, 0x3C, 0x22, 0x47, 0xDB, 0xEB, 0xDE, 0x1A, 0xA8, 0xE7, 0x5F, 0xB8 };
        private byte[] IV = { 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30 };

		private const string xmlKeys = "<RSAKeyValue><Modulus>qMIfEkrXWpRr44ecWMzJHV7Hjg9bnru2PZv3NydzOZ6uab52wET+RoHhIzv+zJb3zBhmETAtsrmNnBXiW7tfqPK0xf6lb9RbvupfnfYSHO5WaEcWEi0JjQRBevg9d8qlETo9Hrfy9PEfpeK1T2WF+xxx73chvBTB12Paa7yT+Ik=</Modulus><Exponent>EQ==</Exponent><P>y1biPGcMKGk2VRKuHJZBvge0A/8YLTM5+gvdiZn2+2sC09iz/zjWa26ARP9ENMA687B6vo9asGtHovQCizWNgw==</P><Q>1HaPHEY726MwhSXUOl9l3Roon/hxOdfEE4ooMSe0z8WYW3+IDXTAbYCpJHMZNIvrTLSEuReuDdNL8W91Kq5wAw==</Q><DP>v2DU7Y4pj3IVBMZJhFEu0Pgw9LPahOTrRbDQgYHZZRlsEq3WldskKOB4uWi4qh5Vmg+ClTugpgqdxotNsDJnEQ==</DP><DQ>V3wcz2g2w9nIr0vP28zttWUfyWZMvXb2YmYQjLX/KGBr6XC/jRH04cuQ8OQZb/1g41lj07502IQuVFsSIKIuHw==</DQ><InverseQ>CnkbOBzROWVuJMlB8YIwswZSkBmhckoz+EjEKqIUSE8cYPnhOr2KNfUnTVFTFLlOzWc4jrrOQPi94rrDae8yPw==</InverseQ><D>RX0b2lsNYYhoqPuauycloq6OZ6v4jKelZKmiB6bVF7nPWLfWi2ez/uovhvqWGAHtkEZIJTH0swEcMTYwB6eBvV5fQPkL1CiZJELi7UGEME9v/UMqsMn/aAs3iqDYn0sR1kC4mZHRRNzIq7l16yufV6XnNaVuwlDMo3OLVWwqWE0=</D></RSAKeyValue>";
        private byte[] userModulus = { };
        private byte[] userExponent = { };
        private RSAParameters rsaparam;
		
		private MxoTwofish tf;
		private Crc32 crc32;
		
		public MarginEncryption(){
			tf = new MxoTwofish();
			crc32 = new Crc32();
		}
		
		public byte[] getTFKey(){
			return TF_Key;
		}
		
		public byte[] getIV(){
			return IV;
		}
		
		
		public byte[] encryptUsersPublic(byte[] plain)
        {

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(rsaparam);
            byte[] encrypted = rsa.Encrypt(plain, true);
            return encrypted;

        }
		
		
		public void setUserPubKey(byte[] exponent, byte[] modulus)
        {
            rsaparam = new RSAParameters();
            
            rsaparam.Modulus = modulus;

            rsaparam.Exponent = exponent;
        }
		
		public byte[] encrypt(byte[] packet){
			
			// Get timestamp 
			byte[] timestamp = TimeUtils.getUnixTime();


			// get size
			int psize = packet.Length;
			byte[] size = NumericalUtils.uint16ToByteArray((UInt16)psize,1);

			//showPacket(size, " Size ");

			// final Packet
			
			DynamicArray temp = new DynamicArray();
			temp.append(size);
			temp.append(timestamp);
			temp.append(packet);
			
            
			// compute CRC32
			byte[] crc32pax = crc32.checksumB(temp.getBytes(),1);         


			// Padding
			int totalLength = temp.getSize() + 4;
			int padding = 16 - (totalLength % 16);
			
			byte[] paddingBytes = new byte[padding];

			for (int i = 0; i < padding; i++)
			{
				paddingBytes[i] = (byte)padding;
			}
			
			temp.append(paddingBytes);
			tf.setIV(IV);
			tf.setKey(TF_Key);

			// We init with 2 more than needed, so no memory reservation is done on dyn array
			DynamicArray finalPlainData = new DynamicArray();
			
			finalPlainData.append(crc32pax);
			finalPlainData.append(temp.getBytes());
			
			temp = null; // Cleaning the house
           
			
			byte[] encryptedData = new byte[finalPlainData.getSize()];
			tf.encrypt(finalPlainData.getBytes(),encryptedData);
			
            
			finalPlainData = null; // Cleaning the house (2)
			
			// add IV before the results
            
			DynamicArray response = new DynamicArray();
			response.append(IV);
			response.append(encryptedData);

			// Display HEX Values after Encryption
			return response.getBytes();
		}
		
		
		public byte[] decryptMargin(byte[] packet)
        {

	        if (packet.Length < 17)
	        {
		        // This happens - but shouldnt happen. 
		        return null; 
	        }
			ArrayUtils.copy(packet,1,IV,0,16);
			
            int decryptBuffer = packet.Length - 17;

            byte[] packetToDecrypt = new byte[decryptBuffer];
            
			ArrayUtils.copy (packet,17,packetToDecrypt,0,packet.Length-17);
			
			tf.setIV(IV);
			tf.setKey(TF_Key);
			
			byte[] decryptedBytes = new byte[packetToDecrypt.Length];
			tf.decrypt(packetToDecrypt,decryptedBytes);
			
            byte[] decryptedPacket = new byte[decryptedBytes.Length - 10];
            
			ArrayUtils.copy(decryptedBytes,10,decryptedPacket,0,decryptedBytes.Length-10);
			
			return decryptedPacket;
        }
		
		
	}
}
