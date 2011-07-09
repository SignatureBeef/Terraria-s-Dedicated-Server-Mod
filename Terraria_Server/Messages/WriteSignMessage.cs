using System;
using System.Text;
using Terraria_Server.Events;
using Terraria_Server.Plugin;

namespace Terraria_Server.Messages
{
    public class WriteSignMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.WRITE_SIGN;
        }

        public int? GetRequiredNetMode()
        {
            return null;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int signIndex = (int)BitConverter.ToInt16(readBuffer, num);
            num += 2;
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            num += 4;

            String SignText = Encoding.ASCII.GetString(readBuffer, num, length - num + start);

            Sign sign = new Sign();
            sign.x = x;
            sign.y = y;

            PlayerEditSignEvent signEvent = new PlayerEditSignEvent();
            signEvent.Sender = Main.players[whoAmI];
            signEvent.Sign = sign;
            signEvent.Text = SignText;
            Program.server.getPluginManager().processHook(Hooks.PLAYER_EDITSIGN, signEvent);
            if (signEvent.Cancelled)
            {
                return;
            }

            Main.sign[signIndex] = sign;
            Sign.TextSign(signIndex, SignText);
        }
    }
}
