using System;

namespace Terraria_Server.Messages
{
    public class SynchBeginMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.SYNCH_BEGIN;
        }

        public int? GetRequiredNetMode()
        {
            return 1;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int playerIndex = (int)readBuffer[num++];
            int isActive = (int)readBuffer[num];
            if (isActive == 1)
            {
                if (!Main.player[playerIndex].active)
                {
                    Main.player[playerIndex] = new Player();
                }
                Main.player[playerIndex].active = true;
                return;
            }
            Main.player[playerIndex].active = false;
        }
    }
}
