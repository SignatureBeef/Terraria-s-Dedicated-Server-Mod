
using System;
using Terraria;
using OTA.Plugin;
using OTA;
using TDSM.Core.Plugin.Hooks;

namespace TDSM.Core
{
    public static class PlayerExtensions
    {
        private const String LastCostlyCommand = "LastCostlyCommand";
        private const String SSCReadyForSave = "SSCReadyForSave";
        private const String AuthenticatedAs = "AuthenticatedAs";
        private const String AuthenticatedBy = "AuthenticatedBy";

        /// <summary>
        /// Gets a value indicating whether this player is authenticated.
        /// </summary>
        /// <value><c>true</c> if this instance is authenticated; otherwise, <c>false</c>.</value>
        public static bool IsAuthenticated(this BasePlayer player)
        {
            return !String.IsNullOrEmpty(player.GetAuthenticatedAs()) && player.GetAuthenticatedBy() != player.name; 
        }

        /// <summary>
        /// Gets a value indicating whether this player is self authenticated.
        /// </summary>
        /// <remarks>This is not for real authentication, rather to prevent self authentications being included</remarks>
        public static bool IsSelfAuthenticated(this BasePlayer player)
        {
            return player.GetAuthenticatedBy() == player.name;
        }

        /// <summary>
        /// Sets who the player is authenticated as
        /// </summary>
        internal static void SetAuthenticatedAs(this BasePlayer player, string value)
        {
            player.SetPluginData(AuthenticatedAs, value);
        }

        /// <summary>
        /// Gets who the player is authenticated as
        /// </summary>
        /// <value>The authenticated as.</value>
        public static string GetAuthenticatedAs(this BasePlayer player)
        {
            return player.GetPluginData(AuthenticatedAs, String.Empty);
        }

        /// <summary>
        /// Sets the authenticating service
        /// </summary>
        internal static void SetAuthenticatedBy(this BasePlayer player, string value)
        {
            player.SetPluginData(AuthenticatedBy, value);
        }

        /// <summary>
        /// Gets the authenticating service
        /// </summary>
        public static string GetAuthenticatedBy(this BasePlayer player)
        {
            return player.GetPluginData(AuthenticatedBy, String.Empty);
        }

        public static void SetLastCostlyCommand(this BasePlayer player, DateTime value)
        {
            player.SetPluginData(LastCostlyCommand, value);
        }

        public static DateTime GetLastCostlyCommand(this BasePlayer player)
        {
            return player.GetPluginData(LastCostlyCommand, DateTime.MinValue);
        }

        public static void SetSSCReadyForSave(this BasePlayer player, bool value)
        {
            player.SetPluginData(SSCReadyForSave, value);
        }

        public static bool GetSSCReadyForSave(this BasePlayer player)
        {
            return player.GetPluginData(SSCReadyForSave, false);
        }

        /// <summary>
        /// Sets the authentication of this user
        /// </summary>
        /// <param name="auth">Auth.</param>
        /// <param name="by">By.</param>
        public static void SetAuthentication(this BasePlayer player, string auth, string by)
        {
            var ctx = new HookContext()
            {
                Player = (Terraria.Player)player,
                Connection = player.Connection.Socket
            };
            var changing = new TDSMHookArgs.PlayerAuthenticationChanging()
            {
                AuthenticatedAs = auth,
                AuthenticatedBy = by
            };
            TDSMHookPoints.PlayerAuthenticationChanging.Invoke(ref ctx, ref changing);
            if (ctx.Result != HookResult.DEFAULT)
                return;

            player.SetAuthenticatedAs(auth);
            player.SetAuthenticatedBy(by);

            ctx = new HookContext()
            {
                Player = (Terraria.Player)player,
                Connection = player.Connection.Socket
            };
            var changed = new TDSMHookArgs.PlayerAuthenticationChanged()
            {
                AuthenticatedAs = auth,
                AuthenticatedBy = by
            };

            TDSMHookPoints.PlayerAuthenticationChanged.Invoke(ref ctx, ref changed);
        }
    }
}
