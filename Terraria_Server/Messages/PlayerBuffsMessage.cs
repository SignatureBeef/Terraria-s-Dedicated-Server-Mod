using System;

namespace Terraria_Server.Messages
{
    public class PlayerBuffsMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.PLAYER_BUFFS;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int playerIndex = whoAmI;
            num++;

            if (playerIndex == Main.myPlayer)
            {
                return;
            }

            Player player = Main.players[playerIndex];
            for (int i = 0; i < 10; i++)
            {
                player.buffType[i] = (int)readBuffer[num++];
                if (player.buffType[i] > 0)
                {
                    player.buffTime[i] = 60;
                }
                else
                {
                    player.buffTime[i] = 0;
                }
            }
            
            NetMessage.SendData(50, -1, whoAmI, "", playerIndex);
        }
    }
}
