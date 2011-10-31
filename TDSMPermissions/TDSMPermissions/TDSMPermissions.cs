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
using TDSMPermissions.Perms;

namespace TDSMPermissions
{
    public class TDSMPermissions : BasePlugin
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
            TDSMBuild = 36;
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
        }

        protected override void Enabled()
        {
            ProgramLog.Log(base.Name + " enabled.");
        }

        protected override void Disabled()
        {
            ProgramLog.Log(base.Name + " disabled.");
        }
        
        [Hook(HookOrder.NORMAL)]
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
                        , args.Color, false
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
                        , args.Color, false
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
                return ((user.hasPerm.Contains(node) && player.AuthenticatedAs != null) || player.Op);

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

public static class Chat
{
    public static Color DEFAULT_CHATCOLOR = ChatColor.AntiqueWhite;

    public static bool IsValidColor(Color color)
    {
        return color != default(Color) && color != DEFAULT_CHATCOLOR;
    }

    public static bool TryGetChatColor(User user, out Color color)
    {
        color = default(Color);

        if (IsValidColor(user.chatColor))
            color = user.chatColor;
        else if (user.group.Count > 0)
        {
            Group grp = Permissions.GetGroup(user.group[0]);
            if (grp != null && IsValidColor(grp.GroupInfo.color))
                color = grp.GroupInfo.color;
        }

        return color != default(Color);
    }

    public static bool TryGetChatPrefix(User user, out string prefix)
    {
        prefix = default(String);

        if (user.prefix != default(String) && user.prefix.Trim().Length > 0)
            prefix = user.prefix;
        else if (user.group.Count > 0)
        {
            Group grp = Permissions.GetGroup(user.group[0]);
            if (grp != null && grp.GroupInfo.Prefix != default(String) && grp.GroupInfo.Prefix.Trim().Length > 0)
                prefix = grp.GroupInfo.Prefix;
        }

        return prefix != default(String);
    }

    public static bool TryGetChatSuffix(User user, out string suffix)
    {
        suffix = default(String);

        if (user.suffix != default(String) && user.suffix.Trim().Length > 0)
            suffix = user.suffix;
        else if (user.group.Count > 0)
        {
            Group grp = Permissions.GetGroup(user.group[0]);
            if (grp != null && grp.GroupInfo.Suffix != default(String) && grp.GroupInfo.Suffix.Trim().Length > 0)
                suffix = grp.GroupInfo.Suffix;
        }

        return suffix != default(String);
    }

    public static bool TryGetChatSeperator(User user, out string seperator)
    {
        seperator = default(String);

        if (user.seperator != default(String) && user.seperator.Trim().Length > 0)
            seperator = user.seperator;
        else if (user.group.Count > 0)
        {
            Group grp = Permissions.GetGroup(user.group[0]);
            if (grp != null && grp.GroupInfo.Seperator != default(String) && grp.GroupInfo.Seperator.Trim().Length > 0)
                seperator = grp.GroupInfo.Seperator;
        }

        return seperator != default(String);
    }
}