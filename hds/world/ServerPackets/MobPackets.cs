using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using hds.shared;
namespace hds
{
    public partial class ServerPackets 
    {
        public void SpawnMobView(WorldClient client, Mob thismob, ClientView mobView)
        {
            Output.WriteDebugLog("Spawn MobView Data ( Entity ID : " + thismob.getEntityId() + " Name:" + thismob.getName() + " ID: " + thismob.getMobId() + " RSI HEX: " + thismob.getRsiHex());
            GameObject viewData = thismob.getCreationData();
            PacketContent pak = new PacketContent();
            
            pak.AddUint16(1, 1);
            pak.AddByteArray(Store.world.objMan.GenerateCreationPacket(viewData, 0x0000, client.playerData.assignSpawnIdCounter()).getBytes());
            pak.AddUint16(mobView.ViewID, 1);
            pak.AddByte(0x00);

            client.messageQueue.addObjectMessage(pak.ReturnFinalPacket(), false);
            client.FlushQueue();
        }

        public void SendNpcUpdateData(UInt16 viewId, WorldClient client, byte[] updateData)
        {
            PacketContent pak = new PacketContent();
            
            pak.AddUint16(viewId, 1);
            pak.AddByteArray(updateData);
            
            client.messageQueue.addObjectMessage(pak.ReturnFinalPacket(), false);
            client.FlushQueue();
            
        }

        public void SendNpcUpdateAnimation(UInt16 viewId, WorldClient client, Mob theMob, byte animation )
        {
            PacketContent pak = new PacketContent();
            pak.AddUint16(viewId, 1);
            pak.AddByte(0x02);
            pak.AddByte(0x0e);
            pak.AddByte(animation);
            pak.AddUShort((UInt16)theMob.getRotation());
            pak.AddFloatLtVector3f((float)theMob.getXPos(), (float)theMob.getYPos(), (float)theMob.getZPos());


            client.messageQueue.addObjectMessage(pak.ReturnFinalPacket(), false);
            client.FlushQueue();

        }

        public void SendNpcDies(UInt16 viewId, WorldClient client, Mob theMob)
        {
            // Falls to ground like dead lol

            //activate loot+f
            PacketContent pak = new PacketContent();
            pak.AddUint16(viewId, 1);
            pak.AddHexBytes("0501c20200808100000000e0010000c0830000de810303fd070000000000000000");
            client.messageQueue.addObjectMessage(pak.ReturnFinalPacket(), false);
            
            PacketContent fallToGroundPak = new PacketContent();
            fallToGroundPak.AddUint16(viewId, 1);
            fallToGroundPak.AddHexBytes("02010d000000000000000000000000000000");
            client.messageQueue.addObjectMessage(fallToGroundPak.ReturnFinalPacket(), false);
            
            // lie on the ground
            PacketContent pak2 = new PacketContent();
            pak2.AddUint16(viewId, 1);
            pak2.AddHexBytes("02010d000000000000000000000000000000");
            client.messageQueue.addObjectMessage(pak2.ReturnFinalPacket(), false);

            client.FlushQueue();

        }


        public void SendNpcUpdatePos(UInt16 viewId, WorldClient client, Mob theMob)
        {
            PacketContent pak = new PacketContent();
            pak.AddUint16(viewId, 1);
            pak.AddByte(0x02);
            pak.AddByte(0x0c);
            pak.AddUShort((UInt16)theMob.getRotation());
            pak.AddFloatLtVector3f((float)theMob.getXPos(), (float)theMob.getYPos(), (float)theMob.getZPos());


            client.messageQueue.addObjectMessage(pak.ReturnFinalPacket(), false);
            client.FlushQueue();
        }

        public void SendLootWindow(UInt32 infoAmount, WorldClient client, UInt32[] objectIds)
        {
            // ToDo: figure the real object packet out (with more than one object) and show it up
            PacketContent pak = new PacketContent();
            pak.AddUint16((UInt16)RPCResponseHeaders.SERVER_LOOT_WINDOW_RESPONSE,0);
            pak.AddHexBytes("000000001000");
            pak.AddUint32(infoAmount,1);
            pak.AddHexBytes("00000000010000000008");
            if (objectIds.Length > 0)
            {
                pak.AddUint32(objectIds[0],1);
            }
            client.messageQueue.addRpcMessage(pak.ReturnFinalPacket());
            client.FlushQueue();
        }

        public void SendLootAccepted(WorldClient client)
        {
            PacketContent pak = new PacketContent();
            pak.AddUint16((UInt16)RPCResponseHeaders.SERVER_LOOT_ACCEPTED_RESPONSE,0);
            client.messageQueue.addRpcMessage(pak.ReturnFinalPacket());
            client.FlushQueue();
        }
    }
}
