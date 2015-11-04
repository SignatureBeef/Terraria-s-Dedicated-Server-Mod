using System;
using OTA.Command;
using OTA;
using Terraria;
using OTA.Sockets;
using Microsoft.Xna.Framework;
using System.Text;
using System.IO;
using OTA.Plugin;
using System.Linq;

namespace TDSM.Core.Command.Commands
{
    public class PluginCommand : CoreCommand
    {
        public override void Initialise()
        {
            Core.AddCommand("plugin")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Manage and view plugins")
                .WithHelpText("list")
                .WithHelpText("stat")
                .WithHelpText("info <plugin>")
                .WithHelpText("enable <plugin>")
                .WithHelpText("disable <plugin>")
                .WithHelpText("reload [-clean] all|<plugin>")
                .WithHelpText("unload all|<plugin>")
                .WithHelpText("load [-replace] <file>")
                .WithPermissionNode("tdsm.plugin")
                .Calls(this.PluginManage);
        }

        public void PluginManage(ISender sender, ArgumentList args)
        {
            /*
             * Commands:
             *      list    - shows all plugins
             *      info    - shows a plugin's author & description etc
             *      disable - disables a plugin
             *      enable  - enables a plugin
             *      reload
             *      unload
             *      status
             *      load
             */

            if (args.Count == 0)
                throw new CommandError("Subcommand expected.");

            string command = args[0];
            args.RemoveAt(0); //Allow the commands to use any additional arguments without also getting the command

            lock (PluginManager._plugins)
                switch (command)
                {
                    case "-l":
                    case "ls":
                    case "list":
                        {
                            if (PluginManager.PluginCount == 0)
                            {
                                sender.Message(255, "No plugins loaded.");
                                return;
                            }

                            var msg = new StringBuilder();
                            msg.Append("Plugins: ");

                            int i = 0;
                            foreach (var plugin in PluginManager.EnumeratePlugins)
                            {
                                if (i > 0)
                                    msg.Append(", ");
                                msg.Append(plugin.Name);

                                if (!String.IsNullOrEmpty(plugin.Version))
                                {
                                    msg.Append(" (");
                                    msg.Append(plugin.Version);
                                    msg.Append(")");
                                }

                                if (!plugin.IsEnabled)
                                    msg.Append("[OFF]");
                                i++;
                            }
                            msg.Append(".");

                            sender.Message(255, Color.DodgerBlue, msg.ToString());

                            break;
                        }

                    case "-s":
                    case "stat":
                    case "status":
                        {
                            if (PluginManager.PluginCount == 0)
                            {
                                sender.Message(255, "No plugins loaded.");
                                return;
                            }

                            var msg = new StringBuilder();

                            foreach (var plugin in PluginManager.EnumeratePlugins)
                            {
                                msg.Clear();
                                msg.Append(plugin.IsDisposed ? "[DISPOSED] " : (plugin.IsEnabled ? "[ON]  " : "[OFF] "));
                                msg.Append(plugin.Name);
                                msg.Append(" ");
                                msg.Append(plugin.Version);
                                if (plugin.Status != null && plugin.Status.Length > 0)
                                {
                                    msg.Append(" : ");
                                    msg.Append(plugin.Status);
                                }
                                sender.Message(255, Color.DodgerBlue, msg.ToString());
                            }

                            break;
                        }

                    case "-i":
                    case "info":
                        {
                            string name;
                            args.ParseOne(out name);

                            var fplugin = PluginManager.GetPlugin(name);
                            if (fplugin != null)
                            {
                                var path = Path.GetFileName(fplugin.FilePath);
                                sender.Message(255, Color.DodgerBlue, fplugin.Name);
                                sender.Message(255, Color.DodgerBlue, "Filename: " + path);
                                sender.Message(255, Color.DodgerBlue, "Version:  " + fplugin.Version);
                                sender.Message(255, Color.DodgerBlue, "Author:   " + fplugin.Author);
                                if (fplugin.Description != null && fplugin.Description.Length > 0)
                                    sender.Message(255, Color.DodgerBlue, fplugin.Description);
                                sender.Message(255, Color.DodgerBlue, "Status:   " + (fplugin.IsEnabled ? "[ON] " : "[OFF] ") + fplugin.Status);
                            }
                            else
                            {
                                sender.SendMessage("The plugin \"" + args[1] + "\" was not found.");
                            }

                            break;
                        }

                    case "-d":
                    case "disable":
                        {
                            string name;
                            args.ParseOne(out name);

                            var fplugin = PluginManager.GetPlugin(name);
                            if (fplugin != null)
                            {
                                if (fplugin.Disable())
                                    sender.Message(255, Color.DodgerBlue, fplugin.Name + " was disabled.");
                                else
                                    sender.Message(255, Color.DodgerBlue, fplugin.Name + " was disabled, errors occured during the process.");
                            }
                            else
                                sender.Message(255, "The plugin \"" + name + "\" could not be found.");

                            break;
                        }

                    case "-e":
                    case "enable":
                        {
                            string name;
                            args.ParseOne(out name);

                            var fplugin = PluginManager.GetPlugin(name);
                            if (fplugin != null)
                            {
                                if (fplugin.Enable())
                                    sender.Message(255, Color.DodgerBlue, fplugin.Name + " was enabled.");
                                else
                                    sender.Message(255, Color.DodgerBlue, fplugin.Name + " was enabled, errors occured during the process.");
                            }
                            else
                                sender.Message(255, "The plugin \"" + name + "\" could not be found.");

                            break;
                        }

                    case "-u":
                    case "-ua":
                    case "unload":
                        {
                            string name;

                            if (command == "-ua" || command == "-uca")
                                name = "all";
                            else
                                args.ParseOne(out name);

                            BasePlugin[] plugs;
                            if (name == "all" || name == "-a")
                            {
                                plugs = PluginManager._plugins.Values.ToArray();
                            }
                            else
                            {
                                var splugin = PluginManager.GetPlugin(name);

                                if (splugin == null)
                                {
                                    sender.Message(255, "The plugin \"" + name + "\" could not be found.");
                                    return;
                                }

                                plugs = new BasePlugin[] { splugin };
                            }

                            foreach (var fplugin in plugs)
                            {
                                if (PluginManager.UnloadPlugin(fplugin))
                                    sender.Message(255, Color.DodgerBlue, fplugin.Name + " was unloaded.");
                                else
                                    sender.Message(255, Color.DodgerBlue, fplugin.Name + " was unloaded, errors occured during the process.");
                            }

                            break;
                        }

                    case "-r":
                    case "-rc":
                    case "-ra":
                    case "-rca":
                    case "reload":
                        {
                            bool save = true;
                            if (command == "-rc" || command == "-rca" || args.TryPop("-c") || args.TryPop("-clean"))
                                save = false;

                            string name;

                            if (command == "-ra" || command == "-rca")
                                name = "all";
                            else
                                args.ParseOne(out name);

                            BasePlugin[] plugs;
                            if (name == "all" || name == "-a")
                            {
                                plugs = PluginManager._plugins.Values.ToArray();
                            }
                            else
                            {
                                var splugin = PluginManager.GetPlugin(name);

                                if (splugin == null)
                                {
                                    sender.Message(255, "The plugin \"" + name + "\" could not be found.");
                                    return;
                                }

                                plugs = new BasePlugin[] { splugin };
                            }

                            foreach (var fplugin in plugs)
                            {
                                var nplugin = PluginManager.ReloadPlugin(fplugin, save);
                                if (nplugin == fplugin)
                                {
                                    sender.Message(255, Color.DodgerBlue, "Errors occured while reloading plugin " + fplugin.Name + ", old instance kept.");
                                }
                                else if (nplugin == null)
                                {
                                    sender.Message(255, Color.DodgerBlue, "Errors occured while reloading plugin " + fplugin.Name + ", it has been unloaded.");
                                }
                            }

                            break;
                        }

                    case "-L":
                    case "-LR":
                    case "load":
                        {
                            bool replace = command == "-LR" || args.TryPop("-R") || args.TryPop("-replace");
                            bool save = command != "-LRc" && !args.TryPop("-c") && !args.TryPop("-clean");

                            var fname = string.Join(" ", args);
                            string path;

                            if (fname == "")
                                throw new CommandError("File name expected");

                            if (Path.IsPathRooted(fname))
                                path = Path.GetFullPath(fname);
                            else
                                path = Path.Combine(Globals.PluginPath, fname);

                            var fi = new FileInfo(path);

                            if (!fi.Exists)
                            {
                                sender.Message(255, "Specified file doesn't exist.");
                                return;
                            }

                            var newPlugin = PluginManager.LoadPluginFromPath(path);

                            if (newPlugin == null)
                            {
                                sender.Message(255, "Unable to load plugin.");
                                return;
                            }

                            var oldPlugin = PluginManager.GetPlugin(newPlugin.Name);
                            if (oldPlugin != null)
                            {
                                if (!replace)
                                {
                                    sender.Message(255, "A plugin named {0} is already loaded, use -replace to replace it.", oldPlugin.Name);
                                    return;
                                }

                                if (PluginManager.ReplacePlugin(oldPlugin, newPlugin, save))
                                {
                                    sender.Message(255, Color.DodgerBlue, "Plugin {0} has been replaced.", oldPlugin.Name);
                                }
                                else if (oldPlugin.IsDisposed)
                                {
                                    sender.Message(255, Color.DodgerBlue, "Replacement of plugin {0} failed, it has been unloaded.", oldPlugin.Name);
                                }
                                else
                                {
                                    sender.Message(255, Color.DodgerBlue, "Replacement of plugin {0} failed, old instance kept.", oldPlugin.Name);
                                }

                                return;
                            }

                            if (!newPlugin.InitializeAndHookUp())
                            {
                                sender.Message(255, Color.DodgerBlue, "Failed to initialize new plugin instance.");
                            }

                            PluginManager._plugins.Add(newPlugin.Name.ToLower().Trim(), newPlugin);

                            if (!newPlugin.Enable())
                            {
                                sender.Message(255, Color.DodgerBlue, "Failed to enable new plugin instance.");
                            }

                            sender.Message(255, Color.DodgerBlue, "New plugin instance loaded.");
                            break;
                        }

                    default:
                        {
                            throw new CommandError("Subcommand not recognized.");
                        }
                }
        }
    }
}

