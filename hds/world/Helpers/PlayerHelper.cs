using System;
using System.Collections;
using System.Text;
using hds.shared;
using System.Collections.Generic;

namespace hds
{
    /*
     * The General Player Helper
     */
    class PlayerHelper
    {
        public void processIncreaseCash(UInt16 amount, UInt16 type)
        {
            // send 02 04 01 00 16 01 0a 80 e4 ff 00 00 00 02 00 00 00;
            byte[] header = {0x80, 0xe4};
            long newCash = Store.currentClient.playerData.getInfo() + (long) amount;
            Store.currentClient.playerData.setInfo(newCash);

            Store.dbManager.WorldDbHandler.SavePlayer(Store.currentClient);

            DynamicArray din = new DynamicArray();

            din.append(header);
            din.append(NumericalUtils.uint32ToByteArray((UInt32) newCash, 1));
            din.append(NumericalUtils.uint16ToByteArray(type, 1));
            din.append(0x00);
            din.append(0x00);
            Store.currentClient.messageQueue.addRpcMessage(din.getBytes());
        }


        public void processDecreaseCash(UInt16 amount, UInt16 type)
        {
            // send 02 04 01 00 16 01 0a 80 e4 ff 00 00 00 02 00 00 00;
            byte[] header = {0x80, 0xe4};
            long newCash = Store.currentClient.playerData.getInfo() - (long) amount;
            Store.currentClient.playerData.setInfo(newCash);

            Store.dbManager.WorldDbHandler.SavePlayer(Store.currentClient);

            DynamicArray din = new DynamicArray();
            din.append(header);
            din.append(NumericalUtils.uint32ToByteArray((UInt32) newCash, 1));
            din.append(NumericalUtils.uint16ToByteArray(type, 1));
            din.append(0x00);
            din.append(0x00);
            Store.currentClient.messageQueue.addRpcMessage(din.getBytes());
        }


        public void processLoadAbility(ref byte[] packet)
        {
            // read the values from the packet
            PacketReader reader = new PacketReader(packet);
            UInt32 staticObjectID = reader.readUInt32(1);
            UInt16 unloadFlag = reader.readUInt16(1);
            UInt16 loadFlag = reader.readUInt16(1);
            UInt16 countAbilities = reader.readUInt16(1);

            int pointer = 11; // Start at index 11
            List<UInt16> abilitySlots = new List<UInt16>();

            for (int i = 1; i <= countAbilities; i++)
            {
                // This must be looped 
                byte[] slotByteID = new byte[2];
                ArrayUtils.copyTo(packet, pointer, slotByteID, 0, 2);
                pointer = pointer + 2;

                byte[] abilityByteID = new byte[2];
                ArrayUtils.copyTo(packet, pointer, abilityByteID, 0, 2);
                pointer = pointer + 4;

                byte[] abilityByteLevel = new byte[2];
                ArrayUtils.copyTo(packet, pointer, abilityByteLevel, 0, 2);
                pointer = pointer + 3;


                UInt16 slotID = NumericalUtils.ByteArrayToUint16(slotByteID, 1);
                UInt16 AbilityID = NumericalUtils.ByteArrayToUint16(abilityByteID, 1);
                UInt16 AbilityLevel = NumericalUtils.ByteArrayToUint16(abilityByteLevel, 1);

                PacketContent pak = new PacketContent();
                if (unloadFlag > 0)
                {
                    pak.addUint16((UInt16) RPCResponseHeaders.SERVER_ABILITY_UNLOAD, 0);
                    pak.addByteArray(abilityByteID);
                }
                else
                {
                    pak.addUint16((UInt16) RPCResponseHeaders.SERVER_ABILITY_LOAD, 0);
                    pak.addByteArray(abilityByteID);
                    pak.addByteArray(abilityByteLevel);
                    pak.addByteArray(slotByteID);
                }
                abilitySlots.Add(slotID);
                Store.currentClient.messageQueue.addRpcMessage(pak.returnFinalPacket());
            }


            if (unloadFlag > 0)
            {
                Store.dbManager.WorldDbHandler.UpdateAbilityLoadOut(abilitySlots, 0);
            }
            else
            {
                Store.dbManager.WorldDbHandler.UpdateAbilityLoadOut(abilitySlots, 1);
            }
        }

        public void processTargetChange(ref byte[] rpcData, WorldClient currentClient)
        {
            UInt16 viewId = NumericalUtils.ByteArrayToUint16(new byte[] {rpcData[0], rpcData[1]}, 1);
            ushort spawnId = rpcData[2];
            // ToDo: add this to the ClientData 
            currentClient.playerData.currentSelectedTargetViewId = viewId;
            currentClient.playerData.currentSelectedTargetSpawnId = spawnId;
            ServerPackets pak = new ServerPackets();
            pak.sendSystemChatMessage(Store.currentClient,
                "TARGET CHANGE For ViewID " + viewId.ToString() + " AND SPAWN ID : " + spawnId.ToString(), "MODAL");
        }

        public void processUpdateExp()
        {
            Random rand = new Random();
            UInt32 expval = (UInt32) rand.Next(1000, 200000);
            ArrayList content = new ArrayList();

            // ToDo  : Save new EXP Value in the Database and update mpm exp
            // ToDo2 : Check if exp events are running to multiple the EXP 
            // The Animation
            DynamicArray expanim = new DynamicArray();
            expanim.append(0x80);
            expanim.append(0xe6);
            expanim.append(NumericalUtils.uint32ToByteArray(expval, 1));
            expanim.append(0x01); // Gain Type 
            expanim.append(StringUtils.hexStringToBytes("000000"));
            Store.currentClient.messageQueue.addRpcMessage(expanim.getBytes());

            // The BAR
            DynamicArray expbar = new DynamicArray();
            expbar.append(0x80);
            expbar.append(0xe5);
            expbar.append(NumericalUtils.uint32ToByteArray(expval, 1));
            expbar.append(0x01); // Gain Type 
            expbar.append(StringUtils.hexStringToBytes("000000"));
            Store.currentClient.messageQueue.addRpcMessage(expbar.getBytes());
        }


        public void processSelfUpdateHealth(UInt16 viewId, UInt16 healthC)
        {
            // ToDo: proove to remove
            DynamicArray din = new DynamicArray();
            din.append(0x03);
            din.append(NumericalUtils.uint16ToByteArray(viewId, 1));
            din.append(0x02);
            din.append(0x80);
            din.append(0x80);
            din.append(0x80);
            din.append(0x40);
            din.append(NumericalUtils.uint16ToByteArray(healthC, 1));
            din.append(0x00);
        }


        // Shows the Animation of a target Player 
        public void processFXfromPlayer(UInt16 viewID, byte[] animation)
        {
            // ToDo: proove to remove
            Random rand = new Random();
            ushort updateViewCounter = (ushort) rand.Next(3, 200);
            byte[] updateCount = NumericalUtils.uint16ToByteArrayShort(updateViewCounter);

            DynamicArray din = new DynamicArray();
            din.append(NumericalUtils.uint16ToByteArray(viewID, 1));
            din.append(0x00);
            din.append(0x80);
            din.append(0x80);
            din.append(0x80);
            din.append(0x0c);
            din.append(animation); // uint32 anim ID
            din.append(updateCount);
            din.append(0x00);
            din.append(0x00);

            Store.currentClient.messageQueue.addObjectMessage(din.getBytes(), false);
        }

        // ToDo: Move it to player Packets and make a ?moa command for it
        public void processChangeMoaRSI(byte[] rsi)
        {
            // ToDo: proove to remove
            DynamicArray din = new DynamicArray();
            din.append(0x03);
            din.append(0x02);
            din.append(0x00);
            din.append(StringUtils.hexStringToBytes("028100808080b052c7de12ab04"));
            din.append(rsi);
            din.append(0x41);
            din.append(0x00);
        }


        /// <summary>
        /// Helper Methods 
        /// </summary>
        public void savePlayerInfo(WorldClient client)
        {
            Store.dbManager.WorldDbHandler.SavePlayer(client);
        }

        public byte[] teleport(int x, int y, int z)
        {
            return PacketsUtils.createTeleportPacket(x, y, z);
        }


        public byte[] changeRsi(string part, int value)
        {
            string[] keys =
            {
                "sex", "body", "hat", "face", "shirt", "coat", "pants", "shoes", "gloves", "glasses", "hair",
                "facialdetail", "shirtcolor", "pantscolor", "coatcolor", "shoecolor", "glassescolor", "haircolor",
                "skintone", "tattoo", "facialdetailcolor", "leggins"
            };

            int pos = -1;

            for (int i = 0; i < keys.Length; i++)
            {
                if (part.Equals(keys[i].ToLower()))
                {
                    pos = i;
                    break;
                }
            }

            if (pos >= 0)
            {
                int[] current = Store.currentClient.playerData.getRsiValues();
                current[pos] = value;
                Store.currentClient.playerData.setRsiValues(current);
                byte[] rsiData = PacketsUtils.getRSIBytes(current);

                DynamicArray din = new DynamicArray();
                byte[] rsiChangeHeader = {0x02, 0x00, 0x02, 0x80, 0x89};
                din.append(rsiChangeHeader);
                din.append(rsiData);

                return din.getBytes();
            }
            else
            {
                throw new FormatException("body part or clothes not found");
            }
        }
    }
}