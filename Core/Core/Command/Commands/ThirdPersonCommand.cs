using System;
using OTA.Command;
using OTA.Logging;
using Terraria;

namespace TDSM.Core.Command.Commands
{
    public class ThirdPersonCommand : CoreCommand
    {
        public override void Initialise()
        {
            Core.AddCommand("me")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("3rd person talk")
                .WithHelpText("<message> - Message to display in third person.")
            //.SetDefaultUsage() //This was causing an additional "me" to be displayed in the help commmand syntax.
                .WithPermissionNode("tdsm.me")
                .Calls(this.Action);
        }

        /// <summary>
        /// 3rd person talking.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="message">Message to send</param>
        public void Action(ISender sender, string message)
        {
            if (!String.IsNullOrEmpty(message))
            {
                ProgramLog.Log("* " + sender.SenderName + " " + message);
                if (sender is Player)
                    NetMessage.SendData(25, -1, -1, "* " + sender.SenderName + " " + message, 255, 200, 100, 0);
                else
                    NetMessage.SendData(25, -1, -1, "* " + sender.SenderName + " " + message, 255, 238, 130, 238);
            }
            else
            {
                sender.SendMessage("Expected message");
            }
        }
    }
}

