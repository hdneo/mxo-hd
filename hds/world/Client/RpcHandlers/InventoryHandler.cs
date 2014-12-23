using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using hds.shared;

namespace hds
{
    class InventoryHandler
    {
        
        public InventoryHandler()
        {
           
            
        }

    
        public void processItemMove(ref byte[] packet)
        {
            byte[] sourceSlot = {packet[0], packet[1]};
            byte[] destSlot   = {packet[2], packet[3]};

            UInt16 fromSlot =  NumericalUtils.ByteArrayToUint16(sourceSlot,1);
            UInt16 toSlot   = NumericalUtils.ByteArrayToUint16(destSlot,1);
            Store.dbManager.WorldDbHandler.updateInventorySlot(fromSlot, toSlot);

            ServerPackets pak = new ServerPackets();
            pak.sendInventoryItemMove(fromSlot, toSlot, Store.currentClient);
        }


        public void processItemAdd(UInt32 itemID, UInt16 type)
        {
            
            // ToDo: Get free slot, add item to Inventory
            // This method is called from looting and vendor buy (so vendor buy must care about money decrease)
            UInt16 freeSlot = Store.dbManager.WorldDbHandler.getFirstNewSlot();
            Store.dbManager.WorldDbHandler.addItemToInventory(freeSlot, itemID);

            ServerPackets pak = new ServerPackets();
            ushort amount = 0; // ToDo : make this dynamic lol
            pak.sendInventoryItemAdd(freeSlot, itemID, amount, type, Store.currentClient);

        }

        public void processItemDelete(ref byte[] packet)
        {
            
        }

        
    }
}
