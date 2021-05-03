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

            pak.AddUint16(viewId, 1);
            // ToDo later : make a real Object Update
            pak.AddByte(0x02);
            pak.AddByte(0x80);
            pak.AddByte(0x80);
            pak.AddByte(0x80);
            pak.AddByte(0x90);
            pak.AddByte(0xed);
            pak.AddByte(0x00);
            pak.AddByte(0x30);
            pak.AddByte(animation);
            pak.AddUint16(incrementCounter, 1);

            client.messageQueue.addObjectMessage(pak.ReturnFinalPacket(), false);
            client.FlushQueue();
        }
    }
}
