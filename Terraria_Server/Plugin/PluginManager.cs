using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Reflection;
using Terraria_Server.Events;

namespace Terraria_Server.Plugin
{
    public class PluginManager
    {
        private string pluginPath = "";
        private ArrayList pluginList = null;
        private Server server = null;

        public PluginManager(string PluginPath, Server Server)
        {
            pluginPath = PluginPath;
            pluginList = new ArrayList();
            server = Server;
        }

        public Plugin loadPlugin(string PluginPath)
        {
            try
            {
                string name = new FileInfo(PluginPath).Name;
                Assembly pluginAssembly = Assembly.LoadFrom(PluginPath);
                if (name.Contains("."))
                {
                    name = name.Split('.')[0];
                }
                
                Type pluginType = pluginAssembly.GetType(name + "." + name);
                Plugin plugin = (Plugin)Activator.CreateInstance(pluginType);
                if (plugin == null)
                {
                    Console.WriteLine("Error Loading Plugin '" + PluginPath + "'. Is it up to Date?");
                }
                else
                {
                    plugin.Server = server;
                    plugin.Load();
                    return plugin;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error Loading Plugin '" + PluginPath + "' : "
                    + exception.ToString());
            }

            return null;
        }

        public void ReloadPlugins() {
            DisablePlugins();

            foreach (string file in Directory.GetFiles(pluginPath))
            {
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Extension.Equals(".dll"))
                {
                    Plugin plugin = loadPlugin(file);
                    if (plugin != null)
                    {
                        pluginList.Add(plugin);
                    }
                }
            }

            EnablePlugins();
        }

        public void EnablePlugins()
        {
            foreach (Plugin plugin in pluginList)
            {
                plugin.Enabled = true;
                plugin.Enable();
            }
        }

        public void DisablePlugins()
        {
            if (pluginList != null)
            {
                foreach (Plugin plugin in pluginList)
                {
                    plugin.Enabled = false;
                    plugin.Disable();
                }

                pluginList.Clear();
            }
        }

        public void processHook(Hooks Hook, Event Event)
		{
            foreach (Plugin plugin in pluginList)
			{
				try
				{
                    if (plugin.containsHook(Hook))
					{
                        switch (Hook)
                        {
                            case Hooks.CONSOLE_COMMAND:
                                {
                                    plugin.onPlayerCommandProcess((ConsoleCommandEvent)Event);
                                    break;
                                }
                            case Hooks.PLAYER_COMMAND:
                                {
                                    plugin.onPlayerCommand((PlayerCommandEvent)Event);
                                    break;
                                }
                            case Hooks.PLAYER_CHAT:
                                {
                                    plugin.onPlayerChat((PlayerChatEvent)Event);
                                    break;
                                }
                            case Hooks.PLAYER_PRELOGIN:
                                {
                                    plugin.onPlayerPreLogin((LoginEvent)Event);
                                    break;
                                }
                            case Hooks.PLAYER_LOGIN:
                                {
                                    plugin.onPlayerJoin((LoginEvent)Event);
                                    break;
                                }
                            case Hooks.PLAYER_LOGOUT:
                                {
                                    plugin.onPlayerLogout((LogoutEvent)Event);
                                    break;
                                }
                            case Hooks.PLAYER_PARTYCHANGE:
                                {
                                    plugin.onPlayerPartyChange((PartyChangeEvent)Event);
                                    break;
                                }
                            case Hooks.TILE_BREAK:
                                {
                                    plugin.onTileBreak((TileBreakEvent)Event);
                                    break;
                                }
                            case Hooks.PLAYER_HURT:
                                {
                                    plugin.onPlayerHurt((PlayerHurtEvent)Event);
                                    break;
                                }
                            case Hooks.PLAYER_CHEST:
                                {
                                    plugin.onPlayerOpenChest((ChestOpenEvent)Event);
                                    break;
                                }
                        }
					}
				}
				catch (Exception exception)
                {
                    Console.WriteLine("Error Passing Event " + Hook.ToString() + " to " + plugin.Name);
					Console.WriteLine(exception.ToString());
				}
			}
        }
    }
}
