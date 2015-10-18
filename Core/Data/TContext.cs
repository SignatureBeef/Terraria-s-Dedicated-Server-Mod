using System;
using OTA.Data;
using System.Data.Entity;
using TDSM.Core.Data.Models;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations.Schema;
using TDSM.Core.ServerCharacters;
using OTA;

namespace TDSM.Core.Data.Models
{
    public class LoadoutItem
    {
        public int Id { get; set; }

        public int ItemId { get; set; }
    }

    public class Character
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public string UUID { get; set; }

        public int Health { get; set; }

        public int MaxHealth { get; set; }

        public int Mana { get; set; }

        public int MaxMana { get; set; }

        public int SpawnX { get; set; }

        public int SpawnY { get; set; }

        public int Hair { get; set; }

        public byte HairDye { get; set; }

        public int HideVisual { get; set; }

        public byte Difficulty { get; set; }

        public uint HairColor { get; set; }

        public uint SkinColor { get; set; }

        public uint EyeColor { get; set; }

        public uint ShirtColor { get; set; }

        public uint UnderShirtColor { get; set; }

        public uint PantsColor { get; set; }

        public uint ShoeColor { get; set; }

        public int AnglerQuests { get; set; }

        public ServerCharacter ToServerCharacter()
        {
            return new ServerCharacter()
            {
                Id = this.Id,
                UserId = this.UserId,
                UUID = this.UUID,

                Health = this.Health,
                MaxHealth = this.MaxHealth,
                Mana = this.Mana,
                MaxMana = this.MaxMana,
                SpawnX = this.SpawnX,
                SpawnY = this.SpawnY,
                Hair = this.Hair,
                HairDye = this.HairDye,
                HideVisual = OTA.Tools.Encoding.DecodeBits(this.HideVisual),
                Difficulty = this.Difficulty,
                HairColor = new Microsoft.Xna.Framework.Color(this.HairColor),
                SkinColor = new Microsoft.Xna.Framework.Color(this.SkinColor),
                EyeColor = new Microsoft.Xna.Framework.Color(this.EyeColor),
                ShirtColor = new Microsoft.Xna.Framework.Color(this.ShirtColor),
                UnderShirtColor = new Microsoft.Xna.Framework.Color(this.UnderShirtColor),
                PantsColor = new Microsoft.Xna.Framework.Color(this.PantsColor),
                ShoeColor = new Microsoft.Xna.Framework.Color(this.ShoeColor),
                AnglerQuests = this.AnglerQuests
            };
        }
    }
}

namespace TDSM.Core.Data
{
    public class TContext : DbContext
    {
        public TContext() : this("terraria_ota")
        {
        }

        public TContext(string nameOrConnectionString = "terraria_ota") : base(nameOrConnectionString)
        {
            
        }

        public DbSet<Character> Characters { get; set; }

        public DbSet<LoadoutItem> DefaultLoadout { get; set; }

        public DbSet<SlotItem> Items { get; set; }


        public DbSet<PlayerGroup> PlayerGroups { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<DbPlayer> Players { get; set; }

        public DbSet<NodePermission> Nodes { get; set; }

        public DbSet<PlayerNode> PlayerNodes { get; set; }

        public DbSet<GroupNode> GroupNodes { get; set; }

        public DbSet<APIAccount> APIAccounts { get; set; }

        public DbSet<APIAccountRole> APIAccountsRoles { get; set; }

        public DbSet<DataSetting> Settings { get; set; }

        protected override void OnModelCreating(DbModelBuilder builder)
        {
            CreateModel(builder);
        }

        public void CreateModel(DbModelBuilder builder)
        {
//            builder.HasDefaultSchema("tdsm");

            OTA.Logging.ProgramLog.Admin.Log("Initialising TDSM database context");
            builder.Conventions.Remove<PluralizingTableNameConvention>();

//            if (this.Database.Connection.GetType().Name == "SQLiteConnection") //Since we support SQLite as default, let's use this hack...
//            {
//                Database.SetInitializer(new SqliteContextInitializer<OTAContext>(builder));
//            }

            builder.Entity<Character>()
                .HasKey(x => x.Id)
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            builder.Entity<LoadoutItem>()
                .HasKey(x => x.Id)
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            builder.Entity<SlotItem>()
                .HasKey(x => x.Id)
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            

            builder.Entity<Group>()
                .HasKey(x => x.Id)
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            builder.Entity<DbPlayer>()
                .ToTable("Player")
                .HasKey(x => x.Id)
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            builder.Entity<PlayerGroup>()
                .HasKey(x => new { x.Id })
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            builder.Entity<NodePermission>()
                .HasKey(x => new { x.Id })
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            builder.Entity<PlayerNode>()
                .HasKey(x => new { x.Id })
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            builder.Entity<GroupNode>()
                .HasKey(x => new { x.Id })
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            builder.Entity<APIAccount>()
                .HasKey(x => new { x.Id })
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            builder.Entity<APIAccountRole>()
                .HasKey(x => new { x.Id })
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            builder.Entity<DataSetting>()
                .HasKey(x => new { x.Id })
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}