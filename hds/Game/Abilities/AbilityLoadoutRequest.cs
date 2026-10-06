using System.Collections.Generic;
using hds.Core.Wire;

namespace hds.Game.Abilities
{
    public readonly struct AbilityLoadoutEntry
    {
        public AbilityLoadoutEntry(ushort slot, ushort abilityId, ushort level)
        {
            Slot = slot;
            AbilityId = abilityId;
            Level = level;
        }

        public ushort Slot { get; }
        public ushort AbilityId { get; }
        public ushort Level { get; }

        public override string ToString() => $"ability {AbilityId} level {Level} slot {Slot}";
    }

    /// <summary>
    /// Client RPC 0xAE (load / unload abilities at a hardline or in the loading area).
    /// <code>
    /// u32 static object id (hardline, 0 in the loading area)
    /// u16 offset of the unload list (0 = none)   offsets are relative to the payload start and point to
    /// u16 offset of the load list   (0 = none)   the first entry; the u16 entry count is stored right before it
    /// entry (9 bytes): u8 0, u16 slot, u16 ability id, u16 0, u16 level
    /// </code>
    /// A request can unload and load in one go (swapping a whole loadout). Verified with 706 logged requests.
    /// </summary>
    public sealed class AbilityLoadoutRequest
    {
        public const int EntrySize = 9;

        public uint StaticObjectId { get; private set; }
        public List<AbilityLoadoutEntry> Unload { get; } = new List<AbilityLoadoutEntry>();
        public List<AbilityLoadoutEntry> Load { get; } = new List<AbilityLoadoutEntry>();

        public static AbilityLoadoutRequest Parse(byte[] payload)
        {
            var reader = new WireReader(payload);
            var request = new AbilityLoadoutRequest {StaticObjectId = reader.ReadUInt32()};
            var unloadOffset = reader.ReadUInt16();
            var loadOffset = reader.ReadUInt16();
            ReadList(payload, unloadOffset, request.Unload);
            ReadList(payload, loadOffset, request.Load);
            return request;
        }

        private static void ReadList(byte[] payload, ushort offset, List<AbilityLoadoutEntry> target)
        {
            if (offset == 0) return;
            if (offset < 2) throw new WireFormatException($"Invalid ability list offset {offset}");

            var reader = new WireReader(payload) {Position = offset - 2};
            var count = reader.ReadUInt16();
            for (var i = 0; i < count; i++)
            {
                reader.ReadByte();
                var slot = reader.ReadUInt16();
                var abilityId = reader.ReadUInt16();
                reader.ReadUInt16();
                var level = reader.ReadUInt16();
                target.Add(new AbilityLoadoutEntry(slot, abilityId, level));
            }
        }

        /// <summary>Builds a request like the client does (used by tests and tools).</summary>
        public byte[] ToBytes()
        {
            var writer = new WireWriter();
            writer.WriteUInt32(StaticObjectId);
            var headerOffsetPosition = writer.Length;
            writer.WriteUInt16(0).WriteUInt16(0);
            if (Unload.Count > 0)
            {
                writer.WriteUInt16((ushort) Unload.Count);
                writer.PatchUInt16(headerOffsetPosition, (ushort) writer.Length);
                WriteEntries(writer, Unload);
            }
            if (Load.Count > 0)
            {
                writer.WriteUInt16((ushort) Load.Count);
                writer.PatchUInt16(headerOffsetPosition + 2, (ushort) writer.Length);
                WriteEntries(writer, Load);
            }
            return writer.ToArray();
        }

        private static void WriteEntries(WireWriter writer, List<AbilityLoadoutEntry> entries)
        {
            foreach (var entry in entries)
            {
                writer.WriteByte(0).WriteUInt16(entry.Slot).WriteUInt16(entry.AbilityId).WriteUInt16(0).WriteUInt16(entry.Level);
            }
        }

        public static AbilityLoadoutRequest Create(uint staticObjectId, IEnumerable<AbilityLoadoutEntry> unload, IEnumerable<AbilityLoadoutEntry> load)
        {
            var request = new AbilityLoadoutRequest {StaticObjectId = staticObjectId};
            if (unload != null) request.Unload.AddRange(unload);
            if (load != null) request.Load.AddRange(load);
            return request;
        }
    }
}
