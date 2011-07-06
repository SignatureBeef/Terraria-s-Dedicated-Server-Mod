﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria_Server.Events;
using Terraria_Server.Commands;

namespace Terraria_Server.Plugin
{
    /// <summary>
    /// Plugin class, used as base for plugin extensions
    /// </summary>
    public abstract class Plugin
    {

        /// <summary>
        /// Name of the plugin
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// Plugin description
        /// </summary>
        public String Description { get; set; }
        /// <summary>
        /// Plugin author
        /// </summary>
        public String Author { get; set; }
        /// <summary>
        /// Plugin version
        /// </summary>
        public String Version { get; set; }
        /// <summary>
        /// Latest compatible TDSM build
        /// </summary>
        public int TDSMBuild { get; set; } //Thinking about adding a warning for out of date plugins.

        /// <summary>
        /// Whether this plugin is enabled or not
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// Pointer to current server instance
        /// </summary>
        public Server Server { get; set; }

        /// <summary>
        /// Load routines, typically setting up plugin instances, initial values, etc; called before Enable() in startup
        /// </summary>
        public abstract void Load();
        //public void abstract UnLoad(); //I have high hopes :3
        /// <summary>
        /// Enable routines, usually no more than enabled announcement and registering hooks
        /// </summary>
        public abstract void Enable();
        /// <summary>
        /// Disabling the plugin, usually announcement
        /// </summary>
        public abstract void Disable();

        /// <summary>
        /// Hook method for console command; currently non-functional
        /// </summary>
        /// <param name="Event">ConsoleCommandEvent info</param>
        public virtual void onConsoleCommand(ConsoleCommandEvent Event) { }

        /// <summary>
        /// Hook method for door state changes
        /// </summary>
        /// <param name="Event">DoorStateChangeEvent info</param>
        public virtual void onDoorStateChange(DoorStateChangeEvent Event) { }

        /// <summary>
        /// Hook method for NPC deaths
        /// </summary>
        /// <param name="Event">NPCDeathEvent info</param>
        public virtual void onNPCDeath(NPCDeathEvent Event) { }

        /// <summary>
        /// Hook method for NPC summoning
        /// </summary>
        /// <param name="Event">NPCSpawnEvent info</param>
        public virtual void onNPCSpawn(NPCSpawnEvent Event) { }

        /// <summary>
        /// Hook method for player chat
        /// </summary>
        /// <param name="Event">MessageEvent info</param>
        public virtual void onPlayerChat(MessageEvent Event) { }

        /// <summary>
        /// Hook method for player command
        /// </summary>
        /// <param name="Event">PlayerCommandEvent info</param>
        public virtual void onPlayerCommand(PlayerCommandEvent Event) { }

        /// <summary>
        /// Hook method for a player death
        /// </summary>
        /// <param name="Event">PlayerDeathEvent info</param>
        public virtual void onPlayerDeath(PlayerDeathEvent Event) { }

        /// <summary>
        /// Hook method for all player-initiated sign edits
        /// </summary>
        /// <param name="Event">PlayerEditSignEvent info</param>
        public virtual void onPlayerEditSign(PlayerEditSignEvent Event) { }

        /// <summary>
        /// Hook method for player damage
        /// </summary>
        /// <param name="Event">PlayerHurtEvent info</param>
        public virtual void onPlayerHurt(PlayerHurtEvent Event) { }

        /// <summary>
        /// Hook method for player joining the server, as they're announced
        /// </summary>
        /// <param name="Event">PlayerLoginEvent info</param>
        public virtual void onPlayerJoin(PlayerLoginEvent Event) { }

        /// <summary>
        /// Hook method for player key press
        /// </summary>
        /// <param name="Event">PlayerKeyPressEvent info</param>
        public virtual void onPlayerKeyPress(PlayerKeyPressEvent Event) { }

        /// <summary>
        /// Hook method for player logging out
        /// </summary>
        /// <param name="Event">PlayerLogoutEvent info</param>
        public virtual void onPlayerLogout(PlayerLogoutEvent Event) { }

        /// <summary>
        /// Hook method for player movement
        /// </summary>
        /// <param name="Event">PlayerMoveEvent info</param>
        public virtual void onPlayerMove(PlayerMoveEvent Event) { }

        /// <summary>
        /// Hook method for player opening a chest
        /// </summary>
        /// <param name="Event">PlayerChestOpenEvent info</param>
        public virtual void onPlayerOpenChest(PlayerChestOpenEvent Event) { }

        /// <summary>
        /// Hook method for player changing parties
        /// </summary>
        /// <param name="Event">PartyChangeEvent info</param>
        public virtual void onPlayerPartyChange(PartyChangeEvent Event) { }

        /// <summary>
        /// Hook method for player's initial connection to the server, before they can start playing
        /// </summary>
        /// <param name="Event">PlayerLoginEvent info</param>
        public virtual void onPlayerPreLogin(PlayerLoginEvent Event) { }

        /// <summary>
        /// Hook method for all player projectile firing
        /// </summary>
        /// <param name="Event">PlayerProjectileEvent info</param>
        public virtual void onPlayerProjectileUse(PlayerProjectileEvent Event) { }

        /// <summary>
        /// Hook method for the player state (Byte) updating
        /// </summary>
        /// <param name="Event">PlayerStateUpdateEvent info</param>
        public virtual void onPlayerStateUpdate(PlayerStateUpdateEvent Event) { }

        /// <summary>
        /// Hook method for the player teleport
        /// </summary>
        /// <param name="Event">PlayerTeleportEvent info</param>
        public virtual void onPlayerTeleport(PlayerTeleportEvent Event) { }
        /// <summary>
        /// Hook method for any player-initiated tile changes (place, break, etc)
        /// </summary>
        /// <param name="Event">PlayerTileChangeEvent info</param>
        public virtual void onTileChange(PlayerTileChangeEvent Event) { }

        /// <summary>
        /// Hash list of all hooks currently registered by the plugin
        /// </summary>
        private HashSet<Hooks> pluginHooks = new HashSet<Hooks>();

        /// <summary>
        /// Checks to see whether the plugin has a hook registered.
        /// </summary>
        /// <param name="hook">Hooks.cs hook enum value</param>
        /// <returns>True if the hook is registered, false if not.</returns>
        public bool containsHook(Hooks hook)
        {
            return pluginHooks.Contains(hook);
        }

        /// <summary>
        /// Registers specified hook for the plugin.
        /// </summary>
        /// <param name="hook">Hooks.cs hook enum value</param>
        /// <returns>True if the hook was registered, false if not.</returns>
        public bool registerHook(Hooks hook)
        {
            if (!containsHook(hook))
            {
                pluginHooks.Add(hook);
                return true;
            }
            return false;
        }

    }
}
