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

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            String password = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
            if (password == Netplay.password)
            {
                Netplay.slots[whoAmI].state = SlotState.ACCEPTED;
                NetMessage.SendData(3, whoAmI);
                return;
            }
            Netplay.slots[whoAmI].Kick ("Incorrect password.");
        }
    }
}
