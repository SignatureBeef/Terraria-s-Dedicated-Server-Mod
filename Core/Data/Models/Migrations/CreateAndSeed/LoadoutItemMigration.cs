using FluentMigrator;
using OTA.Data.Dapper.Extensions;
using OTA.Data.Dapper.Mappers;
using TDSM.Core.ServerCharacters.Models;

namespace TDSM.Core.Data.Models.Migrations
{
    public partial class CreateAndSeed : Migration
    {
        public void LoadoutItem_Up()
        {
            var table = this.Create.Table<LoadoutItem>();

            table.WithColumn("Id")
                .AsInt64()
                .Identity()
                .PrimaryKey()
                .NotNullable()
                .Unique();

            table.WithColumn("ItemId")
                .AsInt64()
                .NotNullable()
                .Indexed()
                .ForeignKey(TableMapper.TypeToName<SlotItem>(), "Id");
        }

        public void LoadoutItem_Down()
        {
            this.Delete.Table<LoadoutItem>();
        }
    }
}
