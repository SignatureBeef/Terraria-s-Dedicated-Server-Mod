using OTA.Plugin;

namespace TDSM.Core.Plugin.Hooks
{
    public static partial class TDSMHookArgs
    {
        public struct PaintTile
        {
            public int X { get; set; }

            public int Y { get; set; }

            public byte Colour { get; set; }
        }
    }

    public static partial class TDSMHookPoints
    {
        public static readonly HookPoint<TDSMHookArgs.PaintTile> PaintTile = new HookPoint<TDSMHookArgs.PaintTile>();
    }
}