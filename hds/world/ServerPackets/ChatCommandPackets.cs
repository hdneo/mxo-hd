using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using hds.shared;
namespace hds
{
    public partial class ServerPackets 
    {

        public void sendWhereami(WorldClient client, byte[] xPos, byte[] yPos, byte[] zPos)
        {         
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_CHAT_WHEREAMI_RESPONSE, 0);
            pak.addByteArray(xPos);
            pak.addByteArray(yPos);
            pak.addByteArray(zPos);
            pak.addByte(0x07);
            pak.addByte(0x01);
            pak.addByte(0x00);
            Store.currentClient.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }

    }
}
