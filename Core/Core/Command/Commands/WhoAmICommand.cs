using System;
using OTA;
using OTA.Command;
using System.Text;
using TDSM.Core.Data;
using Microsoft.Xna.Framework;
using Terraria;

namespace TDSM.Core.Command.Commands
{
    public class WhoAmICommand : CoreCommand
    {
        public override void Initialise()
        {
            AddCommand("whoami")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithPermissionNode("tdsm.whoami")
                .WithDescription("Find out if you are authenticated")
                .SetDefaultUsage()
                .Calls(this.WhoAmI);
        }

        private void WhoAmI(ISender sender, string args)
        {
            var player = sender as Player;
            if (player != null)
            {
                var sb = new StringBuilder();
                sb.Append("You are ");
                if (!player.IsAuthenticated()) sb.Append("not ");

                if (player.IsOp()) sb.Append("an operator");
                else sb.Append("logged in");

                var groupName = String.Empty;
                if (Storage.IsAvailable)
                {
                    var grp = Storage.GetInheritedGroup(player);
                    if (grp != null) groupName = grp.Name;
                }

                if (!String.IsNullOrEmpty(groupName))
                {
                    sb.Append(" and are a part of the ");
                    sb.Append(groupName);
                    sb.Append(" group");
                }
                else sb.Append(" but not apart of any groups");

                sb.Append(".");
                sender.Message(sb.ToString(), Color.Orange);
            }
            else sender.Message("This command is for players only", Color.Red);
        }
    }
}

