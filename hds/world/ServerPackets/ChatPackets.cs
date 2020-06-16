using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using hds.shared;

namespace hds
{
    public partial class ServerPackets
    {
        public void SendChatMessage(WorldClient fromClient, string message, UInt32 charId, string handle, UInt16 scopeId)
        {
            
            UInt32 offsetMessage = (uint) handle.Length + 36 + 3;
            PacketContent pak = new PacketContent();
            pak.addByte((byte) RPCResponseHeaders.SERVER_CHAT_MESSAGE_RESPONSE);
            pak.addUint16(scopeId,1);
            pak.addUint32(charId, 1);
            pak.addUint32(36, 1);
            pak.addUint32(offsetMessage, 1);
            pak.addHexBytes("000000000000000000000000000000000000000000"); // Unknown Zeros currently
            pak.addSizedTerminatedString(handle);
            pak.addSizedTerminatedString(message);

            switch (scopeId)
            {
                // 20 = Faction_Broadcast - never used ?
                case 18:
                    UInt32 crewId =
                        NumericalUtils.ByteArrayToUint32(Store.currentClient.playerInstance.CrewID.getValue(),
                            1);
                    Store.world.SendRPCToCrewMembers(crewId, Store.currentClient, pak.returnFinalPacket(), false);
                    break;
                case 19:
                case 20:
                    UInt32 factionId =
                        NumericalUtils.ByteArrayToUint32(Store.currentClient.playerInstance.FactionID.getValue(),
                            1);
                    Store.world.SendRPCToFactionMembers(factionId, Store.currentClient, pak.returnFinalPacket(), false);
                    break;

                case 21:
                    UInt32 missionTeamId =
                        NumericalUtils.ByteArrayToUint32(Store.currentClient.playerInstance.MissionTeamID.getValue(),
                            1);
                    Store.world.SendRPCToMissionTeamMembers(missionTeamId, Store.currentClient, pak.returnFinalPacket(), false);
                    break;

                default:
                    Store.world.sendRPCToAllOtherPlayers(Store.currentClient.playerData, pak.returnFinalPacket());
                    break;
            }
        }

        public void SendWhisperMesage(WorldClient client, string receiverHandle, string message)
        {
            string senderHandleStringWithPrefix = "SOE+MXO+" + Store.worldConfig.serverName + "+" +
                                                  StringUtils.charBytesToString_NZ(Store.currentClient.playerInstance
                                                      .CharacterName.getValue());
            
            PacketContent pak = new PacketContent();
            pak.addByte((byte) RPCResponseHeaders.SERVER_CHAT_MESSAGE_RESPONSE);
            pak.addByte(0x11);
            pak.addByte(0x00);
            pak.addUint32(0,1); // Always zero
            pak.addUint32(36,1); // Offset Handle Name- maybe its only UInt16 - always 36
            pak.addUint32((uint) (36 + senderHandleStringWithPrefix.Length + 3), 1);
            pak.addHexBytes("000000000000000000000000000000000000000000"); 
            pak.addSizedTerminatedString(senderHandleStringWithPrefix);
            pak.addSizedTerminatedString(message);
            
            Store.world.SendRPCToOnePlayerByHandle(pak.returnFinalPacket(), receiverHandle);
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


            UInt16 messageSize = (UInt16) (message.Length + 1);

            byte[] hexContents =
                StringUtils.hexStringToBytes("00000000000000000024000000000000000000000000000000000000000000000000");

            PacketContent pak = new PacketContent();
            pak.addByte((byte) RPCResponseHeaders.SERVER_CHAT_MESSAGE_RESPONSE);
            pak.addByte(typeByte);
            pak.addByteArray(hexContents);
            pak.addSizedTerminatedString(message);

            client.messageQueue.addRpcMessage(pak.returnFinalPacket());
            client.FlushQueue();
        }
    }
}