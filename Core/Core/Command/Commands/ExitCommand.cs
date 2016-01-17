using System;
using OTA.Command;
using OTA;
using Terraria;
using OTA.Sockets;

namespace TDSM.Core.Command.Commands
{
    public class ExitCommand : CoreCommand
    {
        public override void Initialise()
        {
            AddCommand("exit", true)
                .WithDescription("Stops the server")
                .WithAccessLevel(AccessLevel.CONSOLE)
                .SetDefaultUsage()
                .WithPermissionNode("tdsm.admin")
                .Calls(this.Exit);

            AddCommand("stop") //TODO Add alias
                .WithDescription("Stops the server")
                .WithAccessLevel(AccessLevel.CONSOLE)
                .SetDefaultUsage()
                .WithPermissionNode("tdsm.admin")
                .Calls(this.Exit);
        }

        /// <summary>
        /// Closes the Server all connections.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void Exit(ISender sender, ArgumentList args)
        {
            var accessLevel = Core.Config.ExitAccessLevel;
            if (accessLevel == -1 && sender is Player)
            {
                sender.Message("You cannot perform that action.", 255, 238, 130, 238);
                return;
            }
            else if (!sender.HasAccessLevel((AccessLevel)accessLevel))
            {
                sender.Message("You cannot perform that action.", 255, 238, 130, 238);
                return;
            }

            string message;
            args.TryGetString(0, out message);

            if (String.IsNullOrEmpty(message))
                message = "Server is going down";

            //            args.ParseNone();

            Utils.NotifyAllOps("Exiting on request.");

            if (Netplay.anyClients)
            {
                for (var x = 0; x < Main.player.Length; x++)
                {
                    if (Main.player[x].active)
                    {
                        NetMessage.SendData((int)Packet.DISCONNECT, x, -1, message);

                        var rc = Netplay.Clients[x];
                        if (rc != null && rc.Socket != null && rc.Socket is ClientConnection)
                        {
                            (rc.Socket as ClientConnection).Flush();
                        }
                    }
                }

                //Prevent further connections
                Terraria.Netplay.Connection.Socket.StopListening();

                //                //Wait for total disconnection
                //                while (Netplay.anyClients)
                //                {
                //                    System.Threading.Thread.Sleep(100);
                //                }
            }

            Terraria.IO.WorldFile.saveWorld(false);
            Terraria.Netplay.disconnect = true;

            throw new OTA.Misc.ExitException(sender.SenderName + " requested that TDSM is to shutdown.");
        }
    }
}

