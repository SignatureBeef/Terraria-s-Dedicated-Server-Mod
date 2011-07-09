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
                    Tile tile = (Tile)Main.tile[x, y].Clone();

                    byte b9 = readBuffer[num++];

                    bool wasActive = tile.Active;

                    tile.Active = ((b9 & 1) == 1);

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

                    if (tile.Active)
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
                    
                    Main.tile[x, y] = tile;
                }
            }

            WorldGen.RangeFrame(left, top, left + (int)size, top + (int)size);
            NetMessage.SendData((int)bufferData, -1, whoAmI, "", (int)size, (float)left, (float)top);
        }
    }
}
