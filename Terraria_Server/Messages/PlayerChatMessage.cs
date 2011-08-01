using System;
using System.Text;
using System.Linq;
using Terraria_Server.Events;
using Terraria_Server.Plugin;
using Terraria_Server.Logging;
using Terraria_Server.Misc;

namespace Terraria_Server.Messages
{
    public class PlayerChatMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.PLAYER_CHAT;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int playerIndex = whoAmI;

            String chat = Encoding.ASCII.GetString(readBuffer, start + 5, length - 5).Trim();
            
            var slot = Netplay.slots [whoAmI];
            if (slot.state < SlotState.PLAYING)
            {
                if (chat != "/playing")
                {
                    ProgramLog.Debug.Log ("{0}: sent message PLAYER_CHAT in state {1}.", slot.remoteAddress, slot.state);
                    slot.Kick ("Invalid operation at this state.");
                }
                else
                {
                    ProgramLog.Debug.Log ("Replying to early online player query.");
                    NetMessage.SendData (25, whoAmI, -1,
                        string.Concat ("Current players: ",
                            string.Join (", ", from p in Server.players where p.Active select p.Name), "."),
                        255, 255, 240, 20);
                }
                return;
            }

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

                Color chatColour = ChatColour.White;
                if (Main.players[playerIndex].Op)
                {
                    chatColour = ChatColour.SteelBlue;
                } else if (Main.players[playerIndex].hardCore)
                {
                    chatColour = new Color(238, 160, 238);
                }
                NetMessage.SendData(Packet.PLAYER_CHAT, -1, -1, chat, playerIndex, chatColour.R, chatColour.G, chatColour.B);
                ProgramLog.Chat.Log ("<" + Main.players[playerIndex].Name + "> " + chat);
            }
        }

        private bool ProcessMessage(MessageEvent messageEvent, String text, Hooks hook, int whoAmI)
        {
            messageEvent.Message = text;
            messageEvent.Sender = Main.players[whoAmI];
            Program.server.PluginManager.processHook(hook, messageEvent);

            return !messageEvent.Cancelled;
        }
    }
}
