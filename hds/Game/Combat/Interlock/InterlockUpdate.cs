using System;
using System.Collections.Generic;
using hds.Core.Wire;

namespace hds.Game.Combat.Interlock
{
    /// <summary>A reference to a move of the interlock database (rez file 0x24000B8B).</summary>
    public struct InterlockMoveRef
    {
        public const uint DefaultDatabase = 0x24000B8B;

        public uint MoveId;
        public uint Database;

        public bool IsSet => Database != 0;

        public static InterlockMoveRef None => new InterlockMoveRef();
        public static InterlockMoveRef Of(int moveId) => new InterlockMoveRef {MoveId = (uint) moveId, Database = DefaultDatabase};

        public override string ToString() => IsSet ? $"move {MoveId}" : "none";
    }

    /// <summary>
    /// One exchange of an interlock round. Field sizes are verified with 1570 length-prefixed logged updates;
    /// the meaning of some fields is derived from the client combat scripts (combat_resolution.py) and statistics.
    /// </summary>
    public sealed class InterlockExchange
    {
        /// <summary>Role of combatant A and B in this exchange: 1 = attacker, 2 = defender (0 = not involved).</summary>
        public byte RoleA = 1, RoleB = 2;
        /// <summary>Duration of the style transition of combatant A / B in ms (0 = no transition).</summary>
        public ushort TransitionTimeA, TransitionTimeB;
        /// <summary>Position of combatant A and B relative to the handler position.</summary>
        public float AX, AY, AZ, BX, BY, BZ;
        /// <summary>Unknown, 0-7 in the logs (1024 rarely).</summary>
        public ushort Unknown3;
        /// <summary>Round counter of the fight.</summary>
        public ushort Round;
        /// <summary>Milliseconds since the fight started (handler StartTime).</summary>
        public uint Time;
        /// <summary>Signed start offset of the exchange and the animation durations of both sides (ms).</summary>
        public short ExchangeStart;
        public ushort DurationA, DurationB;
        /// <summary>Bit 0/1: transition moves present, 0x20 opportunity attack, 0x80 seen with finishing / bullet time.</summary>
        public byte Flags;
        public InterlockMoveRef TransitionA, TransitionB, Exchange;
        /// <summary>Combat tactic totals of both combatants (result.attackerValue / defenderValue of the client script).</summary>
        public uint ValueA, ValueB;
        /// <summary>Damage scale of both sides in 1/4096 (4096 = 100%, dodged: ~500).</summary>
        public ushort DamageScaleA = 4096, DamageScaleB = 4096;
        /// <summary>Unknown per side values, pairs like (0,1), (16,2), (0,0) in the logs.</summary>
        public uint TailA1, TailA2, TailB1, TailB2;
        /// <summary>Equipped weapon GO ids (0 = unarmed).</summary>
        public uint WeaponA, WeaponB;

        internal static InterlockExchange Read(WireReader r)
        {
            var e = new InterlockExchange
            {
                RoleA = r.ReadByte(), RoleB = r.ReadByte(),
                TransitionTimeA = r.ReadUInt16(), TransitionTimeB = r.ReadUInt16(),
                AX = r.ReadSingle(), AY = r.ReadSingle(), AZ = r.ReadSingle(),
                BX = r.ReadSingle(), BY = r.ReadSingle(), BZ = r.ReadSingle(),
                Unknown3 = r.ReadUInt16(), Round = r.ReadUInt16(), Time = r.ReadUInt32(),
                ExchangeStart = r.ReadInt16(), DurationA = r.ReadUInt16(), DurationB = r.ReadUInt16(),
                Flags = r.ReadByte(),
            };
            e.TransitionA = ReadMove(r);
            e.TransitionB = ReadMove(r);
            e.Exchange = ReadMove(r);
            var zeros = r.ReadSpan(16);
            foreach (var b in zeros)
            {
                if (b != 0) throw new WireFormatException("Interlock exchange reserved bytes are not zero");
            }
            e.ValueA = r.ReadUInt32();
            e.ValueB = r.ReadUInt32();
            e.DamageScaleA = r.ReadUInt16();
            e.DamageScaleB = r.ReadUInt16();
            e.TailA1 = r.ReadUInt32(); e.TailA2 = r.ReadUInt32(); e.TailB1 = r.ReadUInt32(); e.TailB2 = r.ReadUInt32();
            e.WeaponA = r.ReadUInt32();
            e.WeaponB = r.ReadUInt32();
            return e;
        }

        internal void Write(WireWriter w)
        {
            w.WriteByte(RoleA).WriteByte(RoleB).WriteUInt16(TransitionTimeA).WriteUInt16(TransitionTimeB)
                .WriteSingle(AX).WriteSingle(AY).WriteSingle(AZ).WriteSingle(BX).WriteSingle(BY).WriteSingle(BZ)
                .WriteUInt16(Unknown3).WriteUInt16(Round).WriteUInt32(Time)
                .WriteInt16(ExchangeStart).WriteUInt16(DurationA).WriteUInt16(DurationB).WriteByte(Flags);
            WriteMove(w, TransitionA);
            WriteMove(w, TransitionB);
            WriteMove(w, Exchange);
            w.WriteZeros(16);
            w.WriteUInt32(ValueA).WriteUInt32(ValueB).WriteUInt16(DamageScaleA).WriteUInt16(DamageScaleB)
                .WriteUInt32(TailA1).WriteUInt32(TailA2).WriteUInt32(TailB1).WriteUInt32(TailB2)
                .WriteUInt32(WeaponA).WriteUInt32(WeaponB);
        }

        private static InterlockMoveRef ReadMove(WireReader r) => new InterlockMoveRef {MoveId = r.ReadUInt32(), Database = r.ReadUInt32()};

        private static void WriteMove(WireWriter w, InterlockMoveRef move) => w.WriteUInt32(move.MoveId).WriteUInt32(move.Database);
    }

    /// <summary>
    /// View update of the ILCombatHandler game object (GO 55): component mask, then the interlock component
    /// (bit 0) and the attribute table with the position (bit 1).
    /// <code>
    /// interlock component: vec3d position, u32 round, u32 flags
    ///   [flags bit 31: u32 mode] [bit 0: u16 view + u16 spawn of A] [bit 1: same for B]
    ///   u8 exchange count, exchanges
    /// </code>
    /// </summary>
    public sealed class InterlockUpdate
    {
        public const byte ComponentInterlock = 0x01;
        public const byte ComponentTable = 0x02;

        public byte ComponentMask = ComponentInterlock;
        public double X, Y, Z;
        public uint Round;
        public uint? Mode;
        public (ushort View, ushort Spawn)? CombatantA;
        public (ushort View, ushort Spawn)? CombatantB;
        public List<InterlockExchange> Exchanges { get; } = new List<InterlockExchange>();
        /// <summary>Attribute table (bit 1 of the component mask): the update list only contains Position.</summary>
        public bool TableHasPosition = true;
        public double TableX, TableY, TableZ;

        public static InterlockUpdate Read(WireReader r)
        {
            var u = new InterlockUpdate {ComponentMask = r.ReadByte()};
            if ((u.ComponentMask & ~(ComponentInterlock | ComponentTable)) != 0)
            {
                throw new WireFormatException($"Unknown IL component mask 0x{u.ComponentMask:x2}");
            }
            if ((u.ComponentMask & ComponentInterlock) != 0)
            {
                (u.X, u.Y, u.Z) = r.ReadVector3d();
                u.Round = r.ReadUInt32();
                var flags = r.ReadUInt32();
                if ((flags & ~0x80000003u) != 0) throw new WireFormatException($"Unknown IL flags 0x{flags:x}");
                if ((flags & 0x80000000u) != 0) u.Mode = r.ReadUInt32();
                if ((flags & 1) != 0) u.CombatantA = (r.ReadUInt16(), r.ReadUInt16());
                if ((flags & 2) != 0) u.CombatantB = (r.ReadUInt16(), r.ReadUInt16());
                var count = r.ReadByte();
                for (var i = 0; i < count; i++)
                {
                    u.Exchanges.Add(InterlockExchange.Read(r));
                }
            }
            if ((u.ComponentMask & ComponentTable) != 0)
            {
                var mask = r.ReadByte();
                if ((mask & ~0x01) != 0) throw new WireFormatException($"Unexpected IL attribute table mask 0x{mask:x2}");
                u.TableHasPosition = mask == 0x01;
                if (u.TableHasPosition) (u.TableX, u.TableY, u.TableZ) = r.ReadVector3d();
            }
            return u;
        }

        public void Write(WireWriter w)
        {
            w.WriteByte(ComponentMask);
            if ((ComponentMask & ComponentInterlock) != 0)
            {
                w.WriteVector3d(X, Y, Z);
                w.WriteUInt32(Round);
                uint flags = 0;
                if (Mode.HasValue) flags |= 0x80000000u;
                if (CombatantA.HasValue) flags |= 1;
                if (CombatantB.HasValue) flags |= 2;
                w.WriteUInt32(flags);
                if (Mode.HasValue) w.WriteUInt32(Mode.Value);
                if (CombatantA.HasValue) w.WriteUInt16(CombatantA.Value.View).WriteUInt16(CombatantA.Value.Spawn);
                if (CombatantB.HasValue) w.WriteUInt16(CombatantB.Value.View).WriteUInt16(CombatantB.Value.Spawn);
                if (Exchanges.Count > byte.MaxValue) throw new InvalidOperationException("Too many exchanges");
                w.WriteByte((byte) Exchanges.Count);
                foreach (var exchange in Exchanges)
                {
                    exchange.Write(w);
                }
            }
            if ((ComponentMask & ComponentTable) != 0)
            {
                w.WriteByte(TableHasPosition ? (byte) 0x01 : (byte) 0x00);
                if (TableHasPosition) w.WriteVector3d(TableX, TableY, TableZ);
            }
        }

        public byte[] ToArray()
        {
            var w = new WireWriter(200);
            Write(w);
            return w.ToArray();
        }
    }
}
