using System;
using System.Collections.Generic;
using hds.Core.Wire;
using hds.Game.Abilities;
using hds.Game.Combat.Interlock;
using hds.Protocol.Rpc;
using hds.shared;

namespace hds.Game.Combat
{
    /// <summary>
    /// Client RPCs of the interlock (close) combat:
    /// <code>
    /// 0x40 start:   u16 target view, u16 target spawn, u8 flag, u8 inventory slot of the weapon (0x6b = default)
    /// 0x42 tactic:  u8 tactic (0 speed, 1 power, 2 grab, 3 block), u16, u16 tactic type (0 melee, 1 ranged, 2 viral)
    /// 0x44 leave:   u8 1
    /// </code>
    /// </summary>
    public sealed class CloseCombatService
    {
        /// <summary>Combat style abilities (ingame ability ids).</summary>
        private static readonly (ushort Ability, CombatStyle Style)[] StyleAbilities =
        {
            (101, CombatStyle.Aikido), (133, CombatStyle.KungFu), (132, CombatStyle.Karate), (17, CombatStyle.SelfDefense),
        };

        private readonly Dictionary<WorldClient, CloseCombatSession> sessions = new Dictionary<WorldClient, CloseCombatSession>();
        private readonly InterlockDatabase database;
        private readonly Random random;

        public CloseCombatService(InterlockDatabase database, Random random = null)
        {
            this.database = database;
            this.random = random ?? new Random();
        }

        public bool IsFighting(WorldClient client) => sessions.ContainsKey(client);

        [RpcHandler(RPCRequestHeader.CLIENT_CLOSE_COMBAT)]
        public void OnCloseCombat(RpcContext context)
        {
            var client = context.Client;
            var reader = context.CreateReader();
            var targetView = reader.ReadUInt16();
            var targetSpawn = reader.ReadUInt16();

            if (sessions.ContainsKey(client))
            {
                Output.WriteDebugLog("[Combat] close combat request while already fighting");
                return;
            }

            var view = client.viewMan.getViewById(targetView);
            var mob = view == null ? null : FindMob(view.entityId);
            if (mob == null || mob.getIsDead())
            {
                new ServerPackets(client).sendSystemChatMessage(client, "You can only fight NPCs in close combat right now.", "BROADCAST");
                return;
            }

            var player = CreatePlayerCombatant(client);
            var npc = new Combatant
            {
                Name = mob.getName(),
                Level = mob.getLevel(),
                Health = mob.getHealthC() > 0 ? mob.getHealthC() : 1,
                MaxHealth = Math.Max(mob.getHealthM(), mob.getHealthC()),
                Style = CombatStyle.Karate,
                Tactic = (CombatTactic) random.Next(3),
            };

            var session = new CloseCombatSession(client, mob, targetView, targetSpawn, player, npc,
                new InterlockSceneBuilder(database, random), random, ended => sessions.Remove(ended.Client));
            sessions[client] = session;
            session.Start();
        }

        [RpcHandler(RPCRequestHeader.CLIENT_CHANGE_CT)]
        public void OnChangeTactic(RpcContext context)
        {
            var tactic = context.CreateReader().ReadByte();
            if (tactic > (byte) CombatTactic.Block) return;
            context.Client.CombatTactic = (CombatTactic) tactic;
            if (sessions.TryGetValue(context.Client, out var session))
            {
                session.PlayerSide.Tactic = (CombatTactic) tactic;
            }
        }

        [RpcHandler(RPCRequestHeader.CLIENT_LEAVE_COMBAT)]
        public void OnLeaveCombat(RpcContext context)
        {
            if (sessions.TryGetValue(context.Client, out var session))
            {
                session.Withdraw();
            }
        }

        /// <summary>
        /// Ability use (RPC 0xB9: u16 ability id, u16 target view ...) while fighting: special moves such as
        /// CheapShot are handled by the fight. Returns false for everything else (normal ability handling).
        /// </summary>
        public bool TryUseSpecialMove(WorldClient client, ushort abilityId)
        {
            if (!sessions.TryGetValue(client, out var session)) return false;
            if (!SpecialMoveCatalog.Instance.TryGet(abilityId, out var move)) return false;
            var level = client.Abilities.TryGetOwned(abilityId, out var owned) && owned.Level > 0 ? owned.Level : (ushort) 1;
            session.RequestSpecialMove(move, level);
            return true;
        }

        /// <summary>Ends the fight of a disconnected client.</summary>
        public void OnClientRemoved(WorldClient client)
        {
            if (sessions.TryGetValue(client, out var session)) session.End("client left");
        }

        private static Mob FindMob(ulong entityId)
        {
            foreach (Mob mob in WorldServer.mobs)
            {
                if (mob.getEntityId() == entityId) return mob;
            }
            return null;
        }

        private static Combatant CreatePlayerCombatant(WorldClient client)
        {
            var player = client.playerInstance;
            var combatant = new Combatant
            {
                Name = StringUtils.charBytesToString_NZ(player.CharacterName.getValue()),
                Level = player.Level.getValue()[0],
                Health = NumericalUtils.ByteArrayToUint16(player.Health.getValue(), 1),
                MaxHealth = NumericalUtils.ByteArrayToUint16(player.MaxHealth.getValue(), 1),
                Tactic = client.CombatTactic,
                WeaponGoId = NumericalUtils.ByteArrayToUint32(player.EquippedItemID.getValue(), 1),
            };
            if (combatant.Health <= 0) combatant.Health = Math.Max(1, combatant.MaxHealth);
            combatant.Style = combatant.WeaponGoId != 0 ? CombatStyle.Pistol : StyleFromAbilities(client.Abilities);
            return combatant;
        }

        public static CombatStyle StyleFromAbilities(PlayerAbilities abilities)
        {
            foreach (var (ability, style) in StyleAbilities)
            {
                if (abilities.IsLoaded(ability)) return style;
            }
            return CombatStyle.SelfDefense;
        }
    }
}
