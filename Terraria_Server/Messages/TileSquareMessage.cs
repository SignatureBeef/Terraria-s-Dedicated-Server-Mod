using System;
using Terraria_Server.Plugins;
using Terraria_Server.Misc;
using Terraria_Server.WorldMod;

namespace Terraria_Server.Messages
{
    public class TileSquareMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.TILE_SQUARE;
        }
        
//        static long sameTiles = 0;
//        static long diffTiles = 0;

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            short size = BitConverter.ToInt16(readBuffer, num);
            int left = BitConverter.ToInt32(readBuffer, num + 2);
            int top = BitConverter.ToInt32(readBuffer, num + 6);
            num += 10;
            var slot = NetPlay.slots[whoAmI];
            
			var start = num;
			
			var setting = Program.properties.TileSquareMessages;
			if (setting == "rectify")
			{
				if (size > 7)
				{
					Logging.ProgramLog.Debug.Log ("{0}: Ignoring tile square of size {1}", whoAmI, size);
					return;
				}
				
				//Logging.ProgramLog.Debug.Log ("{0}: TILE_SQUARE at {1}, {2}", whoAmI, left, top);
				
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
						
						tile.Wire = (b9 & 16) == 16;
						
						if (different)
						{
							break;
						}
					}
				}
				
				//Logging.ProgramLog.Debug.Log ("TileSquare({0}): {1}", size, different);
//				if (different)
//				{
//					System.Threading.Interlocked.Add (ref diffTiles, size);
//					if (size != 3)
//						Logging.ProgramLog.Debug.Log ("{0}: TileSquare({1}): {2:0.0} ({3})", whoAmI, size, diffTiles * 100.0 / (sameTiles + diffTiles), diffTiles);
//				}
//				else
//				{
//					System.Threading.Interlocked.Add (ref sameTiles, size);
//					//Logging.ProgramLog.Debug.Log ("{0}: same TileSquare({1}): {2:0.0} ({3})", whoAmI, size, diffTiles * 100.0 / (sameTiles + diffTiles), diffTiles);
//				}

				if (different) NetMessage.SendTileSquare (whoAmI, left, top, size, false);
				return;
			}
			else if (setting == "ignore")
			{
				//if (size == 1) Logging.ProgramLog.Debug.Log ("{0}: TileSquare({1}) from {2}", whoAmI, size, Main.players[whoAmI].Name);
				return;
			}
			
			var player = Main.players[whoAmI];
			
			var ctx = new HookContext
			{
				Sender = player,
				Player = player,
				Connection = player.Connection
			};
			
			var args = new HookArgs.TileSquareReceived
			{
				X = left, Y = top, Size = size,
				readBuffer = readBuffer,
				start = start,
			};
			
			HookPoints.TileSquareReceived.Invoke (ref ctx, ref args);
			
			if (args.applied > 0)
			{
				WorldModify.RangeFrame(null, null, left, top, left + (int)size, top + (int)size);
				NetMessage.SendData(Packet.TILE_SQUARE, -1, whoAmI, "", (int)size, (float)left, (float)top);
			}
			
			if (ctx.CheckForKick() || ctx.Result == HookResult.IGNORE)
				return;
			
			args.ForEach (player, this.EachTile);
		}
		
		TileSquareForEachResult EachTile (int x, int y, ref TileData tile, object state)
		{
			if ((tile.Active && tile.Type >= Main.MAX_TILE_SETS) || (tile.Wall >= Main.MAX_WALL_SETS))
				return TileSquareForEachResult.IGNORE;
			return TileSquareForEachResult.ACCEPT;
		}
    }
}
