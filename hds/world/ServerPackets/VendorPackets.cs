using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using hds.shared;
namespace hds
{
    public partial class ServerPackets 
    {

        public void sendVendorWindow(WorldClient client, Vendor vendor)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_VENDOR_OPEN,0);
            pak.addByteArray(new byte[]{0x7c, 0xad, 0xd9, 0x43});
            // ToDo: Research what position should be here - Vendor or Player and why ?
            pak.addByteArray(Store.currentClient.playerInstance.Position.getValue());
            pak.addByteArray(new byte[]{0x20,0x00}); // Always 20 00 (i think its an offset)
            pak.addUint16((UInt16)vendor.items.Count,1);
            foreach (UInt32 item in vendor.items)
            {
                pak.addUint32(item,1);
            }
            //pak.addHexBytes("7cadd94300000000a0b7f6c000000000003c9440000000000051c8c020000a0000140080002000800018008000040080002c008000f41180009806800090078000c4068000640680");

            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }
        

    }
}