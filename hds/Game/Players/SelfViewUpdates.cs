using System.Text;
using hds.Core.Wire;
using hds.Protocol.View;
using hds.shared;

namespace hds.Game.Players
{
    /// <summary>
    /// Applies the self view (view 2) updates a client sends: movement and its real names.
    /// Movement is forwarded to all players that have a view of this character.
    /// </summary>
    public static class SelfViewUpdates
    {
        public static void Apply(WorldClient client, DecodedViewUpdate update)
        {
            if (update.Movement != null)
            {
                ApplyMovement(client, update.Movement);
            }

            if (update.Attributes != null)
            {
                ApplyAttributes(client, update);
            }
        }

        private static void ApplyMovement(WorldClient client, CharMvt clientMvt)
        {
            var player = client.playerInstance;

            // What other players get: same data in the server layout (no sim time, script block instead of counter)
            var forward = new CharMvt();

            if (clientMvt.Has(CharMvt.FlagPosition))
            {
                player.Position.setValue(NumericalUtils.doublesToLtVector3d(clientMvt.X, clientMvt.Y, clientMvt.Z));
                forward.SetPosition(clientMvt.X, clientMvt.Y, clientMvt.Z);
            }

            if (clientMvt.Has(CharMvt.FlagYaw))
            {
                player.YawInterval.setValue(clientMvt.Yaw);
                forward.Yaw = clientMvt.Yaw;
                forward.Flags |= CharMvt.FlagYaw;
            }

            if (clientMvt.Has(CharMvt.FlagAction))
            {
                player.Action.setValue(clientMvt.Action);
                forward.Action = clientMvt.Action;
                forward.Flags |= CharMvt.FlagAction;
            }

            if (clientMvt.Has(CharMvt.FlagScript))
            {
                // The client started a new animation script (emote, sitting...). Others need the full script block.
                forward.Flags |= CharMvt.FlagScript;
                forward.Script = new CharMvtScript
                {
                    Counter = clientMvt.ClientScriptCounter,
                    Flags = CharMvtScript.FlagStatic,
                    Stance = player.Stance.getValue()[0],
                    Action = player.Action.getValue()[0],
                    AnimId = 0x1000,
                };
            }

            if (forward.Flags == 0) return;

            var writer = new WireWriter(32);
            writer.WriteByte(0x01); // component: character movement
            forward.WriteToClient(writer, false);
            Store.world.SendViewUpdateToObservers(client, writer.ToArray());
        }

        private static void ApplyAttributes(WorldClient client, DecodedViewUpdate update)
        {
            var layout = hds.Protocol.Mpm.ClientFrame.SelfViewLayout.Table.Attributes;
            foreach (var (index, value) in update.Attributes)
            {
                switch (layout[index].Name)
                {
                    case "RealFirstName":
                        client.playerInstance.RealFirstName.setValue(value);
                        break;
                    case "RealLastName":
                        client.playerInstance.RealLastName.setValue(value);
                        break;
                    default:
                        Output.WriteDebugLog("[SelfView] Client changed " + layout[index].Name + " = " + Encoding.ASCII.GetString(value));
                        break;
                }
            }
        }
    }
}
