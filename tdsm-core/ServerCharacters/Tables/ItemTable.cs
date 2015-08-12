using System;
using TDSM.API.Data;
using TDSM.Core.ServerCharacters;

namespace TDSM.Core.ServerCharacters.Tables
{
    internal class ItemTable
    {
        public const String TableName = "SSC_Items";

        static class ColumnNames
        {
            public const String Id = "Id";
            public const String UserId = "UserId";
            public const String TypeId = "TypeId";
            public const String NetId = "NetId";
            public const String Stack = "Stack";
            public const String Prefix = "Prefix";
            public const String Slot = "Slot";
        }

        public static readonly TableColumn[] Columns = new TableColumn[]
        {
            new TableColumn(ColumnNames.Id, typeof(Int32), true, true),
            new TableColumn(ColumnNames.UserId, typeof(Int32), true),
            new TableColumn(ColumnNames.TypeId, typeof(Int32)),
            new TableColumn(ColumnNames.NetId, typeof(Int32)),
            new TableColumn(ColumnNames.Stack, typeof(Int32)),
            new TableColumn(ColumnNames.Prefix, typeof(Int32)),
            new TableColumn(ColumnNames.Slot, typeof(Int32))
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

        public static int NewItem(CharacterManager.ItemType type, int netId, int stack, int prefix, int slot, int? userId = null)
        {
            using (var bl = Storage.GetBuilder(CharacterManager.SQLSafeName))
            {
                bl.InsertInto(TableName, 
                    new DataParameter(ColumnNames.TypeId, (int)type),
                    new DataParameter(ColumnNames.NetId, netId),
                    new DataParameter(ColumnNames.Stack, stack),
                    new DataParameter(ColumnNames.Prefix, prefix),
                    new DataParameter(ColumnNames.Slot, slot),
                    new DataParameter(ColumnNames.UserId, userId)
                );

                return (int)Storage.ExecuteInsert(bl); //Get the new ID
            }
        }
    }
}