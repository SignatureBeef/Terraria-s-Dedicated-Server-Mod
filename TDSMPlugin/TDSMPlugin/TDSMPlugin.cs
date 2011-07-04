using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using TDSMPlugin;
using Terraria_Server.Plugin;
using Terraria_Server;
using Terraria_Server.Events;
using System.IO;

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
        public bool explosivesAllowed = false;
        public bool isEnabled = false;

        public override void Load()
        {
            Name = "TDSMPlugin Example";
            Description = "Plugin Example for TDSM.";
            Author = "DeathCradle";
            Version = "1";
            TDSMBuild = 9;

            string pluginFolder = Statics.PluginPath + Path.DirectorySeparatorChar + "TDSM";
            //Create folder if it doesn't exist
            CreateDirectory(pluginFolder);

            //setup a new properties file
            properties = new Properties(pluginFolder + Path.DirectorySeparatorChar + "tdsmplugin.properties");
            properties.Load();
            //properties.pushData(); //Creates default values if needed. [Out-Dated]
            properties.Save();

            //read properties data
            spawningAllowed = properties.SpawningCancelled;
            tileBreakageAllowed = properties.TileBreakage;
            explosivesAllowed = properties.ExplosivesAllowed;

            isEnabled = true;
        }

        public override void Enable()
        {
            Program.tConsole.WriteLine(base.Name + " enabled.");
            //Register Hooks
            this.registerHook(Hooks.TILE_CHANGE);
            this.registerHook(Hooks.PLAYER_COMMAND);
            this.registerHook(Hooks.PLAYER_PROJECTILE);

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
            string[] commands = Event.Message.ToLower().Split(' '); //Split into sections (to lower case to work with it better)
            if (commands.Length > 0)
            {
                if (commands[0] != null && commands[0].Trim().Length > 0) //If it is nothing, and the string is actually something
                {
                    if (commands[0].Equals("/tdsmpluginexample"))
                    {
                        Program.tConsole.WriteLine("[TSDM Plugin] Player used Plugin Command: " + Event.Player.Name);

                        Player sendingPlayer = Event.Player;
                        sendingPlayer.sendMessage("TDSM Plugin Example, For Build: #" + ServerProtocol, 255, 255f, 255f, 255f);

                        Event.Cancelled = true;
                    }
                }
            }
        }

        public override void onTileChange(PlayerTileChangeEvent Event)
        {
            if (isEnabled == false || tileBreakageAllowed == false) { return; }
            Event.Cancelled = true;
            Program.tConsole.WriteLine("[TSDM Plugin] Cancelled Tile change of Player: " + ((Player)Event.Sender).Name);
        }

        public override void onPlayerProjectileUse(PlayerProjectileEvent Event)
        {
            if (isEnabled == false) { return; }
            if(!explosivesAllowed) {

                int type = Event.Projectile.type;
                if (type == 28 || type ==  29 || type == 37)
                {
                    Event.Cancelled = true;
                    Program.tConsole.WriteLine("[TSDM Plugin] Cancelled Explosive usage of Player: " + ((Player)Event.Sender).Name);
                }
            }
        }

        private static void CreateDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

    }
}
