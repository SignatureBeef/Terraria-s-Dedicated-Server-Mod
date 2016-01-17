using System;
using OTA.Command;
using Terraria;
using OTA;

namespace TDSM.Core.Command.Commands
{
    public class TeleportCommand : CoreCommand
    {
        public override void Initialise()
        {
            AddCommand("tp")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Teleport a player to another player")
                .WithHelpText("<player> <toplayer> - another player")
                .WithHelpText("<player> <x> <y>")
                .WithHelpText("<toplayer>          - yourself")
                .WithHelpText("<x> <y>")
                .WithHelpText("                    - yourself to spawn")
                .WithPermissionNode("tdsm.tp")
                .Calls(this.Teleport);
        }

        /// <summary>
        /// Teleports player1 to a second specified player's location.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void Teleport(ISender sender, ArgumentList args)
        {
            Player subject;
            Player target;

            if (!args.TryPopOne(out subject))
            {
                subject = sender as Player;
                if (subject == null)
                {
                    sender.Message("Need a teleport target");
                    return;
                }

                if (args.Count == 0)
                {
                    if (subject.SpawnX > -1)
                    {
                        subject.Teleport(subject.SpawnX * 16f, subject.SpawnY * 16f - subject.height);
                        Utils.NotifyAllOps(String.Format("{0} has teleported home", subject.name), true);
                    }
                    else
                    {
                        subject.Teleport(Main.spawnTileX * 16f, Main.spawnTileY * 16f - subject.height);
                        Utils.NotifyAllOps(String.Format("{0} has teleported to spawn", subject.name), true);
                    }
                    return;
                }
            }
            else if (args.Count == 0)
            {
                target = subject;

                subject = sender as Player;
                if (subject == null)
                {
                    sender.Message("Need a teleport target");
                    return;
                }

                /*if (*/
                subject.Teleport(target); //)
                {
                    Utils.NotifyAllOps(String.Concat("Teleported ", subject.name, " to ",
                            target.name, ". [", sender.SenderName, "]"), true);
                }
                //else
                //    sender.Message(Languages.TeleportFailed);
                return;
            }

            int x;
            int y;

            if (args.Count == 1)
            {
                if (args.TryParseOne(out target))
                {
                    /*if (*/
                    subject.Teleport(target); //)
                    {
                        Utils.NotifyAllOps(string.Concat("Teleported ", subject.name, " to ",
                                target.name, ". [", sender.SenderName, "]"), true);
                    }
                    //else
                    //    sender.Message(Languages.TeleportFailed);
                }
                else
                    sender.Message("Cannot find player");
                return;
            }
            else if (args.Count == 2)
            {
                if (args.TryParseTwo(out x, out y))
                {
                    if (x < 0 || x >= Main.maxTilesX || y < 0 || y >= Main.maxTilesY)
                    {
                        sender.Message(String.Format("Coordinates out of range of (0, {0}); (0, {1}).", Main.maxTilesX - 1, Main.maxTilesY - 1));
                        return;
                    }
                    const Int32 OutOfBoundsPadding = 0; // 41;

                    /*if (*/
                    subject.Teleport((x - OutOfBoundsPadding) * 16f, (y - OutOfBoundsPadding) * 16f); //)
                    {
                        Utils.NotifyAllOps(string.Concat("Teleported ", subject.name, " to ",
                                x, ":", y, ". [", sender.SenderName, "]"), true);
                    }
                    //else
                    //    sender.Message(Languages.TeleportFailed);
                }
                else
                    throw new CommandError("Invalid coordinates");
                return;
            }

            throw new CommandError(String.Empty);
        }
    }
}

