using System;
using hds.Core.Wire;
using hds.Game.Objects;

namespace hds.Protocol.View
{
    /// <summary>
    /// Commands of the admin view (view id 1). Every admin view entry carries exactly one command:
    /// <code>
    /// create:  u8 0x0c (dynamic) / 0x08 (bound to static object: + u32 static id), u16 goId, u8 spawn counter,
    ///          u16 0xabcd, creation attributes, u16 new view id, followed by the first update of the new view
    /// delete:  u8 0x01, u16 count, u16 view ids
    /// pending: u8 0x02, u16 length (including itself), u16 view id, update data
    /// </code>
    /// Verified with the server packets of the logs (see ServerViewUpdateTests / the view tracking prototype).
    /// </summary>
    public static class AdminView
    {
        public const ushort ViewId = 1;
        public const byte CreateDynamic = 0x0c;
        public const byte CreateStatic = 0x08;
        public const byte Delete = 0x01;
        public const byte PendingUpdate = 0x02;
        private const ushort Separator = 0xabcd;

        /// <summary>Admin view entry that creates a dynamic object; <paramref name="firstUpdate"/> starts with the component mask (0 = none).</summary>
        public static byte[] Create(ushort goId, byte spawnCounter, GoAttributes attributes, ushort newViewId, ReadOnlySpan<byte> firstUpdate)
        {
            var writer = new WireWriter(64 + firstUpdate.Length);
            writer.WriteUInt16(ViewId);
            writer.WriteByte(CreateDynamic);
            writer.WriteUInt16(goId);
            writer.WriteByte(spawnCounter);
            writer.WriteUInt16(Separator);
            AttributeTableCodec.WriteCreation(writer, attributes);
            writer.WriteUInt16(newViewId);
            if (firstUpdate.IsEmpty)
            {
                writer.WriteByte(0);
            }
            else
            {
                writer.WriteBytes(firstUpdate);
            }
            return writer.ToArray();
        }

        public static byte[] DeleteViews(params ushort[] viewIds)
        {
            var writer = new WireWriter(5 + viewIds.Length * 2);
            writer.WriteUInt16(ViewId);
            writer.WriteByte(Delete);
            writer.WriteUInt16((ushort) viewIds.Length);
            foreach (var viewId in viewIds)
            {
                writer.WriteUInt16(viewId);
            }
            return writer.ToArray();
        }

        /// <summary>A plain view update entry: view id followed by the update (component mask + data).</summary>
        public static byte[] Update(ushort viewId, ReadOnlySpan<byte> update)
        {
            var writer = new WireWriter(2 + update.Length);
            writer.WriteUInt16(viewId);
            writer.WriteBytes(update);
            return writer.ToArray();
        }
    }
}
