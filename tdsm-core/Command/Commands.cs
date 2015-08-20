using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TDSM.API;
using TDSM.API.Command;
using TDSM.API.Misc;
using TDSM.Core.Definitions;
using TDSM.API.Logging;

//using TDSM.Core.Messages.Out;
//using TDSM.Core.ServerCore;
using Terraria;
using TDSM.API.Sockets;
using TDSM.API.Data;

namespace TDSM.Core
{
    public partial class Entry
    {
        private static object GetDataValue(Type dataType, string val)
        {
            switch (dataType.Name)
            {
                case "Boolean":
                    return Boolean.Parse(val);
                case "Int16":
                    return Int16.Parse(val);
                case "Int32":
                    return Int32.Parse(val);
                case "Int64":
                    return Int64.Parse(val);
                case "Byte":
                    return Byte.Parse(val);
                case "Double":
                    return Double.Parse(val);
                case "Single":
                    return Single.Parse(val);
                default:
                    throw new CommandError("Unsupported datatype");
            }
        }

        /// <summary>
        /// Allows on the fly variable modifications
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="args">Arguments.</param>
        public void VariableMan(ISender sender, ArgumentList args)
        {
            // var <exec|field>
            // var field <namespace.type> <fieldname>
            // var field <namespace.type> <fieldname> <valuetobeset>

            // var exec <namespace.type> <methodname>
            //No arguments supported yet
            var cmd = args.GetString(0);

            if (cmd == "field")
            {
                var type = args.GetString(1);
                var mem = args.GetString(2);

                //Find the type
                var at = Type.GetType(type);
                if (at == null)
                    at = System.Reflection.Assembly.GetEntryAssembly().GetType(type);
                if (at == null)
                    at = System.Reflection.Assembly.GetCallingAssembly().GetType(type);
                if (at == null)
                    at = System.Reflection.Assembly.GetExecutingAssembly().GetType(type);
                if (at == null)
                    at = typeof(Terraria.Main).Assembly.GetType(type);
                if (at == null)
                    throw new CommandError("Invalid type: " + type);

                //Find the field
                var am = at.GetField(mem);
                if (am == null)
                    throw new CommandError("Invalid field: " + mem);

                string val = null;
                if (args.TryGetString(3, out val))
                {
                    object data = GetDataValue(am.FieldType, val);
                    am.SetValue(null, data);

                    var v = am.GetValue(null);
                    if (v != null)
                        val = v.ToString();
                    else
                        val = "null";
                    sender.Message("Value is now: " + val);
                }
                else
                {
                    var v = am.GetValue(null);
                    if (v != null)
                        val = v.ToString();
                    else
                        val = "null";
                    sender.Message("Value: " + val);
                }
            }
            else if (cmd == "prop")
            {
                var type = args.GetString(1);
                var prop = args.GetString(2);

                //Find the type
                var at = Type.GetType(type);
                if (at == null)
                    at = System.Reflection.Assembly.GetEntryAssembly().GetType(type);
                if (at == null)
                    at = System.Reflection.Assembly.GetCallingAssembly().GetType(type);
                if (at == null)
                    at = System.Reflection.Assembly.GetExecutingAssembly().GetType(type);
                if (at == null)
                    throw new CommandError("Invalid type: " + type);

                //Find the field
                var am = at.GetProperty(prop);
                if (am == null)
                    throw new CommandError("Invalid property: " + prop);

                string val = null;
                if (args.TryGetString(3, out val))
                {
                    object data = GetDataValue(am.PropertyType, val);
                    am.SetValue(null, data, null);

                    var v = am.GetValue(null, null);
                    if (v != null)
                        val = v.ToString();
                    else
                        val = "null";
                    sender.Message("Value is now: " + val);
                }
                else
                {
                    var v = am.GetValue(null, null);
                    if (v != null)
                        val = v.ToString();
                    else
                        val = "null";
                    sender.Message("Value: " + val);
                }
            }
            else if (cmd == "exec")
            {
                var type = args.GetString(1);
                var mthd = args.GetString(2);

                //Find the type
                var at = Type.GetType(type);
                if (at == null)
                    at = System.Reflection.Assembly.GetEntryAssembly().GetType(type);
                if (at == null)
                    at = System.Reflection.Assembly.GetCallingAssembly().GetType(type);
                if (at == null)
                    at = System.Reflection.Assembly.GetExecutingAssembly().GetType(type);
                if (at == null)
                    throw new CommandError("Invalid type: " + type);

                //Find the field
                var am = at.GetMethod(mthd);
                if (am == null)
                    throw new CommandError("Invalid method: " + mthd);

                var prms = am.GetParameters();
                if (prms.Length == 0)
                {
                    var res = am.Invoke(null, null);
                    var result = res == null ? "null" : res.ToString();
                    sender.Message("Result: " + result);
                }
                else
                    sender.Message("Arguments are not yet supported for exec");
            }
            else
                sender.Message("Unsupported var command: " + cmd);
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

        /// <summary>
        /// Closes the Server all connections.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void Exit(ISender sender, ArgumentList args)
        {
            var accessLevel = ExitAccessLevel;
            if (accessLevel == -1 && sender is Player)
            {
                sender.Message("You cannot perform that action.", 255, 238, 130, 238);
                return;
            }
            else if (!CommandParser.CheckAccessLevel((AccessLevel)accessLevel, sender))
            {
                sender.Message("You cannot perform that action.", 255, 238, 130, 238);
                return;
            }

            string message;
            args.TryGetString(0, out message);

            if (String.IsNullOrEmpty(message))
                message = "Server is going down";

//            args.ParseNone();

            Tools.NotifyAllOps("Exiting on request.");

            if (Netplay.anyClients)
            {
                for (var x = 0; x < Main.player.Length; x++)
                {
                    if (Main.player[x].active)
                    {
                        NetMessage.SendData((int)Packet.DISCONNECT, x, -1, message);

                        var rc = Netplay.Clients[x];
                        if (rc != null && rc.Socket != null && rc.Socket is ClientConnection)
                        {
                            (rc.Socket as ClientConnection).Flush();
                        }
                    }
                }

                //Prevent further connections
                Terraria.Netplay.Connection.Socket.StopListening();

                //Wait for total disconnection
                while (Netplay.anyClients)
                {
                    System.Threading.Thread.Sleep(100);
                }
            }

            Terraria.IO.WorldFile.saveWorld(false);
            Terraria.Netplay.disconnect = true;

            throw new ExitException(sender.SenderName + " requested that TDSM is to shutdown.");
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
            //			var sb = new System.Text.StringBuilder ();
            //			for (int i = 0; i < 255; i++)
            //			{
            //				var p = Networking.Connection.packetsPerMessage [i];
            //				var b = Networking.Connection.bytesPerMessage [i];
            //				if (p > 0)
            //					sb.AppendFormat ("{0}({1}p, {2}B), ", (Packet)i, p, b);
            //			}
            //			
            //			sender.Message (sb.ToString());
            //#endif
        }

        ///// <summary>
        ///// Reloads Plugins.
        ///// </summary>
        ///// <param name="sender">Sending player</param>
        ///// <param name="args">Arguments sent with command</param>
        //public static void Reload(ISender sender, ArgumentList args)
        //{

        //}

        /// <summary>
        /// Prints a Playerlist.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void OldList(ISender sender, ArgumentList args)
        {
            args.ParseNone();

            var online = String.Empty;
            if (sender is ConsoleSender)
            {
                var players = from p in Main.player
                                          where p.active
                                          select String.Format("{0} ({1})", p.Name, p.IPAddress);

                online = String.Join(", ", players);
            }
            else
            {
                var players = from p in Main.player
                                          where p.active
                                          select p.Name;

                online = String.Join(", ", players);
            }

            if (String.IsNullOrEmpty(online))
                sender.Message("No players online.", 255, 255, 240, 20);
            else
                sender.Message("Current players: " + online, 255, 255, 240, 20);
        }

        /// <summary>
        /// Prints a player list, Possibly readable by bots.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void List(ISender sender, ArgumentList args)
        {
            args.ParseNone();

            var players = from p in Terraria.Main.player
                                   where p.active && !p.Op
                                   select p.Name;
            var ops = from p in Terraria.Main.player
                               where p.active && p.Op
                               select p.Name;

            var pn = players.Count();
            var on = ops.Count();

            if (on + pn == 0)
            {
                sender.Message("No players online.");
                return;
            }

            string ps = String.Empty;
            string os = String.Empty;

            if (pn > 0)
                ps = (on > 0 ? " | Players: " : "Players: ") + String.Join(", ", players);

            if (on > 0)
                os = "Ops: " + String.Join(", ", ops);

            //sender.Message(string.Concat(os, ps, " (", on + pn, "/", SlotManager.MaxSlots, ")"), 255, 255, 240, 20);
            sender.Message(string.Concat(os, ps, " (", on + pn, "/", Netplay.MaxConnections, ")"), 255, 255, 240, 20);
        }

        /// <summary>
        /// 3rd person talking.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="message">Message to send</param>
        public void Action(ISender sender, string message)
        {
            if (!String.IsNullOrEmpty(message))
            {
                ProgramLog.Log("* " + sender.SenderName + " " + message);
                if (sender is Player)
                    NetMessage.SendData(25, -1, -1, "* " + sender.SenderName + " " + message, 255, 200, 100, 0);
                else
                    NetMessage.SendData(25, -1, -1, "* " + sender.SenderName + " " + message, 255, 238, 130, 238);
            }
            else
            {
                sender.SendMessage("Expected message");
            }
        }

        /// <summary>
        /// Sends a Server Message to all online Players.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="message">Message to send</param>
        public void Say(ISender sender, string message)
        {
            /*ProgramLog.Log("<" + sender.SenderName + "> " + message);
            if (sender is Player)
                NewNetMessage.SendData(25, -1, -1, "<" + sender.SenderName + "> " + message, 255, 255, 255, 255);
            else
                NewNetMessage.SendData(25, -1, -1, "<" + sender.SenderName + "> " + message, 255, 238, 180, 238);*/

            // 'Say' should be used for Server Messages, OP's only. This is used on many large servers to notify
            // Users for a quick restart (example), So the OP will most likely be in game, unless it's major.

            if (!String.IsNullOrEmpty(message))
            {
                ProgramLog.Log("<" + sender.SenderName + "> " + ((sender is ConsoleSender) ? String.Empty : "SERVER: ") + message);
                NetMessage.SendData(25, -1, -1, "SERVER: " + message, 255, 238, 130, 238);
            }
            else
            {
                sender.SendMessage("Expected message");
            }
        }

        /// <summary>
        /// Heals one or all players.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="message">Message to send</param>
        public void Heal(ISender sender, ArgumentList args)
        {
            Player subject;

            if (args.TryPopOne(out subject))
            {
                subject = sender as Player;
                if (subject == null)
                {
                    sender.Message("Need a heal target");
                    return;
                }

                NetMessage.SendData((int)Packet.HEAL_PLAYER, -1, -1, String.Empty, subject.whoAmI, (float)subject.statLifeMax);
                subject.Message("You have been healed!", Color.Green);
            }
            else if (args.TryPop("-all"))
            {
                foreach (var plr in Main.player)
                {
                    if (plr.active)
                    {
                        NetMessage.SendData((int)Packet.HEAL_PLAYER, -1, -1, String.Empty, plr.whoAmI, (float)plr.statLifeMax);
                        plr.Message("You have been healed!", Color.Green);
                    }
                }
            }
            else if (sender is Player)
            {
                var plr = sender as Player;
                NetMessage.SendData((int)Packet.HEAL_PLAYER, -1, -1, String.Empty, plr.whoAmI, (float)plr.statLifeMax);
                plr.Message("You have been healed!", Color.Green);
            }
            else throw new CommandError("Nobody specified to heal");
        }

        /// <summary>
        /// Executes the world data save routine.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void SaveAll(ISender sender, ArgumentList args)
        {
            Tools.NotifyAllOps("Saving world.....");

            Terraria.IO.WorldFile.saveWorld();

            while (WorldGen.saveLock)
                Thread.Sleep(100);

//            Tools.NotifyAllOps("Saving data...", true);

            //Server.BanList.Save();
            //Server.WhiteList.Save();
            //Server.OpList.Save();

            Tools.NotifyAllOps("Saving Complete.", true);
        }

        /////// <summary>
        /////// Adds or removes specified player to/from the white list.
        /////// </summary>
        /////// <param name="sender">Sending player</param>
        /////// <param name="args">Arguments sent with command</param>
        ////public static void WhiteList(ISender sender, ArgumentList args)
        ////{
        ////    // /whitelist <add:remove> <player>
        ////    string Exception, Type = String.Empty;
        ////    if (args.TryParseOne<String>(Languages.Add, out Exception))
        ////    {
        ////        Server.WhiteList.addException(Exception);
        ////        Type = Languages.Added;
        ////    }
        ////    else if (args.TryParseOne<String>(Languages.Remove, out Exception))
        ////    {
        ////        Server.WhiteList.removeException(Exception);
        ////        Type = Languages.RemovedFrom;
        ////    }
        ////    else
        ////    {
        ////        sender.Message(Languages.PleaseReview);
        ////        return;
        ////    }

        ////    Tools.NotifyAllOps(Exception + " was " + Type + " the Whitelist [" + sender.SenderName + "]", true);

        ////    if (!Server.WhiteList.Save())
        ////    {
        ////        Tools.NotifyAllOps(Languages.WhilelistFailedSave + sender.SenderName + "'s " + Languages.Command, true);
        ////    }
        ////}

        void WhitelistMan(ISender sender, ArgumentList args)
        {
            var index = 0;
            var cmd = args.GetString(index++);
            string name, ip;

            switch (cmd)
            {
                case "status":
                case "current":
                case "?":
                    sender.Message("The whitelist is currently " + (WhitelistEnabled ? "enabled" : "disabled"));
                    break;
                case "reload":
                    Whitelist.Load();
                    Tools.NotifyAllOps("The whitelist was reloaded");
                    if (!sender.Op) sender.Message("The whitelist was reloaded", Color.Green);
                    break;
                case "enable":
                    if (!WhitelistEnabled)
                    {
                        WhitelistEnabled = true;

                        if (!ConfigUpdater.IsAvailable || ConfigUpdater.Set("usewhitelist", WhitelistEnabled))
                        {
                            Tools.NotifyAllOps("The whitelist was enabled");
                            if (!sender.Op) sender.Message("The whitelist was enabled", Color.Green);
                        }
                        else sender.Message("Failed to save to config, whitelist is only enabled this session.", Color.Red);
                    }
                    else sender.Message("The whitelist is already enabled", Color.Red);
                    break;
                case "disable":
                    if (WhitelistEnabled)
                    {
                        WhitelistEnabled = false;

                        if (!ConfigUpdater.IsAvailable || ConfigUpdater.Set("usewhitelist", WhitelistEnabled))
                        {
                            Tools.NotifyAllOps("The whitelist was disabled");
                            if (!sender.Op) sender.Message("The whitelist was disabled", Color.Green);
                        }
                        else sender.Message("Failed to save to config, whitelist is only disabled this session.", Color.Red);
                    }
                    else sender.Message("The whitelist is already disabled", Color.Red);
                    break;

                case "addplayer":
                    if (!args.TryGetString(index++, out name))
                    {
                        throw new CommandError("Expected player name after [addplayer]");
                    }

                    var addName = Prefix_WhitelistName + name;
                    if (Whitelist.Add(addName))
                    {
                        Tools.NotifyAllOps(String.Format("Player {0} was added to the whitelist", name));
                        if (!sender.Op) sender.Message(String.Format("Player {0} was added to the whitelist", name), Color.Green);

                        if (!WhitelistEnabled) sender.Message("Note, the whitelist is not enabled", Color.Orange);
                    }

                    else sender.Message("Failed to add " + name + " to the whitelist", Color.Red);
                    break;
                case "removeplayer":
                    if (!args.TryGetString(index++, out name))
                    {
                        throw new CommandError("Expected player name after [removeplayer]");
                    }

                    var removeName = Prefix_WhitelistName + name;
                    if (Whitelist.Remove(removeName))
                    {
                        Tools.NotifyAllOps(String.Format("Player {0} was removed from the whitelist", name));
                        if (!sender.Op) sender.Message(String.Format("Player {0} was removed from the whitelist", name), Color.Green);

                        if (!WhitelistEnabled) sender.Message("Note, the whitelist is not enabled", Color.Orange);
                    }
                    else sender.Message("Failed to remove " + name + " from the whitelist", Color.Red);
                    break;

                case "addip":
                    if (!args.TryGetString(index++, out ip))
                    {
                        throw new CommandError("Expected IP after [addip]");
                    }

                    var addIP = Prefix_WhitelistIp + ip;
                    if (Whitelist.Add(addIP))
                    {
                        Tools.NotifyAllOps(String.Format("IP {0} was added to the whitelist", ip));
                        if (!sender.Op) sender.Message(String.Format("IP {0} was added to the whitelist", ip), Color.Green);

                        if (!WhitelistEnabled) sender.Message("Note, the whitelist is not enabled", Color.Orange);
                    }
                    else sender.Message("Failed to add " + ip + " to the whitelist", Color.Red);
                    break;
                case "removeip":
                    if (!args.TryGetString(index++, out ip))
                    {
                        throw new CommandError("Expected IP after [removeip]");
                    }

                    var removeIP = Prefix_WhitelistIp + ip;
                    if (Whitelist.Remove(removeIP))
                    {
                        Tools.NotifyAllOps(String.Format("IP {0} was removed from the whitelist", ip));
                        if (!sender.Op) sender.Message(String.Format("IP {0} was removed from the whitelist", ip), Color.Green);

                        if (!WhitelistEnabled) sender.Message("Note, the whitelist is not enabled", Color.Orange);
                    }
                    else sender.Message("Failed to remove " + ip + " from the whitelist", Color.Red);
                    break;

                default:
                    throw new CommandError("Unknown whitelist command: " + cmd);
            }
        }

        ///// <summary>
        ///// Adds a player or ip (Exception) to the ban list.
        ///// </summary>
        ///// <param name="sender">Sending player</param>
        ///// <param name="args">Arguments sent with command</param>
        //public static void Ban(ISender sender, ArgumentList args)
        //{
        //    Player banee;
        //    string playerName = null;

        //    if (args.TryGetOnlinePlayer(0, out banee))
        //    {
        //        playerName = banee.Name;
        //        banee.Kick(Languages.Ban_You);
        //        Server.BanList.addException(Netplay.slots[banee.whoAmi].
        //                remoteAddress.Split(':')[0]);
        //    }
        //    else if (!args.TryGetString(0, out playerName))
        //    {
        //        throw new CommandError(Languages.IPExpected);
        //    }

        //    Server.BanList.addException(playerName);

        //    Tools.NotifyAllOps(playerName + Languages.Ban_Banned + " [" + sender.SenderName + "]", true);
        //    if (!Server.BanList.Save())
        //    {
        //        Tools.NotifyAllOps(Languages.Ban_FailedToSave + sender.SenderName + "'s " + Languages.Command, true);
        //    }
        //}

        ///// <summary>
        ///// Removes an exception from the ban list.
        ///// </summary>
        ///// <param name="sender">Sending player</param>
        ///// <param name="args">Arguments sent with command</param>
        //public static void UnBan(ISender sender, ArgumentList args)
        //{
        //    string playerName;
        //    if (!args.TryGetString(0, out playerName))
        //    {
        //        throw new CommandError(Languages.IPExpected);
        //    }

        //    Server.BanList.removeException(playerName);

        //    Tools.NotifyAllOps(playerName + Languages.Ban_UnBanned + " [" + sender.SenderName + "]", true);

        //    if (!Server.BanList.Save())
        //    {
        //        Tools.NotifyAllOps(Languages.Ban_FailedToSave + sender.SenderName + "'s " + Languages.Command, true);
        //    }
        //}

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

        /// <summary>
        /// Gives specified item to the specified player.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void Give(ISender sender, ArgumentList args)
        {
            // /give <stack> <item> [prefix] [player]
            var index = 0;
            int stack = args.GetInt(index++);
            string name = args.GetString(index++);
 
//            var max = Tools.AvailableItemSlots; //Perhaps remove a few incase of new drops
//            if (stack > max)
//            {
//                stack = max; // Set to Tools.AvailableItemSlots because number given was larger than this.
//            }
            int id;
            var results = Int32.TryParse(name, out id) ? DefinitionManager.FindItem(id) : DefinitionManager.FindItem(name);
            if (results != null && results.Length > 0)
            {
                if (results.Length > 1)
                    throw new CommandError(String.Format("More than 1 item found, total is: {0}", results.Length));

                var item = results[0];
                string prefix;
                if (args.TryGetString(index, out prefix))
                {
                    try
                    {
                        Affix afx;
                        if (Enum.TryParse(prefix, out afx))
                        {
                            item.Prefix = (int)afx;
                            index++;
                        }
                    }
                    catch (ArgumentException)
                    {
                        throw new CommandError(String.Format("Error, the Prefix you entered was not found: {0}", args.GetString(3)));
                    }
                }

                Player receiver;
                if (!args.TryGetOnlinePlayer(index, out receiver))
                {
                    if (sender is Player)
                        receiver = sender as Player;
                    else throw new CommandError("Expected an online player");
                }

                receiver.GiveItem(item.Id, stack, item.MaxStack, sender, item.NetId, true, item.Prefix);
            }
            else
                throw new CommandError(String.Format("No item known by: {0}", name));
        }

        /// <summary>
        /// Spawns specified NPC type.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        /// <remarks>This function also allows NPC custom health.</remarks>
        public void SpawnNPC(ISender sender, ArgumentList args)
        {
            //if (Main.stopSpawns && !Program.properties.NPCSpawnsOverride)
            //    throw new CommandError("NPC Spawing is disabled.");

            //var health = -1;
            //var customHealth = args.TryPopAny<Int32>("-health", out health);

            Player player = sender as Player;
            int amount;
            if (args.Count > 4)
                throw new CommandError("Too many arguments");
            else if (sender is ConsoleSender && args.Count <= 2)
            {
                if (!Netplay.anyClients || !Tools.TryGetFirstOnlinePlayer(out player))
                    throw new CommandError("No players online.");
            }
            else if (args.Count >= 3)
                player = args.GetOnlinePlayer(2);

            var npcName = args.GetString(1).ToLower().Trim();

            // Get the class id of the npc
            var npcs = DefinitionManager.FindNPC(npcName);
            if (npcs.Length == 0)
                throw new CommandError("No npc exists by the name {0}", npcName);
            else if (npcs.Length > 1)
            {
                bool first;
                args.TryGetBool(3, out first);

                if (!first)
                    throw new CommandError("Too many results for {0}, total count {1}", npcName, npcs.Length);
            }

            var npc = npcs[0];
            if (npc.Boss.HasValue && npc.Boss == true)
                throw new CommandError("This NPC can only be summoned by the SPAWNBOSS command.");
            try
            {
                amount = args.GetInt(0);
                //if (NPCAmount > Program.properties.SpawnNPCMax && sender is Player)
                //{
                //    (sender as Player).Kick("Don't spawn that many.");
                //    return;
                //}
            }
            catch
            {
                throw new CommandError("Expected amount to spawn");
            }

            var max = Tools.AvailableNPCSlots; //Perhaps remove a few incase of spawns
            if (amount > max)
                throw new CommandError("Cannot spawn that many, available slots: {0}", max);

            string realNPCName = String.Empty;
            for (int i = 0; i < amount; i++)
            {
                Vector2 location = World.GetRandomClearTile(((int)player.position.X / 16), ((int)player.position.Y / 16), 100, 100, 50);
                int npcIndex = NPC.NewNPC(((int)location.X * 16), ((int)location.Y * 16), npc.Id, 0);

                //if (customHealth)
                //{
                //    Main.npc[npcIndex].life = health;
                //    Main.npc[npcIndex].lifeMax = health;
                //}
                Main.npc[npcIndex].netDefaults(npc.NetId);

                realNPCName = Main.npc[npcIndex].name;
            }
            Tools.NotifyAllOps("Spawned " + amount.ToString() + " of " +
                realNPCName + " [" + player.Name + "]", true);
        }

        /// <summary>
        /// Teleports player1 to a second specified player's location.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void Teleport(ISender sender, ArgumentList args)
        {
            Player subject;
            Player target;

            if (!args.TryPopOne(out subject))
            {
                subject = sender as Player;
                if (subject == null)
                {
                    sender.Message("Need a teleport target");
                    return;
                }

                if (args.Count == 0)
                {
                    if (subject.SpawnX > -1)
                    {
                        subject.Teleport(subject.SpawnX * 16f, subject.SpawnY * 16f - subject.height);
                        Tools.NotifyAllOps(String.Format("{0} has teleported home", subject.Name), true);
                    }
                    else
                    {
                        subject.Teleport(Main.spawnTileX * 16f, Main.spawnTileY * 16f - subject.height);
                        Tools.NotifyAllOps(String.Format("{0} has teleported to spawn", subject.Name), true);
                    }
                    return;
                }
            }
            else if (args.Count == 0)
            {
                target = subject;

                subject = sender as Player;
                if (subject == null)
                {
                    sender.Message("Need a teleport target");
                    return;
                }

                /*if (*/
                subject.Teleport(target); //)
                {
                    Tools.NotifyAllOps(String.Concat("Teleported ", subject.Name, " to ",
                            target.Name, ". [", sender.SenderName, "]"), true);
                }
                //else
                //    sender.Message(Languages.TeleportFailed);
                return;
            }

            int x;
            int y;

            if (args.Count == 1)
            {
                if (args.TryParseOne(out target))
                {
                    /*if (*/
                    subject.Teleport(target); //)
                    {
                        Tools.NotifyAllOps(string.Concat("Teleported ", subject.Name, " to ",
                                target.Name, ". [", sender.SenderName, "]"), true);
                    }
                    //else
                    //    sender.Message(Languages.TeleportFailed);
                }
                else
                    sender.Message("Cannot find player");
                return;
            }
            else if (args.Count == 2)
            {
                if (args.TryParseTwo(out x, out y))
                {
                    if (x < 0 || x >= Main.maxTilesX || y < 0 || y >= Main.maxTilesY)
                    {
                        sender.Message(String.Format("Coordinates out of range of (0, {0}); (0, {1}).", Main.maxTilesX - 1, Main.maxTilesY - 1));
                        return;
                    }
                    const Int32 OutOfBoundsPadding = 0; // 41;

                    /*if (*/
                    subject.Teleport((x - OutOfBoundsPadding) * 16f, (y - OutOfBoundsPadding) * 16f); //)
                    {
                        Tools.NotifyAllOps(string.Concat("Teleported ", subject.Name, " to ",
                                x, ":", y, ". [", sender.SenderName, "]"), true);
                    }
                    //else
                    //    sender.Message(Languages.TeleportFailed);
                }
                else
                    throw new CommandError("Invalid coordinates");
                return;
            }

            throw new CommandError(String.Empty);
        }

        /// <summary>
        /// Teleports specified player to sending player's location.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void TeleportHere(ISender sender, ArgumentList args)
        {
            if (sender is Player)
            {
                Player player = ((Player)sender);
                Player subject;

                if (args.TryPopOne(out subject))
                {
                    if (subject == null)
                    {
                        sender.Message("Cannot find player");
                        return;
                    }

                    subject.Teleport(player);

                    Tools.NotifyAllOps("Teleported " + subject.Name + " to " +
                        player.Name + " [" + sender.SenderName + "]", true);
                }
            }
            else
            {
                throw new CommandError("This command is for players only");
            }
        }

        ///// <summary>
        ///// Sets OP status to a given Player.
        ///// </summary>
        ///// <param name="sender">Sending player</param>
        ///// <param name="args">Arguments sent with command</param>
        //public static void OpPlayer(ISender sender, ArgumentList args)
        //{
        //    var playerName = args.GetString(0);
        //    var password = args.GetString(1);
        //    Player player;
        //    if (args.TryGetOnlinePlayer(0, out player))
        //    {
        //        playerName = player.Name;

        //        player.sendMessage(Languages.YouAreNowOP, Color.Green);
        //        player.Op = true;
        //        if (player.HasClientMod)
        //        {
        //            NewNetMessage.SendData(Packet.CLIENT_MOD, player.whoAmi);
        //        }
        //    }

        //    Tools.NotifyAllOps("Opping " + playerName + " [" + sender.SenderName + "]", true);
        //    Server.OpList.addException(playerName + ":" + password, true, playerName.Length + 1);

        //    if (!Server.OpList.Save())
        //    {
        //        Tools.NotifyAllOps(Languages.OPlistFailedSave + " [" + sender.SenderName + "]", true);
        //        return;
        //    }
        //}

        ///// <summary>
        ///// De-OPs a given Player.
        ///// </summary>
        ///// <param name="sender">Sending player</param>
        ///// <param name="args">Arguments sent with command</param>
        //public static void DeopPlayer(ISender sender, ArgumentList args)
        //{
        //    var playerName = args.GetString(0);
        //    Player player;
        //    if (args.TryGetOnlinePlayer(0, out player))
        //    {
        //        playerName = player.Name;

        //        if (Player.isInOpList(playerName))
        //        {
        //            player.sendMessage(Languages.YouAreNowDeop, Color.Green);
        //        }

        //        player.Op = false;
        //        if (player.HasClientMod)
        //        {
        //            NewNetMessage.SendData(Packet.CLIENT_MOD, player.whoAmi);
        //        }
        //    }

        //    if (Player.isInOpList(playerName))
        //    {
        //        Tools.NotifyAllOps("De-Opping " + playerName + " [" + sender.SenderName + "]", true);
        //        Server.OpList.removeException(playerName + ":" + Player.GetPlayerPassword(playerName));
        //    }

        //    if (!Server.OpList.Save())
        //    {
        //        Tools.NotifyAllOps(Languages.OPlistFailedSave + " [" + sender.SenderName + "]", true);
        //        return;
        //    }
        //}

        ///// <summary>
        ///// Allows Operators to login.
        ///// </summary>
        ///// <param name="sender">Sending player</param>
        ///// <param name="args">Arguments sent with command</param>
        //public static void OpLogin(ISender sender, ArgumentList args)
        //{
        //    if (sender is Player)
        //    {
        //        Player player = sender as Player;
        //        string Password = String.Join(" ", args).Trim();
        //        if (player.isInOpList())
        //        {
        //            if (player.Password.Equals(Password))
        //            {
        //                Tools.NotifyAllOps(
        //                    String.Format("{0} " + Languages.SuccessfullyLoggedInOP, player.Name)
        //                );
        //                player.Op = true;
        //                player.sendMessage(Languages.SuccessfullyLoggedInOP, Color.DarkGreen);

        //                if (player.HasClientMod)
        //                {
        //                    NewNetMessage.SendData(Packet.CLIENT_MOD, player.whoAmi);
        //                }
        //            }
        //            else
        //            {
        //                Tools.NotifyAllOps(
        //                    String.Format("{0} " + Languages.FailedLoginWrongPassword, player.Name)
        //                );
        //                player.sendMessage(Languages.IncorrectOPPassword, Color.DarkRed);
        //            }
        //        }
        //        else
        //        {
        //            player.sendMessage(Languages.YouNeedPrivileges, Color.DarkRed);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Allows Operators to logout.
        ///// </summary>
        ///// <param name="sender">Sending player</param>
        ///// <param name="args">Arguments sent with command</param>
        //public static void OpLogout(ISender sender, ArgumentList args)
        //{
        //    if (sender is Player)
        //    {
        //        var player = sender as Player;
        //        if (sender.Op)
        //        {
        //            player.Op = false;
        //            player.sendMessage(Languages.SuccessfullyLoggedOutOP, Color.DarkRed);

        //            Tools.NotifyAllOps(
        //                String.Format("{0} " + Languages.SuccessfullyLoggedOutOP, player.Name)
        //            );

        //            if (player.HasClientMod)
        //            {
        //                NewNetMessage.SendData(Packet.CLIENT_MOD, player.whoAmi);
        //            }
        //        }
        //        else
        //        {
        //            player.sendMessage(Languages.YouNeedPrivileges, Color.DarkRed);
        //        }
        //    }
        //}

        /// <summary>
        /// Enables or disables NPC spawning
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void NPCSpawning(ISender sender, ArgumentList args)
        {
            args.ParseNone();

            StopNPCSpawning = !StopNPCSpawning;
            sender.Message("NPC spawning is now " + (StopNPCSpawning ? "off" : "on") + "!");

            //Program.properties.StopNPCSpawning = Terraria.Main.stopSpawns;
            //Program.properties.Save(false);

            //if (args.TryPop("-clear"))
            //    Purge(sender, new ArgumentList() { "all" });
        }

        /// <summary>
        /// Fast forwards time
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void FastForwardTime(ISender sender, ArgumentList args)
        {
            args.ParseNone();

            TimeFastForwarding = !TimeFastForwarding;
            sender.Message("Time is now " + (TimeFastForwarding ? "fast" : "normal") + "!");
        }

        /// <summary>
        /// Kicks a given Player from the server
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void Kick(ISender sender, ArgumentList args)
        {
            if (args.TryPop("-s"))
            {
                int s;
                args.ParseOne(out s);

                var slot = Terraria.Netplay.Clients[s];

#if TDSMServer
                if (slot.State() != SlotState.VACANT)
                {
                    slot.Kick("You have been kicked by " + sender.SenderName + ".");

                    var player = Main.player[s];
                    if (player != null && player.Name != null)
                        NewNetMessage.SendData(25, -1, -1, player.Name + " has been kicked by " + sender.SenderName + ".", 255);
                }
                else
                {
                    sender.Message("Kick slot is empty");
                }
#else
                NetMessage.SendData(2, slot.Id, -1, "Kicked from server.", 0, 0f, 0f, 0f, 0, 0, 0);
#endif
            }
            else
            {
                Player player;
                args.ParseOne<Player>(out player);

                if (player.Name == null)
                {
                    sender.Message("Kick player name is not set.");
                    return;
                }

                player.Kick("You have been kicked by " + sender.SenderName + ".");
                NetMessage.SendData(25, -1, -1, player.Name + " has been kicked by " + sender.SenderName + ".", 255);
            }
        }

        ///// <summary>
        ///// Restarts the server
        ///// </summary>
        ///// <param name="sender">Sending player</param>
        ///// <param name="args">Arguments sent with command</param>
        //public static void Restart(ISender sender, ArgumentList args)
        //{
        //    Program.Restarting = true;
        //    Tools.NotifyAllOps(Languages.RestartingServer + " [" + sender.SenderName + "]", true);
        //    //Statics.keepRunning = true;

        //    Netplay.StopServer();
        //    while (Netplay.ServerUp) { Thread.Sleep(10); }

        //    Statics.WorldLoaded = false;

        //    ProgramLog.Log(Languages.StartingServer);
        //    Main.Initialize();

        //    Program.LoadPlugins();

        //    WorldIO.LoadWorld(null, null, World.SavePath);

        //    Program.updateThread = new ProgramThread("Updt", Program.UpdateLoop);
        //    Netplay.StartServer();

        //    while (!Netplay.ServerUp) { Thread.Sleep(100); }

        //    HookContext ctx = new HookContext
        //    {
        //        Sender = new ConsoleSender(),
        //    };

        //    HookArgs.ServerStateChange eArgs = new HookArgs.ServerStateChange
        //    {
        //        ServerChangeState = ServerState.LOADED
        //    };

        //    HookPoints.ServerStateChange.Invoke(ref ctx, ref eArgs);

        //    ProgramLog.Console.Print(Languages.Startup_YouCanNowInsertCommands);
        //    Program.Restarting = false;
        //}

        ///// <summary>
        ///// Checks the state of a slot.
        ///// </summary>
        ///// <param name="sender">Sending player</param>
        ///// <param name="args">Arguments sent with command</param>
        //public static void Slots(ISender sender, ArgumentList args)
        //{
        //    bool dinfo = args.Contains("-d") || args.Contains("-dp") || args.Contains("-pd");
        //    bool pinfo = args.Contains("-p") || args.Contains("-dp") || args.Contains("-pd");

        //    int k = 0;
        //    for (int i = 0; i < 255; i++)
        //    {
        //        var slot = Netplay.slots[i];
        //        var player = Main.player[i];

        //        if (slot.state != SlotState.VACANT)
        //        {
        //            k += 1;

        //            var name = String.Empty;
        //            if (player != null)
        //            {
        //                name = string.Concat(", ", player.Op ? "Op. " : String.Empty, "\String.Empty, (player.Name ?? "<null>"), "\String.Empty);
        //                if (player.AuthenticatedAs != null)
        //                {
        //                    if (player.Name == player.AuthenticatedAs)
        //                        name = name + " (auth'd)";
        //                    else
        //                        name = name + " (auth'd as " + player.AuthenticatedAs + ")";
        //                }
        //            }

        //            var addr = "<secret>";
        //            if (!(sender is Player && player.Op))
        //                addr = slot.remoteAddress;

        //            var msg = String.Format("slot {4}{0}: {1}, {2}{3}", i, slot.state, addr, name, SlotManager.IsPrivileged(i) ? "*" : String.Empty);

        //            if (pinfo && player != null)
        //            {
        //                msg += String.Format(", {0}/{1}hp", player.statLife, player.statLifeMax);
        //            }

        //            if (dinfo)
        //            {
        //                msg += String.Format(", {0}{1}{2}, tx:{3:0.0}K, rx:{4:0.0}K, q:{5}, qm:{6:0.0}KB",
        //                    slot.conn.kicking ? "+" : "-", slot.conn.sending ? "+" : "-", slot.conn.receiving ? "+" : "-",
        //                    slot.conn.BytesSent / 1024.0, slot.conn.BytesReceived / 1024.0,
        //                    slot.conn.QueueLength,
        //                    slot.conn.QueueSize / 1024.0);
        //            }

        //            sender.Message(msg);
        //        }
        //    }
        //    sender.Message(String.Format("{0}/{1} slots occupied.", k, SlotManager.MaxSlots));
        //}

        ///// <summary>
        ///// Purge Server data
        ///// </summary>
        ///// <param name="sender">Sending player</param>
        ///// <param name="args">Arguments sent with command</param>
        //public static void Purge(ISender sender, ArgumentList args)
        //{
        //    var all = args.TryPop("all");
        //    var something = false;

        //    if (all || args.TryPop("proj") || args.TryPop("projectiles"))
        //    {
        //        something = true;

        //        ProgramLog.Admin.Log(Languages.PurgingProjectiles);

        //        var msg = NewNetMessage.PrepareThreadInstance();

        //        msg.PlayerChat(255, "<Server> " + Languages.PurgingProjectiles, 255, 180, 100);

        //        lock (Main.updatingProjectiles)
        //        {
        //            foreach (var projectile in Main.projectile)
        //            {
        //                projectile.active = false;
        //                projectile.type = ProjectileType.N0_UNKNOWN;

        //                msg.Projectile(projectile);
        //            }

        //            msg.Broadcast();
        //        }
        //    }

        //    if (all || args.TryPop("npc") || args.TryPop("npcs"))
        //    {
        //        something = true;

        //        ProgramLog.Admin.Log(Languages.PurgingNPC);

        //        var msg = NewNetMessage.PrepareThreadInstance();

        //        msg.PlayerChat(255, "<Server> " + Languages.PurgingNPC, 255, 180, 100);

        //        lock (Main.updatingNPCs)
        //        {
        //            foreach (var npc in Main.npc)
        //            {
        //                if (npc.active)
        //                {
        //                    npc.active = false;
        //                    npc.life = 0;
        //                    npc.netUpdate = false;
        //                    npc.name = String.Empty;

        //                    msg.NPCInfo(npc.whoAmI);
        //                }
        //            }

        //            msg.Broadcast();
        //        }
        //    }

        //    if (all || args.TryPop("item") || args.TryPop("items"))
        //    {
        //        something = true;

        //        ProgramLog.Admin.Log(Languages.PurgingItems);

        //        var msg = NewNetMessage.PrepareThreadInstance();

        //        msg.PlayerChat(255, "<Server> " + Languages.PurgingItems, 255, 180, 100);

        //        lock (TDSM.API.Callbacks.Main.updatingItems)
        //        {
        //            for (int i = 0; i < 200; i++)
        //            {
        //                var item = Main.item[i];
        //                if (item.active)
        //                {
        //                    Main.item[i] = new Item(); // this is what Main does when ignoreErrors is on *shrug*
        //                    msg.ItemInfo(i);
        //                    msg.ItemOwnerInfo(i);
        //                }
        //            }

        //            msg.Broadcast();
        //        }
        //    }

        //    if (!something)
        //        throw new CommandError(String.Empty);
        //}

        enum WorldZone
        {
            Any,
            ZoneCorrupt,
            ZoneCrimson,
            ZoneDesert,
            ZoneDungeon,
            ZoneGlowshroom,
            ZoneHoly,
            ZoneJungle,
            ZoneMeteor,
            ZonePeaceCandle,
            ZoneSnow,
            ZoneTowerNebula,
            ZoneTowerSolar,
            ZoneTowerStardust,
            ZoneTowerVortex,
            ZoneUndergroundDesert,
            ZoneWaterCandle
        }

        static Player FindPlayerWithOptions(WorldZone options)
        {
            switch (options)
            {
                case WorldZone.ZoneCorrupt:
                    return Main.player.Where(x => x.active && x.ZoneCorrupt).Random();
                case WorldZone.ZoneCrimson:
                    return Main.player.Where(x => x.active && x.ZoneCrimson).Random();
                case WorldZone.ZoneDesert:
                    return Main.player.Where(x => x.active && x.ZoneDesert).Random();
                case WorldZone.ZoneDungeon:
                    return Main.player.Where(x => x.active && x.ZoneDungeon).Random();
                case WorldZone.ZoneGlowshroom:
                    return Main.player.Where(x => x.active && x.ZoneGlowshroom).Random();
                case WorldZone.ZoneHoly:
                    return Main.player.Where(x => x.active && x.ZoneHoly).Random();
                case WorldZone.ZoneJungle:
                    return Main.player.Where(x => x.active && x.ZoneJungle).Random();
                case WorldZone.ZoneMeteor:
                    return Main.player.Where(x => x.active && x.ZoneMeteor).Random();
                case WorldZone.ZonePeaceCandle:
                    return Main.player.Where(x => x.active && x.ZonePeaceCandle).Random();
                case WorldZone.ZoneSnow:
                    return Main.player.Where(x => x.active && x.ZoneSnow).Random();
                case WorldZone.ZoneTowerNebula:
                    return Main.player.Where(x => x.active && x.ZoneTowerNebula).Random();
                case WorldZone.ZoneTowerSolar:
                    return Main.player.Where(x => x.active && x.ZoneTowerSolar).Random();
                case WorldZone.ZoneTowerStardust:
                    return Main.player.Where(x => x.active && x.ZoneTowerStardust).Random();
                case WorldZone.ZoneTowerVortex:
                    return Main.player.Where(x => x.active && x.ZoneTowerVortex).Random();
                case WorldZone.ZoneUndergroundDesert:
                    return Main.player.Where(x => x.active && x.ZoneUndergroundDesert).Random();
                case WorldZone.ZoneWaterCandle:
                    return Main.player.Where(x => x.active && x.ZoneWaterCandle).Random();
                default:
                    return Main.player.Where(x => x.active).Random();
            }
        }

        /// <summary>
        /// Summon a Boss
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void SummonBoss(ISender sender, ArgumentList args)
        {
            var count = args.GetInt(0);
            var bossName = args.GetString(1).ToLower();
            Player target;

            if (!args.TryGetOnlinePlayer(2, out target))
            {
                if (sender is Player)
                {
                    target = sender as Player;
                }
                else
                {
                    target = Main.player.Where(x => x.active).Random();
                    if (null == target)
                    {
                        throw new CommandError("No players online");
                    }
                }
            }

            int type = -1, type1 = -1;
            string name = null;

            switch (bossName)
            {
                case "wyvern":
                    type = 87;
                    break;

                case "brain":
                case "brain of cthulhu":
                    type = 266;
                    break;

//                case "crimson mimic":
//                    type = 474;
//                    break;
                case "corrupt mimic":
                    type = 473;
                    break;
//                case "hallowed mimic":
//                    type = 475;
//                    break;

                case "duke fishron":
                case "duke":
                case "fishron":
                    type = 370;
                    break;

                case "everscream":
                    World.SetTime(16200.0, false);
                    type = 344;
                    break;

                case "eye":
                case "cthulhu":
                case "eye of cthulhu":
                    World.SetTime(16200.0, false);
                    type = 4;
                    break;

                case "dutchman":
                case "flying dutchman":
                    type = 491;
                    break;

                case "golem":
                    type = 245;
                    break;

                case "goblin summoner":
                    type = 471;
                    break;

                case "king":
                case "king slime":
                    type = 50;
                    break;

                case "ice golem":
                    type = 243;
                    break;

                case "ice queen":
                    World.SetTime(16200.0, false);
                    type = 345;
                    break;

                case "lunatic":
                case "cultist":
                case "lunatic cultist":
                    type = 439;
                    break;

                case "saucer":
                case "martian saucer":
                    type = 395;
                    break;

                case "moon":
                case "moon lord":
                    type = 398;
                    break;

                case "mothron":
                    if (!Main.eclipse)
                        throw new CommandError("Mothron can only be spawned during a solar eclipse. See the worldevent command.");
                    type = 477;
                    break;

                case "wood":
                case "mourning wood":
                    World.SetTime(16200.0, false);
                    type = 325;
                    break;

                case "paladin":
                    type = 290;
                    break;

                case "captain":
                case "pirate":
                case "pirate captain":
                    World.SetTime(16200.0, false);
                    type = 216;
                    break;

                case "plantera":
                    type = 262;
                    break;

                case "pumpking":
                    World.SetTime(16200.0, false);
                    type = 327;
                    break;

                case "queen":
                case "queen bee":
                    type = 222;
                    break;

                case "santa":
                case "santa nk1":
                case "santa-nk1":
                    World.SetTime(16200.0, false);
                    type = 346;
                    break;

                case "skeletron":
                    World.SetTime(16200.0, false);
                    type = 35;
                    break;

                case "prime":
                case "skeletron prime":
                    type = 127;
                    break;

                case "nebula":
                case "nebula pillar":
                    type = 507;
                    break;
                case "solar":
                case "solar pillar":
                    type = 517;
                    break;
                case "stardust":
                case "stardust pillar":
                    type = 493;
                    break;
                case "vortex":
                case "vortex pillar":
                    type = 422;
                    break;

                case "destroyer":
                case "the destroyer":
                    World.SetTime(16200.0, false);
                    type = 134;
                    break;

                case "twins":
                case "the twins":
                    World.SetTime(16200.0, false);
                    type = 125;
                    type1 = 126;
                    break;

                case "eater":
                case "eater of worlds":
                    type = 13;
                    break;

                case "wall":
                case "flesh":
                case "wall of flesh":
                    if (Main.wof > 0 && Main.npc[Main.wof].active)
                        throw new CommandError("The Wall Of Flesh is already active");

                    if (target.position.Y / 16 < (float)(Main.maxTilesY - 205)) //As per NPC.SpawnWOF
                        throw new CommandError("Player must be in The Underworld to spawn the Eater Of Worlds");

                    type = 113;
                    break;
                default:
                    throw new CommandError("Unknown boss: " + bossName);
            }

            while (count-- > 0)
            {
                var position = World.GetRandomClearTile(target.position.X / 16f, target.position.Y / 16f);
                var id = NPC.NewNPC((int)(position.X * 16f), (int)(position.Y * 16f), type);
                Main.npc[id].SetDefaults(type);
                Main.npc[id].SetDefaults(Main.npc[id].name);

                if (type1 > 0)
                {
                    id = NPC.NewNPC((int)(position.X * 16f), (int)(position.Y * 16f), type1);
                    Main.npc[id].SetDefaults(type1);
                    Main.npc[id].SetDefaults(Main.npc[id].name);
                }

                if (count == 0)
                {
                    var tms = String.Empty;
                    if (count > 1)
                    {
                        tms = " " + count + " times";
                    }
                    Tools.NotifyAllPlayers(Main.npc[id].name + " [" + type + "]" + " summoned by " + sender.SenderName + tms, Color.Purple, true);
                }
            }
        }

        /// <summary>
        /// Manage item rejections
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void ItemRejection(ISender sender, ArgumentList args)
        {
#if TDSMServer
            string exception;
            if (args.TryParseOne<String>("-add", out exception))
            {
                if (!Server.ItemRejections.Contains(exception))
                {
                    Server.ItemRejections.Add(exception);
                    sender.Message(exception + " was successfully added.");
                }
                else
                {
                    throw new CommandError("Item already exists.");
                }
            }
            else if (args.TryParseOne<String>("-remove", out exception))
            {
                if (Server.ItemRejections.Contains(exception))
                {
                    Server.ItemRejections.Remove(exception);
                    sender.Message(exception + " was successfully removed.");
                }
                else
                {
                    throw new CommandError("Item does not exist.");
                }
            }
            else if (args.TryPop("-clear"))
            {
                Server.ItemRejections.Clear();
                sender.Message("Item rejection list cleared.");
            }
            else
            {
                throw new CommandError("Expected argument -add|-remove|-clear");
            }
#endif
        }

        /// <summary>
        /// Refreshes a players area
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void Refresh(ISender sender, ArgumentList args)
        {
            args.ParseNone();

            var player = sender as Player;

            if (player == null)
            {
                sender.Message(255, "This is a player command");
                return;
            }

            if (player.whoAmI < 0)
                return;

            if (!player.Op)
            {
                var diff = DateTime.Now - player.GetLastCostlyCommand();

                if (diff < TimeSpan.FromSeconds(30))
                {
                    sender.Message(255, "You must wait {0:0} more seconds before using this command.", 30.0 - diff.TotalSeconds);
                    return;
                }

                player.SetLastCostlyCommand(DateTime.Now);
            }

            NetMessage.SendTileSquare(player.whoAmI, (int)(player.position.X / 16), (int)(player.position.Y / 16), 32);
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
                Thread.Sleep(5);
            sender.Message("Hard mode is now enabled.");
        }

        ///// <summary>
        ///// Allows a user to take backups and purge old data
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="args"></param>
        //public static void Backups(ISender sender, ArgumentList args)
        //{
        //    var perform = args.TryPop("now");
        //    var purge = args.TryPop("purge");

        //    if (perform)
        //        BackupManager.PerformBackup();
        //    else if (purge)
        //    {
        //        int minutes;
        //        if (args.TryParseOne<Int32>(out minutes))
        //        {
        //            var backups = BackupManager.GetBackupsBefore(Main.worldName, DateTime.Now.AddMinutes(-minutes));

        //            var failCount = 0;
        //            foreach (var backup in backups)
        //                try
        //                {
        //                    File.Delete(backup);
        //                }
        //                catch { failCount++; }

        //            if (failCount > 0)
        //                sender.Message(
        //                    String.Format("Failed to deleted {0} backup(s).", failCount)
        //                );
        //            else
        //                sender.Message(
        //                    String.Format("Deleted {0} backup(s).", backups.Length - failCount)
        //                );
        //        }
        //        else
        //            throw new CommandError("Please specify a time frame.");
        //    }
        //    else
        //        throw new CommandError("Argument expected.");
        //}

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
                    this.TimelockTime = time;
                    this.TimelockRain = Main.raining;
                    this.TimelockSlimeRain = Main.slimeRain;
                    this.UseTimeLock = true;
                }
                else
                    throw new CommandError("Double expected.");
            }
            else
                throw new CommandError("Certain arguments expected.");

            if ((args.Plugin as Entry).UseTimeLock)
            {
                //if (!setNow) NewNetMessage.SendData(Packet.WORLD_DATA);
                if (!setNow)
                    NetMessage.SendData((int)Packet.WORLD_DATA);

                sender.Message(
                    String.Format("Time lock has set at {0}.", (args.Plugin as Entry).TimelockTime),
                    255, 0, 255, 0
                );
            }
        }

        /// <summary>
        /// Sets OP status to a given Player.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void OpPlayer(ISender sender, ArgumentList args)
        {
            var playerName = args.GetString(0);

            if (Storage.IsAvailable)
            {
                var existing = AuthenticatedUsers.GetUser(playerName);
                if (existing != null)
                {
                    if (existing.Value.Operator)
                        throw new CommandError("Player is already an operator");

                    if (AuthenticatedUsers.UpdateUser(playerName, true))
                    {
                        Tools.NotifyAllOps("Opping " + playerName + " [" + sender.SenderName + "]", true);
                        var player = Tools.GetPlayerByName(playerName);
                        if (player != null)
                        {
                            player.SendMessage("You are now a server operator.", Color.Green);
                            player.Op = true;
                            player.SetAuthentication(player.name, "tdsm");
                        }

                        sender.Message("Op success", Color.DarkGreen);
                    }
                    else
                    {
                        sender.Message("Failed to op player", Color.DarkRed);
                    }
                }
                else
                {
                    sender.Message("No user found by " + playerName, Color.DarkRed);
                    sender.Message("Please use the `user` command", Color.DarkRed);
                }
            }
            else
            {
                var password = args.GetString(1);

                Tools.NotifyAllOps("Opping " + playerName + " [" + sender.SenderName + "]", true);
                Ops.Add(playerName, password);

                //Player player;
                //if (args.TryGetOnlinePlayer(0, out player))
                //{
                //    playerName = player.Name;

                //    player.SendMessage("You are now a server operator.", Color.Green);
                //    player.Op = true;
                //}

                var player = Tools.GetPlayerByName(playerName);
                if (player != null)
                {
                    player.SendMessage("You are now a server operator.", Color.Green);
                    player.Op = true;
                    player.SetAuthentication(player.name, "tdsm");
                }

                if (!Ops.Save())
                {
                    Tools.NotifyAllOps("Failed to save op list [" + sender.SenderName + "]", true);
                    return;
                }
            }
        }

        /// <summary>
        /// De-OPs a given Player.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void DeopPlayer(ISender sender, ArgumentList args)
        {
            var playerName = args.GetString(0);
            //Player player;
            //if (args.TryGetOnlinePlayer(0, out player))
            //{
            //    playerName = player.Name;

            //    if (Server.Ops.Contains(playerName, true))
            //    {
            //        player.SendMessage("Your OP privledges have been revoked.", Color.Green);
            //    }

            //    player.Op = false;
            //}

            if (Storage.IsAvailable)
            {
                var existing = AuthenticatedUsers.GetUser(playerName);
                if (existing != null)
                {
                    if (!existing.Value.Operator)
                        throw new CommandError("Player is not an operator");
                    
                    var player = Tools.GetPlayerByName(playerName);
                    if (player != null)
                    {
                        player.SendMessage("Your server operator privledges have been revoked.", Color.DarkRed);
                        player.Op = false;
                        player.SetAuthentication(null, "tdsm");
                    }

                    if (AuthenticatedUsers.UpdateUser(playerName, false))
                    {
                        sender.Message("Deop success", Color.DarkGreen);
                    }
                    else
                    {
                        sender.Message("Failed to deop player", Color.DarkRed);
                    }
                }
                else
                {
                    sender.SendMessage("No user found by " + playerName);
                }
            }
            else
            {
                if (Ops.Contains(playerName))
                {
                    var player = Tools.GetPlayerByName(playerName);
                    if (player != null)
                    {
                        player.SendMessage("Your server operator privledges have been revoked.", Color.Green);
                        player.Op = false;
                        player.SetAuthentication(null, "tdsm");
                    }

                    Tools.NotifyAllOps("De-Opping " + playerName + " [" + sender.SenderName + "]", true);
                    Ops.Remove(playerName, true);

                    if (!Ops.Save())
                    {
                        Tools.NotifyAllOps("Failed to save op list [" + sender.SenderName + "]", true);
                    }
                }
                else
                    sender.SendMessage("No user found by " + playerName);
            }
        }

        /// <summary>
        /// Allows Operators to login.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="password">Password for verification</param>
        public void OpLogin(ISender sender, string password)
        {
            if (sender is Player)
            {
                var player = sender as Player;
                if (Storage.IsAvailable)
                {
                    var existing = AuthenticatedUsers.GetUser(sender.SenderName);
                    if (existing != null)
                    {
                        if (existing.Value.ComparePassword(sender.SenderName, password) && existing.Value.Operator)
                        {
                            Tools.NotifyAllOps(
                                String.Format("{0} successfully logged in.", player.Name)
                            );
                            player.Op = true;
                            player.SetAuthentication(sender.SenderName, "tdsm");
                            player.SendMessage("Successfully logged in.", Color.DarkGreen);
                        }
                        else
                        {
                            sender.Message("Login failed", Color.DarkRed);
                        }
                    }
                    else
                    {
                        sender.Message("Login failed", Color.DarkRed);
                    }
                }
                else
                {
                    if (Ops.Contains(player.name, password))
                    {
                        Tools.NotifyAllOps(
                            String.Format("{0} successfully logged in.", player.Name)
                        );
                        player.Op = true;
                        player.SetAuthentication(sender.SenderName, "tdsm");
                        player.SendMessage("Successfully logged in.", Color.DarkGreen);
                    }
                    else
                    {
                        player.SendMessage("Login failed", Color.DarkRed);
                    }
                }
            }
        }

        /// <summary>
        /// Allows users to log in.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="password">Password for verification</param>
        public void Auth(ISender sender, string password)
        {
            if (sender is Player)
            {
                var player = sender as Player;
                if (Storage.IsAvailable)
                {
                    var existing = AuthenticatedUsers.GetUser(sender.SenderName);
                    if (existing != null)
                    {
                        if (existing.Value.ComparePassword(sender.SenderName, password))
                        {
                            Tools.NotifyAllOps(
                                String.Format("{0} successfully logged in.", player.Name)
                            );
                            player.SendMessage("Successfully logged in.", Color.DarkGreen);
                            player.SetAuthentication(sender.SenderName, "tdsm");
                        }
                        else
                        {
                            sender.Message("Login failed", Color.DarkRed);
                        }
                    }
                    else
                    {
                        sender.Message("Login failed", Color.DarkRed);
                    }
                }
                else
                    sender.Message("This function is unavailable", Color.DarkRed);
            }
        }

        /// <summary>
        /// Allows Operators to logout.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void OpLogout(ISender sender, ArgumentList args)
        {
            if (sender is Player)
            {
                var player = sender as Player;
                if (sender.Op)
                {
                    player.Op = false;
                    player.SendMessage("Ssccessfully logged out", Color.DarkRed);
                    player.SetAuthentication(String.Empty, "tdsm");

                    Tools.NotifyAllOps(
                        String.Format("{0} successfully logged out.", player.Name)
                    );
                }
                else
                {
                    player.SendMessage("You are not logged in.", Color.DarkRed);
                }
            }
        }

        /// <summary>
        /// Manages the server list
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void ServerList(ISender sender, ArgumentList args)
        {
            string first;
            args.TryPopOne(out first);
            switch (first)
            {
                case "enable":
                    if (!Heartbeat.Enabled)
                    {
                        Heartbeat.Begin(CoreBuild);
                        sender.SendMessage("Heartbeat enabled to the TDSM server.");
                    }
                    else
                    {
                        sender.SendMessage("Heartbeat is already enabled.");
                    }
                    break;
                case "disable":
                    if (Heartbeat.Enabled)
                    {
                        Heartbeat.End();
                        sender.SendMessage("Heartbeat disabled to the TDSM server.");
                    }
                    else
                    {
                        sender.SendMessage("Heartbeat is not enabled.");
                    }
                    break;
                case "public":
                    Heartbeat.PublishToList = args.GetBool(0);
                    if (!ConfigUpdater.IsAvailable || ConfigUpdater.Set("server-list", Heartbeat.PublishToList))
                        sender.SendMessage("Server list is now " + (Heartbeat.PublishToList ? "public" : "private"));
                    else sender.Message("Failed to update visibility");
                    break;
                case "desc":
                    string d;
                    if (args.TryPopOne(out d))
                    {
                        Heartbeat.ServerDescription = d;
                        if (!ConfigUpdater.IsAvailable || ConfigUpdater.Set("server-list-desc", d))
                            sender.SendMessage("Description set to: " + Heartbeat.ServerDescription);
                        else sender.Message("Failed to update description");
                    }
                    else
                        sender.SendMessage("Current description: " + Heartbeat.ServerDescription);
                    break;
                case "name":
                    string n;
                    if (args.TryPopOne(out n))
                    {
                        Heartbeat.ServerName = n;
                        if (!ConfigUpdater.IsAvailable || ConfigUpdater.Set("server-list-name", n))
                            sender.SendMessage("Name set to: " + Heartbeat.ServerName);
                        else sender.Message("Failed to update name");
                    }
                    else
                        sender.SendMessage("Current name: " + Heartbeat.ServerName);
                    break;
                case "domain":
                    string h;
                    if (args.TryPopOne(out h))
                    {
                        Heartbeat.ServerDomain = h;
                        if (!ConfigUpdater.IsAvailable || ConfigUpdater.Set("server-list-domain", h))
                            sender.SendMessage("Domain set to: " + Heartbeat.ServerDomain);
                        else sender.Message("Failed to update domain");
                    }
                    else
                        sender.SendMessage("Current domain: " + Heartbeat.ServerDomain);
                    break;
                case "?":
                case "print":
                    sender.SendMessage("Heartbeat is " + (Heartbeat.Enabled ? "enabled" : "disabled"));
                    sender.SendMessage("Server list " + (Heartbeat.PublishToList ? "public" : "private"));
                    sender.SendMessage("Current name: " + Heartbeat.ServerName);
                    sender.SendMessage("Current domain: " + Heartbeat.ServerDomain);
                    sender.SendMessage("Current description: " + Heartbeat.ServerDescription);
                    break;
                default:
                    throw new CommandError("Not a supported serverlist command " + first);
            }
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
                        //TDSM.API.Callbacks.MainCallback.StartEclipse = true;
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
                        //TDSM.API.Callbacks.MainCallback.StartEclipse = true;
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

        Task _tskWaitForPlayers;
        private bool? _waitFPState;

        /// <summary>
        /// Restart and reload the world without reloading the application
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void Restart(ISender sender, ArgumentList args)
        {
#if TDSMServer
            string cmd = null;
            args.TryGetString(0, out cmd);

            if (String.IsNullOrEmpty(cmd))
                PerformRestart();
            else if (cmd == "-wait")
            {
                RestartWhenNoPlayers = !RestartWhenNoPlayers;

                if (_waitFPState == null) _waitFPState = Server.AcceptNewConnections;

                if (RestartWhenNoPlayers)
                {
                    Server.AcceptNewConnections = false;
                    if (ClientConnection.All.Count == 0)
                    {
                        PerformRestart();
                        return;
                    }

                    if (_tskWaitForPlayers == null)
                    {
                        _tskWaitForPlayers = new Task()
                        {
                            Enabled = true,
                            Method = (tsk) =>
                            {
                                Tools.NotifyAllPlayers("The server is waiting to restart.", Color.Orange, false);
                                Tools.NotifyAllPlayers("Please finish what you are doing and disconnect.", Color.Orange, false);

                                var players = from p in Terraria.Main.player where p.active orderby p.name select p.Name;

                                var pn = players.Count();
                                if (pn == 0) return;

                                ProgramLog.Admin.Log("Notified player(s) of restart: " + String.Join(", ", players));
                            },
                            Trigger = 60
                        };
                        Tasks.Schedule(_tskWaitForPlayers);
                    }
                    else
                    {
                        _tskWaitForPlayers.Enabled = true;
                    }
                }
                else
                {
                    Server.AcceptNewConnections = _waitFPState.Value; //Restore
                    if (_tskWaitForPlayers != null && _tskWaitForPlayers.Enabled)
                    {
                        if (_tskWaitForPlayers.HasTriggered)
                        {
                            Tools.NotifyAllPlayers("Restart was terminated.", Color.Orange);
                        }
                        _tskWaitForPlayers.Enabled = false;
                    }
                }

                sender.Message("The server is " + (_tskWaitForPlayers != null && _tskWaitForPlayers.Enabled ? "waiting to restart" : "not restarting"));
            }
            else throw new CommandError("No restart command: " + cmd);
#endif
        }

        void KillNPC(ISender sender, ArgumentList args)
        {
            if (Main.rand == null)
                Main.rand = new Random();
            
            int killed = 0;
            foreach (var npc in Main.npc)
            {
                if (npc != null && npc.active && !npc.townNPC && npc.whoAmI < 255)
                {
                    int damage = Int32.MaxValue;
                    npc.StrikeNPC(damage, 0, 0);
                    NetMessage.SendData((int)Packet.STRIKE_NPC, -1, -1, "", npc.whoAmI, damage);
                    NetMessage.SendData((int)Packet.NPC_INFO, -1, -1, "", npc.whoAmI, 0, 0, 0, 0, 0, 0);

                    killed++;
                }
            }

            sender.Message("Killed {0} npc(s)", Color.Green, killed);
        }
    }
}
