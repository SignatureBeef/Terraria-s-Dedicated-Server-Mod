using FluentMigrator;
using OTA.Data.Dapper.Extensions;
using OTA.Data.Dapper.Mappers;
using TDSM.Core.ServerCharacters.Models;

namespace TDSM.Core.Data.Models.Migrations
{
    public partial class CreateAndSeed : Migration
    {
        public void Character_Up()
        {
            var table = this.Create.Table<Character>();

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

            table.WithColumn("UUID")
                .AsString(50)
                .NotNullable()
                .Indexed()
                .WithDefaultValue(string.Empty);

            table.WithColumn("Health")
                .AsInt32()
                .NotNullable();

            table.WithColumn("MaxHealth")
                .AsInt32()
                .NotNullable();

            table.WithColumn("Mana")
                .AsInt32()
                .NotNullable();

            table.WithColumn("MaxMana")
                .AsInt32()
                .NotNullable();

            table.WithColumn("SpawnX")
                .AsInt32()
                .NotNullable();

            table.WithColumn("SpawnY")
                .AsInt32()
                .NotNullable();

            table.WithColumn("Hair")
                .AsInt32()
                .NotNullable();

            table.WithColumn("HairDye")
                .AsByte()
                .NotNullable();

            table.WithColumn("HideVisual")
                .AsInt32()
                .NotNullable();

            table.WithColumn("Difficulty")
                .AsByte()
                .NotNullable();

            table.WithColumn("HairColor")
                .AsInt32()
                .NotNullable();

            table.WithColumn("SkinColor")
                .AsInt32()
                .NotNullable();

            table.WithColumn("EyeColor")
                .AsInt32()
                .NotNullable();

            table.WithColumn("ShirtColor")
                .AsInt32()
                .NotNullable();

            table.WithColumn("UnderShirtColor")
                .AsInt32()
                .NotNullable();

            table.WithColumn("PantsColor")
                .AsInt32()
                .NotNullable();

            table.WithColumn("ShoeColor")
                .AsInt32()
                .NotNullable();

            table.WithColumn("AnglerQuests")
                .AsInt32()
                .NotNullable();
        }

        public void Character_Down()
        {
            this.Delete.Table<Character>();
        }
    }
}
