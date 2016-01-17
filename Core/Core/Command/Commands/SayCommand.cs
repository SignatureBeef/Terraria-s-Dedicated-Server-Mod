using System;
using OTA;
using OTA.Command;
using OTA.Logging;
using Terraria;

namespace TDSM.Core.Command.Commands
{
    public class SayCommand : CoreCommand
    {
        public override void Initialise()
        {
            AddCommand("say")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Say a message from the server")
                .WithHelpText("<message>")
                .WithPermissionNode("tdsm.say")
                .Calls(this.Say);
        }

        /// <summary>
        /// Sends a Server Message to all online Players.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="message">Message to send</param>
        public void Say(ISender sender, string message)
        {
            /*ProgramLog.Log("<" + sender.SenderName + "> " + message);
            if (sender is Player)
                NewNetMessage.SendData(25, -1, -1, "<" + sender.SenderName + "> " + message, 255, 255, 255, 255);
            else
                NewNetMessage.SendData(25, -1, -1, "<" + sender.SenderName + "> " + message, 255, 238, 180, 238);*/

            // 'Say' should be used for Server Messages, OP's only. This is used on many large servers to notify
            // Users for a quick restart (example), So the OP will most likely be in game, unless it's major.

            if (!String.IsNullOrEmpty(message))
            {
                ProgramLog.Log("<" + sender.SenderName + "> " + ((sender is ConsoleSender) ? String.Empty : "SERVER: ") + message);
                NetMessage.SendData(25, -1, -1, "SERVER: " + message, 255, 238, 130, 238);
            }
            else
            {
                sender.SendMessage("Expected message");
            }
        }
    }
}

