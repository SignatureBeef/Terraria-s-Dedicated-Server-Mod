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
            int player = readBuffer[num++]; //TODO: maybe check for forgery
            byte action = readBuffer[num];
            if (action == 1)
            {
                NPC.SpawnSkeletron();
            }
            else if (action == 2)
            {
                NetMessage.SendData (51, -1, whoAmI, "", player, action, 0f, 0f, 0);
            }
        }
    }
}
