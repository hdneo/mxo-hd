using System;
using System.Diagnostics;

namespace hds
{
	public class ServerTests
	{
		public ServerTests ()
		{
			
		}
		
		private int twofishEncrypt(){
			ArrayUtils au = new ArrayUtils();
			byte[] key = { 0xa4,0x46,0xdc,0x73,0x25,0x08,0xee,0xb7,0x6c,0x9e,0xb4,0x4a,0xb8,0xe7,0x11,0x03 };
			byte[] iv = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
			byte[] plainText = {0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00};
			byte[] knownResult = {0xba,0x05,0xf5,0x6e,0x86,0x5f,0xb6,0xc9,0xb2,0x1d,0xe1,0x00,0xb6,0xec,0x46,0xe9};
			byte[] result = new byte[16];
			MxoTwofish tf = new MxoTwofish();
			tf.setIV(iv);
			tf.setKey(key);
			tf.encrypt(plainText,result);

            if (!ArrayUtils.equal(knownResult, result)){
				Output.Write("Failed\n");
				Output.WriteLine(StringUtils.bytesToString(knownResult));
				Output.WriteLine(StringUtils.bytesToString(result));
				return 0;
			}
			Output.Write("OK\n");
			return 1;
		}
		
		public void doTests(){
			
			/*DynamicTail tail = new DynamicTail();
			Debug.Assert(tail.Size()==0);
			
			tail.Append(1);
			tail.Append(2);
			tail.Append(3);
					
			for (int i = 0;i<tail.Size();i++){
				Console.Write(tail.At(i)+" ");
			}
			Console.WriteLine("");
			
			
			while(tail.DeleteAt(0)>0){}
			
			
			Console.WriteLine("POST DELETING");
			
			for (int i = 0;i<tail.Size();i++){
				Console.Write(tail.At(i)+" ");
			}
						
			Console.WriteLine("");*/
			
			twofishEncrypt();
		}
	}
}

