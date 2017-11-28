using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using hds.shared;

namespace hds
{
    public partial class ServerPackets
    {
        public void sendSpawnStaticObject(WorldClient client, GameObject creationObjectData, UInt64 entityID)
        {

            ClientView staticObjectView = client.viewMan.getViewForEntityAndGo(entityID,NumericalUtils.ByteArrayToUint16(creationObjectData.GetGoid(),1));
            if (staticObjectView.viewCreated == false)
            {
                PacketContent pak = new PacketContent();

                pak.addUint16(1, 1);
                pak.addByteArray(Store.world.objMan.GenerateCreationPacket(creationObjectData, 0x0000, client.playerData.assignSpawnIdCounter()).getBytes());
                pak.addUint16(staticObjectView.ViewID, 1);
                pak.addByte(0x00);

                client.messageQueue.addObjectMessage(pak.returnFinalPacket(), false);
                client.FlushQueue();
                staticObjectView.viewCreated = true;
            }

        }

        public void sendElevatorPanel(WorldClient client, StaticWorldObject objectValues)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_ELEVATOR_PANEL,0);
            pak.addUint16(objectValues.sectorID,1);
            pak.addHexBytes("2200");
            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }

        public void sendHardlineExit(WorldClient client)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_EXIT_HL, 0);
            pak.addByte(0x95);
            pak.addByte(0x00);
            pak.addByte(0x00);
            pak.addByte(0x00);
            pak.addByte(0x01);

            Store.currentClient.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }
    }
}