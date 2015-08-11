using System;
using TDSM.API.Data;
using TDSM.Core.ServerCharacters;

namespace TDSM.Core.ServerCharacters.Tables
{
    internal class DefaultLoadoutTable
    {
        public const String TableName = "SSC_PlayerBuff";

        static class ColumnNames
        {
            public const String Id = "Id";
            public const String TypeId = "TypeId";
            public const String ItemId = "ItemId";
        }

        public static readonly TableColumn[] Columns = new TableColumn[]
            {
                new TableColumn(ColumnNames.Id, typeof(Int32), true, true),
                new TableColumn(ColumnNames.TypeId, typeof(Int32)),
                new TableColumn(ColumnNames.ItemId, typeof(Int32))
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