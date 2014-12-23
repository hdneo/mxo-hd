using System;
using System.Collections;
using System.Text;

namespace hds{

    class VendorHandler{

        public VendorHandler(){
        }

        public void processBuyItem(ref byte[] packet){

            byte[] goByteID = {packet[0],packet[1],packet[2],packet[3]};
            UInt32 itemGoID = NumericalUtils.ByteArrayToUint32(goByteID,1);

            InventoryHandler inventory = new InventoryHandler();
            inventory.processItemAdd(itemGoID,0x10);

            // ToDo: decrease the money ?:)

        }

        public void processSellItem(ref byte[] packet){

        }
    }
}
