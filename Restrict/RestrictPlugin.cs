using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

using Terraria_Server.Plugins;
using Terraria_Server;
using Terraria_Server.Commands;
using Terraria_Server.Definitions;
using Terraria_Server.Misc;
using Terraria_Server.Logging;
using System.IO;

using NDesk.Options;

namespace RestrictPlugin
{
    public partial class RestrictPlugin : BasePlugin
    {
		class RegistrationRequest
		{
			public string name;
			public string address;
			public string password;
		}
		
		PropertiesFile properties;
		PropertiesFile users;
		
		Dictionary<int, RegistrationRequest> requests;
		int requestCount = 0;
		
		bool allowGuests
		{
			get { return properties.getValue ("allow-guests", false); }
		}
		
		bool restrictGuests
		{
			get { return properties.getValue ("restrict-guests", true); }
		}
		
		bool restrictGuestsDoors
		{
			get { return properties.getValue ("restrict-guests-doors", true); }
		}
		
		string serverId
		{
			get { return properties.getValue ("server-id", "tdsm"); }
		}
		
		public RestrictPlugin ()
		{
			Name = "Restrict";
			Description = "Restrict access to the server or character names.";
			Author = "UndeadMiner";
			Version = "0.36.0";
			TDSMBuild = 36;
		}
		       
		protected override void Initialized (object state)
		{
			requests = new Dictionary <int, RegistrationRequest> ();
			
			string pluginFolder = Statics.PluginPath + Path.DirectorySeparatorChar + "Restrict";

			CreateDirectory(pluginFolder);
			
			properties = new PropertiesFile (pluginFolder + Path.DirectorySeparatorChar + "restrict.properties");
			properties.Load();
			var dummy1 = allowGuests;
			var dummy2 = restrictGuests;
			var dummy3 = restrictGuestsDoors;
			var dummy4 = serverId;
			properties.Save();
			
			users = new PropertiesFile (pluginFolder + Path.DirectorySeparatorChar + "restrict_users.properties");
			users.Load();
			users.Save();
			
			AddCommand ("ru")
				.WithDescription ("Register users or change their accounts")
				.WithHelpText ("Adding users or changing passwords:")
				.WithHelpText ("    ru [-o] [-f] <name> <hash>")
				.WithHelpText ("    ru [-o] [-f] <name> -p <password>")
				.WithHelpText ("Changing op status:")
				.WithHelpText ("    ru -o [-f] <name>")
				.WithHelpText ("    ru    [-f] <name>")
				.WithHelpText ("Options:")
				.WithHelpText ("    -o    make the player an operator")
				.WithHelpText ("    -f    force action even if player isn't online")
				.Calls (LockUsers<ISender, ArgumentList>(this.RegisterCommand));

			
			AddCommand ("ur")
				.WithDescription ("Unregister users")
				.WithHelpText ("Deleting users:")
				.WithHelpText ("    ur [-f] <name>")
				.WithHelpText ("Options:")
				.WithHelpText ("    -f    force action even if player isn't online")
				.Calls (LockUsers<ISender, ArgumentList>(this.UnregisterCommand));
			
			AddCommand ("ro")
				.WithDescription ("Configure Restrict")
				.WithHelpText ("Displaying options:")
				.WithHelpText ("    ro")
				.WithHelpText ("Setting options:")
				.WithHelpText ("    ro [-f] [-g {true|false}] [-r {true|false}] [-s <serverId>] [-L]")
				.WithHelpText ("Options:")
				.WithHelpText ("    -f    force action")
				.WithHelpText ("    -g    allow guests to enter the game")
				.WithHelpText ("    -r    restrict guests' ability to alter tiles")
				.WithHelpText ("    -s    set the server identifier used in hashing passwords")
				.WithHelpText ("    -L    reload the user database from disk")
				.Calls (LockUsers<ISender, ArgumentList>(this.OptionsCommand));
			
			AddCommand ("rr")
				.WithDescription ("Manage registration requests")
				.WithHelpText ("Usage: rr          list registration requests")
				.WithHelpText ("       rr -g #     grant a registration request")
				.WithHelpText ("       rr grant #")
				.WithHelpText ("       rr -d #     deny a registration request")
				.WithHelpText ("       rr deny #")
				.Calls (LockUsers<ISender, ArgumentList>(this.RequestsCommand));
			
			AddCommand ("pass")
				.WithDescription ("Change your password")
				.WithAccessLevel (AccessLevel.PLAYER)
				.WithHelpText ("Usage: /pass yourpassword")
				.Calls (LockUsers<ISender, string>(this.PlayerPassCommand));
				
			AddCommand ("reg")
				.WithDescription ("Submit a registration request")
				.WithAccessLevel (AccessLevel.PLAYER)
				.WithHelpText ("Usage: /reg yourpassword")
				.Calls (LockUsers<ISender, string>(this.PlayerRegCommand));
		}
		
		Action<T, U> LockUsers<T, U> (Action<T, U> callback)
		{
			return delegate (T t, U u)
			{
				lock (users)
				{
					callback (t, u);
				}
			};
		}
		
		protected override void Disposed (object state)
		{
		
		}
		
		protected override void Enabled ()
		{
			ProgramLog.Log(base.Name + " enabled.");
		}
		
		protected override void Disabled ()
		{
			ProgramLog.Log(base.Name + " disabled.");
		}
		
		[Hook(HookOrder.EARLY)]
		void OnPlayerDataReceived (ref HookContext ctx, ref HookArgs.PlayerDataReceived args)
		{
			ctx.SetKick ("Malfunction during login process, try again.");
			
			if (! args.NameChecked)
			{
				string error;
				if (! args.CheckName (out error))
				{
					ctx.SetKick (error);
					return;
				}
			}
			
			var player = ctx.Player;
			if (player == null)
			{
				ProgramLog.Error.Log ("Null player passed to Restrict.OnPlayerDataReceived.");
				return;
			}
			
			var name = args.Name;
			var pname = NameTransform (name);
			var oname = OldNameTransform (name);
			string entry = null;
			
			lock (users)
			{
				entry = users.getValue (pname) ?? users.getValue (oname);
			}
			
			if (entry == null)
			{
				if (allowGuests)
				{
					ctx.SetResult (HookResult.DEFAULT);
					player.AuthenticatedAs = null;
					ctx.Connection.DesiredQueue = 0; //(int)LoginPriority.QUEUE_LOW_PRIO;
					
					ProgramLog.Log ("<Restrict> Letting user {0} from {1} in as guest.", name, player.IPAddress);
				}
				else
				{
					ProgramLog.Log ("<Restrict> Unregistered user {0} from {1} attempted to connect.", name, player.IPAddress);
					ctx.SetKick ("Only registered users are allowed.");
					return;
				}
				return;
			}
			
			ProgramLog.Log ("<Restrict> Expecting password for user {0} from {1}.", name, player.IPAddress);
			ctx.SetResult (HookResult.ASK_PASS);
		}
		
		[Hook(HookOrder.EARLY)]
		void OnPlayerPassReceived (ref HookContext ctx, ref HookArgs.PlayerPassReceived args)
		{
			ctx.SetKick ("Malfunction during login process, try again.");
			
			var player = ctx.Player;
			if (player == null)
			{
				ProgramLog.Error.Log ("Null player passed to Restrict.OnPlayerPassReceived.");
				return;
			}

			var name = player.Name;
			var pname = NameTransform (name);
			var oname = OldNameTransform (name);
			string entry = null;
			
			lock (users)
			{
				entry = users.getValue (pname) ?? users.getValue (oname);
			}
			
			if (entry == null)
			{
				if (allowGuests)
				{
					ctx.SetResult (HookResult.DEFAULT);
					player.AuthenticatedAs = null;
					ctx.Connection.DesiredQueue = 0;
				}
				else
					ctx.SetKick ("Only registered users are allowed.");

				return;
			}
			
			var split = entry.Split (':');
			var hash  = split[0];
			var hash2 = Hash (name, args.Password);
			
			if (hash != hash2)
			{
				ctx.SetKick ("Incorrect password for user: " + name);
				return;
			}
			
			if (split.Length > 1 && split[1] == "op")
			{
				player.Op = true;
				ctx.Connection.DesiredQueue = 3;
			}
			else
			{
				ctx.Connection.DesiredQueue = 1;
			}
			
			player.AuthenticatedAs = name;
			ctx.SetResult (HookResult.DEFAULT);
		}
		
		[Hook(HookOrder.TERMINAL)]
		void OnJoin (ref HookContext ctx, ref HookArgs.PlayerEnteredGame args)
		{
			var player = ctx.Player;
			
			if (player.Name == null) return;

			if (player.AuthenticatedAs == null)
				player.Message (255, "You are a guest, to register type: /reg yourpassword");
			else if (player.Op)
				player.Message (255, new Color (128, 128, 255), "This humble server welcomes back Their Operating Highness.");
			else
				player.Message (255, new Color (128, 255, 128), "Welcome back, registered user.");
			
			return;
		}
		
//		public override void onPlayerLogout (PlayerLogoutEvent ev)
//		{
//			if (ev.Player.Name == null) return;
//
//			PlayerRecord record;
//			if (records.TryGetValue (ev.Player.Name, out record))
//			{
//				records.Remove (ev.Player.Name);
//			}
//		}

		[Hook(HookOrder.EARLY)]
		void OnSignTextSet (ref HookContext ctx, ref HookArgs.SignTextSet args)
		{
			var player = ctx.Player;
			
			if (player == null || player.Name == null)
			{
				ProgramLog.Log ("<Restrict> Invalid player in OnSignTextSet.");
				ctx.SetResult (HookResult.IGNORE);
				return;
			}
			
			if (! restrictGuests) return;
			
			if (player.AuthenticatedAs == null)
			{
				ctx.SetResult (HookResult.IGNORE);
				player.sendMessage ("<Restrict> You are not allowed to edit signs as a guest.");
				player.sendMessage ("<Restrict> Type \"/reg password\" to request registration.");
			}
		}
		
		[Hook(HookOrder.EARLY)]
		void OnAlter (ref HookContext ctx, ref HookArgs.PlayerWorldAlteration args)
		{
			var player = ctx.Player;
			
			if (player == null && ctx.Sender is Projectile)
				player = (ctx.Sender as Projectile).Creator as Player;
			
			if (player == null || player.Name == null)
			{
				ProgramLog.Error.Log ("<Restrict> Invalid player in OnAlter.");
				ctx.SetResult (HookResult.IGNORE);
				return;
			}
			
			if (! restrictGuests) return;
			
			if (player.AuthenticatedAs == null)
			{
				ctx.SetResult (HookResult.RECTIFY);
				player.sendMessage ("<Restrict> You are not allowed to alter the world as a guest.");
				player.sendMessage ("<Restrict> Type \"/reg password\" to request registration.");
			}
		}
		
		[Hook(HookOrder.EARLY)]
		void OnChestBreak (ref HookContext ctx, ref HookArgs.ChestBreakReceived args)
		{
			var player = ctx.Player;
			
			if (player == null || player.Name == null)
			{
				ProgramLog.Log ("<Restrict> Invalid player in OnChestBreak.");
				ctx.SetResult (HookResult.IGNORE);
				return;
			}
			
			if (! restrictGuests) return;
			
			if (player.AuthenticatedAs == null)
			{
				ctx.SetResult (HookResult.RECTIFY);
				player.sendMessage ("<Restrict> You are not allowed to alter the world as a guest.");
				player.sendMessage ("<Restrict> Type \"/reg password\" to request registration.");
			}
		}

		[Hook(HookOrder.EARLY)]
		void OnChestOpen (ref HookContext ctx, ref HookArgs.ChestOpenReceived args)
		{
			var player = ctx.Player;
			
			if (player == null || player.Name == null)
			{
				ProgramLog.Log ("<Restrict> Invalid player in OnChestOpen.");
				ctx.SetResult (HookResult.IGNORE);
				return;
			}
			
			if (! restrictGuests) return;
			
			if (player.AuthenticatedAs == null)
			{
				ctx.SetResult (HookResult.IGNORE);
				player.sendMessage ("<Restrict> You are not allowed to open chests as a guest.");
				player.sendMessage ("<Restrict> Type \"/reg password\" to request registration.");
			}
		}
		
		[Hook(HookOrder.LATE)]
		void OnLiquidFlow (ref HookContext ctx, ref HookArgs.LiquidFlowReceived args)
		{
			var player = ctx.Player;
			
			if (player == null || player.Name == null)
			{
				ProgramLog.Log ("<Restrict> Invalid player in OnLiquidFlow.");
				ctx.SetResult (HookResult.IGNORE);
				return;
			}
			
			if (! restrictGuests) return;
			
			if (player.AuthenticatedAs == null)
			{
				ctx.SetResult (HookResult.RECTIFY);
				player.sendMessage ("<Restrict> You are not allowed to alter the world as a guest.");
				player.sendMessage ("<Restrict> Type \"/reg password\" to request registration.");
			}
		}
		
		[Hook(HookOrder.EARLY)]
		void OnProjectile (ref HookContext ctx, ref HookArgs.ProjectileReceived args)
		{
			var player = ctx.Player;
			
			if (player == null && ctx.Sender is Projectile)
				player = (ctx.Sender as Projectile).Creator as Player;
			
			if (player == null || player.Name == null)
			{
				ProgramLog.Error.Log ("<Restrict> Invalid player in OnProjectile.");
				ctx.SetResult (HookResult.IGNORE);
				return;
			}
			
			if (! restrictGuests) return;
			
			if (player.AuthenticatedAs == null)
			{
				switch (args.Type)
				{
					case ProjectileType.POWDER_PURIFICATION:
					case ProjectileType.POWDER_VILE:
					case ProjectileType.BOMB:
					case ProjectileType.BOMB_STICKY:
					case ProjectileType.DYNAMITE:
					case ProjectileType.GRENADE:
					case ProjectileType.BALL_SAND_DROP:
					case ProjectileType.BALL_MUD:
					case ProjectileType.BALL_ASH:
					case ProjectileType.BALL_SAND_GUN:
					case ProjectileType.TOMBSTONE:
					case ProjectileType.GLOWSTICK:
					case ProjectileType.GLOWSTICK_STICKY:
						ctx.SetResult (HookResult.ERASE);
						player.sendMessage ("<Restrict> You are not allowed to use this projectile as a guest.");
						player.sendMessage ("<Restrict> Type \"/reg password\" to request registration.");
						return;
					default:
						break;
				}
			}
			
			return;
		}
		
		[Hook(HookOrder.EARLY)]
		void OnDoorStateChanged (ref HookContext ctx, ref HookArgs.DoorStateChanged args)
		{
			if ((!restrictGuests) || (!restrictGuestsDoors)) return;
			
			var player = ctx.Player;
			
			if (player == null) return;

			if (player.AuthenticatedAs == null)
			{
				ctx.SetResult (HookResult.RECTIFY);
				player.sendMessage ("<Restrict> You are not allowed to open and close doors as a guest.");
				player.sendMessage ("<Restrict> Type \"/reg password\" to request registration.");
			}
		}
    }
}
