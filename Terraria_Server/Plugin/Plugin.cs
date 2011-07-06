using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria_Server.Events;
using Terraria_Server.Commands;

namespace Terraria_Server.Plugin
{
    public abstract class Plugin
    {

        public String Name { get; set; }
        public String Description { get; set; }
        public String Author { get; set; }
        public String Version { get; set; }
        public int TDSMBuild { get; set; } //Thinking about adding a warning for out of date plugins.

        public bool Enabled { get; set; }
        public Server Server { get; set; }

        public abstract void Load();
        public abstract void Enable();
        public abstract void Disable();

        public virtual void onConsoleCommand(ConsoleCommandEvent Event) { }
        public virtual void onDoorStateChange(DoorStateChangeEvent Event) { }
        public virtual void onNPCDeath(NPCDeathEvent Event) { }
        public virtual void onNPCSpawn(NPCSpawnEvent Event) { }
        public virtual void onPlayerChat(MessageEvent Event) { }
        public virtual void onPlayerCommand(PlayerCommandEvent Event) { }
        public virtual void onPlayerDeath(PlayerDeathEvent Event) { }
        public virtual void onPlayerEditSign(PlayerEditSignEvent Event) { }
        public virtual void onPlayerHurt(PlayerHurtEvent Event) { }
        public virtual void onPlayerJoin(PlayerLoginEvent Event) { }
        public virtual void onPlayerKeyPress(PlayerKeyPressEvent Event) { }
        public virtual void onPlayerLogout(PlayerLogoutEvent Event) { }
        public virtual void onPlayerMove(PlayerMoveEvent Event) { }
        public virtual void onPlayerOpenChest(PlayerChestOpenEvent Event) { }
        public virtual void onPlayerPartyChange(PartyChangeEvent Event) { }
        public virtual void onPlayerPreLogin(PlayerLoginEvent Event) { }
        public virtual void onPlayerProjectileUse(PlayerProjectileEvent Event) { }
        public virtual void onPlayerStateUpdate(PlayerStateUpdateEvent Event) { }
        public virtual void onPlayerTeleport(PlayerTeleportEvent Event) { }
        public virtual void onTileChange(PlayerTileChangeEvent Event) { }

        private HashSet<Hooks> pluginHooks = new HashSet<Hooks>();

        public bool containsHook(Hooks hook)
        {
            return pluginHooks.Contains(hook);
        }

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
