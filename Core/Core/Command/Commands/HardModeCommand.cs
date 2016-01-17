using System;
using OTA;
using OTA.Command;
using Terraria;

namespace TDSM.Core.Command.Commands
{
    public class HardModeCommand : CoreCommand
    {
        public override void Initialise()
        {
            AddCommand("hardmode")
                .WithAccessLevel(AccessLevel.OP)
                .SetDefaultUsage()
                .WithDescription("Enables hard mode.")
                .WithPermissionNode("tdsm.hardmode")
                .Calls(this.HardMode);
        }

        /// <summary>
        /// Enables hardmode
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void HardMode(ISender sender, ArgumentList args)
        {
            args.ParseNone();

            if (Main.hardMode)
                throw new CommandError("Hard mode is already enabled");

            sender.Message("Changing to hard mode...");
            WorldGen.IsGeneratingHardMode = true;
            Terraria.WorldGen.StartHardmode();
            while (WorldGen.IsGeneratingHardMode)
                System.Threading.Thread.Sleep(5);
            sender.Message("Hard mode is now enabled.");
        }
    }
}

