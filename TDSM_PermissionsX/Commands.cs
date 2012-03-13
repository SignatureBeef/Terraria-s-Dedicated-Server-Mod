using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Plugins;
using Terraria_Server;
using Terraria_Server.Commands;

namespace TDSM_PermissionsX
{
	public partial class PermissionsX
	{
		public void Users(ISender sender, ArgumentList args)
		{
			var add = args.TryPop("add"); //xuser adduser username
			var forced = args.TryPop("-f");
			var save = args.TryPop("-save");

			if (add)
			{
				var username = args.GetString(0);
				var user = Server.GetPlayerByName(username);

				if (user == null && !forced)
					throw new CommandError("No online player found, Use -f if you know for certain that the name is correct.");

				var trueUser = user == null ? username : (user.Name ?? username);

				if (XmlParser.HasUser(trueUser))
					throw new CommandError("Definitions already exist for that user.");

				XmlParser.AddUser(trueUser);

				if (save) XmlParser.Save();

				sender.sendMessage(
					String.Format("Definitions for `{0}` have been created.", trueUser)
				);
			}
			else throw new CommandError("Arguments expected.");
		}

		public void UserPermissions(ISender sender, ArgumentList args)
		{
			var addPerms = args.TryPop("addperms");
			var addGroup = args.TryPop("addgroup");
			var save = args.TryPop("-save");

			if (addPerms)
			{
				string user, permission;
				if (args.TryParseTwo<String, String>(out user, out permission))
				{
					var permissions = permission.Split(',');

					if (!XmlParser.HasUser(user))
						throw new CommandError("No user `{0}`", user);

					int added = 0, failed = 0;
					foreach (var node in permissions)
					{
						var res = XmlParser.AddNodeToUser(user, node);

						if (res) added++;
						else failed++;
					}

					if (save) XmlParser.Save();

					sender.sendMessage(
						String.Format("Added {0} node(s) where {1} failed.",
							added, failed
						)
					);
				}
				else throw new CommandError("User & permission node(s) expected.");
			}
			else if (addGroup)
			{
				string user, group;
				if (args.TryParseTwo<String, String>(out user, out group))
				{
					var groups = group.Split(',');

					if (!XmlParser.HasUser(user)) throw new CommandError("No user `{0}`", user);

					int added = 0, failed = 0;
					foreach (var node in groups)
					{
						if (!XmlParser.HasGroup(node))
						{
							sender.sendMessage(String.Format("No group `{0}`", node));
							continue;
						}

						var res = XmlParser.AddGroupToUser(user, node);

						if (res) added++;
						else failed++;
					}

					if (save) XmlParser.Save();

					sender.sendMessage(
						String.Format("Added {0} node(s) where {1} failed.",
							added, failed
						)
					);
				}
				else throw new CommandError("User & group(s) expected.");
			}
			else throw new CommandError("Arguments expected.");
		}

		public void Groups(ISender sender, ArgumentList args)
		{
			var add = args.TryPop("add"); //xgroup add groupname
			var save = args.TryPop("-save");

			if (add)
			{
				var groupName = args.GetString(0);

				if (XmlParser.HasGroup(groupName))
					throw new CommandError("Definitions already exist for that group.");

				XmlParser.AddGroup(groupName);

				if (save) XmlParser.Save();

				sender.sendMessage(
					String.Format("Definitions for `{0}` have been created.", groupName)
				);
			}
			else throw new CommandError("Arguments expected.");
		}
	}
}
