using FluentMigrator;
using OTA.Data.Dapper.Extensions;

namespace TDSM.Core.Data.Models.Migrations
{
    public partial class CreateAndSeed : Migration
    {
        public void PermissionNode_Up()
        {
            var table = this.Create.Table<PermissionNode>();

            table.WithColumn("Id")
                .AsInt64()
                .Identity()
                .PrimaryKey()
                .NotNullable()
                .Unique();

            table.WithColumn("Node")
                .AsString(255)
                .NotNullable();

            table.WithColumn("Permission")
                .AsInt32()
                .NotNullable();
        }

        public void PermissionNode_Down()
        {
            this.Delete.Table<PermissionNode>();
        }
    }
}
