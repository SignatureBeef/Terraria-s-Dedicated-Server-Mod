using System;
using Terraria;
using OTA;
using OTA.Command;

namespace TDSM.Core.Command.Commands
{
    public class StatusCommand : CoreCommand
    {
        public override void Initialise()
        {
            AddCommand("status")
                .WithDescription("Server status")
                .SetDefaultUsage()
                .WithPermissionNode("tdsm.status")
                .Calls(this.ServerStatus);
        }

        /// <summary>
        /// Outputs statistics of the servers performance.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void ServerStatus(ISender sender, ArgumentList args)
        {
            args.ParseNone();

            var process = System.Diagnostics.Process.GetCurrentProcess();
            sender.Message(String.Format("Virtual memory:  {0:0.0}/{1:0.0}MB",
                    process.VirtualMemorySize64 / 1024.0 / 1024.0,
                    process.PeakVirtualMemorySize64 / 1024.0 / 1024.0));
            sender.Message(String.Format("Physical memory: {0:0.0}/{1:0.0}MB",
                    process.WorkingSet64 / 1024.0 / 1024.0,
                    process.PeakWorkingSet64 / 1024.0 / 1024.0));
            var time = process.TotalProcessorTime;
            sender.Message(String.Format("Total cpu usage:        {0:0.00}% ({1})",
                    100.0 * time.TotalMilliseconds / (DateTime.Now - process.StartTime).TotalMilliseconds, time));

            //if (LoadMonitor.LoadLastSecond >= 0)
            //    sender.Message(String.Format("Cpu usage last second:  {0:0.00}%", LoadMonitor.LoadLastSecond));

            //if (LoadMonitor.LoadLastMinute >= 0)
            //    sender.Message(String.Format("Cpu usage last minute:  {0:0.00}%", LoadMonitor.LoadLastMinute));

            //sender.Message(String.Format("Last world update took: {0:0.000}ms (plr: {1:0.0}ms, npc: {2:0.0}ms, proj: {3:0.0}ms, item: {4:0.0}ms, world: {5:0.0}ms, time: {6:0.0}ms, inva: {7:0.0}ms, serv: {8:0.0}ms)",
            //    Program.LastUpdateTime.TotalMilliseconds,
            //    Main.LastPlayerUpdateTime.TotalMilliseconds,
            //    Main.LastNPCUpdateTime.TotalMilliseconds,
            //    Main.LastProjectileUpdateTime.TotalMilliseconds,
            //    Main.LastItemUpdateTime.TotalMilliseconds,
            //    Main.LastWorldUpdateTime.TotalMilliseconds,
            //    Main.LastTimeUpdateTime.TotalMilliseconds,
            //    Main.LastInvasionUpdateTime.TotalMilliseconds,
            //    Main.LastServerUpdateTime.TotalMilliseconds
            //    ));

            var projs = 0;
            var uprojs = 0;
            var npcs = 0;
            var unpcs = 0;
            var items = 0;

            foreach (var npc in Main.npc)
            {
                if (!npc.active)
                    continue;
                npcs += 1;
                if (!npc.netUpdate)
                    continue;
                unpcs += 1;
            }

            foreach (var proj in Main.projectile)
            {
                if (!proj.active)
                    continue;
                projs += 1;
                if (!proj.netUpdate)
                    continue;
                uprojs += 1;
            }

            foreach (var item in Main.item)
            {
                if (!item.active)
                    continue;
                items += 1;
            }

            sender.Message(String.Format("NPCs: {0}a/{1}u, projectiles: {2}a/{3}u, items: {4}", npcs, unpcs, projs, uprojs, items));
            //long diff = Connection.TotalOutgoingBytesUnbuffered - Connection.TotalOutgoingBytes;
            //sender.Message(String.Format("NPCs: {0}a/{1}u, projectiles: {2}a/{3}u, items: {4}, bytes saved: {5:0.0}K ({6:0.0}%)", npcs, unpcs, projs, uprojs, items, diff, diff * 100.0 / Connection.TotalOutgoingBytesUnbuffered));

            //#if BANDWIDTH_ANALYSIS
            //          var sb = new System.Text.StringBuilder ();
            //          for (int i = 0; i < 255; i++)
            //          {
            //              var p = Networking.Connection.packetsPerMessage [i];
            //              var b = Networking.Connection.bytesPerMessage [i];
            //              if (p > 0)
            //                  sb.AppendFormat ("{0}({1}p, {2}B), ", (Packet)i, p, b);
            //          }
            //          
            //          sender.Message (sb.ToString());
            //#endif
        }
    }
}

