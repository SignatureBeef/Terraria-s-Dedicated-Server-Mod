using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria_Server.Events;
using Terraria_Server.Commands;
using System.Collections;

namespace Terraria_Server.Plugin
{
    public abstract class Plugin 
    {

        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string ServerProtocol { get; set; } //Soon to be removed. ~‎Saturday, ‎June ‎25, ‎2011
        public int TDSMBuild { get; set; } //Thinking about adding a warning for out of date plugins.

        public bool Enabled { get; set; }
        public Server Server { get; set; }

        public abstract void Load();
        //public void abstract UnLoad(); //I have high hopes :3
        public abstract void Enable();
        public abstract void Disable();

        public virtual void onPlayerChat(PlayerChatEvent Event) { }
        public virtual void onPlayerCommand(PlayerCommandEvent Event) { }
        public virtual void onPlayerCommandProcess(ConsoleCommandEvent Event) { }
        public virtual void onPlayerHurt(PlayerHurtEvent Event) { }
        public virtual void onPlayerJoin(PlayerLoginEvent Event) { }
        public virtual void onPlayerPreLogin(PlayerLoginEvent Event) { }
        public virtual void onPlayerLogout(PlayerLogoutEvent Event) { }
        public virtual void onPlayerPartyChange(PartyChangeEvent Event) { }
        public virtual void onTileChange(PlayerTileChangeEvent Event) { }
        public virtual void onPlayerOpenChest(PlayerChestOpenEvent Event) { }
        public virtual void onPlayerStateUpdate(PlayerStateUpdateEvent Event) { }
        public virtual void onPlayerDeath(PlayerDeathEvent Event) { }
        public virtual void onDoorStateChange(DoorStateChangeEvent Event) { }
        public virtual void onPlayerEditSign(PlayerEditSignEvent Event) { }
        public virtual void onPlayerProjectileUse(PlayerProjectileEvent Event) { }

    	private ArrayList pluginHooks = new ArrayList();

        public bool containsHook(Hooks Hook)
        {
            return pluginHooks.Contains(Hook);
        }

        public bool registerHook(Hooks Hook)
        {
            if (!containsHook(Hook))
            {
                pluginHooks.Add(Hook);
                return true;
            }
            return false;
        }

    }
}
