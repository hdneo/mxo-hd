using System;
using System.Collections;
using System.Drawing.Printing;
using System.IO;
using System.Security.Cryptography;
using ManyMonkeys.Cryptography;

namespace hds
{
    public class WorldEncryption : IWorldEncryption
    {
        public byte[] TF_Key = { 0x6C, 0xAB, 0x8E, 0xCC, 0xE7, 0x3C, 0x22, 0x47, 0xDB, 0xEB, 0xDE, 0x1A, 0xA8, 0xE7, 0x5F, 0xB8 };
        public byte[] IV = { 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30 };
        
        private byte[] innerBuffer;		        
        private Crc32 crc32;
        private MxoTwofish tf;

        public WorldEncryption()
        {
            innerBuffer = new byte [2048];
            InitCrypto();
        }

        public void InitCrypto()
        {
            crc32 = new Crc32();
            tf = new MxoTwofish();
            tf.SetIV(IV);
            tf.SetKey(TF_Key);
        }
        
        public byte[] encrypt(byte[] plainData, int length, ushort pss, ushort cseq, ushort sseq)
        {
            UInt32 seqValue = (uint)pss << 24 | (uint)cseq << 12 | (uint)sseq;
      
            // Create the packet that needs to be CRC32 checksummed
            PacketContent packet = new PacketContent();
            packet.AddUint16((UInt16)(plainData.Length + 4),1);
            packet.AddByteArray(TimeUtils.getCurrentTime());
            packet.AddUint32(seqValue,0);
            packet.AddByteArray(plainData);

            byte[] paddingBytes = getPaddedBytes(packet.ReturnFinalPacket());
            
            // Final Packet which needs to be encrypted
            PacketContent packetWithCrc = new PacketContent();
            packetWithCrc.AddByteArray(crc32.checksumB(packet.ReturnFinalPacket(),1));
            packetWithCrc.AddByteArray(packet.ReturnFinalPacket());
            packetWithCrc.AddByteArray(paddingBytes);
            string hexPackCrcedPlain = StringUtils.bytesToString(packetWithCrc.ReturnFinalPacket());
            string paddingBytesHex = StringUtils.bytesToString(paddingBytes);
            
            innerBuffer = new byte[packetWithCrc.ReturnFinalPacket().Length];
            tf.Encrypt(packetWithCrc.ReturnFinalPacket(),innerBuffer);
            PacketContent encryptedPacket = new PacketContent();
            encryptedPacket.AddByte(0x01);
            encryptedPacket.AddByteArray(IV);
            encryptedPacket.AddByteArray(innerBuffer);
            innerBuffer = new byte [2048];
            string hexPackEncrypt = StringUtils.bytesToString(encryptedPacket.ReturnFinalPacket());
            return encryptedPacket.ReturnFinalPacket();
        }

        public ArrayList decrypt(byte[] encryptedData, int length)
        {
            ArrayList response = new ArrayList();

            PacketReader readerEncrypted = new PacketReader(encryptedData);
            readerEncrypted.IncrementOffsetByValue(1); // skip 0x01
            IV = readerEncrypted.ReadBytes(16);
            tf.SetIV(IV);

            // Read the whole Data
            innerBuffer = new byte[encryptedData.Length - 17];
            byte[] decryptBuffer = readerEncrypted.ReadBytes(encryptedData.Length - 17);
            tf.Decrypt(decryptBuffer,innerBuffer);

            // just copy to clean
            byte[] decryptedPacket = innerBuffer;
            innerBuffer = new byte[2048];

            string decryptedPacketHex = StringUtils.bytesToString(decryptedPacket);
            PacketReader reader = new PacketReader(decryptedPacket);
            UInt32 crc32fromPacket = reader.ReadUInt32(1);
            UInt16 decryptionLen = (ushort) ((reader.ReadUInt16(1)) - 4 + 10);
            reader.SetOffsetOverrideValue(4);
            
            byte[] crc32FromPacketBytes = NumericalUtils.uint32ToByteArray(crc32fromPacket, 1); // ToDo: Remove when working
            
            // Verify CRC32 Data part
            byte[] packetCheckData = reader.ReadBytes(decryptionLen);
            UInt32 crc32plainDataToCompare = NumericalUtils.ByteArrayToUint32(crc32.checksumB(packetCheckData,1),1);

            if (crc32plainDataToCompare != crc32fromPacket)
            {
                Output.WriteLine("Oh oh, CRC didnt matched in this packet (From packet: " + StringUtils.bytesToString(crc32FromPacketBytes) +" generated: " + StringUtils.bytesToString(crc32.checksumB(packetCheckData,1)));
            }
            
            // as we read the whole packet for verifiy we reset the offset to read the other things
            reader.SetOffsetOverrideValue(4);
            UInt16 packetSize = reader.ReadUInt16(1);
//            UInt32 timeStamp = reader.readUInt32(1); // Well we didnt need it - but just read it as uint32
            reader.SetOffsetOverrideValue(10);
            UInt32 seqValues = reader.ReadUInt32(0);

            UInt16 pss=0;
            UInt16 cseq=0;
            UInt16 sseq=0;
            
            cseq = (UInt16)(seqValues&0xFFF);
            sseq = (UInt16) (seqValues >> 12&0xFFF);
            pss = (ushort) (seqValues >> 24&0xFF);

            // Finally add our responses and read the rest of the packet
            response.Add(reader.ReadBytes(packetSize-4));
            response.Add(pss);
            response.Add(cseq);
            response.Add(sseq);
			
            return response;
        }

        public byte[] getPaddedBytes(byte[] packet)
        {
            int totalLength = packet.Length + 4;
            int padding = 16 - (totalLength % 16);
            
            int i = 0;
            byte[] padded = new byte[padding];
           
            for (i = 0; i < padding; i++)
            {
                padded[i] += (byte)padding;
            }
            return padded;
        }
    }
}