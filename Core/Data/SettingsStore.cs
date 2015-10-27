using System;
using System.Linq;
using OTA.Logging;
using TDSM.Core.Data.Models;
using TDSM.Core.Data.Old;

namespace TDSM.Core.Data
{
#if DATA_CONNECTOR
    /// <summary>
    /// The generic setting store for OTA and its plugins
    /// </summary>
    public static class SettingsStore
    {
        public const String SQLSafeName = "tdsm";

        /// <summary>
        /// The settings table definition
        /// </summary>
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

            /// <summary>
            /// Checks if the table exists
            /// </summary>
            public static bool Exists()
            {
                using (var bl = Storage.GetBuilder(SQLSafeName))
                {
                    bl.TableExists(TableName);

                    return Storage.Execute(bl);
                }
            }

            /// <summary>
            /// Creates the table
            /// </summary>
            public static bool Create()
            {
                using (var bl = Storage.GetBuilder(SQLSafeName))
                {
                    bl.TableCreate(TableName, Columns);

                    return Storage.ExecuteNonQuery(bl) > 0;
                }
            }

            /// <summary>
            /// Determines if a setting exists
            /// </summary>
            /// <param name="key">Key.</param>
            public static bool Exists(string key)
            {
                using (var bl = Storage.GetBuilder(SQLSafeName))
                {
                    bl.SelectAll(TableName, new WhereFilter(ColumnNames.Key, key));

                    return Storage.Execute(bl);
                }
            }

            /// <summary>
            /// Creates a setting
            /// </summary>
            /// <param name="key">Key.</param>
            /// <param name="value">Value.</param>
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

            /// <summary>
            /// Updates a settings value
            /// </summary>
            /// <param name="key">Key.</param>
            /// <param name="value">Value.</param>
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

        /// <summary>
        /// Store a setting
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public static void Set(string key, object value)
        {
            Set(key, value.ToString());
        }

        /// <summary>
        /// Store a setting
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
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

        /// <summary>
        /// Fetches the setting by key
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="key">Key.</param>
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

        /// <summary>
        /// Fetches the int value of a setting
        /// </summary>
        /// <returns>The int.</returns>
        /// <param name="key">Key.</param>
        public static int GetInt(string key)
        {
            return Convert.ToInt32(GetString(key));
        }
    }
#else
    /// <summary>
    /// The generic setting store for OTA and its plugins
    /// </summary>
    public static class SettingsStore
    {
        public const String SQLSafeName = "tdsm";

        /// <summary>
        /// Store a setting
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public static void Set(string key, object value)
        {
            Set(key, value.ToString());
        }

        /// <summary>
        /// Store a setting
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public static void Set(string key, string value)
        {
            using (var ctx = new TContext())
            {
                var item = ctx.Settings.Where(x => x.DataKey == key).SingleOrDefault();
                if (item != null)
                {
                    item.DataValue = value;
                }
                else
                {
                    ctx.Settings.Add(item = new DataSetting()
                        {
                            DataKey = key,
                            DataValue = value
                        });
                }

                ctx.SaveChanges();
            }
        }

        /// <summary>
        /// Fetches the setting by key
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="key">Key.</param>
        public static string GetString(string key)
        {
            using (var ctx = new TContext())
            {
                var stg = ctx.Settings.SingleOrDefault(x => x.DataKey == key);

                if (stg != null) return stg.DataValue;
            }

            return null;
        }

        /// <summary>
        /// Fetches the int value of a setting
        /// </summary>
        /// <returns>The int.</returns>
        /// <param name="key">Key.</param>
        public static int GetInt(string key)
        {
            return Convert.ToInt32(GetString(key));
        }
    }
#endif
}

