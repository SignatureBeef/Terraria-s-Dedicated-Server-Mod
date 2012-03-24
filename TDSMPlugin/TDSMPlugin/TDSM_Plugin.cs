using System.IO;

using Terraria_Server;
using Terraria_Server.Plugins;
using Terraria_Server.Logging;
using Terraria_Server.Commands;
using Terraria_Server.Definitions;
using Terraria_Server.Permissions;
using TDSMExamplePlugin.Commands;

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
		 */

		/*
		 * Resist statics within a plugin. 
		 * Plugins are passed in command args as an object, See PluginCommands.cs. However, as better practice use partial classes.
		 */
		public Properties properties;
		public Languages Languages;

		public bool spawningAllowed = false;
		public bool tileBreakageAllowed = false;
		public bool explosivesAllowed = false;

		public Node ExampleChatNode = Node.FromPath("tdsmexamplenode.chat");

		public TDSM_Plugin()
		{
			/* Declare these in the constructor */

			Name = "TDSMPlugin Example";
			Description = "Plugin Example for TDSM.";
			Author = "DeathCradle";
			Version = "1";
			TDSMBuild = 38; //You put here the release this plugin was made/build for.
		}

		protected override void Initialized(object state)
		{
			Languages = new Languages();
			Languages.LoadLanguages(this);

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

			/*
			 * Look at the alternate method 'OnChat' using HookAttributes
			 *      You will not be required to add the following when using the Attribute.
				Hook(HookPoints.PlayerChat, HookOrder.NORMAL,    OnChat);
			 */

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
			if (!tileBreakageAllowed) return;
			Log("Cancelled Tile change of Player: " + ctx.Player.Name);
			ctx.SetResult(HookResult.RECTIFY);
		}

		void OnReceiveProjectile(ref HookContext ctx, ref HookArgs.ProjectileReceived args)
		{
			if (!explosivesAllowed && args.Type.IsExplosive())
			{
				Log("Cancelled Explosive usage of Player: " + ctx.Player.Name);
				ctx.SetResult(HookResult.ERASE);
			}
		}

		[Hook(HookOrder.NORMAL)]
		void OnChat(ref HookContext ctx, ref HookArgs.PlayerChat args)
		{
			//Merely an example of HookAttribute (Above, 'Hook(...)') and Permissions

			if (IsRestrictedForUser(ctx.Player, ExampleChatNode))
			{
				//Player is not allowed
			}
			else
			{
				//Player is allowed
			}
		}

#endregion

#region Misc
		private static void CreateDirectory(string dirPath)
		{
			if (!Directory.Exists(dirPath))
			{
				Directory.CreateDirectory(dirPath);
			}
		}
#endregion

#region Permissions
		/* Checks whether a Permissions Handler is taken place */
		public bool IsRunningPermissions()
		{
			return Program.permissionManager.IsPermittedImpl != null;
		}

		/* If a Permissions Handler is found, It checks if they are permitted, Else they are not (false). */
		public bool IsRestrictedForUser(Player player, Node node)
		{
			return (IsRunningPermissions()) ? Program.permissionManager.IsPermittedImpl(node.Path, player) : false;
		}
#endregion
	}
}
