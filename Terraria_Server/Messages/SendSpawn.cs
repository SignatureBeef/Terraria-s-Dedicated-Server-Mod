using System;

namespace Terraria_Server.Messages
{
    public class SendSpawn : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.SEND_SPAWN;
        }

        public int? GetRequiredNetMode()
        {
            return null;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            //Client Only
        }
    }
}
