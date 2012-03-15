using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Plugins;
using Terraria_Server;
using Terraria_Server.Commands;
using Terraria_Server.Misc;

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
				Player user = null;
				var matches = Server.FindPlayerByPart(username);
				if (matches.Count == 1)
					user = matches.ToArray()[0];

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
			else throw new CommandError("Arguments expected - xuser adduser [-f -save] username");
		}

		public void UserPermissions(ISender sender, ArgumentList args)
		{
			var addPerms = args.TryPop("addperms");
			var addGroup = args.TryPop("addgroup");
			var denyPerms = args.TryPop("denyperms");
			var removeGroup = args.TryPop("removegroup");
			var removePerms = args.TryPop("removeperms");
			var removeDeniedPerms = args.TryPop("removedenied");
			var save = args.TryPop("-save");

			if (addPerms || denyPerms || removePerms || removeDeniedPerms)
			{
				string user, permission;
				if (args.TryParseTwo<String, String>(out user, out permission))
				{
					if (!XmlParser.HasUser(user)) throw new CommandError("No user `{0}`", user);

					var permissions = permission.Split(',');
					var add = addPerms || denyPerms;

					int added = 0, failed = 0;
					foreach (var node in permissions)
					{
						var res = false;

						if (add)
						{
							if (addPerms) res = XmlParser.AddNodeToUser(user, node);
							else res = XmlParser.AddDeniedNodeToUser(user, node);
						}
						else
						{
							if (removePerms) res = XmlParser.RemovePermissionFromUser(user, node);
							else res = XmlParser.RemoveDeniedPermissionFromUser(user, node);
						}

						if (res) added++;
						else failed++;
					}

					if (save) XmlParser.Save();

					sender.sendMessage(
						String.Format("{2} {0} node(s) where {1} failed.",
						added, failed, add ? "Added" : "Removed"
						)
					);
				}
				else throw new CommandError("User & permission node(s) expected.");
			}
			else if (addGroup || removeGroup)
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

						var res = false;
						if (addGroup) res = XmlParser.AddGroupToUser(user, node);
						else res = XmlParser.RemoveGroupFromUser(user, node);

						if (res) added++;
						else failed++;
					}

					if (save) XmlParser.Save();

					sender.sendMessage(
						String.Format("{2} {0} node(s) where {1} failed.",
						added, failed, addGroup ? "Added" : "Removed"
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

		public void GroupPermissions(ISender sender, ArgumentList args)
		{
			var addPerms = args.TryPop("addperms");
			var denyPerms = args.TryPop("denyperms");
			var removePerms = args.TryPop("removeperms");
			var removeDeniedPerms = args.TryPop("removedenied");
			var save = args.TryPop("-save");

			if (addPerms || denyPerms || removePerms || removeDeniedPerms)
			{
				string group, permission;
				if (args.TryParseTwo<String, String>(out group, out permission))
				{
					if (!XmlParser.HasGroup(group)) throw new CommandError("No group `{0}`", group);

					var permissions = permission.Split(',');
					var add = addPerms || denyPerms;

					int added = 0, failed = 0;
					foreach (var node in permissions)
					{
						var res = false;

						if (add)
						{
							if (addPerms) res = XmlParser.AddNodeToGroup(group, node);
							else res = XmlParser.AddDeniedNodeToGroup(group, node);
						}
						else
						{
							if (removePerms) res = XmlParser.RemovePermissionFromGroup(group, node);
							else res = XmlParser.RemoveDeniedPermissionFromGroup(group, node);
						}

						if (res) added++;
						else failed++;
					}

					if (save) XmlParser.Save();

					sender.sendMessage(
						String.Format("{2} {0} node(s) where {1} failed.",
						added, failed, add ? "Added" : "Removed"
						)
					);
				}
				else throw new CommandError("Group & permission node(s) expected.");
			}
			else throw new CommandError("Arguments expected.");
		}

		public void UserAttributes(ISender sender, ArgumentList args)
		{
			var save = args.TryPop("-save");
			var requestedUser = args.GetString(0);
			var attribute = args.GetString(1);
			var value = args.GetString(2);

			if (!XmlParser.HasUser(requestedUser))
				throw new CommandError("No user `{0}`", requestedUser);

			IPermission user = XmlParser.GetUser(requestedUser);

			SetAttribute(ref user, attribute, value);

			var res = XmlParser.UpdateDefiniton(user);

			sender.sendMessage(
				String.Format("{0} updating user attribute.", res ? "Success" : "Failed")
			);
		}

		public void GroupAttributes(ISender sender, ArgumentList args)
		{
			var save = args.TryPop("-save");
			var requestedGroup = args.GetString(0);
			var attribute = args.GetString(1);
			var value = args.GetString(2);

			if (!XmlParser.HasGroup(requestedGroup))
				throw new CommandError("No group `{0}`", requestedGroup);

			IPermission group = XmlParser.GetGroup(requestedGroup);

			SetAttribute(ref group, attribute, value);

			var res = XmlParser.UpdateDefiniton(group);

			sender.sendMessage(
				String.Format("{0} updating group attribute.", res ? "Success" : "Failed")
			);
		}

		public void Management(ISender sender, ArgumentList args)
		{
			var save = args.TryPop("save");
			var reload = args.TryPop("reload");
			var res = false;

			if (save) res = XmlParser.Save();
			else if (reload) res = XmlParser.Load();
			else throw new CommandError("Arguments expected.");

			sender.sendMessage(
				String.Format("{0} {1}.", res ? "Success" : "Failure", save ? "saving" : "loading")
			);
		}

		private void SetAttribute(ref IPermission def, string attribute, string value)
		{
			switch (attribute.ToLower())
			{
				case "prefix":
					def.SetPrefix(value);
					break;
				case "suffix":
					def.SetSuffix(value);
					break;
				case "color":
					Color colour;
					if (Color.TryParseRGB(value, out colour)) def.SetColor(colour);
					else throw new CommandError("Invalid color value, try `R,G,B`.");
					break;
				case "chatseperator":
					def.SetChatSeperator(value);
					break;
				case "canbuild":
					bool canBuild;
					if (Boolean.TryParse(value, out canBuild)) def.SetCanBuild(canBuild);
					else throw new CommandError("Invalid boolean value.");
					break;
				default:
					throw new CommandError("Attribute expected: prefix, suffix, color, chatseperator, canbuild.");
			}
		}
	}
}
