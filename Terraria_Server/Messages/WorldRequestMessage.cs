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

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            if (Netplay.slots[whoAmI].state == SlotState.ACCEPTED)
            {
                Netplay.slots[whoAmI].state = SlotState.SENDING_WORLD;
            }
            NetMessage.SendData(7, whoAmI);
        }
    }
}
