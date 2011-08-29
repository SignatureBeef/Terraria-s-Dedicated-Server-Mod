using System;
using System.Text;
using Terraria_Server.Events;
using Terraria_Server.Plugin;
using Terraria_Server.Logging;

namespace Terraria_Server.Messages
{
    public class KillPlayerPvPMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.KILL_PLAYER_PVP;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            int start = num - 1;
            int playerIndex = readBuffer[num++];
            
            if (playerIndex != whoAmI)
            {
                Netplay.slots[whoAmI].Kick ("Cheating detected (KILL_PLAYER forgery).");
                return;
            }

            playerIndex = whoAmI;

            int direction = (int)(readBuffer[num++] - 1);

            short damage = BitConverter.ToInt16(readBuffer, num);
            num += 2;
            byte pvpFlag = readBuffer[num++];

            String deathText = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
            bool pvp = (pvpFlag != 0);

            var player = Main.players[playerIndex];
            
            PlayerDeathEvent pDeath = new PlayerDeathEvent();
            pDeath.DeathMessage = deathText;
            pDeath.Sender = player;
            Program.server.PluginManager.processHook(Hooks.PLAYER_DEATH, pDeath);
            if (pDeath.Cancelled)
            {
                return;
            }
            
            ProgramLog.Death.Log ("{0} @ {1}: [Death] {2}{3}", player.IPAddress, whoAmI, player.Name ?? "<null>", deathText);

            Main.players[playerIndex].KillMe((double)damage, direction, pvp, deathText);

            NetMessage.SendData(44, -1, whoAmI, deathText, playerIndex, (float)direction, (float)damage, (float)pvpFlag);
        }
    }
}
