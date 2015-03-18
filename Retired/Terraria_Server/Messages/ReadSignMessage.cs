using System;
using Terraria_Server.Plugins;

namespace Terraria_Server.Messages
{
    public class ReadSignMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.READ_SIGN;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            num += 4;

            int signIndex = Sign.ReadSign(x, y);
            
			var player = Main.players[whoAmI];
			
			var ctx = new HookContext
			{
				Connection = player.Connection,
				Sender = player,
				Player = player,
			};
			
			var args = new HookArgs.SignTextGet
			{
				X = x, Y = y,
				SignIndex = (short)signIndex,
				Text = (signIndex >= 0 && Main.sign[signIndex] != null) ? Main.sign[signIndex].text : null,
			};
			
			HookPoints.SignTextGet.Invoke (ref ctx, ref args);
			
			if (ctx.CheckForKick () || ctx.Result == HookResult.IGNORE)
				return;
			
			if (args.Text != null)
			{
				var msg = NetMessage.PrepareThreadInstance ();
				msg.WriteSign (signIndex, x, y, args.Text);
				msg.Send (whoAmI);
			}
        }
    }
}
