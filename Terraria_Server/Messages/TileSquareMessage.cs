using System;
using Terraria_Server.Events;
using Terraria_Server.Plugin;
using Terraria_Server.Misc;
using Terraria_Server.Definitions.Tile;

namespace Terraria_Server.Messages
{
    public class TileSquareMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.TILE_SQUARE;
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
                    TileData tile = Main.tile.At(x, y).Data;

                    byte b9 = readBuffer[num++];

                    bool wasActive = tile.Active;

                    tile.Active = ((b9 & 1) == 1);

                    if ((b9 & 2) == 2)
                    {
                        tile.Lighted = true;
                    }

                    if ((b9 & 4) == 4)
                    {
                        tile.Wall = 1;
                    }
                    else
                    {
                        tile.Wall = 0;
                    }

                    if ((b9 & 8) == 8)
                    {
                        tile.Liquid = 1;
                    }
                    else
                    {
                        tile.Liquid = 0;
                    }

                    if (tile.Active)
                    {
                        int wasType = (int)tile.Type;
                        tile.Type = readBuffer[num++];
                        if (Main.tileFrameImportant[(int)tile.Type])
                        {
                            tile.FrameX = BitConverter.ToInt16(readBuffer, num);
                            num += 2;
                            tile.FrameY = BitConverter.ToInt16(readBuffer, num);
                            num += 2;
                        }
                        else if (!wasActive || (int)tile.Type != wasType)
                        {
                            tile.FrameX = -1;
                            tile.FrameY = -1;
                        }
                    }

                    if (tile.Wall > 0)
                    {
                        tile.Wall = readBuffer[num++];
                    }

                    if (tile.Liquid > 0)
                    {
                        tile.Liquid = readBuffer[num++];
                        byte b10 = readBuffer[num++];
                        tile.Lava = (b10 == 1);
                    }

                    PlayerTileChangeEvent tileEvent = new PlayerTileChangeEvent();
                    tileEvent.Sender = Main.players[whoAmI];
                    tileEvent.Tile = tile;
                    tileEvent.Type = tile.type;
                    tileEvent.Action = (tile.Active) ? TileAction.PLACED : TileAction.BREAK; //Not sure of this
                    tileEvent.TileType = (tile.wall == 1) ? TileType.WALL : TileType.BLOCK;
                    tileEvent.Position = new Vector2(x, y);
                    Program.server.getPluginManager().processHook(Hooks.PLAYER_TILECHANGE, tileEvent);
                    if (tileEvent.Cancelled)
                    {
                        NetMessage.SendTileSquare(whoAmI, x, y, 1);
                        return;
                    }
                    
                    Main.tile.At(x, y).SetData (tile);
                }
            }

            WorldGen.RangeFrame(left, top, left + (int)size, top + (int)size);
            NetMessage.SendData((int)bufferData, -1, whoAmI, "", (int)size, (float)left, (float)top);
        }
    }
}
