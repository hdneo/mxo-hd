using System;
using System.Collections.Generic;
using System.Text;

namespace hds
{
    public class PacketContent
    {
        private DynamicArray packet = new DynamicArray();

        public void AddHexBytes(String hexbytes)
        {
            packet.append(StringUtils.hexStringToBytes(hexbytes));
        }

        public void AddInt16(Int16 value, int reversed)
        {
            packet.append(NumericalUtils.int16ToByteArray(value, reversed));
        }

        public void AddInt32(Int32 value, int reversed)
        {
            packet.append(NumericalUtils.int32ToByteArray(value, reversed));
        }
        public void AddUint32(UInt32 value,int reversed)
        {
            packet.append(NumericalUtils.uint32ToByteArray(value, reversed));
        }

        public void AddUint16(UInt16 value, int reversed)
        {
            packet.append(NumericalUtils.uint16ToByteArray(value, reversed));
        }

        public void AddUShort(UInt16 value)
        {
            packet.append(NumericalUtils.uint16ToByteArrayShort(value));
        }

        public void AddByteArray(byte[] array)
        {
            packet.append(array);
        }

        public void AddByte(byte value)
        {
            packet.append(value);
        }

        public void AddFloat(float value, int reversed)
        {
            packet.append(NumericalUtils.floatToByteArray(value, reversed));
        }

        public void AddDouble(double value, int reversed)
        {
            packet.append(NumericalUtils.doubleToByteArray(value, reversed));
        }

        public void AddFloatLtVector3f(float x, float y, float z)
        {
            packet.append(NumericalUtils.floatsToLtVector3f(x, y, z));
        }

        public void AddDoubleLtVector3d(double x, double y, double z)
        {
            packet.append(NumericalUtils.doublesToLtVector3d(x, y, z));
        }

        public byte[] ReturnFinalPacket()
        {
            return packet.getBytes();
        }

        public void AddString(string value)
        {
            packet.append(StringUtils.stringToBytes(value));
        }

        public void AddSizedTerminatedString(string value)
        {
            int size = value.Length + 1;
            packet.append(NumericalUtils.uint16ToByteArray((UInt16)size, 1));
            packet.append(StringUtils.stringToBytes(value));
            packet.append(0x00);
        }

        public void AddStringWithFixedSized(string value,int size)
        {
            packet.append(StringUtils.stringToBytes(value));
            int paddingSize = size - value.Length;
            int currentSize = value.Length;
            if (paddingSize > 0)
            {
                // we need to add "0x00" bytes until its full
                while (currentSize <= paddingSize + value.Length)
                {
                    packet.append(0x00);
                    currentSize++;
                }

                int len = packet.getSize();
            }
        }

        public void AddSizedString(string value)
        {
            packet.append(NumericalUtils.uint16ToByteArray((UInt16)value.Length, 1));
            packet.append(StringUtils.stringToBytes(value));

        }
       
    }
}
