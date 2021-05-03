using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using hds.shared;
namespace hds
{
    public partial class ServerPackets 
    {

        public void SendWhoCMD(WorldClient client)
        {
            
            PacketContent pak = new PacketContent();
            pak.AddUint16((UInt16)RPCResponseHeaders.SERVER_CHAT_WHO_RESPONSE,0);
            pak.AddUint16(5,1); // Alsways there
            pak.AddByte(0x00); // We are not sure why but as its just 3 bytes for the nums we need "space" bytes
            pak.AddUint16((UInt16)WorldServer.Clients.Count,1);
            pak.AddByte(0x00); // Again there is space
            foreach (string clientKey in WorldServer.Clients.Keys)
            {
                WorldClient theClient = WorldServer.Clients[clientKey] as WorldClient;
                // ToDo: this is not complete implemented - the first is maybe an offset- needs more research firstex               
                pak.AddHexBytes("3c014801"); // This is just from logs
                pak.AddByte(0xef); // profession
                pak.AddHexBytes("010000");
                pak.AddByteArray(theClient.playerInstance.Level.getValue());
                pak.AddByte(0x01);
                pak.AddByte(0x00);
            }

            foreach (string clientKey in WorldServer.Clients.Keys)
            {
                WorldClient theClient = WorldServer.Clients[clientKey] as WorldClient;
                // ToDo: this is really dirty hacky 
                string charname = StringUtils.charBytesToString_NZ(theClient.playerInstance.CharacterName.getValue());
                pak.AddSizedTerminatedString(charname);
            }
            Store.currentClient.messageQueue.addRpcMessage(pak.ReturnFinalPacket());

        }

        public void sendWhereami(WorldClient client, byte[] xPos, byte[] yPos, byte[] zPos)
        {         
            PacketContent pak = new PacketContent();
            pak.AddUint16((UInt16)RPCResponseHeaders.SERVER_CHAT_WHEREAMI_RESPONSE, 0);
            pak.AddByteArray(xPos);
            pak.AddByteArray(yPos);
            pak.AddByteArray(zPos);
            pak.AddByte(0x07);
            pak.AddByte(0x01);
            pak.AddByte(0x00);
            Store.currentClient.messageQueue.addRpcMessage(pak.ReturnFinalPacket());
        }

    }
    
}
