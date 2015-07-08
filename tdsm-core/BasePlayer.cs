
using System;
using Terraria;

namespace tdsm.core
{
    public static class PlayerExtensions
    {
        private const String LastCostlyCommand = "LastCostlyCommand";

        //public static void SetAuthenticatedAs(this Player player, string value)
        //{
        //    const String Key = "AuthenticatedAs";
        //    /*if (player.PluginData.ContainsKey(Key))
        //        player.PluginData.Add("AuthenticatedAs", value);
        //    else*/
        //    if (player.PluginData == null) player.PluginData = new System.Collections.Hashtable();
        //    player.PluginData[Key] = value;
        //}

        //public static string GetAuthenticatedAs(this Player player)
        //{
        //    if (player.PluginData == null) player.PluginData = new System.Collections.Hashtable();

        //    const String Key = "AuthenticatedAs";
        //    return player.PluginData[Key] as String;
        //}

        public static void SetLastCostlyCommand(this Player player, DateTime value)
        {
            if (player.PluginData == null)
                player.PluginData = new System.Collections.Concurrent.ConcurrentDictionary<string, object>();
            player.PluginData[LastCostlyCommand] = value;
        }

        public static DateTime GetLastCostlyCommand(this Player player)
        {
            if (player.PluginData == null)
                player.PluginData = new System.Collections.Concurrent.ConcurrentDictionary<string, object>();

            if (player.PluginData.ContainsKey(LastCostlyCommand))
            {
                return (DateTime)player.PluginData[LastCostlyCommand];
            }
            return DateTime.MinValue;
        }
    }
}
