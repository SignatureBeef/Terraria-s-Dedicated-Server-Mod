using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria_Server.Plugin;
using Terraria_Server;
using Terraria_Server.Events;
using Terraria_Server.Commands;

namespace TDSMExamplePlugin
{
    public class TDSMPlugin : Plugin
    {
        /*
         * @Developers
         * 
         * Plugins need to be in .NET 3.5
         * Otherwise TDSM will be unable to load it. 
         * 
         * As of June 16, 1:15 AM, TDSM should now load Plugins Dynamically.
         */

        public override void Load()
        {
            Name = "TDSMPlugin Example";
            Description = "Plugin Example for TDSM.";
            Author = "DeathCradle";
            Version = "1";
        }

        public override void Enable()
        {
            Console.WriteLine(base.Name + " enabled.");
            this.registerHook(Hooks.TILE_BREAK);
            this.registerHook(Hooks.PLAYER_COMMAND);
        }

        public override void Disable()
        {
            Console.WriteLine(base.Name + " disabled.");
        }

        public override void onPlayerCommand(PlayerCommandEvent Event)
        {
            string[] commands = Event.getMessage().ToLower().Split(' '); //Split into sections (to lower case to work with it better)
            if (commands.Length > 0)
            {
                if (commands[0] != null && commands[0].Trim().Length > 0) //If it is nothing, and the string is actually something
                {
                    if (commands[0].Equals("/myplayercmd"))
                    {
                        Console.WriteLine("Player used Plugin Command: " + Event.getPlayer().name);
                        Event.setCancelled(true);
                    }
                }
            }
        }

        public override void onTileBreak(TileBreakEvent Event)
        {
            Console.WriteLine("Cancelling Tile change of Player: " + ((Player)Event.getSender()).name);
            Event.setCancelled(true);
        }

    }
}
