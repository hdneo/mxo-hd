using System;
using System.Collections.Generic;
using hds.Core.Wire;
using hds.Game.Objects;

namespace hds.Protocol.View
{
    /// <summary>Kind of a view component (sub view) inside a 03 view update.</summary>
    public enum ViewComponentKind
    {
        /// <summary>Attribute table (update list or self view list).</summary>
        AttributeTable,
        /// <summary>Character movement, see <see cref="CharMvt"/>.</summary>
        CharMvt,
        /// <summary>One byte state of NPCs (purpose unknown, usually 0).</summary>
        NpcState,
        /// <summary>Interlock combat scene data of the ILCombatHandler object.</summary>
        InterlockScene,
    }

    /// <summary>
    /// Describes which components a view update of a game object contains. The first byte of every
    /// update is a bit mask; bit n selects component n of this layout.
    /// </summary>
    public sealed class ViewLayout
    {
        public const ushort PlayerCharacterGoId = 12;
        public const ushort IlCombatHandlerGoId = 55;

        /// <summary>
        /// Attributes of the player self view ("auto view", view id 2). The owner gets more attributes
        /// than other players; the list is sorted by attribute id descending like all update lists.
        /// </summary>
        public static readonly string[] PlayerSelfViewAttributes =
        {
            "UseRSIDescription", "TitleAbility", "SelectionRangeDebuff", "RippleMagnitude", "RepelDistance", "RealLastName", "RealFirstName",
            "RSIDescription", "OrganizationID", "NoiseLevel", "MovementScale", "MissionTypeFlags", "MissionTeamID", "MissionKey",
            "MaxHealth", "Level", "JumpPeakHeight", "JumpFlags", "JumpEndTime", "JumpDestination", "IsDuelDeath",
            "IsDead", "InteractionFlags", "InnerStrengthMax", "InnerStrengthCommitted", "InnerStrengthAvailable", "HeavyLuggableID", "Health",
            "FollowingPath", "FactionID", "EvadeShieldHealth", "EquippedItemID", "EffectID", "EffectCounter", "EffectCommand",
            "DuelID", "DissemblingType", "Description", "DeathPenalty", "CurCombatExclusiveAbility", "CrewID", "ConquestPoints",
            "ConditionStateFlags", "CombatantMode", "CancelAbilityCounter", "CancelAbility", "AbandonedState", "AFK",
        };

        private static readonly Dictionary<string, AttributeTableLayout> selfLayouts = new Dictionary<string, AttributeTableLayout>();
        private static readonly Dictionary<string, AttributeTableLayout> updateLayouts = new Dictionary<string, AttributeTableLayout>();

        private ViewLayout(IReadOnlyList<ViewComponentKind> components, AttributeTableLayout table)
        {
            Components = components;
            Table = table;
        }

        public IReadOnlyList<ViewComponentKind> Components { get; }
        public AttributeTableLayout Table { get; }

        public int BitOf(ViewComponentKind kind)
        {
            for (var i = 0; i < Components.Count; i++)
            {
                if (Components[i] == kind) return 1 << i;
            }
            return 0;
        }

        public static ViewLayout For(GoTypeInfo type, bool selfView)
        {
            var schema = type.Schema;
            if (selfView)
            {
                return new ViewLayout(new[] {ViewComponentKind.CharMvt, ViewComponentKind.AttributeTable}, SelfTable(schema));
            }

            var updateTable = UpdateTable(schema);
            if (type.GoId == PlayerCharacterGoId)
            {
                return new ViewLayout(new[] {ViewComponentKind.CharMvt, ViewComponentKind.AttributeTable}, updateTable);
            }
            if (type.GoId == IlCombatHandlerGoId)
            {
                return new ViewLayout(new[] {ViewComponentKind.InterlockScene, ViewComponentKind.AttributeTable}, updateTable);
            }
            if (HasCharMovement(schema))
            {
                return new ViewLayout(new[] {ViewComponentKind.NpcState, ViewComponentKind.CharMvt, ViewComponentKind.AttributeTable}, updateTable);
            }
            return new ViewLayout(new[] {ViewComponentKind.AttributeTable}, updateTable);
        }

        /// <summary>Objects with a Movement::Stance attribute own a CharMvt component (NPCs, mobs).</summary>
        public static bool HasCharMovement(GoClassSchema schema)
        {
            foreach (var attribute in schema.Create)
            {
                if (attribute.Type == "Movement::Stance") return true;
            }
            return false;
        }

        private static AttributeTableLayout SelfTable(GoClassSchema schema)
        {
            lock (selfLayouts)
            {
                if (!selfLayouts.TryGetValue(schema.Key, out var layout))
                {
                    layout = AttributeTableLayout.FromNames(schema, PlayerSelfViewAttributes);
                    selfLayouts[schema.Key] = layout;
                }
                return layout;
            }
        }

        private static AttributeTableLayout UpdateTable(GoClassSchema schema)
        {
            lock (updateLayouts)
            {
                if (!updateLayouts.TryGetValue(schema.Key, out var layout))
                {
                    layout = AttributeTableLayout.ForUpdates(schema);
                    updateLayouts[schema.Key] = layout;
                }
                return layout;
            }
        }
    }

    /// <summary>Decoded view update, mainly used for logging, tests and client packets.</summary>
    public sealed class DecodedViewUpdate
    {
        public byte ComponentMask;
        public CharMvt Movement;
        public byte NpcState;
        public byte[] InterlockScene;
        public List<(int Index, byte[] Value)> Attributes;

        public static DecodedViewUpdate Read(WireReader reader, ViewLayout layout, bool fromServer, bool selfView)
        {
            var update = new DecodedViewUpdate {ComponentMask = reader.ReadByte()};
            if ((update.ComponentMask >> layout.Components.Count) != 0)
            {
                throw new WireFormatException($"Component mask 0x{update.ComponentMask:x2} exceeds {layout.Components.Count} components");
            }
            for (var i = 0; i < layout.Components.Count; i++)
            {
                if ((update.ComponentMask & (1 << i)) == 0) continue;
                switch (layout.Components[i])
                {
                    case ViewComponentKind.CharMvt:
                        update.Movement = fromServer ? CharMvt.ReadFromServer(reader, selfView) : CharMvt.ReadFromClient(reader);
                        break;
                    case ViewComponentKind.AttributeTable:
                        update.Attributes = AttributeTableCodec.ReadTable(reader, layout.Table.Attributes);
                        break;
                    case ViewComponentKind.NpcState:
                        update.NpcState = reader.ReadByte();
                        break;
                    case ViewComponentKind.InterlockScene:
                        throw new NotSupportedException("Interlock scene decoding is handled by the combat module");
                }
            }
            return update;
        }

        /// <summary>Writes the update again (tables must be re-encoded from raw values).</summary>
        public void Write(WireWriter writer, ViewLayout layout, bool selfView)
        {
            writer.WriteByte(ComponentMask);
            for (var i = 0; i < layout.Components.Count; i++)
            {
                if ((ComponentMask & (1 << i)) == 0) continue;
                switch (layout.Components[i])
                {
                    case ViewComponentKind.CharMvt:
                        Movement.WriteToClient(writer, selfView);
                        break;
                    case ViewComponentKind.AttributeTable:
                        WriteRawTable(writer, layout.Table.Attributes, Attributes);
                        break;
                    case ViewComponentKind.NpcState:
                        writer.WriteByte(NpcState);
                        break;
                    default:
                        throw new NotSupportedException(layout.Components[i].ToString());
                }
            }
        }

        private static void WriteRawTable(WireWriter writer, IReadOnlyList<GoAttributeDef> list, List<(int Index, byte[] Value)> values)
        {
            var byIndex = new Dictionary<int, byte[]>();
            var last = 0;
            foreach (var (index, value) in values)
            {
                byIndex[index] = value;
                last = Math.Max(last, index);
            }
            for (var groupStart = 0; groupStart <= last; groupStart += 7)
            {
                var maskPosition = writer.Length;
                writer.WriteByte(0);
                byte mask = 0;
                for (var bit = 0; bit < 7; bit++)
                {
                    if (!byIndex.TryGetValue(groupStart + bit, out var value)) continue;
                    mask |= (byte) (1 << bit);
                    if (list[groupStart + bit].IsString) writer.WriteByte((byte) value.Length);
                    writer.WriteBytes(value);
                }
                if (groupStart + 7 <= last) mask |= 0x80;
                writer.PatchByte(maskPosition, mask);
            }
        }
    }
}
