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
            initCrypto();
        }

        public void initCrypto()
        {
            crc32 = new Crc32();
            tf = new MxoTwofish();
            tf.setIV(IV);
            tf.setKey(TF_Key);
        }
        
        public byte[] encrypt(byte[] plainData, int length, ushort pss, ushort cseq, ushort sseq)
        {
             
            UInt32 seqValue = (uint)pss << 24 | (uint)cseq << 12 | (uint)sseq;
            byte[] sequences = BitConverter.GetBytes(seqValue);
            Array.Reverse(sequences);
            
            // Create the packet that needs to be CRC32 checksummed
            PacketContent packet = new PacketContent();
            packet.addUint16((UInt16)(plainData.Length + 4),1);
            packet.addByteArray(TimeUtils.getCurrentTime());
            packet.addByteArray(sequences);
            packet.addByteArray(plainData);

            byte[] paddingBytes = getPaddedBytes(packet.returnFinalPacket());
            
            // Final Packet which needs to be encrypted
            PacketContent packetWithCrc = new PacketContent();
            packetWithCrc.addByteArray(crc32.checksumB(packet.returnFinalPacket(),1));
            packetWithCrc.addByteArray(packet.returnFinalPacket());
            packetWithCrc.addByteArray(paddingBytes);
            string hexPackCrcedPlain = StringUtils.bytesToString(packetWithCrc.returnFinalPacket());
            string paddingBytesHex = StringUtils.bytesToString(paddingBytes);
            
            innerBuffer = new byte[packetWithCrc.returnFinalPacket().Length];
            tf.encrypt(packetWithCrc.returnFinalPacket(),innerBuffer);
            PacketContent encryptedPacket = new PacketContent();
            encryptedPacket.addByte(0x01);
            encryptedPacket.addByteArray(IV);
            encryptedPacket.addByteArray(innerBuffer);
            innerBuffer = new byte [2048];
            string hexPackEncrypt = StringUtils.bytesToString(encryptedPacket.returnFinalPacket());
            return encryptedPacket.returnFinalPacket();
        }

        public ArrayList decrypt(byte[] encryptedData, int length)
        {
            ArrayList response = new ArrayList();
			
            UInt16 pss=0;
            UInt16 cseq=0;
            UInt16 sseq=0;
			
            PacketReader readerEncrypted = new PacketReader(encryptedData);
            readerEncrypted.incrementOffsetByValue(1); // skip 0x01
            IV = readerEncrypted.readBytes(16);
            tf.setIV(IV);

            // Read the whole Data
            innerBuffer = new byte[encryptedData.Length - 17];
            byte[] decryptBuffer = readerEncrypted.readBytes(encryptedData.Length - 17);
            tf.decrypt(decryptBuffer,innerBuffer);

            // ToDo: remove its just for debugging
            string hexData = StringUtils.bytesToString(innerBuffer);
            Output.writeToLogForConsole(hexData);

            // just copy to clean
            byte[] decryptedPacket = innerBuffer;
            innerBuffer = new byte[2048];
            
            PacketReader reader = new PacketReader(decryptedPacket);
            UInt32 crc32fromPacket = reader.readUInt32(1);

            byte[] packetCheckData = reader.readBytes(decryptedPacket.Length - 4);
            UInt32 crc32plainDataToCompare = NumericalUtils.ByteArrayToUint32(crc32.checksumB(packetCheckData,1),1);

            if (crc32plainDataToCompare != crc32fromPacket)
            {
                Output.WriteLine("Oh oh, CRC didnt matched in this packet");
            }
            
            // as we read the whole packet for verifiy we reset the offset to read the other things
            reader.setOffsetOverrideValue(4);
            UInt16 packetSize = reader.readUInt16(1);
            UInt32 timeStamp = reader.readUInt32(1); // Well we didnt need it - but just read it as uint32
            UInt32 seqValues = reader.readUInt32(0);
            
            cseq = (UInt16)(seqValues&0xFFF);
            sseq = (UInt16) (seqValues >> 12&0xFFF);
            pss = (UInt16) (seqValues >> 24&0xFF);
            
            
            // Finally add our responses and read the rest of the packet
            response.Add(reader.readBytes(packetSize-4));
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