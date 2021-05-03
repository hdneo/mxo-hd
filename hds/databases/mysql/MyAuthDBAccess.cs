using System;
using System.Data;
using System.Linq;
using hds.databases.Entities;
using hds.databases.interfaces;
using hds.shared;

namespace hds.databases
{
    public class MyAuthDBAccess : IAuthDBHandler
    {
        private MatrixDbContext dbContext;

        public MyAuthDBAccess()
        {
            var config = Store.config;
            dbContext = new MatrixDbContext(config.dbParams);
        }


        public bool FetchWorldList(ref WorldList wl)
        {
            // Doesnt exist by default
            wl.setExistance(false);

            var worldList = wl;

            try
            {
                var player = dbContext.Users
                    .Where(u => u.Username == worldList.getUsername())
                    .Single(u => u.Passwordmd5 == worldList.getPassword());

                byte[] publicModulus = new byte[96];
                byte[] privateExponent = new byte[96];

                ArrayUtils.copy(player.PublicModulus, 0, publicModulus, 0, 96);
                ArrayUtils.copy(player.PrivateExponent, 0, privateExponent, 0, 96);
                var dateTimeCreated = new DateTimeOffset(player.TimeCreated).ToUniversalTime().ToUnixTimeMilliseconds();

                wl.setPublicModulus(publicModulus);
                wl.setPrivateExponent(privateExponent);
                wl.setExistance(true);
                wl.setUserID((int) player.UserId);
                wl.setTimeCreated((int) dateTimeCreated);
            }
            catch (Exception ex)
            {
                String msg = "Player not found on DB with #" + wl.getUsername() + "# and #" + wl.getPassword() + "#";
                Output.WriteLine(msg);
                return false;
            }

            // Prepare to read characters
            var userId = wl.getUserID();
            var characters = dbContext.Characters.Where(c => c.UserId == userId).Where(c => c.IsDeleted == 0)
                .ToList();

            wl.getCharPack().setTotalChars(characters.Count);

            foreach (var character in characters)
            {
                wl.getCharPack().addCharacter(character.Handle, (int) character.CharId, character.Status,
                    character.WorldId);
            }

            var worlds = dbContext.Worlds.OrderBy(w => w.WorldId).ToList();
            foreach (var world in worlds)
            {
                wl.getWorldPack().AddWorld(world.Name, world.WorldId, world.Status, world.Type,
                    (UInt16) world.NumPlayers);
            }

            return true;
        }
    }
}