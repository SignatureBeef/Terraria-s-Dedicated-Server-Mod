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

        public virtual void onPlayerChat(PlayerChatEvent Event) { }
        public virtual void onPlayerCommand(PlayerCommandEvent Event) { }
        public virtual void onPlayerCommandProcess(ConsoleCommandEvent Event) { }
        public virtual void onPlayerHurt(PlayerHurtEvent Event) { }
        public virtual void onPlayerJoin(LoginEvent Event) { }
        public virtual void onPlayerPreLogin(LoginEvent Event) { }
        public virtual void onPlayerLogout(LogoutEvent Event) { }
        public virtual void onPlayerPartyChange(PartyChangeEvent Event) { }
        public virtual void onTileBreak(TileBreakEvent Event) { }
        public virtual void onPlayerOpenChest(ChestOpenEvent Event) { }
        public virtual void onPlayerStateUpdate(PlayerStateUpdateEvent Event) { }
        public virtual void onPlayerDeath(PlayerDeathEvent Event) { }

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
