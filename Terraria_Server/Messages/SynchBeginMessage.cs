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
                if (!Main.players[playerIndex].Active)
                {
                    Main.players[playerIndex] = new Player();
                }
                Main.players[playerIndex].Active = true;
                return;
            }
            Main.players[playerIndex].Active = false;
        }
    }
}
