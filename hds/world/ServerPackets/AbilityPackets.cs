using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using hds.shared;
namespace hds
{
    public partial class ServerPackets 
    {
        // Place Methods here for Skills
        public void sendCastAbilityBar(UInt16 abilityID, float timeProcessing)
        {
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_CAST_BAR_ABILITY, 0);
            pak.addUint16(abilityID, 1);
            pak.addHexBytes("00000000000000000000");
            pak.addFloat(timeProcessing, 1);
            Store.currentClient.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }

        public void sendCastAbilityOnEntityId(UInt16 viewId, UInt32 animationId)
        {
            ClientView theView = Store.currentClient.viewMan.getViewById(viewId);

            byte[] updateCount = NumericalUtils.uint16ToByteArrayShort(Store.currentClient.playerData.assignSpawnIdCounter());

            Random rand = new Random();
            ushort randomHealth = (ushort)rand.Next(3, 1800);
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
                    pak.addByteArray(updateCount);
                    break;

                case 599:
                    pak.addByte(0x04);
                    pak.addByte(0x80);
                    pak.addByte(0x80);
                    pak.addByte(0x80);
                    pak.addByte(0xc0);
                    pak.addUint16(randomHealth, 1); // health
                    pak.addByte(0xc0);
                    pak.addUint32(animationId, 1);
                    pak.addByteArray(updateCount);
                    pak.addByte(0x05);
                    pak.addByte(0x00);
                    pak.addByte(0x00);
                    string hexNPC = StringUtils.bytesToString(pak.returnFinalPacket());
                    // Its more a demo - we "one hit" the mob currently so we must update this 
                    lock (WorldSocket.npcs.SyncRoot)
                    {
                        for (int i = 0; i < WorldSocket.npcs.Count; i++)
                        {
                            npc thismob = (npc)WorldSocket.npcs[i];
                            if (thismob.getEntityId() == theView.entityId)
                            {
                                thismob.setIsDead(true);
                                thismob.setIsLootable(true);
                                WorldSocket.npcs[i] = thismob;
                                this.sendNPCDies(theView.ViewID, Store.currentClient, thismob);

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
                    pak.addByteArray(updateCount);
                    break;
            }

            string hex = StringUtils.bytesToString(pak.returnFinalPacket());
            Store.currentClient.messageQueue.addObjectMessage(pak.returnFinalPacket(), false);
            Store.currentClient.flushQueue();
        }

        public void sendHyperSpeed()
        {
            byte[] updateCount = NumericalUtils.uint16ToByteArrayShort(Store.currentClient.playerData.assignSpawnIdCounter());
            PacketContent pak = new PacketContent();
            pak.addUint16(2, 1);
            pak.addHexBytes("0288058c1e6666F63F80903200b056060028");
            pak.addByteArray(updateCount);
            pak.addHexBytes("0200000000");
            Store.currentClient.messageQueue.addObjectMessage(pak.returnFinalPacket(), false);
            Store.currentClient.flushQueue();
        }

        public void sendHyperJumpID(UInt32 possibleJumpID)
        {
            
            PacketContent pak = new PacketContent();
            pak.addUint16((UInt16)RPCResponseHeaders.SERVER_HYPERJUMP_ID, 0);
            pak.addUint32(possibleJumpID,1);
            Store.currentClient.messageQueue.addRpcMessage(pak.returnFinalPacket());
        }

        public void SendHyperJumpUpdate(float xFromPos, float yFromPos, float zFromPos, float xDestPos, float yDestPos, float zDestPos, UInt32 startTime, UInt32 endTime)
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
            pak.addDoubleLtVector3d((double)xDestPos, (double)yDestPos, (double)zDestPos);
            pak.addByteArray(new byte[] { 0x10, 0xff, 0xff });
            pak.addByte(0x00);
            pak.addByte(0x00);
            pak.addByte(0x00);
            Store.currentClient.messageQueue.addObjectMessage(pak.returnFinalPacket(), true);
        }
        

        public void sendAbilitySelfAnimation(UInt16 viewId, UInt16 abilityId, UInt32 animId)
        {
            ClientView theView = Store.currentClient.viewMan.getViewById(viewId);

            PacketContent pak = new PacketContent();
            pak.addUint16(viewId,1);
            pak.addByteArray(new byte[]{ 0x02, 0x80, 0x80, 0x80, 0x90, 0xed, 0x00, 0x30});
            pak.addUint32(animId,0);
            pak.addUintShort(Store.currentClient.playerData.assignSpawnIdCounter());
            Store.currentClient.messageQueue.addObjectMessage(pak.returnFinalPacket(),false);

        }

        public void sendAbilityBuffToEntity()
        {
            
        }
    }
}