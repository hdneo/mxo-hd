using System;
using System.Buffers.Binary;
using System.Text;

namespace hds.Core.Wire
{
    /// <summary>
    /// Bounds checked little endian reader for MxO wire data.
    /// All multi byte values in the MxO protocol are little endian unless noted otherwise.
    /// </summary>
    public sealed class WireReader
    {
        private readonly byte[] buffer;
        private readonly int end;

        public WireReader(byte[] buffer) : this(buffer, 0, buffer.Length)
        {
        }

        public WireReader(byte[] buffer, int offset, int count)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0 || count < 0 || offset + count > buffer.Length) throw new ArgumentOutOfRangeException(nameof(count));
            this.buffer = buffer;
            Position = offset;
            end = offset + count;
        }

        public int Position { get; set; }
        public int Remaining => end - Position;
        public bool EndOfData => Position >= end;

        private void Require(int size)
        {
            if (size < 0 || Position + size > end)
            {
                throw new WireFormatException($"Tried to read {size} bytes at offset {Position} but only {Remaining} are left");
            }
        }

        public byte PeekByte()
        {
            Require(1);
            return buffer[Position];
        }

        public byte ReadByte()
        {
            Require(1);
            return buffer[Position++];
        }

        public sbyte ReadSByte() => (sbyte) ReadByte();

        public bool ReadBool() => ReadByte() != 0;

        public ushort ReadUInt16()
        {
            Require(2);
            var value = BinaryPrimitives.ReadUInt16LittleEndian(buffer.AsSpan(Position, 2));
            Position += 2;
            return value;
        }

        /// <summary>Some legacy fields (e.g. the RPC counter) are big endian.</summary>
        public ushort ReadUInt16BigEndian()
        {
            Require(2);
            var value = BinaryPrimitives.ReadUInt16BigEndian(buffer.AsSpan(Position, 2));
            Position += 2;
            return value;
        }

        public short ReadInt16() => (short) ReadUInt16();

        public uint ReadUInt32()
        {
            Require(4);
            var value = BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan(Position, 4));
            Position += 4;
            return value;
        }

        public int ReadInt32() => (int) ReadUInt32();

        public float ReadSingle()
        {
            Require(4);
            var value = BinaryPrimitives.ReadSingleLittleEndian(buffer.AsSpan(Position, 4));
            Position += 4;
            return value;
        }

        public double ReadDouble()
        {
            Require(8);
            var value = BinaryPrimitives.ReadDoubleLittleEndian(buffer.AsSpan(Position, 8));
            Position += 8;
            return value;
        }

        public byte[] ReadBytes(int count)
        {
            Require(count);
            var result = new byte[count];
            Buffer.BlockCopy(buffer, Position, result, 0, count);
            Position += count;
            return result;
        }

        public ReadOnlySpan<byte> ReadSpan(int count)
        {
            Require(count);
            var span = new ReadOnlySpan<byte>(buffer, Position, count);
            Position += count;
            return span;
        }

        public void Skip(int count)
        {
            Require(count);
            Position += count;
        }

        public byte[] ReadToEnd() => ReadBytes(Remaining);

        /// <summary>
        /// Variable length size used by RPC framing: 1 byte, or 2 bytes big endian when the high bit is set.
        /// </summary>
        public int ReadVarSize()
        {
            int value = ReadByte();
            if (value >= 0x80)
            {
                value = ((value & 0x7F) << 8) | ReadByte();
            }
            return value;
        }

        public (double X, double Y, double Z) ReadVector3d() => (ReadDouble(), ReadDouble(), ReadDouble());

        public (float X, float Y, float Z) ReadVector3f() => (ReadSingle(), ReadSingle(), ReadSingle());

        /// <summary>Fixed size, zero padded ASCII string.</summary>
        public string ReadFixedString(int size)
        {
            var span = ReadSpan(size);
            var length = span.IndexOf((byte) 0);
            if (length < 0) length = size;
            return Encoding.ASCII.GetString(span.Slice(0, length));
        }

        /// <summary>uint16 length prefixed string. The length includes a trailing zero when <paramref name="zeroTerminated"/> is set.</summary>
        public string ReadSizedString(bool zeroTerminated)
        {
            int size = ReadUInt16();
            var span = ReadSpan(size);
            if (zeroTerminated && size > 0 && span[size - 1] == 0)
            {
                span = span.Slice(0, size - 1);
            }
            return Encoding.ASCII.GetString(span);
        }
    }

    public sealed class WireFormatException : Exception
    {
        public WireFormatException(string message) : base(message)
        {
        }
    }
}
