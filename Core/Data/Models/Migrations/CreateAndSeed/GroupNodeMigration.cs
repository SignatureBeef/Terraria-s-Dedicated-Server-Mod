using FluentMigrator;
using OTA.Data.Dapper.Extensions;
using OTA.Data.Dapper.Mappers;

namespace TDSM.Core.Data.Models.Migrations
{
    public partial class CreateAndSeed : Migration
    {
        public void GroupNode_Up()
        {
            var table = this.Create.Table<GroupNode>();

            table.WithColumn("Id")
                .AsInt64()
                .Identity()
                .PrimaryKey()
                .NotNullable()
                .Unique();

            table.WithColumn("GroupId")
                .AsInt64()
                .NotNullable()
                .Indexed()
                .ForeignKey(TableMapper.TypeToName<Group>(), "Id");

            table.WithColumn("NodeId")
                .AsInt64()
                .NotNullable()
                .ForeignKey(TableMapper.TypeToName<PermissionNode>(), "Id");
        }

        public void GroupNode_Down()
        {
            this.Delete.Table<GroupNode>();
        }
    }
}
