using System;
using tdsm.api;
using tdsm.api.Command;

namespace tdsm.core
{
    public partial class Entry
    {
        public void RepositoryCommand(ISender sender, ArgumentList args)
        {
            var cmd = args.GetString(0);
            string pkg;
            bool all;
            switch (cmd)
            {
                /*case "search": Lets see how many plugins we have
                    break;*/
#if DEBUG
                case "check":
                    //Purpose is to check if all plugins are supported by the latest heartbeat TDSM version
                    //Gather plugin names + version and send to heartbeat in one request

                    //Heartbeat responds json messages direct to console:
                    // [
                    //  {
                    //      Colour: {},
                    //      Message: "TDSM Core [READY]"
                    //  },
                    //  {
                    //      Colour: {},
                    //      Message: "Restrict [No update]"
                    //  }
                    // ]
                    sender.Message("Not yet implemented");
                    break;
#endif
                case "status":
                    pkg = args.GetString(1);
                    all = pkg == "-all";

                    sender.Message("Finding updates...", Microsoft.Xna.Framework.Color.Yellow);

                    var packages = Repository.GetAvailableUpdate(all ? null : pkg);
                    if (packages != null && packages.Length > 0)
                    {
                        sender.Message("Found update{0}:", Microsoft.Xna.Framework.Color.Yellow, packages.Length > 1 ? "s" : String.Empty);
                        foreach (var upd in packages)
                        {
                            var vers = String.Empty;
                            if (!String.IsNullOrEmpty(upd.Version)) vers = " [" + upd.Version + "]";

                            var desc = String.Empty;
                            if (!String.IsNullOrEmpty(upd.ShortDescription)) desc = " - " + upd.ShortDescription;

                            sender.Message(upd.Name + vers + desc, Microsoft.Xna.Framework.Color.Green);
                        }
                    }
                    else
                    {
                        sender.Message(all ? "No updates available" : ("No package called: " + pkg), Microsoft.Xna.Framework.Color.Red);
                    }
                    break;
                case "update":
                    pkg = args.GetString(1);
                    all = pkg == "-all";

                    sender.Message("Finding updates...", Microsoft.Xna.Framework.Color.Yellow);

                    try
                    {
                        var updates = Repository.GetAvailableUpdate(all ? null : pkg);
                        if (updates != null && updates.Length > 0)
                        {
                            if (!all && updates.Length > 1)
                            {
                                sender.Message("Too many packages found, please inform the TDSM Team", Microsoft.Xna.Framework.Color.Red);
                                return;
                            }

                            sender.Message("Found {0} update{1}:", Microsoft.Xna.Framework.Color.Yellow, updates.Length, updates.Length > 1 ? "s" : String.Empty);
                            foreach (var upd in updates)
                            {
                                sender.Message("Updating {0}", Microsoft.Xna.Framework.Color.Green, upd.Name);

                                try
                                {
                                    if (Repository.PerformUpdate(upd))
                                    {
                                        sender.Message("Install complete", Microsoft.Xna.Framework.Color.Green);
                                    }
                                    else
                                    {
                                        sender.Message("Install failed", Microsoft.Xna.Framework.Color.Red);
                                    }
                                }
                                catch (RepositoryError err)
                                {
                                    sender.Message(err.Message, Microsoft.Xna.Framework.Color.Red);
                                }
                            }
                        }
                        else
                        {
                            sender.Message(all ? "No updates available" : ("No update or package for: " + pkg), Microsoft.Xna.Framework.Color.Red);
                        }
                    }
                    catch (RepositoryError err)
                    {
                        sender.Message(err.Message, Microsoft.Xna.Framework.Color.Red);
                    }
                    break;
                case "install":
                    pkg = args.GetString(1);

                    sender.Message("Finding package...", Microsoft.Xna.Framework.Color.Yellow);

                    try
                    {
                        var install = Repository.GetAvailableUpdate(pkg, false);
                        if (install != null && install.Length > 0)
                        {
                            if (install.Length > 1)
                            {
                                sender.Message("Too many packages found, please inform the TDSM Team", Microsoft.Xna.Framework.Color.Red);
                                return;
                            }

                            //Ensure not already installed
                            foreach (var plg in PluginManager.EnumeratePlugins)
                            {
                                if (plg.Name.ToLower() == pkg)
                                {
                                    sender.Message("Package already installed, please use 'repo update \"" + pkg + "\"'", Microsoft.Xna.Framework.Color.Red);
                                    break;
                                }
                            }

                            foreach (var upd in install)
                            {
                                sender.Message("Installing {0}", Microsoft.Xna.Framework.Color.Green, upd.Name);

                                try
                                {
                                    if (Repository.PerformUpdate(upd))
                                    {
                                        sender.Message("Install complete", Microsoft.Xna.Framework.Color.Green);
                                    }
                                    else
                                    {
                                        sender.Message("Install failed", Microsoft.Xna.Framework.Color.Red);
                                    }
                                }
                                catch (RepositoryError err)
                                {
                                    sender.Message(err.Message, Microsoft.Xna.Framework.Color.Red);
                                }
                            }
                        }
                        else
                        {
                            sender.Message("No package called: " + pkg, Microsoft.Xna.Framework.Color.Red);
                        }
                    }
                    catch (RepositoryError err)
                    {
                        sender.Message(err.Message, Microsoft.Xna.Framework.Color.Red);
                    }
                    break;
                default:
                    throw new CommandError("No such command: " + cmd);
            }
        }

    }
}
