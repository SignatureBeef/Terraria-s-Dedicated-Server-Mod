using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Terraria_Server.Events;
using System.Diagnostics;
using System.Reflection.Emit;

namespace Terraria_Server.Plugin
{
    /*
     * Handles all Plugins
     *  - I would love to be able to unload a plugin from memory :3 (Todo?...Someone? xD)
     * 
     */
    /// <summary>
    /// PluginManager class.  Handles all input/output, loading, enabling, disabling, and hook processing for plugins
    /// </summary>
    public class PluginManager
    {
        private String pluginPath = String.Empty;
        private Dictionary<String, Plugin> plugins;
        private Server server;
        /// <summary>
        /// PluginManager class constructor
        /// </summary>
        /// <param name="pluginPath">Path to plugin directory</param>
        /// <param name="server">Current Server instance</param>
        public PluginManager(String pluginPath, Server server)
        {
            this.pluginPath = pluginPath;
            this.server = server;

            plugins = new Dictionary<String, Plugin>();
        }

        /// <summary>
        /// Initializes Plugin (Loads) and Checks for Out of Date Plugins.
        /// </summary>
        public void LoadPlugins()
        {
            ReloadPlugins();

            foreach (Plugin plugin in plugins.Values)
            {
                if (plugin.TDSMBuild != Statics.BUILD)
                {
                    Program.tConsole.WriteLine("[WARNING] Plugin Build Incorrect: " + plugin.Name);
                }
            }
        }
                
        /// <summary>
        /// Load the plugin located at the specified path.
        /// This only loads one plugin.
        /// </summary>
        /// <param name="pluginPath">Path to plugin</param>
        /// <returns>Instance of the successfully loaded plugin, otherwise null</returns>
        public Plugin loadPlugin(String pluginPath)
        {
            try
            {
            	Type type = typeof(Plugin);
                foreach (Type messageType in Assembly.LoadFrom(pluginPath).GetTypes()
	                .Where(x => type.IsAssignableFrom(x) && x != type))
	            {
                    if (!messageType.IsAbstract)
                    {
                        Plugin plugin = (Plugin)Activator.CreateInstance(messageType);
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
            }
            catch (Exception exception)
            {
                Program.tConsole.WriteLine("Error Loading Plugin '" + pluginPath + "'. Is it up to Date?");
                Program.tConsole.WriteLine("Plugin Load Exception '" + pluginPath + "' : "
                    + exception.ToString());
            }

            return null;
        }

        /// <summary>
        /// Reloads all plugins currently running on the server
        /// </summary>
        public void ReloadPlugins() {
            DisablePlugins();

            foreach (String file in Directory.GetFiles(pluginPath))
            {
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Extension.ToLower().Equals(".dll"))
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

        /// <summary>
        /// Enables all plugins available to the server
        /// </summary>
        public void EnablePlugins()
        {
            foreach (Plugin plugin in plugins.Values)
            {
                plugin.Enabled = true;
                plugin.Enable();
            }
        }

        /// <summary>
        /// Disables all plugins currently running on the server
        /// </summary>
        public void DisablePlugins()
        {
            foreach (Plugin plugin in plugins.Values)
            {
                plugin.Enabled = false;
                try
                {
                    plugin.Disable();
                }
                catch (Exception exception)
                {
                    Program.tConsole.WriteLine("Plugin Disable Exception '" + pluginPath + "' : "
                    + exception.ToString());
                }
            }

            plugins.Clear();
        }
        
        /// <summary>
        /// Enables a plugin by name. Currently unused in core
        /// </summary>
        /// <param name="name">Plugin name</param>
        /// <returns>Returns true on plugin successfully Enabling</returns>
        public bool EnablePlugin(String name)
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

        /// <summary>
        /// Disables a plugin by name.  Currently unused in core
        /// </summary>
        /// <param name="name">Name of plugin</param>
        /// <returns>Returns true on plugin successfully Disabling</returns>
        public bool DisablePlugin(String name)
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

        /// <summary>
        /// Gets plugin instance by name.
        /// </summary>
        /// <param name="name">Plugin name</param>
        /// <returns>Returns found plugin if successful, otherwise returns null</returns>
        public Plugin getPlugin(String name)
        {
        	String cleanedName = name.ToLower().Trim();
        	if(plugins.ContainsKey(cleanedName))
        	{
        		return plugins[cleanedName];
        	}
        	return null;
        }

        /// <summary>
        /// Gets the PluginManagers Loaded Plugins.
        /// </summary>
        /// <returns>Returns the current Dictionary of Plugins</returns>
        public Dictionary<String, Plugin> PluginList
        {
            get 
            {
                return plugins;
            }
        }
        
        /// <summary>
        /// Determines whether a plugins has registered a hook, then fires the appropriate method if so
        /// </summary>
        /// <param name="hook">Hook to process</param>
        /// <param name="hookEvent">Event instance to pass to any hooked methods</param>
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
                                    plugin.onConsoleCommand((ConsoleCommandEvent)hookEvent);
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
                            case Hooks.PLAYER_TILECHANGE:
                                {
                                    plugin.onPlayerTileChange((PlayerTileChangeEvent)hookEvent);
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
                            /*case Hooks.PLAYER_STATEUPDATE:
                                {
                                    plugin.onPlayerStateUpdate((PlayerStateUpdateEvent)hookEvent);
                                    break;
                                }*/
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
                            case Hooks.NPC_DEATH:
                                {
                                    plugin.onNPCDeath((NPCDeathEvent)hookEvent);
                                    break;
                                }
                            case Hooks.NPC_SPAWN:
                                {
                                    plugin.onNPCSpawn((NPCSpawnEvent)hookEvent);
                                    break;
                                }
                            case Hooks.PLAYER_TELEPORT:
                                {
                                    plugin.onPlayerTeleport((PlayerTeleportEvent)hookEvent);
                                    break;
                                }
                            case Hooks.PLAYER_MOVE:
                                {
                                    plugin.onPlayerMove((PlayerMoveEvent)hookEvent);
                                    break;
                                }
                            case Hooks.PLAYER_KEYPRESS:
                                {
                                    plugin.onPlayerKeyPress((PlayerKeyPressEvent)hookEvent);
                                    break;
                                }
                            case Hooks.PLAYER_PVPCHANGE:
                                {
                                    plugin.onPlayerPvPChange((PlayerPvPChangeEvent)hookEvent);
                                    break;
                                }

                            case Hooks.PLAYER_AUTH_QUERY:
                                plugin.onPlayerAuthQuery ((PlayerLoginEvent) hookEvent);
                                break;
                            
                            case Hooks.PLAYER_AUTH_REPLY:
                                plugin.onPlayerAuthReply ((PlayerLoginEvent) hookEvent);
                                break;

                            case Hooks.NPC_BOSSDEATH:
                                {
                                    plugin.onNPCBossDeath((NPCBossDeathEvent)hookEvent);
                                    break;
                                }
                            case Hooks.NPC_BOSSSUMMON:
                                {
                                    plugin.onNPCBossSummon((NPCBossSummonEvent)hookEvent);
                                    break;
                                }
                            case Hooks.PLAYER_FLOWLIQUID:
                                plugin.onPlayerFlowLiquid ((PlayerFlowLiquidEvent)hookEvent);
                                break;
                                
                            case Hooks.PLAYER_CHESTBREAK:
                                plugin.onPlayerChestBreak ((PlayerChestBreakEvent)hookEvent);
                                break;
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
