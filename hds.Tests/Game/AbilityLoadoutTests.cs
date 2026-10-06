using System.IO;
using System.Linq;
using hds.Core.Wire;
using hds.Game.Abilities;
using hds.Protocol.Rpc;
using Xunit;

namespace hds.Tests.Game
{
    public class AbilityLoadoutTests
    {
        private static AbilityBonusCatalog HyperJumpCatalog()
        {
            // row from data/ability_bonuses.txt: HyperJump grants SuperJump +3 per level
            return AbilityBonusCatalog.Parse(new StringReader("11\t567\t45\t3.0\t0\t0.0\tprops\n"));
        }

        [Fact]
        public void ParsesAndRebuildsAllLoggedRequests()
        {
            var count = 0;
            foreach (var hex in TestData.Lines("ability_loadout_requests.txt"))
            {
                var request = AbilityLoadoutRequest.Parse(TestData.Hex(hex));
                Assert.Equal(hex, TestData.ToHex(request.ToBytes()));
                count++;
            }
            Assert.True(count > 500);
        }

        [Fact]
        public void ParsesSwapRequestWithBothLists()
        {
            // Vector28: unload 35 abilities and load a new set in one request (second list at offset 0x147)
            var hex = TestData.Lines("ability_loadout_requests.txt").First(h => h.StartsWith("ea0130390a004701"));
            var request = AbilityLoadoutRequest.Parse(TestData.Hex(hex));
            Assert.Equal(35, request.Unload.Count);
            Assert.True(request.Load.Count > 30);
            Assert.Equal(0x0147, 10 + 35 * AbilityLoadoutRequest.EntrySize + 2);
        }

        [Fact]
        public void LoadAnswersLikeTheOriginalServer()
        {
            // Client: load HyperJump (0x0b) level 50 into slot 0x16
            var request = AbilityLoadoutRequest.Parse(TestData.Hex("0000000000000a0001000016000b0000003200"));
            var state = new PlayerAbilities();
            var result = AbilityLoadout.Apply(request, state, HyperJumpCatalog());

            // Server (log): 08 80 b2 0b 00 32 00 16 00, then the SuperJump bonus with value 150
            Assert.Equal(2, result.Messages.Count);
            Assert.Equal(0xB2, result.Messages[0].Opcode);
            Assert.Equal("0b0032001600", TestData.ToHex(result.Messages[0].Payload));

            var bonus = new WireReader(result.Messages[1].Payload);
            Assert.Equal(0xBC, result.Messages[1].Opcode);
            Assert.Equal(0x45, bonus.ReadByte());
            bonus.Skip(4); // bonus id
            Assert.Equal(11u, bonus.ReadUInt32());
            Assert.Equal(567, bonus.ReadUInt16());
            Assert.Equal(150, bonus.ReadInt32());
            Assert.False(bonus.ReadBool());
        }

        [Fact]
        public void UnloadRemovesTheBonusWithTheSameId()
        {
            var state = new PlayerAbilities();
            var load = AbilityLoadout.Apply(AbilityLoadoutRequest.Parse(TestData.Hex("0000000000000a0001000016000b0000003200")), state, HyperJumpCatalog());
            var unload = AbilityLoadout.Apply(AbilityLoadoutRequest.Parse(TestData.Hex("000000000a00000001000016000b0000003200")), state, HyperJumpCatalog());

            Assert.Equal(0xB3, unload.Messages[0].Opcode);
            Assert.Equal("0b00", TestData.ToHex(unload.Messages[0].Payload));
            var added = load.Messages[1].Payload;
            var removed = unload.Messages[1].Payload;
            Assert.Equal(TestData.ToHex(added.Take(15).ToArray()), TestData.ToHex(removed.Take(15).ToArray()));
            Assert.Equal(1, removed[15]); // remove flag
            Assert.False(state.IsLoaded(0x0b));
        }

        [Fact]
        public void UsesTheOwnedLevelAndRejectsUnknownAbilities()
        {
            var state = new PlayerAbilities();
            state.SetOwned(new[] {new OwnedAbility {AbilityId = 0x0b, Slot = 0x16, Level = 10}});
            // client claims level 50 for HyperJump and tries to load HyperSpeed (0x0c) which it does not own
            var request = AbilityLoadoutRequest.Create(0, null, new[]
            {
                new AbilityLoadoutEntry(0x16, 0x0b, 50),
                new AbilityLoadoutEntry(0x30, 0x0c, 50),
            });
            var result = AbilityLoadout.Apply(request, state, HyperJumpCatalog());

            Assert.Single(result.Loaded);
            Assert.Equal(10, result.Loaded[0].Level);
            Assert.Single(result.Rejected);
            Assert.Equal("0b000a001600", TestData.ToHex(result.Messages[0].Payload));
        }

        [Fact]
        public void BonusValueFollowsTheStatModifierFormula()
        {
            var bonus = new AbilityBonusDefinition(74, 127, 0x45, 0.481f, 0, 0);
            // logged: level 28 -> 13, 40 -> 19, 50 -> 24
            Assert.Equal(13, bonus.ValueAt(28));
            Assert.Equal(19, bonus.ValueAt(40));
            Assert.Equal(24, bonus.ValueAt(50));
        }

        [Fact]
        public void HandlerIsRegisteredForTheLoaderRpc()
        {
            var dispatcher = new RpcDispatcher();
            dispatcher.RegisterHandlers(new AbilityLoadoutHandler(HyperJumpCatalog()));
            Assert.True(dispatcher.IsHandled(0xAE));
        }

        [Fact]
        public void GeneratedBonusFileLoads()
        {
            var catalog = AbilityBonusCatalog.Load(TestData.FindRepoFile(Path.Combine("data", "ability_bonuses.txt")));
            Assert.True(catalog.Count > 200);
            var hyperJump = catalog.For(11).Single();
            Assert.Equal(567, hyperJump.TargetAbility);
            Assert.Equal(150, hyperJump.ValueAt(50));
        }
    }
}
