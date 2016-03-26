using System;
using System.Linq;
using Terraria;
using OTA;
using OTA.Command;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TDSM.Core.Misc;
using OTA.Extensions;

namespace TDSM.Core.Command.Commands
{
    public class PrivateMessageCommand : CoreCommand
    {
        public override void Initialise()
        {
            AddCommand("pm")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Private message another player")
                .WithHelpText("<player> <message>")
                .WithPermissionNode("tdsm.pm")
                .Calls(this.PrivateMessageSend);

            AddCommand("r")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Reply to the last person you private messaged")
                .WithHelpText("<message>")
                .WithPermissionNode("tdsm.pm")
                .Calls(this.PrivateMessageReply);
        }

        public void PrivateMessageSend(ISender sender, ArgumentList args)
        {
            var playerFrom = sender as Player;
            if (playerFrom == null)
            {
                sender.SendMessage("This is a player command.", G: 0, B: 0);
                return;
            }

            var playerTo = args.GetOnlinePlayer(0);
            var msg = string.Join(" ", args.Skip(1).ToArray());

            playerTo.Message(255, Color.Orange, $"[PM] {sender.SenderName}: {msg}");

            playerFrom.SetPluginData("LastPM", playerTo.name);
        }

        public void PrivateMessageReply(ISender sender, string message)
        {
            var playerFrom = sender as Player;
            if (playerFrom == null)
            {
                sender.SendMessage("This is a player command.", G: 0, B: 0);
                return;
            }

            if (String.IsNullOrEmpty(message))
            {
                sender.SendMessage("Invalid message", G: 0, B: 0);
                return;
            }

            var lastPlayerName = playerFrom.GetPluginData("LastPM", String.Empty);
            if(String.IsNullOrEmpty(lastPlayerName))
            {
                sender.SendMessage("You are required to have started a conversation.", G: 0, B: 0);
                return;
            }

            var playerTo = Tools.GetPlayerByName(lastPlayerName);
            if(playerTo == null || !playerTo.active)
            {
                sender.SendMessage($"Last PM'd user ({lastPlayerName}) is no longer online.", G: 0, B: 0);
                return;
            }

            playerTo.Message(255, Color.Orange, $"[PM] {playerFrom.SenderName}: {message}");
        }
    }
}

