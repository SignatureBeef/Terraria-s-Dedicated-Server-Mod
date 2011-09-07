using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Events;
using Terraria_Server.Misc;
using Terraria_Server.Plugin;
using Terraria_Server.Definitions.Tile;
using Terraria_Server.WorldMod;

namespace Terraria_Server.Messages
{
    public class TileBreakMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.TILE_BREAK;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            var slot = NetPlay.slots[whoAmI];
            
            byte tileAction = readBuffer[num++];
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            num += 4;

            byte tileType = readBuffer[num++];
            int style = (int)readBuffer[num];
            bool failFlag = (tileType == 1);

            bool placed = false;
            bool wall = false;
            
            //Logging.ProgramLog.Debug.Log ("TileBreak({0}s, {4}a, {1}t) @ {2}, {3}", style, tileType, x, y, tileAction);
			
			if (x < 0 || y < 0 || x >= Main.maxTilesX || y >= Main.maxTilesY)
			{
				slot.Kick ("Invalid tile received from client.");
				return;
			}
			
			var tile = Main.tile.At(x, y).Data;
			
            switch (tileAction)
            {
                case 1:
                    if (tileType >= Main.MAX_TILE_SETS)
                    {
                        slot.Kick ("Invalid tile received from client.");
                        return;
                    }
                    
                    tile.Type = tileType;

                    placed = true;
                    break;
                case 2:
                    wall = true;
                    break;
                case 3:
                    if (tileType >= Main.MAX_WALL_SETS)
                    {
                        slot.Kick ("Invalid tile received from client.");
                        return;
                    }

                    wall = true;
                    placed = true;
                    
                    tile.Wall = tileType;
                    break;
            }
            
            var player = Main.players[whoAmI];

            PlayerTileChangeEvent tileEvent = new PlayerTileChangeEvent();
            tileEvent.Sender = player;
            tileEvent.Tile = tile;
            tileEvent.Action = (placed) ? TileAction.PLACED : TileAction.BREAK;
            tileEvent.TileType = (wall) ? TileType.WALL : TileType.BLOCK;
            tileEvent.Position = new Vector2(x, y);
            Server.PluginManager.processHook(Hooks.PLAYER_TILECHANGE, tileEvent);
            if (tileEvent.Cancelled)
            {
                //if (!Main.tileFrameImportant[tile.Type])
                    NetMessage.SendTileSquare(whoAmI, x, y, 1);
                //else
                    //Insert some method to handle annoying data here
                return;
            }
           
			if (!failFlag)
			{
				if (tileAction == 0 || tileAction == 2 || tileAction == 4)
				{
					NetPlay.slots[whoAmI].spamDelBlock += 1f;
				}
				else if (tileAction == 1 || tileAction == 3)
				{
					NetPlay.slots[whoAmI].spamAddBlock += 1f;
				}
			}

			if (!NetPlay.slots[whoAmI].tileSection[NetPlay.GetSectionX(x), NetPlay.GetSectionY(y)])
			{
				failFlag = true;
			}
			
			lock (WorldModify.playerEditLock)
            switch (tileAction)
            {
                case 0:
                    WorldModify.KillTile(x, y, failFlag, false, false);
                    break;
                case 1:
                    if (WorldModify.PlaceTile(x, y, (int)tileType, false, true, whoAmI, style))
					{
						if (tileType == 15 && player.direction == 1)
						{
							Main.tile.At(x, y).AddFrameX (18);
							Main.tile.At(x, y - 1).AddFrameX (18);
						}
						else if (tileType == 106)
						{
							WorldModify.SquareTileFrame (x, y, true);
						}
					}
                    
                    break;
                case 2:
                    WorldModify.KillWall(x, y, failFlag);
                    break;
                case 3:
                    WorldModify.PlaceWall(x, y, (int)tileType, false);
                    break;
                case 4:
                    WorldModify.KillTile(x, y, failFlag, false, true);
                    break;
            }

            NetMessage.SendData(17, -1, whoAmI, "", (int)tileAction, (float)x, (float)y, (float)tileType, style);
            if (tileAction == 1 && tileType == 53)
            {
                NetMessage.SendTileSquare(-1, x, y, 1);
            }
        }
    }
}
