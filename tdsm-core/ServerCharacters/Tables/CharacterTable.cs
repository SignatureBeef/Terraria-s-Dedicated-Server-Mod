using System;
using TDSM.API.Data;
using TDSM.Core.ServerCharacters;

namespace TDSM.Core.ServerCharacters.Tables
{
    internal class CharacterTable
    {
        public const String TableName = "SSC";

        static class ColumnNames
        {
            public const String UserId = "UserId";
            public const String Health = "Health";
            public const String MaxHealth = "MaxHealth";
            public const String Mana = "Mana";
            public const String MaxMana = "MaxMana";
            public const String SpawnX = "SpawnX";
            public const String SpawnY = "SpawnY";
            public const String Hair = "Hair";
            public const String HairDye = "HairDye";
            public const String HideVisual = "HideVisual";
            public const String Difficulty = "Difficulty";
            public const String HairColor = "HairColor";
            public const String SkinColor = "SkinColor";
            public const String EyeColor = "EyeColor";
            public const String ShirtColor = "ShirtColor";
            public const String UnderShirtColor = "UnderShirtColor";
            public const String PantsColor = "PantsColor";
            public const String ShoeColor = "ShoeColor";
            public const String AnglerQuests = "AnglerQuests";
        }

        public static readonly TableColumn[] Columns = new TableColumn[]
        {
            new TableColumn(ColumnNames.UserId, typeof(Int32), true, true),
            new TableColumn(ColumnNames.Health, typeof(Int32)),
            new TableColumn(ColumnNames.MaxHealth, typeof(Int32)),
            new TableColumn(ColumnNames.Mana, typeof(Int32)),
            new TableColumn(ColumnNames.MaxMana, typeof(Int32)),
            new TableColumn(ColumnNames.SpawnX, typeof(Int32)),
            new TableColumn(ColumnNames.SpawnY, typeof(Int32)),
            new TableColumn(ColumnNames.Hair, typeof(Int32)),
            new TableColumn(ColumnNames.HairDye, typeof(Byte)),

            //This could techincally fit in a short, but that would leave only 6 bits left (HideVisual is 10)
            //I would rather use a typical int just for furture additions
            new TableColumn(ColumnNames.HideVisual, typeof(Int32)), 

            new TableColumn(ColumnNames.Difficulty, typeof(Byte)),
            new TableColumn(ColumnNames.HairColor, typeof(UInt32)),
            new TableColumn(ColumnNames.SkinColor, typeof(UInt32)),
            new TableColumn(ColumnNames.EyeColor, typeof(UInt32)),
            new TableColumn(ColumnNames.ShirtColor, typeof(UInt32)),
            new TableColumn(ColumnNames.UnderShirtColor, typeof(UInt32)),
            new TableColumn(ColumnNames.PantsColor, typeof(UInt32)),
            new TableColumn(ColumnNames.ShoeColor, typeof(UInt32)),
            new TableColumn(ColumnNames.AnglerQuests, typeof(Int32))
        };

        public static bool Exists()
        {
            using (var bl = Storage.GetBuilder(CharacterManager.SQLSafeName))
            {
                bl.TableExists(TableName);

                return Storage.Execute(bl);
            }
        }

        public static bool Create()
        {
            using (var bl = Storage.GetBuilder(CharacterManager.SQLSafeName))
            {
                bl.TableCreate(TableName, Columns);

                return Storage.ExecuteNonQuery(bl) > 0;
            }
        }
    }
}