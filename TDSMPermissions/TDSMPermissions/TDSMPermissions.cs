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
            
            //Add Commands
			AddCommand("permissions")
				.WithAccessLevel(AccessLevel.PLAYER)
				.WithDescription("Test command")
				.Calls(Commands.PluginCommands.PermissionsCommand);

            Program.permissionManager.AddNodes(Permissions.nodesToAdd);
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
                if (Permissions.users.TryGetValue(ctx.Player.Name, out usr))
                {
                    if (usr.chatColor != default(Color) && usr.chatColor != ChatColor.AntiqueWhite)
                        args.Color = usr.chatColor;
                    else if (usr.group.Count > 0)
                    {
                        Group grp = Permissions.GetGroup(usr.group[0]);
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