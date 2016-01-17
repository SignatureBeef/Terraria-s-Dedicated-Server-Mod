using System;
using Microsoft.Xna.Framework;
using TDSM.Core.Command;
using OTA.Command;
using TDSM.Core.RemoteConsole;

namespace OTA
{
    /// <summary>
    /// BasePlayer extensions
    /// </summary>
    public static class BasePlayerExtensions
    {
        private const String Key_Op = "tdsm_operator";

        /// <summary>
        /// Determines if the player is an operator
        /// </summary>
        public static bool IsOp(this BasePlayer player)
        {
            return player.GetPluginData(Key_Op, false);
        }

        /// <summary>
        /// Update operator status for the player
        /// </summary>
        public static void SetOp(this BasePlayer player, bool op)
        {
            player.SetPluginData(Key_Op, op);
        }
    }
}

