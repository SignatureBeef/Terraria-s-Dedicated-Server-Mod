using System;
using System.Text;
using Terraria_Server.Plugins;

namespace Terraria_Server.Messages
{
    public class WriteSignMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.WRITE_SIGN;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num) //TODO: redesign signs
        {
            int start = num - 1;
            short signIndex = BitConverter.ToInt16(readBuffer, num);
            num += 2;
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            num += 4;

			short existing = (short)Sign.ReadSign (x, y);
			if (existing >= 0)
				signIndex = existing;

            string SignText;
            if (!ParseString(readBuffer, num, length - num + start, out SignText))
                return; // invalid characters
			
			var player = Main.players[whoAmI];
			
			var ctx = new HookContext
			{
				Connection = player.Connection,
				Sender = player,
				Player = player,
			};
			
			var args = new HookArgs.SignTextSet
			{
				X = x, Y = y,
				SignIndex = signIndex,
				Text = SignText,
				OldSign = Main.sign[signIndex],
			};
			
			HookPoints.SignTextSet.Invoke (ref ctx, ref args);
			
			if (ctx.CheckForKick () || ctx.Result == HookResult.IGNORE)
				return;
			
			
			if (Main.sign[signIndex] == null) Main.sign[signIndex] = new Sign ();
			Main.sign[signIndex].x = args.X;
			Main.sign[signIndex].y = args.Y;
			Sign.TextSign (signIndex, args.Text);
        }
    }
}
