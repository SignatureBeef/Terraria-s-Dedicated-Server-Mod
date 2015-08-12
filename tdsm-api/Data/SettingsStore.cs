using System;
using TDSM.API.Logging;

namespace TDSM.API.Data
{
    public static class SettingsStore
    {
        public const String SQLSafeName = "tdsm";

        public class SettingsTable
        {
            public const String TableName = "settings";

            public static class ColumnNames
            {
                public const String Id = "Id";
                public const String Key = "StoreKey";
                public const String Value = "StoreValue";
            }

            public static readonly TableColumn[] Columns = new TableColumn[]
            {
                new TableColumn(ColumnNames.Id, typeof(Int32), true, true),
                new TableColumn(ColumnNames.Key, typeof(String), 255),
                new TableColumn(ColumnNames.Value, typeof(String), 255)
            };

            public static bool Exists()
            {
                using (var bl = Storage.GetBuilder(SQLSafeName))
                {
                    bl.TableExists(TableName);

                    return Storage.Execute(bl);
                }
            }

            public static bool Create()
            {
                using (var bl = Storage.GetBuilder(SQLSafeName))
                {
                    bl.TableCreate(TableName, Columns);

                    return Storage.ExecuteNonQuery(bl) > 0;
                }
            }

            public static bool Exists(string key)
            {
                using (var bl = Storage.GetBuilder(SQLSafeName))
                {
                    bl.SelectAll(TableName, new WhereFilter(ColumnNames.Key, key));

                    return Storage.Execute(bl);
                }
            }

            public static int Insert(string key, string value)
            {
                using (var bl = Storage.GetBuilder(SQLSafeName))
                {
                    bl.InsertInto(TableName,
                        new DataParameter(ColumnNames.Key, key),
                        new DataParameter(ColumnNames.Value, value)
                    );

                    return (int)Storage.ExecuteInsert(bl);
                }
            }

            public static bool Update(string key, string value)
            {
                using (var bl = Storage.GetBuilder(SQLSafeName))
                {
                    bl.Update(TableName,
                        new DataParameter[]
                        {
                            new DataParameter(ColumnNames.Value, value)
                        },
                        new WhereFilter(ColumnNames.Key, key)
                    );

                    return Storage.ExecuteNonQuery(bl) > 0;
                }
            }
        }

        internal static void Initialise()
        {
            if (!SettingsTable.Exists())
            {
                ProgramLog.Admin.Log("Common settings table does not exist and will now be created");
                SettingsTable.Create();
            }
        }

        public static void Set(string key, object value)
        {
            Set(key, value.ToString());
        }

        public static void Set(string key, string value)
        {
            if (SettingsTable.Exists(key))
            {
                SettingsTable.Update(key, value);
            }
            else
            {
                SettingsTable.Insert(key, value);
            }
        }

        public static string GetString(string key)
        {
            using (var bl = Storage.GetBuilder(SQLSafeName))
            {
                bl.SelectFrom(SettingsTable.TableName, new string[] { SettingsTable.ColumnNames.Value },
                    new WhereFilter(SettingsTable.ColumnNames.Key, key)
                );

                return Storage.ExecuteScalar<String>(bl);
            }
        }

        public static int GetInt(string key)
        {
            return Convert.ToInt32(GetString(key));
        }
    }
}