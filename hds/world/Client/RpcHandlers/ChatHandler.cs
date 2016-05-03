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
                    pak.sendChatMessage(Store.currentClient, text, Store.currentClient.playerData.getCharID(), StringUtils.charBytesToString_NZ(Store.currentClient.playerInstance.CharacterName.getValue()), "AREA");
                    // ToDo: Send the ChatMessage to the Scope of Players

                    
                }
            }

        }

    }
}
