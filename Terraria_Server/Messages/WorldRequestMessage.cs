using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Messages
{
    public class WorldRequestMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.WORLD_REQUEST;
        }

        public int? GetRequiredNetMode()
        {
            return 2;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            if (Netplay.serverSock[whoAmI].state == 1)
            {
                Netplay.serverSock[whoAmI].state = 2;
            }
            NetMessage.SendData(7, whoAmI);
        }
    }
}
