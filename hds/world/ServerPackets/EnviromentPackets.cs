using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using hds.resources.gameobjects;
using hds.shared;

namespace hds
{
    public partial class ServerPackets
    {
        public void SendSpawnGameObject(WorldClient client, GameObject creationObjectData, UInt64 entityID)
        {
            ClientView staticObjectView = client.viewMan.GetViewForEntityAndGo(entityID,NumericalUtils.ByteArrayToUint16(creationObjectData.GetGoid(),1));
            if (staticObjectView.viewCreated == false)
            {
                PacketContent pak = new PacketContent();

                pak.AddUint16(1, 1);
                pak.AddByteArray(Store.world.objMan.GenerateCreationPacket(creationObjectData, 0x0000, client.playerData.assignSpawnIdCounter()).getBytes());
                pak.AddUint16(staticObjectView.ViewID, 1);
                pak.AddByte(0x00);
 
                client.messageQueue.addObjectMessage(pak.ReturnFinalPacket(), false);
                client.FlushQueue();
                staticObjectView.viewCreated = true;
            }

        }

        public void SendUpdateViewStatePacket(WorldClient client, UInt16 viewId, byte[] updateData)
        {
            PacketContent pak = new PacketContent();
            pak.AddUint16(viewId,1);
            pak.AddByteArray(updateData);
            client.messageQueue.addObjectMessage(pak.ReturnFinalPacket(), true);
        }

        public void SendElevatorPanel(WorldClient client, StaticWorldObject objectValues)
        {
            PacketContent pak = new PacketContent();
            pak.AddUint16((UInt16)RPCResponseHeaders.SERVER_ELEVATOR_PANEL,0);
            pak.AddUint16((UInt16)NumericalUtils.RotateRight(objectValues.sectorID,4),1);
            pak.AddHexBytes("4400");
            client.messageQueue.addRpcMessage(pak.ReturnFinalPacket());
        }

        public void SendHardlineExit(WorldClient client)
        {
            PacketContent pak = new PacketContent();
            pak.AddUint16((UInt16)RPCResponseHeaders.SERVER_EXIT_HL, 0);
            pak.AddByte(0x95);
            pak.AddByte(0x00);
            pak.AddByte(0x00);
            pak.AddByte(0x00);
            pak.AddByte(0x01);

            Store.currentClient.messageQueue.addRpcMessage(pak.ReturnFinalPacket());
        }

        public void SendInteractionSubway(WorldClient client, double x, double y, double z)
        {
            PacketContent pak = new PacketContent();
            pak.AddUint16((UInt16)RPCResponseHeaders.SERVER_SUBWAY_INTERACTION,0);
            pak.AddDouble(x,1);
            pak.AddDouble(y,1);
            pak.AddDouble(z,1);
            client.messageQueue.addRpcMessage(pak.ReturnFinalPacket());
        }
        public void SendSubwaymapWindow(WorldClient client)
        {
            PacketContent pak = new PacketContent();
            pak.AddUint16((UInt16)RPCResponseHeaders.SERVER_SUBWAY_MAP, 0);
            pak.AddUShort(10);
            Store.currentClient.messageQueue.addRpcMessage(pak.ReturnFinalPacket());

        }
    }
}