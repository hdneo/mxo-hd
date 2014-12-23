using System;

namespace hds
{
    public class CharacterPack
    {

        int numChars;
        int totalChars;

        DynamicArray charData;
        DynamicArray charNames;

        public CharacterPack()
        {
            charData = new DynamicArray();
            charNames = new DynamicArray();

            numChars = 0;
            totalChars = 0;
        }

        public void setTotalChars(int total)
        {
            this.totalChars = total;
        }

        public byte[] getByteContents()
        {
            DynamicArray response = new DynamicArray();
            byte[] totalCharsH = NumericalUtils.uint16ToByteArray((UInt16)totalChars, 1);
            response.append(totalCharsH);
            response.append(charData.getBytes());
            response.append(charNames.getBytes());
            return response.getBytes();
        }

        public void addCharacter(string charName, int charId, int status, int serverId)
        {
            byte[] charNameB = new byte[charName.Length + 3];
            byte[] hexSize = NumericalUtils.uint16ToByteArray((UInt16)(charName.Length + 1), 1);
            charNameB[0] = hexSize[0];
            charNameB[1] = hexSize[1];
            charNameB[charName.Length + 2] = 0x00;
            for (int i = 0; i < charName.Length; i++)
            {
                charNameB[2 + i] = (byte)charName[i];
            }


            byte[] charDataB = new byte[14];
            byte[] charIdB = NumericalUtils.uint32ToByteArray((UInt32)charId, 1);
            charDataB[3] = charIdB[0];
            charDataB[4] = charIdB[1];
            charDataB[5] = charIdB[2];
            charDataB[6] = charIdB[3];
            charDataB[9] = 0x00;
            charDataB[10] = (byte)status;
            charDataB[11] = 0x00;
            charDataB[12] = (byte)serverId;

            numChars++; // Lets say we have done one ;)

            int offset = 0;

            // Use formula to calculate offset here 

            offset = 14 + (14 * (totalChars - numChars));

            if (numChars == 1)
            {
                offset += 0;
            }
            else
            {
                offset += charNames.getSize();
            }

            // End of formula

            byte[] offsetH = NumericalUtils.uint16ToByteArray((UInt16)offset, 1);
            charDataB[1] = offsetH[0];
            charDataB[2] = offsetH[1];

            // After all calculations, append it to the dynamic arrays
            charData.append(charDataB);
            charNames.append(charNameB);

        }

        public int getPackLength()
        {
            // We will count the 2bytes of content size too ;)
            return charNames.getSize() + charData.getSize() + 2;
        }

    }
}