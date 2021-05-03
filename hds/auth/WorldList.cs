using System;

namespace hds
{
    public class WorldList
    {
        bool playerExist;

        CharacterPack cp;
        WorldsPack wp;

        string username;
        string password;
        int userID;

        int timeCreated;
        byte[] publicModulus;
        byte[] privateExponent;

        public WorldList()
        {
            cp = new CharacterPack();
            wp = new WorldsPack();
            playerExist = false;
            privateExponent = new byte[96];
            publicModulus = new byte[96];
        }

        public CharacterPack getCharPack()
        {
            return cp;
        }

        public WorldsPack getWorldPack()
        {
            return wp;
        }

        public void setUsername(string username)
        {
            this.username = username;
        }

        public void setPassword(string password)
        {
            this.password = password;
        }

        public string getUsername()
        {
            return username;
        }

        public string getPassword()
        {
            return password;
        }

        public bool getExistance()
        {
            return playerExist;
        }

        public void setExistance(bool param)
        {
            this.playerExist = param;
        }

        public void setUserID(int param)
        {
            this.userID = param;
        }

        public int getUserID()
        {
            return userID;
        }

        public void setPublicModulus(byte[] param)
        {
            this.publicModulus = param;
        }

        public byte[] getPublicModulus()
        {
            return publicModulus;
        }


        public void setTimeCreated(int param)
        {
            timeCreated = param;
        }

        public int getTimeCreated()
        {
            return timeCreated;
        }

        public void setPrivateExponent(byte[] param)
        {
            this.privateExponent = param;
        }

        public byte[] getPrivateExponent()
        {
            return privateExponent;
        }
    }
}