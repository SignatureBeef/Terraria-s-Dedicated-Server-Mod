#if ENTITY_FRAMEWORK_7
using OTA.Data.EF7;
using OTA.Data.EF7.Extensions;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata.Builders;
using System;
using System.Linq.Expressions;
using TDSM.Core.Data.Models;
using TDSM.Core.ServerCharacters;
#endif

namespace TDSM.Core.Data
{
#if ENTITY_FRAMEWORK_6
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
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            CreateModel(modelBuilder);
        }

        public void CreateModel(DbModelBuilder builder)
        {
            builder.Conventions.Remove<PluralizingTableNameConvention>();

            if (this.Database.Connection.GetType().Name == "SQLiteConnection") //Since we support SQLite as default, let's use this hack...
            {
                //Database.SetInitializer(new SqliteContextInitializer<OTAContext>(builder)); LETS
            }

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
#elif ENTITY_FRAMEWORK_7
    public class TContext : OTAContext
    {
        //public TContext() : this("terraria_ota")
        //{
        //}

        //public TContext(string nameOrConnectionString = "terraria_ota") : base(nameOrConnectionString)
        //{

        //}

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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            CreateModel(modelBuilder);
        }

        private EntityTypeBuilder<T> DefaultEntity<T>(ModelBuilder builder, Expression<Func<T, object>> keyExpression) where T : class
        {
            var entity = builder.Entity<T>();

            entity.HasKey(keyExpression);
            entity.Property(keyExpression).ValueGeneratedOnAdd();

            return entity;
        }

        public void CreateModel(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            DefaultEntity<Character>(builder, x => x.Id);
            DefaultEntity<LoadoutItem>(builder, x => x.Id);
            DefaultEntity<SlotItem>(builder, x => x.Id);
            DefaultEntity<Group>(builder, x => x.Id);
            DefaultEntity<DbPlayer>(builder, x => x.Id).ToTable("Player");
            DefaultEntity<PlayerGroup>(builder, x => x.Id);
            DefaultEntity<NodePermission>(builder, x => x.Id);
            DefaultEntity<PlayerNode>(builder, x => x.Id);
            DefaultEntity<GroupNode>(builder, x => x.Id);
            DefaultEntity<APIAccount>(builder, x => x.Id);
            DefaultEntity<APIAccountRole>(builder, x => x.Id);
            DefaultEntity<DataSetting>(builder, x => x.Id);
        }
    }
#endif
}