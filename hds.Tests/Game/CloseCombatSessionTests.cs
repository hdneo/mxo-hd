using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using hds.Core;
using hds.Core.Wire;
using hds.Game.Combat;
using hds.Game.Combat.Interlock;
using hds.Protocol.Rpc;
using hds.shared;
using Xunit;

namespace hds.Tests.Game
{
    /// <summary>Runs a fight without network: messages are inspected in the client's object message queue.</summary>
    [Collection("GlobalState")]
    public class CloseCombatSessionTests
    {
        private static WorldClient CreateClient()
        {
            _ = TestData.Schema;
            Store.world ??= new WorldServer();
            const string key = "127.0.0.1:1";
            Store.world.objMan.PushClient(key);
            var client = new WorldClient(new IPEndPoint(IPAddress.Loopback, 1), null, key);
            client.playerInstance.CharacterName.setValue("Tester");
            client.playerInstance.Level.setValue((byte) 20);
            client.playerInstance.Health.setValue((ushort) 1000);
            client.playerInstance.MaxHealth.setValue((ushort) 1000);
            client.playerInstance.Position.setValue(NumericalUtils.doublesToLtVector3d(100, 95, 200));
            return client;
        }

        private static Mob CreateMob(WorldClient client, out ushort viewId)
        {
            var mob = new Mob();
            mob.setGoId(NumericalUtils.uint16ToByteArray(599, 1));
            mob.setEntityId(424242);
            mob.setLevel(20);
            mob.setHealthC(30000);
            mob.setHealthM(30000);
            mob.setName("Test Agent");
            mob.setXPos(150);
            mob.setYPos(95);
            mob.setZPos(200);
            WorldServer.mobs.Add(mob);
            var view = client.viewMan.GetViewForEntityAndGo(424242, 599);
            view.viewCreated = true;
            viewId = view.ViewID;
            return mob;
        }

        [Fact]
        public void FightCreatesHandlerAndPlaysRounds()
        {
            Store.Loop = new WorldLoop();
            var client = CreateClient();
            var mob = CreateMob(client, out var mobView);
            var database = InterlockDatabase.Load(TestData.FindRepoFile(Path.Combine("data", "interlock_moves.txt")),
                TestData.FindRepoFile(Path.Combine("data", "interlock_timing.txt")));
            var service = new CloseCombatService(database, new Random(42));

            try
            {
                // 0x40: target view + spawn 0x21, flag 1, default weapon slot 0x6b
                var payload = new WireWriter().WriteUInt16(mobView).WriteUInt16(0x21).WriteByte(1).WriteByte(0x6b).ToArray();
                service.OnCloseCombat(new RpcContext(client, new RpcMessage(0x40, payload, 0)));
                Assert.True(service.IsFighting(client));

                var messages = client.messageQueue.ObjectMessagesQueue.Cast<SequencedMessage>().Select(m => m.content).ToList();
                var create = Assert.Single(messages);
                Assert.Equal("01000c3700", TestData.ToHex(create.Take(5).ToArray()));
                var handlerView = BitConverter.ToUInt16(create, create.Length - 3);

                // run the world loop until two rounds were played
                for (var i = 0; i < 100 && client.messageQueue.ObjectMessagesQueue.Count < 3; i++)
                {
                    Thread.Sleep(100);
                    Store.Loop.RunOnce();
                }

                var rounds = client.messageQueue.ObjectMessagesQueue.Cast<SequencedMessage>()
                    .Select(m => m.content)
                    .Where(m => BitConverter.ToUInt16(m, 0) == handlerView)
                    .Select(m => InterlockUpdate.Read(new WireReader(m, 2, m.Length - 2)))
                    .ToList();
                Assert.NotEmpty(rounds);
                Assert.Equal(1u, rounds[0].Round);
                Assert.Equal(mobView, rounds[0].CombatantA.Value.View);
                Assert.Equal((ushort) 2, rounds[0].CombatantB.Value.View);
                Assert.True(rounds[0].Exchanges[0].Exchange.IsSet);

                // tactic change and withdraw
                service.OnChangeTactic(new RpcContext(client, new RpcMessage(0x42, TestData.Hex("030000000000"), 0)));
                Assert.Equal(CombatTactic.Block, client.CombatTactic);
                service.OnLeaveCombat(new RpcContext(client, new RpcMessage(0x44, new byte[] {1}, 0)));
                Assert.False(service.IsFighting(client));
                var last = client.messageQueue.ObjectMessagesQueue.Cast<SequencedMessage>().Last().content;
                Assert.Equal("01000101" + "00", TestData.ToHex(last.Take(5).ToArray()));
            }
            finally
            {
                WorldServer.mobs.Remove(mob);
            }
        }
    }
}
