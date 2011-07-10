using System;

namespace Terraria_Server.Messages
{
    public class ReadSignMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.READ_SIGN;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            num += 4;

            int signIndex = Sign.ReadSign(x, y);
            if (signIndex >= 0)
            {
                NetMessage.SendData(47, whoAmI, -1, "", signIndex);
            }
        }
    }
}
