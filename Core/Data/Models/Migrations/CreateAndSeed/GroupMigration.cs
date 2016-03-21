using FluentMigrator;
using OTA.Data.Dapper.Extensions;
using TDSM.Core.ServerCharacters.Models;

namespace TDSM.Core.Data.Models.Migrations
{
    public partial class CreateAndSeed : Migration
    {
        public void Group_Up()
        {
            var table = this.Create.Table<Group>();

            table.WithColumn("Id")
                .AsInt64()
                .Identity()
                .PrimaryKey()
                .NotNullable()
                .Unique();

            table.WithColumn("Name")
                .AsString(255)
                .NotNullable();

            table.WithColumn("ApplyToGuests")
                .AsBoolean()
                .NotNullable();

            table.WithColumn("Parent")
                .AsString(255)
                .Nullable();

            table.WithColumn("Chat_Red")
                .AsByte()
                .NotNullable()
                .WithDefaultValue(255);

            table.WithColumn("Chat_Green")
                .AsByte()
                .NotNullable()
                .WithDefaultValue(255);

            table.WithColumn("Chat_Blue")
                .AsByte()
                .NotNullable()
                .WithDefaultValue(255);

            table.WithColumn("Chat_Prefix")
                .AsString(25)
                .Nullable();

            table.WithColumn("Chat_Suffix")
                .AsString(25)
                .Nullable();
        }

        public void Group_Down()
        {
            this.Delete.Table<Group>();
        }
    }
}
