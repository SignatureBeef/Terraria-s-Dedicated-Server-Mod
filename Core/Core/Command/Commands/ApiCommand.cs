using System;
using OTA.Command;
using Terraria;
using OTA;

namespace TDSM.Core.Command.Commands
{
    public class ApiCommand : CoreCommand
    {
        public override void Initialise()
        {
            AddCommand("api")
                .WithPermissionNode("tdsm.api")
                .Calls(ManageApi);
        }

        void ManageApi(ISender sender, ArgumentList args)
        {

        }
    }
}

