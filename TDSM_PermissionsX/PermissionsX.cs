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
			TDSMBuild = 37;
			Author = "TDSM Dev Team";
			Version = "1.0.0.0";
		}

		protected override void Initialized(object state)
		{
			XLog("Initializing...");

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
		}

		public void Touch()
		{
			if (!Directory.Exists(GetPath)) Directory.CreateDirectory(GetPath);
		}

		protected override void Enabled()
		{
			XLog("Enabled");
		}

		protected override void Disabled()
		{
			XLog("Disabled");
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

#region Permissions

		public bool GetParentGroup(User user, out Group group)
		{
			group = default(Group);

			if (XmlParser.HasDefaultGroup())
			{
				group = XmlParser.GetDefaultGroup();

				int rank = -1;
				foreach (var userGroup in user.Groups)
				{
					if (userGroup.Rank > rank)
					{
						group = userGroup;
						rank = userGroup.Rank;
					}
				}

				return true;
			}

			return false;
		}

		public bool GetPrefix(User user, out string prefix)
		{
			prefix = String.Empty;

			if (XmlParser.HasDefaultGroup())
			{
				prefix = XmlParser.GetDefaultGroup().Prefix;

				int rank = -1;
				foreach (var userGroup in user.Groups)
				{
					if (userGroup.Rank > rank)
					{
						prefix = userGroup.Prefix;
						rank = userGroup.Rank;
					}
				}

				return true;
			}

			return false;
		}

		public bool GetSuffix(User user, out string suffix)
		{
			suffix = String.Empty;

			if (XmlParser.HasDefaultGroup())
			{
				suffix = XmlParser.GetDefaultGroup().Suffix;

				int rank = -1;
				foreach (var userGroup in user.Groups)
				{
					if (userGroup.Rank > rank)
					{
						suffix = userGroup.Suffix;
						rank = userGroup.Rank;
					}
				}

				return true;
			}

			return false;
		}

		public bool GetColor(User user, out Color color)
		{
			color = ChatColor.White;

			if (XmlParser.HasDefaultGroup())
			{
				color = XmlParser.GetDefaultGroup().Color;

				int rank = -1;
				foreach (var userGroup in user.Groups)
				{
					if (userGroup.Rank > rank)
					{
						color = userGroup.Color;
						rank = userGroup.Rank;
					}
				}

				return true;
			}

			return false;
		}

		public bool GetChatSeperator(User user, out string chatSeperator)
		{
			chatSeperator = " ";

			if (XmlParser.HasDefaultGroup())
			{
				chatSeperator = XmlParser.GetDefaultGroup().ChatSeperator;

				int rank = -1;
				foreach (var userGroup in user.Groups)
				{
					if (userGroup.Rank > rank)
					{
						chatSeperator = userGroup.ChatSeperator;
						rank = userGroup.Rank;
					}
				}

				return true;
			}

			return false;
		}
#endregion
	}
}
