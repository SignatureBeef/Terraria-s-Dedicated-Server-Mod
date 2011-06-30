using System;

namespace Terraria_Server.Messages
{
    public class PasswordRequestMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.PASSWORD_REQUEST;
        }

        public int? GetRequiredNetMode()
        {
            return 1;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            if (Main.autoPass)
            {
                NetMessage.SendData(38, -1, -1, Netplay.password);
                Main.autoPass = false;
                return;
            }
            Netplay.password = "";
            Main.menuMode = 31;
        }
    }
}
