using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria_Server.Plugin;
using Terraria_Server;
using Terraria_Server.Events;
using Terraria_Server.Commands;

namespace HomePlugin
{
    public class HomePlugin : Plugin
    {

        public override void Load()
        {
            data = new HomeProperties("data.home");
            Name = "Homes";
            Description = "Simple plugin that allows players to set and spawn at homes.";
            Author = "vaiquero";
            Version = "1";
        }

        public override void Enable()
        {
            Console.WriteLine(base.Name + " has been enabled.");
            this.registerHook(Hooks.PLAYER_COMMAND);
        }

        public override void Disable()
        {
            Console.WriteLine(base.Name + " has been disabled.");
        }

        public override void onPlayerCommand(PlayerCommand Event)
        {
            if (Event.getSender() is Player)
            {
                Player p = (Player)Event.getSender();
                if (Event.getMessage().Equals("/sethome"))
                {
                        data.setHome(p);
                        p.sendMessage("[Home] You have setted your home.");

                }
                else if (Event.getMessage().Equals("/home"))
                {
                    if (data.hasHome(p))
                    {
                        p.bodyPosition.X = data.getHomeX(p);
                        p.bodyPosition.Y = data.getHomeY(p);
                        p.sendMessage("[Home] You have been taken to your home.");
                    }
                    else
                    {
                        p.sendMessage("[Home] You do not have a home set.");
                    }
                }
            }
        }


        internal HomeProperties data { get; set; }
    }
}
