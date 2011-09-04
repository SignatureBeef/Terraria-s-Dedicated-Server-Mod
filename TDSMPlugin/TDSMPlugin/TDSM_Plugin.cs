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
using Terraria_Server.Definitions;

namespace TDSMExamplePlugin
{
    public class TDSM_Plugin : Plugin
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
        public static TDSM_Plugin tdsmPlugin;

        public override void Load()
        {
            Name = "TDSMPlugin Example";
            Description = "Plugin Example for TDSM.";
            Author = "DeathCradle";
            Version = "1";
            TDSMBuild = 33; //You put here the release this plugin was made/build for.

            tdsmPlugin = this;

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
            ProgramLog.Plugin.Log(base.Name + " enabled.");
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
                ProgramLog.Plugin.Log("Disabled NPC Spawning");
            }
        }

        public override void Disable()
        {
            ProgramLog.Plugin.Log(base.Name + " disabled.");
        }

        public static void Log(string fmt, params object[] args)
        {
            ProgramLog.Plugin.Log("[TPlugin] " + fmt, args);
        }

#region Events

        public override void onPlayerTileChange(PlayerTileChangeEvent Event)
        {
            if (Event.Cancelled || !Enabled || tileBreakageAllowed == false) { return; }
            Log("Cancelled Tile change of Player: " + ((Player)Event.Sender).Name);
            Event.Cancelled = true;
        }

        public override void onPlayerProjectileUse(PlayerProjectileEvent Event)
        {
            if (!Event.Cancelled && Enabled && !explosivesAllowed)
            {

                int type = Event.Projectile.Type;
                if (type == (int)ProjectileType.BOMB /* 28 */ || 
                    type == (int)ProjectileType.DYNAMITE /* 29 */ ||
                    type == (int)ProjectileType.BOMB_STICKY /* 37 */)
                {
                    Event.Cancelled = true;
                    Log("Cancelled Explosive usage of Player: " + ((Player)Event.Sender).Name);
                }
            }
        }

#endregion

        private static void CreateDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

    }
}
