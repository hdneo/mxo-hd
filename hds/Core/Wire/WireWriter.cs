using System;
using System.Buffers.Binary;
using System.Text;

namespace hds.Core.Wire
{
    /// <summary>
    /// Growable little endian writer for MxO wire data.
    /// </summary>
    public sealed class WireWriter
    {
        private byte[] buffer;

        public WireWriter(int initialCapacity = 64)
        {
            buffer = new byte[Math.Max(16, initialCapacity)];
        }

        public int Length { get; private set; }

        private Span<byte> Grow(int size)
        {
            if (Length + size > buffer.Length)
            {
                var newSize = Math.Max(buffer.Length * 2, Length + size);
                Array.Resize(ref buffer, newSize);
            }
            var span = buffer.AsSpan(Length, size);
            Length += size;
            return span;
        }

        public WireWriter WriteByte(byte value)
        {
            Grow(1)[0] = value;
            return this;
        }

        public WireWriter WriteBool(bool value) => WriteByte(value ? (byte) 1 : (byte) 0);

        public WireWriter WriteUInt16(ushort value)
        {
            BinaryPrimitives.WriteUInt16LittleEndian(Grow(2), value);
            return this;
        }

        public WireWriter WriteUInt16BigEndian(ushort value)
        {
            BinaryPrimitives.WriteUInt16BigEndian(Grow(2), value);
            return this;
        }

        public WireWriter WriteInt16(short value) => WriteUInt16((ushort) value);

        public WireWriter WriteUInt32(uint value)
        {
            BinaryPrimitives.WriteUInt32LittleEndian(Grow(4), value);
            return this;
        }

        public WireWriter WriteInt32(int value) => WriteUInt32((uint) value);

        public WireWriter WriteSingle(float value)
        {
            BinaryPrimitives.WriteSingleLittleEndian(Grow(4), value);
            return this;
        }

        public WireWriter WriteDouble(double value)
        {
            BinaryPrimitives.WriteDoubleLittleEndian(Grow(8), value);
            return this;
        }

        public WireWriter WriteBytes(ReadOnlySpan<byte> data)
        {
            data.CopyTo(Grow(data.Length));
            return this;
        }

        public WireWriter WriteZeros(int count)
        {
            Grow(count).Clear();
            return this;
        }

        /// <summary>Counterpart of <see cref="WireReader.ReadVarSize"/>.</summary>
        public WireWriter WriteVarSize(int value)
        {
            if (value < 0 || value > 0x7FFF) throw new ArgumentOutOfRangeException(nameof(value));
            if (value > 0x7F)
            {
                return WriteUInt16BigEndian((ushort) (value | 0x8000));
            }
            return WriteByte((byte) value);
        }

        public WireWriter WriteVector3d(double x, double y, double z) => WriteDouble(x).WriteDouble(y).WriteDouble(z);

        public WireWriter WriteVector3f(float x, float y, float z) => WriteSingle(x).WriteSingle(y).WriteSingle(z);

        public WireWriter WriteFixedString(string value, int size)
        {
            var target = Grow(size);
            target.Clear();
            if (!string.IsNullOrEmpty(value))
            {
                var bytes = Encoding.ASCII.GetBytes(value);
                bytes.AsSpan(0, Math.Min(bytes.Length, size)).CopyTo(target);
            }
            return this;
        }

        public WireWriter WriteSizedString(string value, bool zeroTerminated)
        {
            var bytes = Encoding.ASCII.GetBytes(value ?? string.Empty);
            WriteUInt16((ushort) (bytes.Length + (zeroTerminated ? 1 : 0)));
            WriteBytes(bytes);
            if (zeroTerminated) WriteByte(0);
            return this;
        }

        /// <summary>Overwrites an already written byte (e.g. a flag byte that is only known after the payload).</summary>
        public void PatchByte(int position, byte value)
        {
            if (position < 0 || position >= Length) throw new ArgumentOutOfRangeException(nameof(position));
            buffer[position] = value;
        }

        public void PatchUInt16(int position, ushort value)
        {
            if (position < 0 || position + 2 > Length) throw new ArgumentOutOfRangeException(nameof(position));
            BinaryPrimitives.WriteUInt16LittleEndian(buffer.AsSpan(position, 2), value);
        }

        public byte[] ToArray() => buffer.AsSpan(0, Length).ToArray();

        public ReadOnlySpan<byte> AsSpan() => buffer.AsSpan(0, Length);
    }
}
