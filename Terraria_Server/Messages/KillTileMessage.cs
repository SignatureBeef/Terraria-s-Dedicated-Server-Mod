using System;
using Terraria_Server.Misc;
using Terraria_Server.Plugins;
using Terraria_Server.WorldMod;

namespace Terraria_Server.Messages
{
    public class KillTileMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.KILL_TILE;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            
			if (Main.tile.At(x, y).Type != 21)
				return;
			
			var player = Main.players[whoAmI];
			                
			var ctx = new HookContext
			{
				Connection = player.Connection,
				Player = player,
				Sender = player,
			};
			
			var args = new HookArgs.ChestBreakReceived
			{
				X = x, Y = y,
			};
			
			HookPoints.ChestBreakReceived.Invoke (ref ctx, ref args);
			
			if (ctx.CheckForKick ())
			{
				return;
			}
			
			if (ctx.Result == HookResult.IGNORE)
			{
				return;
			}
			
			if (ctx.Result == HookResult.RECTIFY)
			{
				NetMessage.SendTileSquare(whoAmI, x, y, 3);
				return;
			}

			WorldModify.KillTile(null, null, x, y);

			if (!Main.tile.At(x, y).Active || Main.tile.At(x, y).Type != 21)
				NetMessage.SendData(17, -1, -1, "", 0, (float)x, (float)y);
        }
    }
}
