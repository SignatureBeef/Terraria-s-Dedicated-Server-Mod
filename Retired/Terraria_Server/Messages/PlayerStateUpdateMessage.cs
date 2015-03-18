using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Plugins;
using Terraria_Server.Misc;
using Terraria_Server.Definitions;

namespace Terraria_Server.Messages
{
    public class PlayerStateUpdateMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.PLAYER_STATE_UPDATE;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            if (readBuffer[num++] != whoAmI)
            {
                NetPlay.slots[whoAmI].Kick ("Cheating detected (PLAYER_STATE_UPDATE forgery).");
                return;
            }
            
			var player = Main.players[whoAmI];
			
			var ctx = new HookContext
			{
				Connection = player.Connection,
				Sender = player,
				Player = player,
			};
			
			var args = new HookArgs.StateUpdateReceived ();
			
			args.Parse (readBuffer, num);
			
			HookPoints.StateUpdateReceived.Invoke (ref ctx, ref args);
			
			if (ctx.CheckForKick ())
				return;
			
			player.oldVelocity = player.Velocity;
			args.ApplyParams (player);
			args.ApplyKeys (player);
			player.fallStart = (int)(player.Position.Y / 16f);

            if (NetPlay.slots[whoAmI].state == SlotState.PLAYING)
            {
                NetMessage.SendData(13, -1, whoAmI, "", whoAmI);
            }
        }
    }
}
