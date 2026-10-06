using System;
using hds.Core.Wire;

namespace hds.Protocol.View
{
    /// <summary>
    /// Character movement sub view (client side: CLTGOCmpCharMvtClient).
    ///
    /// Wire layout, all fields optional and selected by the leading flag byte, in this order:
    /// <code>
    /// 0x20 script block   client: u8 script counter
    ///                     server: u8 counter, u8 flags, u8 stance, u8 action,
    ///                             [action 0x25/0x29 (emotes): u8 emote], u16 anim id,
    ///                             [flags&amp;0x3F==1: u32 target] [==2: u32 target, vec3d, f32]
    ///                             [==3: vec3d position, f32 rotation]
    ///                     flags: 0x40 = static object, 0x80 = no interpolation (no extra data)
    /// 0x10 teleport       u8 counter, u16 sector, u16 zone, u16 unknown (server only)
    /// 0x01 stance         u8 stance, u8 demeanor (server only)
    /// 0x02 action         u8 action
    /// 0x04 yaw            u8 yaw (0..255 = 0..360 degrees)
    /// 0x08 position       3 x f32; the server adds u32 sim time for the owner's self view
    /// </code>
    /// Verified against all client packets and the server view updates of the packet logs.
    /// </summary>
    public sealed class CharMvt
    {
        public const byte FlagStance = 0x01;
        public const byte FlagAction = 0x02;
        public const byte FlagYaw = 0x04;
        public const byte FlagPosition = 0x08;
        public const byte FlagTeleport = 0x10;
        public const byte FlagScript = 0x20;

        public byte Flags;

        public byte Stance;
        public byte Demeanor;
        public byte Action;
        public byte Yaw;
        public float X, Y, Z;
        public uint SimTime;

        public CharMvtTeleport Teleport;
        public CharMvtScript Script;

        /// <summary>Script counter sent by the client (it only reports the counter it started).</summary>
        public byte ClientScriptCounter;

        public bool Has(byte flag) => (Flags & flag) != 0;

        public void SetPosition(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
            Flags |= FlagPosition;
        }

        #region client -> server

        public static CharMvt ReadFromClient(WireReader reader)
        {
            var mvt = new CharMvt {Flags = reader.ReadByte()};
            if (mvt.Has(FlagScript)) mvt.ClientScriptCounter = reader.ReadByte();
            if (mvt.Has(FlagTeleport)) mvt.Teleport = CharMvtTeleport.Read(reader);
            if (mvt.Has(FlagStance))
            {
                mvt.Stance = reader.ReadByte();
                mvt.Demeanor = reader.ReadByte();
            }
            if (mvt.Has(FlagAction)) mvt.Action = reader.ReadByte();
            if (mvt.Has(FlagYaw)) mvt.Yaw = reader.ReadByte();
            if (mvt.Has(FlagPosition))
            {
                mvt.X = reader.ReadSingle();
                mvt.Y = reader.ReadSingle();
                mvt.Z = reader.ReadSingle();
            }
            if ((mvt.Flags & 0xC0) != 0) throw new WireFormatException($"Unknown char movement flags 0x{mvt.Flags:x2}");
            return mvt;
        }

        #endregion

        #region server -> client

        public static CharMvt ReadFromServer(WireReader reader, bool selfView)
        {
            var mvt = new CharMvt {Flags = reader.ReadByte()};
            if (mvt.Has(FlagScript)) mvt.Script = CharMvtScript.Read(reader);
            if (mvt.Has(FlagTeleport)) mvt.Teleport = CharMvtTeleport.Read(reader);
            if (mvt.Has(FlagStance))
            {
                mvt.Stance = reader.ReadByte();
                mvt.Demeanor = reader.ReadByte();
            }
            if (mvt.Has(FlagAction)) mvt.Action = reader.ReadByte();
            if (mvt.Has(FlagYaw)) mvt.Yaw = reader.ReadByte();
            if (mvt.Has(FlagPosition))
            {
                mvt.X = reader.ReadSingle();
                mvt.Y = reader.ReadSingle();
                mvt.Z = reader.ReadSingle();
                if (selfView) mvt.SimTime = reader.ReadUInt32();
            }
            if ((mvt.Flags & 0xC0) != 0) throw new WireFormatException($"Unknown char movement flags 0x{mvt.Flags:x2}");
            return mvt;
        }

        public void WriteToClient(WireWriter writer, bool selfView)
        {
            writer.WriteByte(Flags);
            if (Has(FlagScript)) (Script ?? throw new InvalidOperationException("Script flag without script block")).Write(writer);
            if (Has(FlagTeleport)) (Teleport ?? throw new InvalidOperationException("Teleport flag without teleport block")).Write(writer);
            if (Has(FlagStance))
            {
                writer.WriteByte(Stance);
                writer.WriteByte(Demeanor);
            }
            if (Has(FlagAction)) writer.WriteByte(Action);
            if (Has(FlagYaw)) writer.WriteByte(Yaw);
            if (Has(FlagPosition))
            {
                writer.WriteSingle(X).WriteSingle(Y).WriteSingle(Z);
                if (selfView) writer.WriteUInt32(SimTime);
            }
        }

        #endregion
    }

    /// <summary>Teleport block (flag 0x10): the client reloads its sector/zone.</summary>
    public sealed class CharMvtTeleport
    {
        public byte Counter;
        public ushort Sector;
        public ushort Zone;
        public ushort Unknown;

        public static CharMvtTeleport Read(WireReader reader) => new CharMvtTeleport
        {
            Counter = reader.ReadByte(),
            Sector = reader.ReadUInt16(),
            Zone = reader.ReadUInt16(),
            Unknown = reader.ReadUInt16()
        };

        public void Write(WireWriter writer)
        {
            writer.WriteByte(Counter).WriteUInt16(Sector).WriteUInt16(Zone).WriteUInt16(Unknown);
        }
    }

    /// <summary>
    /// Script / animation block (flag 0x20). Every new animation script increments the counter,
    /// the client plays <see cref="Action"/> (see Movement::Action) with the given stance.
    /// </summary>
    public sealed class CharMvtScript
    {
        /// <summary>Target is set: u32 target (view id | spawn id &lt;&lt; 16).</summary>
        public const byte ModeTarget = 1;
        /// <summary>Target plus world position and rotation.</summary>
        public const byte ModeTargetAndPosition = 2;
        /// <summary>Script is bound to a world position (sitting on benches, ladders...).</summary>
        public const byte ModePosition = 3;

        public const byte FlagStatic = 0x40;
        public const byte FlagNoLerp = 0x80;

        public byte Counter;
        public byte Flags;
        public byte Stance;
        public byte Action;
        /// <summary>Emote id, only present for the emote actions (see <see cref="HasEmoteByte"/>).</summary>
        public byte Emote;
        /// <summary>Animation id, 0x1000 when the action does not need one.</summary>
        public ushort AnimId;
        public uint Target;
        public double PosX, PosY, PosZ;
        public float Rotation;

        public int Mode => Flags & 0x3F;

        public static bool HasEmoteByte(byte action) => action == 0x25 || action == 0x29;

        public static CharMvtScript Read(WireReader reader)
        {
            var script = new CharMvtScript
            {
                Counter = reader.ReadByte(),
                Flags = reader.ReadByte(),
                Stance = reader.ReadByte(),
                Action = reader.ReadByte(),
            };
            if (HasEmoteByte(script.Action)) script.Emote = reader.ReadByte();
            script.AnimId = reader.ReadUInt16();
            switch (script.Mode)
            {
                case 0:
                    break;
                case ModeTarget:
                    script.Target = reader.ReadUInt32();
                    break;
                case ModeTargetAndPosition:
                    script.Target = reader.ReadUInt32();
                    (script.PosX, script.PosY, script.PosZ) = reader.ReadVector3d();
                    script.Rotation = reader.ReadSingle();
                    break;
                case ModePosition:
                    (script.PosX, script.PosY, script.PosZ) = reader.ReadVector3d();
                    script.Rotation = reader.ReadSingle();
                    break;
                default:
                    throw new WireFormatException($"Unknown script block mode {script.Mode}");
            }
            return script;
        }

        public void Write(WireWriter writer)
        {
            writer.WriteByte(Counter).WriteByte(Flags).WriteByte(Stance).WriteByte(Action);
            if (HasEmoteByte(Action)) writer.WriteByte(Emote);
            writer.WriteUInt16(AnimId);
            switch (Mode)
            {
                case 0:
                    break;
                case ModeTarget:
                    writer.WriteUInt32(Target);
                    break;
                case ModeTargetAndPosition:
                    writer.WriteUInt32(Target).WriteVector3d(PosX, PosY, PosZ).WriteSingle(Rotation);
                    break;
                case ModePosition:
                    writer.WriteVector3d(PosX, PosY, PosZ).WriteSingle(Rotation);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown script block mode {Mode}");
            }
        }

        public static uint MakeTarget(ushort viewId, ushort spawnId) => (uint) (viewId | (spawnId << 16));
    }
}
