using System;
using System.Text;

namespace Terraria_Server.Messages
{
    public class StrikePlayerMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.STRIKE_PLAYER;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int playerIndex = (int)readBuffer[num++];
            Player player = Main.players[playerIndex];
            if (whoAmI != playerIndex && (!player.hostile || !Main.players[whoAmI].hostile))
            {
                return;
            }

            int hitDirection = (int)(readBuffer[num++] - 1);
            short damage = BitConverter.ToInt16(readBuffer, num);
            num += 2;
            byte pvpFlag = readBuffer[num++];
            bool pvp = (pvpFlag != 0);
            String deathText = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
            
            if (player.Hurt((int)damage, hitDirection, pvp, true, deathText) > 0.0)
            {
                NetMessage.SendData(26, -1, whoAmI, deathText, playerIndex, (float)hitDirection, (float)damage, (float)pvpFlag, 0);
            }
        }
    }
}
