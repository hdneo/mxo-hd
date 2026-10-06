using System;
using System.Buffers.Binary;
using System.Text;

namespace hds.Game.Objects
{
    /// <summary>
    /// Attribute values of one game object instance, laid out like the creation list of its class.
    /// Tracks which attributes were set (sent on creation) and which changed since the last update.
    /// </summary>
    public sealed class GoAttributes
    {
        private readonly byte[][] values;
        private readonly bool[] assigned;
        private readonly bool[] dirty;

        public GoAttributes(GoClassSchema schema)
        {
            Schema = schema ?? throw new ArgumentNullException(nameof(schema));
            var count = schema.Create.Count;
            values = new byte[count][];
            assigned = new bool[count];
            dirty = new bool[count];
            for (var i = 0; i < count; i++)
            {
                values[i] = new byte[schema.Create[i].Size];
            }
        }

        public GoClassSchema Schema { get; }

        /// <summary>True if any attribute changed since <see cref="ClearDirty"/>.</summary>
        public bool AnyDirty { get; private set; }

        public int IndexOf(string name)
        {
            var index = Schema.CreateIndexOf(name);
            if (index < 0) throw new ArgumentException($"Attribute '{name}' does not exist in class {Schema.Key}");
            return index;
        }

        public bool Has(string name) => Schema.CreateIndexOf(name) >= 0;

        public bool IsAssigned(int index) => assigned[index];
        public bool IsDirty(int index) => dirty[index];

        public ReadOnlySpan<byte> GetRaw(int index) => values[index];
        public ReadOnlySpan<byte> GetRaw(string name) => values[IndexOf(name)];

        public void SetRaw(int index, ReadOnlySpan<byte> data)
        {
            var target = values[index];
            var changed = !assigned[index] || !data.SequenceEqual(target.AsSpan(0, Math.Min(target.Length, data.Length)));
            target.AsSpan().Clear();
            data.Slice(0, Math.Min(data.Length, target.Length)).CopyTo(target);
            assigned[index] = true;
            if (changed)
            {
                dirty[index] = true;
                AnyDirty = true;
            }
        }

        public void SetRaw(string name, ReadOnlySpan<byte> data) => SetRaw(IndexOf(name), data);

        /// <summary>Marks an attribute for the next update even if the value did not change.</summary>
        public void Touch(string name)
        {
            var index = IndexOf(name);
            assigned[index] = true;
            dirty[index] = true;
            AnyDirty = true;
        }

        public void ClearDirty()
        {
            Array.Clear(dirty, 0, dirty.Length);
            AnyDirty = false;
        }

        #region typed accessors

        public void SetByte(string name, byte value) => SetRaw(name, stackalloc byte[] {value});
        public void SetBool(string name, bool value) => SetByte(name, value ? (byte) 1 : (byte) 0);

        public void SetUInt16(string name, ushort value)
        {
            Span<byte> b = stackalloc byte[2];
            BinaryPrimitives.WriteUInt16LittleEndian(b, value);
            SetRaw(name, b);
        }

        public void SetInt16(string name, short value) => SetUInt16(name, (ushort) value);

        public void SetUInt32(string name, uint value)
        {
            Span<byte> b = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32LittleEndian(b, value);
            SetRaw(name, b);
        }

        public void SetSingle(string name, float value)
        {
            Span<byte> b = stackalloc byte[4];
            BinaryPrimitives.WriteSingleLittleEndian(b, value);
            SetRaw(name, b);
        }

        public void SetVector3d(string name, double x, double y, double z)
        {
            Span<byte> b = stackalloc byte[24];
            BinaryPrimitives.WriteDoubleLittleEndian(b, x);
            BinaryPrimitives.WriteDoubleLittleEndian(b.Slice(8), y);
            BinaryPrimitives.WriteDoubleLittleEndian(b.Slice(16), z);
            SetRaw(name, b);
        }

        public void SetVector3f(string name, float x, float y, float z)
        {
            Span<byte> b = stackalloc byte[12];
            BinaryPrimitives.WriteSingleLittleEndian(b, x);
            BinaryPrimitives.WriteSingleLittleEndian(b.Slice(4), y);
            BinaryPrimitives.WriteSingleLittleEndian(b.Slice(8), z);
            SetRaw(name, b);
        }

        public void SetString(string name, string value)
        {
            var index = IndexOf(name);
            var bytes = Encoding.ASCII.GetBytes(value ?? string.Empty);
            // keep one byte for the terminating zero
            var length = Math.Min(bytes.Length, values[index].Length - 1);
            SetRaw(index, bytes.AsSpan(0, Math.Max(0, length)));
        }

        public byte GetByte(string name) => values[IndexOf(name)][0];
        public bool GetBool(string name) => GetByte(name) != 0;
        public ushort GetUInt16(string name) => BinaryPrimitives.ReadUInt16LittleEndian(values[IndexOf(name)]);
        public uint GetUInt32(string name) => BinaryPrimitives.ReadUInt32LittleEndian(values[IndexOf(name)]);
        public float GetSingle(string name) => BinaryPrimitives.ReadSingleLittleEndian(values[IndexOf(name)]);

        public (double X, double Y, double Z) GetVector3d(string name)
        {
            var b = values[IndexOf(name)].AsSpan();
            return (BinaryPrimitives.ReadDoubleLittleEndian(b), BinaryPrimitives.ReadDoubleLittleEndian(b.Slice(8)),
                BinaryPrimitives.ReadDoubleLittleEndian(b.Slice(16)));
        }

        public string GetString(string name)
        {
            var b = values[IndexOf(name)].AsSpan();
            var length = b.IndexOf((byte) 0);
            return Encoding.ASCII.GetString(length < 0 ? b : b.Slice(0, length));
        }

        #endregion
    }
}
