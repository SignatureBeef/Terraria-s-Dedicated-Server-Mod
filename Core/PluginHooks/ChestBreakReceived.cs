using System;
using OTA.Plugin;

namespace TDSM.Core.Plugin.Hooks
{
    public static partial class TDSMHookArgs
    {
        /// <summary>
        /// Chest break received data.
        /// </summary>
        public struct ChestBreakReceived
        {
            /// <summary>
            /// The X coordinate of the chest
            /// </summary>
            public int X { get; set; }

            /// <summary>
            /// The Y coordinate of the chest
            /// </summary>
            public int Y { get; set; }
        }
    }

    public static partial class TDSMHookPoints
    {
        /// <summary>
        /// Triggered when a chest break packet is received.
        /// </summary>
        /// <description>
        /// This hook has multiple states:
        ///     1) IGNORE - disregard the change
        ///     2) RECTIFY - used to ignore the change, but to also refresh the sender with the initial tile square.
        ///     3) KICK - commonly used with HookContext.SetKick, to remove the player from the server.
        /// </description>
        public static readonly HookPoint<TDSMHookArgs.ChestBreakReceived> ChestBreakReceived = new HookPoint<TDSMHookArgs.ChestBreakReceived>();
    }
}