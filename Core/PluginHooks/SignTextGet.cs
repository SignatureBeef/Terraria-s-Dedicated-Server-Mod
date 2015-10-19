using System;
using OTA.Plugin;

namespace TDSM.Core.Plugin.Hooks
{
    public static partial class TDSMHookArgs
    {
        public struct SignTextGet
        {
            public int X { get; set; }

            public int Y { get; set; }

            public int SignIndex { get; set; }

            public string Text { get; set; }
        }
    }

    public static partial class TDSMHookPoints
    {
        public static readonly HookPoint<TDSMHookArgs.SignTextGet> SignTextGet = new HookPoint<TDSMHookArgs.SignTextGet>();
    }
}