using System;
using OTA.Command;
using OTA.Misc;
using Microsoft.Xna.Framework;
using OTA;

namespace TDSM.Core.Command.Commands
{
    public class PlatformCommand : CoreCommand
    {
        public override void Initialise()
        {
            AddCommand("platform")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Show what type of server is running TDSM")
                .SetDefaultUsage()
                .WithPermissionNode("tdsm.platform")
                .Calls(this.OperatingSystem);
        }

        /// <summary>
        /// Informs the sender of what system TDSM is running on
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="args">Arguments.</param>
        public void OperatingSystem(ISender sender, ArgumentList args)
        {
            var platform = Platform.Type.ToString();

            switch (Platform.Type)
            {
                case Platform.PlatformType.LINUX:
                    platform = "Linux";
                    break;
                case Platform.PlatformType.MAC:
                    platform = "Mac";
                    break;
                case Platform.PlatformType.WINDOWS:
                    platform = "Windows";
                    break;
            }

            sender.Message("TDSM is running on OS: " + platform, Color.DarkGreen);
        }
    }
}

