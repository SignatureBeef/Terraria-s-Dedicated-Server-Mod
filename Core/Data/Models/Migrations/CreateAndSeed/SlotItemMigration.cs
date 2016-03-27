
using FluentMigrator;
using OTA.Data.Dapper.Extensions;
using TDSM.Core.ServerCharacters.Models;

namespace TDSM.Core.Data.Models.Migrations
{
    public partial class CreateAndSeed : Migration
    {
        public void SlotItem_Up()
        {
            var table = this.Create.Table<SlotItem>();

            table.WithColumn("Id")
                .AsInt64()
                .Identity()
                .PrimaryKey()
                .NotNullable()
                .Unique();

            table.WithColumn("NetId")
                .AsInt32()
                .NotNullable();

            table.WithColumn("Stack")
                .AsInt32()
                .NotNullable();

            table.WithColumn("Prefix")
                .AsInt32()
                .NotNullable();

            table.WithColumn("Favorite")
                .AsBoolean()
                .NotNullable();

            table.WithColumn("CharacterId")
                .AsInt64()
                .Nullable();

            table.WithColumn("Type")
                .AsInt32()
                .NotNullable();

            table.WithColumn("Slot")
                .AsInt32()
                .NotNullable();
        }

        public void SlotItem_Down()
        {
            this.Delete.Table<SlotItem>();
        }
    }
}
