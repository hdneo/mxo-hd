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
            pak.sendSystemChatMessage(Store.currentClient, "Handle RPC Client Request Header " + StringUtils.bytesToString_NS(NumericalUtils.int32ToByteArray(header,0)) , "BROADCAST");
		    switch (header)
		    {

		        case (int) RPCRequestHeader.CLIENT_SPAWN_READY:
		            new PlayerHandler().processSpawn();
		            new PlayerHandler().processAttributes();
		            break;

		        case (int) RPCRequestHeader.CLIENT_CLOSE_COMBAT:
		            Output.WriteRpcLog("CLOSE COMBAT REQUEST");
		            new TestUnitHandler().testCloseCombat(ref rpcData);
		            break;
		        case (int) RPCRequestHeader.CLIENT_LEAVE_COMBAT:

		            break;

		        case (int) RPCRequestHeader.CLIENT_RANGE_COMBAT:
		            Output.WriteRpcLog("RANGE COMBAT REQUEST");
		            new TestUnitHandler().testCloseCombat(ref rpcData);
		            break;
		        case (int) RPCRequestHeader.CLIENT_CHAT:
		            new ChatHandler().processChat(ref rpcData);
		            break;

		        case (int) RPCRequestHeader.CLIENT_OBJECTINTERACTION_DYNAMIC:
		            new ObjectInteractionHandler().processObjectDynamic(ref rpcData);
		            Output.writeToLogForConsole("RPCMAIN : Handle OBJECTINTERACTION_DYNAMIC");
		            break;
		        case (int) RPCRequestHeader.CLIENT_OBJECTINTERACTION_STATIC:
		            new ObjectInteractionHandler().processObjectStatic(ref rpcData);
		            Output.writeToLogForConsole("RPCMAIN : Handle OBJECTINTERACTION_STATIC");
		            break;
		        case (int) RPCRequestHeader.CLIENT_JUMP:
		            Output.writeToLogForConsole("RPCMAIN : Handle JUMP");
		            //ToDo: Split Jump and Hyperjump
		            //new TestUnitHandler().processHyperJump(ref rpcData);
		            new TestUnitHandler().processHyperJump(ref rpcData);
		            break;
		        case (int) RPCRequestHeader.CLIENT_TARGET:
		            new PlayerHelper().processTargetChange(ref rpcData, Store.currentClient);
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
		        case (int) RPCRequestHeader.CLIENT_PARTY_LEAVE:
		            break;

		        // Team
		        case (int) RPCRequestHeader.CLIENT_HANDLE_MISSION_INVITE:
		            new TeamHandler().processTeamInviteAnswer(ref rpcData);
		            break;

		        // Faction and Crews
		        case (int) RPCRequestHeader.CLIENT_FACTION_INFO:
		            new FCHandler().processLoadFactionName(ref rpcData);
		            // ToDo: implement response with following format :
		            // size + 80 f5 + uint32 factionId + String(40 size? unusual...)
		            // Example: 30 80 f5 11 ba 00 00 48 79 50 6e 30 74 69 5a 65 44 20 4d 69 4e 64 5a 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
		            break;

		        // Abilitys
		        case (int) RPCRequestHeader.CLIENT_UPGRADE_ABILITY_LEVEL:
		            // ToDo: Research and implement^^
		            break;
		        case (int) RPCRequestHeader.CLIENT_ABILITY_HANDLER:
		            new AbilityHandler().processAbility(ref rpcData);
		            break;
		        case (int) RPCRequestHeader.CLIENT_CHANGE_CT:
		            new PlayerHelper().processUpdateExp();
		            break;

		        case (int) RPCRequestHeader.CLIENT_ABILITY_LOADER:
		            new PlayerHelper().processLoadAbility(ref rpcData);
		            break;

		        case (int) RPCRequestHeader.CLIENT_HARDLINE_EXIT_LA_CONFIRM:
		            new TeleportHandler().processHardlineExitConfirm(ref rpcData);
		            break;

		        case (int) RPCRequestHeader.CLIENT_READY_WORLDCHANGE:
		            Output.WriteLine("RPCMAIN : RESET_RPC detect");
		            new TeleportHandler().processTeleportReset(ref rpcData);
		            break;

		        case (int) RPCRequestHeader.CLIENT_TELEPORT_HL:
		            new TeleportHandler().processHardlineTeleport(ref rpcData);
		            break;

		        case (int) RPCRequestHeader.CLIENT_HARDLINE_STATUS_REQUEST:
		            new TeleportHandler().processHardlineStatusRequest(ref rpcData);
		            break;


		        // Inventory
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

		        // MarketPlace
		        case (int) RPCRequestHeader.CLIENT_MP_OPEN:
		            new TestUnitHandler().processMarketTest(ref rpcData);
		            break;

		        case (int) RPCRequestHeader.CLIENT_MP_LIST_ITEMS:
		            new MarketPlaceHandler().processMarketplaceList(ref rpcData);
		            break;

		        // Player

		        case (int) RPCRequestHeader.CLIENT_PLAYER_GET_BACKGROUND:
		            new PlayerHandler().processGetBackgroundRequest(ref rpcData);
		            break;
		        case (int) RPCRequestHeader.CLIENT_PLAYER_SET_BACKGROUND:
		            new PlayerHandler().processSetBackgroundRequest(ref rpcData);
		            break;
		        // Command Helper
		        case (int) RPCRequestHeader.CLIENT_CMD_WHEREAMI:
		            new CommandHandler().processWhereamiCommand(ref rpcData);
		            break;

		        // Emote and Mood Helpers
		        case (int) RPCRequestHeader.CLIENT_CHANGE_MOOD:
		            new PlayerHandler().processMood(ref rpcData);
		            break;

		        case (int) RPCRequestHeader.CLIENT_EMOTE:
		            new PlayerHandler().processEmote(ref rpcData);
		            break;

		        case (int) RPCRequestHeader.CLIENT_REGION_LOADED:
		            new RegionHandler().processRegionLoaded(ref rpcData);
		            //new PlayerInitHelper().processRegionSettings();
		            break;


		        default:
		            //PASS :D
		            byte[] headers = NumericalUtils.int32ToByteArray(header, 1);

		            string message = "RPCMAIN : Unknown Header " +
		                             StringUtils.bytesToString_NS(new byte[] {headers[0], headers[1]}) + " \n Content:\n " +
		                             StringUtils.bytesToString_NS(rpcData);
		            Output.WriteLine(message);
		            Output.WriteRpcLog(message);

		            break;

		    }
		}
	}
}

