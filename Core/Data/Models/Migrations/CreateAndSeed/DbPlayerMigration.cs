using FluentMigrator;
using OTA.Data.Dapper.Extensions;

namespace TDSM.Core.Data.Models.Migrations
{
    public partial class CreateAndSeed : Migration
    {
        public void Player_Up()
        {
            var table = this.Create.Table<DbPlayer>();

            table.WithColumn("Id")
                .AsInt64()
                .Identity()
                .PrimaryKey()
                .NotNullable()
                .Unique();

            table.WithColumn("Name")
                .AsString(50)
                .Unique()
                .NotNullable();

            table.WithColumn("Password")
                .AsString(255)
                .NotNullable();

            table.WithColumn("Operator")
                .AsBoolean()
                .NotNullable();

            table.WithColumn("DateAdded")
                .AsDateTime()
                .NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime);
        }

        public void Player_Down()
        {
            this.Delete.Table<DbPlayer>();
        }
    }
}
