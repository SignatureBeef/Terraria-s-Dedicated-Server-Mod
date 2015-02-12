using System;
using Terraria;

namespace tdsm.core
{
    public static class PlayerExtensions
    {
        public static void SetAuthenticatedAs(this Player player, string value)
        {
            const String Key = "AuthenticatedAs";
            /*if (player.PluginData.ContainsKey(Key))
                player.PluginData.Add("AuthenticatedAs", value);
            else*/
            player.PluginData[Key] = value;
        }

        public static string GetAuthenticatedAs(this Player player)
        {
            const String Key = "AuthenticatedAs";
            return player.PluginData[Key] as String;
        }
    }
}
