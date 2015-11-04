using System;
using OTA.Command;

namespace TDSM.Core.Command.Commands
{
    public class NpcSpawningCommand : CoreCommand
    {
        public override void Initialise()
        {
            Core.AddCommand("npcspawning")
                .WithDescription("Turn NPC spawning on or off.")
                .WithAccessLevel(AccessLevel.OP)
                .WithHelpText("<true|false>")
                .WithPermissionNode("tdsm.npcspawning")
                .Calls(this.NPCSpawning);
        }

        /// <summary>
        /// Enables or disables NPC spawning
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void NPCSpawning(ISender sender, ArgumentList args)
        {
            args.ParseNone();

            Core.StopNPCSpawning = !Core.StopNPCSpawning;
            sender.Message("NPC spawning is now " + (Core.StopNPCSpawning ? "off" : "on") + "!");
        }
    }
}

