using System;

namespace hds.Game.Combat
{
    /// <summary>A participant of an interlock fight (player or NPC).</summary>
    public sealed class Combatant
    {
        public string Name;
        public ushort Level = 1;
        public int Health = 100;
        public int MaxHealth = 100;
        public CombatStyle Style = CombatStyle.SelfDefense;
        public CombatTactic Tactic = CombatTactic.Speed;
        /// <summary>GO id of the equipped weapon (0 = unarmed).</summary>
        public uint WeaponGoId;
        public int WeaponBaseDamage;
        public bool Withdrawing;

        /// <summary>Special move ability the combatant tries this round (requestedSpecialMove of the scripts).</summary>
        public SpecialMove RequestedSpecialMove;
        /// <summary>Level of the requested special move ability (scales its damage).</summary>
        public ushort SpecialMoveLevel = 1;

        public bool IsDead => Health <= 0;
    }

    public sealed class RoundResult
    {
        public Combatant Attacker;
        public Combatant Defender;
        public ExchangeOutcome Outcome;
        public int DamageToDefender;
        public int DamageToAttacker;
        /// <summary>Combat tactic totals, sent to the client as the exchange values.</summary>
        public uint AttackerValue;
        public uint DefenderValue;
        /// <summary>Set when the attacker's special move succeeded (<see cref="ExchangeOutcome.SpecialMove"/>).</summary>
        public SpecialMove SpecialMove;
    }

    /// <summary>
    /// Interlock exchange resolution, ported from the client scripts combat_resolution.py and
    /// combat_calculations.py. combatlib.getTacticRoll is native code and approximated by a normal distribution.
    /// </summary>
    public static class CombatResolver
    {
        public static double CombatTacticsMod(CombatTactic tactic) => tactic switch
        {
            CombatTactic.Block => 0.0,
            CombatTactic.Speed => 1.15,
            CombatTactic.Power => 0.9,
            CombatTactic.Grab => 1.05,
            _ => 1.0,
        };

        public static double DefenseTacticsMod(CombatTactic tactic) => tactic switch
        {
            CombatTactic.Block => 1.5,
            CombatTactic.Speed => 0.9,
            CombatTactic.Power => 1.0,
            CombatTactic.Grab => 1.3,
            _ => 1.0,
        };

        public static double DamageMod(CombatTactic tactic) => tactic switch
        {
            CombatTactic.Block => 0.0,
            CombatTactic.Speed => 1.0,
            CombatTactic.Power => 1.2,
            CombatTactic.Grab => 0.85,
            _ => 1.0,
        };

        /// <summary>isPreferredTactical: Power beats Speed, Grab beats Power, Speed beats Grab, anything beats Block.</summary>
        public static bool IsPreferredTactic(CombatTactic s1, CombatTactic s2) => s2 switch
        {
            CombatTactic.Speed => s1 == CombatTactic.Power,
            CombatTactic.Power => s1 == CombatTactic.Grab,
            CombatTactic.Grab => s1 == CombatTactic.Speed,
            CombatTactic.Block => s1 != CombatTactic.Block,
            _ => false,
        };

        /// <summary>Tactic points without abilities/items. Tuned so equal levels hit about half of the time.</summary>
        public static double CombatTacticBase(Combatant c) => 60 + 2.0 * c.Level;
        public static double DefenseTacticBase(Combatant c) => 0.9 * CombatTacticBase(c);

        public static Combatant DetermineAttacker(Combatant a, Combatant b, Random random)
        {
            if (a.Withdrawing) return b;
            if (b.Withdrawing) return a;
            if (a.Tactic != b.Tactic)
            {
                return IsPreferredTactic(a.Tactic, b.Tactic) ? a : b;
            }
            if (a.Level != b.Level) return a.Level > b.Level ? a : b;
            return random.Next(2) == 0 ? a : b;
        }

        public static RoundResult Resolve(Combatant a, Combatant b, Random random)
        {
            var attacker = DetermineAttacker(a, b, random);
            var defender = attacker == a ? b : a;

            // a special move adds its Ability/TacticBonus to the combat tactics of this round
            var attackerCt = Roll((CombatTacticBase(attacker) + SpecialMoveBonus(attacker)) * CombatTacticsMod(attacker.Tactic), random);
            var defenderCt = Roll((CombatTacticBase(defender) + SpecialMoveBonus(defender)) * CombatTacticsMod(defender.Tactic), random);
            var attackerD = Roll(DefenseTacticBase(attacker) * DefenseTacticsMod(attacker.Tactic), random);
            var defenderD = Roll(DefenseTacticBase(defender) * DefenseTacticsMod(defender.Tactic), random);

            if (defender.Withdrawing) defenderCt = 0;
            if (attacker.Withdrawing) attackerCt = 0;

            var attackerHits = attackerCt - defenderD > 0;
            var defenderHits = defenderCt - attackerD > 0;

            // determineResultForStandardCombat: the roles swap (true attacker) if only the defender hits, if the
            // defender's special move hits, or if both special moves hit and the defender rolled the higher CT
            var attackerSm = attacker.RequestedSpecialMove != null;
            var defenderSm = defender.RequestedSpecialMove != null;
            var swap = defenderHits && !attackerHits;
            if (attackerHits && defenderHits && attackerSm && defenderSm) swap = defenderCt > attackerCt;
            else if (defenderSm && defenderHits) swap = true;
            if (swap)
            {
                (attacker, defender) = (defender, attacker);
                (attackerCt, defenderCt) = (defenderCt, attackerCt);
                (attackerD, defenderD) = (defenderD, attackerD);
                (attackerHits, defenderHits) = (true, false);
            }

            var result = new RoundResult
            {
                Attacker = attacker, Defender = defender,
                AttackerValue = (uint) Math.Max(0, Math.Round(attackerCt)),
                DefenderValue = (uint) Math.Max(0, Math.Round(defenderCt)),
            };

            if (attacker.RequestedSpecialMove != null && attackerHits)
            {
                // handleSpecialMove: the special move plays, only its ability damage counts
                result.Outcome = ExchangeOutcome.SpecialMove;
                result.SpecialMove = attacker.RequestedSpecialMove;
                result.DamageToDefender = SpecialMoveDamage(attacker, defender, random);
                return result;
            }

            if (attacker.Tactic == CombatTactic.Block)
            {
                // handleBlock: nobody gets hurt; a much faster opponent makes it a dodge
                result.Outcome = attackerCt - defenderD > 50 ? ExchangeOutcome.Dodged : ExchangeOutcome.Blocked;
                return result;
            }

            if (attackerHits) result.DamageToDefender = Damage(attacker, defender, random);
            if (defenderHits) result.DamageToAttacker = Damage(defender, attacker, random);

            if (attackerHits && defenderHits) result.Outcome = ExchangeOutcome.BothHit;
            else if (attackerHits) result.Outcome = ExchangeOutcome.AttackerHitsDefenderMisses;
            else result.Outcome = defender.Tactic == CombatTactic.Block ? ExchangeOutcome.Blocked : ExchangeOutcome.BothMiss;

            var defenderDies = result.DamageToDefender >= defender.Health;
            var attackerDies = result.DamageToAttacker >= attacker.Health;
            if (defenderDies && attackerDies) result.Outcome = ExchangeOutcome.FinishingMoveBoth;
            else if (defenderDies) result.Outcome = result.Outcome == ExchangeOutcome.BothHit ? ExchangeOutcome.FinishingMoveAttacker : ExchangeOutcome.Killing;
            else if (attackerDies)
            {
                // the defender finishes the attacker: present it from the winner's side
                (result.Attacker, result.Defender) = (defender, attacker);
                (result.DamageToAttacker, result.DamageToDefender) = (result.DamageToDefender, result.DamageToAttacker);
                (result.AttackerValue, result.DefenderValue) = (result.DefenderValue, result.AttackerValue);
                result.Outcome = ExchangeOutcome.FinishingMoveAttacker;
            }
            return result;
        }

        private static double SpecialMoveBonus(Combatant c) => c.RequestedSpecialMove?.TacticBonus ?? 0;

        /// <summary>
        /// GetFullAbilityDamage: the ability damage (Damage/MinValue..MaxValue, scaled with the ability level as
        /// Ability/MinValueScaleLevel = 1 suggests) times the tactic damage mod, reduced like a normal hit.
        /// Special moves without damage (throws, sweeps) still deal a normal hit.
        /// </summary>
        public static int SpecialMoveDamage(Combatant winner, Combatant loser, Random random)
        {
            var move = winner.RequestedSpecialMove;
            if (move == null || move.DamageMax <= 0) return Damage(winner, loser, random);
            var perLevel = random.Next(Math.Min(move.DamageMin, move.DamageMax), Math.Max(move.DamageMin, move.DamageMax) + 1);
            var amount = perLevel * Math.Max((int) winner.SpecialMoveLevel, 1) * Math.Max(DamageMod(winner.Tactic), 1.0);
            var reduction = DamageReduction(winner.Level, loser.Level, 0);
            return Math.Max(1, (int) (amount - (int) (amount * reduction)));
        }

        /// <summary>GetCloseCombatDamage + GetDamageReductionValue (no armor or damage abilities yet).</summary>
        public static int Damage(Combatant winner, Combatant loser, Random random)
        {
            var baseDamage = 0.0;
            if (winner.WeaponBaseDamage > 0)
            {
                baseDamage = winner.WeaponBaseDamage;
            }
            var bonuses = 5 + 1.2 * winner.Level;
            var amount = baseDamage + bonuses;

            // gaussian: stddev 10%, clamped to 1/3 .. 4/3
            amount = Math.Clamp(amount + Gaussian(random) * amount / 10.0, amount / 3.0, 4.0 * amount / 3.0);
            amount *= DamageMod(winner.Tactic);
            amount *= 4.0;

            var reduction = DamageReduction(winner.Level, loser.Level, 0);
            var final = (int) (amount - (int) (amount * reduction));
            return Math.Max(1, final);
        }

        public static double DamageReduction(int attackerLevel, int defenderLevel, double armor)
        {
            const double levelScale = 4.0, scalePerLevel = 0.2, minArmor = -0.33, maxArmor = 0.33, armorStart = -20.0;
            var levelReduction = (defenderLevel - attackerLevel) * scalePerLevel;
            var totalRange = defenderLevel * levelScale - armorStart;
            var tick = (maxArmor - minArmor) / totalRange;
            var armorReduction = Math.Min(maxArmor, tick * (armor - armorStart) + minArmor);
            return Math.Clamp(levelReduction + armorReduction, -0.9, 0.9);
        }

        /// <summary>Approximation of combatlib.getTacticRoll (native): normal distribution, 15% deviation.</summary>
        private static double Roll(double baseValue, Random random)
        {
            if (baseValue <= 0) return 0;
            return Math.Clamp(baseValue + Gaussian(random) * baseValue * 0.15, 0, baseValue * 2);
        }

        private static double Gaussian(Random random)
        {
            var u1 = 1.0 - random.NextDouble();
            var u2 = random.NextDouble();
            return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
        }
    }
}
