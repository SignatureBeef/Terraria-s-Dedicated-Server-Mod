using FluentMigrator;
using OTA.Data.Dapper.Extensions;

namespace TDSM.Core.Data.Models.Migrations
{
    public partial class CreateAndSeed : Migration
    {
        public void PlayerGroup_Up()
        {
            var table = this.Create.Table<PlayerGroup>();

            table.WithColumn("Id")
                .AsInt64()
                .Identity()
                .PrimaryKey()
                .NotNullable()
                .Unique();

            table.WithColumn("PlayerId")
                .AsInt64()
                .NotNullable()
                .Indexed()
                .ForeignKey(TableMapper.TypeToName<DbPlayer>(), "Id");

            table.WithColumn("GroupId")
                .AsInt64()
                .NotNullable()
                .Indexed()
                .ForeignKey(TableMapper.TypeToName<Group>(), "Id");
        }

        public void PlayerGroup_Down()
        {
            this.Delete.Table<PlayerGroup>();
        }
    }
}
