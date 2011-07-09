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
            return 1;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
//            if (Netplay.clientSock.state == 6)
//            {
//                Netplay.clientSock.state = 10;
//                Main.players[Main.myPlayer].Spawn();
//            }
        }
    }
}
