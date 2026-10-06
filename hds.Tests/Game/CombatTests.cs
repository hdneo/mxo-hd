using System;
using System.IO;
using System.Linq;
using hds.Core.Wire;
using hds.Game.Combat;
using hds.Game.Combat.Interlock;
using hds.Game.Objects;
using hds.Protocol.View;
using Xunit;

namespace hds.Tests.Game
{
    public class CombatTests
    {
        private static readonly Lazy<InterlockDatabase> Database = new Lazy<InterlockDatabase>(() =>
            InterlockDatabase.Load(TestData.FindRepoFile(Path.Combine("data", "interlock_moves.txt")),
                TestData.FindRepoFile(Path.Combine("data", "interlock_timing.txt"))));

        public CombatTests()
        {
            _ = TestData.Schema;
        }

        [Fact]
        public void CreatesTheCombatHandlerLikeTheOriginalServer()
        {
            // logged: 01 00 | 0c 37 00 a9 cd ab | 02 05 <position> <start time cd ec 40 23> | view 04 00 | 00
            var type = GoSchemaRegistry.Instance.Get(CloseCombatSession.IlCombatHandlerGoId);
            var attributes = new GoAttributes(type.Schema);
            attributes.SetRaw("Position", TestData.Hex("000000a071c7d1400000000000907f40000000e01c08bc40"));
            attributes.SetRaw("StartTime", TestData.Hex("cdec4023"));

            var bytes = AdminView.Create(55, 0xa9, attributes, 4, ReadOnlySpan<byte>.Empty);
            Assert.Equal("01000c3700a9cdab0205000000a071c7d1400000000000907f40000000e01c08bc40cdec4023040000", TestData.ToHex(bytes));
        }

        [Fact]
        public void DeletesViews()
        {
            Assert.Equal("0100010100 1200".Replace(" ", ""), TestData.ToHex(AdminView.DeleteViews(0x12)));
        }

        [Fact]
        public void DatabaseLoadsAllMoves()
        {
            Assert.Equal(25695, Database.Value.MoveCount);
            Assert.True(Database.Value.TimingCount > 200);
            var move = Database.Value.Get(0x2367);
            Assert.Equal("PD vs PB:Hit:HM", move.Group);
            Assert.Equal(2000, move.DurationMs);
            Assert.Equal("Trans AS-GB Draw", Database.Value.Get(InterlockDatabase.DefaultDrawTransition).Description);
        }

        [Fact]
        public void FindsExchangeForStylesTacticsAndOutcome()
        {
            var move = Database.Value.FindExchange(CombatStyle.Pistol, CombatTactic.Grab, CombatStyle.Pistol, CombatTactic.Block,
                ExchangeOutcome.AttackerHitsDefenderMisses, new Random(1));
            Assert.Equal("PD vs PB:Hit:HM", move.Group);

            var killing = Database.Value.FindExchange(CombatStyle.Karate, CombatTactic.Power, CombatStyle.Aikido, CombatTactic.Speed,
                ExchangeOutcome.Killing, new Random(1));
            // there are no karate vs aikido killing moves in the database: the finishing move is the fallback
            Assert.Equal("KP vs AS:Finishing Moves:HM", killing.Group);
        }

        [Fact]
        public void FindsStyleTransitions()
        {
            var transition = Database.Value.FindTransition(CombatStyle.Pistol, CombatStyle.Aikido, new Random(1));
            Assert.Equal("Transitions from Pistol:to AikidoAbility", transition.Group);
        }

        [Theory]
        [InlineData(CombatTactic.Power, CombatTactic.Speed, true)]
        [InlineData(CombatTactic.Grab, CombatTactic.Power, true)]
        [InlineData(CombatTactic.Speed, CombatTactic.Grab, true)]
        [InlineData(CombatTactic.Speed, CombatTactic.Power, false)]
        [InlineData(CombatTactic.Speed, CombatTactic.Block, true)]
        [InlineData(CombatTactic.Block, CombatTactic.Block, false)]
        public void TacticTriangleMatchesTheClientScripts(CombatTactic s1, CombatTactic s2, bool preferred)
        {
            Assert.Equal(preferred, CombatResolver.IsPreferredTactic(s1, s2));
        }

        [Fact]
        public void BlockingCombatantNeverDealsDamage()
        {
            var random = new Random(7);
            for (var i = 0; i < 200; i++)
            {
                var blocker = new Combatant {Name = "blocker", Level = 20, Health = 10000, Tactic = CombatTactic.Block};
                var other = new Combatant {Name = "other", Level = 20, Health = 10000, Tactic = CombatTactic.Power};
                var result = CombatResolver.Resolve(blocker, other, random);
                var damageByBlocker = result.Attacker == blocker ? result.DamageToDefender : result.DamageToAttacker;
                Assert.Equal(0, damageByBlocker);
            }
        }

        [Fact]
        public void EqualFightersProduceAllKindsOfOutcomes()
        {
            var random = new Random(3);
            var outcomes = Enumerable.Range(0, 400).Select(_ =>
            {
                var a = new Combatant {Level = 30, Health = 100000, Tactic = (CombatTactic) random.Next(3)};
                var b = new Combatant {Level = 30, Health = 100000, Tactic = (CombatTactic) random.Next(3)};
                return CombatResolver.Resolve(a, b, random).Outcome;
            }).ToList();
            Assert.Contains(ExchangeOutcome.AttackerHitsDefenderMisses, outcomes);
            Assert.Contains(ExchangeOutcome.BothHit, outcomes);
            Assert.Contains(ExchangeOutcome.BothMiss, outcomes);
        }

        [Fact]
        public void KillingBlowIsDetected()
        {
            var random = new Random(11);
            for (var i = 0; i < 100; i++)
            {
                var strong = new Combatant {Level = 50, Health = 100000, Tactic = CombatTactic.Power};
                var weak = new Combatant {Level = 1, Health = 1, Tactic = CombatTactic.Speed};
                var result = CombatResolver.Resolve(strong, weak, random);
                if (result.DamageToDefender >= 1 && result.Defender == weak)
                {
                    Assert.True(result.Outcome == ExchangeOutcome.Killing || result.Outcome == ExchangeOutcome.FinishingMoveAttacker);
                    return;
                }
            }
            Assert.Fail("the strong fighter never hit");
        }

        [Fact]
        public void FirstRoundSceneSerializesLikeLoggedRounds()
        {
            var random = new Random(5);
            var npc = new Combatant {Name = "npc", Level = 10, Health = 500, Style = CombatStyle.Karate, Tactic = CombatTactic.Power};
            var player = new Combatant {Name = "player", Level = 10, Health = 500, Style = CombatStyle.Aikido, Tactic = CombatTactic.Speed};
            var result = CombatResolver.Resolve(npc, player, random);
            var exchange = new InterlockSceneBuilder(Database.Value, random).Build(result, npc, player, null, null, 1, 275,
                (-12f, -10f, -68f), (12f, -10f, 68f));

            // unarmed fighters draw from the neutral (SubM) stance into their style, as with the original server
            Assert.Equal("Transitions from SubM:to KarateAbility", Database.Value.Get((int) exchange.TransitionA.MoveId).Group);
            Assert.Equal("Transitions from SubM:to AikidoAbility", Database.Value.Get((int) exchange.TransitionB.MoveId).Group);
            Assert.True(exchange.Exchange.IsSet);
            Assert.Equal(0x03, exchange.Flags & 0x03);

            var update = new InterlockUpdate {X = 1, Y = 2, Z = 3, Round = 1, CombatantA = (0x47, 0x47), CombatantB = (2, 0x63)};
            update.Exchanges.Add(exchange);
            var bytes = update.ToArray();
            var again = InterlockUpdate.Read(new WireReader(bytes));
            Assert.Equal(TestData.ToHex(bytes), TestData.ToHex(again.ToArray()));
            // header: mask, position, round, flags, 2 combatant ids, exchange count; one exchange = 121 bytes
            Assert.Equal(1 + 24 + 4 + 4 + 8 + 1 + 121, bytes.Length);
        }

        [Fact]
        public void SecondRoundHasTransitionOnlyForStyleChanges()
        {
            var random = new Random(9);
            var npc = new Combatant {Level = 10, Health = 500, Style = CombatStyle.Karate};
            var player = new Combatant {Level = 10, Health = 500, Style = CombatStyle.Aikido};
            var result = CombatResolver.Resolve(npc, player, random);
            var exchange = new InterlockSceneBuilder(Database.Value, random).Build(result, npc, player,
                CombatStyle.Karate, CombatStyle.Pistol, 2, 3500, (0, 0, 0), (0, 0, 0));
            Assert.False(exchange.TransitionA.IsSet);
            Assert.True(exchange.TransitionB.IsSet);
            Assert.Equal("Transitions from Pistol:to AikidoAbility", Database.Value.Get((int) exchange.TransitionB.MoveId).Group);
        }

        private static readonly Lazy<SpecialMoveCatalog> SpecialMoves = new Lazy<SpecialMoveCatalog>(() =>
            SpecialMoveCatalog.Load(TestData.FindRepoFile(Path.Combine("data", "combat_special_moves.txt"))));

        [Fact]
        public void CheapShotPlaysItsOwnInterlockMove()
        {
            Assert.True(SpecialMoves.Value.TryGet(198, out var cheapShot));
            Assert.Equal("CheapShotAbility", cheapShot.Name);
            Assert.Equal(20, cheapShot.TacticBonus);

            var random = new Random(4);
            var hits = 0;
            for (var i = 0; i < 200; i++)
            {
                var npc = new Combatant {Level = 20, Health = 100000, Style = CombatStyle.Karate, Tactic = CombatTactic.Power};
                var player = new Combatant {Level = 20, Health = 100000, Style = CombatStyle.SelfDefense, Tactic = CombatTactic.Speed,
                    RequestedSpecialMove = cheapShot, SpecialMoveLevel = 10};
                var result = CombatResolver.Resolve(npc, player, random);
                if (result.Outcome != ExchangeOutcome.SpecialMove) continue;
                hits++;
                // the special move always belongs to the player (true attacker) and only the loser is hurt
                Assert.Same(player, result.Attacker);
                Assert.Equal(0, result.DamageToAttacker);
                Assert.True(result.DamageToDefender > 0);
                var exchange = new InterlockSceneBuilder(Database.Value, random).Build(result, npc, player, CombatStyle.Karate,
                    CombatStyle.SelfDefense, 2, 3000, (0, 0, 0), (0, 0, 0));
                Assert.Equal("CheapShot", Database.Value.Get((int) exchange.Exchange.MoveId).Description);
            }
            Assert.InRange(hits, 40, 199);
        }

        [Fact]
        public void SpecialMovesPickTheVariantOfTheOpponentsStyle()
        {
            Assert.True(SpecialMoves.Value.TryGet(138, out var guardBreaker));
            var move = Database.Value.FindSpecialMove(guardBreaker, CombatStyle.KungFu, CombatTactic.Speed, new Random(1));
            Assert.EndsWith("vs WS", move.Description);
        }

        [Fact]
        public void UnarmedFightersNeverGetWeaponMoves()
        {
            var random = new Random(8);
            foreach (CombatStyle a in new[] {CombatStyle.Aikido, CombatStyle.KungFu, CombatStyle.Karate, CombatStyle.SelfDefense})
            foreach (CombatStyle d in new[] {CombatStyle.Aikido, CombatStyle.KungFu, CombatStyle.Karate, CombatStyle.SelfDefense})
            foreach (ExchangeOutcome outcome in Enum.GetValues(typeof(ExchangeOutcome)))
            foreach (CombatTactic t in Enum.GetValues(typeof(CombatTactic)))
            {
                var move = Database.Value.FindExchange(a, t, d, CombatTactic.Power, outcome, random);
                Assert.NotNull(move);
                Assert.Contains(move.Attacker[0], "AWKS");
                Assert.Contains(move.Defender[0], "AWKS");
                var transition = Database.Value.FirstRoundTransition(a, random);
                Assert.EndsWith(CombatNames.TransitionName(a), transition.Group);
            }
        }

        [Fact]
        public void BuildsNpcHealthUpdate()
        {
            // NPC_BASE (599): component mask 0x04 (attribute table is the third component), then Health only
            var update = CombatNotifications.BuildHealthUpdate(599, 0x0123);
            var layout = ViewLayout.For(GoSchemaRegistry.Instance.Get(599), false);
            var decoded = DecodedViewUpdate.Read(new WireReader(update), layout, true, false);
            Assert.Equal(0x04, decoded.ComponentMask);
            var attribute = Assert.Single(decoded.Attributes);
            Assert.Equal("Health", layout.Table.Attributes[attribute.Index].Name);
            Assert.Equal("2301", TestData.ToHex(attribute.Value));
        }
    }
}
