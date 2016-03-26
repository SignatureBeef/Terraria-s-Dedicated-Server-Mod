using FluentMigrator;
using OTA.Data.Dapper.Extensions;

namespace TDSM.Core.Data.Models.Migrations
{
    public partial class CreateAndSeed : Migration
    {
        public void APIAccount_Up()
        {
            var table = this.Create.Table<APIAccount>();

            table.WithColumn("Id")
                .AsInt64()
                .Identity()
                .PrimaryKey()
                .NotNullable()
                .Unique();

            table.WithColumn("Username")
                .AsString(255)
                .NotNullable()
                .Unique();

            table.WithColumn("PasswordHash")
                .AsString(512)
                .NotNullable();

            table.WithColumn("PasswordFormat")
                .AsInt32()
                .NotNullable();
        }

        public void APIAccount_Down()
        {
            this.Delete.Table<APIAccount>();
        }
    }
}
