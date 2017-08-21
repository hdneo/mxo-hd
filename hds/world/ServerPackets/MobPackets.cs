using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using hds.shared;
namespace hds
{
    public partial class ServerPackets 
    {
        public void spawnMobView(WorldClient client, npc thismob, ClientView mobView)
        {
            Object599 viewData = thismob.getCreationData();
            PacketContent pak = new PacketContent();
            
            pak.addUint16(1, 1);
            pak.addByteArray(Store.world.objMan.GenerateCreationPacket(viewData, 0x0000, client.playerData.assignSpawnIdCounter()).getBytes());
            pak.addUint16(mobView.ViewID, 1);
            pak.addByte(0x00);

            client.messageQueue.addObjectMessage(pak.returnFinalPacket(), false);
            client.flushQueue();
        }

        public void sendNPCUpdateData(UInt16 viewId, WorldClient client, byte[] updateData)
        {
            PacketContent pak = new PacketContent();
            
            pak.addUint16(viewId, 1);
            pak.addByteArray(updateData);

            client.messageQueue.addObjectMessage(pak.returnFinalPacket(), false);
            client.flushQueue();
            
        }

        public void sendNPCUpdateAnimation(UInt16 viewId, WorldClient client, npc theMob, byte animation )
        {
            PacketContent pak = new PacketContent();
            pak.addUint16(viewId, 1);
            pak.addByte(0x02);
            pak.addByte(0x0e);
            pak.addByte(animation);
            pak.addUintShort((UInt16)theMob.getRotation());
            pak.addFloatLtVector3f((float)theMob.getXPos(), (float)theMob.getYPos(), (float)theMob.getZPos());


            client.messageQueue.addObjectMessage(pak.returnFinalPacket(), false);
            client.flushQueue();

        }

        public void sendNPCDies(UInt16 viewId, WorldClient client, npc theMob)
        {
            // Falls to ground like dead lol
            // 02010d000000000000000000000000000000


            //activate loot+f
            PacketContent pak = new PacketContent();
            pak.addUint16(viewId, 1);
            pak.addHexBytes("0501c20200808100000000e0010000c0830000de810303fd070000000000000000");
            client.messageQueue.addObjectMessage(pak.returnFinalPacket(), false);

            // lie on the ground
            /*
            PacketContent pak2 = new PacketContent();
            pak2.addUint16(viewId, 1);
            pak2.addHexBytes("02010d000000000000000000000000000000");
            client.messageQueue.addObjectMessage(pak2.returnFinalPacket(), false);
            */
            
            PacketContent fallToGroundPak = new PacketContent();
            fallToGroundPak.addUint16(viewId, 1);
            fallToGroundPak.addHexBytes("02010d000000000000000000000000000000");
            client.messageQueue.addObjectMessage(fallToGroundPak.returnFinalPacket(), false);
            

            client.flushQueue();

        }


        public void sendNPCUpdatePos(UInt16 viewId, WorldClient client, npc theMob)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16(viewId, 1);
            pak.addByte(0x02);
            pak.addByte(0x0c);
            pak.addUintShort((UInt16)theMob.getRotation());
            pak.addFloatLtVector3f((float)theMob.getXPos(), (float)theMob.getYPos(), (float)theMob.getZPos());


            client.messageQueue.addObjectMessage(pak.returnFinalPacket(), false);
            client.flushQueue();
        }

        public void sendLootWindow(UInt32 infoAmount, WorldClient client, UInt32[] objectIds)
        {
            // ToDo: figure the real object packet out (with more than one object) and show it up
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_LOOT_WINDOW_RESPONSE,0);
            pak.addHexBytes("000000001000");
            pak.addUint32(infoAmount,1);
            pak.addHexBytes("00000000010000000008");
            if (objectIds.Length > 0)
            {
                pak.addUint32(objectIds[0],1);
            }
            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
            client.flushQueue();
        }

        public void sendLootAccepted(WorldClient client)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_LOOT_ACCEPTED_RESPONSE,0);
            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
            client.flushQueue();
        }

        

        
    }
}
