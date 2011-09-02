using System;
using Terraria_Server.Logging;

namespace Terraria_Server.Messages
{
    public class SummonSkeletronMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.SUMMON_SKELETRON;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            int playerId = readBuffer[num++]; //TODO: maybe check for forgery
            byte action = readBuffer[num];
            if (action == 1)
            {
				var player = Main.players[whoAmI];
				ProgramLog.Users.Log ("{0} @ {1}: Skeletron summoned by {2}.", player.IPAddress, whoAmI, player.Name);
				NetMessage.SendData (Packet.PLAYER_CHAT, -1, -1, string.Concat (player.Name, " has summoned Skeletron!"), 255, 255, 128, 150);
                NPC.SpawnSkeletron();
            }
            else if (action == 2)
            {
                NetMessage.SendData (51, -1, whoAmI, "", playerId, action, 0f, 0f, 0);
            }
        }
    }
}
