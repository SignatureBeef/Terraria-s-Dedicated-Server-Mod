using OTA.Plugin;

namespace TDSM.Core.Plugin.Hooks
{
    public static partial class TDSMHookArgs
    {
        public struct PaintWall
        {
            public int X { get; set; }

            public int Y { get; set; }

            public byte Colour { get; set; }
        }
    }

    public static partial class TDSMHookPoints
    {
        public static readonly HookPoint<TDSMHookArgs.PaintWall> PaintWall = new HookPoint<TDSMHookArgs.PaintWall>();
    }
}