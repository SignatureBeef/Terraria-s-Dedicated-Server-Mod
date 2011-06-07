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

        public bool Enabled { get; set; }
        public Server Server { get; set; }

        public abstract void Load();
        //public void abstract UnLoad();
        public abstract void Enable();
        public abstract void Disable();

        public virtual void onPlayerCommand(PlayerCommandEvent Event) { }
        public virtual void onPlayerCommandProcess(ConsoleCommandEvent Event) { }
        public virtual void onPlayerPreLogin(LoginEvent Event) { }
        public virtual void onPlayerJoin(LoginEvent Event) { }
        public virtual void onPlayerLogout(LogoutEvent Event) { }
        public virtual void onPlayerPartyChange(PartyChangeEvent Event) { }
        public virtual void onPlayerChat(PlayerChatEvent Event) { }
        public virtual void onTileChange(TileChangeEvent Event) { }
        
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
