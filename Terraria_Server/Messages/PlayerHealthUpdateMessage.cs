using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Messages
{
    public class PlayerHealthUpdateMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.PLAYER_HEALTH_UPDATE;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int playerIndex = readBuffer[num++];
            
            if (playerIndex != whoAmI)
            {
                Netplay.slots[whoAmI].Kick ("Cheating detected (PLAYER_HEALTH_UPDATE forgery).");
                return;
            }

            int statLife = (int)BitConverter.ToInt16(readBuffer, num);
            num += 2;
            int statLifeMax = (int)BitConverter.ToInt16(readBuffer, num);

            playerIndex = whoAmI;

            Player player = Main.players[playerIndex];
            player.statLife = statLife;
            player.statLifeMax = statLifeMax;

            if (player.statLife <= 0)
            {
                player.dead = true;
            }

            //if (Main.players[playerIndex].HasHackedData())
            //{
            //    Main.players[playerIndex].Kick("No Hacked Health is allowed.");
            //}

            NetMessage.SendData(16, -1, whoAmI, "", playerIndex);
        }
    }
}
