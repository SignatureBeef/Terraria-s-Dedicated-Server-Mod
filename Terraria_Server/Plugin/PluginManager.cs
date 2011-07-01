using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Terraria_Server.Events;

namespace Terraria_Server.Plugin
{
    /*
     * Handles all Plugins
     *  - I would love to be able to unload a plugin from memory :3 (Todo?...Someone? xD)
     * 
     */

    public class PluginManager
    {
        private string pluginPath = String.Empty;
        private Dictionary<String, Plugin> plugins;
        private Server server;

        public PluginManager(string pluginPath, Server server)
        {
            this.pluginPath = pluginPath;
            this.server = server;

            plugins = new Dictionary<String, Plugin>();
        }

        /// <summary>
        /// Load the plugin located at the specified path.
        /// This only loads one plugin.
        /// </summary>
        /// <param name="PluginPath"></param>
        /// <returns></returns>
        public Plugin loadPlugin(string pluginPath)
        {
            try
            {
            	Type type = typeof(Plugin);
	            foreach(Type messageType in Assembly.LoadFrom(pluginPath).GetTypes()
	                .Where(x => type.IsAssignableFrom(x) && x != type))
	            {
	                Plugin plugin = (Plugin)Activator.CreateInstance(type);
                    if (plugin == null)
                    {
                        throw new Exception("Could not Instantiate");
                    }
                    else
                    {
                        plugin.Server = server;
                        plugin.Load();
                        return plugin;
                    }
	            }
            }
            catch (Exception exception)
            {
                Program.tConsole.WriteLine("Error Loading Plugin '" + pluginPath + "'. Is it up to Date?");
                Program.tConsole.WriteLine("Plugin Load Exception '" + pluginPath + "' : "
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
                    	plugins.Add(plugin.Name.ToLower().Trim(), plugin);
                    }
                }
            }
            EnablePlugins();
        }

        public void EnablePlugins()
        {
            foreach (Plugin plugin in plugins.Values)
            {
                plugin.Enabled = true;
                plugin.Enable();
            }
        }

        public void DisablePlugins()
        {
            foreach (Plugin plugin in plugins.Values)
            {
                plugin.Enabled = false;
                plugin.Disable();
            }

            plugins.Clear();
        }
        
        //Returns true on plugin successfully Enabling
        public bool EnablePlugin(string name)
        {
        	String cleanedName = name.ToLower().Trim();
        	if(plugins.ContainsKey(cleanedName))
        	{
	        	Plugin plugin = plugins[cleanedName];
	            plugin.Enabled = true;
	            plugin.Enable();
	            return true;
        	}
            return false;
        }

        //Returns true on plugin successfully Disabling
        public bool DisablePlugin(string name)
        {
        	String cleanedName = name.ToLower().Trim();
        	if(plugins.ContainsKey(cleanedName))
        	{
	        	Plugin plugin = plugins[cleanedName];
	            plugin.Enabled = false;
	            plugin.Disable();
	            return true;
        	}
            return false;
        }

        public Plugin getPlugin(string name)
        {
        	String cleanedName = name.ToLower().Trim();
        	if(plugins.ContainsKey(cleanedName))
        	{
        		return plugins[cleanedName];
        	}
        	return null;
        }
        
        public void processHook(Hooks hook, Event hookEvent)
		{
            foreach (Plugin plugin in plugins.Values)
			{
				try
				{
                    if (plugin.containsHook(hook))
					{
                        switch (hook)
                        {
                            case Hooks.CONSOLE_COMMAND:
                                {
                                    plugin.onPlayerCommandProcess((ConsoleCommandEvent)hookEvent);
                                    break;
                                }
                            case Hooks.PLAYER_COMMAND:
                                {
                                    plugin.onPlayerCommand((PlayerCommandEvent)hookEvent);
                                    break;
                                }
                            case Hooks.PLAYER_CHAT:
                                {
                                    plugin.onPlayerChat((MessageEvent)hookEvent);
                                    break;
                                }
                            case Hooks.PLAYER_PRELOGIN:
                                {
                                    plugin.onPlayerPreLogin((PlayerLoginEvent)hookEvent);
                                    break;
                                }
                            case Hooks.PLAYER_LOGIN:
                                {
                                    plugin.onPlayerJoin((PlayerLoginEvent)hookEvent);
                                    break;
                                }
                            case Hooks.PLAYER_LOGOUT:
                                {
                                    plugin.onPlayerLogout((PlayerLogoutEvent)hookEvent);
                                    break;
                                }
                            case Hooks.PLAYER_PARTYCHANGE:
                                {
                                    plugin.onPlayerPartyChange((PartyChangeEvent)hookEvent);
                                    break;
                                }
                            case Hooks.TILE_CHANGE:
                                {
                                    plugin.onTileChange((PlayerTileChangeEvent)hookEvent);
                                    break;
                                }
                            case Hooks.PLAYER_HURT:
                                {
                                    plugin.onPlayerHurt((PlayerHurtEvent)hookEvent);
                                    break;
                                }
                            case Hooks.PLAYER_CHEST:
                                {
                                    plugin.onPlayerOpenChest((PlayerChestOpenEvent)hookEvent);
                                    break;
                                }
                            case Hooks.PLAYER_STATEUPDATE:
                                {
                                    plugin.onPlayerStateUpdate((PlayerStateUpdateEvent)hookEvent);
                                    break;
                                }
                            case Hooks.PLAYER_DEATH:
                                {
                                    plugin.onPlayerDeath((PlayerDeathEvent)hookEvent);
                                    break;
                                }
                            case Hooks.DOOR_STATECHANGE:
                                {
                                    plugin.onDoorStateChange((DoorStateChangeEvent)hookEvent);
                                    break;
                                }
                            case Hooks.PLAYER_EDITSIGN:
                                {
                                    plugin.onPlayerEditSign((PlayerEditSignEvent)hookEvent);
                                    break;
                                }
                            case Hooks.PLAYER_PROJECTILE:
                                {
                                    plugin.onPlayerProjectileUse((PlayerProjectileEvent)hookEvent);
                                    break;
                                }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Program.tConsole.WriteLine("Error Passing Event " + hook.ToString() + " to " + plugin.Name);
                    Program.tConsole.WriteLine(exception.ToString());
                }
            }
        }
    }
}
