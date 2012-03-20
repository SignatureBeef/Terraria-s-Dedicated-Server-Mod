using System;

using Terraria_Server.Logging;
using Terraria_Server.Plugins;

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
            int playerId = readBuffer[num++];
            byte action = readBuffer[num];

			if (playerId != whoAmI)
			{
				Main.players[whoAmI].Kick("SummonSkeletron Player Forgery.");
				return;
			}

            if (action == 1)
            {
				var player = Main.players[whoAmI];
				
				var ctx = new HookContext
				{
					Connection = player.Connection,
					Sender = player,
					Player = player,
				};
				
				var args = new HookArgs.PlayerTriggeredEvent
				{
					Type = WorldEventType.BOSS,
					Name = "Skeletron",
				};
				
				HookPoints.PlayerTriggeredEvent.Invoke (ref ctx, ref args);
				
				if (ctx.CheckForKick () || ctx.Result == HookResult.IGNORE)
					return;
				
				//ProgramLog.Users.Log ("{0} @ {1}: Skeletron summoned by {2}.", player.IPAddress, whoAmI, player.Name);
				//NetMessage.SendData (Packet.PLAYER_CHAT, -1, -1, string.Concat (player.Name, " has summoned Skeletron!"), 255, 255, 128, 150);
                NPC.SpawnSkeletron(player);
            }
            else if (action == 2)
            {
                NetMessage.SendData (51, -1, whoAmI, "", playerId, action, 0f, 0f, 0);
            }
        }
    }
}
