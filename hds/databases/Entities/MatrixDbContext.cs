using Microsoft.EntityFrameworkCore;

#nullable disable

namespace hds.databases.Entities
{
    public partial class MatrixDbContext : DbContext
    {
        private DbParams dbParams;

        public MatrixDbContext(DbParams dbParams)
        {
            this.dbParams = dbParams;
        }

        public MatrixDbContext(DbContextOptions<MatrixDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Buddylist> Buddylists { get; set; }
        public virtual DbSet<CharAbility> CharAbilities { get; set; }
        public virtual DbSet<CharHardline> CharHardlines { get; set; }
        public virtual DbSet<Character> Characters { get; set; }
        public virtual DbSet<Crew> Crews { get; set; }
        public virtual DbSet<CrewMember> CrewMembers { get; set; }
        public virtual DbSet<DataHardline> DataHardlines { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<Faction> Factions { get; set; }
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Marketplace> Marketplaces { get; set; }
        public virtual DbSet<Rsivalue> Rsivalues { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<World> Worlds { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(
                    "server=" + dbParams.Host + ";port=" + dbParams.Port + ";user=" + dbParams.Username + ";password=" +
                    dbParams.Password + ";database=" + dbParams.DatabaseName + ";ConvertZeroDateTime=True",
                    ServerVersion.FromString("5.7.29-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Buddylist>(entity =>
            {
                entity.ToTable("buddylist");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.CharId)
                    .HasColumnType("int(11)")
                    .HasColumnName("charId");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.FriendId)
                    .HasColumnType("int(11)")
                    .HasColumnName("friendId");

                entity.Property(e => e.IsIgnored)
                    .HasColumnType("tinyint(4)")
                    .HasColumnName("is_ignored");
            });

            modelBuilder.Entity<CharAbility>(entity =>
            {
                entity.ToTable("char_abilities");

                entity.HasIndex(e => e.CharId, "char_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.AbilityId)
                    .HasColumnType("bigint(11)")
                    .HasColumnName("ability_id");

                entity.Property(e => e.AbilityName)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasColumnName("ability_name")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Added)
                    .HasColumnType("datetime")
                    .HasColumnName("added")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CharId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("char_id");

                entity.Property(e => e.IsLoaded).HasColumnName("is_loaded");

                entity.Property(e => e.Level)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("level");

                entity.Property(e => e.Slot)
                    .HasColumnType("int(11)")
                    .HasColumnName("slot");
            });

            modelBuilder.Entity<CharHardline>(entity =>
            {
                entity.ToTable("char_hardlines");

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.Added)
                    .HasColumnType("datetime")
                    .HasColumnName("added");

                entity.Property(e => e.CharId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("char_id");

                entity.Property(e => e.DistrictId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("district_id");

                entity.Property(e => e.HlId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("hl_id");
            });

            modelBuilder.Entity<Character>(entity =>
            {
                entity.HasKey(e => e.CharId)
                    .HasName("PRIMARY");

                entity.ToTable("characters");

                entity.HasIndex(e => e.Handle, "handle");

                entity.Property(e => e.CharId)
                    .HasColumnType("bigint(11) unsigned")
                    .HasColumnName("charId");

                entity.Property(e => e.Alignment)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("alignment");

                entity.Property(e => e.Background)
                    .HasColumnType("varchar(1024)")
                    .HasColumnName("background")
                    .HasDefaultValueSql("''")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Cash)
                    .HasColumnType("int(11)")
                    .HasColumnName("cash")
                    .HasDefaultValueSql("'1000'");

                entity.Property(e => e.ConquestPoints)
                    .HasColumnType("int(11)")
                    .HasColumnName("conquest_points");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created");

                entity.Property(e => e.CrewId)
                    .HasColumnType("mediumint(8) unsigned")
                    .HasColumnName("crewId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.District)
                    .IsRequired()
                    .HasColumnType("varchar(256)")
                    .HasColumnName("district")
                    .HasDefaultValueSql("'slums'")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.DistrictId)
                    .HasColumnType("tinyint(3) unsigned")
                    .HasColumnName("districtId")
                    .HasDefaultValueSql("'1'")
                    .HasComment("Default must be changed to 0 (tutorial) later");

                entity.Property(e => e.Exp)
                    .HasColumnType("int(11)")
                    .HasColumnName("exp");

                entity.Property(e => e.FactionId)
                    .HasColumnType("mediumint(8) unsigned")
                    .HasColumnName("factionId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasColumnName("firstName")
                    .HasDefaultValueSql("''")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Handle)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasColumnName("handle")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.HealthC)
                    .HasColumnType("mediumint(11)")
                    .HasColumnName("healthC")
                    .HasDefaultValueSql("'500'");

                entity.Property(e => e.HealthM)
                    .HasColumnType("mediumint(11)")
                    .HasColumnName("healthM")
                    .HasDefaultValueSql("'500'");

                entity.Property(e => e.InnerStrC)
                    .HasColumnType("mediumint(11)")
                    .HasColumnName("innerStrC")
                    .HasDefaultValueSql("'200'");

                entity.Property(e => e.InnerStrM)
                    .HasColumnType("mediumint(11)")
                    .HasColumnName("innerStrM")
                    .HasDefaultValueSql("'200'");

                entity.Property(e => e.IsDeleted)
                    .HasColumnType("int(11)")
                    .HasColumnName("is_deleted");

                entity.Property(e => e.IsOnline)
                    .HasColumnType("tinyint(4)")
                    .HasColumnName("is_online");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasColumnName("lastName")
                    .HasDefaultValueSql("''")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Level)
                    .HasColumnType("mediumint(11)")
                    .HasColumnName("level")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Profession)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("profession")
                    .HasDefaultValueSql("'2'");

                entity.Property(e => e.Pvpflag)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("pvpflag");

                entity.Property(e => e.RepCyph)
                    .HasColumnType("int(11)")
                    .HasColumnName("repCYPH");

                entity.Property(e => e.RepEpn)
                    .HasColumnType("int(11)")
                    .HasColumnName("repEPN");

                entity.Property(e => e.RepGm)
                    .HasColumnType("int(11)")
                    .HasColumnName("repGM");

                entity.Property(e => e.RepMachine)
                    .HasColumnType("int(11)")
                    .HasColumnName("repMachine");

                entity.Property(e => e.RepMero)
                    .HasColumnType("int(11)")
                    .HasColumnName("repMero");

                entity.Property(e => e.RepNiobe)
                    .HasColumnType("int(11)")
                    .HasColumnName("repNiobe");

                entity.Property(e => e.RepZion)
                    .HasColumnType("int(11)")
                    .HasColumnName("repZion");

                entity.Property(e => e.Rotation)
                    .HasColumnType("mediumint(11)")
                    .HasColumnName("rotation")
                    .HasDefaultValueSql("'139'");

                entity.Property(e => e.Status)
                    .HasColumnType("tinyint(3) unsigned")
                    .HasColumnName("status")
                    .HasComment("transit/banned");

                entity.Property(e => e.UserId)
                    .HasColumnType("int(11) unsigned")
                    .HasColumnName("userId");

                entity.Property(e => e.WorldId)
                    .HasColumnType("smallint(5) unsigned")
                    .HasColumnName("worldId");

                entity.Property(e => e.X)
                    .HasColumnName("x")
                    .HasDefaultValueSql("'17020'");

                entity.Property(e => e.Y)
                    .HasColumnName("y")
                    .HasDefaultValueSql("'495'");

                entity.Property(e => e.Z)
                    .HasColumnName("z")
                    .HasDefaultValueSql("'2693'");
            });

            modelBuilder.Entity<Crew>(entity =>
            {
                entity.ToTable("crews");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CrewName)
                    .HasColumnType("varchar(256)")
                    .HasColumnName("crew_name")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.FactionId)
                    .HasColumnType("int(11)")
                    .HasColumnName("faction_id");

                entity.Property(e => e.FactionRank)
                    .HasColumnType("int(11)")
                    .HasColumnName("faction_rank");

                entity.Property(e => e.MasterPlayerHandle)
                    .HasColumnType("varchar(256)")
                    .HasColumnName("master_player_handle")
                    .HasDefaultValueSql("''")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Money)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("money");

                entity.Property(e => e.Org)
                    .HasColumnType("int(11)")
                    .HasColumnName("org");
            });

            modelBuilder.Entity<CrewMember>(entity =>
            {
                entity.ToTable("crew_members");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.CharId)
                    .HasColumnType("int(11)")
                    .HasColumnName("char_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.CrewId)
                    .HasColumnType("int(11)")
                    .HasColumnName("crew_id");

                entity.Property(e => e.IsCaptain)
                    .HasColumnType("int(11)")
                    .HasColumnName("is_captain");

                entity.Property(e => e.IsFirstMate)
                    .HasColumnType("int(11)")
                    .HasColumnName("is_first_mate");
            });

            modelBuilder.Entity<DataHardline>(entity =>
            {
                entity.ToTable("data_hardlines");

                entity.HasIndex(e => e.Id, "Id")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("int(11) unsigned");

                entity.Property(e => e.DistrictId).HasColumnType("smallint(6) unsigned");

                entity.Property(e => e.HardLineId).HasColumnType("smallint(6) unsigned");

                entity.Property(e => e.HardlineName)
                    .IsRequired()
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ObjectId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("objectId");

                entity.Property(e => e.Rot).HasColumnName("ROT");
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.ToTable("districts");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("key")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("path")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Faction>(entity =>
            {
                entity.ToTable("factions");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.MasterPlayerHandle)
                    .HasColumnType("varchar(256)")
                    .HasColumnName("master_player_handle")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Money)
                    .HasColumnType("int(11)")
                    .HasColumnName("money");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(256)")
                    .HasColumnName("name")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.HasKey(e => e.InvId)
                    .HasName("PRIMARY");

                entity.ToTable("inventory");

                entity.Property(e => e.InvId)
                    .HasColumnType("int(11) unsigned")
                    .HasColumnName("invId");

                entity.Property(e => e.CharId)
                    .HasColumnType("bigint(11) unsigned")
                    .HasColumnName("charId");

                entity.Property(e => e.Count)
                    .HasColumnType("int(11)")
                    .HasColumnName("count");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Goid)
                    .HasColumnType("bigint(11)")
                    .HasColumnName("goid");

                entity.Property(e => e.Purity)
                    .HasColumnType("int(11)")
                    .HasColumnName("purity");

                entity.Property(e => e.Slot)
                    .HasColumnType("tinyint(11) unsigned")
                    .HasColumnName("slot");

                entity.Property(e => e.Updated)
                    .HasColumnType("datetime")
                    .HasColumnName("updated")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("locations");

                entity.HasIndex(e => e.Id, "Id")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("int(10) unsigned");

                entity.Property(e => e.Command)
                    .IsRequired()
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.District).HasColumnType("tinyint(3) unsigned");
            });

            modelBuilder.Entity<Marketplace>(entity =>
            {
                entity.ToTable("marketplace");

                entity.Property(e => e.Id)
                    .HasColumnType("int(10)")
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasColumnType("varchar(15)")
                    .HasColumnName("category")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CharId)
                    .HasColumnType("int(10)")
                    .HasColumnName("charID");

                entity.Property(e => e.CharRep)
                    .HasColumnType("int(10)")
                    .HasColumnName("charRep");

                entity.Property(e => e.Created)
                    .HasColumnType("int(10)")
                    .HasColumnName("created");

                entity.Property(e => e.DelistPrice)
                    .HasColumnType("int(10)")
                    .HasColumnName("delistPrice");

                entity.Property(e => e.IsSold)
                    .HasColumnType("int(10)")
                    .HasColumnName("is_sold");

                entity.Property(e => e.ItemId)
                    .HasColumnType("int(10)")
                    .HasColumnName("itemID");

                entity.Property(e => e.Price)
                    .HasColumnType("int(10)")
                    .HasColumnName("price");

                entity.Property(e => e.Purity)
                    .HasColumnType("int(10)")
                    .HasColumnName("purity");
            });

            modelBuilder.Entity<Rsivalue>(entity =>
            {
                entity.HasKey(e => e.Charid)
                    .HasName("PRIMARY");

                entity.ToTable("rsivalues");

                entity.Property(e => e.Charid)
                    .HasColumnType("smallint(6)")
                    .ValueGeneratedNever()
                    .HasColumnName("charid");

                entity.Property(e => e.Body)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("body");

                entity.Property(e => e.Coat)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("coat");

                entity.Property(e => e.Coatcolor)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("coatcolor");

                entity.Property(e => e.Face)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("face");

                entity.Property(e => e.Facialdetail)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("facialdetail");

                entity.Property(e => e.Facialdetailcolor)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("facialdetailcolor");

                entity.Property(e => e.Glasses)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("glasses");

                entity.Property(e => e.Glassescolor)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("glassescolor");

                entity.Property(e => e.Gloves)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("gloves");

                entity.Property(e => e.Hair)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("hair");

                entity.Property(e => e.Haircolor)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("haircolor");

                entity.Property(e => e.Hat)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("hat");

                entity.Property(e => e.Leggins)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("leggins");

                entity.Property(e => e.Pants)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("pants");

                entity.Property(e => e.Pantscolor)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("pantscolor");

                entity.Property(e => e.Sex)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("sex");

                entity.Property(e => e.Shirt)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("shirt");

                entity.Property(e => e.Shirtcolor)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("shirtcolor");

                entity.Property(e => e.Shoecolor)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("shoecolor");

                entity.Property(e => e.Shoes)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("shoes");

                entity.Property(e => e.Skintone)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("skintone");

                entity.Property(e => e.Tattoo)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("tattoo");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasIndex(e => e.UserId, "id")
                    .IsUnique();

                entity.HasIndex(e => e.Username, "username")
                    .IsUnique();

                entity.Property(e => e.UserId)
                    .HasColumnType("int(11) unsigned")
                    .HasColumnName("userId");

                entity.Property(e => e.AccountStatus)
                    .HasColumnType("int(11)")
                    .HasColumnName("account_status")
                    .HasComment("if banned");

                entity.Property(e => e.EmailAdress)
                    .HasColumnType("varchar(255)")
                    .HasColumnName("email_adress")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.PasswordHash)
                    .HasColumnType("varchar(40)")
                    .HasColumnName("passwordHash")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.PasswordSalt)
                    .HasColumnType("varchar(32)")
                    .HasColumnName("passwordSalt")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Passwordmd5)
                    .HasColumnType("varchar(40)")
                    .HasColumnName("passwordmd5")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.PrivateExponent)
                    .HasColumnType("tinyblob")
                    .HasColumnName("privateExponent");

                entity.Property(e => e.PublicExponent)
                    .HasColumnType("smallint(11) unsigned")
                    .HasColumnName("publicExponent");

                entity.Property(e => e.PublicModulus)
                    .HasColumnType("tinyblob")
                    .HasColumnName("publicModulus");

                entity.Property(e => e.Sessionid)
                    .HasColumnType("varchar(100)")
                    .HasColumnName("sessionid")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.TimeCreated)
                    .HasColumnType("timestamp")
                    .HasColumnName("timeCreated")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasColumnName("username")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<World>(entity =>
            {
                entity.ToTable("worlds");

                entity.Property(e => e.WorldId)
                    .HasColumnType("smallint(5) unsigned")
                    .HasColumnName("worldId");

                entity.Property(e => e.Load)
                    .HasColumnType("tinyint(3) unsigned")
                    .HasColumnName("load")
                    .HasDefaultValueSql("'49'");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(20)")
                    .HasColumnName("name")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.NumPlayers)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("numPlayers");

                entity.Property(e => e.Status)
                    .HasColumnType("tinyint(11) unsigned")
                    .HasColumnName("status")
                    .HasDefaultValueSql("'1'")
                    .HasComment("World Status (Down, Open etc.)");

                entity.Property(e => e.Type)
                    .HasColumnType("tinyint(11) unsigned")
                    .HasColumnName("type")
                    .HasDefaultValueSql("'1'")
                    .HasComment("1 for no pvp, 2 for pvp");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}