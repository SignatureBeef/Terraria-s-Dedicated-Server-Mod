using System;
using Terraria_Server.Plugin;
using Terraria_Server.Events;

namespace Terraria_Server.Messages
{
    public class PlayerPvPChangeMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.PLAYER_PVP_CHANGE;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            int playerIndex = readBuffer[num];
            
            if (playerIndex != whoAmI)
            {
                Netplay.slots[whoAmI].Kick ("Cheating detected (PLAYER_PVP_CHANGE forgery).");
                return;
            }
            
            playerIndex = whoAmI;

            Player player = (Player)Main.players[playerIndex].Clone();
            player.hostile = (readBuffer[num + 1] == 1);

            PlayerPvPChangeEvent playerEvent = new PlayerPvPChangeEvent();
            playerEvent.Sender = player;
            Program.server.PluginManager.processHook(Hooks.PLAYER_PVPCHANGE, playerEvent);
            if (playerEvent.Cancelled)
            {
                return;
            }

            Main.players[playerIndex] = player;

            NetMessage.SendData(30, -1, whoAmI, "", playerIndex);

            string message;
            if(player.hostile)
            {
                message = " has enabled PvP!";
            }
            else
            {
                message = " has disabled PvP!";
            }
            NetMessage.SendData(25, -1, -1, player.Name + message, 255, (float)Main.teamColor[player.team].R, (float)Main.teamColor[player.team].G, (float)Main.teamColor[player.team].B);
        }
    }
}
