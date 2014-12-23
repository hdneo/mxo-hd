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
            pak.addByteArray(Store.world.objMan.generateCreationPacket(viewData, 0x0000).getBytes());
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

        

        
    }
}
