using System;
using OTA.Plugin;

namespace TDSM.Core.Plugin.Hooks
{
    public static partial class TDSMHookArgs
    {
        /// <summary>
        /// Chest open received data.
        /// </summary>
        public struct ChestOpenReceived
        {
            /// <summary>
            /// X coordinate of the chest
            /// </summary>
            public int X { get; set; }

            /// <summary>
            /// Y coordinate of the chest
            /// </summary>
            public int Y { get; set; }

            /// <summary>
            /// The chest array index that the user currently has open
            /// </summary>
            /// <seealso cref="Terraria.Player.chest"/>
            public int ChestIndex { get; set; }
        }
    }

    public static partial class TDSMHookPoints
    {
        /// <summary>
        /// Triggered when a player opens a chest (via the ChestOpen packet)
        /// </summary>
        /// <description>
        /// This hook accepts multiple HookContext.Result values:
        ///     1) KICK - commonly used with HookContext.SetKick, to remove the player from the server
        ///     2) IGNORE - to disregard and not to send the contents of the chest
        /// </description>
        public static readonly HookPoint<TDSMHookArgs.ChestOpenReceived> ChestOpenReceived = new HookPoint<TDSMHookArgs.ChestOpenReceived>();
    }
}