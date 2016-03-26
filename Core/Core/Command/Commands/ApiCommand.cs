using System;
using OTA.Command;
using Terraria;
using OTA;
using TDSM.Core.Data;
using TDSM.Core.Data.Management;
using TDSM.Core.Data.Models;

namespace TDSM.Core.Command.Commands
{
    public class ApiCommand : CoreCommand
    {
        public override void Initialise()
        {
            AddCommand("api")
                .WithPermissionNode("tdsm.api")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Manage API accounts")
                .WithHelpText("addaccount <username> <password>")
                .WithHelpText("addrole <account> <type> <value>")
                .WithHelpText("removeaccount <username>")
                .WithHelpText("removerole <account> <type> <value>")
                .WithHelpText("search <term>")
                .Calls(ManageApi);
        }

        void ManageApi(ISender sender, ArgumentList args)
        {
            if (!Storage.IsAvailable)
                throw new CommandError("No permissions plugin or data plugin is attached");

            var a = 0;
            string name, pass, type, value;
            APIAccount acc = null;
            var cmd = args.GetString(a++);

            switch (cmd)
            {
                case "addaccount":
                    //api addaccount "username" "password"
                    if (!args.TryGetString(a++, out name))
                        throw new CommandError("Expected username after [" + cmd + "]");

                    if (!args.TryGetString(a++, out pass))
                        throw new CommandError("Expected password after username");

                    acc = APIAccountManager.FindByName(name);
                    if (acc == null)
                    {
                        acc = APIAccountManager.Create(name, pass);
                        if (acc.Id > 0)
                        {
                            sender.SendMessage("Successfully created account.", R: 0, B: 0);
                        }
                        else
                        {
                            sender.SendMessage("Failed to create account.", G: 0, B: 0);
                        }
                    }
                    else
                    {
                        throw new CommandError("Existing API account found by " + name);
                    }
                    break;
                case "removeaccount":
                    //api removeaccount "username"
                    if (!args.TryGetString(a++, out name))
                        throw new CommandError("Expected username after [" + cmd + "]");

                    acc = APIAccountManager.FindByName(name);
                    if (acc != null)
                    {
                        if (APIAccountManager.DeleteAccount(acc.Id))
                        {
                            sender.SendMessage("Successfully removed account.", R: 0, B: 0);
                        }
                        else
                        {
                            sender.SendMessage("Failed to remove account.", G: 0, B: 0);
                        }
                    }
                    else
                    {
                        throw new CommandError("No API account found by " + name);
                    }
                    break;
                case "addrole":
                    //api addrole "account" "type" "value"
                    if (!args.TryGetString(a++, out name))
                        throw new CommandError("Expected username after [" + cmd + "]");

                    if (!args.TryGetString(a++, out type))
                        throw new CommandError("Expected type after username");

                    if (!args.TryGetString(a++, out value))
                        throw new CommandError("Expected value after type");

                    acc = APIAccountManager.FindByName(name);
                    if (acc != null)
                    {
                        var role = APIAccountManager.AddType(acc.Id, type, value);
                        if (role != null && role.Id > 0)
                        {
                            sender.SendMessage("Successfully added role account.", R: 0, B: 0);
                        }
                        else
                        {
                            sender.SendMessage("Failed to add role to account.", G: 0, B: 0);
                        }
                    }
                    else
                    {
                        throw new CommandError("No API account found by " + name);
                    }
                    break;
                case "removerole":
                    //api removerole "account" "type" "value"
                    if (!args.TryGetString(a++, out name))
                        throw new CommandError("Expected username after [" + cmd + "]");

                    if (!args.TryGetString(a++, out type))
                        throw new CommandError("Expected type after username");

                    if (!args.TryGetString(a++, out value))
                        throw new CommandError("Expected value after type");

                    acc = APIAccountManager.FindByName(name);
                    if (acc != null)
                    {
                        var role = APIAccountManager.DeleteType(acc.Id, type, value);
                        if (role)
                        {
                            sender.SendMessage("Successfully removed role account.", R: 0, B: 0);
                        }
                        else
                        {
                            sender.SendMessage("Failed to removed role from account.", G: 0, B: 0);
                        }
                    }
                    else
                    {
                        throw new CommandError("No API account found by " + name);
                    }
                    break;

                case "search":
                    //api search "part"
                    if (!args.TryGetString(a++, out name))
                        throw new CommandError("Expected part of a acount name after [" + cmd + "]");

                    var matches = APIAccountManager.FindAccountsByPrefix(name);
                    if (matches != null && matches.Length > 0)
                    {
                        sender.Message("Matches:");
                        foreach (var mth in matches)
                        {
                            sender.Message("\t" + mth);
                        }
                    }
                    else
                    {
                        sender.Message("There are no registered accounts matching " + name);
                    }
                    break;
                default:
                    throw new CommandError("Invalid command " + cmd);
            }
        }
    }
}

