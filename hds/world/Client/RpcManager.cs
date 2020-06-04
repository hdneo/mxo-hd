using System;
using hds.shared;

namespace hds{

	public class RpcManager{

        public RpcManager (){

		}
		
        /** 
         * This handles all RPC requests for the client
         */

		public void HandleRpc(int header,ref byte[] rpcData){
            ServerPackets pak = new ServerPackets();
			#if DEBUG
            pak.sendSystemChatMessage(Store.currentClient, "Handle RPC Client Request Header " + StringUtils.bytesToString_NS(NumericalUtils.int32ToByteArray(header,0)) , "BROADCAST");
			#endif
		    switch (header)
		    {

		        case (int) RPCRequestHeader.CLIENT_SPAWN_READY:
		            new PlayerHandler().processSpawn();
		            new PlayerHandler().processAttributes();
		            break;
			        
		        case (int) RPCRequestHeader.CLIENT_CLOSE_COMBAT:
					#if DEBUG
		            Output.WriteRpcLog("CLOSE COMBAT REQUEST");
					#endif
		            new TestUnitHandler().testCloseCombat(ref rpcData);
		            break;
		        case (int) RPCRequestHeader.CLIENT_LEAVE_COMBAT:

		            break;

		        case (int) RPCRequestHeader.CLIENT_RANGE_COMBAT:
			        #if DEBUG
		            Output.WriteRpcLog("RANGE COMBAT REQUEST");
					#endif
		            new TestUnitHandler().testCloseCombat(ref rpcData);
		            break;

		        case (int) RPCRequestHeader.CLIENT_CHAT:
		            new ChatHandler().processChat(ref rpcData);
		            break;

		        case (int) RPCRequestHeader.CLIENT_WHISPER:
			        new ChatHandler().ProcessWhisperPlayer(ref rpcData);
			        break;
		        case (int) RPCRequestHeader.CLIENT_OBJECTINTERACTION_DYNAMIC:
		            new ObjectInteractionHandler().processObjectDynamic(ref rpcData);
		            break;
		        case (int) RPCRequestHeader.CLIENT_OBJECTINTERACTION_STATIC:
		            new ObjectInteractionHandler().processObjectStatic(ref rpcData);
		            break;
		        case (int) RPCRequestHeader.CLIENT_JUMP_START:
		            //ToDo: Split Jump and Hyperjump
			        new AbilityHandler().processHyperJump(ref rpcData);
		            break;
		        case (int) RPCRequestHeader.CLIENT_JUMP_CANCEL:
					// ToDo: 
					new AbilityHandler().processHyperJumpCancel(ref rpcData);
			        break;
		        case (int) RPCRequestHeader.CLIENT_TARGET:
		            new PlayerHelper().processTargetChange(ref rpcData, Store.currentClient);
		            break;
			    case (int) RPCRequestHeader.CLIENT_MISSION_INVITE_PLAYER:
				    new MissionHandler().processInvitePlayerToMissionTeam(ref rpcData);
				    break;
		        case (int) RPCRequestHeader.CLIENT_MISSION_REQUEST:
		            new MissionHandler().processMissionList(ref rpcData);
		            break;
		        case (int) RPCRequestHeader.CLIENT_MISSION_INFO:
		            new MissionHandler().processLoadMissionInfo(ref rpcData);
		            break;
		        case (int) RPCRequestHeader.CLIENT_MISSION_ACCEPT:
		            new MissionHandler().processMissionaccept(ref rpcData);
		            break;
		        case (int) RPCRequestHeader.CLIENT_MISSION_ABORT:
		            new MissionHandler().processAbortMission(ref rpcData);
		            break;
		        case (int) RPCRequestHeader.CLIENT_LEAVE_GROUP:
			        new FCHandler().ProcessLeaveGroup(ref rpcData);
		            break;

		        // Team
		        case (int) RPCRequestHeader.CLIENT_HANDLE_MISSION_INVITE:
		            new TeamHandler().processInviteAnswer(ref rpcData);
		            break;

		        // Faction and Crews
		        case (int) RPCRequestHeader.CLIENT_CREW_INVITE_PLAYER:
		            new FCHandler().processInvitePlayerToCrew(ref rpcData);
		            break;
		        case (int) RPCRequestHeader.CLIENT_FACTION_INFO:
		            new FCHandler().processLoadFactionName(ref rpcData);
		            break;
		        case (int) RPCRequestHeader.CLIENT_DEPOSIT_MONEY_FACTION_CREW:
			        new FCHandler().ProcessDepositMoney(ref rpcData);
			        break;
		        case (int) RPCRequestHeader.CLIENT_FACTION_DISBAND_FACTION:
					new FCHandler().ProcessDisbandFaction(ref rpcData);
					break;
		        // Abilitys
		        case (int) RPCRequestHeader.CLIENT_UPGRADE_ABILITY_LEVEL:
		            // ToDo: Research and implement^^
		            break;
		        case (int) RPCRequestHeader.CLIENT_ABILITY_HANDLER:
		            new AbilityHandler().processAbility(ref rpcData);
		            break;
		        case (int) RPCRequestHeader.CLIENT_CHANGE_CT:
			        // ToDo: Implement Change of CT
		            break;

		        case (int) RPCRequestHeader.CLIENT_ABILITY_LOADER:
		            new PlayerHelper().processLoadAbility(ref rpcData);
		            break;

		        case (int) RPCRequestHeader.CLIENT_HARDLINE_EXIT_LA_CONFIRM:
		            new TeleportHandler().processHardlineExitConfirm(ref rpcData);
		            break;
			    case (int)RPCRequestHeader.CLIENT_EXIT_GAME_FINISH:
				    new TeleportHandler().processGameFinish();
				    break;

		        case (int) RPCRequestHeader.CLIENT_READY_WORLDCHANGE:
		            new TeleportHandler().processTeleportReset(ref rpcData);
		            break;

		        case (int) RPCRequestHeader.CLIENT_TELEPORT_HL:
		            new TeleportHandler().processHardlineTeleport(ref rpcData);
		            break;

		        case (int) RPCRequestHeader.CLIENT_HARDLINE_STATUS_REQUEST:
		            new TeleportHandler().processHardlineStatusRequest(ref rpcData);
		            break;


		        // Inventory
		        case (int) RPCRequestHeader.CLIENT_ITEM_MOUNT_RSI:
		            new InventoryHandler().processMountItem(ref rpcData);
		            break;
		        case (int) RPCRequestHeader.CLIENT_ITEM_UNMOUNT_RSI:
		            new InventoryHandler().processUnmountItem(ref rpcData);
		            break;
		        case (int) RPCRequestHeader.CLIENT_ITEM_MOVE_SLOT:
		            new InventoryHandler().processItemMove(ref rpcData);
		            break;

		        case (int) RPCRequestHeader.CLIENT_ITEM_RECYCLE:
		            new InventoryHandler().processItemDelete(ref rpcData);
		            break;

		        // Vendor
		        case (int) RPCRequestHeader.CLIENT_VENDOR_BUY:
		            new VendorHandler().processBuyItem(ref rpcData);
		            break;

		        case (int) RPCRequestHeader.CLIENT_ADD_FRIEND:
			        new BuddylistHandler().ProcessAddFriend(ref rpcData);
			        break;
		        case (int) RPCRequestHeader.CLIENT_REMOVE_FRIEND:
			        new BuddylistHandler().ProcessRemoveFriend(ref rpcData);
			        break;
		        // MarketPlace
		        case (int) RPCRequestHeader.CLIENT_MP_OPEN:
		            new TestUnitHandler().processMarketTest(ref rpcData);
		            break;

		        case (int) RPCRequestHeader.CLIENT_MP_LIST_ITEMS:
		            new MarketPlaceHandler().processMarketplaceList(ref rpcData);
		            break;

		        // Player
				case (int) RPCRequestHeader.CLIENT_PLAYER_GET_DETAILS:
					new PlayerHandler().ProcessPlayerGetDetails(ref rpcData);
					break;
		        case (int) RPCRequestHeader.CLIENT_PLAYER_GET_BACKGROUND:
		            new PlayerHandler().processGetBackgroundRequest(ref rpcData);
		            break;
		        case (int) RPCRequestHeader.CLIENT_PLAYER_SET_BACKGROUND:
		            new PlayerHandler().ProcessSetBackgroundRequest(ref rpcData);
		            break;
		        // Command Helper
		        case (int) RPCRequestHeader.CLIENT_CMD_WHEREAMI:
		            new CommandHandler().processWhereamiCommand(ref rpcData);
		            break;
                case (int)RPCRequestHeader.CLIENT_CMD_WHO:
                    new CommandHandler().processWhoCommand(ref rpcData);
                    break;

                // Emote and Mood Helpers
                case (int) RPCRequestHeader.CLIENT_CHANGE_MOOD:
		            new PlayerHandler().processMood(ref rpcData);
		            break;

		        case (int) RPCRequestHeader.CLIENT_EMOTE:
		            new PlayerHandler().processEmote(ref rpcData);
		            break;

		        case (int) RPCRequestHeader.CLIENT_REGION_LOADED:
		            new RegionHandler().ProcessRegionLoaded(ref rpcData);
		            //new PlayerInitHelper().processRegionSettings();
		            break;

		         case (int)RPCRequestHeader.CLIENT_LOOT_ACCEPT:
		             new PlayerHandler().ProcessLootAccepted();
		             break;


		        default:
		            //PASS :D
		            byte[] headers = NumericalUtils.int32ToByteArray(header, 1);

			        #if DEBUG
		            string message = "RPCMAIN : Unknown Header " + 
		                             StringUtils.bytesToString_NS(new byte[] {headers[0], headers[1]}) + " \n Content:\n " +
		                             StringUtils.bytesToString_NS(rpcData);
		            Output.WriteRpcLog(message);
					#endif

		            break;

		    }
		}
	}
}

