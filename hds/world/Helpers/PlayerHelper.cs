using System;
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

        /// <summary>
        /// Helper Methods 
        /// </summary>
        public void SavePlayerInfo(WorldClient client)
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