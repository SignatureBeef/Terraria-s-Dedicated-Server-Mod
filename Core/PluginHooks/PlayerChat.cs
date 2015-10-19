using System;
using OTA.Plugin;

namespace TDSM.Core.Plugin.Hooks
{
    public static partial class TDSMHookArgs
    {
        public struct PlayerChat
        {
            public string Message { get; set; }

            public Microsoft.Xna.Framework.Color Color { get; set; }
        }
    }

    public static partial class TDSMHookPoints
    {
        public static readonly HookPoint<TDSMHookArgs.PlayerChat> PlayerChat = new HookPoint<TDSMHookArgs.PlayerChat>();
    }
}