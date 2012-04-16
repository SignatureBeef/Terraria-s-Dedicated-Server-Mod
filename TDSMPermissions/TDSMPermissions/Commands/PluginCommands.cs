using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server;
using Terraria_Server.Permissions;
using Terraria_Server.Commands;
using Terraria_Server.Misc;

using TDSMPermissions;
using TDSMPermissions.Definitions;
using TDSMPermissions.Perms;

namespace TDSMPermissions
{
    /// <summary>
    /// TODO: Allow saving.
    /// </summary>
    public partial class TDSMPermissions
    {
        void Perms_Add(ISender sender, ArgumentList args)
        {
            //permsadd -group <group> -node <node>
            //permsadd -user <user> -group <group>
            //permsadd -user <user> -node <node>
            var Name = "";
            var Secondary = "";

            if (args.TryParseTwo("-group", out Name, "-node", out Secondary))
            {
                Group grp = Permissions.GetGroup(Name);
                if (grp == null)
                    throw new CommandError("Group not found.");

                var token = Secondary.Substring(0, 1) == "-"; //Allowed or not?.
                grp.permissions.Add(Secondary, token);
            }
            else if (args.TryParseTwo("-user", out Name, "-group", out Secondary))
            {
                Group grp = Permissions.GetGroup(Secondary);
                if (grp == null)
                    throw new CommandError("Group not found.");

                User usr = Permissions.GetUser(Name);
                if (usr == null)
                    throw new CommandError("User not found.");

                usr.group.Add(Secondary);
            }
            else if (args.TryParseTwo("-user", out Name, "-node", out Secondary))
            {
                User usr = Permissions.GetUser(Name);
                if (usr == null)
                    throw new CommandError("User not found.");

                usr.hasPerm.Add(Secondary);
            }
            else
                throw new CommandError("Unknown command.");

            sender.sendMessage(
                String.Format("`{0}` successfully added to `{1}`", Secondary, Name)
            );
        }
    }
}
