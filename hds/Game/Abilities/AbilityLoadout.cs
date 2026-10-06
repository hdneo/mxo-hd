using System.Collections.Generic;

namespace hds.Game.Abilities
{
    /// <summary>A server RPC to send (opcode without the 0x80 marker + payload).</summary>
    public readonly struct OutgoingRpc
    {
        public OutgoingRpc(ushort opcode, byte[] payload)
        {
            Opcode = opcode;
            Payload = payload;
        }

        public ushort Opcode { get; }
        public byte[] Payload { get; }
    }

    public sealed class AbilityLoadoutResult
    {
        public List<OutgoingRpc> Messages { get; } = new List<OutgoingRpc>();
        public List<AbilityLoadoutEntry> Unloaded { get; } = new List<AbilityLoadoutEntry>();
        public List<AbilityLoadoutEntry> Loaded { get; } = new List<AbilityLoadoutEntry>();
        public List<string> Rejected { get; } = new List<string>();
    }

    /// <summary>
    /// Applies an ability loadout change. Like the original server all unloads are answered first
    /// (0x80B3 + removal of their bonuses), then the loads (0x80B2 + their passive bonuses).
    /// </summary>
    public static class AbilityLoadout
    {
        public static AbilityLoadoutResult Apply(AbilityLoadoutRequest request, PlayerAbilities state, AbilityBonusCatalog bonuses)
        {
            var result = new AbilityLoadoutResult();

            foreach (var entry in request.Unload)
            {
                if (state.HasOwnershipData && !state.TryGetOwned(entry.AbilityId, out _))
                {
                    result.Rejected.Add("unload of unknown " + entry);
                    continue;
                }
                result.Messages.Add(new OutgoingRpc(AbilityMessages.UnloadAbility, AbilityMessages.Unload(entry.AbilityId)));
                foreach (var bonus in state.TakeBonuses(entry.AbilityId))
                {
                    result.Messages.Add(BonusMessage(bonus, true));
                }
                state.MarkLoaded(entry, false);
                result.Unloaded.Add(entry);
            }

            foreach (var entry in request.Load)
            {
                var level = entry.Level;
                if (state.HasOwnershipData)
                {
                    if (!state.TryGetOwned(entry.AbilityId, out var owned))
                    {
                        result.Rejected.Add("load of unknown " + entry);
                        continue;
                    }
                    level = owned.Level; // never trust the level sent by the client
                }

                var accepted = new AbilityLoadoutEntry(entry.Slot, entry.AbilityId, level);
                result.Messages.Add(new OutgoingRpc(AbilityMessages.LoadAbility, AbilityMessages.Load(accepted.AbilityId, accepted.Level, accepted.Slot)));
                state.MarkLoaded(accepted, true);
                AddBonuses(state, bonuses, accepted.AbilityId, accepted.Level, result.Messages);
                result.Loaded.Add(accepted);
            }

            return result;
        }

        /// <summary>Sends the passive bonuses of all loaded abilities that are not active in the client yet.</summary>
        public static List<OutgoingRpc> SendMissingBonuses(PlayerAbilities state, AbilityBonusCatalog bonuses)
        {
            var messages = new List<OutgoingRpc>();
            foreach (var ability in state.Owned)
            {
                if (!ability.Loaded || state.HasBonuses(ability.AbilityId)) continue;
                AddBonuses(state, bonuses, ability.AbilityId, ability.Level, messages);
            }
            return messages;
        }

        private static void AddBonuses(PlayerAbilities state, AbilityBonusCatalog bonuses, ushort abilityId, ushort level, List<OutgoingRpc> messages)
        {
            foreach (var definition in bonuses.For(abilityId))
            {
                var bonus = new ActiveBonus {BonusId = state.NextBonusId(), Definition = definition, Value = definition.ValueAt(level)};
                state.AddBonus(abilityId, bonus);
                messages.Add(BonusMessage(bonus, false));
            }
        }

        private static OutgoingRpc BonusMessage(ActiveBonus bonus, bool remove)
        {
            var d = bonus.Definition;
            return new OutgoingRpc(AbilityMessages.ManageBonus,
                AbilityMessages.Bonus(d.Flags, bonus.BonusId, d.SourceAbility, d.TargetAbility, bonus.Value, remove, d.Duration));
        }
    }
}
