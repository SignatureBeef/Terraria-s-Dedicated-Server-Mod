using System;
using System.Text;
using Terraria_Server.Plugins;
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
                NetPlay.slots[whoAmI].Kick ("Cheating detected (KILL_PLAYER forgery).");
                return;
            }

			var player = Main.players[whoAmI];
			
			var ctx = new HookContext
			{
				Connection = NetPlay.slots[whoAmI].conn,
				Sender = player,
				Player = player,
			};
			
			var args = new HookArgs.ObituaryReceived
			{
				Direction = (int)readBuffer[num++] - 1,
				Damage = BitConverter.ToInt16 (readBuffer, num++),
				PvpFlag = readBuffer[++num]
			};
			string obituary;
			if (!ParseString(readBuffer, num + 1, length - num - 1 + start, out obituary))
			{
				NetPlay.slots[whoAmI].Kick("Invalid characters in obituary message.");
				return;
			}
			args.Obituary = obituary;
			
			HookPoints.ObituaryReceived.Invoke (ref ctx, ref args);
			
			if (ctx.CheckForKick ())
				return;
				
			if (ctx.Result == HookResult.IGNORE)
				return;
			
			ProgramLog.Death.Log ("{0} @ {1}: [Death] {2}{3}", player.IPAddress, whoAmI, player.Name ?? "<null>", args.Obituary);
			
			player.KillMe (args.Damage, args.Direction, args.PvpFlag == 1, args.Obituary);
			
			NetMessage.SendData(44, -1, whoAmI, args.Obituary, whoAmI, args.Direction, args.Damage, args.PvpFlag);
        }
    }
}
