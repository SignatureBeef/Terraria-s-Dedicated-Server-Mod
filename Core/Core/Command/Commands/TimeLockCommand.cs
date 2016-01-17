using System;
using OTA.Command;
using Terraria;
using OTA;
using TDSM.Core.Misc;

namespace TDSM.Core.Command.Commands
{
    public class TimeLockCommand : CoreCommand
    {
        public override void Initialise()
        {
            AddCommand("timelock")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Forces the time to stay at a certain point.")
                .WithHelpText("now")
                .WithHelpText("set day|dawn|dusk|noon|night")
                .WithHelpText("setat <time>")
                .WithHelpText("disable")
                .WithPermissionNode("tdsm.timelock")
                .Calls(this.Timelock);
        }

        /// <summary>
        /// Allows an OP to force the time to dtay at a certain point.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void Timelock(ISender sender, ArgumentList args)
        {
            var disable = args.TryPop("disable");
            var setNow = args.TryPop("now");
            var setMode = args.TryPop("set");
            var setAt = args.TryPop("setat");

            if (disable)
            {
                if (!(args.Plugin as Entry).UseTimeLock)
                {
                    sender.Message("Time lock is already disabled", 255, 255, 0, 0);
                    return;
                }

                (args.Plugin as Entry).UseTimeLock = false;
                sender.Message("Time lock has been disabled.", 255, 0, 255, 0);
                return;
            }
            else if (setNow)
                (args.Plugin as Entry).UseTimeLock = true;
            else if (setMode)
            {
                string caseType = args.GetString(0);
                switch (caseType)
                {
                    case "day":
                        {
                            World.SetTime(13500.0);
                            break;
                        }
                    case "dawn":
                        {
                            World.SetTime(0);
                            break;
                        }
                    case "dusk":
                        {
                            World.SetTime(0, false);
                            break;
                        }
                    case "noon":
                        {
                            World.SetTime(27000.0);
                            break;
                        }
                    case "night":
                        {
                            World.SetTime(16200.0, false);
                            break;
                        }
                    default:
                        {
                            sender.Message("Please review your command.", 255, 255, 0, 0);
                            return;
                        }
                }
                (args.Plugin as Entry).UseTimeLock = true;
            }
            else if (setAt)
            {
                double time;
                if (args.TryParseOne<Double>(out time))
                {
                    Core.TimelockTime = time;
                    Core.TimelockRain = Main.raining;
                    Core.TimelockSlimeRain = Main.slimeRain;
                    Core.UseTimeLock = true;
                }
                else
                    throw new CommandError("Double expected.");
            }
            else
                throw new CommandError("Certain arguments expected.");

            if ((args.Plugin as Entry).UseTimeLock)
            {
                if (!setNow)
                    NetMessage.SendData((int)Packet.WORLD_DATA);

                sender.Message(
                    String.Format("Time lock has set at {0}.", (args.Plugin as Entry).TimelockTime),
                    255, 0, 255, 0
                );
            }
        }
    }
}

