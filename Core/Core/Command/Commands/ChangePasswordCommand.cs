using System;
using OTA.Command;
using Terraria;
using OTA;
using TDSM.Core.Data;
using TDSM.Core.Data.Management;
using TDSM.Core.Data.Models;

namespace TDSM.Core.Command.Commands
{
    public class ChangePasswordCommand : CoreCommand
    {
        const int MinPasswordLength = 5; //TODO: password complexity

        public override void Initialise()
        {
            var api = AddCommand("changepassword", "cpass")
                .WithPermissionNode("tdsm.api")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Change your password")
                .WithHelpText("<password>")
                .Calls(ChangePassword);
        }

        void ChangePassword(ISender sender, string password)
        {
            if (!Storage.IsAvailable)
                throw new CommandError("No permissions plugin or data plugin is attached");

            var player = sender as Player;
            if (player == null)
            {
                sender.SendMessage("This is a player command", G: 0, B: 0);
                return;
            }
            else if (!player.IsAuthenticated())
            {
                sender.SendMessage("You are not authenticated.", G: 0, B: 0);
                return;
            }
            else if (password == null || password.Trim().Length < MinPasswordLength)
            {
                sender.SendMessage("Please specify a password longer than 5 characters.", G: 0, B: 0);
                return;
            }

            var auth = player.GetAuthenticatedAs();

            if (Authentication.UpdatePlayer(auth, password))
            {
                sender.SendMessage("Your password is now updated.", R: 0, B: 0);
            }
            else
            {
                sender.SendMessage("Failed to update your password.", G: 0, B: 0);
            }
        }
    }
}

