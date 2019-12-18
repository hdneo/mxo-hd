using System;
using System.Collections.Generic;
using System.Collections;
using System.Data.Common;
using System.Text;

using hds.shared;

namespace hds
{
    class RegionHandler
    {
        public RegionHandler()
        {

        }

        public void processRegionLoaded(ref byte[] packet)
        {

            PacketReader pakReader = new PacketReader(packet);
            UInt16 sectorID = pakReader.readUInt16(1);
            UInt16 objectID = pakReader.readUInt16(1);
            float xPos = pakReader.readFloat(1);
            float yPos = pakReader.readFloat(1);
            float zPos = pakReader.readFloat(1);

            ServerPackets pak = new ServerPackets();
            #if DEBUG
            pak.sendSystemChatMessage(Store.currentClient,"Region Object ID " + objectID + " in Sector ID" + sectorID + " X:" + xPos + "Y:" + yPos + "Z:" + zPos,"BROADCAST");
            Output.WriteDebugLog("Region Object ID " + objectID + " in Sector ID" + sectorID + " X:" + xPos + "Y:" + yPos + "Z:" + zPos);
            #endif

            //Store.currentClient.messageQueue.addObjectMessage(StringUtils.hexStringToBytes("020002808080808010110000"));
            /*
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80B31100"));
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("3A050011000A0050765053657276657200040002000000")); // PVP Server Setting
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("3A0500170010005076504D6178536166654C6576656C0004000F000000")); // PVP Max Safe Level
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("3A050014000D0057525F52657A4576656E7473002B0048616C6C6F7765656E5F4576656E742C57696E7465723348616C6C6F7765656E466C794579655453454300")); // WR_REZ_events, Halloween, winter etc.
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("3A0500190012004576656E74536C6F74315F456666656374000000")); // Event Slot : 1_Effect
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("3A0500190012004576656E74536C6F74325F456666656374000D00666C796D616E5F69646C653300")); // Event Slot : 2_Effect flyman_idle
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("3A05001B001400466978656442696E6B49444F766572726964650002002000")); // FixedBinkIDOverride
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("8167170020001C2200C60111000000000000002900000807006D786F656D750007006D786F656D750002000200000000000000")); // Holds character name
            Store.currentClient.messageQueue.addRpcMessage(StringUtils.hexStringToBytes("80bd051100000000000001"));
             */
        }
    }
}
