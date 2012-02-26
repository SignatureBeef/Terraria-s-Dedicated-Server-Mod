using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Misc;
using Terraria_Server.Plugins;
using Terraria_Server.WorldMod;
using Terraria_Server.Logging;

namespace Terraria_Server.Messages
{
    public class TileBreakMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.TILE_BREAK;
        }
						
		public static SandboxEditor<PlayerSandbox> staticEditor = new SandboxEditor<PlayerSandbox> (new PlayerSandbox ());
		
        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            var slot = NetPlay.slots[whoAmI];
            
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
			
			if (! NetPlay.slots[whoAmI].tileSection[NetPlay.GetSectionX(x), NetPlay.GetSectionY(y)])
			{
				ProgramLog.Debug.Log ("{0} @ {1}: {2} attempted to alter world in unloaded tile.");
				return;
			}
			
			var editor = staticEditor; //new SandboxEditor<PlayerSandbox> ();
			var sandbox = editor.Sandbox;
			
			lock (WorldModify.playerEditLock)
			{
				sandbox.Initialize ();
				var player = Main.players[whoAmI];
				
				switch (tileAction)
				{
					case 0:
						editor.KillTile(x, y, failFlag);
						//[TODO] Get block modifications outside the x,y axis to update on Client end
						//WorldModify.KillTile(null, x, y, failFlag);
						break;						
					case 1:
						if (editor.PlaceTile(x, y, (int)tileType, false, true, whoAmI, style))
						{
							if (tileType == 15 && player.direction == 1)
							{
								sandbox.SetFrameXAt (x, y, (short)(sandbox.FrameXAt (x, y) + 18));
								sandbox.SetFrameXAt (x, y - 1, (short)(sandbox.FrameXAt (x, y - 1) + 18));
							}
							else if (tileType == 106)
							{
								editor.SquareTileFrame (x, y, true);
							}
						}
						break;						
					case 2:
						editor.KillWall(x, y, failFlag);
						break;						
					case 3:
						editor.PlaceWall(x, y, (int)tileType, false);
						break;						
					case 4:
						editor.KillTile(x, y, failFlag, false, true);
						break;
					case 5:
						editor.PlaceWire(x, y);
						break;
					case 6:
						editor.KillWire(x, y);
						break;
				}
				
//				if (sandbox.ChangedTileCount == 0)
//					return;
				
				var ctx = new HookContext
				{
					Connection = NetPlay.slots[whoAmI].conn,
					Sender = player,
					Player = player,
				};
				
				var args = new HookArgs.PlayerWorldAlteration
				{
					X = x, Y = y,
					Action = tileAction,
					Type = tileType,
					Style = style,
					Sandbox = sandbox,
				};
				
				HookPoints.PlayerWorldAlteration.Invoke (ref ctx, ref args);
				
				if (ctx.CheckForKick ())
					return;
					
				if (ctx.Result == HookResult.IGNORE)
					return;
				
				if (ctx.Result != HookResult.RECTIFY)
				{
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
							NetPlay.slots[whoAmI].spamDelBlock += 1f;
						}
						else if (tileAction == 1 || tileAction == 3)
						{
							NetPlay.slots[whoAmI].spamAddBlock += 1f;
						}
					}
					
					NetMessage.SendData(17, -1, whoAmI, "", (int)tileAction, (float)x, (float)y, (float)tileType, style);
					
					sandbox.Apply ();
					
					return;
				}

				if (player.rowsToRectify.Count > 0 || sandbox.changedRows.Count > 0)
				{
					lock (player.rowsToRectify)
					{
						foreach (var kv in sandbox.changedRows)
						{
							int y0 = kv.Key;
							var x1 = kv.Value.Min;
							var x2 = kv.Value.Max;
							uint row;
							if (player.rowsToRectify.TryGetValue((ushort)y0, out row))
							{
								player.rowsToRectify[(ushort)y0] = (uint)(Math.Min(x1, row >> 16) << 16) | (uint)(Math.Max(x2, row & 0xffff));
							}
							else
							{
								player.rowsToRectify[(ushort)y0] = (uint)(x1 << 16) | (uint)x2;
							}
						}
					}
				}
			}

			//if (tileAction == 1 && tileType == 53)
			//{
			//    NetMessage.SendTileSquare(-1, x, y, 1);
			//}
        }
    }
}
