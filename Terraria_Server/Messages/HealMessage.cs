using System;

namespace Terraria_Server.Messages
{
    public class HealMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.HEAL_PLAYER;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int playerIndex = (int)readBuffer[num++];
            
            if (playerIndex != whoAmI)
            {
                Netplay.slots[whoAmI].Kick ("Cheating detected (HEAL_PLAYER forgery).");
                return;
            }

            playerIndex = whoAmI;

            int heal = (int)BitConverter.ToInt16(readBuffer, num);
            num += 2;

            if (playerIndex != Main.myPlayer)
            {
                Main.players[playerIndex].HealEffect(heal);
            }

            NetMessage.SendData(35, -1, whoAmI, "", playerIndex, (float)heal);
        }
    }
}
