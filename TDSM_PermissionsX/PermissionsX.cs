using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Plugins;
using Terraria_Server.Logging;
using System.IO;
using Terraria_Server;
using Terraria_Server.Commands;
using Terraria_Server.Misc;

namespace TDSM_PermissionsX
{
	public partial class PermissionsX : BasePlugin
	{
		public XProperties Properties { get; set; }
		public Languages Languages { get; set; }
		public Xml XmlParser { get; set; }

		public string GetPath
		{
			get
			{ return Path.Combine(Statics.PluginPath, "XPermissions"); }
		}

		public string GetPropertiesPath
		{
			get
			{ return Path.Combine(GetPath, "XPermissions.properties"); }
		}

		public string GetPermissionsFile
		{
			get
			{ return Path.Combine(GetPath, "XPermissions.xml"); }
		}

		public PermissionsX()
		{
			Name = "Permissions X";
			Description = "XML based permissions system.";
			TDSMBuild = 38;
			Author = "TDSM Dev Team";
			Version = "1.0.0.0";
		}

		protected override void Initialized(object state)
		{
			Languages = new Languages();
			Languages.LoadLanguages(this);

			XLog(Languages.Initializing);

			Touch();

			Properties = new XProperties(GetPropertiesPath);
			Properties.Load();
			Properties.pushData();
			Properties.Save(false);

			XmlParser = new Xml(GetPermissionsFile);

			AddCommand("xuser")
				.WithAccessLevel(AccessLevel.OP)
				.WithPermissionNode("xperms.xuser")
				.Calls(Users);

			AddCommand("xuserperms")
				.WithAccessLevel(AccessLevel.OP)
				.WithPermissionNode("xperms.xuserperms")
				.Calls(UserPermissions);

			AddCommand("xgroup")
				.WithAccessLevel(AccessLevel.OP)
				.WithPermissionNode("xperms.xgroup")
				.Calls(Groups);

			AddCommand("xgroupperms")
				.WithAccessLevel(AccessLevel.OP)
				.WithPermissionNode("xperms.xgroupperms")
				.Calls(GroupPermissions);

			AddCommand("xuserattribute")
				.WithAccessLevel(AccessLevel.OP)
				.WithPermissionNode("xperms.xuserattribute")
				.Calls(UserAttributes);

			AddCommand("xgroupattribute")
				.WithAccessLevel(AccessLevel.OP)
				.WithPermissionNode("xperms.xgroupattribute")
				.Calls(GroupAttributes);

			AddCommand("xmanagement")
				.WithAccessLevel(AccessLevel.OP)
				.WithPermissionNode("xperms.xmanagement")
				.Calls(Management);
		}

		public void Touch()
		{
			if (!Directory.Exists(GetPath)) Directory.CreateDirectory(GetPath);
		}

		protected override void Enabled()
		{
			Program.permissionManager.IsPermittedImpl = IsPermitted;
			Statics.PermissionsEnabled = true;

			XLog(Languages.Enabled);
		}

		protected override void Disabled()
		{
			XLog(Languages.DisabledMessaged);
		}

		public static void XLog(string format, params object[] args)
		{
			ProgramLog.Plugin.Log("[XPermission] " + format, args);
		}

		[Hook(HookOrder.FIRST)]
		void OnChat(ref HookContext ctx, ref HookArgs.PlayerChat args)
		{
			if (!IsEnabled) return;

			if (ctx.Player.AuthenticatedAs != null)
			{
				var name = ctx.Player.Name;
				if (XmlParser.HasUser(name))
				{
					var user = XmlParser.GetUser(name);

					string chatSeperator, prefix, suffix;
					Color chatColour;

					GetChatSeperator(user, out chatSeperator);
					GetColor(user, out chatColour);
					GetPrefix(user, out prefix);
					GetSuffix(user, out suffix);

					args.Color = chatColour; //Might set this for next plugins.

					ctx.SetResult(HookResult.IGNORE);
					Server.notifyAll(
						String.Concat(prefix, ctx.Player.Name, chatSeperator, args.Message, suffix)
						, args.Color
					);
				}
				else if (XmlParser.HasDefaultGroup())
				{
					var defaultGroup = XmlParser.GetDefaultGroup();

					ctx.SetResult(HookResult.IGNORE);
					Server.notifyAll(
						String.Concat
							(
								defaultGroup.Prefix,
								ctx.Player.Name,
								defaultGroup.ChatSeperator, args.Message,
								defaultGroup.Suffix
							)
						, defaultGroup.Color
					);
				}
			}
			else if (ctx.Player.AuthenticatedAs == null)
			{
				if (XmlParser.HasDefaultGroup())
				{
					var defaultGroup = XmlParser.GetDefaultGroup();

					ctx.SetResult(HookResult.IGNORE);
					Server.notifyAll(
						String.Concat
							(
								defaultGroup.Prefix,
								ctx.Player.Name,
								defaultGroup.ChatSeperator, args.Message,
								defaultGroup.Suffix
							)
						, defaultGroup.Color
					);
				}
			}
		}

		[Hook(HookOrder.LATE)]
		void OnPluginsLoaded(ref HookContext ctx, ref HookArgs.PluginsLoaded args)
		{
			if (Properties.AllowRestrictAutoDownload)
			{
				var authSystem = new Auth();
				if (!Server.UsingLoginSystem)
					authSystem.InitSystem(Languages);

				if (authSystem.IsRestrictRunning())
					ProgramLog.Plugin.Log(Languages.Protected);
				else
					ProgramLog.Plugin.Log(Languages.GetAuth);
			}
			else ProgramLog.Plugin.Log(Languages.GetAuth);
		}

		#region Permissions
		public bool GetParentGroup(User user, out Group group)
		{
			group = default(Group);

			int rank = -1;
			foreach (var userGroup in user.Groups)
			{
				if (userGroup.Rank > rank)
				{
					group = userGroup;
					rank = userGroup.Rank;
				}
			}
			if (rank != -1) return true;

			if (XmlParser.HasDefaultGroup())
			{
				group = XmlParser.GetDefaultGroup();
				return true;
			}

			return false;
		}

		public bool GetPrefix(User user, out string prefix)
		{
			prefix = String.Empty;

			if (user.Prefix.Length > 0)
			{
				prefix = user.Prefix;
				return true;
			}

			int rank = -1;
			foreach (var userGroup in user.Groups)
			{
				if (userGroup.Rank > rank)
				{
					prefix = userGroup.Prefix;
					rank = userGroup.Rank;
				}
			}
			if (rank != -1) return true;

			if (XmlParser.HasDefaultGroup())
			{
				prefix = XmlParser.GetDefaultGroup().Prefix;
				return true;
			}

			return false;
		}

		public bool GetSuffix(User user, out string suffix)
		{
			suffix = String.Empty;

			if (user.Suffix.Length > 0)
			{
				suffix = user.Suffix;
				return true;
			}

			int rank = -1;
			foreach (var userGroup in user.Groups)
			{
				if (userGroup.Rank > rank)
				{
					suffix = userGroup.Suffix;
					rank = userGroup.Rank;
				}
			}
			if (rank != -1) return true;

			if (XmlParser.HasDefaultGroup())
			{
				suffix = XmlParser.GetDefaultGroup().Suffix;
				return true;
			}

			return false;
		}

		public bool GetColor(User user, out Color color)
		{
			color = ChatColor.White;

			if (user.Color != ChatColor.White)
			{
				color = user.Color;
				return true;
			}

			int rank = -1;
			foreach (var userGroup in user.Groups)
			{
				if (userGroup.Rank > rank)
				{
					color = userGroup.Color;
					rank = userGroup.Rank;
				}
			}
			if (rank != -1) return true;

			if (XmlParser.HasDefaultGroup())
			{
				color = XmlParser.GetDefaultGroup().Color;

				return true;
			}

			return false;
		}

		public bool GetChatSeperator(User user, out string chatSeperator)
		{
			chatSeperator = ": ";

			if (user.ChatSeperator != chatSeperator && user.ChatSeperator != String.Empty)
			{
				chatSeperator = user.ChatSeperator;
				return true;
			}

			int rank = -1;
			foreach (var userGroup in user.Groups)
			{
				if (userGroup.Rank > rank)
				{
					chatSeperator = userGroup.ChatSeperator;
					rank = userGroup.Rank;
				}
			}
			if (rank != -1) return true;

			if (XmlParser.HasDefaultGroup())
			{
				chatSeperator = XmlParser.GetDefaultGroup().ChatSeperator;
				return true;
			}

			return false;
		}

		public bool HasPermission(User user, string node)
		{
			var cleanNode = node.Trim().ToLower();
			foreach (var _group in user.Groups)
			{
				var groupHasDeny = (from x in _group.DenyPermissions where x.Trim().ToLower() == cleanNode || x.Trim().ToLower() == "*" select x).Count() > 0;
				var groupHas = (from x in _group.Permissions where x.Trim().ToLower() == cleanNode || x.Trim().ToLower() == "*" select x).Count() > 0;
				if (groupHasDeny) return false;
				if (groupHas) return true;
			}

			if ((from x in user.DenyPermissions where x.Trim().ToLower() == cleanNode || x.Trim().ToLower() == "*" select x).Count() > 0) return false;

			return (from x in user.Permissions where x.Trim().ToLower() == cleanNode || x.Trim().ToLower() == "*" select x).Count() > 0;
		}

		public bool CanBuild(User user)
		{
			if (!user.CanBuild) return false;

			if ((from x in user.Groups where !x.CanBuild select x).Count() > 0) return false;

			return true;
		}

		public bool CanBuild(string username)
		{
			if (XmlParser.HasUser(username))
			{
				var user = XmlParser.GetUser(username);
				return CanBuild(user);
			}

			if (XmlParser.HasDefaultGroup())
			{
				var group = XmlParser.GetDefaultGroup();
				return group.CanBuild;
			}

			return true;
		}

		//This is what TDSM will check.
		public bool IsPermitted(string node, Player player)
		{
			var name = player.Name;
			if (XmlParser.HasUser(name))
			{
				var user = XmlParser.GetUser(name);
				return ((player.AuthenticatedAs != null && HasPermission(user, node)) || player.Op);
			}

			return player.Op;
		}
		#endregion
	}
}
