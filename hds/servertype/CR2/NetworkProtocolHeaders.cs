using System;
//namespace hds.servertype.cr2
namespace hds
{

    // This list is the client REQUEST rpc headers
    public enum RPCRequestHeader
    {
        CLIENT_SPAWN_READY = 0x05,
        CLIENT_CHAT = 0x28,
        CLIENT_CLOSE_COMBAT = 0x40,
        CLIENT_RANGE_COMBAT = 0x41,
        CLIENT_LEAVE_COMBAT = 0x44,
        CLIENT_JUMP_START = 0xc2,
        CLIENT_JUMP_CANCLE = 0xc4,
        CLIENT_OBJECTINTERACTION_DYNAMIC = 0xc7,
        CLIENT_OBJECTINTERACTION_STATIC = 0xc8,
        CLIENT_REGION_LOADED = 0xc9,   
        CLIENT_TARGET = 0x151,
        CLIENT_MISSION_INVITE_PLAYER = 0x8c,
        CLIENT_MISSION_REQUEST = 0x94,
        CLIENT_MISSION_INFO = 0x98,
        CLIENT_MISSION_ACCEPT = 0x9b,
        CLIENT_MISSION_ABORT = 0xa6,
        CLIENT_PARTY_LEAVE = 0x71,
        CLIENT_ABILITY_HANDLER = 0xB9,
        CLIENT_CHANGE_CT = 0x42,

        // Hardline
        CLIENT_HARDLINE_STATUS_REQUEST = 0x6c,
        CLIENT_HARDLINE_EXIT_LA_CONFIRM = 0xfc,
        CLIENT_EXIT_GAME_FINISH = 0xfe,

        CLIENT_HANDLE_MISSION_INVITE = 0x6f,

        // Ability Handlers
        CLIENT_ABILITY_LOADER = 0xae,
        CLIENT_UPGRADE_ABILITY_LEVEL = 0xb7,
        CLIENT_DISABLE_BUFF = 0xB6,

        CLIENT_LOOT_ALL = 0x117,
        CLIENT_LOOT_ACCEPT = 0x11f,

        // Inventory
        CLIENT_ITEM_MOUNT_RSI = 0x63,
        CLIENT_ITEM_UNMOUNT_RSI = 0x64,
        CLIENT_ITEM_MOVE_SLOT = 0x65,
        CLIENT_ITEM_RECYCLE = 0x5D,
        CLIENT_VENDOR_BUY = 0x10e,
        CLIENT_VENDOR_SELL = 0x111,

        // MarketPlace
        CLIENT_MP_LIST_ITEMS = 0x124,
        CLIENT_MP_OPEN = 0x121,

        //Commands
        CLIENT_CMD_WHEREAMI = 0x154,
        CLIENT_CMD_WHO = 0x152, // ToDo: implement
        CLIENT_PLAYER_GET_DETAILS = 0x192, // ToDo: implement
        CLIENT_PLAYER_GET_BACKGROUND = 0x194, // ToDo: implement
        CLIENT_PLAYER_SET_BACKGROUND = 0x196,

        // Char Emotes and things
        CLIENT_CHANGE_MOOD = 0x35,

        // Teleport
        CLIENT_TELEPORT_HL = 0x18e,

        // Reset Client RPC
        CLIENT_READY_WORLDCHANGE = 0x108,

        // Currently unhandled but maybe useful
        // http://code.google.com/p/mxo-singularity/wiki/RpcPacketMap
        CLIENT_CHAT_WHISPER = 0x29,
        CLIENT_EMOTE = 0x30,
        CLIENT_ANIMATION_START = 0x33,
        CLIENT_ANIMATION_STOP = 0x34,

        // Faction and crew
        CLIENT_FACTION_INFO = 0xf4,
        CREW_INVITE_PLAYER = 0x84,
    }

    public enum RPCResponseHeaders
    {
        // World
        SERVER_LOAD_WORLD_CMD = 0x06,
        SERVER_LOAD_RPC_RESET = 0x8107,
        SERVER_LOAD_WORLD = 0x8108,
        SERVER_FLASH_TRAFFIC = 0x81a9,

        // Enviroment
        SERVER_ELEVATOR_PANEL = 0x813f,

        // Player
        SERVER_PLAYER_ATTRIBUTE = 0x80b2, 
        SERVER_MANAGE_BONUS = 0xbc,
        SERVER_EXIT_HL = 0x80fb,
        SERVER_JACKOUT_FINISH = 0x80fd,
        SERVER_PLAYER_EXP   = 0x80e5,
        SERVER_PLAYER_EXP_ANIM   = 0x80e6,
        SERVER_PLAYER_INFO  = 0x80e4,
        SERVER_PLAYER_GET_BACKGROUND = 0x8195,

        // Friendlist
        SERVER_FRIENDLIST_STATUS = 0x80D7,

        // Mission related
        SERVER_MISSION_RESPONSE_LIST    = 0x8095,
        SERVER_MISSION_RESPONSE_NAME    = 0x8096,
        SERVER_MISSION_RESPONSE_UNKNOWN = 0x8097,
        SERVER_MISSION_SET_OBJECTIVE = 0x80a0,
        SERVER_MISSION_SET_NAME = 0x809c,
        SERVER_MISION_INFO_RESPONSE = 0x8099,
        SERVER_MISION_LOCATION_POSITION = 0x809E,
        SERVER_LOOT_WINDOW_RESPONSE = 0x8119,
        SERVER_LOOT_ACCEPTED_RESPONSE = 0x811a,
        SERVER_TEAM_CREATE        = 0x808d,
        SERVER_TEAM_INVITE_MEMBER = 0x808f,

        // Abilitys
        SERVER_CAST_BAR_ABILITY = 0x80ac,
        SERVER_ABILITY_LOAD = 0x80b2,
        SERVER_ABILITY_UNLOAD = 0x80b3,
        SERVER_HYPERJUMP_ID = 0x80c3,

        // Chat & Commands
        SERVER_CHAT_MESSAGE_RESPONSE = 0x2e,
        SERVER_CHAT_WHEREAMI_RESPONSE = 0x8154, // CR2: 0x8154


        // Marketplace
        SERVER_LOAD_MARKERPLACE = 0x8125,
        SERVER_VENDOR_OPEN = 0x810d,

        // Crew & Faction
        SERVER_FACTION_NAME_RESPONSE = 0x80f5,
        SERVER_FACTION_ENABLED_WINDOW = 0x8086,

    }

    public enum ProtocolHeaders
    {
        OBJECT_VIEW_PROTOCOL = 0x03,
        RPC_PROTOCOL = 0x04,
        MPM_05_PROTOCOL = 0x05

    }
}

