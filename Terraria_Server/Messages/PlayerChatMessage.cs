using System;
using System.Text;
using System.Linq;
using Terraria_Server.Events;
using Terraria_Server.Plugin;
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
            int playerIndex = whoAmI;
            //var slot = Netplay.slots[whoAmI];

            if (chat.Length > 0)
            {
                if (chat.Substring(0, 1).Equals("/"))
                {
                    if (Main.players[playerIndex].Op)
                        ProgramLog.Admin.Log (Main.players[playerIndex].Name + " sent command: " + chat);
                    else
                        ProgramLog.Users.Log (Main.players[playerIndex].Name + " sent command: " + chat);
                    
                    if (!ProcessMessage(new PlayerCommandEvent(), chat, Hooks.PLAYER_COMMAND, whoAmI))
                    {
                        return;
                    }
                    
                    Program.commandParser.ParsePlayerCommand(Main.players[playerIndex], chat);
                    return;
                }
                else
                {
                    if (!ProcessMessage(new MessageEvent(), chat, Hooks.PLAYER_CHAT, playerIndex))
                    {
                        return;
                    }
                }

                Color chatColour = ChatColor.White;
                if (Main.players[playerIndex].Op)
                {
                    chatColour = ChatColor.SteelBlue;
                }
                else if (Main.players[playerIndex].Difficulty == 1)
                {
                    chatColour = ChatColor.BlanchedAlmond;
                }
                else if (Main.players[playerIndex].Difficulty == 2)
                {
                    chatColour = ChatColor.Tomato;
                }
                NetMessage.SendData(Packet.PLAYER_CHAT, -1, -1, chat, playerIndex, chatColour.R, chatColour.G, chatColour.B);
                ProgramLog.Chat.Log ("<" + Main.players[playerIndex].Name + "> " + chat);
            }
        }

        private bool ProcessMessage(MessageEvent messageEvent, string text, Hooks hook, int whoAmI)
        {
            messageEvent.Message = text;
            messageEvent.Sender = Main.players[whoAmI];
            Server.PluginManager.processHook(hook, messageEvent);

            return !messageEvent.Cancelled;
        }
    }
}
