using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hds
{
    public partial class ServerPackets 
    {
        public void sendPlayFXOnView(UInt16 viewId, WorldClient client, byte animation, UInt16 incrementCounter)
        {
            PacketContent pak = new PacketContent();

            pak.addUint16(viewId, 1);
            // ToDo later : make a real Object Update
            pak.addByte(0x02);
            pak.addByte(0x80);
            pak.addByte(0x80);
            pak.addByte(0x80);
            pak.addByte(0x90);
            pak.addByte(0xed);
            pak.addByte(0x00);
            pak.addByte(0x30);
            pak.addByte(animation);
            pak.addUint16(incrementCounter, 1);

            client.messageQueue.addObjectMessage(pak.returnFinalPacket(), false);
            client.flushQueue();
        }
    }
}
