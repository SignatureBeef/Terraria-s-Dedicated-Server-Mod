using System;
using System.Text;

namespace Terraria_Server.Messages
{
    public class PasswordResponseMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.PASSWORD_RESPONSE;
        }

        public int? GetRequiredNetMode()
        {
            return 2;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            String password = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
            if (password == Netplay.password)
            {
                Netplay.serverSock[whoAmI].state = 1;
                NetMessage.SendData(3, whoAmI);
                return;
            }
            NetMessage.SendData(2, whoAmI, -1, "Incorrect password.");
        }
    }
}
