using System;
using System.Collections.Generic;
using hds.Core.Wire;
using hds.Game.Objects;
using hds.Protocol.View;
using hds.shared;

namespace hds.Game.Combat
{
    /// <summary>View updates and messages caused by combat.</summary>
    public static class CombatNotifications
    {
        /// <summary>Health update of a mob view: component "attribute table" with only the Health attribute.</summary>
        public static byte[] BuildHealthUpdate(ushort goId, ushort health)
        {
            if (!GoSchemaRegistry.Instance.TryGet(goId, out var type)) return null;
            var layout = ViewLayout.For(type, false);
            var attributes = new GoAttributes(type.Schema);
            if (!attributes.Has("Health")) return null;
            attributes.SetUInt16("Health", health);
            var healthIndex = attributes.IndexOf("Health");

            var writer = new WireWriter(16);
            writer.WriteByte((byte) layout.BitOf(ViewComponentKind.AttributeTable));
            var written = AttributeTableCodec.WriteTable(writer, attributes, layout.Table, index => index == healthIndex);
            return written > 0 ? writer.ToArray() : null;
        }

        public static void SetMobHealth(WorldClient client, Mob mob, ushort mobViewId, ushort health)
        {
            mob.setHealthC(health);
            var update = BuildHealthUpdate(NumericalUtils.ByteArrayToUint16(mob.getGoId(), 1), health);
            if (update == null) return;
            client.messageQueue.addObjectMessage(AdminView.Update(mobViewId, update), false);
        }

        public static void SetPlayerHealth(WorldClient client, ushort health)
        {
            var player = client.playerInstance;
            player.Health.setValue(health);
            var changed = new List<Attribute> {player.Health};

            client.messageQueue.addObjectMessage(AdminView.Update(2, player.GetSelfUpdateAttributes(changed, true)), false);
            Store.world.SendViewPacketToAllPlayers(player.GetUpdateAttributes(changed), client.playerData.getCharID(),
                NumericalUtils.ByteArrayToUint16(player.GetGoid(), 1), client.playerData.getEntityId());
        }

        public static void MobKilled(WorldClient client, Mob mob, ushort mobViewId)
        {
            mob.setIsDead(true);
            mob.setIsLootable(true);
            new ServerPackets(client).SendNpcDies(mobViewId, client, mob);

            // experience like the previous ability based kill handling: level * random modifier
            var exp = (uint) (mob.getLevel() * Random.Shared.Next(100, 500));
            new PlayerHandler(client).IncrementPlayerExp(exp);
        }
    }
}
