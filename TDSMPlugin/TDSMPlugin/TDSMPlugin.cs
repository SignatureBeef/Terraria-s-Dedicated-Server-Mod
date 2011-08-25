using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using Terraria_Server;
using Terraria_Server.Plugin;
using Terraria_Server.Commands;
using Terraria_Server.Events;
using Terraria_Server.Logging;
using TDSMPlugin.Commands;

namespace TDSMExamplePlugin
{
    public class TDSMPlugin : Plugin
    {
        /*
         * @Developers
         * 
         * Plugins need to be in .NET 4.0
         * Otherwise TDSM will be unable to load it. 
         * 
         * As of June 16, 1:15 AM, TDSM should now load Plugins Dynamically.
         */

        public Properties properties;
        public bool spawningAllowed = false;
        public bool tileBreakageAllowed = false;
        public bool explosivesAllowed = false;
        public static TDSMPlugin plugin;

        public override void Load()
        {
            Name = "TDSMPlugin Example";
            Description = "Plugin Example for TDSM.";
            Author = "DeathCradle";
            Version = "1";
            TDSMBuild = 32; //You put here the release this plugin was made/build for.

            plugin = this;

            string pluginFolder = Statics.PluginPath + Path.DirectorySeparatorChar + "TDSM";
            //Create folder if it doesn't exist
            CreateDirectory(pluginFolder);

            //setup a new properties file
            properties = new Properties(pluginFolder + Path.DirectorySeparatorChar + "tdsmplugin.properties");
            properties.Load();
            properties.pushData(); //Creates default values if needed. [Out-Dated]
            properties.Save();

            //read properties data
            spawningAllowed = properties.SpawningCancelled;
            tileBreakageAllowed = properties.TileBreakage;
            explosivesAllowed = properties.ExplosivesAllowed;
        }

        public override void Enable()
        {
            Program.tConsole.WriteLine(base.Name + " enabled.");
            //Register Hooks
            this.registerHook(Hooks.PLAYER_TILECHANGE);
            this.registerHook(Hooks.PLAYER_PROJECTILE);

            //Add Commands
            AddCommand("tdsmpluginexample")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("A Command Example for TDSM")
                .WithHelpText("Usage:   /tdsmpluginexample")
                .Calls(PluginCommands.ExampleCommand);

            Main.stopSpawns = !spawningAllowed;
            if (Main.stopSpawns)
            {
                ProgramLog.Log("Disabled NPC Spawning");
            }
        }

        public override void Disable()
        {
            Program.tConsole.WriteLine(base.Name + " disabled.");
        }

        public override void onPlayerTileChange(PlayerTileChangeEvent Event)
        {
            if (!Enabled || tileBreakageAllowed == false) { return; }
            ProgramLog.Log("[TSDM Plugin] Cancelled Tile change of Player: " + ((Player)Event.Sender).Name);
            Event.Cancelled = true;
        }

        public override void onPlayerProjectileUse(PlayerProjectileEvent Event)
        {
            if (Enabled && !explosivesAllowed) {

                int type = Event.Projectile.Type;
                if (type == 28 || type ==  29 || type == 37)
                {
                    Event.Cancelled = true;
                    ProgramLog.Log("[TSDM Plugin] Cancelled Explosive usage of Player: " + ((Player)Event.Sender).Name);
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
