using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TDSMPermissions.Definitions;
using YaTools.Yaml;
using System.IO;
using Terraria_Server.Misc;
using Terraria_Server.Logging;
using Terraria_Server;
using System.Threading;

namespace TDSMPermissions.Perms
{
    public static class Permissions
    {
        public  static List<Group>              groups          { get; set; }
        public  static Dictionary<String, User> users           { get; set; }
        private static User                     currentUser     { get; set; }
        private static Group                    currentGroup    { get; set; }
        private static YamlScanner              sc              { get; set; }
        private static bool                     inUsers         { get; set; }
        private static string                   currentUserName { get; set; }

        static Permissions()
        {
            groups = new List<Group>();
            users = new Dictionary<String, User>();
            currentUser = new User();
        }

        public static void LoadPerms(string permissionsYML)
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

        public static void ProcessGroups()
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
                                currentUser.hasPerm.Add(node);
                            else
                                currentUser.notHasPerm.Add(node);
                        }
                        currentUser.group.Add(group.Name);
                        if (!users.ContainsKey(currentUserName))
                        {
                            string[] sUsers = currentUserName.Split(',');

                            foreach (string usr in sUsers)
                            {
                                users.Add(usr, currentUser);
                            }
                        }
                    }
                }
            }
        }

        public static void ProcessIndent()
        {
            string tokenText = sc.TokenText;
            if (sc.NextToken() == Token.IndentSpaces)
                tokenText += sc.TokenText;

            if (tokenText == "    ")
            {
                while (sc.NextToken() != Token.TextContent) { }

                if (!inUsers)
                    currentGroup = new Group(sc.TokenText);
                else
                {
                    currentUserName = sc.TokenText;
                    currentUser = new User();
                }
            }
        }

        public static void WaitNext(YamlScanner sc, string node)
        {
            while (sc.TokenText != node && sc.Token != Token.EndOfStream && sc.Token != Token.Outdent)
                sc.NextToken();
        }

        public const string CrLf = "\r\n";
        public static string GetNextToken(YamlScanner sc)
        {
            while (sc.NextToken() != Token.TextContent && sc.TokenText != CrLf && sc.Token != Token.EndOfStream) { }
                //Thread.Sleep(100);

            return (sc.TokenText != CrLf) ? sc.TokenText : String.Empty;
        }

        public static void ProcessInfo()
        {
            bool Default = false;
            bool CanBuild = false;
            string Prefix = "";
            string Suffix = "";
            string Seperator = ": ";
            Color color = new Color(255, 255, 255);
			int Rank = -1;

			while (sc.Token != Token.Outdent && sc.Token != Token.EndOfStream)
			{
				switch (sc.TokenText.ToLower())
				{
					case "default":
						{
							try
							{
								Default = Convert.ToBoolean(GetNextToken(sc));
							}
							catch
							{
							}
							break;
						}
					case "prefix":
						{
							Prefix = GetNextToken(sc);
							break;
						}
					case "suffix":
						{
							Suffix = GetNextToken(sc);
							break;
						}
					case "separator":
						{
							Seperator = GetNextToken(sc);
							break;
						}
					case "build":
						{
							string RE = GetNextToken(sc);
							try
							{
								CanBuild = Convert.ToBoolean(RE);
							}
							catch
							{
							}
							break;
						}
					case "color":
						{
							Color col;
							string token = GetNextToken(sc);
							if (Color.TryParseRGB(sc.TokenText, out col))
								color = col;
							break;
						}
					case "rank":
						{
							Rank = Convert.ToInt32(GetNextToken(sc));
							break;
						}

				}
				sc.NextToken();
			}

            if (inUsers)
                currentUser.SetUserInfo(Prefix, Suffix, Seperator, CanBuild, color);
            else
                currentGroup.SetGroupInfo(Default, CanBuild, Prefix, Suffix, Seperator, color, Rank);
        }

        public static void ParseInheritance()
        {
            while (sc.NextToken() != Token.TextContent)
            {
                if (sc.Token == Token.Outdent)
                    return;
            }
            currentGroup.Inherits.Add(sc.TokenText);
        }

        public static void ProcessInheritance()
        {
            //ProgramLog.Debug.Log("Processing group inheritances");
            foreach (Group group in groups)
            {
                //ProgramLog.Debug.Log("Processing " + group.Name + "'s inheritance");
                group.Inherits.ForEach(
                    delegate(string s)
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
                    }
                );
            }
        }

        public static void ProcessPermissions()
        {
            while (sc.NextToken() != Token.Outdent)
            {
                while (sc.NextToken() != Token.TextContent)
                {
                    if (sc.Token == Token.Outdent)
                    {
                        if (!inUsers)
                            groups.Add(currentGroup);
                        else if (!users.ContainsKey(currentUserName))
                            users.Add(currentUserName, currentUser);
                        return;
                    }
                }

                bool toggle = !(sc.TokenText[0] == '-');
                string tokenText = sc.TokenText;

                if (!toggle)
                    tokenText = sc.TokenText.Trim('-');

                if (!inUsers)
                {
                    if (toggle)
                    {
                        if (tokenText == "*")
                        {
                            foreach (string s in Program.permissionManager.ActiveNodes)
                            {
                                if (!currentGroup.permissions.ContainsKey(s))
                                    currentGroup.permissions.Add(s, toggle);
                            }
                        }
                        else if (tokenText.Contains("*"))
                        {
                            string temp = tokenText.Remove(tokenText.Length - 2);

                            foreach (string s in Program.permissionManager.ActiveNodes)
                                if (s.Contains(temp))
                                {
                                    if (!currentGroup.permissions.ContainsKey(s))
                                        currentGroup.permissions.Add(s, toggle);
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
                                currentUser.hasPerm.Add(s);
                        }
                        else if (tokenText.Contains("*"))
                        {
                            string temp = tokenText.Remove(tokenText.Length - 2);
                            foreach (string s in Program.permissionManager.ActiveNodes)
                            {
                                if (s.Contains(temp))
                                    currentUser.hasPerm.Add(s);
                            }
                        }
                        else
                            currentUser.hasPerm.Add(tokenText);
                    }
                    else
                        currentUser.notHasPerm.Add(tokenText);
                }
            }
        }

        public static Group GetGroup(Func<Group, Boolean> predicate)
        {
            foreach (Group grp in groups.Where(predicate))
                return grp;

            return null;
        }

        public static User GetUser(Func<KeyValuePair<String, User>, Boolean> predicate)
        {
            foreach (KeyValuePair<String, User> pair in users.Where(predicate))
                return pair.Value;

            return null;
        }

        public static Group GetGroup(string Name)
        {
            return GetGroup(x => x.Name == Name);
        }

        public static User GetUser(string Name)
        {
            return GetUser(x => x.Key == Name);
        }

        public static Group GetDefaultGroup()
        {
            return GetGroup(x => x.GroupInfo.Default);
        }

        public static bool CanUserBuild(User user)
        {
            if(user.CanBuild)
                return true;

            bool canBuild = false;
            user.group.ForEach(
                delegate(string grpName)
                {
                    Group grp = GetGroup(grpName);
                    if (grp != null & grp.GroupInfo.CanBuild)
                    {
                        canBuild = true;
                        return;
                    }
                }
            );

            return canBuild;
        }
    }
}
