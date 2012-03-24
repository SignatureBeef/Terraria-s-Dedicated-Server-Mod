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
					throw new CommandError(Languages.NoPlayersAndForce);

				var trueUser = user == null ? username : (user.Name ?? username);

				if (XmlParser.HasUser(trueUser))
					throw new CommandError(Languages.DefinitionsExist + Languages.User);

				XmlParser.AddUser(trueUser);

				if (save) XmlParser.Save();

				sender.sendMessage(
					String.Format("`{0}` {1}", trueUser, Languages.HasBeenCreated)
				);
			}
			else throw new CommandError("{0} - xuser adduser [-f -save] username", Languages.ArgumentsExpected);
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
					if (!XmlParser.HasUser(user)) throw new CommandError("{1} `{0}`", user, Languages.NoUser);

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
						String.Format("{2} {0} {3} {1} {4}",
						added, failed, add ? Languages.Added : Languages.Removed, Languages.NodesWhere, Languages.Failed
						)
					);
				}
				else throw new CommandError(Languages.UserAndNodeExpected);
			}
			else if (addGroup || removeGroup)
			{
				string user, group;
				if (args.TryParseTwo<String, String>(out user, out group))
				{
					var groups = group.Split(',');

					if (!XmlParser.HasUser(user)) throw new CommandError("{1} `{0}`", user, Languages.NoUser);

					int added = 0, failed = 0;
					foreach (var node in groups)
					{
						if (!XmlParser.HasGroup(node))
						{
							sender.sendMessage(String.Format("{1} `{0}`", node, Languages.NoGroup));
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
						String.Format("{2} {0} {3} {1} {4}",
						added, failed, addGroup ? Languages.Added : Languages.Removed, Languages.NodesWhere, Languages.Failed
						)
					);
				}
				else throw new CommandError(Languages.UserAndGrpExpected);
			}
			else throw new CommandError(Languages.ArgumentsExpected);
		}

		public void Groups(ISender sender, ArgumentList args)
		{
			var add = args.TryPop("add"); //xgroup add groupname
			var save = args.TryPop("-save");

			if (add)
			{
				var groupName = args.GetString(0);

				if (XmlParser.HasGroup(groupName))
					throw new CommandError(Languages.DefinitionsExist + Languages.Group);

				XmlParser.AddGroup(groupName);

				if (save) XmlParser.Save();

				sender.sendMessage(
					String.Format("`{0}` {1}", groupName, Languages.HasBeenCreated)
				);
			}
			else throw new CommandError(Languages.ArgumentsExpected);
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
					if (!XmlParser.HasGroup(group)) throw new CommandError("{1} `{0}`", group,Languages.NoGroup);

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
						String.Format("{2} {0} {3} {1} {4}",
						added, failed, add ? Languages.Added : Languages.Removed, Languages.NodesWhere, Languages.Failed
						)
					);
				}
				else throw new CommandError(Languages.GrpAndNodeExpected);
			}
				else throw new CommandError(Languages.ArgumentsExpected);
		}

		public void UserAttributes(ISender sender, ArgumentList args)
		{
			var save = args.TryPop("-save");
			var requestedUser = args.GetString(0);
			var attribute = args.GetString(1);
			var value = args.GetString(2);

			if (!XmlParser.HasUser(requestedUser))
				throw new CommandError("{1} `{0}`", requestedUser, Languages.NoUser);

			IPermission user = XmlParser.GetUser(requestedUser);

			SetAttribute(ref user, attribute, value);

			var res = XmlParser.UpdateDefiniton(user);

			if (save) XmlParser.Save();

			sender.sendMessage(
				String.Format("{0} {1}", res ? Languages.Success : Languages.Failure, Languages.UpdatingAttribute)
			);
		}

		public void GroupAttributes(ISender sender, ArgumentList args)
		{
			var save = args.TryPop("-save");
			var requestedGroup = args.GetString(0);
			var attribute = args.GetString(1);
			var value = args.GetString(2);

			if (!XmlParser.HasGroup(requestedGroup))
				throw new CommandError("{1} `{0}`", requestedGroup, Languages.NoGroup);

			IPermission group = XmlParser.GetGroup(requestedGroup);

			SetAttribute(ref group, attribute, value);

			var res = XmlParser.UpdateDefiniton(group);

			if (save) XmlParser.Save();

			sender.sendMessage(
				String.Format("{0} {1}", res ? Languages.Success : Languages.Failure, Languages.UpdatingAttribute)
			);
		}

		public void Management(ISender sender, ArgumentList args)
		{
			var save = args.TryPop("save");
			var reload = args.TryPop("reload");
			var res = false;

			if (save) res = XmlParser.Save();
			else if (reload) res = XmlParser.Load();
			else throw new CommandError(Languages.ArgumentsExpected);

			sender.sendMessage(
				String.Format("{0} {1}.", res ? Languages.Success : Languages.Failure, save ? Languages.Saving : Languages.Loading)
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
					else throw new CommandError("{0} `R,G,B`.", Languages.InvalidColorMsg);
					break;
				case "chatseperator":
					def.SetChatSeperator(value);
					break;
				case "canbuild":
					bool canBuild;
					if (Boolean.TryParse(value, out canBuild)) def.SetCanBuild(canBuild);
					else throw new CommandError(Languages.InvalidBooleanValue);
					break;
				default:
					throw new CommandError("{0}: prefix, suffix, color, chatseperator, canbuild.", Languages.AttributeExpected);
			}
		}
	}
}
