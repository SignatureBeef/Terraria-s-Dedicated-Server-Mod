using FluentMigrator;
using OTA.Data.Dapper.Extensions;

namespace TDSM.Core.Data.Models.Migrations
{
    public partial class CreateAndSeed : Migration
    {
        public void PlayerNode_Up()
        {
            var table = this.Create.Table<PlayerNode>();

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
                .ForeignKey("DbPlayer", "Id");

            table.WithColumn("NodeId")
                .AsInt64()
                .NotNullable()
                .Indexed()
                .ForeignKey("PermissionNode", "Id");
        }

        public void PlayerNode_Down()
        {
            this.Delete.Table<PlayerNode>();
        }
    }
}
