
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

        public void AddWorld(string worldName, UInt16 worldId, ushort worldStatus, ushort worldStyle, ushort worldPopulation)
        {
            PacketContent pak = new PacketContent();
            pak.AddUShort(0);
            pak.AddUint16(worldId,1);
            pak.AddStringWithFixedSized(worldName,19);
            pak.AddUShort(worldStatus);
            pak.AddUShort(worldStyle);
            pak.AddByte(0xd9);
            pak.AddByte(0x21);
            pak.AddByte(0x07);
            pak.AddByte(0x00);
            pak.AddByte(0x01);
            pak.AddByte(0x00);
            
            pak.AddUShort(worldPopulation);

            worlds.append(pak.ReturnFinalPacket());
            numWorlds++;
        }
    }
}