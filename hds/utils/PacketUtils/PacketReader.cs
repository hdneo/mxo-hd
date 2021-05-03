using System;

namespace hds
{
    public class PacketReader
    {
        private byte[] packetData;
        private int offset = 0;
        private int lenght = 0;

        public PacketReader(byte[] packetData)
        {
            this.packetData = packetData;
            if (packetData.Length > 0)
            {
                lenght = packetData.Length;
            }
        }

        public void IncrementOffsetByValue(int value)
        {
            offset = offset + value;
        }

        public void SetOffsetOverrideValue(int value)
        {
            offset = value;
        }

        public int GetOffset()
        {
            return offset;
        }

        public uint ReadUint8()
        {
            uint value = 0;
            value = (uint) packetData[offset];
            offset++;
            return value;

        }

        public UInt16 ReadUInt16(int reversed)
        {
            UInt16 value = 0;
            value = NumericalUtils.ByteArrayToUint16(new[]{packetData[offset], packetData[offset+1]},reversed);
            offset = offset + 2;
            return value;
        }

        public UInt32 ReadUInt32(int reversed)
        {
            UInt32 value = 0;
            value = NumericalUtils.ByteArrayToUint32(new[]{packetData[offset], packetData[offset+1], packetData[offset+2], packetData[offset+3]},reversed);
            offset = offset + 4;
            return value;
        }

        public float ReadFloat(int reversed)
        {
            float value = 0;
            value = NumericalUtils.byteArrayToFloat(new byte[]{packetData[offset], packetData[offset+1], packetData[offset+2], packetData[offset+3]},reversed);
            offset = offset + 4;
            return value;
        }

        public double ReadDouble(int reversed)
        {
            double value = 0;
            value = NumericalUtils.byteArrayToDouble(new byte[]{packetData[offset], packetData[offset+1], packetData[offset+2], packetData[offset+3], packetData[offset+4], packetData[offset+5], packetData[offset+6], packetData[offset+7]},reversed);
            offset = offset + 8;
            return value;
        }

        public string ReadSizedString()
        {
            string value = "";
            UInt16 sizeOfString = ReadUInt16(1);

            byte[] stringBytes = new byte[sizeOfString];

            ArrayUtils.copy(packetData, offset, stringBytes, 0, sizeOfString);
            value = StringUtils.charBytesToString(stringBytes);
            offset = offset + sizeOfString;
            return value;
        }

        public string ReadSizedZeroTerminatedString()
        {
            string value = "";
            UInt16 sizeOfString = ReadUInt16(1);

            byte[] stringBytes = new byte[sizeOfString - 1];

            ArrayUtils.copy(packetData, offset , stringBytes, 0, sizeOfString - 1); // -1 strips the zero byte
            value = StringUtils.charBytesToString(stringBytes);
            offset = offset + sizeOfString; // just one for the zero termination
            return value;
        }

        public byte[] ReadBytes(int size)
        {
            // Just read amount of bytes
            byte[] newByteArray = new byte[size];
            ArrayUtils.copy(packetData,offset,newByteArray,0,size);
            offset = offset + size;
            return newByteArray;
        }

        public byte[] ReturnFullData()
        {
            return packetData;
        }
    }
}