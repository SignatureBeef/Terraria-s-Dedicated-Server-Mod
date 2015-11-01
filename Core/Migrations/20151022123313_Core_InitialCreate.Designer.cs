using System;
using TDSM.Core.Data;

namespace TDSM.Core.Migrations
{
#if ENTITY_FRAMEWORK_7
    [DbContext(typeof(TContext))]
    [Migration("20151022123313_Core_InitialCreate")]
    partial class Core_InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Annotation("ProductVersion", "7.0.0-beta8-15964");

            modelBuilder.Entity("TDSM.Core.Data.DbPlayer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateAddedUTC");

                    b.Property<string>("Name");

                    b.Property<bool>("Operator");

                    b.Property<string>("Password");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("TDSM.Core.Data.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("ApplyToGuests");

                    b.Property<byte>("Chat_Blue");

                    b.Property<byte>("Chat_Green");

                    b.Property<string>("Chat_Prefix");

                    b.Property<byte>("Chat_Red");

                    b.Property<string>("Chat_Suffix");

                    b.Property<string>("Name");

                    b.Property<string>("Parent");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("TDSM.Core.Data.Models.APIAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("PasswordFormat");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("Username");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("TDSM.Core.Data.Models.APIAccountRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccountId");

                    b.Property<DateTime>("DateFrom");

                    b.Property<DateTime?>("DateTo");

                    b.Property<string>("Type");

                    b.Property<string>("Value");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("TDSM.Core.Data.Models.Character", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AnglerQuests");

                    b.Property<byte>("Difficulty");

                    b.Property<uint>("EyeColor");

                    b.Property<int>("Hair");

                    b.Property<uint>("HairColor");

                    b.Property<byte>("HairDye");

                    b.Property<int>("Health");

                    b.Property<int>("HideVisual");

                    b.Property<int>("Mana");

                    b.Property<int>("MaxHealth");

                    b.Property<int>("MaxMana");

                    b.Property<uint>("PantsColor");

                    b.Property<uint>("ShirtColor");

                    b.Property<uint>("ShoeColor");

                    b.Property<uint>("SkinColor");

                    b.Property<int>("SpawnX");

                    b.Property<int>("SpawnY");

                    b.Property<string>("UUID");

                    b.Property<uint>("UnderShirtColor");

                    b.Property<int?>("UserId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("TDSM.Core.Data.Models.DataSetting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DataKey");

                    b.Property<string>("DataValue");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("TDSM.Core.Data.Models.GroupNode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("GroupId");

                    b.Property<int>("NodeId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("TDSM.Core.Data.Models.LoadoutItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ItemId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("TDSM.Core.Data.Models.NodePermission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Node");

                    b.Property<byte>("Permission");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("TDSM.Core.Data.Models.PlayerGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("GroupId");

                    b.Property<int>("UserId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("TDSM.Core.Data.Models.PlayerNode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("NodeId");

                    b.Property<int>("UserId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("TDSM.Core.ServerCharacters.SlotItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CharacterId");

                    b.Property<bool>("Favorite");

                    b.Property<int>("NetId");

                    b.Property<int>("Prefix");

                    b.Property<int>("Slot");

                    b.Property<int>("Stack");

                    b.Property<int>("Type");

                    b.HasKey("Id");
                });
        }
    }
#endif
}
