using System;
using OTA.Data;
using TDSM.Core.ServerCharacters;

namespace TDSM.Core.ServerCharacters.Tables
{
//    internal class PlayerBuffTable
//    {
//        public const String TableName = "SSC_PlayerBuff";
//
//        static class ColumnNames
//        {
//            public const String Id = "Id";
//            public const String UserId = "UserId";
//            public const String BuffType = "BuffType";
//            public const String Duration = "Duration";
//            public const String Slot = "Slot";
//        }
//
//        public static readonly TableColumn[] Columns = new TableColumn[]
//        {
//            new TableColumn(ColumnNames.Id, typeof(Int32), true, true),
//            new TableColumn(ColumnNames.UserId, typeof(Int32)),
//            new TableColumn(ColumnNames.BuffType, typeof(Int32)),
//            new TableColumn(ColumnNames.Duration, typeof(Int32)),
//            new TableColumn(ColumnNames.Slot, typeof(Int32))
//        };
//
//        public static bool Exists()
//        {
//            using (var bl = Storage.GetBuilder(CharacterManager.SQLSafeName))
//            {
//                bl.TableExists(TableName);
//
//                return Storage.Execute(bl);
//            }
//        }
//
//        public static bool Create()
//        {
//            using (var bl = Storage.GetBuilder(CharacterManager.SQLSafeName))
//            {
//                bl.TableCreate(TableName, Columns);
//
//                return Storage.ExecuteNonQuery(bl) > 0;
//            }
//        }
//    }
}