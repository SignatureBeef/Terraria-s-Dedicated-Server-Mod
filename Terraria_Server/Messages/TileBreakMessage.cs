using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Events;
using Terraria_Server.Misc;
using Terraria_Server.Plugins;
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
            var slot = Netplay.slots[whoAmI];
            
            byte tileAction = readBuffer[num++];
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            num += 4;

            byte tileType = readBuffer[num++];
            byte style = readBuffer[num];
            bool failFlag = (tileType == 1);
			
			if (x < 0 || y < 0 || x >= Main.maxTilesX || y >= Main.maxTilesY)
			{
				slot.Kick ("Out of range tile received from client.");
				return;
			}
			
			var player = Main.players[whoAmI];
			
			var ctx = new HookContext
			{
				Connection = Netplay.slots[whoAmI].conn,
				Sender = player,
				Player = player,
			};
			
			var args = new HookArgs.TileChangeReceived
			{
				X = x, Y = y,
				Action = tileAction,
				Type = tileType,
				Style = style,
			};
			
			HookPoints.TileChangeReceived.Invoke (ref ctx, ref args);
			
			if (ctx.CheckForKick ())
				return;
				
			if (ctx.Result == HookResult.IGNORE)
				return;
			
			if (ctx.Result == HookResult.RECTIFY)
			{
				NetMessage.SendTileSquare(whoAmI, x, y, 1); // FIXME
				return;
			}
			
			if (! args.TypeChecked)
			{
				switch (tileAction)
				{
					case 1:
					case 4:
						if (tileType >= Main.MAX_TILE_SETS)
						{
							slot.Kick ("Invalid tile type received from client.");
							return;
						}
						break;
					
					case 3:
						if (tileType >= Main.MAX_WALL_SETS)
						{
							slot.Kick ("Invalid wall type received from client.");
							return;
						}
						break;
				}
			}
			
			if (!failFlag)
			{
				if (tileAction == 0 || tileAction == 2 || tileAction == 4)
				{
					Netplay.slots[whoAmI].spamDelBlock += 1f;
				}
				else if (tileAction == 1 || tileAction == 3)
				{
					Netplay.slots[whoAmI].spamAddBlock += 1f;
				}
			}

			if (!Netplay.slots[whoAmI].tileSection[Netplay.GetSectionX(x), Netplay.GetSectionY(y)])
			{
				failFlag = true;
			}
			
			lock (WorldModify.playerEditLock)
			{
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
			}

            NetMessage.SendData(17, -1, whoAmI, "", (int)tileAction, (float)x, (float)y, (float)tileType, style);
            if (tileAction == 1 && tileType == 53)
            {
                NetMessage.SendTileSquare(-1, x, y, 1);
            }
        }
    }
}
