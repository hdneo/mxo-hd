using System;
using System.Collections.Generic;
using hds.shared;

namespace hds
{
    public partial class ServerPackets
    {
        // Place Methods here for Skills
        public void SendCastAbilityBar(UInt16 abilityID, float timeProcessing)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16) RPCResponseHeaders.SERVER_CAST_BAR_ABILITY, 0);
            pak.addUint16(abilityID, 1);
            pak.addHexBytes("00000000000000000000");
            pak.addFloat(timeProcessing, 1);
            Store.currentClient.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }

        public void SendCastAbilityOnEntityId(UInt16 viewId, UInt32 animationId, UInt16 value)
        {
            ClientView theView = Store.currentClient.viewMan.getViewById(viewId);
            
            if (viewId == 0)
            {
                viewId = 2;
            }

            UInt32 theGoID = 0;
            if (theView != null)
            {
                theGoID = theView.GoID;
            }

            PacketContent myselfStateData = new PacketContent();
            myselfStateData.addUint16(2, 1);
            PacketContent viewStateOtherData = new PacketContent();

            switch (theGoID)
            {
                case 599:

                    // Its more a demo - we "one hit" the mob currently so we must update this 
                    lock (WorldSocket.mobs.SyncRoot)
                    {
                        for (int i = 0; i < WorldSocket.mobs.Count; i++)
                        {
                            Mob thismob = (Mob) WorldSocket.mobs[i];
                            if (theView != null && thismob.getEntityId() == theView.entityId)
                            {
                                thismob.HitEnemyWithDamage(value, animationId);

                                if (thismob.getHealthC() <= 0)
                                {
                                    thismob.setIsDead(true);
                                    SendNpcDies(theView.ViewID, Store.currentClient, thismob);

                                    // We got some Exp for it - currently we just make a simple trick to calculate some exp
                                    // Just take currentLevel * modifier
                                    Random rand = new Random();

                                    UInt32 expModifier = (UInt32) rand.Next(100, 500);
                                    UInt32 expGained = thismob.getLevel() * expModifier;
                                    // Update EXP
                                    new PlayerHandler().IncrementPlayerExp(expGained);
                                    thismob.setIsLootable(true);
                                }

                                WorldSocket.mobs[i] = thismob;
                            }
                        }
                    }

                    break;

                default:
                    List<Attribute> updateAttributes = new List<Attribute>();
                    Store.currentClient.playerInstance.EffectID.enable();
                    Store.currentClient.playerInstance.EffectCounter.enable();

                    Store.currentClient.playerInstance.EffectID.setValue(animationId);
                    uint effectCounter = 0;

                    effectCounter = (uint) Store.currentClient.playerInstance.EffectCounter.getValue()[0] + 1;
                    Store.currentClient.playerInstance.EffectCounter.setValue(effectCounter);
                    
                    updateAttributes.Add(Store.currentClient.playerInstance.EffectID);
                    updateAttributes.Add(Store.currentClient.playerInstance.EffectCounter);

                    viewStateOtherData.addByteArray(
                        Store.currentClient.playerInstance.GetUpdateAttributes(updateAttributes));
                    myselfStateData.addByteArray(
                        Store.currentClient.playerInstance.GetSelfUpdateAttributes(updateAttributes));

                    String hexViewStateOther = StringUtils.bytesToString_NS(viewStateOtherData.returnFinalPacket());
                    String hexMyselfStateData = StringUtils.bytesToString_NS(myselfStateData.returnFinalPacket());
                    
                    if (viewId == 2 || viewId == 0)
                    {
                        Output.WriteDebugLog("View ID Ability SelfState for View ID 2 from " + Store.currentClient.playerData.getCharID() + " : " + hexMyselfStateData);
                        Output.WriteDebugLog("Update Ability FX on OtherState Views from " + Store.currentClient.playerData.getCharID() + " : " + hexMyselfStateData);
                        Store.currentClient.messageQueue.addObjectMessage(myselfStateData.returnFinalPacket(), false);
                        Store.world.SendViewPacketToAllPlayers(viewStateOtherData.returnFinalPacket(), Store.currentClient.playerData.getCharID(), NumericalUtils.ByteArrayToUint16(Store.currentClient.playerInstance.GetGoid(), 1), Store.currentClient.playerData.getEntityId());
                    }
                    else
                    {
                        // This should show the FX only on otherViews but on the castTarget it should be shown the SelfViewUpdate
                        // Send selfView Packet to the Target
                        Output.WriteDebugLog("View ID Ability SelfState for View ID " + viewId + " from " + Store.currentClient.playerData.getCharID() + " : " + hexMyselfStateData);
                        Store.world.SendSelfViewUpdateToTarget(myselfStateData, viewId, Store.currentClient);
                        Store.world.SendViewPacketToAllPlayers(viewStateOtherData.returnFinalPacket(), Store.currentClient.playerData.getCharID(), NumericalUtils.ByteArrayToUint16(Store.currentClient.playerInstance.GetGoid(), 1), Store.currentClient.playerData.getEntityId());
                    }

                    break;
            }
            
        }

        public void SendHyperSpeed()
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
            pak.addUint32(maybeTimeBasedValue, 1);
            // ToDo: Insert 2 missing bytes (or 4 as the next 2 bytes MAYBE wrong)
            pak.addByte(0x8a);
            pak.addByte(0x04);
            pak.addByte(0x80);
            pak.addByte(0x88);
            pak.addByteArray(new byte[] {0x00, 0x00, 0x00, 0x00, 0xbc});
            pak.addFloat(jumpHeight, 1);
            pak.addUint16(4, 1);
            pak.addUint32(endtime, 1);
            pak.addDoubleLtVector3d(xDestPos, yDestPos, zDestPos);
            pak.addByteArray(new byte[] {0x80, 0x81, 0x00, 0x02});
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


        public void SendAbilitySelfAnimation(UInt16 viewId, UInt16 abilityId, UInt32 animId)
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