using System;
using Terraria_Server.Misc;
using Terraria_Server.Plugins;
using Terraria_Server.WorldMod;

namespace Terraria_Server.Messages
{
    public class FlowLiquidMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.FLOW_LIQUID;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            byte liquid = readBuffer[num++];
            byte lavaFlag = readBuffer[num]++;
            
            var player = Main.players[whoAmI];

            if (NetPlay.spamCheck) // dead code...
            {
                int centerX = (int)(player.Position.X + (float)(player.Width / 2));
                int centerY = (int)(player.Position.Y + (float)(player.Height / 2));
                int disperseDistance = 10;
                int left = centerX - disperseDistance;
                int right = centerX + disperseDistance;
                int top = centerY - disperseDistance;
                int bottom = centerY + disperseDistance;
                if (centerX < left || centerX > right || centerY < top || centerY > bottom)
                {
                    NetMessage.BootPlayer(whoAmI, "Cheating attempt detected: Liquid spam");
                    return;
                }
            }
            
			var ctx = new HookContext
			{
				Connection = player.Connection,
				Player = player,
				Sender = player,
			};
			
			var args = new HookArgs.LiquidFlowReceived
			{
				X = x, Y = y,
				Amount = liquid,
				Lava = lavaFlag == 1,
			};
			
			HookPoints.LiquidFlowReceived.Invoke (ref ctx, ref args);
			
			if (ctx.CheckForKick ())
				return;
			
			if (ctx.Result == HookResult.IGNORE)
				return;
			
			if (ctx.Result == HookResult.RECTIFY)
			{
				var msg = NetMessage.PrepareThreadInstance ();
				msg.FlowLiquid (x, y);
				msg.Send (whoAmI);
				return;
			}
            
            TileRef tile = Main.tile.At(x, y);
            {
                tile.SetLiquid (liquid);
                tile.SetLava (lavaFlag == 1);

                WorldModify.SquareTileFrame(null, null, x, y, true);
            }
        }
    }
}
