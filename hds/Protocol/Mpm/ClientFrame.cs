using System.Collections.Generic;
using hds.Core.Wire;
using hds.Game.Objects;
using hds.Protocol.Rpc;
using hds.Protocol.View;

namespace hds.Protocol.Mpm
{
    /// <summary>
    /// A decrypted MPM packet sent by the client.
    /// <code>
    /// u8 flags   (0x02 = ack, 0x20 = reset done, 0x40 = reset rcc, 0x80 = sim time follows)
    /// [f32 sim time]
    /// [0x03 { u16 viewId, view update } u16 0]   object/view protocol (the client only updates its self view 2)
    /// [0x04 rpc block]                           RPC protocol
    /// [0x05 u32]
    /// </code>
    /// </summary>
    public sealed class ClientFrame
    {
        public const byte FlagAck = 0x02;
        public const byte FlagResetDone = 0x20;
        public const byte FlagResetRcc = 0x40;
        public const byte FlagSimTime = 0x80;

        public byte Flags;
        public float SimTime;
        public readonly List<(ushort ViewId, DecodedViewUpdate Update)> Views = new List<(ushort, DecodedViewUpdate)>();
        public List<RpcMessage> Rpcs = new List<RpcMessage>();
        public byte[] Block05;

        public bool IsAckOnly => Views.Count == 0 && Rpcs.Count == 0 && Block05 == null;

        public bool HasFlag(byte flag) => (Flags & flag) != 0;

        /// <summary>Layout of the self view updates the client sends.</summary>
        public static ViewLayout SelfViewLayout =>
            selfLayout ??= ViewLayout.For(GoSchemaRegistry.Instance.Get(ViewLayout.PlayerCharacterGoId), true);

        private static ViewLayout selfLayout;

        public static ClientFrame Parse(byte[] data) => Parse(data, SelfViewLayout);

        public static ClientFrame Parse(byte[] data, ViewLayout selfViewLayout)
        {
            var reader = new WireReader(data);
            var frame = new ClientFrame {Flags = reader.ReadByte()};
            if (frame.HasFlag(FlagSimTime)) frame.SimTime = reader.ReadSingle();

            while (!reader.EndOfData)
            {
                var block = reader.ReadByte();
                switch (block)
                {
                    case 0x03:
                        ushort viewId;
                        while ((viewId = reader.ReadUInt16()) != 0)
                        {
                            if (viewId != 2)
                            {
                                throw new WireFormatException($"Client sent an update for view {viewId}, only the self view (2) is known");
                            }
                            frame.Views.Add((viewId, DecodedViewUpdate.Read(reader, selfViewLayout, false, true)));
                        }
                        break;
                    case 0x04:
                        frame.Rpcs = RpcBlockCodec.Read(reader);
                        break;
                    case 0x05:
                        frame.Block05 = reader.ReadBytes(4);
                        break;
                    default:
                        throw new WireFormatException($"Unknown MPM block 0x{block:x2} at offset {reader.Position - 1}");
                }
            }
            return frame;
        }
    }
}
