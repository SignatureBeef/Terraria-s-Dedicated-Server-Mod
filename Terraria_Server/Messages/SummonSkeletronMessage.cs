using System;

namespace Terraria_Server.Messages
{
    public class SummonSkeletronMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.SUMMON_SKELETRON;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            byte action = readBuffer[num];
            if (action == 1)
            {
                NPC.SpawnSkeletron();
            }
            else if (action == 2)
            {
                NetMessage.SendData (51, -1, whoAmI, "", action, (float)readBuffer[num + 1], 0f, 0f, 0);
            }
        }
    }
}
