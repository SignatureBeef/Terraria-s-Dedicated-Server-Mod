using System;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class TileSquareMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.TILE_SQUARE;
        }

        //        static long sameTiles = 0;
        //        static long diffTiles = 0;

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            //TODO [TileSquareReceived]

            short num43 = ReadInt16(readBuffer);
            int num44 = (int)ReadInt16(readBuffer);
            int num45 = (int)ReadInt16(readBuffer);
            BitsByte bitsByte6 = 0;
            BitsByte bitsByte7 = 0;
            for (int num46 = num44; num46 < num44 + (int)num43; num46++)
            {
                for (int num47 = num45; num47 < num45 + (int)num43; num47++)
                {
                    if (Main.tile[num46, num47] == null)
                    {
                        Main.tile[num46, num47] = new Tile();
                    }
                    Tile tile = Main.tile[num46, num47];
                    bool flag5 = tile.active();
                    bitsByte6 = ReadByte(readBuffer);
                    bitsByte7 = ReadByte(readBuffer);
                    tile.active(bitsByte6[0]);
                    tile.wall = (byte)(bitsByte6[2] ? 1 : 0);
                    bool flag6 = bitsByte6[3];
                    if (Main.netMode != 2)
                    {
                        tile.liquid = (byte)(flag6 ? 1 : 0);
                    }
                    tile.wire(bitsByte6[4]);
                    tile.halfBrick(bitsByte6[5]);
                    tile.actuator(bitsByte6[6]);
                    tile.inActive(bitsByte6[7]);
                    tile.wire2(bitsByte7[0]);
                    tile.wire3(bitsByte7[1]);
                    if (bitsByte7[2])
                    {
                        tile.color(ReadByte(readBuffer));
                    }
                    if (bitsByte7[3])
                    {
                        tile.wallColor(ReadByte(readBuffer));
                    }
                    if (tile.active())
                    {
                        int type2 = (int)tile.type;
                        tile.type = ReadUInt16(readBuffer);
                        if (Main.tileFrameImportant[(int)tile.type])
                        {
                            tile.frameX = ReadInt16(readBuffer);
                            tile.frameY = ReadInt16(readBuffer);
                        }
                        else
                        {
                            if (!flag5 || (int)tile.type != type2)
                            {
                                tile.frameX = -1;
                                tile.frameY = -1;
                            }
                        }
                        byte b4 = 0;
                        if (bitsByte7[4])
                        {
                            b4 += 1;
                        }
                        if (bitsByte7[5])
                        {
                            b4 += 2;
                        }
                        if (bitsByte7[6])
                        {
                            b4 += 4;
                        }
                        tile.slope(b4);
                    }
                    if (tile.wall > 0)
                    {
                        tile.wall = ReadByte(readBuffer);
                    }
                    if (flag6)
                    {
                        tile.liquid = ReadByte(readBuffer);
                        tile.liquidType((int)ReadByte(readBuffer));
                    }
                }
            }
            WorldGen.RangeFrame(num44, num45, num44 + (int)num43, num45 + (int)num43);
            if (Main.netMode == 2)
            {
                NewNetMessage.SendData((int)Packet.TILE_SQUARE, -1, whoAmI, String.Empty, (int)num43, (float)num44, (float)num45, 0f, 0);
                return;
            }
            return;
        }
    }
}
