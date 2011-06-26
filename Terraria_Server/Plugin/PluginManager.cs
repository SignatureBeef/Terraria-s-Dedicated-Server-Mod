using System;
using System.IO;
using System.Reflection;
using System.Collections;
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
        private ArrayList pluginList = null;
        private Server server = null;

        public PluginManager(string pluginPath, Server server)
        {
            this.pluginPath = pluginPath;
            this.server = server;

            pluginList = new ArrayList();
        }

        public Plugin loadPlugin(string PluginPath)
        {
            try
            {
                string name = new FileInfo(PluginPath).Name;
                Type[] types = Assembly.LoadFrom(PluginPath).GetTypes();
                for (int i = 0; i < types.Length; i++)
                {
                    Type type = types[i];
                    if (!type.IsAbstract)
                    {
                        Type baseType = type.BaseType;
                        if (baseType == typeof(Plugin))
                        {
                            Plugin plugin = (Plugin)Activator.CreateInstance(type);
                            if (plugin == null)
                            {
                                throw new Exception("Could not Intanciate");
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
                
                
            }
            catch (Exception exception)
            {
                Program.tConsole.WriteLine("Error Loading Plugin '" + PluginPath + "'. Is it up to Date?");
                Program.tConsole.WriteLine("Plugin Load Exception '" + PluginPath + "' : "
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
        
        //Returns true on plugin successfully Enabling
        public bool EnablePlugin(string name)
        {
            foreach (Plugin plugin in pluginList)
            {
                if (plugin.Name == name)
                {
                    plugin.Enabled = true;
                    plugin.Enable();
                    return true;
                }
            }
            return false;
        }

        //Returns true on plugin successfully Disabling
        public bool DisablePlugin(string name)
        {
            if (pluginList != null)
            {
                Plugin dPlugin = null;
                foreach (Plugin plugin in pluginList)
                {
                    if (plugin.Name == name)
                    {
                        dPlugin = plugin;
                        break;
                    }
                }

                if (dPlugin != null) 
                {
                    dPlugin.Enabled = false;
                    dPlugin.Disable();

                    return true;
                }
            }
            return false;
        }

        public Plugin getPlugin(string name)
        {
            foreach (Plugin plugin in pluginList)
            {
                if(plugin.Name.Trim().ToLower().Equals(name.Trim().ToLower())) {
                    return plugin;
                }
            }
            return null;
        }
        
        public void processHook(Hooks hook, Event hookEvent)
		{
            foreach (Plugin plugin in pluginList)
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
                                    plugin.onPlayerChat((PlayerChatEvent)hookEvent);
                                    break;
                                }
                            case Hooks.PLAYER_PRELOGIN:
                                {
                                    plugin.onPlayerPreLogin((LoginEvent)hookEvent);
                                    break;
                                }
                            case Hooks.PLAYER_LOGIN:
                                {
                                    plugin.onPlayerJoin((LoginEvent)hookEvent);
                                    break;
                                }
                            case Hooks.PLAYER_LOGOUT:
                                {
                                    plugin.onPlayerLogout((LogoutEvent)hookEvent);
                                    break;
                                }
                            case Hooks.PLAYER_PARTYCHANGE:
                                {
                                    plugin.onPlayerPartyChange((PartyChangeEvent)hookEvent);
                                    break;
                                }
                            case Hooks.TILE_BREAK:
                                {
                                    plugin.onTileBreak((TileBreakEvent)hookEvent);
                                    break;
                                }
                            case Hooks.PLAYER_HURT:
                                {
                                    plugin.onPlayerHurt((PlayerHurtEvent)hookEvent);
                                    break;
                                }
                            case Hooks.PLAYER_CHEST:
                                {
                                    plugin.onPlayerOpenChest((ChestOpenEvent)hookEvent);
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
