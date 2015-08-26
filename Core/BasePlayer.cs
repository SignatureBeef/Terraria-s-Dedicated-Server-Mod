
using System;
using Terraria;

namespace TDSM.Core
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
            player.SetPluginData(LastCostlyCommand, value);
        }

        public static DateTime GetLastCostlyCommand(this Player player)
        {
            return player.GetPluginData(LastCostlyCommand, DateTime.MinValue);
        }
    }
}
