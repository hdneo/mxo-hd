using System;
using System.Collections.Generic;
using hds.shared;

namespace hds
{
    public partial class ServerPackets
    {
        public void SendUpgradeAbilityLevel(UInt16 ability, UInt16 currentLevel)
        {
            PacketContent pak = new PacketContent();
            pak.AddUint16((UInt16) RPCResponseHeaders.SERVER_PLAYER_UPGRADE_ABILITY, 0);
            pak.AddUint16(ability, 1);
            pak.AddUint16(currentLevel, 1);
            pak.AddUint32(0, 1); // Needs more Research - found only 4 bytes zeros 
            Client.messageQueue.addRpcMessage(pak.ReturnFinalPacket());
        }

        // Place Methods here for Skills
        public void SendCastAbilityBar(UInt16 abilityID, float timeProcessing)
        {
            PacketContent pak = new PacketContent();
            pak.AddUint16((UInt16) RPCResponseHeaders.SERVER_CAST_BAR_ABILITY, 0);
            pak.AddUint16(abilityID, 1);
            pak.AddHexBytes("00000000000000000000");
            pak.AddFloat(timeProcessing, 1);
            Client.messageQueue.addRpcMessage(pak.ReturnFinalPacket());
        }

        public void SendCastAbilityOnEntityId(UInt16 viewId, UInt32 animationId, UInt16 value)
        {
            ClientView theView = Client.viewMan.getViewById(viewId);

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
            myselfStateData.AddUint16(2, 1);
            PacketContent viewStateOtherData = new PacketContent();

            switch (theGoID)
            {
                case 599:

                    // Its more a demo - we "one hit" the mob currently so we must update this 
                    lock (WorldServer.mobs.SyncRoot)
                    {
                        for (int i = 0; i < WorldServer.mobs.Count; i++)
                        {
                            Mob thismob = (Mob) WorldServer.mobs[i];
                            if (theView != null && thismob.getEntityId() == theView.entityId)
                            {
                                thismob.HitEnemyWithDamage(value, animationId);

                                if (thismob.getHealthC() <= 0)
                                {
                                    thismob.setIsDead(true);
                                    SendNpcDies(theView.ViewID, Client, thismob);

                                    // We got some Exp for it - currently we just make a simple trick to calculate some exp
                                    // Just take currentLevel * modifier
                                    Random rand = new Random();

                                    UInt32 expModifier = (UInt32) rand.Next(100, 500);
                                    UInt32 expGained = thismob.getLevel() * expModifier;
                                    // Update EXP
                                    new PlayerHandler(Client).IncrementPlayerExp(expGained);
                                    thismob.setIsLootable(true);
                                }

                                WorldServer.mobs[i] = thismob;
                            }
                        }
                    }

                    break;

                default:
                    List<Attribute> updateAttributes = new List<Attribute>();
                    Client.playerInstance.EffectID.enable();
                    Client.playerInstance.EffectCounter.enable();

                    Client.playerInstance.EffectID.setValue(animationId);
                    uint effectCounter = 0;

                    effectCounter = (uint) Client.playerInstance.EffectCounter.getValue()[0] + 1;
                    Client.playerInstance.EffectCounter.setValue(effectCounter);

                    updateAttributes.Add(Client.playerInstance.EffectID);
                    updateAttributes.Add(Client.playerInstance.EffectCounter);

                    viewStateOtherData.AddByteArray(
                        Client.playerInstance.GetUpdateAttributes(updateAttributes));
                    myselfStateData.AddByteArray(
                        Client.playerInstance.GetSelfUpdateAttributes(updateAttributes, false));

                    String hexViewStateOther = StringUtils.bytesToString_NS(viewStateOtherData.ReturnFinalPacket());
                    String hexMyselfStateData = StringUtils.bytesToString_NS(myselfStateData.ReturnFinalPacket());

                    if (viewId == 2 || viewId == 0)
                    {
                        Output.WriteDebugLog("View ID Ability SelfState for View ID 2 from " +
                                             Client.playerData.getCharID() + " : " + hexMyselfStateData);
                        Output.WriteDebugLog("Update Ability FX on OtherState Views from " +
                                             Client.playerData.getCharID() + " : " + hexMyselfStateData);
                        Client.messageQueue.addObjectMessage(myselfStateData.ReturnFinalPacket(), false);
                        Store.world.SendViewPacketToAllPlayers(viewStateOtherData.ReturnFinalPacket(),
                            Client.playerData.getCharID(),
                            NumericalUtils.ByteArrayToUint16(Client.playerInstance.GetGoid(), 1),
                            Client.playerData.getEntityId());
                    }
                    else
                    {
                        // This should show the FX only on otherViews but on the castTarget it should be shown the SelfViewUpdate
                        // Send selfView Packet to the Target
                        Output.WriteDebugLog("View ID Ability SelfState for View ID " + viewId + " from " +
                                             Client.playerData.getCharID() + " : " + hexMyselfStateData);
                        Store.world.SendSelfViewUpdateToTarget(myselfStateData, viewId, Client);
                        Store.world.SendViewPacketToAllPlayers(viewStateOtherData.ReturnFinalPacket(),
                            Client.playerData.getCharID(),
                            NumericalUtils.ByteArrayToUint16(Client.playerInstance.GetGoid(), 1),
                            Client.playerData.getEntityId());
                    }

                    break;
            }
        }

        public void SendHyperSpeed()
        {
            byte[] updateCount =
                NumericalUtils.uint16ToByteArrayShort(Client.playerData.assignSpawnIdCounter());
            PacketContent pak = new PacketContent();
            pak.AddUint16(2, 1);
            pak.AddHexBytes("0288058c1e6666F63F80903200b056060028");
            pak.AddByteArray(updateCount);
            pak.AddHexBytes("0200000000");
            Client.messageQueue.addObjectMessage(pak.ReturnFinalPacket(), false);
            Client.FlushQueue();
        }

        public void SendHyperJumpID(UInt32 possibleJumpID)
        {
            PacketContent pak = new PacketContent();
            pak.AddUint16((UInt16) RPCResponseHeaders.SERVER_HYPERJUMP_ID, 0);
            pak.AddUint32(possibleJumpID, 1);
            Client.messageQueue.addRpcMessage(pak.ReturnFinalPacket());
        }

        public void SendCodeWriterWindow()
        {
            // Fist i dont know what it is
            PacketContent coderFirstPak = new PacketContent();
            coderFirstPak.AddUint16((UInt16) RPCResponseHeaders.SERVER_CODER_ATTRIBUTE_UNKNOWN, 0);
            coderFirstPak.AddHexBytes("05ca03000000000001");
            
            // Maybe we need to send attribute 
            PacketContent bonusRpc = new PacketContent();
            bonusRpc.AddHexBytes("80bc");
            bonusRpc.AddHexBytes("1d00b80a00ca030000c803000000000000000000");
            
            // should be the window only
            PacketContent coderWindowPak = new PacketContent();
            coderWindowPak.AddUint16((UInt16) RPCResponseHeaders.SERVER_CODER_UNKNOWN, 0);
            coderWindowPak.AddHexBytes("00009e079e070301322000");
            
            Client.messageQueue.addRpcMessage(coderFirstPak.ReturnFinalPacket());
            Client.messageQueue.addRpcMessage(bonusRpc.ReturnFinalPacket());
            Client.messageQueue.addRpcMessage(coderWindowPak.ReturnFinalPacket());
            Client.FlushQueue();
        }

        public void SendHyperJumpStepUpdate(LtVector3f currentPos, double xDestPos, double yDestPos, double zDestPos,
            float jumpHeight, uint endtime, ushort stepJumpId, uint maybeTimeBasedValue, bool isLastStep = false)
        {
            PacketContent pak = new PacketContent();
            pak.AddUint16(2, 1);
            pak.AddByte(0x03);
            pak.AddByte(0x0d);
            pak.AddByte(0x08);
            pak.AddByte(0x00);
            pak.AddUShort(Client.playerData.getJumpID());
            pak.AddFloatLtVector3f(currentPos.x, currentPos.y, currentPos.z);
            pak.AddUShort(stepJumpId);
            pak.AddUint32(maybeTimeBasedValue, 1);
            // ToDo: Insert 2 missing bytes (or 4 as the next 2 bytes MAYBE wrong)
            pak.AddByte(0x8a);
            pak.AddByte(0x04);
            pak.AddByte(0x80);
            pak.AddByte(0x88);
            pak.AddByteArray(new byte[] {0x00, 0x00, 0x00, 0x00, 0xbc});
            pak.AddFloat(jumpHeight, 1);
            pak.AddUint16(4, 1);
            pak.AddUint32(endtime, 1);
            pak.AddDoubleLtVector3d(xDestPos, yDestPos, zDestPos);
            pak.AddByteArray(new byte[] {0x80, 0x81, 0x00, 0x02});
            if (isLastStep)
            {
                pak.AddByte(0x00);
                Client.playerData.isJumping = false;
            }
            else
            {
                pak.AddByte(0x01);
            }

            Client.messageQueue.addObjectMessage(pak.ReturnFinalPacket(), false);
            Client.FlushQueue();
        }

        public void SendHyperJumpWithOneStep(float xDestPos,
            float yDestPos,
            float zDestPos, UInt32 peakHeight, UInt32 endTime)
        {
            
            List<Attribute> updateAttributes = new List<Attribute>();
            Client.playerInstance.JumpPeakHeight.enable();
            Client.playerInstance.JumpFlags.enable();
            Client.playerInstance.JumpDestination.enable();
            Client.playerInstance.JumpEndTime.enable();
            Client.playerInstance.InnerStrengthAvailable.enable();

            Client.playerInstance.JumpPeakHeight.setValue(peakHeight);
            Client.playerInstance.JumpFlags.setValue(4);
            Client.playerInstance.JumpEndTime.setValue(endTime);
            Client.playerInstance.InnerStrengthAvailable.setValue(284);
            Client.playerInstance.JumpDestination.setValue(NumericalUtils.doublesToLtVector3d(xDestPos, yDestPos, zDestPos));
            
            updateAttributes.Add(Client.playerInstance.JumpPeakHeight);
            updateAttributes.Add(Client.playerInstance.JumpFlags);
            updateAttributes.Add(Client.playerInstance.JumpDestination);
            updateAttributes.Add(Client.playerInstance.JumpEndTime);
            updateAttributes.Add(Client.playerInstance.InnerStrengthAvailable);

            PacketContent myselfStateData = new PacketContent();
            myselfStateData.AddByte(0x03);
            myselfStateData.AddByte(0x01);
            myselfStateData.AddUint16(8,1);
            myselfStateData.AddByteArray(
                Client.playerInstance.GetSelfUpdateAttributes(updateAttributes, false));

            Store.world.SendSelfViewUpdate(myselfStateData, 2, Client, true);
            
            // PacketContent pak = new PacketContent();
            // pak.AddUint16(2, 1);
            // pak.AddByteArray(new byte[] {0x03, 0x01, 0x08, 0x00, 0x80, 0x80, 0xbc});
            // pak.AddUint32(peakHeight, 1);
            // pak.AddByteArray(new byte[] {0x04, 0x00, 0x33, 0x4a, 0x31, 0x05});
            // pak.AddDoubleLtVector3d(xDestPos, yDestPos, zDestPos);
            // pak.AddByteArray(new byte[] {0x10, 0x1c, 0x01});
            // Client.messageQueue.addObjectMessage(pak.ReturnFinalPacket(), true);
        }

        public void SendHyperJumpUpdate(float xFromPos, float yFromPos, float zFromPos, float xDestPos, float yDestPos,
            float zDestPos, UInt32 startTime, UInt32 endTime)
        {
            // ToDo: make real repsonse 
            PacketContent pak = new PacketContent();
            pak.AddByte(0x02);
            pak.AddByte(0x00);
            pak.AddByte(0x03);
            pak.AddByte(0x09);
            pak.AddByte(0x08);
            pak.AddByte(0x00);
            pak.AddFloatLtVector3f(xFromPos, yFromPos, zFromPos);
            pak.AddUint32(startTime, 1);
            pak.AddByte(0x80);
            pak.AddByte(0x80);
            pak.AddByte(0xb8);
            pak.AddByte(0x14); // if 0xb8
            pak.AddByte(0x00); // if 0xb8
            pak.AddUint32(endTime, 1);
            pak.AddDoubleLtVector3d((double) xDestPos, (double) yDestPos, (double) zDestPos);
            pak.AddByteArray(new byte[] {0x10, 0xff, 0xff});
            pak.AddByte(0x00);
            pak.AddByte(0x00);
            pak.AddByte(0x00);
            Client.messageQueue.addObjectMessage(pak.ReturnFinalPacket(), true);
        }


        public void SendAbilitySelfAnimation(UInt16 viewId, UInt16 abilityId, UInt32 animId)
        {
            ClientView theView = Client.viewMan.getViewById(viewId);

            PacketContent pak = new PacketContent();
            pak.AddUint16(viewId, 1);
            pak.AddByteArray(new byte[] {0x02, 0x80, 0x80, 0x80, 0x90, 0xed, 0x00, 0x30});
            pak.AddUint32(animId, 0);
            pak.AddUShort(Client.playerData.assignSpawnIdCounter());
            Client.messageQueue.addObjectMessage(pak.ReturnFinalPacket(), false);
        }

        public void sendAbilityBuffToEntity()
        {
        }
    }
}