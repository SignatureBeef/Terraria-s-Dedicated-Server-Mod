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
            byte buffer = readBuffer[num];
            if (buffer == 1)
            {
                NPC.SpawnSkeletron();
            }
        }
    }
}
