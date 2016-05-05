using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using hds.shared;

namespace hds
{
    class FCHandler
    {
        public void processLoadFactionName(ref byte[] packet)
        {
            byte[] factionBytes = { packet[0], packet[1], packet[2],packet[3] };
            UInt32 factionId = NumericalUtils.ByteArrayToUint32(factionBytes, 1);

            ServerPackets pak = new ServerPackets();
            pak.sendFactionName(Store.currentClient, factionId, "The Duality Dude");
        }

    }
}
