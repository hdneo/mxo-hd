using System;
using System.IO;
using hds.databases;
using hds.shared;
using MySql.Data.MySqlClient;
using Xunit;

namespace hds.Tests.Integration
{
    /// <summary>
    /// Read-only checks against the real database. Only run when HDS_DB_TESTS=1 and a Config.xml is found
    /// (HDS_CONFIG=path or hds/bin/Debug/net8.0/Config.xml).
    /// </summary>
    public class DatabaseTests
    {
        private static string ConfigPath()
        {
            var fromEnv = Environment.GetEnvironmentVariable("HDS_CONFIG");
            if (!string.IsNullOrEmpty(fromEnv)) return fromEnv;
            try
            {
                return TestData.FindRepoFile(Path.Combine("hds", "bin", "Debug", "net8.0", "Config.xml"));
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        private static bool Enabled(out string configPath)
        {
            configPath = ConfigPath();
            return Environment.GetEnvironmentVariable("HDS_DB_TESTS") == "1" && configPath != null;
        }

        [Fact]
        public void AuthUserQueryWorks()
        {
            if (!Enabled(out var configPath)) return;
            Store.config = new ServerConfig(configPath);
            Store.config.LoadDbParams();
            using var context = new hds.databases.Entities.MatrixDbContext(Store.config.dbParams);
            // same kind of query as AuthHandler.ProcessHandleAuthChallenge_Response
            var users = System.Linq.Enumerable.ToList(System.Linq.Queryable.Take(context.Users, 5));
            Assert.NotEmpty(users);
        }

        [Fact]
        public void FetchWorldListForExistingUser()
        {
            if (!Enabled(out var configPath)) return;
            Store.config = new ServerConfig(configPath);
            Store.config.LoadDbParams();
            hds.databases.Entities.User user;
            using (var context = new hds.databases.Entities.MatrixDbContext(Store.config.dbParams))
            {
                user = System.Linq.Enumerable.First(context.Users);
            }

            var worldList = new WorldList();
            worldList.setUsername(user.Username);
            worldList.setPassword(user.Passwordmd5);
            Assert.True(new MyAuthDBAccess().FetchWorldList(ref worldList));
            Assert.True(worldList.getWorldPack().getTotalSize() > 0);
        }

        [Fact]
        public void LoadsAbilitiesOfExistingCharacters()
        {
            if (!Enabled(out var configPath)) return;

            Store.config = new ServerConfig(configPath);
            Store.config.LoadDbParams();
            var db = new MyWorldDbAccess();

            // pick characters that own abilities
            var p = Store.config.dbParams;
            using var conn = new MySqlConnection($"Server={p.Host};Port={p.Port};Database={p.DatabaseName};User ID={p.Username};Password={p.Password};Pooling=false;");
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT DISTINCT char_id FROM char_abilities WHERE char_id > 0 LIMIT 20";
            using var reader = cmd.ExecuteReader();
            var checkedCharacters = 0;
            var realAbilities = 0;
            while (reader.Read())
            {
                var charId = Convert.ToUInt32(reader.GetValue(0));
                var abilities = db.GetCharacterAbilities(charId);
                Assert.NotEmpty(abilities);
                // ability 0 ("None", GO id 0x80000000) exists in the data as a placeholder row
                Assert.All(abilities, a => Assert.True(a.AbilityId < 4096, "ability id " + a.AbilityId));
                foreach (var a in abilities) if (a.AbilityId > 0) realAbilities++;
                checkedCharacters++;
            }
            Assert.True(checkedCharacters > 0, "no char_abilities rows in the database");
            Assert.True(realAbilities > 0, "only placeholder abilities found");
        }
    }
}
