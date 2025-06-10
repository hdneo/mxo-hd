using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using hds.shared;
using System.Collections;
namespace hds
{
    public partial class ServerPackets 
    {

        public void sendMissionList(UInt16 contactId, uint orgID, List<Mission> missions, WorldClient client)
        {
            /*
             * ToDo: check if mission team was created or not
             */

            /* 
             * ToDo: Figure the "unknowns" out
             * Examples:
             * Neo: 18 80 95 00 00 00 00 06 00 07 00 00 01 d0 07 00 00 31 00 00 b4 c0 0c 00 28 
             * Luz: 18 80 95 00 00 00 00 05 00 01 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00
             * Raj: 18 80 95 00 00 00 00 00 00 01 00 01 00 22 00 00 00 f3 13 00 58 a5 11 00 0a (just one mission in list "The Hunter" - PB i think)
             * Raj: 18 80 95 00 00 00 00 00 00 01 00 01 00 34 00 00 00 15 14 00 58 a8 11 00 0a (just one mission in list "Pretty Retribution" - PB i think)
             */
            PacketContent pak = new PacketContent();
            pak.AddUint16((UInt16)RPCResponseHeaders.SERVER_MISSION_RESPONSE_LIST,0);
            pak.AddUint32(0, 0); // Unknown UInt32
            pak.AddUint16(contactId, 1);
            pak.AddUint16((ushort) missions.Count,1); // len of missions "categorys" to list
            pak.AddUShort(0);
            pak.AddUShort((ushort)orgID);
            pak.AddHexBytes("000000000000000000000000"); // The big unknown part - maybe some informations about the contact
            client.messageQueue.addRpcMessage(pak.ReturnFinalPacket());

            
            // ArrayList possibleMissions = new ArrayList();
            // possibleMissions.Add("Change the Organisation");
            // possibleMissions.Add("Welcome to HDS Experimental!");
            // possibleMissions.Add("Party like '99");

            UInt16 i = 0;
            foreach (Mission mission in missions)
            {
                PacketContent missionListPak = new PacketContent();
                missionListPak.AddUint16((UInt16)RPCResponseHeaders.SERVER_MISSION_RESPONSE_NAME, 0);
                missionListPak.AddUint16(contactId, 1);
                missionListPak.AddUint16(i, 1);
                missionListPak.AddHexBytes("0f0001"); // curently unknown
                missionListPak.AddUint32(0,1); // ContactId or something (if zero it just takes from the org but maybe needed for special contacts - seen d0070000 (Tyndal))
                missionListPak.AddUint16(0,1); // Unknown
                missionListPak.AddSizedTerminatedString(mission.title);
                client.messageQueue.addRpcMessage(missionListPak.ReturnFinalPacket());
                i++;
            }

            // And finally again a resposne
            PacketContent finalResponse = new PacketContent();
            finalResponse.AddUint16((UInt16)RPCResponseHeaders.SERVER_MISSION_LIST_END, 0);
            finalResponse.AddUint16(contactId, 1);
            client.messageQueue.addRpcMessage(finalResponse.ReturnFinalPacket());
            
        }

        public void sendMissionInfo(Mission mission, WorldClient client)
        {
            // Test a MissionList 
            PacketContent pak = new PacketContent();
            pak.AddUint16((UInt16)RPCResponseHeaders.SERVER_MISSION_INFO_RESPONSE, 0);
            pak.AddHexBytes("0000000000002f000100000002");

            // ToDo: calculate distance for the first and last location 
            float firstLocationDistance = 120 * 100;
            float maxLocationDistance = 230 * 100;
            
            pak.AddFloat(firstLocationDistance,1);
            pak.AddFloat(maxLocationDistance,1);
            pak.AddHexBytes("000000000000000000d0060000310000b4c00c0028");
            pak.AddHexBytes("42000001000002000D001300020002030200FFFFFDFF");
            pak.AddSizedTerminatedString(mission.description);
            client.messageQueue.addRpcMessage(pak.ReturnFinalPacket());
            client.FlushQueue();
        }

        public void sendMissionAccept(WorldClient client, UInt16 contactId, ushort missionId)
        {
            // This is still a testing message mate

            PacketContent selfUpdate = new PacketContent();
            selfUpdate.AddHexBytes("0200028040002a03020000");
            client.messageQueue.addObjectMessage(selfUpdate.ReturnFinalPacket(), false);

            PacketContent pak = new PacketContent();
            pak.AddUint16((UInt16)RPCResponseHeaders.SERVER_MISSION_SET_NAME, 0);
            pak.AddHexBytes("00000000");
            pak.AddUint16(contactId, 1);
            pak.AddHexBytes("2B0001002F0002D0070000310000B4C00C002814000026700A00280100003E003B00000E00417373617373696E6174696F6E000100000300AA0600009C070000DC050000");

            PacketContent pak2 = new PacketContent();
            pak2.AddHexBytes("80A33000030078001A000000030000003C000010EF4600007FC30020E4C50088DB4600007FC300E0ABC500000000000032015768696C65206F7665727420636F6E666C696374207365727665732069747320707572706F73652C20736F6D6574696D65732061206D6F726520737562746C6520617070726F6163682069732072657175697265642E20496E207468697320636173652C20616E20617373617373696E6174696F6E2E0D0A0D0A496E206F7264657220746F2072656163682042656E67742C20796F752077696C6C206669727374206E65656420746F206163717569726520612063657274696E2044617461204469736B207468617420686F6C64");

            // This tells the location 
            PacketContent locationMessage = new PacketContent();
            locationMessage.AddHexBytes("809E00000000000026DB40000000008078C1400000000000F8B1C00100000022018007");

            client.messageQueue.addRpcMessage(pak.ReturnFinalPacket());
            client.messageQueue.addRpcMessage(pak2.ReturnFinalPacket());
            client.messageQueue.addRpcMessage(locationMessage.ReturnFinalPacket());
            
            /*
            PacketContent pakWayPoint = new PacketContent();
            pakWayPoint.addHexBytes("01000213001200020C9DE1B8D747F0BF2D443752BA450000");
            client.messageQueue.addObjectMessage(pakWayPoint.returnFinalPacket(),false);
            */
            client.FlushQueue();

        }

        public void SendMissionAbort(WorldClient client)
        {
            PacketContent pak = new PacketContent();
            pak.AddHexBytes("80a20800000000b4658db5000000000000000000000000000000");
            client.messageQueue.addRpcMessage(pak.ReturnFinalPacket());
            client.FlushQueue();
        }



        public void sendMissionDownloadMap()
        {

        }

        public void SendSetMissionObjective(ushort id, ushort state, string missionObjective, WorldClient client)
        {
            //format : rpcsize+uint16header+ uint8(0) + uint16(0600), + uint8 state + sizedString (uint16 size + string + 00?)
            PacketContent pak = new PacketContent();
            pak.AddUint16((UInt16)RPCResponseHeaders.SERVER_MISSION_SET_OBJECTIVE, 0);
            pak.AddUShort(id);
            pak.AddUint16(6, 1);
            pak.AddUShort(state);
            pak.AddSizedTerminatedString(missionObjective);
            
            // Now send the message to the player queue
            client.messageQueue.addRpcMessage(pak.ReturnFinalPacket());
            
        }
    }
}