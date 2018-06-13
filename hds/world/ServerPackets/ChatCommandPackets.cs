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
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_CHAT_WHO_RESPONSE,0);
            pak.addUint16(5,1); // Alsways there
            pak.addByte(0x00); // We are not sure why but as its just 3 bytes for the nums we need "space" bytes
            pak.addUint16((UInt16)WorldSocket.Clients.Count,1);
            pak.addByte(0x00); // Again there is space
            foreach (string clientKey in WorldSocket.Clients.Keys)
            {
                WorldClient theClient = WorldSocket.Clients[clientKey] as WorldClient;
                // ToDo: this is not complete implemented - the first is maybe an offset- needs more research firstex               
                pak.addHexBytes("3c014801"); // This is just from logs
                pak.addByte(0xef); // profession
                pak.addHexBytes("010000");
                pak.addByteArray(theClient.playerInstance.Level.getValue());
                pak.addByte(0x01);
                pak.addByte(0x00);
            }

            foreach (string clientKey in WorldSocket.Clients.Keys)
            {
                WorldClient theClient = WorldSocket.Clients[clientKey] as WorldClient;
                // ToDo: this is really dirty hacky 
                string charname = StringUtils.charBytesToString_NZ(theClient.playerInstance.CharacterName.getValue());
                pak.addSizedTerminatedString(charname);
            }
            Store.currentClient.messageQueue.addRpcMessage(pak.returnFinalPacket());

        }

        public void sendWhereami(WorldClient client, byte[] xPos, byte[] yPos, byte[] zPos)
        {         
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_CHAT_WHEREAMI_RESPONSE, 0);
            pak.addByteArray(xPos);
            pak.addByteArray(yPos);
            pak.addByteArray(zPos);
            pak.addByte(0x07);
            pak.addByte(0x01);
            pak.addByte(0x00);
            Store.currentClient.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }
        
        public void sendLootWindowTest(WorldClient client, UInt32 objectId)
        {
            // This is just for reversing the missing model IDs
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_LOOT_WINDOW_RESPONSE,0);
            pak.addHexBytes("000000001000");
            pak.addUint32(99999,1);
            pak.addHexBytes("00000000010000000008");
            if (objectId > 0)
            {
                pak.addUint32(objectId,1);
            }
            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
            client.FlushQueue();
        }

    }
    
}
