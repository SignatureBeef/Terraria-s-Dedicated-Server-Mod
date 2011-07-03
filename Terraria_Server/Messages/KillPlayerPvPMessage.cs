using System;
using System.Text;
using Terraria_Server.Events;
using Terraria_Server.Plugin;

namespace Terraria_Server.Messages
{
    public class KillPlayerPvPMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.KILL_PLAYER_PVP;
        }

        public int? GetRequiredNetMode()
        {
            return null;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int playerIndex = readBuffer[num++];
            if (playerIndex == Main.myPlayer)
            {
                return;
            }

            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }

            int direction = (int)(readBuffer[num++] - 1);

            short damage = BitConverter.ToInt16(readBuffer, num);
            num += 2;
            byte pvpFlag = readBuffer[num++];

            string deathText = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
            bool pvp = (pvpFlag != 0);

            PlayerDeathEvent pDeath = new PlayerDeathEvent();
            pDeath.DeathMessage = deathText;
            pDeath.Sender = Main.players[playerIndex];
            Program.server.getPluginManager().processHook(Hooks.PLAYER_DEATH, pDeath);
            if (pDeath.Cancelled)
            {
                return;
            }

            Main.players[playerIndex].KillMe((double)damage, direction, pvp, deathText);

            if (Main.netMode == 2)
            {
                NetMessage.SendData(44, -1, whoAmI, deathText, playerIndex, (float)direction, (float)damage, (float)pvpFlag);
            }
        }
    }
}
