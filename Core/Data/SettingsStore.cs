using System;
using System.Linq;
using TDSM.Core.Data.Models;

namespace TDSM.Core.Data
{
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
}

