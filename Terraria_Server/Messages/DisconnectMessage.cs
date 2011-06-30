using System.Text;

namespace Terraria_Server.Messages
{
    public class DisconnectMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.DISCONNECT;
        }

        public int? GetRequiredNetMode()
        {
            return 1;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            Netplay.disconnect = true;
            Main.statusText = Encoding.ASCII.GetString(readBuffer, start + 1, length - 1);
        }
    }
}
