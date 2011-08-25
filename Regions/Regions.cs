using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Plugin;
using Terraria_Server.Commands;
using Terraria_Server.Logging;

namespace Regions
{
    public class Regions : Plugin
    {
        public override void Load()
        {
            base.Name = "Regions";
            base.Description = "A region plguin for TDSM";
            base.Author = "DeathCradle";
            base.Version = "0.1";
            base.TDSMBuild = 32;

            AddCommand("region select")
                .WithAccessLevel(AccessLevel.OP)
                .WithHelpText("Usage:    region select")
                .WithDescription("Turn on the Selectin Tool Mode")
                .Calls(Commands.SelectionToolToggle);
        }

        public override void Enable()
        {
            ProgramLog.Log("Regions for TDSM #" + base.TDSMBuild + " enabled.");
        }

        public override void Disable()
        {
            ProgramLog.Log("Regions disabled.");
        }
    }
}
