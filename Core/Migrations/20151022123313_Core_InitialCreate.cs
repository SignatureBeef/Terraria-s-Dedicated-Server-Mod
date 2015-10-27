using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using OTA.Data.EF7;
using OTA.Data.EF7.Extensions;

namespace TDSM.Core.Migrations
{
    public partial class Core_InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            Console.WriteLine("migrationBuilder.ActiveProvider: " + migrationBuilder.ActiveProvider);
            migrationBuilder.CreateTable(
                name: "DbPlayer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .AutoIncrement(migrationBuilder.ActiveProvider),
                    DateAddedUTC = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Operator = table.Column<bool>(nullable: false),
                    Password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbPlayer", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .AutoIncrement(migrationBuilder.ActiveProvider),
                    ApplyToGuests = table.Column<bool>(nullable: false),
                    Chat_Blue = table.Column<byte>(nullable: false),
                    Chat_Green = table.Column<byte>(nullable: false),
                    Chat_Prefix = table.Column<string>(nullable: true),
                    Chat_Red = table.Column<byte>(nullable: false),
                    Chat_Suffix = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Parent = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "APIAccount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .AutoIncrement(migrationBuilder.ActiveProvider),
                    PasswordFormat = table.Column<int>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APIAccount", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "APIAccountRole",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .AutoIncrement(migrationBuilder.ActiveProvider),
                    AccountId = table.Column<int>(nullable: false),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APIAccountRole", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "Character",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .AutoIncrement(migrationBuilder.ActiveProvider),
                    AnglerQuests = table.Column<int>(nullable: false),
                    Difficulty = table.Column<byte>(nullable: false),
                    EyeColor = table.Column<uint>(nullable: false),
                    Hair = table.Column<int>(nullable: false),
                    HairColor = table.Column<uint>(nullable: false),
                    HairDye = table.Column<byte>(nullable: false),
                    Health = table.Column<int>(nullable: false),
                    HideVisual = table.Column<int>(nullable: false),
                    Mana = table.Column<int>(nullable: false),
                    MaxHealth = table.Column<int>(nullable: false),
                    MaxMana = table.Column<int>(nullable: false),
                    PantsColor = table.Column<uint>(nullable: false),
                    ShirtColor = table.Column<uint>(nullable: false),
                    ShoeColor = table.Column<uint>(nullable: false),
                    SkinColor = table.Column<uint>(nullable: false),
                    SpawnX = table.Column<int>(nullable: false),
                    SpawnY = table.Column<int>(nullable: false),
                    UUID = table.Column<string>(nullable: true),
                    UnderShirtColor = table.Column<uint>(nullable: false),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Character", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "DataSetting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .AutoIncrement(migrationBuilder.ActiveProvider),
                    DataKey = table.Column<string>(nullable: true),
                    DataValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSetting", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "GroupNode",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .AutoIncrement(migrationBuilder.ActiveProvider),
                    GroupId = table.Column<int>(nullable: false),
                    NodeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupNode", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "LoadoutItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .AutoIncrement(migrationBuilder.ActiveProvider),
                    ItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoadoutItem", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "NodePermission",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .AutoIncrement(migrationBuilder.ActiveProvider),
                    Node = table.Column<string>(nullable: true),
                    Permission = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodePermission", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "PlayerGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .AutoIncrement(migrationBuilder.ActiveProvider),
                    GroupId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerGroup", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "PlayerNode",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .AutoIncrement(migrationBuilder.ActiveProvider),
                    NodeId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerNode", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "SlotItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .AutoIncrement(migrationBuilder.ActiveProvider),
                    CharacterId = table.Column<int>(nullable: true),
                    Favorite = table.Column<bool>(nullable: false),
                    NetId = table.Column<int>(nullable: false),
                    Prefix = table.Column<int>(nullable: false),
                    Slot = table.Column<int>(nullable: false),
                    Stack = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlotItem", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("DbPlayer");
            migrationBuilder.DropTable("Group");
            migrationBuilder.DropTable("APIAccount");
            migrationBuilder.DropTable("APIAccountRole");
            migrationBuilder.DropTable("Character");
            migrationBuilder.DropTable("DataSetting");
            migrationBuilder.DropTable("GroupNode");
            migrationBuilder.DropTable("LoadoutItem");
            migrationBuilder.DropTable("NodePermission");
            migrationBuilder.DropTable("PlayerGroup");
            migrationBuilder.DropTable("PlayerNode");
            migrationBuilder.DropTable("SlotItem");
        }
    }
}
