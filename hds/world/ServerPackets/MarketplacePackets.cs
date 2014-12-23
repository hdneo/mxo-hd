using hds.shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hds
{
    public partial class ServerPackets
    {
        public void sendMarketPlaceList(WorldClient client)
        {
            // ToDo: make it dynamic with database later (but the "handler" should give the items as arguments for this method
            // List Marketplace Items
            
            byte[] headerSeperator = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            // Should foreach this later on "Real Server" 
            DynamicArray listItems = new DynamicArray();
            listItems.append(StringUtils.hexStringToBytes("8703000000000034bdcf1200401f00000310db6a4a")); // Foot Wear
            listItems.append(0x00); // Seperate each item

            listItems.append(StringUtils.hexStringToBytes("2e00000003a04a000bd012000000FFFF01aedc6a4a")); // Google
            listItems.append(0x00); // Seperate each item

            listItems.append(StringUtils.hexStringToBytes("721a000003a04a000bd012000000FFFF01aedc6a4a")); // Item Data
            listItems.append(0x00); // Seperate each item

            listItems.append(StringUtils.hexStringToBytes("2e00000003a04a000bd01200000000FF01aedc6a4a")); // Item Data
            listItems.append(0x00); // Seperate each item

            // get data size
            byte[] itemSize = NumericalUtils.uint16ToByteArray((UInt16)listItems.getBytes().Length, 1);

            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_LOAD_MARKERPLACE, 0);        
            pak.addUintShort(9); // list offset
            pak.addByteArray(headerSeperator);
            pak.addUint16((UInt16)listItems.getBytes().Length,1);
            pak.addByteArray(listItems.getBytes());
            client.messageQueue.addRpcMessage(pak.returnFinalPacket());


        }
    }
}
