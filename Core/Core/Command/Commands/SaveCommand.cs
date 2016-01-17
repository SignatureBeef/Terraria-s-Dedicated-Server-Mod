using System;
using OTA.Command;
using OTA;
using Terraria;
using System.Threading;

namespace TDSM.Core.Command.Commands
{
    public class SaveCommand : CoreCommand
    {
        public override void Initialise()
        {
            AddCommand("save", true)
                .WithDescription("Save world and configuration data")
                .WithAccessLevel(AccessLevel.OP)
                .SetDefaultUsage()
                .WithPermissionNode("tdsm.admin")
                .Calls(this.SaveAll);

            AddCommand("save-all")
                .WithDescription("Save world and configuration data")
                .WithAccessLevel(AccessLevel.OP)
                .SetDefaultUsage()
                .WithPermissionNode("tdsm.admin")
                .Calls(this.SaveAll);
        }

        /// <summary>
        /// Executes the world data save routine.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void SaveAll(ISender sender, ArgumentList args)
        {
            Utils.NotifyAllOps("Saving world.....");

            Terraria.IO.WorldFile.saveWorld();

            while (WorldGen.saveLock)
                Thread.Sleep(100);

            //            Utils.NotifyAllOps("Saving data...", true);

            //Server.BanList.Save();
            //Server.WhiteList.Save();
            //Server.IsOp()List.Save();

            Utils.NotifyAllOps("Saving Complete.", true);
        }
    }
}

