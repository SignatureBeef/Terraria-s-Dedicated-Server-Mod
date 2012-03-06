using System;
using System.Text;
using System.Linq;
using Terraria_Server.Plugins;
using Terraria_Server.Logging;
using Terraria_Server.Misc;
using Terraria_Server.Networking;

namespace Terraria_Server.Messages
{
    public class PlayerChatMessage : MessageHandler
    {
		public PlayerChatMessage ()
		{
			ValidStates = SlotState.CONNECTED | SlotState.PLAYING;
		}
		
        public override Packet GetPacket()
        {
            return Packet.PLAYER_CHAT;
        }

        public override void Process (ClientConnection conn, byte[] readBuffer, int length, int num)
        {
            string chat = Encoding.ASCII.GetString(readBuffer, num + 4, length - 5).Trim();
            
            foreach (var c in chat)
            {
                if (c < 32 || c > 126)
                {
                    conn.Kick ("Invalid characters in chat message.");
                    return;
                }
            }
            
            if (conn.State < SlotState.PLAYING)
            {
                if (chat != "/playing")
                {
                    ProgramLog.Debug.Log ("{0}: sent message PLAYER_CHAT in state {1}.", conn.RemoteAddress, conn.State);
                    conn.Kick ("Invalid operation at this state.");
                }
                else
                {
                    ProgramLog.Debug.Log ("Replying to early online player query.");
                    var msg = NetMessage.PrepareThreadInstance ();
                    msg.PlayerChat (255, string.Concat ("Current players: ",
                            String.Join (", ", from p in Main.players where p.Active select p.Name), "."),
                            255, 240, 20);
                    conn.Send (msg.Output);
                }
                return;
            }
            
            int whoAmI = conn.SlotIndex;
            var player = Main.players[whoAmI];

			if (chat.Length == 0) //TODO: check for undetectable spam
				return;
				
			if (chat.Substring(0, 1).Equals("/"))
			{
				if (Main.players[whoAmI].Op)
					ProgramLog.Admin.Log (player.Name + " sent command: " + chat);
				else
					ProgramLog.Users.Log (player.Name + " sent command: " + chat);
				
				Program.commandParser.ParsePlayerCommand (player, chat);
				return;
			}
			
			Color color = ChatColor.White;
			if (player.Op)
			{
				color = ChatColor.DeepSkyBlue;
			}
			else if (player.Difficulty == 1)
			{
				color = ChatColor.Khaki;
			}
			else if (player.Difficulty == 2)
			{
				color = ChatColor.Tomato;
			}
			else if (player.team > 0 && player.team < Main.teamColor.Length)
			{
				color = Main.teamColor[player.team];
			}
			
			var ctx = new HookContext
			{
				Connection = player.Connection,
				Sender = player,
				Player = player,
			};
			
			var args = new HookArgs.PlayerChat
			{
				Message = chat,
				Color = color,
			};
			
			HookPoints.PlayerChat.Invoke (ref ctx, ref args);
			
			if (ctx.CheckForKick() || ctx.Result == HookResult.IGNORE)
				return;
			
			NetMessage.SendData (Packet.PLAYER_CHAT, -1, -1, chat, whoAmI, args.Color.R, args.Color.G, args.Color.B);
			ProgramLog.Chat.Log ("<" + player.Name + "> " + chat, SendingLogger.PLAYER);
        }
    }
}
