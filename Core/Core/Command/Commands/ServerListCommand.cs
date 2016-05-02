using System;
using OTA;
using Terraria;
using OTA.Command;
using TDSM.Core.Net.Web;
using OTA.Config;

namespace TDSM.Core.Command.Commands
{
    public class ServerListCommand : CoreCommand
    {
        public override void Initialise()
        {
            AddCommand("serverlist")
                .WithDescription("Manages the heartbeat and server list")
                .WithAccessLevel(AccessLevel.OP)
                .WithHelpText("print|?              - Displays the current details")
                .WithHelpText("enable|disable       - Enable/disable the heartbeat")
                .WithHelpText("public true|false    - Allows public viewing")
                .WithHelpText("desc|name|domain     - Displays the current")
                .WithHelpText("desc <description>")
                .WithHelpText("name <name>")
                .WithHelpText("domain <domain>")
                .WithPermissionNode("tdsm.serverlist")
                .Calls(this.ServerList);
        }

        /// <summary>
        /// Manages the server list
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void ServerList(ISender sender, ArgumentList args)
        {
            string first;
            args.TryPopOne(out first);
            switch (first)
            {
                case "enable":
                    if (!Heartbeat.Enabled)
                    {
                        Heartbeat.Begin(Entry.CoreBuild);
                        sender.SendMessage("Heartbeat enabled to the TDSM server.");
                    }
                    else
                    {
                        sender.SendMessage("Heartbeat is already enabled.");
                    }
                    break;
                case "disable":
                    if (Heartbeat.Enabled)
                    {
                        Heartbeat.End();
                        sender.SendMessage("Heartbeat disabled to the TDSM server.");
                    }
                    else
                    {
                        sender.SendMessage("Heartbeat is not enabled.");
                    }
                    break;
                case "public":
                    Heartbeat.PublishToList = args.GetBool(0);
                    if (!ConfigUpdater.IsAvailable || ConfigUpdater.Set("server-list", Heartbeat.PublishToList))
                        sender.SendMessage("Server list is now " + (Heartbeat.PublishToList ? "public" : "private"));
                    else sender.Message("Failed to update visibility");
                    break;
                case "desc":
                    string d;
                    if (args.TryPopOne(out d))
                    {
                        Heartbeat.ServerDescription = d;
                        if (!ConfigUpdater.IsAvailable || ConfigUpdater.Set("server-list-desc", d))
                            sender.SendMessage("Description set to: " + Heartbeat.ServerDescription);
                        else sender.Message("Failed to update description");
                    }
                    else
                        sender.SendMessage("Current description: " + Heartbeat.ServerDescription);
                    break;
                case "name":
                    string n;
                    if (args.TryPopOne(out n))
                    {
                        Heartbeat.ServerName = n;
                        if (!ConfigUpdater.IsAvailable || ConfigUpdater.Set("server-list-name", n))
                            sender.SendMessage("Name set to: " + Heartbeat.ServerName);
                        else sender.Message("Failed to update name");
                    }
                    else
                        sender.SendMessage("Current name: " + Heartbeat.ServerName);
                    break;
                case "domain":
                    string h;
                    if (args.TryPopOne(out h))
                    {
                        Heartbeat.ServerDomain = h;
                        if (!ConfigUpdater.IsAvailable || ConfigUpdater.Set("server-list-domain", h))
                            sender.SendMessage("Domain set to: " + Heartbeat.ServerDomain);
                        else sender.Message("Failed to update domain");
                    }
                    else
                        sender.SendMessage("Current domain: " + Heartbeat.ServerDomain);
                    break;
                case "?":
                case "print":
                    sender.SendMessage("Heartbeat is " + (Heartbeat.Enabled ? "enabled" : "disabled"));
                    if (Heartbeat.LastBeat.HasValue)
                        sender.SendMessage("Last beat: " + Heartbeat.LastBeat);
                    else
                        sender.SendMessage("Last beat: n/a");
                    sender.SendMessage("Server list " + (Heartbeat.PublishToList ? "public" : "private"));
                    sender.SendMessage("Current name: " + Heartbeat.ServerName);
                    sender.SendMessage("Current domain: " + Heartbeat.ServerDomain);
                    sender.SendMessage("Current description: " + Heartbeat.ServerDescription);
                    break;
                default:
                    throw new CommandError("Not a supported serverlist command " + first);
            }
        }
    }
}

