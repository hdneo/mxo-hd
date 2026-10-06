using System.Collections.Generic;

namespace hds.Game.Abilities
{
    /// <summary>An ability a character owns (row of char_abilities).</summary>
    public sealed class OwnedAbility
    {
        public ushort AbilityId;
        public ushort Slot;
        public ushort Level;
        public bool Loaded;
    }

    /// <summary>A bonus that is currently shown in the client, remembered so it can be removed again.</summary>
    public sealed class ActiveBonus
    {
        public uint BonusId;
        public AbilityBonusDefinition Definition;
        public int Value;
    }

    /// <summary>Ability state of one character in the world.</summary>
    public sealed class PlayerAbilities
    {
        private readonly Dictionary<ushort, OwnedAbility> owned = new Dictionary<ushort, OwnedAbility>();
        private readonly Dictionary<ushort, List<ActiveBonus>> bonusesBySource = new Dictionary<ushort, List<ActiveBonus>>();
        private uint nextBonusId = 1;

        /// <summary>Abilities are known (loaded from the database). Without it every client request is trusted.</summary>
        public bool HasOwnershipData { get; private set; }

        public IEnumerable<OwnedAbility> Owned => owned.Values;

        public static ushort AbilityIdFromGoId(long goId) => (ushort) ((goId & 0x7FFFFFFF) >> 10);

        public void SetOwned(IEnumerable<OwnedAbility> abilities)
        {
            owned.Clear();
            foreach (var ability in abilities)
            {
                owned[ability.AbilityId] = ability;
            }
            HasOwnershipData = true;
        }

        public bool TryGetOwned(ushort abilityId, out OwnedAbility ability) => owned.TryGetValue(abilityId, out ability);

        public bool IsLoaded(ushort abilityId) => owned.TryGetValue(abilityId, out var a) && a.Loaded;

        public void MarkLoaded(AbilityLoadoutEntry entry, bool loaded)
        {
            if (!owned.TryGetValue(entry.AbilityId, out var ability))
            {
                ability = new OwnedAbility {AbilityId = entry.AbilityId, Slot = entry.Slot, Level = entry.Level};
                owned[entry.AbilityId] = ability;
            }
            ability.Slot = entry.Slot;
            ability.Loaded = loaded;
        }

        public uint NextBonusId() => nextBonusId++;

        public void AddBonus(ushort source, ActiveBonus bonus)
        {
            if (!bonusesBySource.TryGetValue(source, out var list))
            {
                list = new List<ActiveBonus>();
                bonusesBySource[source] = list;
            }
            list.Add(bonus);
        }

        /// <summary>Removes and returns the bonuses granted by <paramref name="source"/>.</summary>
        public List<ActiveBonus> TakeBonuses(ushort source)
        {
            if (!bonusesBySource.Remove(source, out var list)) return new List<ActiveBonus>();
            return list;
        }

        public bool HasBonuses(ushort source) => bonusesBySource.ContainsKey(source);

        /// <summary>
        /// Called when the client loads a new world. Assumption (not verified in the logs yet): the client
        /// rebuilds its bonus table then, so all bonuses are sent again on the next spawn.
        /// </summary>
        public void ForgetSentBonuses() => bonusesBySource.Clear();
    }
}
