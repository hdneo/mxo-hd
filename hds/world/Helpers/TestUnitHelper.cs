using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using hds.shared;

namespace hds
{
    class TestUnitHandler{

        /// <summary>
        /// This is JUST for Testing Packets and so on ;)
        /// Will be removed later
        /// </summary>
        /// <param name="packet"></param>
        public void processAnimTest(ref byte[] packet){


            // From : http://mxoemu.info/mxoanim/movementAnims.txt
            // Is needed to display the animations of other players

        }

        public void processMarketTest(ref byte[] packet)
        {
            byte[] marketCat = new byte[4];
            ArrayUtils.copy(packet, 0, marketCat, 0, 4);
            Console.WriteLine("Open Market Place for Category : " + StringUtils.charBytesToString(marketCat));
            byte[] response = { 0x0b, 0x81, 0x25, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] notEmptyResponse = StringUtils.hexStringToBytes("812a090000000000001800f09a00000000040086d1120080841e00a086010030086e4a"); // Should be the viewData when the marketplace get opened
            Store.currentClient.messageQueue.addRpcMessage(notEmptyResponse);

        }

        public void processChangeTactic(ref byte[] packet)
        {
            byte[] tactic = StringUtils.hexStringToBytes("80bc15008a0000f90300000802ecffffff0100000000");
            Store.currentClient.messageQueue.addRpcMessage(tactic);
        }

        public void processTestCombat(ref byte[] packet)
        {
            // Description:
            // 0xbc Header should set a bonus
            new AnimationHelper().processPlayFX((UInt32)FXList.FX_CHARACTER_STUN_CAST);
            byte[] tactic = StringUtils.hexStringToBytes("81682D001D00030000007200000050004D0000000000003DB9000000000E00416674657257686F72754E656F000A00436F72727570746564001680BC1100D9030077030000890300000000010000A040"); 
            Store.currentClient.messageQueue.addRpcMessage(tactic);
           
        }

        public void processTestRPC()
        {
            // This is just a test function for brute forcing RPC 
            
        }


        public void testHyperJumpPaket()
        {
            // TEST PAK 
            // FULL PAK : 829AF325490302000308F3FA8446BB2E0D463E2251C633F98328FF016401000000000000000000000008416E646572736F6E0654686F6D6173FF0480731AE98280600400028E000000010000000000008F010002002A0302FF280A3200F0204604008E168428000000605085D64000000000007EA3400000008025DAC9C000FF0000000000F7000000EB0000000000280AFF00315E00000000000000000000000000000000FF000000000000000000000000803F110000004BB10000710200003F0000000122000000000000000500010004F3FA8446BB2E0D463E2251C60000
            byte[] hyperJumpTestPak = StringUtils.hexStringToBytes("02000308F3FA8446BB2E0D463E2251C633F98328FF016401000000000000000000000008416E646572736F6E0654686F6D6173FF0480731AE98280600400028E000000010000000000008F010002002A0302FF280A3200F0204604008E168428000000605085D64000000000007EA3400000008025DAC9C000FF0000000000F7000000EB0000000000280AFF00315E00000000000000000000000000000000FF000000000000000000000000803F110000004BB10000710200003F0000000122000000000000000500010004F3FA8446BB2E0D463E2251C60000");
            Store.currentClient.messageQueue.addObjectMessage(hyperJumpTestPak, true);
        }

        public void testCloseCombat(ref byte[] packet)
        {
            byte[] targetViewWithSpawnId = new byte[4];
            ArrayUtils.copy(packet, 0, targetViewWithSpawnId, 0, 4);
            string hexString = StringUtils.bytesToString_NS(targetViewWithSpawnId);
            string hexStringPak = StringUtils.bytesToString_NS(packet);
            Store.currentClient.messageQueue.addObjectMessage(StringUtils.hexStringToBytes("020003010C00808400808080800100001000"), false); // Make me combat mode "on"

            // The 55 View Packet
            PacketContent ilCombatHandler = new PacketContent();
            ilCombatHandler.addHexBytes("01000C370036CDAB0205");
            ilCombatHandler.addByteArray(Store.currentClient.playerInstance.Position.getValue());
            ilCombatHandler.addHexBytes("cdec4023"); // Time starts i think
            ilCombatHandler.addHexBytes("fd0000"); // view ID fd00
            Store.currentClient.messageQueue.addObjectMessage(ilCombatHandler.returnFinalPacket(), false);
            Store.currentClient.flushQueue();
            // The other 03 Packet for combat
            PacketContent unknownCreatePak = new PacketContent();
            unknownCreatePak.addByteArray(StringUtils.hexStringToBytes("010002A700"));
            unknownCreatePak.addByteArray(StringUtils.hexStringToBytes("FD00")); // ViewID from Combat Object 
            unknownCreatePak.addByteArray(StringUtils.hexStringToBytes("01"));
            unknownCreatePak.addByteArray(Store.currentClient.playerInstance.Position.getValue());
            unknownCreatePak.addByteArray(StringUtils.hexStringToBytes("0100000003000000"));
            unknownCreatePak.addByteArray(targetViewWithSpawnId);
            unknownCreatePak.addUint16(2, 1);
            unknownCreatePak.addUint16(Store.currentClient.playerData.selfSpawnIdCounter, 1); 
            unknownCreatePak.addByteArray(StringUtils.hexStringToBytes("01010207030000200BF5C2000020C19420B9C300000000000020C100000000070001001201000007037608E00603145200008B0B0024145200008B0B0024882300008B0B00240000000000000000000000000000000064000000640000000010001010000000020000001000000002000000000000000000000000"));
            Store.currentClient.messageQueue.addObjectMessage(unknownCreatePak.returnFinalPacket(), false);
            Store.currentClient.flushQueue();

        }

        public void testRemoveCloseCombat(ref byte[] packet)
        {
            ServerPackets serverPackets = new ServerPackets();
            serverPackets.sendDeleteViewPacket(Store.currentClient, 253);
        }

        public float getDistance(float x1,float y1,float z1,float x2,float y2, float z2)
        {
            float deltaX = x1 - x2;
            float deltaY = y1 - y2;
            float deltaZ = z1 - z2;
            float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
            return distance;
        }

        public void processHyperJump(ref byte[] packet)
        {
            byte[] destXBytes = new byte[8];
            byte[] destYBytes = new byte[8];
            byte[] destZBytes = new byte[8];
            byte[] maxHeight = new byte[4];
            byte[] theLast4 = new byte[4]; // we dont know what this is lol
            DynamicArray restBytes = new DynamicArray();

            ArrayUtils.copy(packet, 0, destXBytes, 0, 8);
            ArrayUtils.copy(packet, 8, destYBytes, 0, 8);
            ArrayUtils.copy(packet, 16, destZBytes, 0, 8);
            ArrayUtils.copy(packet, 30, maxHeight, 0, 4);
            ArrayUtils.copy(packet, packet.Length - 4,theLast4, 0, 4);

            // Players current X Z Y
            double x = 0; double y = 0; double z = 0;
            byte[] Ltvector3d = Store.currentClient.playerInstance.Position.getValue();
            NumericalUtils.LtVector3dToDoubles(Ltvector3d, ref x, ref y, ref z);
            int rotation = (int)Store.currentClient.playerInstance.YawInterval.getValue()[0];
            float xPos = (float)x;
            float yPos = (float)y;
            float zPos = (float)z;

            float xDestFloat = (float)NumericalUtils.byteArrayToDouble(destXBytes,1);
            float yDestFloat = (float)NumericalUtils.byteArrayToDouble(destYBytes, 1);
            float zDestFloat = (float)NumericalUtils.byteArrayToDouble(destZBytes, 1);

            float distance = getDistance(xPos, yPos, zPos, xDestFloat, yDestFloat, zDestFloat);
            UInt16 duration = (UInt16)(distance * 1.5);
            //UInt32 startTime = TimeUtils.getUnixTimeUint32() - 100000;
            //UInt32 endTime = startTime + duration;

            UInt32 startTime = TimeUtils.getUnixTimeUint32();
            UInt32 endTime = startTime + duration;
            PacketContent pak = new PacketContent();
            pak.addByte(0x02);
            pak.addByte(0x00);
            pak.addByte(0x03);
            pak.addByte(0x09);
            pak.addByte(0x08);
            pak.addByte(0x00);
            pak.addFloatLtVector3f(xPos, yPos, zPos);
            pak.addUint32(startTime, 1);
            pak.addByte(0x80);
            pak.addByte(0x80);
            pak.addByte(0xb8);
            pak.addByte(0x14); // if 0xb8
            pak.addByte(0x00); // if 0xb8
            pak.addUint32(endTime, 1);
            pak.addByteArray(destXBytes);
            pak.addByteArray(destYBytes);
            pak.addByteArray(destZBytes);
            pak.addByteArray(new byte[] { 0x10, 0xff, 0xff });
            pak.addByte(0x00);
            pak.addByte(0x00);
            pak.addByte(0x00);
            Store.currentClient.messageQueue.addObjectMessage(pak.returnFinalPacket(), true);
        
        }

      

    }
}
