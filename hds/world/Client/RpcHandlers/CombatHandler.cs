using System;
using System.Timers;
using hds.shared;

namespace hds
{
    public class CombatHandler
    {
        public Timer combatTimer;
        public bool isCombatRunning = true;
        public UInt16 ilCombatViewId = 0;

        public void ProcessChangeTactic(ref byte[] packet)
        {
            PacketReader reader = new PacketReader(packet);
            string hexString = StringUtils.bytesToString_NS(packet);
            uint state = reader.ReadUint8();

            if (isCombatRunning && combatTimer != null)
            {
                combatTimer.Stop();
                combatTimer.Dispose();
            }
        }

        public void ProcessRequestCloseCombat(ref byte[] packet)
        {
            PacketReader reader = new PacketReader(packet);
            UInt32 targetViewWithSpanwId = reader.ReadUInt32(1);


            string hexStringPak = StringUtils.bytesToString_NS(packet);
            Store.currentClient.messageQueue.addObjectMessage(
                StringUtils.hexStringToBytes("020003010C00808400808080800100001000"),
                false); // Make me combat mode "on"

            var ilCombatHandler = new GameObjectDefinitions().Object55;
            ilCombatHandler.DisableAllAttributes();
            ilCombatHandler.StartTime.enable();
            ilCombatHandler.Position.enable();

            ilCombatHandler.StartTime.setValue(TimeUtils.getCurrentSimTime());
            ilCombatHandler.Position.setValue(Store.currentClient.playerInstance.Position.getValue());

            UInt64 currentEntityId = WorldServer.entityIdCounter;
            WorldServer.entityIdCounter++;
            WorldServer.gameServerEntities.Add(ilCombatHandler);

            ServerPackets packets = new ServerPackets();
            packets.SendSpawnGameObject(Store.currentClient, ilCombatHandler, currentEntityId);

            ClientView theView = Store.currentClient.viewMan.GetViewForEntityAndGo(currentEntityId,
                NumericalUtils.ByteArrayToUint16(ilCombatHandler.GetGoid(), 1));
            ilCombatViewId = theView.ViewID;

            // The other 03 Packet for combat
            PacketContent combatInitialUpdatePacket = new PacketContent();
            combatInitialUpdatePacket.AddByteArray(StringUtils.hexStringToBytes("010002A700"));

            combatInitialUpdatePacket.AddUint16(theView.ViewID, 1);
            combatInitialUpdatePacket.AddByteArray(StringUtils.hexStringToBytes("01"));
            combatInitialUpdatePacket.AddByteArray(Store.currentClient.playerInstance.Position.getValue());
            combatInitialUpdatePacket.AddByteArray(StringUtils.hexStringToBytes("0100000003000000"));
            combatInitialUpdatePacket.AddUint32(targetViewWithSpanwId, 1);
            combatInitialUpdatePacket.AddUint16(2, 1);
            combatInitialUpdatePacket.AddUint16(Store.currentClient.playerData.selfSpawnIdCounter, 1);
            combatInitialUpdatePacket.AddHexBytes("010102");
            // Combat Data 
            //combatInitialUpdatePacket.addByteArray(StringUtils.hexStringToBytes("07030000200BF5C2000020C19420B9C300000000000020C100000000070001001201000007037608E00603145200008B0B0024145200008B0B0024882300008B0B00240000000000000000000000000000000064000000640000000010001010000000020000001000000002000000000000000000000000"));
            // bob tutorial test
            //combatInitialUpdatePacket.addByteArray(StringUtils.hexStringToBytes("00000000005AC4C1000020C180D509C300000000000020C10000000001000100040100009A023405350503145200008B0B0024145200008B0B0024672300008B0B0024000000000000000000000000000000004C0000005E00000000100010000000000100000000000000010000000000000000000000"));
            // combat.log
            //combatInitialUpdatePacket.addByteArray(StringUtils.hexStringToBytes("070300008060acc2000020c13ca6dc4200000000000020c1000000000700010013010000a1053405350583145200008b0b0024145200008b0b00246d2300008b0b002400000000000000000000000000000000470000006600000000100010000000000100000000000000010000000000000000000000"));

            // Special agent test - here i stand still and not the first player "hit" 
            combatInitialUpdatePacket.AddByteArray(StringUtils.hexStringToBytes(
                "0703070300bafc42000020c1801baf4200803e40000020c1e0b319430000010013010000f40134059a02233c5200008b0b0024145200008b0b0024262000008b0b00240000000000000000000000000000000021000000700000000010001000000000000000000000000000010000022b600000000000"));
            Store.currentClient.messageQueue.addObjectMessage(combatInitialUpdatePacket.ReturnFinalPacket(), false);
            Store.currentClient.FlushQueue();

            combatTimer = new Timer(3000);
            combatTimer.Elapsed += UpdateCloseCombat;
            combatTimer.AutoReset = true;
            combatTimer.Enabled = true;
        }

        public void UpdateCloseCombat(Object source, ElapsedEventArgs e)
        {
            // ToDo: Update combat (this is currently more a test pak)
            if (ilCombatViewId > 0 && isCombatRunning)
            {
                PacketContent pak = new PacketContent();
                pak.AddUint16(ilCombatViewId, 1);
                pak.AddUShort(3); // 00000011 (Position Updates ON)
                pak.AddByteArray(Store.currentClient.playerInstance.Position.getValue());
                pak.AddHexBytes("030000000000000001020100000000002869C00000000000CF8B42002869400000000000CF8BC2000003006E290000A6009C0FF60E0200000000000000000000000000000000E54E00008B0B002400000000000000000000000000000000350000005200000000100010000000000000000000000000000000000000000000000000010000005864EBD9C000000000004491C000000040AF08EA400000");
                // pak.addHexBytes(
                //     "0300000020da1db84000000000004491c000000060260ac4c00200000000000000000100000020da1db84000000000004491c000000060260ac4c00000");
                Store.currentClient.messageQueue.addObjectMessage(pak.ReturnFinalPacket(), false);
                Store.currentClient.FlushQueue();
            }
        }


        public void ProcessRangeCombatRequest(ref byte[] packet)
        {
            // ToDo: Implement
        }

        public void ProcessLeaveCloseCombat(ref byte[] rpcData)
        {
            // ToDo: Implement leave logic
            if (isCombatRunning)
            {
                combatTimer?.Stop();
            }
        }
    }
}