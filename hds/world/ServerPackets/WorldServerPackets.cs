using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using hds.shared;

namespace hds
{
    public partial class ServerPackets 
    {
        public void sendWorldCMD(WorldClient client, uint districtId, string enviromentOptions)
        {
            // Prepare the dict
            Dictionary<uint, string> locs = new Dictionary<uint, string>();
            locs.Add((uint)MxOLocations.TUTORIAL, "resource/worlds/final_world/tutorial_v2/tutorial_v2.metr");
            locs.Add((uint)MxOLocations.SLUMS, "resource/worlds/final_world/slums_barrens_full.metr");
            locs.Add((uint)MxOLocations.DOWNTOWN, "resource/worlds/final_world/downtown/dt_world.metr");
            locs.Add((uint)MxOLocations.INTERNATIONAL, "resource/worlds/final_world/international/it.metr");
            locs.Add((uint)MxOLocations.ARCHIVE01, "resource/worlds/final_world/constructs/archive/archive01/archive01.metr");
            locs.Add((uint)MxOLocations.ARCHIVE02, "resource/worlds/final_world/constructs/archive/archive02/archive02.metr");
            locs.Add((uint)MxOLocations.ASHENCOURT, "resource/worlds/final_world/constructs/archive/archive_ashencourte/archive_ashencourte.metr");
            locs.Add((uint)MxOLocations.DATAMINE, "resource/worlds/final_world/constructs/archive/archive_datamine/datamine.metr");
            locs.Add((uint)MxOLocations.SAKURA, "resource/worlds/final_world/constructs/archive/archive_sakura/archive_sakura.metr");
            locs.Add((uint)MxOLocations.SATI, "resource/worlds/final_world/constructs/archive/archive_sati/sati.metr");
            locs.Add((uint)MxOLocations.WIDOWSMOOR, "resource/worlds/final_world/constructs/archive/archive_widowsmoor/archive_widowsmoor.metr");
            locs.Add((uint)MxOLocations.YUKI, "resource/worlds/final_world/constructs/archive/archive_yuki/archive_yuki.metr");
            locs.Add((uint)MxOLocations.LARGE01, "resource/worlds/final_world/constructs/large/large01/large01.metr");
            locs.Add((uint)MxOLocations.LARGE02, "resource/worlds/final_world/constructs/large/large02/large02.metr");
            locs.Add((uint)MxOLocations.MEDIUM01, "resource/worlds/final_world/constructs/medium/medium01/medium01.metr");
            locs.Add((uint)MxOLocations.MEDIUM02, "resource/worlds/final_world/constructs/medium/medium02/medium02.metr");
            locs.Add((uint)MxOLocations.MEDIUM03, "resource/worlds/final_world/constructs/medium/medium03/medium03.metr");
            locs.Add((uint)MxOLocations.SMALL03, "resource/worlds/final_world/constructs/small/small03/small03.metr");
            locs.Add((uint)MxOLocations.CAVES, "resource/worlds/final_world/zion_caves.metr");
            locs.Add((uint)MxOLocations.LA, "resource/worlds/loading_area/la.metr");

            string path = locs[districtId];
            UInt16 offsetWeatherEvent = (UInt16)(path.Length + 17);

            PacketContent pak = new PacketContent();
            pak.addUintShort((ushort)RPCResponseHeaders.SERVER_LOAD_WORLD_CMD);
            pak.addUintShort(0x0e); // dunno if header or not - just part of this
            pak.addUintShort(0);
            pak.addUint32(districtId, 1); // Atlas Byte
            pak.addByteArray(TimeUtils.getCurrentSimTime());
            pak.addByte(0x01); // SimeTime + 01 
            pak.addUint16(offsetWeatherEvent, 1);
            pak.addSizedTerminatedString(path);
            pak.addSizedTerminatedString(enviromentOptions);

            client.messageQueue.addRpcMessage(pak.returnFinalPacket());

            
            
        }

        public void SendServerSettingUInt(WorldClient client, string key, UInt32 value)
        {
            PacketContent pak = new PacketContent();
            pak.addUintShort((ushort)RPCResponseHeaders.SERVER_FEATURE_EVENT);
            int fullLen = key.Length + 2 + 4;
            pak.addInt16((short)fullLen,1);
            pak.addSizedString(key);
            pak.addUint16(4,1);
            pak.addUint32(value,1);
            
            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }
        
        public void SendServerSettingCheckMotdMessage(WorldClient client, string key)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((ushort)RPCResponseHeaders.SERVER_FEATURE_EVENT,0);
            int fullLen = key.Length + 2 + 4;
            pak.addInt16((short)fullLen,1);
            pak.addSizedString(key);
            pak.addUint16(1,0);
            pak.addUint16(1,0);

            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }
        
        public void SendServerSettingString(WorldClient client, string key, string value)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((ushort)RPCResponseHeaders.SERVER_FEATURE_EVENT,0);
            int fullLen = key.Length + value.Length;
            pak.addInt16((short)fullLen,1);
            pak.addSizedString(key);
            pak.addSizedString(value);
            
            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }


        public void SendWorldSetup(WorldClient client)
        {
            // The Packet with PVP Flag etc. 
            // ToDo: load world setting and define it
            if (Store.worldConfig.IsPvpServer)
            {
                SendServerSettingUInt(client, "PvPServer", 6);
                SendServerSettingUInt(client, "PvPMaxSafeLevel", 16);
            }

            if (Store.worldConfig.FixedBinkIDOverride > 0)
            {
                SendServerSettingUInt(client, "FixedBinkIDOverride", Store.worldConfig.FixedBinkIDOverride );
            }
            
            // Load Events
            if (Store.worldConfig.events.Count > 0)
            {
                foreach (DictionaryEntry eventItem in Store.worldConfig.events)
                {
                    SendServerSettingString(client, eventItem.Key.ToString(), eventItem.Value.ToString());
                }
            }

            SendServerSettingCheckMotdMessage(client, "CheckMotdTimestamp");

        }

        public void createFlashTraffic(WorldClient client, string url)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_FLASH_TRAFFIC,0);
            pak.addHexBytes("0000070005");
            pak.addSizedString(url);
            pak.addByte(0);
            client.messageQueue.addRpcMessage(pak.returnFinalPacket());

        }
    }
}
