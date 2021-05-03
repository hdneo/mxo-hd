using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using hds.shared;

namespace hds
{
    public partial class ServerPackets
    {

        public void spawnDataNodeView(WorldClient client)
        {

            // ToDo: implement it in our Object Attribute Generation System
            // #VALUES FOR GOID: 47080, DataNodeContainer
            // #USING 305, Binary-Present: YES
            // 276,Position,LTVector3d,24
            // 140,HalfExtents,LTVector3f,12
            // 194,MissionKey,MissionKey,4
            // 304,UseCount,uint8,1
            // 108,CurrentState,EventID,4
            // 112,CurrentTimerState,EventID,4
            // 260,Orientation,LTQuaternion,16
            WorldServer.entityIdCounter++;
            ClientView view = client.viewMan.GetViewForEntityAndGo(WorldServer.entityIdCounter, 47080);

            PacketContent pak = new PacketContent();
            pak.AddUint16(1, 1);
            pak.AddHexBytes("0ce8b7f5cdab0329");
            pak.AddByteArray(client.playerInstance.Position.getValue());
            pak.AddUShort(1); // UseCount
            pak.AddUint32(4001,1); // 112,CurrentTimerState,EventID,4
            pak.AddUint16(view.ViewID, 1);
            pak.AddByte(0x00);
            client.messageQueue.addObjectMessage(pak.ReturnFinalPacket(), false);
            client.FlushQueue();
        }
    }

}