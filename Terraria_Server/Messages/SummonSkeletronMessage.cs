using System;

namespace Terraria_Server.Messages
{
    public class SummonSkeletronMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.SUMMON_SKELETRON;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
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
