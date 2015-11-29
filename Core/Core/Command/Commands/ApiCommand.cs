using System;
using OTA.Command;
using Terraria;

namespace TDSM.Core.Command.Commands
{
    public class ApiCommand : CoreCommand
    {
        public override void Initialise()
        {
            Core.AddCommand("api")
                .WithAccessLevel(AccessLevel.OP)
                .WithPermissionNode("tdsm.api")
                .Calls(ManageApi);
        }

        void ManageApi(ISender sender, ArgumentList args)
        {

        }
    }
}

