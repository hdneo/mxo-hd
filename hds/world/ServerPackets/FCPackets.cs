using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using hds.shared;

namespace hds
{
    public partial class ServerPackets
    {

        public void sendFactionName(WorldClient client, UInt32 factionID, string factionName)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_FACTION_NAME_RESPONSE, 0);
            pak.addUint32(factionID,1);
            // Add 42 Bytes long faction name 
            pak.addStringWithFixedSized(factionName, 42);
            Store.currentClient.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }
    }
}
