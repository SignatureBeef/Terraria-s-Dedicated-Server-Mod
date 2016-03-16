using System;
using OTA;
using TDSM.Core.Data;
using OTA.Command;
using Microsoft.Xna.Framework;
using Terraria;

namespace TDSM.Core.Command.Commands
{
    public class OperatorCommand : CoreCommand
    {
        public override void Initialise()
        {
            AddCommand("auth")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithPermissionNode("tdsm.auth")
                .WithDescription("Sign in")
                .Calls(this.Auth);
            
            AddCommand("op")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Allows a player server operator status")
                .WithHelpText("<player> <password> - Sets the player as an operator on the server and sets the OP password for that player.")
                .WithPermissionNode("tdsm.IsOp()")
                .Calls(this.OpPlayer);

            AddCommand("deop")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Removes server operator status from a player")
                .WithHelpText("<player> - Removes server operator status from the specified player.")
                .WithPermissionNode("tdsm.deop")
                .Calls(this.DeopPlayer);
        }

        /// <summary>
        /// Sets OP status to a given Player.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void OpPlayer(ISender sender, ArgumentList args)
        {
            var playerName = args.GetString(0);

            if (Storage.IsAvailable)
            {
                var existing = Authentication.GetPlayer(playerName);
                if (existing != null)
                {
                    if (existing.Operator)
                        throw new CommandError("Player is already an operator");

                    if (Authentication.UpdatePlayer(playerName, null, op: true))
                    {
                        Utils.NotifyAllOps("Opping " + playerName + " [" + sender.SenderName + "]", true);
                        var player = Tools.GetPlayerByName(playerName);
                        if (player != null)
                        {
                            player.SendMessage("You are now a server operator.", Color.Green);
                            player.SetOp(true);
                            player.SetAuthentication(player.name, "tdsm");
                        }

                        sender.Message("Op success", Color.DarkGreen);
                    }
                    else
                    {
                        sender.Message("Failed to op player", Color.DarkRed);
                    }
                }
                else
                {
                    sender.Message("No user found by " + playerName, Color.DarkRed);
                    sender.Message("Please use the `user` command", Color.DarkRed);
                }
            }
            else
            {
                var password = args.GetString(1);

                Utils.NotifyAllOps("Opping " + playerName + " [" + sender.SenderName + "]", true);
                Core.Ops.Add(playerName, password);

                var player = Tools.GetPlayerByName(playerName);
                if (player != null)
                {
                    player.SendMessage("You are now a server operator.", Color.Green);
                    player.SetOp(true);
                    player.SetAuthentication(player.name, "tdsm");
                }

                if (!Core.Ops.Save())
                {
                    Utils.NotifyAllOps("Failed to save op list [" + sender.SenderName + "]", true);
                    return;
                }
            }
        }

        /// <summary>
        /// De-OPs a given Player.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void DeopPlayer(ISender sender, ArgumentList args)
        {
            var playerName = args.GetString(0);
            if (Storage.IsAvailable)
            {
                var existing = Authentication.GetPlayer(playerName);
                if (existing != null)
                {
                    if (!existing.Operator)
                        throw new CommandError("Player is not an operator");

                    var player = Tools.GetPlayerByName(playerName);
                    if (player != null)
                    {
                        player.SendMessage("Your server operator privledges have been revoked.", Color.DarkRed);
                        player.SetOp(false);
                        player.SetAuthentication(null, "tdsm");
                    }

                    if (Authentication.UpdatePlayer(playerName, null, false))
                    {
                        sender.Message("Deop success", Color.DarkGreen);
                    }
                    else
                    {
                        sender.Message("Failed to deop player", Color.DarkRed);
                    }
                }
                else
                {
                    sender.SendMessage("No user found by " + playerName);
                }
            }
            else
            {
                if (Core.Ops.Contains(playerName))
                {
                    var player = Tools.GetPlayerByName(playerName);
                    if (player != null)
                    {
                        player.SendMessage("Your server operator privledges have been revoked.", Color.Green);
                        player.SetOp(false);
                        player.SetAuthentication(null, "tdsm");
                    }

                    Utils.NotifyAllOps("De-Opping " + playerName + " [" + sender.SenderName + "]", true);
                    Core.Ops.Remove(playerName, true);

                    if (!Core.Ops.Save())
                    {
                        Utils.NotifyAllOps("Failed to save op list [" + sender.SenderName + "]", true);
                    }
                }
                else
                    sender.SendMessage("No user found by " + playerName);
            }
        }

        /// <summary>
        /// Allows users to log in.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="password">Password for verification</param>
        public void Auth(ISender sender, string password)
        {
            if (sender is Player)
            {
                var player = sender as Player;
                if (Storage.IsAvailable)
                {
                    var existing = Authentication.GetPlayer(sender.SenderName);
                    if (existing != null)
                    {
                        if (existing.ComparePassword(sender.SenderName, password))
                        {
                            Utils.NotifyAllOps(
                                String.Format("{0} successfully logged in.", player.name)
                            );
                            player.SendMessage("Successfully logged in.", Color.DarkGreen);
                            player.SetAuthentication(sender.SenderName, "tdsm");
                            player.SetOp(existing.Operator);
                        }
                        else
                        {
                            sender.Message("Login failed", Color.DarkRed);
                        }
                    }
                    else
                    {
                        sender.Message("Login failed", Color.DarkRed);
                    }
                }
                else
                    sender.Message("This function is unavailable", Color.DarkRed);
            }
        }

        /// <summary>
        /// Allows players to logout.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void LogOut(ISender sender, ArgumentList args)
        {
            if (sender is Player)
            {
                var player = sender as Player;
                if (player.IsOp())
                {
                    player.SetOp(false);
                    player.SendMessage("Successfully logged out", Color.DarkRed);
                    player.SetAuthentication(String.Empty, "tdsm");

                    Utils.NotifyAllOps(
                        String.Format("{0} successfully logged out.", player.name)
                    );
                }
                else
                {
                    player.SendMessage("You are not logged in.", Color.DarkRed);
                }
            }
        }
    }
}

