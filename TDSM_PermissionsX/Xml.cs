using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using Terraria_Server.Misc;
using Terraria_Server.Logging;

namespace TDSM_PermissionsX
{
	public class Xml
	{
		private string _fileLocation { get; set; }

		private List<Group> Groups { get; set; }
		private List<User> Users { get; set; }

		public Xml(string fileLocation, bool load = true)
		{
			_fileLocation = fileLocation;

			Groups = new List<Group>();
			Users = new List<User>();

			if (load) Load();
		}

		public bool Load()
		{
			try
			{
				if (!File.Exists(_fileLocation)) return false; // File.Create(_fileLocation).Close();

				Groups.Clear();
				Users.Clear();

				using (var reader = new XmlTextReader(_fileLocation))
				{
					var doc = new XmlDocument();
					doc.Load(reader);
					var list = doc.SelectNodes("XPermissions");
					if (list.Count > 0)
					{
						//while (reader.Read())
						foreach (XmlNode node in list.Item(0))
						{
							switch (node.NodeType)
							{
								case XmlNodeType.Element:
									var elementName = node.Name;

									switch (elementName)
									{
										case "Groups":
											ParseKnownElement(node, false);
											break;
										case "Users":
											ParseKnownElement(node);
											break;
									}
									break;
							}
						}
					}
				}
				return true;
			}
			catch (Exception e)
			{
				PermissionsX.XLog("Exception loading in {0}\n{1}", GetType().FullName, e);
			}

			return false;
		}

		public bool Save()
		{
			try
			{
				if (File.Exists(_fileLocation)) File.Delete(_fileLocation);

				using (var writer = new XmlTextWriter(_fileLocation, Encoding.ASCII))
				{
					writer.WriteStartDocument();
					writer.WriteStartElement("XPermissions");

					writer.WriteStartElement("Groups");
					foreach (var group in Groups) group.WriteElement(writer);
					writer.WriteEndElement();

					writer.WriteStartElement("Users");
					foreach (var user in Users) user.WriteElement(writer);
					writer.WriteEndElement();

					writer.WriteEndElement();
					writer.WriteEndDocument();

					return true;
				}
			}
			catch (Exception e)
			{
				PermissionsX.XLog("Exception saving in {0}\n{1}", GetType().FullName, e);
			}

			return false;
		}

		public void ParseKnownElement(XmlNode node, bool User = true)
		{
			if (node.HasChildNodes)
			{
				foreach (XmlNode cNode in node.ChildNodes)
				{
					if (User) ParseUser(cNode);
					else ParseGroup(cNode);
				}
			}
		}

		public void ParseGroup(XmlNode node)
		{
			if (node.Attributes.Count > 0)
			{
				var group = new Group();
				group.Initialize();

				foreach (XmlAttribute attribute in node.Attributes)
				{
					switch (attribute.Name)
					{
						case "Name":
							group.Name = attribute.Value;
							break;
						case "Color":
							Color colour;
							if (Color.TryParseRGB(attribute.Value, out colour)) group.Color = colour;
							break;
						case "CanBuild":
							bool canBuild;
							if (Boolean.TryParse(attribute.Value, out canBuild)) group.CanBuild = canBuild;
							break;
						case "Default":
							if (HasDefaultGroup())
							{
								ProgramLog.Admin.Log("[Warning] Multiple default groups");
								continue;
							}

							bool _default;
							if (Boolean.TryParse(attribute.Value, out _default)) group.Default = _default;
							break;
						case "Prefix":
							group.Prefix = attribute.Value;
							break;
						case "Suffix":
							group.Suffix = attribute.Value;
							break;
						case "ChatSeperator":
							group.ChatSeperator = attribute.Value;
							break;
						case "Rank":
							int rank;
							if (Int32.TryParse(attribute.Value, out rank)) group.Rank = rank;
							break;
					}
				}

				if (node.HasChildNodes)
				{
					foreach (XmlNode cNode in node.ChildNodes)
					{
						switch (cNode.Name)
						{
							case "Permissions":
								if (cNode.HasChildNodes)
									foreach (XmlNode gNode in cNode.ChildNodes)
									{
										var permission = gNode.InnerText.Trim();
										if (permission != null && permission.Length > 0) group.Permissions.Add(permission);
									}
								break;
							case "DenyPermissions":
								if (cNode.HasChildNodes)
									foreach (XmlNode gNode in cNode.ChildNodes)
									{
										var permission = gNode.InnerText.Trim();
										if (permission != null && permission.Length > 0) group.DenyPermissions.Add(permission);
									}
								break;
							default:
								PermissionsX.XLog("Uknown element `{0}`", cNode.Name);
								break;
						}
					}
				}

				Groups.Add(group);
			}
		}

		public void ParseUser(XmlNode node)
		{
			if (node.Attributes.Count > 0)
			{
				var user = new User();
				user.Initialize();

				foreach (XmlAttribute attribute in node.Attributes)
				{
					switch (attribute.Name)
					{
						case "Name":
							user.Name = attribute.Value;
							break;
						case "Color":
							Color colour;
							if (Color.TryParseRGB(attribute.Value, out colour)) user.Color = colour;
							break;
						case "CanBuild":
							bool canBuild;
							if (Boolean.TryParse(attribute.Value, out canBuild)) user.CanBuild = canBuild;
							break;
						case "Prefix":
							user.Prefix = attribute.Value;
							break;
						case "Suffix":
							user.Suffix = attribute.Value;
							break;
						case "ChatSeperator":
							user.ChatSeperator = attribute.Value;
							break;
					}
				}

				if (node.HasChildNodes)
				{
					foreach (XmlNode cNode in node.ChildNodes)
					{
						switch (cNode.Name)
						{
							case "UserGroups":
								if (cNode.HasChildNodes)
									foreach (XmlNode gNode in cNode.ChildNodes)
									{
										var groupName = gNode.InnerText.Trim();
										if (HasGroup(groupName))
										{
											var group = GetGroup(groupName);
											user.Groups.Add(group);
										}
									}
								break;
							case "Permissions":
								if (cNode.HasChildNodes)
									foreach (XmlNode gNode in cNode.ChildNodes)
									{
										var permission = gNode.InnerText.Trim();
										if (permission != null && permission.Length > 0) user.Permissions.Add(permission);
									}
								break;
							case "DenyPermissions":
								if (cNode.HasChildNodes)
									foreach (XmlNode gNode in cNode.ChildNodes)
									{
										var permission = gNode.InnerText.Trim();
										if (permission != null && permission.Length > 0) user.DenyPermissions.Add(permission);
									}
								break;
							default:
								PermissionsX.XLog("Uknown element `{0}`", cNode.Name);
								break;
						}
					}
				}

				Users.Add(user);
			}
		}

		public void AddUser(string name)
		{
			var user = new User()
			{
				Name = name
			};
			user.Initialize();
			Users.Add(user);
		}

		public void AddGroup(string name)
		{
			var group = new Group()
			{
				Name = name
			};
			group.Initialize();
			Groups.Add(group);
		}

		public bool HasUser(string name)
		{
			return (from x in Users where x.Name == name select x).Count() > 0;
		}

		public bool HasGroup(string name)
		{
			return (from x in Groups where x.Name == name select x).Count() > 0;
		}

		public Group GetGroup(string name)
		{
			var groups = from x in Groups where x.Name == name select x;
			if (groups.Count() == 0) return default(Group);

			return groups.ElementAt(0);
		}

		public User GetUser(string name)
		{
			var users = from x in Users where x.Name == name select x;
			if (users.Count() == 0) return default(User);

			return users.ElementAt(0);
		}

		public bool UpdateDefiniton(IPermission def)
		{
			if (def is User)
				for (var i = 0; i < Users.Count; i++)
				{
					if (Users[i].Name == def.Name)
					{
						Users[i] = Users[i].Merge(def);
						return true;
					}
				}
			else if (def is Group)
				for (var i = 0; i < Groups.Count; i++)
				{
					if (Groups[i].Name == def.Name)
					{
						Groups[i] = Groups[i].Merge(def);

						foreach (var user in Users) user.UpdateGroup(Groups[i]);
						return true;
					}
				}

			return false;
		}

		public bool HasDefaultGroup()
		{
			return (from x in Groups where x.Default select x).Count() > 0;
		}

		public Group GetDefaultGroup()
		{
			var groups = from x in Groups where x.Default select x;
			if (groups.Count() == 0) return default(Group);

			return groups.ElementAt(0);
		}

		public bool AddNodeToUser(string name, string permission)
		{
			var cleanPerm = permission.Trim();
			for (var i = 0; i < Users.Count; i++)
			{
				var user = Users[i];
				if (user.Name == name && !user.Permissions.Contains(cleanPerm))
				{
					Users[i].Permissions.Add(cleanPerm);
					return true;
				}
			}

			return false;
		}

		public bool AddNodeToGroup(string name, string permission)
		{
			var cleanPerm = permission.Trim();
			for (var i = 0; i < Groups.Count; i++)
			{
				var group = Groups[i];
				if (group.Name == name && !group.Permissions.Contains(cleanPerm))
				{
					Groups[i].Permissions.Add(cleanPerm);
					return true;
				}
			}

			return false;
		}

		public bool AddGroupToUser(string name, string group)
		{
			var cleanGroup = group.Trim();
			for (var i = 0; i < Users.Count; i++)
			{
				var user = Users[i];
				if (user.Name == name && !user.HasGroup(cleanGroup))
				{
					var matches = (from x in Groups where x.Name == cleanGroup select x);
					if (matches.Count() > 0)
					{
						var match = matches.ElementAt(0);
						Users[i].Groups.Add(match);
						return true;
					}
				}
			}

			return false;
		}

		public bool AddDeniedNodeToUser(string name, string permission)
		{
			var cleanPerm = permission.Trim();
			for (var i = 0; i < Users.Count; i++)
			{
				var user = Users[i];
				if (user.Name == name && !user.DenyPermissions.Contains(cleanPerm))
				{
					Users[i].DenyPermissions.Add(cleanPerm);
					return true;
				}
			}

			return false;
		}

		public bool AddDeniedNodeToGroup(string name, string permission)
		{
			var cleanPerm = permission.Trim();
			for (var i = 0; i < Groups.Count; i++)
			{
				var group = Groups[i];
				if (group.Name == name && !group.DenyPermissions.Contains(cleanPerm))
				{
					Groups[i].DenyPermissions.Add(cleanPerm);
					return true;
				}
			}

			return false;
		}

		public bool RemoveGroupFromUser(string name, string group)
		{
			var cleanGroup = group.Trim();
			var instance = GetGroup(cleanGroup);
			for (var i = 0; i < Users.Count; i++)
			{
				var user = Users[i];
				if (user.Name == name && user.Groups.Contains(instance))
					return Users[i].Groups.RemoveAll(x => x.Name == instance.Name) > 0;
			}
			return false;
		}

		public bool RemovePermissionFromUser(string name, string permission)
		{
			var cleanPermission = permission.Trim();
			for (var i = 0; i < Users.Count; i++)
			{
				var user = Users[i];
				if (user.Name == name && user.Permissions.Contains(cleanPermission))
					return Users[i].Permissions.RemoveAll(x => x == cleanPermission) > 0;
			}
			return false;
		}

		public bool RemovePermissionFromGroup(string name, string permission)
		{
			var cleanPermission = permission.Trim();
			for (var i = 0; i < Groups.Count; i++)
			{
				var group = Groups[i];
				if (group.Name == name && group.Permissions.Contains(cleanPermission))
					return Groups[i].Permissions.RemoveAll(x => x == cleanPermission) > 0;
			}
			return false;
		}

		public bool RemoveDeniedPermissionFromUser(string name, string permission)
		{
			var cleanPermission = permission.Trim();
			for (var i = 0; i < Users.Count; i++)
			{
				var user = Users[i];
				if (user.Name == name && user.DenyPermissions.Contains(cleanPermission))
					return Users[i].DenyPermissions.RemoveAll(x => x == cleanPermission) > 0;
			}
			return false;
		}

		public bool RemoveDeniedPermissionFromGroup(string name, string permission)
		{
			var cleanPermission = permission.Trim();
			for (var i = 0; i < Groups.Count; i++)
			{
				var group = Groups[i];
				if (group.Name == name && group.DenyPermissions.Contains(cleanPermission))
					return Groups[i].DenyPermissions.RemoveAll(x => x == cleanPermission) > 0;
			}
			return false;
		}
	}
}
