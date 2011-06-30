using System;
using System.Text;

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

            string string11 = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
            Main.sign[signIndex] = new Sign();
            Sign sign = Main.sign[signIndex];
            sign.x = x;
            sign.y = y;
            Sign.TextSign(signIndex, string11);
            Player player = Main.player[Main.myPlayer];

            if (Main.netMode == 1 
                && sign != null
                && signIndex != player.sign)
            {
                Main.playerInventory = false;
                player.talkNPC = -1;
                Main.editSign = false;
                player.sign = signIndex;
                Main.npcChatText = sign.text;
            }
        }
    }
}
