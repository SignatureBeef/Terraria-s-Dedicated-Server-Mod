using System.Collections;
using Terraria_Server.Events;

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
        public abstract void Enable();
        public abstract void Disable();

        public virtual void onPlayerChat(MessageEvent myEvent) { }
        public virtual void onPlayerCommand(PlayerCommandEvent myEvent) { }
        public virtual void onPlayerCommandProcess(ConsoleCommandEvent myEvent) { }
        public virtual void onPlayerHurt(PlayerHurtEvent myEvent) { }
        public virtual void onPlayerJoin(PlayerLoginEvent myEvent) { }
        public virtual void onPlayerPreLogin(PlayerLoginEvent myEvent) { }
        public virtual void onPlayerLogout(PlayerLogoutEvent myEvent) { }
        public virtual void onPlayerPartyChange(PartyChangeEvent myEvent) { }
        public virtual void onTileBreak(TileBreakEvent myEvent) { }
        public virtual void onPlayerOpenChest(ChestOpenEvent myEvent) { }
        public virtual void onPlayerStateUpdate(PlayerStateUpdateEvent myEvent) { }
        public virtual void onPlayerDeath(PlayerDeathEvent myEvent) { }
        public virtual void onDoorStateChange(DoorStateChangeEvent myEvent) { }

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
