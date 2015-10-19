using System;
using OTA.Plugin;

namespace TDSM.Core.Plugin.Hooks
{
    public static partial class TDSMHookArgs
    {
        public struct TileSquareReceived
        {
            public int X { get; set; }

            public int Y { get; set; }

            public int Size { get; set; }

            public byte[] ReadBuffer;
            public int Start;
            public int Applied;

            //            public void ForEach(object state, TileSquareForEachFunc func)
            //            {
            //                int num = start;
            //
            //                for (int x = X; x < X + Size; x++)
            //                {
            //                    for (int y = Y; y < Y + Size; y++)
            //                    {
            //                        TileData tile = Main.tile.At(x, y).Data;
            //
            //                        byte b9 = readBuffer[num++];
            //
            //                        bool wasActive = tile.Active;
            //
            //                        tile.Active = ((b9 & 1) == 1);
            //
            //                        if ((b9 & 2) == 2)
            //                        {
            //                            tile.Lighted = true;
            //                        }
            //
            //                        if (tile.Active)
            //                        {
            //                            int wasType = (int)tile.Type;
            //                            tile.Type = readBuffer[num++];
            //
            //                            if (tile.Type < Main.MAX_TILE_SETS && Main.tileFrameImportant[(int)tile.Type])
            //                            {
            //                                tile.FrameX = BitConverter.ToInt16(readBuffer, num);
            //                                num += 2;
            //                                tile.FrameY = BitConverter.ToInt16(readBuffer, num);
            //                                num += 2;
            //                            }
            //                            else if (!wasActive || (int)tile.Type != wasType)
            //                            {
            //                                tile.FrameX = -1;
            //                                tile.FrameY = -1;
            //                            }
            //                        }
            //
            //                        if ((b9 & 4) == 4)
            //                            tile.Wall = readBuffer[num++];
            //                        else
            //                            tile.Wall = 0;
            //
            //                        if ((b9 & 8) == 8)
            //                        {
            //                            tile.Liquid = readBuffer[num++];
            //                            byte b10 = readBuffer[num++];
            //                            tile.Lava = (b10 == 1);
            //                        }
            //                        else
            //                            tile.Liquid = 0;
            //
            //                        var result = func(x, y, ref tile, state);
            //                        if (result == TileSquareForEachResult.ACCEPT)
            //                        {
            //                            applied += 1;
            //                            Main.tile.At(x, y).SetData(tile);
            //                        }
            //                        else if (result == TileSquareForEachResult.BREAK)
            //                        {
            //                            return;
            //                        }
            //                    }
            //                }
            //            }
        }
    }

    public static partial class TDSMHookPoints
    {
        public static readonly HookPoint<TDSMHookArgs.TileSquareReceived> TileSquareReceived = new HookPoint<TDSMHookArgs.TileSquareReceived>();
    }
}