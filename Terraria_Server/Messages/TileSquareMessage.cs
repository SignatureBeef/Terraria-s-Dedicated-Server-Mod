using System;
using Terraria_Server.Events;
using Terraria_Server.Plugin;
using Terraria_Server.Misc;
using Terraria_Server.Definitions.Tile;
using Terraria_Server.WorldMod;

namespace Terraria_Server.Messages
{
    public class TileSquareMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.TILE_SQUARE;
        }
        
#if IGNORE_TILE_SQUARE
        static long sameTiles = 0;
        static long diffTiles = 0;
#endif

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            short size = BitConverter.ToInt16(readBuffer, start + 1);
            int left = BitConverter.ToInt32(readBuffer, start + 3);
            int top = BitConverter.ToInt32(readBuffer, start + 7);
            num = start + 11;
            var slot = Netplay.slots[whoAmI];
            
#if IGNORE_TILE_SQUARE
            if (size > 7)
            {
                Logging.ProgramLog.Debug.Log ("{0}: Ignoring tile square of size {1}", whoAmI, size);
                return;
            }
            
            bool different = false;
            for (int x = left; x < left + (int)size; x++)
            {
                for (int y = top; y < top + (int)size; y++)
                {
                    TileData tile = Main.tile.At(x, y).Data;

                    byte b9 = readBuffer[num++];

                    bool wasActive = tile.Active;

                    tile.Active = ((b9 & 1) == 1);
                    different |= tile.Active != wasActive;

                    if ((b9 & 2) == 2)
                    {
                        different |= tile.Lighted == false;
                        tile.Lighted = true;
                    }

                    if (tile.Active)
                    {
                        int wasType = (int)tile.Type;
                        tile.Type = readBuffer[num++];
                        
                        different |= tile.Type != wasType;
                        
                        short framex = tile.FrameX;
                        short framey = tile.FrameY;

                        if (tile.Type >= Main.MAX_TILE_SETS)
                        {
                            slot.Kick ("Invalid tile received from client.");
                            return;
                        }
                        
                        if (Main.tileFrameImportant[(int)tile.Type])
                        {
                            framex = BitConverter.ToInt16(readBuffer, num);
                            num += 2;
                            framey = BitConverter.ToInt16(readBuffer, num);
                            num += 2;
                        }
                        else if (!wasActive || (int)tile.Type != wasType)
                        {
                            framex = -1;
                            framey = -1;
                        }
                        
                        different |= (framex != tile.FrameX) || (framey != tile.FrameY);
                    }

                    if ((b9 & 4) == 4)
                    {
                        different |= tile.Wall == 0;
                        different |= tile.Wall != readBuffer[num++];
                    }

                    if ((b9 & 8) == 8)
                    {
                        // TODO: emit a liquid event
                        different |= tile.Liquid != readBuffer[num++];
                        different |= (tile.Lava ? 1 : 0) != readBuffer[num++];
                    }
                    
                    if (different)
                    {
                        break;
                    }
				}
			}
            
            //Logging.ProgramLog.Debug.Log ("TileSquare({0}): {1}", size, different);
            if (different)
            {
                System.Threading.Interlocked.Add (ref diffTiles, size);
                if (size != 3)
                    Logging.ProgramLog.Debug.Log ("{0}: TileSquare({1}): {2:0.0} ({3})", whoAmI, size, diffTiles * 100.0 / (sameTiles + diffTiles), diffTiles);
            }
            else
            {
                System.Threading.Interlocked.Add (ref sameTiles, size);
                //Logging.ProgramLog.Debug.Log ("{0}: same TileSquare({1}): {2:0.0} ({3})", whoAmI, size, diffTiles * 100.0 / (sameTiles + diffTiles), diffTiles);
            }

            if (different) NetMessage.SendTileSquare (whoAmI, left, top, size, false);
            return;
#endif
                        
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

                        if (tile.Type >= Main.MAX_TILE_SETS)
                        {
                            slot.Kick ("Invalid tile received from client.");
                            return;
                        }
                        
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
                        
                        if (tile.Wall >= Main.MAX_WALL_SETS)
                        {
                            slot.Kick ("Invalid tile received from client.");
                            return;
                        }
                    }

                    if (tile.Liquid > 0)
                    {
                        // TODO: emit a liquid event
                        tile.Liquid = readBuffer[num++];
                        byte b10 = readBuffer[num++];
                        tile.Lava = (b10 == 1);
                    }

                    PlayerTileChangeEvent tileEvent = new PlayerTileChangeEvent();
                    tileEvent.Sender = Main.players[whoAmI];
                    tileEvent.Tile = tile;
                    tileEvent.Action = (tile.Active) ? TileAction.PLACED : TileAction.BREAK; //Not sure of this
                    tileEvent.TileType = (tile.wall == 1) ? TileType.WALL : TileType.BLOCK;
                    tileEvent.Position = new Vector2(x, y);
                    Program.server.PluginManager.processHook(Hooks.PLAYER_TILECHANGE, tileEvent);
                    if (tileEvent.Cancelled)
                    {
                        NetMessage.SendTileSquare(whoAmI, x, y, 1);
                        return;
                    }
                    
                    Main.tile.At(x, y).SetData(tile);
                }
            }

            WorldModify.RangeFrame(left, top, left + (int)size, top + (int)size);
            NetMessage.SendData((int)bufferData, -1, whoAmI, "", (int)size, (float)left, (float)top);
        }
    }
}
