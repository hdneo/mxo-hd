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
            Store.currentClient.playerData.spawnViewUpdateCounter++;
            byte[] updateCount = NumericalUtils.uint16ToByteArrayShort(Store.currentClient.playerData.spawnViewUpdateCounter);

            // RSI Health FX "send 02 03 02 00 02 80 80 80 90 ed 00 30 22 0a 00 28 06 00 00;"
            PacketContent pak = new PacketContent();

            pak.addUint16(viewId, 1);
            pak.addByte(0x02);
            pak.addByte(0x80);
            pak.addByte(0x80);
            pak.addByte(0x80);
            pak.addByte(0x90);
            pak.addByte(0xed);
            pak.addByte(0x00);
            pak.addByte(0x30);
            pak.addUint32(animationId,1);
            pak.addByteArray(updateCount);


            Store.currentClient.messageQueue.addObjectMessage(pak.returnFinalPacket(), false);
            Store.currentClient.flushQueue();
        }

        public void sendAbilityBuffToEntity()
        {

        }
    }
}