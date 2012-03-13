using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Plugins;
using Terraria_Server.Logging;
using System.IO;
using Terraria_Server;
using Terraria_Server.Commands;

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

		[Hook(HookOrder.NORMAL)]
		void OnChat(ref HookContext ctx, ref HookArgs.PlayerChat args)
		{
			if (!IsEnabled) return;

			if (ctx.Player.AuthenticatedAs != null)
			{
				var name = ctx.Player.Name;
				if (XmlParser.HasUser(name))
				{
					var user = XmlParser.GetUser(name);

					ctx.SetResult(HookResult.IGNORE);
					Server.notifyAll(
						String.Concat(user.Prefix, ctx.Player.Name, user.ChatSeperator, args.Message, user.Suffix)
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
	}
}
