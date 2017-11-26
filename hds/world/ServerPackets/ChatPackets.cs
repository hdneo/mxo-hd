using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using hds.shared;
namespace hds
{
    public partial class ServerPackets
    {

        public void sendChatMessage(WorldClient fromClient, string message, UInt32 charId, string handle, string scope)
        {
            byte typeByte;
            switch (scope)
            {

                case "TEAM":
                    typeByte = 0x05;
                    break;
                case "CREW":
                    typeByte = 0x02;
                    break;

                case "FACTION":
                    typeByte = 0x03;
                    break;

                case "AREA":
                    typeByte = 0x10;
                    break;
                default:
                    typeByte = 0x07;
                    break;
            }


            UInt16 messageSize = (UInt16)(message.Length + 1);

            byte[] messageSizeHex = NumericalUtils.uint16ToByteArray(messageSize, 1);

            UInt32 offsetMessage = (uint)handle.Length + 35 + 2 + 2;
            PacketContent pak = new PacketContent();
            pak.addByte((byte)RPCResponseHeaders.SERVER_CHAT_MESSAGE_RESPONSE);
            pak.addByte(typeByte);
            pak.addUint32(charId, 0);
            pak.addUint16(36, 0);
            pak.addUint32(offsetMessage, 0);
            pak.addHexBytes("000000000000000000000000000000000000000000000000");
            pak.addSizedTerminatedString(handle);
            pak.addSizedTerminatedString(message);

            switch (typeByte)
            {
                case 0x02:
                    Store.world.sendRPCToCrewMembers(Store.currentClient, pak.returnFinalPacket());
                    break;
                case 0x03:
                    Store.world.sendRPCToFactionMembers(Store.currentClient, pak.returnFinalPacket());
                    break;
                    
                case 0x05:
                    Store.world.sendRPCToMissionTeamMembers(Store.currentClient, pak.returnFinalPacket());
                    break;
            
                default:
                    Store.world.sendRPCToAllOtherPlayers(Store.currentClient.playerData, pak.returnFinalPacket());
                    break;
                           
            }
             
            
        }
        
        public void sendSystemChatMessage(WorldClient client, string message, string type)
        {
            byte typeByte;
            switch (type)
            {

               
                case "SYSTEM":
                    typeByte = 0x07;
                    break;
                case "MODAL":
                    typeByte = 0x17;
                    break;
                case "FRAMEMODAL":
                    typeByte = 0xd7;
                    break;
                case "BROADCAST":
                    typeByte = 0xc7;
                    break;

                default:
                    typeByte = 0x07;
                    break;
            }
            

            UInt16 messageSize = (UInt16)(message.Length + 1);
                        
            byte[] hexContents = StringUtils.hexStringToBytes("00000000000000000024000000000000000000000000000000000000000000000000");

            PacketContent pak = new PacketContent();
            pak.addByte((byte)RPCResponseHeaders.SERVER_CHAT_MESSAGE_RESPONSE);
            pak.addByte(typeByte);
            pak.addByteArray(hexContents);
            pak.addSizedTerminatedString(message);
                        
            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
            client.FlushQueue();
        }
        

    }
}
