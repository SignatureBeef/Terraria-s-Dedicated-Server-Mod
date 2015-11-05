using System;
using Terraria;
using OTA.Command;
using OTA.Logging;
using OTA;
using TDSM.Core.Misc;

namespace TDSM.Core.Command.Commands
{
    public class WorldEventCommand : CoreCommand
    {
        public override void Initialise()
        {
            Core.AddCommand("worldevent")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Start or stop an event")
                .WithHelpText("eclipse|bloodmoon|pumpkinmoon|snowmoon|slimerain")
                .WithPermissionNode("tdsm.worldevent")
                .Calls(this.WorldEvent);
        }

        /// <summary>
        /// Starts an event
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void WorldEvent(ISender sender, ArgumentList args)
        {
            string first;
            args.TryPopOne(out first);
            switch (first)
            {
                case "eclipse":
                    if (!Main.eclipse)
                    {
                        _disableActiveEvents(sender);

                        World.SetTime(0);
                        //OTA.Callbacks.MainCallback.StartEclipse = true;
                        Main.eclipse = true;

                        NetMessage.SendData(25, -1, -1, Lang.misc[20], 255, 50f, 255f, 130f, 0);
                        NetMessage.SendData((int)Packet.WORLD_DATA);

                        ProgramLog.Admin.Log(Lang.misc[20]);
                    }
                    else
                    {
                        Main.eclipse = false;
                        sender.Message("The eclipse was disabled.");
                    }
                    break;
                case "snowmoon":
                    if (!Main.snowMoon)
                    {
                        _disableActiveEvents(sender);
                        World.SetTime(16200.0, false);
                        NetMessage.SendData(25, -1, -1, Lang.misc[34], 255, 50f, 255f, 130f, 0);
                        Main.startSnowMoon();
                        NetMessage.SendData((int)Packet.WORLD_DATA);

                        ProgramLog.Admin.Log(Lang.misc[34]);
                        ProgramLog.Admin.Log("First Wave: Zombie Elf and Gingerbread Man");
                    }
                    else
                    {
                        Main.stopMoonEvent();
                        sender.Message("The snow moon was disabled.");
                    }
                    break;
                case "pumpkinmoon":
                    if (!Main.pumpkinMoon)
                    {
                        _disableActiveEvents(sender);
                        World.SetTime(16200.0, false);
                        NetMessage.SendData(25, -1, -1, Lang.misc[31], 255, 50f, 255f, 130f, 0);
                        Main.startPumpkinMoon();
                        NetMessage.SendData((int)Packet.WORLD_DATA);

                        ProgramLog.Admin.Log(Lang.misc[31]);
                        ProgramLog.Admin.Log("First Wave: " + Main.npcName[305]);
                    }
                    else
                    {
                        Main.stopMoonEvent();
                        sender.Message("The pumpkin moon was disabled.");
                    }
                    break;
                case "bloodmoon":
                    if (!Main.bloodMoon)
                    {
                        _disableActiveEvents(sender);
                        World.SetTime(0, false);
                        //OTA.Callbacks.MainCallback.StartEclipse = true;
                        Main.bloodMoon = true;
                        NetMessage.SendData((int)Packet.WORLD_DATA);
                        NetMessage.SendData(25, -1, -1, Lang.misc[8], 255, 50f, 255f, 130f, 0);

                        ProgramLog.Admin.Log(Lang.misc[8]);
                    }
                    else
                    {
                        Main.bloodMoon = false;
                        sender.Message("The blood moon was disabled.");
                    }
                    break;
                case "slimerain":
                    if (!Main.slimeRain)
                    {
                        _disableActiveEvents(sender);
                        Main.slimeRain = true;
                        NetMessage.SendData((int)Packet.WORLD_DATA);
                        Main.StartSlimeRain();

                        sender.Message("Slime rain was enabled.");
                    }
                    else
                    {
                        Main.slimeRain = false;
                        NetMessage.SendData((int)Packet.WORLD_DATA);
                        sender.Message("The slime rain was disabled.");
                    }
                    break;
                case "rain":
                    if (!Main.raining)
                    {
                        _disableActiveEvents(sender);
                        //                        Main.raining = true;
                        //                        Main.rainTime = 3600;
                        //                        NetMessage.SendData((int)Packet.WORLD_DATA);
                        Main.StartRain();

                        sender.Message("Rain was enabled.");
                    }
                    else
                    {
                        //                        Main.raining = false;
                        //                        NetMessage.SendData((int)Packet.WORLD_DATA);
                        Main.StopRain();
                        sender.Message("The rain was disabled.");
                    }
                    break;
                default:
                    throw new CommandError("Not a supported event " + first);
            }
        }

        //TODO clean code, only have command methods in this file; everything else in entry perhaps.
        static void _disableActiveEvents(ISender sender)
        {
            if (Main.bloodMoon)
            {
                Main.bloodMoon = false;
                sender.Message("The blood moon was disabled.");
            }
            if (Main.eclipse)
            {
                Main.eclipse = false;
                sender.Message("The eclipse was disabled.");
            }
            if (Main.snowMoon)
            {
                Main.snowMoon = false;
                sender.Message("The snow moon was disabled.");
            }
            if (Main.pumpkinMoon)
            {
                Main.pumpkinMoon = false;
                sender.Message("The pumpkin moon was disabled.");
            }
            if (Main.slimeRain)
            {
                //Main.StopSlimeRain();
                Main.slimeRain = false;
                sender.Message("The slime rain was disabled.");
            }
            if (Main.raining)
            {
                //Main.StopRain();
                Main.raining = false;
                sender.Message("The rain was disabled.");
            }
        }
    }
}

