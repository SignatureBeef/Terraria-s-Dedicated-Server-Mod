using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Messages
{
    public class PlayerUseManaUpdateMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.PLAYER_USE_MANA_UPDATE;
        }

        public int? GetRequiredNetMode()
        {
            return null;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int playerIndex = (int)readBuffer[num++];
            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }

            int manaAmount = (int)BitConverter.ToInt16(readBuffer, num);
            num += 2;

            if (playerIndex != Main.myPlayer)
            {
                Main.player[playerIndex].ManaEffect(manaAmount);
            }

            if (Main.netMode == 2)
            {
                NetMessage.SendData(43, -1, whoAmI, "", playerIndex, (float)manaAmount);
            }
        }
    }
}
