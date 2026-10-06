using System.Linq;
using hds.Core.Wire;
using hds.Protocol.Rpc;
using hds.shared;

namespace hds.Game.Abilities
{
    /// <summary>RPC 0xAE: the client changes its ability loadout (hardline / loading area).</summary>
    public sealed class AbilityLoadoutHandler
    {
        private readonly AbilityBonusCatalog bonuses;

        public AbilityLoadoutHandler(AbilityBonusCatalog bonuses)
        {
            this.bonuses = bonuses;
        }

        [RpcHandler(RPCRequestHeader.CLIENT_ABILITY_LOADER)]
        public void OnLoadoutRequest(RpcContext context)
        {
            var client = context.Client;
            AbilityLoadoutRequest request;
            try
            {
                request = AbilityLoadoutRequest.Parse(context.Payload);
            }
            catch (WireFormatException ex)
            {
                Output.WriteRpcLog("[Abilities] Invalid loadout request: " + ex.Message + " " + StringUtils.bytesToString_NS(context.Payload));
                return;
            }

            var result = AbilityLoadout.Apply(request, client.Abilities, bonuses);
            foreach (var rpc in result.Messages)
            {
                client.SendRpc(rpc.Opcode, rpc.Payload);
            }
            foreach (var rejected in result.Rejected)
            {
                Output.WriteDebugLog("[Abilities] Rejected " + rejected + " for char " + client.playerData.getCharID());
            }

            var charId = client.playerData.getCharID();
            if (result.Unloaded.Count > 0)
            {
                Store.dbManager.WorldDbHandler.UpdateAbilityLoadOut(charId, result.Unloaded.Select(e => e.Slot), false);
            }
            if (result.Loaded.Count > 0)
            {
                Store.dbManager.WorldDbHandler.UpdateAbilityLoadOut(charId, result.Loaded.Select(e => e.Slot), true);
            }
        }
    }
}
