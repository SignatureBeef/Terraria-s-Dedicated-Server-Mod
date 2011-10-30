using System;
using System.IO;
using System.Collections.Generic;

using Terraria_Server;
using Terraria_Server.Misc;
using Terraria_Server.Logging;
using Terraria_Server.Permissions;
using TDSMPermissions.Definitions;
using Terraria_Server.Commands;

using YaTools.Yaml;
using Terraria_Server.Plugins;
using TDSMPermissions.Auth;

namespace TDSMPermissions
{
    public class TDSMPermissions : BasePlugin
    {
        public Properties properties;
        public string pluginFolder;
        public string permissionsYML;

        public bool spawningAllowed = false;
        public bool tileBreakageAllowed = false;
        public bool explosivesAllowed = false;
        public TDSMPermissions plugin;
        public string defaultGroup;

        private List<Group> groups = new List<Group>();
        private Dictionary<String, User> users = new Dictionary<String, User>();
        private User currentUser = new User();
        private Group currentGroup;
        private YamlScanner sc;
        private bool inUsers;
        private String currentUserName = null;
		private string[] nodesToAdd = 
        {
            "permissions.test"
        };

        public TDSMPermissions()
        {
            Name = "TDSMPermissions";
            Description = "Permissions for TDSM.";
            Author = "Malkierian";
            Version = "1";
            TDSMBuild = 36;
        }

        protected override void Initialized(object state)
        {
            plugin = this;

            pluginFolder = Statics.PluginPath + Path.DirectorySeparatorChar + "TDSMPermissions";
            permissionsYML = pluginFolder + Path.DirectorySeparatorChar + "permissions.yml";

            //Create folder if it doesn't exist
            CreateDirectory(pluginFolder);

            if (!File.Exists(permissionsYML))
                File.Create(permissionsYML).Close();

            //set internal permission check method to plugins handler
            Program.permissionManager.IsPermittedImpl = IsPermitted;
			Statics.PermissionsEnabled = true;
        }

        protected override void Enabled()
        {
            ProgramLog.Log(base.Name + " enabled.");
            
            //Add Commands
			AddCommand("permissions")
				.WithAccessLevel(AccessLevel.PLAYER)
				.WithDescription("Test command")
				.Calls(Commands.PluginCommands.PermissionsCommand);

			Program.permissionManager.AddNodes(nodesToAdd);
        }

        protected override void Disabled()
        {
            ProgramLog.Log(base.Name + " disabled.");
        }

        //[Hook(HookOrder.NORMAL)]
        //void PlayerEnteredGame(ref HookContext ctx, ref HookArgs.PlayerEnteredGame args)
        //{
        //    if (ctx.Player.AuthenticatedAs != null)
        //    {
        //        User usr;
        //        if (users.TryGetValue(ctx.Player.Name, out usr))
        //        {
        //            //usr.chatColor.
        //            ctx.Player.
        //        }
        //    }
        //}

        [Hook(HookOrder.NORMAL)]
        void OnChat(ref HookContext ctx, ref HookArgs.PlayerChat args)
        {
            if (ctx.Player.AuthenticatedAs != null)
            {
                User usr;

                if (users.TryGetValue(ctx.Player.Name, out usr))
                {
                    if (usr.chatColor != default(Color) && usr.chatColor != ChatColor.AntiqueWhite)
                        args.Color = usr.chatColor;
                    else if (usr.group.Count > 0)
                    {
                        Group grp = GetGroup(usr.group[0]);
                        if (grp != null && grp.GroupInfo.color != default(Color) && grp.GroupInfo.color != ChatColor.AntiqueWhite)
                            args.Color = grp.GroupInfo.color;
                    }

                    args.Message = usr.prefix + args.Message + usr.suffix;
                }
            }
        }

        [Hook(HookOrder.LATE)]
        void OnPluginsLoaded(ref HookContext ctx, ref HookArgs.PluginsLoaded args)
        {
            LoadPerms();

            if (!Server.UsingLoginSystem)
                Login.InitSystem();

            if (Login.IsRestrictRunning())
                ProgramLog.Plugin.Log("Your Server is now protected!");
            else
                ProgramLog.Plugin.Log("Your Server is vulnerable, Get an Authentication system!");
        }
        
        public void LoadPerms()
        {
            Token to;
            TextReader re = File.OpenText(permissionsYML);

            sc = new YamlScanner();
            sc.SetSource(re);

            inUsers = false;

            while ((to = sc.NextToken()) != Token.EndOfStream)
            {
                switch (to)
                {
                    case Token.TextContent:
						{
                            switch (sc.TokenText)
                            {
                                case "info":
                                    {
                                        ProcessInfo();
                                        break;
                                    }
                                case "permissions":
                                    {
										ProcessPermissions();
                                        break;
                                    }
                                case "inheritance":
                                    {
                                        ParseInheritance();
                                        break;
                                    }
                                case "groups":
                                    {
                                        if (inUsers)
                                        {
                                            ProcessGroups();
                                        }
                                        break;
                                    }
                                case "users":
                                    {
                                        inUsers = true;
										ProcessInheritance();
                                        break;
                                    }
                                default:
                                    break;
                            }
                            break;
                        }
                    case Token.IndentSpaces:
                        {
                            ProcessIndent();
                            break;
                        }
                    case Token.Outdent:
                    case Token.ValueIndicator:
                    case Token.BlockSeqNext:
                    case Token.Comma:
                    case Token.Escape:
                    case Token.InconsistentIndent:
                    case Token.Unexpected:
                    case Token.DoubleQuote:
                    case Token.SingleQuote:
                    case Token.EscapedLineBreak:
                    default:
                        break;
                }
            }
        }

        private void ProcessGroups()
        {
            while (sc.NextToken() != Token.Outdent)
            {
                while (sc.NextToken() != Token.TextContent)
				{
                    if (sc.Token == Token.Outdent)
                        return;
				}
                foreach (Group group in groups)
                {
                    if (group.Name == sc.TokenText)
                    {
                        foreach (string node in group.permissions.Keys)
                        {
                            bool toggle = false;
                            group.permissions.TryGetValue(node, out toggle);
                            if (toggle)
                            {
                                currentUser.hasPerm.Add(node);
                            }
                            else
                            {
                                currentUser.notHasPerm.Add(node);
                            }
						}
						currentUser.group.Add(group.Name);
                        if (!users.ContainsKey(currentUserName))
                            users.Add(currentUserName, currentUser);
                    }
				}
            }
        }

        private void ProcessIndent()
        {
            string tokenText = sc.TokenText;
            if (sc.NextToken() == Token.IndentSpaces)
                tokenText += sc.TokenText;
            if (tokenText == "    ")
            {
                while (sc.NextToken() != Token.TextContent)
                {
                }
                if (!inUsers)
                {
                    currentGroup = new Group(sc.TokenText);
                }
                else
                {
                    currentUserName = sc.TokenText;
                    currentUser = new User();
                }
            }
        }

        private void ProcessInfo()
        {
            bool Default;
            string Prefix;
            string Suffix;
            Color color = default(Color);
            while (sc.TokenText != "default")
            {
                sc.NextToken();
            }
            while (sc.NextToken() != Token.TextContent)
            { }
            Default = Convert.ToBoolean(sc.TokenText);
            if (Default)
            {
                defaultGroup = currentGroup.Name;
            }
            while (sc.TokenText != "prefix")
            {
                sc.NextToken();
            }
            while (sc.NextToken() != Token.TextContent)
            { }
            Prefix = sc.TokenText;
            while (sc.TokenText != "suffix")
            {
                sc.NextToken();
            }
            while (sc.NextToken() != Token.TextContent)
            { }
            Suffix = sc.TokenText;
            while (sc.TokenText != "color")
            {
                sc.NextToken();
                if (sc.Token == Token.Outdent)
				{
					color = ChatColor.White;
                    break;
				}
            }

			if (sc.Token == Token.TextContent && sc.TokenText == "color")
			{
				while (sc.NextToken() != Token.TextContent)
				{
					if (sc.Token == Token.Outdent)
						break;
				}
                Color col;
                if (sc.Token == Token.TextContent && Color.TryParseRGB(sc.TokenText, out col))
                    color = col;
                //else
                //    color = ChatColor.White;
			}

            if (inUsers)
                currentUser.SetUserInfo(Prefix, Suffix, color);
            else
                currentGroup.SetGroupInfo(Default, Prefix, Suffix, color);
        }

        private void ParseInheritance()
        {
            while (sc.NextToken() != Token.TextContent)
            {
                if (sc.Token == Token.Outdent)
                    return;
            }
            currentGroup.Inherits.Add(sc.TokenText);
        }

		private void ProcessInheritance()
		{
            //ProgramLog.Debug.Log("Processing group inheritances");
			foreach (Group group in groups)
			{
                //ProgramLog.Debug.Log("Processing " + group.Name + "'s inheritance");
				group.Inherits.ForEach(delegate (string s)
				{
                    //ProgramLog.Debug.Log("Group " + group.Name + " inherits from " + s);
					foreach (Group groupIn in groups)
					{
						if (groupIn.Name == s)
						{
							foreach (string node in groupIn.permissions.Keys)
							{
								if (!group.permissions.ContainsKey(node))
								{
									//ProgramLog.Debug.Log("Adding node " + node + " from " + s + " to " + group.Name);
									bool toggle = false;
									groupIn.permissions.TryGetValue(node, out toggle);
									group.permissions.Add(node, toggle);
								}
							}
						}
					}
				});
			}
		}

        private void ProcessPermissions()
        {
            while (sc.NextToken() != Token.Outdent)
            {
                while (sc.NextToken() != Token.TextContent)
                {
					if (sc.Token == Token.Outdent)
					{
						if (!inUsers)
						{
							 groups.Add(currentGroup);
						}
                        else if (!users.ContainsKey(currentUserName))
						{
							users.Add(currentUserName, currentUser);
						}
						return;
					}
                 }
                bool toggle;
                string tokenText;
                if (sc.TokenText.Contains("-"))
                {
                    toggle = false;
                    tokenText = sc.TokenText.Substring(1, sc.TokenText.Length - 1);
                }
                else
                {
                    toggle = true;
                    tokenText = sc.TokenText;
                }
                if (!inUsers)
                {
                    if (toggle)
                    {
                        if (tokenText == "*")
                        {
							foreach (string s in Program.permissionManager.ActiveNodes)
                            {
                                if(!currentGroup.permissions.ContainsKey(s))
                                    currentGroup.permissions.Add(s, toggle);
                            }
                        }
                        else if (tokenText.Contains("*"))
                        {
                            string temp = tokenText.Remove(tokenText.Length - 2);
							foreach (string s in Program.permissionManager.ActiveNodes)
                            {
                                if (s.Contains(temp))
                                {
                                    if (!currentGroup.permissions.ContainsKey(s))
                                        currentGroup.permissions.Add(s, toggle);
                                }
                            }
                        }
                    }

                    if (!currentGroup.permissions.ContainsKey(tokenText))
                        currentGroup.permissions.Add(tokenText, toggle);
                }
                else
                {
                    if (toggle)
                    {
                        if (tokenText == "*")
                        {
							foreach (string s in Program.permissionManager.ActiveNodes)
                            {
                                currentUser.hasPerm.Add(s);
                            }
                        }
						else if (tokenText.Contains("*"))
						{
							string temp = tokenText.Remove(tokenText.Length - 2);
							foreach (string s in Program.permissionManager.ActiveNodes)
							{
								if (s.Contains(temp))
								{
									currentUser.hasPerm.Add(s);
								}
							}
						}
						else
						{
							currentUser.hasPerm.Add(tokenText);
						}
                    }
                    else
                    {
                        currentUser.notHasPerm.Add(tokenText);
                    }
                }
            }
        }

        public Group GetGroup(string Name)
        {
            foreach (Group grp in groups)
            {
                if (grp.Name == Name)
                    return grp;
            }

            return null;
        }

        public bool IsPermitted(string node, Player player)
        {
			User user;
            if (users.TryGetValue(player.Name, out user))
            {
                return ((user.hasPerm.Contains(node) && player.AuthenticatedAs != null) || player.Op);
            }
            return false;
        }

        private static void CreateDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

    }
}