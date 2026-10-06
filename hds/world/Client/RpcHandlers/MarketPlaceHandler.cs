using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using hds.shared;

namespace hds{

    public class MarketPlaceHandler{

        private readonly WorldClient client;


        public MarketPlaceHandler(WorldClient client)

        {

            this.client = client;

        }


        public void processMarketplaceList(ref byte[] packet)
        {
            
            // ToDo: Load Data from Database for each goCategory 
            ServerPackets pak = new ServerPackets(client);
            pak.sendMarketPlaceList(client);
        }
    }
}
