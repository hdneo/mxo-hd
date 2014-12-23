using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using hds.shared;
namespace hds
{
    public partial class ServerPackets 
    {
        // Place Methods here for Inventory

        public void sendInventoryItemMove(UInt16 sourceSlot, UInt16 destSlot, WorldClient client)
        {
            PacketContent pak = new PacketContent();
            pak.addByte(0x65);
            pak.addUint16(sourceSlot,1);
            pak.addUint16(destSlot,1);
            pak.addByte(0x00);
            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }

        public void sendInventoryItemAdd(UInt16 freeSlot, UInt32 itemId, ushort amount, UInt16 type, WorldClient client)
        {
            PacketContent pak = new PacketContent();
            pak.addByte(0x5e);
            pak.addUint16(freeSlot, 1);
            pak.addUint32(itemId, 1);
            pak.addUint16(0,1);
            pak.addUintShort(amount);
            pak.addUintShort(type);
            pak.addUintShort(1);
            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
            

            
        }

        public void sendInventoryItemDelete()
        {

        }
        
    }
}