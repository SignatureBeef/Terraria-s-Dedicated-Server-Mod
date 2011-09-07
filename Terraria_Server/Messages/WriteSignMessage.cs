using System;
using System.Text;
using Terraria_Server.Events;
using Terraria_Server.Plugin;

namespace Terraria_Server.Messages
{
    public class WriteSignMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.WRITE_SIGN;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            int start = num - 1;
            int signIndex = (int)BitConverter.ToInt16(readBuffer, num);
            num += 2;
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            num += 4;

            string SignText = Encoding.ASCII.GetString(readBuffer, num, length - num + start);

            Sign sign = new Sign();
            sign.x = x;
            sign.y = y;

            PlayerEditSignEvent signEvent = new PlayerEditSignEvent();
            signEvent.Sender = Main.players[whoAmI];
            signEvent.Sign = sign;
            signEvent.Text = SignText;
            Server.PluginManager.processHook(Hooks.PLAYER_EDITSIGN, signEvent);
            if (signEvent.Cancelled)
            {
                return;
            }

            Main.sign[signIndex] = sign;
            Sign.TextSign(signIndex, SignText);
        }
    }
}
