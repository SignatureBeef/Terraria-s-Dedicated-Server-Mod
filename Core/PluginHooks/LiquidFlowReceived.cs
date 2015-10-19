using System;
using OTA.Plugin;

namespace TDSM.Core.Plugin.Hooks
{
    public static partial class TDSMHookArgs
    {
        public struct LiquidFlowReceived
        {
            public int X { get; set; }

            public int Y { get; set; }

            public byte Amount { get; set; }

            public bool Lava { get; set; }

            public bool Water
            {
                get { return !Lava; }
                set { Lava = !value; }
            }
        }
    }

    public static partial class TDSMHookPoints
    {
        public static readonly HookPoint<TDSMHookArgs.LiquidFlowReceived> LiquidFlowReceived = new HookPoint<TDSMHookArgs.LiquidFlowReceived>();
    }
}