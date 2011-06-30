using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using TDSMPlugin;
using Terraria_Server.Plugin;
using Terraria_Server;
using Terraria_Server.Events;

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

        // tConsole is used for when logging Output to the console & a log file.

        public Properties properties;
        public bool spawningAllowed = false;
        public bool tileBreakageAllowed = false;
        public bool isEnabled = false;

        public override void Load()
        {
            Name = "TDSMPlugin Example";
            Description = "Plugin Example for TDSM.";
            Author = "DeathCradle";
            Version = "1";
            TDSMBuild = 9;

            string pluginFolder = Statics.getPluginPath + Statics.systemSeperator + "TDSM";
            //Create fodler if it doesn't exist
            if (!Program.createDirectory(pluginFolder, true))
            {
                Program.tConsole.WriteLine("[TSDM Plugin] Failed to create crucial Folder");
                return;
            }

            //setup a new properties file
            properties = new Properties(pluginFolder + Statics.systemSeperator + "tdsmplugin.properties");
            properties.Load();
            properties.pushData(); //Creates default values if needed.
            properties.Save();

            //read properties data
            spawningAllowed = properties.isSpawningCancelled();
            tileBreakageAllowed = properties.getTileBreakage();

            isEnabled = true;
        }

        public override void Enable()
        {
            Program.tConsole.WriteLine(base.Name + " enabled.");
            //Register Hooks
            this.registerHook(Hooks.TILE_BREAK);
            this.registerHook(Hooks.PLAYER_COMMAND);

            Main.stopSpawns = isEnabled;
            if (isEnabled)
            {
                Program.tConsole.WriteLine("Disabled NPC Spawning");
            }
        }

        public override void Disable()
        {
            Program.tConsole.WriteLine(base.Name + " disabled.");
            isEnabled = false;
        }

        public override void onPlayerCommand(PlayerCommandEvent Event)
        {
            if (isEnabled == false) { return; }
            string[] commands = Event.getMessage().ToLower().Split(' '); //Split into sections (to lower case to work with it better)
            if (commands.Length > 0)
            {
                if (commands[0] != null && commands[0].Trim().Length > 0) //If it is nothing, and the string is actually something
                {
                    if (commands[0].Equals("/tdsmpluginexample"))
                    {
                        Program.tConsole.WriteLine("[TSDM Plugin] Player used Plugin Command: " + Event.getPlayer().name);

                        Player sendingPlayer = Event.getPlayer();
                        sendingPlayer.sendMessage("TDSM Plugin Example, For Build: #" + ServerProtocol, 255, 255f, 255f, 255f);

                        Event.setCancelled(true);
                    }
                }
            }
        }

        public override void onTileBreak(PlayerTileBreakEvent Event)
        {
            if (isEnabled == false || tileBreakageAllowed == false) { return; }
            Event.setCancelled(true);
            Program.tConsole.WriteLine("[TSDM Plugin] Cancelled Tile change of Player: " + ((Player)Event.getSender()).name);
        }
        
    }
}
