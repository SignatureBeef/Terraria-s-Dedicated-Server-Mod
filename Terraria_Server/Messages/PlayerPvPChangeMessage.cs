using System;

namespace Terraria_Server.Messages
{
    public class PlayerPvPChangeMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.PLAYER_PVP_CHANGE;
        }

        public int? GetRequiredNetMode()
        {
            return null;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int playerIndex = readBuffer[num++];
            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }

            Player player = Main.players[playerIndex];
            player.hostile = (readBuffer[num] == 1);

            if (Main.netMode == 2)
            {
                NetMessage.SendData(30, -1, whoAmI, "", playerIndex);

                String message;
                if(player.hostile)
                {
                    message = " has enabled PvP!";
                }
                else
                {
                    message = " has disabled PvP!";
                }
                NetMessage.SendData(25, -1, -1, player.name + message, 255, (float)Main.teamColor[player.team].R, (float)Main.teamColor[player.team].G, (float)Main.teamColor[player.team].B);
            }
        }
    }
}
