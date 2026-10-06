using System;

namespace hds.Game.Combat.Interlock
{
    /// <summary>Builds the exchange block of an interlock round from the resolved round.</summary>
    public sealed class InterlockSceneBuilder
    {
        private readonly InterlockDatabase database;
        private readonly Random random;

        public InterlockSceneBuilder(InterlockDatabase database, Random random)
        {
            this.database = database;
            this.random = random;
        }

        /// <param name="a">Combatant A of the handler (first id in the update)</param>
        /// <param name="b">Combatant B of the handler</param>
        /// <param name="previousStyleA">Style A used in the previous round (null in the first round)</param>
        /// <param name="round">1-based round counter</param>
        /// <param name="timeMs">Milliseconds since the handler StartTime</param>
        public InterlockExchange Build(RoundResult result, Combatant a, Combatant b, CombatStyle? previousStyleA, CombatStyle? previousStyleB,
            ushort round, uint timeMs, (float X, float Y, float Z) relativeA, (float X, float Y, float Z) relativeB)
        {
            var attackerIsA = result.Attacker == a;
            var exchange = new InterlockExchange
            {
                RoleA = attackerIsA ? (byte) 1 : (byte) 2,
                RoleB = attackerIsA ? (byte) 2 : (byte) 1,
                AX = relativeA.X, AY = relativeA.Y, AZ = relativeA.Z,
                BX = relativeB.X, BY = relativeB.Y, BZ = relativeB.Z,
                Round = round,
                Time = timeMs,
                ValueA = attackerIsA ? result.AttackerValue : result.DefenderValue,
                ValueB = attackerIsA ? result.DefenderValue : result.AttackerValue,
                WeaponA = a.WeaponGoId,
                WeaponB = b.WeaponGoId,
            };

            var transitionA = Transition(previousStyleA, a.Style);
            var transitionB = Transition(previousStyleB, b.Style);
            if (transitionA != null) exchange.TransitionA = InterlockMoveRef.Of(transitionA.Id);
            if (transitionB != null) exchange.TransitionB = InterlockMoveRef.Of(transitionB.Id);

            var move = result.Outcome == ExchangeOutcome.SpecialMove && result.SpecialMove != null && result.SpecialMove.HasMove
                ? database.FindSpecialMove(result.SpecialMove, result.Defender.Style, result.Defender.Tactic, random)
                : database.FindExchange(result.Attacker.Style, result.Attacker.Tactic, result.Defender.Style,
                    result.Defender.Tactic, result.Outcome, random);
            if (move != null) exchange.Exchange = InterlockMoveRef.Of(move.Id);

            ApplyTiming(exchange, move, transitionA != null, transitionB != null);
            return exchange;
        }

        private InterlockMove Transition(CombatStyle? previous, CombatStyle current)
        {
            if (previous == null) return database.FirstRoundTransition(current, random);
            if (previous.Value == current) return null;
            return database.FindTransition(previous.Value, current, random);
        }

        private void ApplyTiming(InterlockExchange exchange, InterlockMove move, bool hasTransitionA, bool hasTransitionB)
        {
            // bit 0/1 = the transition moves are set (same as the logged flags 0x03)
            byte flags = 0;
            if (hasTransitionA) flags |= 0x01;
            if (hasTransitionB) flags |= 0x02;

            if (move != null && database.TryGetTiming(move.Id, out var timing))
            {
                exchange.TransitionTimeA = hasTransitionA ? (timing.TransitionTimeA != 0 ? timing.TransitionTimeA : (ushort) 675) : (ushort) 0;
                exchange.TransitionTimeB = hasTransitionB ? timing.TransitionTimeB : (ushort) 0;
                exchange.Unknown3 = timing.Unknown3;
                exchange.ExchangeStart = timing.ExchangeStart;
                exchange.DurationA = timing.DurationA;
                exchange.DurationB = timing.DurationB;
                exchange.Flags = (byte) (flags | timing.Flags);
                exchange.DamageScaleA = timing.DamageScaleA;
                exchange.DamageScaleB = timing.DamageScaleB;
                exchange.TailA1 = timing.TailA1;
                exchange.TailA2 = timing.TailA2;
                exchange.TailB1 = timing.TailB1;
                exchange.TailB2 = timing.TailB2;
                return;
            }

            // No logged template: animations run at 1.5x (DB duration 2000 ms -> 1333 ms in the logs)
            var duration = (ushort) ((move?.DurationMs ?? 2000) * 2 / 3);
            exchange.TransitionTimeA = hasTransitionA ? (ushort) 675 : (ushort) 0;
            exchange.TransitionTimeB = hasTransitionB ? (ushort) 675 : (ushort) 0;
            exchange.ExchangeStart = (short) Math.Max(exchange.TransitionTimeA, exchange.TransitionTimeB);
            exchange.DurationA = duration;
            exchange.DurationB = duration;
            exchange.Flags = flags;
        }

        /// <summary>Total time the client needs to play the round (used to schedule damage and the next round).</summary>
        public static int PlaybackMs(InterlockExchange exchange) =>
            Math.Max(0, (int) exchange.ExchangeStart) + Math.Max((int) exchange.DurationA, (int) exchange.DurationB);
    }
}
