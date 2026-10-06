using System.Collections.Generic;
using hds.Core.Wire;

namespace hds.Protocol.Rpc
{
    /// <summary>
    /// One RPC message of the 04 protocol. The opcode is a 15 bit number encoded like a var size
    /// (values &gt;= 0x80 are sent as two bytes with the high bit set, e.g. 0x80 0xAE = 0xAE).
    /// </summary>
    public readonly struct RpcMessage
    {
        public RpcMessage(ushort opcode, byte[] payload, ushort listCounter)
        {
            Opcode = opcode;
            Payload = payload;
            ListCounter = listCounter;
        }

        public ushort Opcode { get; }
        public byte[] Payload { get; }

        /// <summary>RPC counter of the list the message arrived in.</summary>
        public ushort ListCounter { get; }

        public override string ToString() => $"RPC 0x{Opcode:x3} ({Payload.Length} bytes)";
    }

    /// <summary>
    /// Reader/writer for the 04 block:
    /// <code>u8 listCount { u16be counter, varsize messageCount { varsize length, varsize opcode, payload } }</code>
    /// The message length includes the opcode bytes.
    /// </summary>
    public static class RpcBlockCodec
    {
        public static List<RpcMessage> Read(WireReader reader)
        {
            var result = new List<RpcMessage>();
            var lists = reader.ReadVarSize();
            for (var list = 0; list < lists; list++)
            {
                var counter = reader.ReadUInt16BigEndian();
                var messages = reader.ReadVarSize();
                for (var i = 0; i < messages; i++)
                {
                    var length = reader.ReadVarSize();
                    var start = reader.Position;
                    var opcode = (ushort) reader.ReadVarSize();
                    var payloadLength = length - (reader.Position - start);
                    if (payloadLength < 0) throw new WireFormatException($"RPC 0x{opcode:x} length {length} is shorter than its opcode");
                    result.Add(new RpcMessage(opcode, reader.ReadBytes(payloadLength), counter));
                }
            }
            return result;
        }

        /// <summary>Encodes one message body (opcode + payload) including its length prefix.</summary>
        public static void WriteMessage(WireWriter writer, ushort opcode, byte[] payload)
        {
            var opcodeSize = opcode > 0x7F ? 2 : 1;
            writer.WriteVarSize(opcodeSize + payload.Length);
            writer.WriteVarSize(opcode);
            writer.WriteBytes(payload);
        }

        /// <summary>
        /// Converts the legacy header constants (e.g. 0x80B2 = two byte header written raw) to an opcode.
        /// </summary>
        public static ushort FromLegacyHeader(int header) => (ushort) (header >= 0x8000 ? header & 0x7FFF : header);
    }
}
