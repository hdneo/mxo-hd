using System;
using System.Collections;
using System.Text;

namespace hds{

    class VendorHandler{

        private readonly WorldClient client;


        public VendorHandler(WorldClient client)

        {

            this.client = client;

        }


        public void processBuyItem(ref byte[] packet){

            byte[] goByteID = {packet[0],packet[1],packet[2],packet[3]};
            UInt32 itemGoID = NumericalUtils.ByteArrayToUint32(goByteID,1);

            InventoryHandler inventory = new InventoryHandler(client);
            inventory.processItemAdd(itemGoID,0x10);

            // ToDo: decrease the money ?:)

        }

        public void processSellItem(ref byte[] packet){

        }
    }
}
