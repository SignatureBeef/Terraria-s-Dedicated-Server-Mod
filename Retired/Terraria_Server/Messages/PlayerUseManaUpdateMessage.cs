using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Messages
{
    public class PlayerUseManaUpdateMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.PLAYER_USE_MANA_UPDATE;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            int playerIndex = readBuffer[num];
            
            if (playerIndex != whoAmI)
            {
                NetPlay.slots[whoAmI].Kick ("Cheating detected (PLAYER_USE_MANA_UPDATE forgery).");
                return;
            }
        
            playerIndex = whoAmI;
            num++;

            int manaAmount = (int)BitConverter.ToInt16(readBuffer, num);
            num += 2;

            if (playerIndex != Main.myPlayer)
            {
                Main.players[playerIndex].ManaEffect(manaAmount);
            }

            NetMessage.SendData(43, -1, whoAmI, "", playerIndex, (float)manaAmount);
        }
    }
}
