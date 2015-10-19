using System;
using OTA.Plugin;

namespace TDSM.Core.Plugin.Hooks
{
    public static partial class TDSMHookArgs
    {
        /// <summary>
        /// Player authentication changed.
        /// </summary>
        public struct PlayerAuthenticationChanged
        {
            /// <summary>
            /// The authentication name the player is now known by
            /// </summary>
            public string AuthenticatedAs { get; set; }

            /// <summary>
            /// The source from which the player was authenticated by
            /// </summary>
            public string AuthenticatedBy { get; set; }
        }
    }

    public static partial class TDSMHookPoints
    {
        /// <summary>
        /// Occurs when a players authenticationg has just changed
        /// </summary>
        public static readonly HookPoint<TDSMHookArgs.PlayerAuthenticationChanged> PlayerAuthenticationChanged = new HookPoint<TDSMHookArgs.PlayerAuthenticationChanged>();
    }
}