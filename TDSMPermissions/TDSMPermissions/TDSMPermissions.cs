using System;
using System.IO;
using System.Collections.Generic;

using Terraria_Server;
using Terraria_Server.Misc;
using Terraria_Server.Logging;
using Terraria_Server.Permissions;
using TDSMPermissions.Definitions;

using YaTools.Yaml;
using Terraria_Server.Plugins;

namespace TDSMPermissions
{
    public class TDSMPermissions : BasePlugin
    {
        /*
         * @Developers
         * 
         * Plugins need to be in .NET 4.0
         * Otherwise TDSM will be unable to load it. 
         */

        public Properties properties;
        public string pluginFolder;
        public string permissionsYML;

        public bool spawningAllowed = false;
        public bool tileBreakageAllowed = false;
        public bool explosivesAllowed = false;
        public static TDSMPermissions plugin;
		private List<Group> groups = new List<Group>();
		private Dictionary<String, List<String>> users;
		private Group currentGroup;
		public string defaultGroup;
        private YamlScanner sc;
		private bool inUsers;
		private String currentUser;
		private List<String> userNodes;

		private bool inGroups = false;

        protected override void Initialized(object state)
        {
            Name = "TDSMPermissions";
            Description = "Permissions for TDSM.";
            Author = "Malkierian";
            Version = "1";
            TDSMBuild = 32;

            plugin = this;

            pluginFolder = Statics.PluginPath + Path.DirectorySeparatorChar + "TDSMPermissions";
            permissionsYML = pluginFolder + Path.DirectorySeparatorChar + "permissions.yml";

            //Create folder if it doesn't exist
            CreateDirectory(pluginFolder);

            if (!File.Exists(permissionsYML))
                File.Create(permissionsYML).Close();

            //setup a new properties file
			//properties = new Properties(pluginFolder + Path.DirectorySeparatorChar + "tdsmplugin.properties");
			//properties.Load();
			//properties.pushData(); //Creates default values if needed. [Out-Dated]
			//properties.Save();

            //read properties data
			Node.isPermittedImpl = this.isPermitted;
			LoadPerms();
        }

        protected override void Enabled()
        {
            ProgramLog.Log(base.Name + " enabled.");
            //Register Hooks
			//registerHook(Hooks.PLAYER_PRELOGIN);

            //Add Commands
        }

        protected override void Disabled()
        {
            ProgramLog.Log(base.Name + " disabled.");
        }

		//public override void onPlayerPreLogin(Terraria_Server.Events.PlayerLoginEvent Event)
		//{
		//    base.onPlayerPreLogin(Event);
		//}

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
										ProcessInheritance();
										break;
									}
								case "groups":
									{
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
					currentUser = sc.TokenText;
				}
			}
		}

		private void ProcessInfo()
		{
			bool Default;
            string Prefix;
            string Suffix;
			Color color;
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
			//while (sc.TokenText != "color")
			//{
			//    sc.NextToken();
			//}
			//ProgramLog.Debug.Log("Color token found");
			//while (sc.NextToken() != Token.TextContent)
			//{ }
			//color = GetChatColor(sc.TokenText);
			currentGroup.SetGroupInfo(Default, Prefix, Suffix, ChatColor.Tan);
		}

		private void ProcessInheritance()
		{
			while (sc.NextToken() != Token.TextContent)
			{
				if (sc.Token == Token.Outdent)
					return;
			}
			if (sc.TokenText == "permissions")
			{
				ProcessPermissions();
				return;
			}
			while (sc.Token != Token.Outdent)
			{
				if (sc.Token == Token.TextContent)
				{
					currentGroup.Inherits.Add(sc.TokenText);
				}
			}
		}

		private void ProcessPermissions()
		{
			while (sc.NextToken() != Token.Outdent)
			{
				while (sc.NextToken() != Token.TextContent)
				{
					if (sc.Token == Token.Outdent)
						return;
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
					currentGroup.permissions.Add(tokenText, toggle);
				}
				else
				{
					userNodes.Add(tokenText);
				}
				ProgramLog.Debug.Log("Node " + tokenText + " added with " + toggle + " status");
			}
			groups.Add(currentGroup);
		}

		public bool isPermitted(Node node, Player player)
		{
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
