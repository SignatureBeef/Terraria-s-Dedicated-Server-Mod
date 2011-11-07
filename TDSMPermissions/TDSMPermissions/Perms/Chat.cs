using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Misc;
using TDSMPermissions.Definitions;

namespace TDSMPermissions.Perms
{
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
}
