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
    /*
     * Handles all Plugins
     *  - I would love to be able to unload a plugin from memory :3 (Todo?...Someone? xD)
     * 
     */

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
                                throw new Exception("Could not Instanciate");
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
        public bool EnablePlugin(string Name)
        {
            foreach (Plugin plugin in pluginList)
            {
                if (plugin.Name == Name)
                {
                    plugin.Enabled = true;
                    plugin.Enable();
                    return true;
                }
            }
            return false;
        }

        //Returns true on plugin successfully Disabling
        public bool DisablePlugin(string Name)
        {
            if (pluginList != null)
            {
                Plugin dPlugin = null;
                foreach (Plugin plugin in pluginList)
                {
                    if (plugin.Name == Name)
                    {
                        dPlugin = plugin;
                        break;
                    }
                }

                if (dPlugin != null) 
                {
                    dPlugin.Enabled = false;
                    dPlugin.Disable();

                    //pluginList.Remove(dPlugin); //Not sure if i should remove it from the list.

                    return true;
                }
            }
            return false;
        }

        public Plugin getPlugin(string Name)
        {
            foreach (Plugin plugin in pluginList)
            {
                if(plugin.Name.Trim().ToLower().Equals(Name.Trim().ToLower())) {
                    return plugin;
                }
            }
            return null;
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
                                    plugin.onPlayerPreLogin((PlayerLoginEvent)Event);
                                    break;
                                }
                            case Hooks.PLAYER_LOGIN:
                                {
                                    plugin.onPlayerJoin((PlayerLoginEvent)Event);
                                    break;
                                }
                            case Hooks.PLAYER_LOGOUT:
                                {
                                    plugin.onPlayerLogout((PlayerLogoutEvent)Event);
                                    break;
                                }
                            case Hooks.PLAYER_PARTYCHANGE:
                                {
                                    plugin.onPlayerPartyChange((PartyChangeEvent)Event);
                                    break;
                                }
                            case Hooks.TILE_CHANGE:
                                {
                                    plugin.onTileChange((PlayerTileChangeEvent)Event);
                                    break;
                                }
                            case Hooks.PLAYER_HURT:
                                {
                                    plugin.onPlayerHurt((PlayerHurtEvent)Event);
                                    break;
                                }
                            case Hooks.PLAYER_CHEST:
                                {
                                    plugin.onPlayerOpenChest((PlayerChestOpenEvent)Event);
                                    break;
                                }
                            case Hooks.PLAYER_STATEUPDATE:
                                {
                                    plugin.onPlayerStateUpdate((PlayerStateUpdateEvent)Event);
                                    break;
                                }
                            case Hooks.PLAYER_DEATH:
                                {
                                    plugin.onPlayerDeath((PlayerDeathEvent)Event);
                                    break;
                                }
                            case Hooks.DOOR_STATECHANGE:
                                {
                                    plugin.onDoorStateChange((DoorStateChangeEvent)Event);
                                    break;
                                }
                            case Hooks.PLAYER_EDITSIGN:
                                {
                                    plugin.onPlayerEditSign((PlayerEditSignEvent)Event);
                                    break;
                                }
                            case Hooks.PLAYER_PROJECTILE:
                                {
                                    plugin.onPlayerProjectileUse((PlayerProjectileEvent)Event);
                                    break;
                                }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Program.tConsole.WriteLine("Error Passing Event " + Hook.ToString() + " to " + plugin.Name);
                    Program.tConsole.WriteLine(exception.ToString());
                }
            }
        }
    }
}
