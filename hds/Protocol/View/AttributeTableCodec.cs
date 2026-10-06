using System;
using System.Collections.Generic;
using hds.Core.Wire;
using hds.Game.Objects;

namespace hds.Protocol.View
{
    /// <summary>
    /// Ordered list of attributes that make up one serialized attribute table (update list, self view list, ...),
    /// mapped onto the creation layout of a <see cref="GoAttributes"/> instance.
    /// </summary>
    public sealed class AttributeTableLayout
    {
        public AttributeTableLayout(GoClassSchema schema, IReadOnlyList<GoAttributeDef> attributes)
        {
            Attributes = attributes;
            CreateIndices = new int[attributes.Count];
            for (var i = 0; i < attributes.Count; i++)
            {
                CreateIndices[i] = schema.CreateIndexOf(attributes[i].Name);
            }
        }

        public IReadOnlyList<GoAttributeDef> Attributes { get; }

        /// <summary>For every table entry the index in the creation list (-1 if the object has no such attribute).</summary>
        public int[] CreateIndices { get; }

        public static AttributeTableLayout ForUpdates(GoClassSchema schema) => new AttributeTableLayout(schema, schema.Update);

        /// <summary>Builds a layout from attribute names (e.g. the player self view list).</summary>
        public static AttributeTableLayout FromNames(GoClassSchema schema, IEnumerable<string> names)
        {
            var list = new List<GoAttributeDef>();
            foreach (var name in names)
            {
                var index = schema.CreateIndexOf(name);
                if (index < 0) throw new ArgumentException($"Attribute '{name}' is not part of class {schema.Key}");
                list.Add(schema.Create[index]);
            }
            return new AttributeTableLayout(schema, list);
        }
    }

    /// <summary>
    /// Serializer for 03 attribute tables. Attributes are grouped by 7, every group starts with a bit mask
    /// (bit n = attribute n of the group is present, bit 7 = another group follows).
    /// </summary>
    public static class AttributeTableCodec
    {
        /// <summary>
        /// Writes the creation block: attribute count, group masks and fixed size values of all assigned attributes.
        /// </summary>
        public static void WriteCreation(WireWriter writer, GoAttributes attributes)
        {
            var list = attributes.Schema.Create;
            var count = 0;
            var last = -1;
            for (var i = 0; i < list.Count; i++)
            {
                if (!attributes.IsAssigned(i)) continue;
                count++;
                last = i;
            }

            writer.WriteByte((byte) count);
            if (count == 0) return;

            for (var groupStart = 0; groupStart <= last; groupStart += 7)
            {
                var maskPosition = writer.Length;
                writer.WriteByte(0);
                byte mask = 0;
                for (var bit = 0; bit < 7 && groupStart + bit < list.Count; bit++)
                {
                    var index = groupStart + bit;
                    if (!attributes.IsAssigned(index)) continue;
                    mask |= (byte) (1 << bit);
                    writer.WriteBytes(attributes.GetRaw(index));
                }
                if (groupStart + 7 <= last) mask |= 0x80;
                writer.PatchByte(maskPosition, mask);
            }
        }

        /// <summary>Number of table entries that would be written for the given filter.</summary>
        public static int Count(GoAttributes attributes, AttributeTableLayout layout, Func<int, bool> include)
        {
            var count = 0;
            foreach (var index in layout.CreateIndices)
            {
                if (index >= 0 && include(index)) count++;
            }
            return count;
        }

        /// <summary>
        /// Writes an update table (group masks + values, no count byte). Strings are u8 length prefixed.
        /// Returns the number of written attributes; nothing is written when it is zero.
        /// </summary>
        public static int WriteTable(WireWriter writer, GoAttributes attributes, AttributeTableLayout layout, Func<int, bool> include)
        {
            var indices = layout.CreateIndices;
            var last = -1;
            for (var i = 0; i < indices.Length; i++)
            {
                if (indices[i] >= 0 && include(indices[i])) last = i;
            }
            if (last < 0) return 0;

            var written = 0;
            for (var groupStart = 0; groupStart <= last; groupStart += 7)
            {
                var maskPosition = writer.Length;
                writer.WriteByte(0);
                byte mask = 0;
                for (var bit = 0; bit < 7 && groupStart + bit < indices.Length; bit++)
                {
                    var entry = groupStart + bit;
                    var index = indices[entry];
                    if (index < 0 || !include(index)) continue;
                    mask |= (byte) (1 << bit);
                    WriteValue(writer, layout.Attributes[entry], attributes.GetRaw(index));
                    written++;
                }
                if (groupStart + 7 <= last) mask |= 0x80;
                writer.PatchByte(maskPosition, mask);
            }
            return written;
        }

        private static void WriteValue(WireWriter writer, GoAttributeDef def, ReadOnlySpan<byte> value)
        {
            if (!def.IsString)
            {
                writer.WriteBytes(value);
                return;
            }
            var length = value.IndexOf((byte) 0);
            if (length < 0) length = value.Length;
            writer.WriteByte((byte) length);
            writer.WriteBytes(value.Slice(0, length));
        }

        /// <summary>Reads an update table. Returns (table index, raw value) pairs; strings are returned without prefix.</summary>
        public static List<(int Index, byte[] Value)> ReadTable(WireReader reader, IReadOnlyList<GoAttributeDef> list)
        {
            var result = new List<(int, byte[])>();
            for (var groupStart = 0;; groupStart += 7)
            {
                var mask = reader.ReadByte();
                for (var bit = 0; bit < 7; bit++)
                {
                    if ((mask & (1 << bit)) == 0) continue;
                    var index = groupStart + bit;
                    if (index >= list.Count) throw new WireFormatException($"Attribute table index {index} exceeds {list.Count} entries");
                    var def = list[index];
                    var value = def.IsString ? reader.ReadBytes(reader.ReadByte()) : reader.ReadBytes(def.Size);
                    result.Add((index, value));
                }
                if ((mask & 0x80) == 0) break;
            }
            return result;
        }

        /// <summary>Reads a creation block (count byte + groups with fixed size values).</summary>
        public static List<(int Index, byte[] Value)> ReadCreation(WireReader reader, IReadOnlyList<GoAttributeDef> list)
        {
            var result = new List<(int, byte[])>();
            var count = reader.ReadByte();
            if (count == 0) return result;
            for (var groupStart = 0;; groupStart += 7)
            {
                var mask = reader.ReadByte();
                for (var bit = 0; bit < 7; bit++)
                {
                    if ((mask & (1 << bit)) == 0) continue;
                    var index = groupStart + bit;
                    if (index >= list.Count) throw new WireFormatException($"Creation index {index} exceeds {list.Count} entries");
                    result.Add((index, reader.ReadBytes(list[index].Size)));
                }
                if ((mask & 0x80) == 0) break;
            }
            if (result.Count != count) throw new WireFormatException($"Creation block announced {count} attributes but contained {result.Count}");
            return result;
        }
    }
}
