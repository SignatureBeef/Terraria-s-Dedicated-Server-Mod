using System;
using System.IO;
using System.Collections.Generic;

using Terraria_Server;
using Terraria_Server.Misc;
using Terraria_Server.Plugin;
using Terraria_Server.Logging;
using Terraria_Server.Permissions;
using TDSMPermissions.Definitions;

using YaTools.Yaml;

namespace TDSMPermissions
{
    public class TDSMPermissions : Plugin
    {
        /*
         * @Developers
         * 
         * Plugins need to be in .NET 4.0
         * Otherwise TDSM will be unable to load it. 
         * 
         * As of June 16, 1:15 AM, TDSM should now load Plugins Dynamically.
         */

        public Properties properties;
		public String pluginFolder;
        public String permissionsYML;

        public bool spawningAllowed = false;
        public bool tileBreakageAllowed = false;
        public bool explosivesAllowed = false;
        public static TDSMPermissions plugin;
		private List<Group> groups = new List<Group>();
		private Group currentGroup;
		public string defaultGroup;
        private YamlScanner sc;

		private bool inGroups = false;

        public override void Load()
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

        public override void Enable()
        {
            ProgramLog.Log(base.Name + " enabled.");
            //Register Hooks
			registerHook(Hooks.PLAYER_PRELOGIN);

            //Add Commands
        }

        public override void Disable()
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

			while ((to = sc.NextToken()) != Token.EndOfStream)
			{
				switch (to)
				{
					//case Token.BeginningOfStream:
					//case Token.BlockMappingBegin:
					//case Token.LeadingWhiteSpace:
					//case Token.PlainScalar:
					//case Token.ImplicitKey:
					//case Token.Newline:
					//case Token.EscapeWhiteSpace:
					//case Token.Directive:
					//case Token.DirectivesEnd:
					//case Token.BlockSeqBegin:
					//case Token.BlockKeyIndicator:
					//case Token.FlowKeyIndicator:
					//case Token.OpenBrace:
					//case Token.CloseBrace:
					//case Token.OpenBracket:
					//case Token.CloseBracket:
					//case Token.PlainEnd:
					//case Token.NoOp:
					//case Token.Comment:
					//case Token.EmptyLine:
					//case Token.Anchor:
					//case Token.Alias:
					//case Token.At:
					//case Token.Backtick:
					//case Token.Tag:
					//case Token.Literal:
					//case Token.Folded:
					//    break;
					case Token.TextContent:
						{
							switch (sc.TokenText)
							{
								//case "groups":
								//    {
								//        while (sc.NextToken() != Token.TextContent)
								//        {
								//        }
								//        currentGroup = new Group(sc.TokenText);
								//        ProgramLog.Debug.Log("Group name: " + currentGroup.Name);
								//        break;
								//    }
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
			//foreach (Group g in groups)
			//{
			//    ProgramLog.Debug.Log("Group info for group " + g.Name + ":");
			//    ProgramLog.Debug.Log("Default: " + g.GroupInfo.Default);
			//    ProgramLog.Debug.Log("Prefix: " + g.GroupInfo.Prefix);
			//    ProgramLog.Debug.Log("Suffix: " + g.GroupInfo.Suffix);
			//    ProgramLog.Debug.Log("Permissions:");
			//    foreach (String p in g.permissions.Keys)
			//    {
			//        bool value;
			//        g.permissions.TryGetValue(p, out value);
			//        ProgramLog.Debug.Log(p + ": " + value);
			//    }
			//}
		}

		private void ProcessIndent()
		{
			String tokenText = sc.TokenText;
			if (sc.NextToken() == Token.IndentSpaces)
				tokenText += sc.TokenText;
			if (tokenText == "    ")
			{
				while (sc.NextToken() != Token.TextContent)
				{
				}
				currentGroup = new Group(sc.TokenText);
				ProgramLog.Debug.Log("Group name: " + currentGroup.Name);
			}
		}

		private void ProcessInfo()
		{
			bool Default;
			String Prefix;
			String Suffix;
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
			//color = GetChatColour(sc.TokenText);
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
					currentGroup.Inherits.Add(sc.TokenText);
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
				String tokenText;
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
				currentGroup.permissions.Add(tokenText, toggle);
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
