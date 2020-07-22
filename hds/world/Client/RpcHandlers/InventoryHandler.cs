using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using hds.shared;

namespace hds
{
    class InventoryHandler
    {
        public void processItemMove(ref byte[] packet)
        {
            byte[] sourceSlot = {packet[0], packet[1]};
            byte[] destSlot   = {packet[2], packet[3]};

            UInt16 fromSlot =  NumericalUtils.ByteArrayToUint16(sourceSlot,1);
            UInt16 toSlot   = NumericalUtils.ByteArrayToUint16(destSlot,1);
            Store.dbManager.WorldDbHandler.UpdateInventorySlot(fromSlot, toSlot, Store.currentClient.playerData.getCharID());

            ServerPackets pak = new ServerPackets();
            pak.sendInventoryItemMove(fromSlot, toSlot, Store.currentClient);
        }


        public void processItemAdd(UInt32 itemID, UInt16 type)
        {
            
            // ToDo: Get free slot, add item to Inventory
            // This method is called from looting and vendor buy (so vendor buy must care about money decrease)
            UInt16 freeSlot = Store.dbManager.WorldDbHandler.GetFirstNewSlot();
            Store.dbManager.WorldDbHandler.AddItemToInventory(freeSlot, itemID);

            ServerPackets pak = new ServerPackets();
            ushort amount = 0; // ToDo : make this dynamic lol
            pak.sendInventoryItemAdd(freeSlot, itemID, amount, type, Store.currentClient);

        }

        public void processItemDelete(ref byte[] packet)
        {
            // ToDo: implement
            throw new NotImplementedException();
        }


        public void processUnmountItem(ref byte[] rpcData)
        {
            byte sourceSlot = rpcData[0];
            byte destSlot   = rpcData[1];

            UInt32 clothingItemId = Store.dbManager.WorldDbHandler.GetItemGOIDAtInventorySlot(rpcData[0]);
            DataLoader dataLoader = DataLoader.getInstance();
            ClothingItem item = dataLoader.getItemValues(clothingItemId);

            // Update Appeareance as we know which appeareance to update (as we unmount item it should be ok to set zeros for the values)
            string partModel = null;
            string partColor = null;
            switch (item.getClothesType())
            {
                case "1950892361":
                case "FEMALE HAT":
                case "MALE HAT":
                    partModel = "hat";
                    break;
                case "2001027401":
                case "FEMALE GLASSES":
                case "MALE GLASSES":
                    partModel = "glasses";
                    partColor = "glassescolor";
                    break;
                case "1750286665": // New Way but lets stay old values there
                case "FEMALE SHIRT":
                case "MALE SHIRT":
                    partModel = "shirt";
                    partColor = "shirtcolor";
                    break;
                case "1816609097":
                case "FEMALE GLOVES":
                case "MALE GLOVES":
                    partModel = "gloves";
                    break;
                case "2001682761":
                case "FEMALE COAT":
                case "MALE COAT":
                    partModel = "coat";
                    partColor = "coatcolor";
                    break;
                case "1397768521":
                case "MALE PANTS":
                case "FEMALE PANTS":
                    partModel = "pants";
                    partColor = "pantscolor";
                    break;
                case "1733050697":
                case "FEMALE LEGGINGS":
                    partModel = "leggins";
                    break;
                case "2001092937":
                case "FEMALE SHOES":
                case "MALE SHOES":
                    partModel = "shoes";
                    partColor = "shoecolor";
                    break;

            }

            if (partModel != null)
            {
                Store.dbManager.WorldDbHandler.UpdateRsiPartValue(partModel,0, Store.currentClient.playerData.getCharID());
            }

            if (partColor != null && item.getColorId()>0)
            {
                Store.dbManager.WorldDbHandler.UpdateRsiPartValue(partColor,0, Store.currentClient.playerData.getCharID());
            }
            // Move slot
            UInt16 newSlotId = Store.dbManager.WorldDbHandler.GetFirstNewSlot();
            Store.dbManager.WorldDbHandler.UpdateInventorySlot(sourceSlot,newSlotId, Store.currentClient.playerData.getCharID());

            // Send move packet
            ServerPackets pak = new ServerPackets();
            pak.sendInventoryItemMove(sourceSlot, newSlotId, Store.currentClient);

            Store.dbManager.WorldDbHandler.SetRsiValues();

            int[] current = Store.currentClient.playerData.getRsiValues();
            Store.currentClient.playerData.setRsiValues(current);
            byte[] rsiData = PacketsUtils.getRSIBytes(current);
            pak.sendAppeareanceUpdate(Store.currentClient, rsiData);
        }


        public void processMountItem(ref byte[] rpcData)
        {
            byte sourceSlot = rpcData[0];
            byte destSlot   = rpcData[1];


            UInt32 clothingItemId = Store.dbManager.WorldDbHandler.GetItemGOIDAtInventorySlot(rpcData[0]);
            DataLoader dataLoader = DataLoader.getInstance();
            ClothingItem item = dataLoader.getItemValues(clothingItemId);


            string partModel = null;
            string partColor = null;

            UInt16 toRealSlotId = 0;

            switch (item.getClothesType())
            {
                case "1950892361":
                case "FEMALE HAT":
                case "MALE HAT":
                    toRealSlotId = 0x61;
                    partModel = "hat";
                    break;
                case "2001027401":
                case "FEMALE GLASSES":
                case "MALE GLASSES":
                    toRealSlotId = 0x62;
                    partModel = "glasses";
                    partColor = "glassescolor";
                    break;
                case "1750286665": // New Way but lets stay old values there
                case "FEMALE SHIRT":
                case "MALE SHIRT":
                    toRealSlotId = 0x63;
                    partModel = "shirt";
                    partColor = "shirtcolor";
                    break;
                case "1816609097":
                case "FEMALE GLOVES":
                case "MALE GLOVES":
                    toRealSlotId = 0x64;
                    partModel = "gloves";
                    break;
                    
                case "2001682761":
                case "FEMALE COAT":
                case "MALE COAT":
                    toRealSlotId = 0x65;
                    partModel = "coat";
                    partColor = "coatcolor";
                    break;
                case "1397768521":
                case "MALE PANTS":
                case "FEMALE PANTS":
                    toRealSlotId = 0x66;
                    partModel = "pants";
                    partColor = "pantscolor";
                    break;
                case "1733050697":
                case "FEMALE LEGGINGS":
                    toRealSlotId = 0x67;
                    partModel = "leggins";
                    break;
                case "2001092937":
                case "FEMALE SHOES":
                case "MALE SHOES":
                    toRealSlotId = 0x68;
                    partModel = "shoes";
                    partColor = "shoecolor";
                    break;

            }

            if (partModel != null)
            {
                Store.dbManager.WorldDbHandler.UpdateRsiPartValue(partModel,item.getModelId(), Store.currentClient.playerData.getCharID());
            }

            if (partColor != null)
            {
                Store.dbManager.WorldDbHandler.UpdateRsiPartValue(partColor,item.getColorId(), Store.currentClient.playerData.getCharID());
            }
            Store.dbManager.WorldDbHandler.SetRsiValues();

            int[] current = Store.currentClient.playerData.getRsiValues();
            Store.currentClient.playerData.setRsiValues(current);
            byte[] rsiData = PacketsUtils.getRSIBytes(current);


            // Move the wear item from the wearing slot to the next new slot (but maybe doesnt work if inventory is full)

            ServerPackets pak = new ServerPackets();
            if (Store.dbManager.WorldDbHandler.IsSlotinUseByItem(toRealSlotId))
            {
                UInt16 newSlotId = Store.dbManager.WorldDbHandler.GetFirstNewSlot();
                Store.dbManager.WorldDbHandler.UpdateInventorySlot(toRealSlotId,newSlotId, Store.currentClient.playerData.getCharID());
                pak.sendInventoryItemMove(toRealSlotId, newSlotId, Store.currentClient);
            }
            pak.sendAppeareanceUpdate(Store.currentClient, rsiData);
            Store.dbManager.WorldDbHandler.UpdateInventorySlot(sourceSlot,toRealSlotId, Store.currentClient.playerData.getCharID());
            pak.sendInventoryItemMove(sourceSlot, toRealSlotId, Store.currentClient);



        }
    }
}
