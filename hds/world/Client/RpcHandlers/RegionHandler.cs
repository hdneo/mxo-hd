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

        public void ProcessRegionLoaded(ref byte[] packet)
        {

            byte[] objectIDBytes = new byte[4];
            byte[] sectorIDBytes = new byte[2];
            ArrayUtils.copyTo(packet, 0, objectIDBytes, 0, 4);
            ArrayUtils.copyTo(objectIDBytes, 2, sectorIDBytes, 0, 2);
            
            PacketReader pakReader = new PacketReader(packet);
            UInt16 sectorID = pakReader.readUInt16(1);
            UInt16 objectID = pakReader.readUInt16(1);
            float xPos = pakReader.readFloat(1);
            float yPos = pakReader.readFloat(1);
            float zPos = pakReader.readFloat(1);
            
            DataLoader objectLoader = DataLoader.getInstance();
            StaticWorldObject objectValues = objectLoader.getObjectValues(NumericalUtils.ByteArrayToUint32(objectIDBytes,1));

            UInt16 goId = NumericalUtils.ByteArrayToUint16(objectValues.type, 1);
            
            ServerPackets pak = new ServerPackets();
            #if DEBUG
            pak.sendSystemChatMessage(Store.currentClient,"Region Object ID " + objectID + " (GoType ID: " + goId + ") in Sector ID" + sectorID + " X:" + xPos + "Y:" + yPos + "Z:" + zPos,"BROADCAST");
            Output.WriteDebugLog("Region Object ID " + objectID + " (GoType ID: " + goId + ") in Sector ID" + sectorID + " X:" + xPos + "Y:" + yPos + "Z:" + zPos);
            #endif

            // TRy to spawn
            switch (goId)
            {
                case 8400:
                    // Spawn Signpost
                    ObjectAttributes8400 signpost = new ObjectAttributes8400("SIGNPOST",goId,objectValues.mxoId);
                    signpost.DisableAllAttributes();
                    
                    signpost.Position.enable();
                    signpost.SignpostNameString.enable();
                    signpost.AnimationID0.enable();
                    signpost.SignpostOrgID.enable();
                    signpost.DescriptionID.enable();
                    signpost.SignpostReqLevel.enable();
                    signpost.Orientation.enable();

                    signpost.Position.setValue(NumericalUtils.doublesToLtVector3d(xPos, yPos, zPos));
                    signpost.SignpostNameString.setValue(StringUtils.stringToBytes("NPC_SIGN_ID_" + objectID));
                    signpost.AnimationID0.setValue(new byte[]{ 0x02, 0x00, 0x00, 0x3A});
                    signpost.SignpostOrgID.setValue(0);
                    signpost.DescriptionID.setValue(new byte[]{ 0x3D, 0x0B, 0x00, 0x58});
                    signpost.SignpostReqLevel.setValue(0);
                    signpost.Orientation.setValue(StringUtils.hexStringToBytes(objectValues.quat));
                    
                    String entityMxOHackString = "" + objectValues.metrId + "" + objectValues.mxoId;
                    UInt64 entityId = UInt64.Parse(entityMxOHackString);
                    
                    pak.SendSpawnStaticObject(Store.currentClient,signpost,entityId);
                    break;
                
            }
           
        }
    }
}
