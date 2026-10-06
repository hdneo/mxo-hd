using System.Collections.Generic;
using hds.Core.Wire;
using hds.Game.Combat.Interlock;
using Xunit;

namespace hds.Tests.Game
{
    public class InterlockUpdateTests
    {
        [Fact]
        public void RoundTripsLoggedCombatUpdates()
        {
            var failures = new List<string>();
            var count = 0;
            foreach (var line in TestData.Lines("il_combat_updates.txt"))
            {
                var hex = line.Substring(2);
                count++;
                try
                {
                    var reader = new WireReader(TestData.Hex(hex));
                    var update = InterlockUpdate.Read(reader);
                    Assert.True(reader.EndOfData);
                    var encoded = TestData.ToHex(update.ToArray());
                    if (encoded != hex) failures.Add(hex + " != " + encoded);
                }
                catch (WireFormatException ex)
                {
                    failures.Add(hex + " -> " + ex.Message);
                }
            }
            Assert.True(count > 3000);
            Assert.True(failures.Count == 0, $"{failures.Count}/{count} failed, first: {(failures.Count > 0 ? failures[0] : "")}");
        }

        [Fact]
        public void DecodesFirstRoundOfAFight()
        {
            // round 1 of a logged fight: NPC (view 0x47) vs. the player (view 2), PD vs PB hit
            var line = "01000000408bbbd0400000000000907f4000000000244e9b40010000000300000047004700020063000101029a0200007072f0c2000020c1c0798fc200000000000020c100000000000001001301000034053405350503145200008b0b0024145200008b0b0024672300008b0b002400000000000000000000000000000000b1000000c4000000001000100000000001000000000000000100000000000000000000000";
            var update = InterlockUpdate.Read(new WireReader(TestData.Hex(line.Substring(0, line.Length - 1))));
            Assert.Equal(1u, update.Round);
            Assert.Equal((ushort) 0x47, update.CombatantA.Value.View);
            Assert.Equal((ushort) 2, update.CombatantB.Value.View);
            var exchange = Assert.Single(update.Exchanges);
            Assert.Equal(0x5214u, exchange.TransitionA.MoveId);
            Assert.Equal(0x2367u, exchange.Exchange.MoveId);
            Assert.Equal(InterlockMoveRef.DefaultDatabase, exchange.Exchange.Database);
            Assert.Equal(177u, exchange.ValueA);
            Assert.Equal(196u, exchange.ValueB);
        }
    }
}
