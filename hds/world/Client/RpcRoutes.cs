using hds.Protocol.Rpc;
using hds.shared;

namespace hds
{
    /// <summary>
    /// Registers the handlers of all client RPCs. New features register handler objects with
    /// [RpcHandler] methods (see <see cref="RpcDispatcher.RegisterHandlers"/>); the routes below adapt the
    /// existing handler classes that still take the payload by ref.
    /// </summary>
    public static class RpcRoutes
    {
        public static RpcDispatcher CreateDispatcher()
        {
            var d = new RpcDispatcher();

            // Spawn / world
            d.Register(RPCRequestHeader.CLIENT_SPAWN_READY, c =>
            {
                var handler = new PlayerHandler(c.Client);
                handler.processSpawn();
                handler.processAttributes();
            });
            d.Register(RPCRequestHeader.CLIENT_REGION_LOADED, c => Legacy(c, (h, p) => new RegionHandler(h).ProcessRegionLoaded(ref p)));
            d.Register(RPCRequestHeader.CLIENT_READY_WORLDCHANGE, c => Legacy(c, (h, p) => new TeleportHandler(h).processTeleportReset(ref p)));

            // Combat
            // close combat (0x40), tactic (0x42) and leaving (0x44) are handled by Game.Combat.CloseCombatService
            d.Register(RPCRequestHeader.CLIENT_RANGE_COMBAT, c => Legacy(c, (h, p) => new CombatHandler(h).ProcessRangeCombatRequest(ref p)));

            // Chat
            d.Register(RPCRequestHeader.CLIENT_CHAT, c => Legacy(c, (h, p) => new ChatHandler(h).processChat(ref p)));
            d.Register(RPCRequestHeader.CLIENT_WHISPER, c => Legacy(c, (h, p) => new ChatHandler(h).ProcessWhisperPlayer(ref p)));

            // Objects
            d.Register(RPCRequestHeader.CLIENT_OBJECTINTERACTION_DYNAMIC, c => Legacy(c, (h, p) => new ObjectInteractionHandler(h).processObjectDynamic(ref p)));
            d.Register(RPCRequestHeader.CLIENT_OBJECTINTERACTION_STATIC, c => Legacy(c, (h, p) => new ObjectInteractionHandler(h).processObjectStatic(ref p)));

            // Abilities and movement
            d.Register(RPCRequestHeader.CLIENT_JUMP_START, c => Legacy(c, (h, p) => new AbilityHandler(h).ProcessJump(ref p)));
            d.Register(RPCRequestHeader.CLIENT_JUMP_CANCEL, c => Legacy(c, (h, p) => new AbilityHandler(h).processHyperJumpCancel(ref p)));
            d.Register(RPCRequestHeader.CLIENT_UPGRADE_ABILITY_LEVEL, c => Legacy(c, (h, p) => new AbilityHandler(h).ProcessUpgradeAbility(ref p)));
            d.Register(RPCRequestHeader.CLIENT_ABILITY_HANDLER, c =>
            {
                // special moves (CheapShot ...) during a close combat fight are part of the interlock
                if (c.Message.Payload.Length >= 2 &&
                    Store.CloseCombat != null && Store.CloseCombat.TryUseSpecialMove(c.Client, System.BitConverter.ToUInt16(c.Message.Payload, 0)))
                {
                    return;
                }
                Legacy(c, (h, p) => new AbilityHandler(h).ProcessAbility(ref p));
            });
            d.Register(RPCRequestHeader.CLIENT_TARGET, c => Legacy(c, (h, p) => new PlayerHandler(h).ProcessTargetChange(ref p, h)));

            // Missions and teams
            d.Register(RPCRequestHeader.CLIENT_MISSION_INVITE_PLAYER, c => Legacy(c, (h, p) => new MissionHandler(h).processInvitePlayerToMissionTeam(ref p)));
            d.Register(RPCRequestHeader.CLIENT_MISSION_REQUEST, c => Legacy(c, (h, p) => new MissionHandler(h).processMissionList(ref p)));
            d.Register(RPCRequestHeader.CLIENT_MISSION_INFO, c => Legacy(c, (h, p) => new MissionHandler(h).processLoadMissionInfo(ref p)));
            d.Register(RPCRequestHeader.CLIENT_MISSION_ACCEPT, c => Legacy(c, (h, p) => new MissionHandler(h).ProcessMissionaccept(ref p)));
            d.Register(RPCRequestHeader.CLIENT_MISSION_ABORT, c => Legacy(c, (h, p) => new MissionHandler(h).processAbortMission(ref p)));
            d.Register(RPCRequestHeader.CLIENT_HANDLE_MISSION_INVITE, c => Legacy(c, (h, p) => new TeamHandler(h).processInviteAnswer(ref p)));

            // Crews and factions
            d.Register(RPCRequestHeader.CLIENT_LEAVE_GROUP, c => Legacy(c, (h, p) => new FCHandler(h).ProcessLeaveGroup(ref p)));
            d.Register(RPCRequestHeader.CLIENT_FACTION_CREW_INVITE_PLAYER, c => Legacy(c, (h, p) => new FCHandler(h).processInvitePlayerToCrew(ref p)));
            d.Register(RPCRequestHeader.CLIENT_FACTION_INVITE, c => Legacy(c, (h, p) => new FCHandler(h).ProcessInvitePlayerToNewFaction(ref p)));
            d.Register(RPCRequestHeader.CLIENT_FACTION_INFO, c => Legacy(c, (h, p) => new FCHandler(h).ProcessLoadFactionName(ref p)));
            d.Register(RPCRequestHeader.CLIENT_DEPOSIT_MONEY_FACTION_CREW, c => Legacy(c, (h, p) => new FCHandler(h).ProcessDepositMoney(ref p)));
            d.Register(RPCRequestHeader.CLIENT_FACTION_DISBAND_FACTION, c => Legacy(c, (h, p) => new FCHandler(h).ProcessDisbandFaction(ref p)));

            // Hardlines and teleport
            d.Register(RPCRequestHeader.CLIENT_HARDLINE_EXIT_LA_CONFIRM, c => Legacy(c, (h, p) => new TeleportHandler(h).processHardlineExitConfirm(ref p)));
            d.Register(RPCRequestHeader.CLIENT_EXIT_GAME_FINISH, c => new TeleportHandler(c.Client).processGameFinish());
            d.Register(RPCRequestHeader.CLIENT_TELEPORT_HL, c => Legacy(c, (h, p) => new TeleportHandler(h).processHardlineTeleport(ref p)));
            d.Register(RPCRequestHeader.CLIENT_HARDLINE_STATUS_REQUEST, c => Legacy(c, (h, p) => new TeleportHandler(h).processHardlineStatusRequest(ref p)));

            // Inventory and vendors
            d.Register(RPCRequestHeader.CLIENT_ITEM_MOUNT_RSI, c => Legacy(c, (h, p) => new InventoryHandler(h).processMountItem(ref p)));
            d.Register(RPCRequestHeader.CLIENT_ITEM_UNMOUNT_RSI, c => Legacy(c, (h, p) => new InventoryHandler(h).processUnmountItem(ref p)));
            d.Register(RPCRequestHeader.CLIENT_ITEM_MOVE_SLOT, c => Legacy(c, (h, p) => new InventoryHandler(h).processItemMove(ref p)));
            d.Register(RPCRequestHeader.CLIENT_ITEM_RECYCLE, c => Legacy(c, (h, p) => new InventoryHandler(h).processItemDelete(ref p)));
            d.Register(RPCRequestHeader.CLIENT_VENDOR_BUY, c => Legacy(c, (h, p) => new VendorHandler(h).processBuyItem(ref p)));
            d.Register(RPCRequestHeader.CLIENT_LOOT_ACCEPT, c => new PlayerHandler(c.Client).ProcessLootAccepted());

            // Buddy list
            d.Register(RPCRequestHeader.CLIENT_ADD_FRIEND, c => Legacy(c, (h, p) => new BuddylistHandler(h).ProcessAddFriend(ref p)));
            d.Register(RPCRequestHeader.CLIENT_REMOVE_FRIEND, c => Legacy(c, (h, p) => new BuddylistHandler(h).ProcessRemoveFriend(ref p)));

            // Marketplace
            d.Register(RPCRequestHeader.CLIENT_MP_OPEN, c => Legacy(c, (h, p) => new TestUnitHandler(h).processMarketTest(ref p)));
            d.Register(RPCRequestHeader.CLIENT_MP_LIST_ITEMS, c => Legacy(c, (h, p) => new MarketPlaceHandler(h).processMarketplaceList(ref p)));

            // Player info, commands, emotes
            d.Register(RPCRequestHeader.CLIENT_PLAYER_GET_DETAILS, c => Legacy(c, (h, p) => new PlayerHandler(h).ProcessPlayerGetDetails(ref p)));
            d.Register(RPCRequestHeader.CLIENT_PLAYER_GET_BACKGROUND, c => Legacy(c, (h, p) => new PlayerHandler(h).processGetBackgroundRequest(ref p)));
            d.Register(RPCRequestHeader.CLIENT_PLAYER_SET_BACKGROUND, c => Legacy(c, (h, p) => new PlayerHandler(h).ProcessSetBackgroundRequest(ref p)));
            d.Register(RPCRequestHeader.CLIENT_CMD_WHEREAMI, c => Legacy(c, (h, p) => new CommandHandler(h).processWhereamiCommand(ref p)));
            d.Register(RPCRequestHeader.CLIENT_CMD_WHO, c => Legacy(c, (h, p) => new CommandHandler(h).processWhoCommand(ref p)));
            d.Register(RPCRequestHeader.CLIENT_CHANGE_MOOD, c => Legacy(c, (h, p) => new PlayerHandler(h).processMood(ref p)));
            d.Register(RPCRequestHeader.CLIENT_EMOTE, c => Legacy(c, (h, p) => new PlayerHandler(h).processEmote(ref p)));

            // Feature modules with [RpcHandler] methods
            d.RegisterHandlers(new hds.Game.Abilities.AbilityLoadoutHandler(hds.Game.Abilities.AbilityBonusCatalog.Instance));
            Store.CloseCombat = new hds.Game.Combat.CloseCombatService(hds.Game.Combat.Interlock.InterlockDatabase.Instance);
            d.RegisterHandlers(Store.CloseCombat);

            return d;
        }

        private delegate void LegacyHandler(WorldClient client, byte[] payload);

        /// <summary>Adapter for the existing handlers that take the payload by ref.</summary>
        private static void Legacy(RpcContext context, LegacyHandler handler)
        {
            handler(context.Client, context.Payload);
        }
    }
}
