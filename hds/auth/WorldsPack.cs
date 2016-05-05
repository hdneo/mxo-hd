
using System;

namespace hds
{

    public class WorldsPack
    {

        DynamicArray worlds;
        int numWorlds;

        public WorldsPack()
        {
            worlds = new DynamicArray();

        }

        public byte[] getByteContents()
        {
            DynamicArray response = new DynamicArray();
            byte[] numWorldsH = NumericalUtils.uint16ToByteArray((UInt16)numWorlds, 1);
            response.append(numWorldsH);
            response.append(worlds.getBytes());
            return response.getBytes();
        }

        public int getTotalSize()
        {
            // We also add the 2 length bytes
            return worlds.getSize() + 2;
        }

        public void addWorld(string worldName, int worldId, int worldStatus, int worldStyle, int worldPopulation)
        {
            byte[] world = new byte[32];
            world[0] = 0x00;
            world[1] = (byte)worldId;

            for (int i = 0; i < worldName.Length; i++)
            {
                world[3 + i] = (byte)worldName[i];
            }


            world[23] = (byte)worldStatus;
            world[24] = (byte)worldStyle;


            world[25] = 0xd9;
            world[26] = 0x21;
            world[27] = 0x07;
            world[28] = 0x00;
            world[29] = 0x01;
            world[30] = 0x00;

            world[31] = (byte)worldPopulation;

            worlds.append(world);
            numWorlds++;
        }
    }
}