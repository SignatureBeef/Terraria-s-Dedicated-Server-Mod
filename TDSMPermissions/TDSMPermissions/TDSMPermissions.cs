using System;
using System.IO;
using System.Collections.Generic;

using Terraria_Server;
using Terraria_Server.Misc;
using Terraria_Server.Logging;
using Terraria_Server.Plugins;
using Terraria_Server.Commands;
using Terraria_Server.Permissions;

using TDSMPermissions.Auth;
using TDSMPermissions.Perms;
using TDSMPermissions.Definitions;

using YaTools.Yaml;

namespace TDSMPermissions
{
    public partial class TDSMPermissions : BasePlugin
    {
        public Properties properties;
        public string pluginFolder;
        public string permissionsYML;

        public TDSMPermissions()
        {
            Name = "TDSMPermissions";
            Description = "Permissions for TDSM.";
            Author = "Malkierian";
            Version = "1";
            TDSMBuild = 37;
        }

        protected override void Initialized(object state)
        {
            pluginFolder = Statics.PluginPath + Path.DirectorySeparatorChar + "TDSMPermissions";
            permissionsYML = pluginFolder + Path.DirectorySeparatorChar + "permissions.yml";

            //Create folder if it doesn't exist
            CreateDirectory(pluginFolder);

            if (!File.Exists(permissionsYML))
                File.Create(permissionsYML).Close();

            //set internal permission check method to plugins handler
            Program.permissionManager.IsPermittedImpl = IsPermitted;
			Statics.PermissionsEnabled = true;

            AddCommand("permsadd")
                .WithAccessLevel(AccessLevel.OP)
                .WithPermissionNode("permissions.permsadd")
                .WithDescription("Add permissions to a Player.")
                .WithHelpText("Usage: ")
                .Calls(Perms_Add);
        }

        protected override void Enabled()
        {
            ProgramLog.Plugin.Log(base.Name + " enabled.");
        }

        protected override void Disabled()
        {
			ProgramLog.Plugin.Log(base.Name + " disabled.");
        }
        
        [Hook(HookOrder.FIRST)]
        void OnChat(ref HookContext ctx, ref HookArgs.PlayerChat args)
        {
            if (ctx.Player.AuthenticatedAs != null)
            {
                User usr;
                if (Permissions.users.TryGetValue(ctx.Player.Name, out usr))
                {
                    Color col;
                    if (Chat.TryGetChatColor(usr, out col))
                        args.Color = col;

                    string prefix;
                    if (Chat.TryGetChatPrefix(usr, out prefix))
                        usr.prefix = prefix;

                    string suffix;
                    if (Chat.TryGetChatSuffix(usr, out suffix))
                        usr.suffix = suffix;

                    string seperator;
                    Chat.TryGetChatSeperator(usr, out seperator);
                    
                    ctx.SetResult(HookResult.IGNORE);
                    Server.notifyAll(
                        String.Concat(usr.prefix, ctx.Player.Name, seperator, args.Message, usr.suffix)
                        , args.Color
                    );
                }
            }
            else if (ctx.Player.AuthenticatedAs == null)
            {
                Group dGrp = Permissions.GetDefaultGroup();
                if (dGrp != null)
                {
                    if (Chat.IsValidColor(dGrp.GroupInfo.color))
                        args.Color = dGrp.GroupInfo.color;

                    ctx.SetResult(HookResult.IGNORE);
                    Server.notifyAll(
                        String.Concat(  dGrp.GroupInfo.Prefix, ctx.Player.Name, 
                                        dGrp.GroupInfo.Seperator, args.Message, 
                                        dGrp.GroupInfo.Suffix)
                        , args.Color
                    );
                }
            }
        }

        [Hook(HookOrder.LATE)]
        void OnPluginsLoaded(ref HookContext ctx, ref HookArgs.PluginsLoaded args)
        {
            Permissions.LoadPerms(permissionsYML);

            if (!Server.UsingLoginSystem)
                Login.InitSystem();

            if (Login.IsRestrictRunning())
                ProgramLog.Plugin.Log("Your Server is now protected!");
            else
                ProgramLog.Plugin.Log("Your Server is vulnerable, Get an Authentication system!");
        }               

        //This is what TDSM will check.
        public bool IsPermitted(string node, Player player)
        {
			User user;
            if (Permissions.users.TryGetValue(player.Name, out user))
                return ((player.AuthenticatedAs != null && IsUserAllowed(node, user)) || player.Op);

            return false;
        }

        public bool IsUserAllowed(string Node, User user)
        {
            if (user.hasPerm.Contains(Node) || user.hasPerm.Contains("*"))
                return true;

            Group grp;
            foreach (string group in user.group)
            {
                grp = Permissions.GetGroup(group);

                if (grp != null && grp.permissions != null && (grp.permissions.ContainsKey(Node) || grp.permissions.ContainsKey("*")))
                    return true;
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