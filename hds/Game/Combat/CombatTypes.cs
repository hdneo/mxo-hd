using System;

namespace hds.Game.Combat
{
    /// <summary>Fighting style of a combatant as used by the interlock move names.</summary>
    public enum CombatStyle
    {
        Aikido,
        KungFu,
        Karate,
        SelfDefense,
        Pistol,
        DualWeapons,
        SubMachineGun,
        Rifle,
    }

    /// <summary>Interlock tactic. The numbers are the values of the client RPC 0x42 and the move rules.</summary>
    public enum CombatTactic : byte
    {
        Speed = 0,
        Power = 1,
        /// <summary>"DEFENSE" in the client combat scripts, D in the move names.</summary>
        Grab = 2,
        Block = 3,
    }

    /// <summary>Result of one exchange from the attacker's point of view (constants.combat of the client scripts).</summary>
    public enum ExchangeOutcome
    {
        AttackerHitsDefenderMisses,
        BothHit,
        BothMiss,
        Blocked,
        Dodged,
        FinishingMoveAttacker,
        FinishingMoveBoth,
        Killing,
        /// <summary>The attacker's special move (ability) hit: its own interlock move plays.</summary>
        SpecialMove,
    }

    public static class CombatNames
    {
        /// <summary>Style letter used in the move group names ("PD vs PB:Hit:HM").</summary>
        public static char Letter(CombatStyle style) => style switch
        {
            CombatStyle.Aikido => 'A',
            CombatStyle.KungFu => 'W',
            CombatStyle.Karate => 'K',
            CombatStyle.SelfDefense => 'S',
            CombatStyle.Pistol => 'P',
            CombatStyle.DualWeapons => 'D',
            CombatStyle.SubMachineGun => 'G',
            CombatStyle.Rifle => 'R',
            _ => throw new ArgumentOutOfRangeException(nameof(style)),
        };

        public static char Letter(CombatTactic tactic) => tactic switch
        {
            CombatTactic.Speed => 'S',
            CombatTactic.Power => 'P',
            CombatTactic.Grab => 'D',
            CombatTactic.Block => 'B',
            _ => throw new ArgumentOutOfRangeException(nameof(tactic)),
        };

        /// <summary>Style name used by the transition groups ("Transitions from AikidoAbility:to SubM").</summary>
        public static string TransitionName(CombatStyle style) => style switch
        {
            CombatStyle.Aikido => "AikidoAbility",
            CombatStyle.KungFu => "KungFuAbility",
            CombatStyle.Karate => "KarateAbility",
            CombatStyle.SelfDefense => "Self-Defense",
            CombatStyle.Pistol => "Pistol",
            CombatStyle.DualWeapons => "DualWeapon",
            CombatStyle.SubMachineGun => "SubM",
            CombatStyle.Rifle => "Rifle",
            _ => throw new ArgumentOutOfRangeException(nameof(style)),
        };

        /// <summary>Outcome part of the exchange group names, most specific first.</summary>
        public static string[] OutcomeGroups(ExchangeOutcome outcome) => outcome switch
        {
            ExchangeOutcome.AttackerHitsDefenderMisses => new[] {"Hit:HM"},
            ExchangeOutcome.BothHit => new[] {"Hit:HH", "Hit:HM"},
            ExchangeOutcome.BothMiss => new[] {"MM"},
            ExchangeOutcome.Blocked => new[] {"Blocked:MM", "MM"},
            ExchangeOutcome.Dodged => new[] {"Dodged", "Dodged:MM", "MM"},
            ExchangeOutcome.FinishingMoveAttacker => new[] {"Finishing Moves:HM", "Killing", "Hit:HM"},
            ExchangeOutcome.FinishingMoveBoth => new[] {"Finishing Moves:HH", "Finishing Moves:HM", "Killing"},
            ExchangeOutcome.Killing => new[] {"Killing", "Finishing Moves:HM", "Hit:HM"},
            // a special move without an interlock move of its own is shown as a normal hit
            ExchangeOutcome.SpecialMove => new[] {"Hit:HM"},
            _ => throw new ArgumentOutOfRangeException(nameof(outcome)),
        };

        public static bool IsMelee(CombatStyle style) =>
            style == CombatStyle.Aikido || style == CombatStyle.KungFu || style == CombatStyle.Karate || style == CombatStyle.SelfDefense;
    }
}
