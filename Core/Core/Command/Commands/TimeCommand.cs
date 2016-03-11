using System;
using OTA.Command;
using OTA;
using Microsoft.Xna.Framework;
using Terraria;
using TDSM.Core.Misc;
using OTA.Misc;

namespace TDSM.Core.Command.Commands
{
    public class TimeCommand : CoreCommand
    {
        public override void Initialise()
        {
            AddCommand("time", true)
                .WithDescription("Change the time of day")
                .WithAccessLevel(AccessLevel.OP)
                .WithHelpText("set <numeric time>")
                .WithHelpText("set 5:10am")
                .WithHelpText("now|?")
                .WithHelpText("day|dawn|dusk|noon|night")
                .WithPermissionNode("tdsm.time")
                .Calls(this.Time);
            
            AddCommand("fastforwardtime")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Fast forwards time until disabled.")
                .WithPermissionNode("tdsm.fastforwardtime")
                .Calls(this.FastForwardTime);
        }

        /// <summary>
        /// Fast forwards time
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void FastForwardTime(ISender sender, ArgumentList args)
        {
            args.ParseNone();

            Core.TimeFastForwarding = !Core.TimeFastForwarding;
            sender.Message("Time is now " + (Core.TimeFastForwarding ? "fast" : "normal") + "!");
        }

        /// <summary>
        /// Sets the time in the game.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void Time(ISender sender, ArgumentList args)
        {
            double time;
            WorldTime text;
            if (args.TryParseOne<Double>("-set", out time) || args.TryParseOne<Double>("set", out time))
            {
                if (time >= WorldTime.TimeMin && time <= WorldTime.TimeMax)
                {
                    World.SetTime(time);
                }
                else
                {
                    sender.SendMessage(String.Format("Invalid time specified, must be from {0} to {1}", WorldTime.TimeMin, WorldTime.TimeMax));
                    return;
                }
            }
            else if (args.TryParseOne<WorldTime>("-set", out text) || args.TryParseOne<WorldTime>("set", out text))
            {
                time = text.GameTime;
                World.SetParsedTime(time);
            }
            else
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
                    case "?":
                    case "now":
                    case "-now":
                        {
                            sender.Message("Current time: " + WorldTime.Parse(World.GetParsableTime()).ToString());
                            return;
                        }
                    default:
                        {
                            sender.Message("Please review your command");
                            return;
                        }
                }
            }

            NetMessage.SendData((int)Packet.WORLD_DATA); //Update Data
            var current = WorldTime.Parse(World.GetParsableTime()).Value;
            Tools.NotifyAllPlayers(String.Format("Time set to {0} ({1}) by {2}", current.ToString(), current.GameTime, sender.SenderName), Color.Green);
        }
    }
}

