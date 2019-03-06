using System;
using hds.shared;

namespace hds
{
    public partial class ServerPackets
    {
        // Place Methods here for Skills
        public void sendCastAbilityBar(UInt16 abilityID, float timeProcessing)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16) RPCResponseHeaders.SERVER_CAST_BAR_ABILITY, 0);
            pak.addUint16(abilityID, 1);
            pak.addHexBytes("00000000000000000000");
            pak.addFloat(timeProcessing, 1);
            Store.currentClient.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }

        public void sendCastAbilityOnEntityId(UInt16 viewId, UInt32 animationId, UInt16 value)
        {
            ClientView theView = Store.currentClient.viewMan.getViewById(viewId);

            Random randomObject = new Random();
            ushort randomHealth = (ushort) randomObject.Next(3, 1800);
            // RSI Health FX "send 02 03 02 00 02 80 80 80 90 ed 00 30 22 0a 00 28 06 00 00;"
            PacketContent pak = new PacketContent();
            if (viewId == 0)
            {
                viewId = 2;
            }
            pak.addUint16(viewId, 1);

            UInt32 theGoID = 12;
            if (theView != null)
            {
                theGoID = theView.GoID;
            }

            switch (theGoID)
            {
                case 12:
                    pak.addByte(0x02);
                    pak.addByte(0x80);
                    pak.addByte(0x80);
                    pak.addByte(0x80);
                    if (viewId == 2)
                    {
                        pak.addByte(0x80);
                        pak.addByte(0xb0);
                    }
                    else
                    {
                        pak.addByte(0x0c);
                    }
                    pak.addUint32(animationId, 1);
                    pak.addUintShort(Store.currentClient.playerData.assignSpawnIdCounter());
                    break;

                case 599:

                    // Its more a demo - we "one hit" the mob currently so we must update this 
                    lock (WorldSocket.npcs.SyncRoot)
                    {
                        for (int i = 0; i < WorldSocket.npcs.Count; i++)
                        {
                            Mob thismob = (Mob) WorldSocket.npcs[i];
                            if (theView != null && thismob.getEntityId() == theView.entityId)
                            {
                                thismob.HitEnemyWithDamage(value, animationId);

                                if (thismob.getHealthC() <= 0)
                                {
                                    thismob.setIsDead(true);
                                    this.SendNpcDies(theView.ViewID, Store.currentClient, thismob);

                                    // We got some Exp for it - currently we just make a simple trick to calculate some exp
                                    // Just take currentLevel * modifier
                                    Random rand = new Random();

                                    UInt32 expModifier = (UInt32)rand.Next(100, 500);
                                    UInt32 expGained = thismob.getLevel() * expModifier;
                                    // Update EXP
                                    new PlayerHandler().IncrementPlayerExp(expGained);
                                    thismob.setIsLootable(true);
                                }
                                WorldSocket.npcs[i] = thismob;
                                
                            }
                        }
                    }
                    break;

                default:
                    pak.addByte(0x02);
                    pak.addByte(0x80);
                    pak.addByte(0x80);
                    pak.addByte(0x80);
                    if (viewId == 2)
                    {
                        pak.addByte(0x80);
                        pak.addByte(0xb0);
                    }
                    else
                    {
                        pak.addByte(0x0c);
                    }
                    pak.addUint32(animationId, 1);
                    pak.addUintShort(Store.currentClient.playerData.assignSpawnIdCounter());
                    break;
            }

            string hex = StringUtils.bytesToString(pak.returnFinalPacket());
            Store.currentClient.messageQueue.addObjectMessage(pak.returnFinalPacket(), false);
            Store.currentClient.FlushQueue();
        }

        public void sendHyperSpeed()
        {
            byte[] updateCount =
                NumericalUtils.uint16ToByteArrayShort(Store.currentClient.playerData.assignSpawnIdCounter());
            PacketContent pak = new PacketContent();
            pak.addUint16(2, 1);
            pak.addHexBytes("0288058c1e6666F63F80903200b056060028");
            pak.addByteArray(updateCount);
            pak.addHexBytes("0200000000");
            Store.currentClient.messageQueue.addObjectMessage(pak.returnFinalPacket(), false);
            Store.currentClient.FlushQueue();
        }

        public void sendHyperJumpID(UInt32 possibleJumpID)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16) RPCResponseHeaders.SERVER_HYPERJUMP_ID, 0);
            pak.addUint32(possibleJumpID, 1);
            Store.currentClient.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }

        public void SendHyperJumpStepUpdate(LtVector3f currentPos, double xDestPos, double yDestPos, double zDestPos,
            float jumpHeight, uint endtime, ushort stepJumpId, uint maybeTimeBasedValue, bool isLastStep = false)
        {

            PacketContent pak = new PacketContent();
            pak.addUint16(2, 1);
            pak.addByte(0x03);
            pak.addByte(0x0d);
            pak.addByte(0x08);
            pak.addByte(0x00);
            pak.addUintShort(Store.currentClient.playerData.getJumpID());
            pak.addFloatLtVector3f(currentPos.x, currentPos.y, currentPos.z);
            pak.addUintShort(stepJumpId);
            pak.addUint32(maybeTimeBasedValue,1);
            // ToDo: Insert 2 missing bytes (or 4 as the next 2 bytes MAYBE wrong)
            pak.addByte(0x8a);
            pak.addByte(0x04);
            pak.addByte(0x80);
            pak.addByte(0x88);
            pak.addByteArray(new byte[]{ 0x00, 0x00, 0x00, 0x00, 0xbc });
            pak.addFloat(jumpHeight, 1);
            pak.addUint16(4, 1);
            pak.addUint32(endtime,1);
            pak.addDoubleLtVector3d(xDestPos, yDestPos, zDestPos);
            pak.addByteArray(new byte[] { 0x80, 0x81, 0x00, 0x02});
            if (isLastStep)
            {
                pak.addByte(0x00);
                Store.currentClient.playerData.isJumping = false;
            }
            else
            {
                pak.addByte(0x01);    
            }
            
            Store.currentClient.messageQueue.addObjectMessage(pak.returnFinalPacket(), false);
            Store.currentClient.FlushQueue();
        }

        public void SendHyperJumpUpdate(float xFromPos, float yFromPos, float zFromPos, float xDestPos, float yDestPos,
            float zDestPos, UInt32 startTime, UInt32 endTime)
        {
            // ToDo: make real repsonse 
            PacketContent pak = new PacketContent();
            pak.addByte(0x02);
            pak.addByte(0x00);
            pak.addByte(0x03);
            pak.addByte(0x09);
            pak.addByte(0x08);
            pak.addByte(0x00);
            pak.addFloatLtVector3f(xFromPos, yFromPos, zFromPos);
            pak.addUint32(startTime, 1);
            pak.addByte(0x80);
            pak.addByte(0x80);
            pak.addByte(0xb8);
            pak.addByte(0x14); // if 0xb8
            pak.addByte(0x00); // if 0xb8
            pak.addUint32(endTime, 1);
            pak.addDoubleLtVector3d((double) xDestPos, (double) yDestPos, (double) zDestPos);
            pak.addByteArray(new byte[] {0x10, 0xff, 0xff});
            pak.addByte(0x00);
            pak.addByte(0x00);
            pak.addByte(0x00);
            Store.currentClient.messageQueue.addObjectMessage(pak.returnFinalPacket(), true);
        }


        public void sendAbilitySelfAnimation(UInt16 viewId, UInt16 abilityId, UInt32 animId)
        {
            ClientView theView = Store.currentClient.viewMan.getViewById(viewId);

            PacketContent pak = new PacketContent();
            pak.addUint16(viewId, 1);
            pak.addByteArray(new byte[] {0x02, 0x80, 0x80, 0x80, 0x90, 0xed, 0x00, 0x30});
            pak.addUint32(animId, 0);
            pak.addUintShort(Store.currentClient.playerData.assignSpawnIdCounter());
            Store.currentClient.messageQueue.addObjectMessage(pak.returnFinalPacket(), false);
        }

        public void sendAbilityBuffToEntity()
        {
        }
    }
}