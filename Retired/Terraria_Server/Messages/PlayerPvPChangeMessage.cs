using System;
using Terraria_Server.Plugins;

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
                NetPlay.slots[whoAmI].Kick ("Cheating detected (PLAYER_PVP_CHANGE forgery).");
                return;
            }
            
			var pvp = readBuffer[num + 1] == 1;
			var player = Main.players[whoAmI];
			
			var ctx = new HookContext
			{
				Connection = player.Connection,
				Player = player,
				Sender = player,
			};
			
			var args = new HookArgs.PvpSettingReceived
			{
				PvpFlag = pvp,
			};
			
			HookPoints.PvpSettingReceived.Invoke (ref ctx, ref args);
			
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
				NetMessage.SendData(30, whoAmI, -1, "", whoAmI);
				return;
			}
			
			player.hostile = pvp;
			
            string message = (player.hostile) ? " has enabled PvP!" : " has disabled PvP!";

            NetMessage.SendData(30, -1, whoAmI, "", whoAmI);
            NetMessage.SendData(25, -1, -1, player.Name + message, 255, (float)Main.teamColor[player.team].R, (float)Main.teamColor[player.team].G, (float)Main.teamColor[player.team].B);
        }
    }
}
