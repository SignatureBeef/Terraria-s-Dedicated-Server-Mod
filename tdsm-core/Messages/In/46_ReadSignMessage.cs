using tdsm.api;
using tdsm.api.Plugin;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class ReadSignMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.READ_SIGN;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int x = (int)ReadInt16(readBuffer);
            int y = (int)ReadInt16(readBuffer);

            int signIndex = Sign.ReadSign(x, y);

            var player = Main.player[whoAmI];

            var ctx = new HookContext
            {
                Connection = player.Connection,
                Sender = player,
                Player = player,
            };

            var args = new HookArgs.SignTextGet
            {
                X = x,
                Y = y,
                SignIndex = (short)signIndex,
                Text = (signIndex >= 0 && Main.sign[signIndex] != null) ? Main.sign[signIndex].text : null,
            };

            HookPoints.SignTextGet.Invoke(ref ctx, ref args);

            if (ctx.CheckForKick() || ctx.Result == HookResult.IGNORE)
                return;

            if (args.Text != null)
            {
                var msg = NewNetMessage.PrepareThreadInstance();
                msg.WriteSign(signIndex, x, y, args.Text);
                msg.Send(whoAmI);
            }
        }
    }
}
