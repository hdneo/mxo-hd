using System;
using System.Collections.Generic;
using System.Text;
using hds.shared;

namespace hds
{
    public class ChatHandler
    {

        private ChatCommandsHelper chatCommands;


        public void processChat(ref byte[] packetData)
        {

            PacketReader reader = new PacketReader(packetData);
            UInt16 typeId = reader.readUInt16(1);

            string typeString = "AREA";

            switch (typeId)
            {
                case 19:
                    typeString = "FACTION";
                    break;
                case 18:
                    typeString = "CREW";
                    break;
                case 21:
                    typeString = "TEAM";
                    break;
                default:
                    typeString = "AREA";
                    break;
                
            }
            
            chatCommands = new ChatCommandsHelper();
            int offset = 0;

            //10 00 08 00 00 00 00 06 00 3f 53 61 76 65 00 

            int length = (int)packetData[7];
            offset = 9; //Move to length +2
            if (length > 0)
            {
                byte[] textB = new byte[length - 1];
                ArrayUtils.copy(packetData, offset, textB, 0, length - 1);
                string text = StringUtils.charBytesToString(textB);                

                if (text[0] == '?')
                {
                    //Maybe a parameter			
                    chatCommands.parseCommand(text); // Parse for commands
                }
                else
                {
                    // Not a Param - lets distribute the Message throw our Area 
                    ServerPackets pak = new ServerPackets();
                    pak.SendChatMessage(Store.currentClient, text, Store.currentClient.playerData.getCharID(), StringUtils.charBytesToString_NZ(Store.currentClient.playerInstance.CharacterName.getValue()), typeId);
                    // ToDo: Send the ChatMessage to the Scope of Players

                    
                }
            }

        }

        public void ProcessWhisperPlayer(ref byte[] rpcData)
        {
            PacketReader reader = new PacketReader(rpcData);
            // We read the offset but we dont need it really
            UInt16 offsetName = reader.readUInt16(1);
            UInt16 offsetMessage = reader.readUInt16(1);
            uint unknownZeroShort = reader.readUint8();
            uint chatCounter = reader.readUint8(); // ChatCounter (increment by 1 each message)
            string receiverHandleServerString = reader.readSizedZeroTerminatedString();
            string receiverMessage = reader.readSizedZeroTerminatedString();

            string cleanHandle = receiverHandleServerString.Replace("SOE+MXO+" + Store.worldConfig.serverName +"+", "");
            
            ServerPackets packets = new ServerPackets();
            packets.SendWhisperMesage(Store.currentClient, cleanHandle, receiverMessage);
        }
    }
}
