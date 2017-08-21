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

        public void sendCrewAndFactionEnableWindow(WorldClient client)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_FACTION_ENABLED_WINDOW,0);
            pak.addHexBytes("15A0070000000000000000000000000000000000000000210000000000230000000000");
            client.messageQueue.addRpcMessage(pak.returnFinalPacket());

        }

        public void sendCrewInviteToPlayer(string playerHandle, string crewName)
        {
            // ToDo: fix the name display issue ?
            string charname = StringUtils.charBytesToString_NZ(Store.currentClient.playerInstance.CharacterName.getValue());
            UInt16 crewOffset = (UInt16) (charname.Length + 7 + 3);
            PacketContent pak = new PacketContent();
            pak.addHexBytes("8088");
            pak.addUint16(7,1); // Start Offsset for Charactername
            pak.addUint16(crewOffset,1);
            pak.addByte(0x01);
            pak.addSizedTerminatedString(StringUtils.charBytesToString_NZ(Store.currentClient.playerInstance.CharacterName.getValue()));
            pak.addSizedTerminatedString(crewName);
            Store.world.sendRPCToOnePlayerByHandle(pak.returnFinalPacket(),playerHandle);

        }
        
    }
}
