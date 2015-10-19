using System;
using OTA.Plugin;

namespace TDSM.Core.Plugin.Hooks
{
    public static partial class TDSMHookArgs
    {
        /// <summary>
        /// Barrier state changed data.
        /// </summary>
        public struct BarrierStateChange
        {
            /// <summary>
            /// Location of the player when the state was changed
            /// </summary>
            public Microsoft.Xna.Framework.Vector2 Position { get; set; }

            /// <summary>
            /// X coordinate
            /// </summary>
            public int X { get; set; }

            /// <summary>
            /// Y Coordinate
            /// </summary>
            public int Y { get; set; }

            /// <summary>
            /// The direction of the barrier
            /// </summary>
            public int Direction { get; set; }

            /// <summary>
            /// Type of barrier being opened
            /// </summary>
            public int Kind { get; set; }
        }
    }

    public static partial class TDSMHookPoints
    {
        /// <summary>
        /// Triggered when a player opens or closes a barrier (door, trapdoor)
        /// </summary>
        /// <description>
        /// This hook accepts HookContext.Result values of:
        ///     1) DEFAULT - to perform vanilla actions
        ///     2) RECTIFY - ignores changes, and reupdates the client
        ///     3) Anything else is considered as IGNORE, and is actionless.
        /// </description>
        public static readonly HookPoint<TDSMHookArgs.BarrierStateChange> BarrierStateChange = new HookPoint<TDSMHookArgs.BarrierStateChange>();
    }
}