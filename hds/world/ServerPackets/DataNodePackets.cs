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
            WorldSocket.entityIdCounter++;
            ClientView view = client.viewMan.getViewForEntityAndGo(WorldSocket.entityIdCounter, 47080);

            PacketContent pak = new PacketContent();
            pak.addUint16(1, 1);
            pak.addHexBytes("0ce8b7f5cdab0329");
            pak.addByteArray(client.playerInstance.Position.getValue());
            pak.addUintShort(1); // UseCount
            pak.addUint32(4001,1); // 112,CurrentTimerState,EventID,4
            pak.addUint16(view.ViewID, 1);
            pak.addByte(0x00);
            client.messageQueue.addObjectMessage(pak.returnFinalPacket(), false);
            client.FlushQueue();
        }
    }

}