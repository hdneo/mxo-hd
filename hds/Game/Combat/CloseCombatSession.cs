using System;
using System.Collections.Generic;
using hds.Core;
using hds.Game.Combat.Interlock;
using hds.Game.Objects;
using hds.Protocol.View;
using hds.shared;

namespace hds.Game.Combat
{
    /// <summary>
    /// An interlock fight between a player and a mob. Like the original server, the fight is shown by an
    /// ILCombatHandler object (GO 55): it is created between the combatants and gets one update per round
    /// with the moves to play; it is deleted when the fight ends.
    /// </summary>
    public sealed class CloseCombatSession
    {
        public const ushort IlCombatHandlerGoId = 55;
        public static readonly TimeSpan FirstRoundDelay = TimeSpan.FromMilliseconds(250);
        public const int MinRoundIntervalMs = 2500;
        /// <summary>Pause after the animations before the next round starts.</summary>
        public const int RoundPauseMs = 700;

        private readonly WorldClient client;
        private readonly Mob mob;
        private readonly ushort mobViewId;
        private readonly ushort mobSpawnId;
        private readonly InterlockSceneBuilder scenes;
        private readonly Random random;
        private readonly Action<CloseCombatSession> onEnded;

        private ushort handlerViewId;
        private double x, y, z;
        private uint startMs;
        private ushort round;
        private CombatStyle? previousStyleMob;
        private CombatStyle? previousStylePlayer;
        private ScheduledTask nextRound;

        public CloseCombatSession(WorldClient client, Mob mob, ushort mobViewId, ushort mobSpawnId, Combatant player, Combatant npc,
            InterlockSceneBuilder scenes, Random random, Action<CloseCombatSession> onEnded)
        {
            this.client = client;
            this.mob = mob;
            this.mobViewId = mobViewId;
            this.mobSpawnId = mobSpawnId;
            this.scenes = scenes;
            this.random = random;
            this.onEnded = onEnded;
            PlayerSide = player;
            MobSide = npc;
        }

        public Combatant PlayerSide { get; }
        public Combatant MobSide { get; }
        public bool Ended { get; private set; }
        public WorldClient Client => client;
        public Mob Mob => mob;

        public void Start()
        {
            // the handler sits between both combatants
            double px = 0, py = 0, pz = 0;
            NumericalUtils.LtVector3dToDoubles(client.playerInstance.Position.getValue(), ref px, ref py, ref pz);
            // a walking mob would leave the fight scene: stop it (at its current position) and face the player
            mob.EnterCombat(px, pz);
            x = (px + mob.getXPos()) / 2;
            y = (py + mob.getYPos()) / 2;
            z = (pz + mob.getZPos()) / 2;
            startMs = ServerClock.SimMilliseconds;

            var type = GoSchemaRegistry.Instance.Get(IlCombatHandlerGoId);
            var attributes = new GoAttributes(type.Schema);
            attributes.SetVector3d("Position", x, y, z);
            attributes.SetUInt32("StartTime", startMs);

            var entityId = WorldServer.entityIdCounter++;
            var view = client.viewMan.addView(entityId, IlCombatHandlerGoId);
            view.viewCreated = true;
            view.spawnId = client.playerData.assignSpawnIdCounter();
            handlerViewId = view.ViewID;

            client.messageQueue.addObjectMessage(
                AdminView.Create(IlCombatHandlerGoId, (byte) view.spawnId, attributes, handlerViewId, ReadOnlySpan<byte>.Empty), false);
            client.FlushQueue();

            Output.WriteDebugLog($"[Combat] {PlayerSide.Name} vs {MobSide.Name}: handler view {handlerViewId}");
            nextRound = Store.Loop.Schedule(FirstRoundDelay, PlayRound);
        }

        private void PlayRound()
        {
            if (Ended) return;
            round++;

            var result = CombatResolver.Resolve(MobSide, PlayerSide, random);
            LogSpecialMove(PlayerSide, result);
            // a special move is tried for one round only (hit or not)
            PlayerSide.RequestedSpecialMove = null;
            MobSide.RequestedSpecialMove = null;
            var timeMs = ServerClock.SimMilliseconds - startMs;
            var exchange = scenes.Build(result, MobSide, PlayerSide, previousStyleMob, previousStylePlayer, round, timeMs,
                Relative(mob.getXPos(), mob.getYPos(), mob.getZPos()), RelativePlayer());
            previousStyleMob = MobSide.Style;
            previousStylePlayer = PlayerSide.Style;

            var update = new InterlockUpdate
            {
                ComponentMask = InterlockUpdate.ComponentInterlock,
                X = x, Y = y, Z = z,
                Round = round,
            };
            if (round == 1)
            {
                // the first round tells the client who is fighting (A = the mob, B = the player's self view)
                update.CombatantA = (mobViewId, mobSpawnId);
                update.CombatantB = (2, client.playerData.selfSpawnIdCounter);
            }
            update.Exchanges.Add(exchange);

            client.messageQueue.addObjectMessage(AdminView.Update(handlerViewId, update.ToArray()), false);
            client.FlushQueue();

            // damage becomes visible when the animation hits, the next round starts after the playback
            var playback = InterlockSceneBuilder.PlaybackMs(exchange);
            Store.Loop.Schedule(TimeSpan.FromMilliseconds(playback), () => ApplyDamage(result));
            var interval = Math.Max(MinRoundIntervalMs, playback + RoundPauseMs);
            nextRound = Store.Loop.Schedule(TimeSpan.FromMilliseconds(interval), PlayRound);

            // the NPC changes its tactic from time to time
            if (random.Next(3) == 0) MobSide.Tactic = (CombatTactic) random.Next(3);
        }

        private void ApplyDamage(RoundResult result)
        {
            if (Ended) return;
            var damageToMob = result.Defender == MobSide ? result.DamageToDefender : result.DamageToAttacker;
            var damageToPlayer = result.Defender == PlayerSide ? result.DamageToDefender : result.DamageToAttacker;

            if (damageToMob > 0)
            {
                MobSide.Health = Math.Max(0, MobSide.Health - damageToMob);
                CombatNotifications.SetMobHealth(client, mob, mobViewId, (ushort) MobSide.Health);
            }
            if (damageToPlayer > 0)
            {
                PlayerSide.Health = Math.Max(0, PlayerSide.Health - damageToPlayer);
                CombatNotifications.SetPlayerHealth(client, (ushort) PlayerSide.Health);
            }
            client.FlushQueue();

            if (MobSide.IsDead)
            {
                CombatNotifications.MobKilled(client, mob, mobViewId);
                End("mob died");
            }
            else if (PlayerSide.IsDead)
            {
                End("player died");
            }
        }

        /// <summary>The player used a special move ability: it is tried in the next round.</summary>
        public void RequestSpecialMove(SpecialMove move, ushort level)
        {
            PlayerSide.RequestedSpecialMove = move;
            PlayerSide.SpecialMoveLevel = level;
            Output.WriteDebugLog($"[Combat] {PlayerSide.Name} tries {move.Name} (level {level}) next round");
        }

        private void LogSpecialMove(Combatant side, RoundResult result)
        {
            if (side.RequestedSpecialMove == null) return;
            var hit = result.Outcome == ExchangeOutcome.SpecialMove && result.Attacker == side;
            Output.WriteDebugLog($"[Combat] {side.RequestedSpecialMove.Name}: " +
                                 (hit ? $"hit for {result.DamageToDefender}" : $"failed (outcome {result.Outcome})"));
        }

        public void Withdraw()
        {
            PlayerSide.Withdrawing = true;
            End("withdraw");
        }

        public void End(string reason)
        {
            if (Ended) return;
            Ended = true;
            nextRound?.Cancel();
            mob.LeaveCombat();
            client.messageQueue.addObjectMessage(AdminView.DeleteViews(handlerViewId), false);
            client.viewMan.removeViewByViewId(handlerViewId);
            client.FlushQueue();
            Output.WriteDebugLog($"[Combat] fight {PlayerSide.Name} vs {MobSide.Name} ended: {reason}");
            onEnded(this);
        }

        private (float, float, float) Relative(double px, double py, double pz) => ((float) (px - x), (float) (py - y), (float) (pz - z));

        private (float, float, float) RelativePlayer()
        {
            double px = 0, py = 0, pz = 0;
            NumericalUtils.LtVector3dToDoubles(client.playerInstance.Position.getValue(), ref px, ref py, ref pz);
            return Relative(px, py, pz);
        }
    }
}
