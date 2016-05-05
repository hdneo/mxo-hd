using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using hds.shared;
namespace hds
{
    public partial class ServerPackets 
    {

        public void sendVendorWindow()
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_VENDOR_OPEN,0);
            pak.addHexBytes("7cadd94300000000a0b7f6c000000000003c9440000000000051c8c020000a0000140080002000800018008000040080002c008000f41180009806800090078000c4068000640680");
            Store.currentClient.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }
        

    }
}