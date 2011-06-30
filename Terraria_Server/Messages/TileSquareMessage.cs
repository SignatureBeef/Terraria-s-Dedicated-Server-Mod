using System;

namespace Terraria_Server.Messages
{
    public class TileSquareMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.TILE_SQUARE;
        }

        public int? GetRequiredNetMode()
        {
            return null;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            short size = BitConverter.ToInt16(readBuffer, start + 1);
            int left = BitConverter.ToInt32(readBuffer, start + 3);
            int top = BitConverter.ToInt32(readBuffer, start + 7);
            num = start + 11;
            for (int x = left; x < left + (int)size; x++)
            {
                for (int y = top; y < top + (int)size; y++)
                {
                    if (Main.tile[x, y] == null)
                    {
                        Main.tile[x, y] = new Tile();
                    }
                    Tile tile = Main.tile[x, y];

                    byte b9 = readBuffer[num++];

                    bool wasActive = tile.active;

                    tile.active = ((b9 & 1) == 1);

                    if ((b9 & 2) == 2)
                    {
                        tile.lighted = true;
                    }

                    if ((b9 & 4) == 4)
                    {
                        tile.wall = 1;
                    }
                    else
                    {
                        tile.wall = 0;
                    }

                    if ((b9 & 8) == 8)
                    {
                        tile.liquid = 1;
                    }
                    else
                    {
                        tile.liquid = 0;
                    }

                    if (tile.active)
                    {
                        int wasType = (int)tile.type;
                        tile.type = readBuffer[num++];
                        if (Main.tileFrameImportant[(int)tile.type])
                        {
                            tile.frameX = BitConverter.ToInt16(readBuffer, num);
                            num += 2;
                            tile.frameY = BitConverter.ToInt16(readBuffer, num);
                            num += 2;
                        }
                        else if (!wasActive || (int)tile.type != wasType)
                        {
                            tile.frameX = -1;
                            tile.frameY = -1;
                        }
                    }

                    if (tile.wall > 0)
                    {
                        tile.wall = readBuffer[num++];
                    }

                    if (tile.liquid > 0)
                    {
                        tile.liquid = readBuffer[num++];
                        byte b10 = readBuffer[num++];
                        tile.lava = (b10 == 1);
                    }
                }
            }

            WorldGen.RangeFrame(left, top, left + (int)size, top + (int)size);
            if (Main.netMode == 2)
            {
                NetMessage.SendData((int)bufferData, -1, whoAmI, "", (int)size, (float)left, (float)top);
            }
        }
    }
}
