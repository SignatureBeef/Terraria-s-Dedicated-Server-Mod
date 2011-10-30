using System.IO;

using Terraria_Server;
using TDSMPlugin.Commands;
using Terraria_Server.Plugins;
using Terraria_Server.Logging;
using Terraria_Server.Commands;
using Terraria_Server.Definitions;

namespace TDSMExamplePlugin
{
    public class TDSM_Plugin : BasePlugin
    {
        /*
         * @Developers
         * 
         * Plugins need to be in .NET 4.0
         * Otherwise TDSM will be unable to load it.
         * 
         * [TODO] Show example of Permission nodes
         * 
         */

        //Resist statics within a plugin.
        public Properties properties;
        public bool spawningAllowed = false;
        public bool tileBreakageAllowed = false;
        public bool explosivesAllowed = false;

        public TDSM_Plugin()
        {
            /* Declare these in the constructor */

            Name = "TDSMPlugin Example";
            Description = "Plugin Example for TDSM.";
            Author = "DeathCradle";
            Version = "1";
            TDSMBuild = 36; //You put here the release this plugin was made/build for.
        }

        protected override void Initialized(object state)
        {
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

        protected override void Enabled()
        {
            ProgramLog.Plugin.Log(base.Name + " enabled.");

            //Register Hooks            
            Hook(HookPoints.PlayerWorldAlteration, OnPlayerWorldAlteration);
            Hook(HookPoints.ProjectileReceived, HookOrder.FIRST, OnReceiveProjectile); //Priorites

            //Add Commands
            AddCommand("tdsmpluginexample")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("A Command Example for TDSM")
                .WithHelpText("Usage:   /tdsmpluginexample")
                .WithPermissionNode("tdsm.examplecommand")
                .Calls(PluginCommands.ExampleCommand);

            Main.stopSpawns = !spawningAllowed;
            if (Main.stopSpawns)
                ProgramLog.Plugin.Log("Disabled NPC Spawning");
        }

        protected override void Disabled()
        {
            ProgramLog.Plugin.Log(base.Name + " disabled.");
        }

        public static void Log(string fmt, params object[] args)
        {
            ProgramLog.Plugin.Log("[TPlugin] " + fmt, args);
        }

#region Events

        void OnPlayerWorldAlteration(ref HookContext ctx, ref HookArgs.PlayerWorldAlteration args)
        {
            if (!tileBreakageAllowed) { return; }
            Log("Cancelled Tile change of Player: " + ctx.Player.Name);
            ctx.SetResult(HookResult.RECTIFY);
        }

        void OnReceiveProjectile(ref HookContext ctx, ref HookArgs.ProjectileReceived args)
        {
            if (!explosivesAllowed)
            {
                int type = (int)args.Type;
                if (type == (int)ProjectileType.BOMB /* 28 */ || 
                    type == (int)ProjectileType.DYNAMITE /* 29 */ ||
                    type == (int)ProjectileType.BOMB_STICKY /* 37 */)
                {
                    Log("Cancelled Explosive usage of Player: " + ctx.Player.Name);
                    ctx.SetResult(HookResult.ERASE);
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
