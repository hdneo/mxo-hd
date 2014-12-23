using System;
using System.Collections.Generic;
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

            string path = locs[districtId];
            UInt16 offsetWeatherEvent = (UInt16)(path.Length + 17);

            PacketContent pak = new PacketContent();
            pak.addUintShort((ushort)RPCResponseHeaders.SERVER_LOAD_WORLD_CMD);
            pak.addUintShort(0x0e); // dunno if header or not - just part of this
            pak.addUintShort(0);
            pak.addUint32(districtId, 1); // Atlas Byte
            pak.addByteArray(new byte[] { 0xd8, 0x68, 0xc8, 0x47, 0x01 }); // Unknown
            pak.addUint16(offsetWeatherEvent, 1);
            pak.addSizedTerminatedString(path);
            pak.addSizedTerminatedString(enviromentOptions);

            client.messageQueue.addRpcMessage(pak.returnFinalPacket());

            
            
        }

        public void sendSetWeather(WorldClient client)
        {

        }

        public void sendWorldSetup(WorldClient client)
        {
            // The Packet with PVP Flag etc. 
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
