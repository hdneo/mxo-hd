using hds.Core.Wire;

namespace hds.Game.Abilities
{
    /// <summary>Server RPCs of the ability system (payloads without opcode).</summary>
    public static class AbilityMessages
    {
        public const ushort LoadAbility = 0xB2;    // legacy header 0x80B2
        public const ushort UnloadAbility = 0xB3;  // legacy header 0x80B3
        public const ushort ManageBonus = 0xBC;    // legacy header 0x80BC

        /// <summary>u16 ability, u16 level, u16 slot</summary>
        public static byte[] Load(ushort abilityId, ushort level, ushort slot) =>
            new WireWriter(6).WriteUInt16(abilityId).WriteUInt16(level).WriteUInt16(slot).ToArray();

        /// <summary>u16 ability</summary>
        public static byte[] Unload(ushort abilityId) => new WireWriter(2).WriteUInt16(abilityId).ToArray();

        /// <summary>
        /// Adds or removes a bonus shown in the client (0x80BC):
        /// <code>
        /// u8 flags, bonus id (u16 big endian low word, u16 high word), u32 source ability,
        /// u16 target ability, i32 value, u8 remove, f32 duration
        /// </code>
        /// Bonus ids are unique per session; a remove repeats the id and values of its add.
        /// </summary>
        public static byte[] Bonus(byte flags, uint bonusId, uint sourceAbility, ushort targetAbility, int value, bool remove, float duration)
        {
            return new WireWriter(20)
                .WriteByte(flags)
                .WriteUInt16BigEndian((ushort) bonusId)
                .WriteUInt16((ushort) (bonusId >> 16))
                .WriteUInt32(sourceAbility)
                .WriteUInt16(targetAbility)
                .WriteInt32(value)
                .WriteBool(remove)
                .WriteSingle(duration)
                .ToArray();
        }
    }
}
