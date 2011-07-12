using System;
using System.Text;
using Terraria_Server.Events;
using Terraria_Server.Plugin;

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

            if (chat.Length > 0)
            {
                if (chat.Substring(0, 1).Equals("/"))
                {
                    if (!ProcessMessage(new PlayerCommandEvent(), chat, Hooks.PLAYER_COMMAND, whoAmI))
                    {
                        return;
                    }

                    Program.tConsole.WriteLine(Main.players[playerIndex].Name + " Sent Command: " + chat);
                    Program.commandParser.parsePlayerCommand(Main.players[playerIndex], chat);
                    return;
                }
                else
                {
                    if (!ProcessMessage(new MessageEvent(), chat, Hooks.PLAYER_CHAT, playerIndex))
                    {
                        return;
                    }
                }

                NetMessage.SendData(25, -1, -1, chat, playerIndex, (float)255, (float)255, (float)255);
                Program.tConsole.WriteLine("<" + Main.players[playerIndex].Name + "> " + chat);
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
