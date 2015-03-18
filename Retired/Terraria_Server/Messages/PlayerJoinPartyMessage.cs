using System;
using Terraria_Server.Plugins;

namespace Terraria_Server.Messages
{
    public class PlayerJoinPartyMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.PLAYER_JOIN_PARTY;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            int playerIndex = readBuffer[num];
            
            if (playerIndex != whoAmI)
            {
                NetPlay.slots[whoAmI].Kick ("Cheating detected (PLAYER_JOIN_PARTY forgery).");
                return;
            }
            
			var teamIndex = readBuffer[num + 1];
			var player = Main.players[whoAmI];
			int currentTeam = player.team;
			
			var ctx = new HookContext
			{
				Connection = player.Connection,
				Player = player,
				Sender = player,
			};
			
			var args = new HookArgs.PartySettingReceived
			{
				Party = teamIndex,
			};
			
			HookPoints.PartySettingReceived.Invoke (ref ctx, ref args);
			
			if (ctx.CheckForKick ())
			{
				return;
			}
			
			if (ctx.Result == HookResult.IGNORE)
			{
				return;
			}
			
			if (ctx.Result == HookResult.RECTIFY)
			{
				NetMessage.SendData(45, whoAmI, -1, "", whoAmI);
				return;
			}
            
            string joinMessage = "";
            switch (teamIndex)
            {
                case 1:
                    joinMessage = " has joined the red party.";
                    break;
                case 2:
                    joinMessage = " has joined the green party.";
                    break;
                case 3:
                    joinMessage = " has joined the blue party.";
                    break;
                case 4:
                    joinMessage = " has joined the yellow party.";
                    break;
                default:
                    joinMessage = " is no longer in a party.";
                    break;
            }

            player.team = teamIndex;
            
            NetMessage.SendData(45, -1, whoAmI, "", playerIndex);
            
            for (int i = 0; i < 255; i++)
            {
                if (i == whoAmI
                    || (currentTeam > 0 && player.team == currentTeam)
                    || (teamIndex > 0 && player.team == teamIndex))
                {
                    NetMessage.SendData(25, i, -1, player.Name + joinMessage, 255, (float)Main.teamColor[teamIndex].R, (float)Main.teamColor[teamIndex].G, (float)Main.teamColor[teamIndex].B);
                }
            }
        }
    }
}
