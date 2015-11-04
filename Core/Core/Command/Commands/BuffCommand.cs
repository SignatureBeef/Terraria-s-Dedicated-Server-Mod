using System;
using OTA.Command;
using Terraria;

namespace TDSM.Core.Command.Commands
{
    public class BuffCommand : CoreCommand
    {
        public override void Initialise()
        {
            Core.AddCommand("abuff")
                .WithAccessLevel(AccessLevel.OP)
                .WithPermissionNode("tdsm.abuff")
                .Calls((ISender sender, ArgumentList args) =>
                {
                    var time = args.GetInt(0);

                    (sender as Player).AddBuff(21, time);

                    NetMessage.SendData(55, -1, -1, "", (sender as Player).whoAmI, 21, time, 0, 0, 0, 0);
                    NetMessage.SendData(55, (sender as Player).whoAmI, -1, "", (sender as Player).whoAmI, 21, time);
                });
        }
    }
}

