using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using tdsm.api;
using tdsm.api.Command;
using tdsm.api.Misc;
using tdsm.core.Definitions;
using tdsm.core.Logging;
using tdsm.core.Messages.Out;
using tdsm.core.ServerCore;
using Terraria;

namespace tdsm.core
{
    public partial class Entry
    {
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
            var accessLevel = AccessLevel.OP; //Program.properties.ExitAccessLevel;
            if ((int)accessLevel == -1 && sender is Player)
            {
                sender.Message("You cannot perform that action.", 255, 238, 130, 238);
                return;
            }
            else if (!CommandParser.CheckAccessLevel(accessLevel, sender))
            {
                sender.Message("You cannot perform that action.", 255, 238, 130, 238);
                return;
            }

            args.ParseNone();

            Tools.NotifyAllOps("Exiting on request.");
            ServerCore.Server.StopServer();
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

            var projs = 0; var uprojs = 0;
            var npcs = 0; var unpcs = 0;
            var items = 0;

            foreach (var npc in Main.npc)
            {
                if (!npc.active) continue;
                npcs += 1;
                if (!npc.netUpdate) continue;
                unpcs += 1;
            }

            foreach (var proj in Main.projectile)
            {
                if (!proj.active) continue;
                projs += 1;
                if (!proj.netUpdate) continue;
                uprojs += 1;
            }

            foreach (var item in Main.item)
            {
                if (!item.active) continue;
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

            var players = from p in Main.player where p.active select p.Name;
            var line = String.Concat("Current players:", String.Join(", ", players), (players.Count() > 0) ? "." : String.Empty);

            sender.Message(line, 255, 255, 240, 20);
        }

        /// <summary>
        /// Prints a player list, Possibly readable by bots.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void List(ISender sender, ArgumentList args)
        {
            args.ParseNone();

            var players = from p in Terraria.Main.player where p.active && !p.Op select p.Name;
            var ops = from p in Terraria.Main.player where p.active && p.Op select p.Name;

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

            sender.Message(string.Concat(os, ps, " (", on + pn, "/", SlotManager.MaxSlots, ")"), 255, 255, 240, 20);
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
                    NewNetMessage.SendData(25, -1, -1, "* " + sender.SenderName + " " + message, 255, 200, 100, 0);
                else
                    NewNetMessage.SendData(25, -1, -1, "* " + sender.SenderName + " " + message, 255, 238, 130, 238);
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
                NewNetMessage.SendData(25, -1, -1, "SERVER: " + message, 255, 238, 130, 238);
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

                //subject.HealEffect(subject.statLifeMax, true);
                NewNetMessage.SendData(35, -1, -1, String.Empty, subject.whoAmi, (float)subject.statLifeMax, 0f, 0f, 0);
            }
            else if (args.TryPop("-all"))
            {
                foreach (var plr in Main.player)
                {
                    if (plr.active)
                    {
                        NewNetMessage.SendData(35, -1, -1, String.Empty, plr.whoAmi, (float)plr.statLifeMax, 0f, 0f, 0);
                    }
                }
            }
            else throw new CommandError("No player or options were specified");
        }

        /// <summary>
        /// Executes the world data save routine.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void SaveAll(ISender sender, ArgumentList args)
        {
            Tools.NotifyAllOps("Saving world.....");

            WorldFile.saveWorld();

            while (WorldGen.saveLock)
                Thread.Sleep(100);

            Tools.NotifyAllOps("Saving data...", true);

            //Server.BanList.Save();
            //Server.WhiteList.Save();
            //Server.OpList.Save();

            Tools.NotifyAllOps("Saving Complete.", true);
        }

        ///// <summary>
        ///// Sends the help list to the requesting player's chat.
        ///// </summary>
        ///// <param name="sender">Sending player</param>
        ///// <param name="args">Arguments sent with command</param>
        //public static void ShowHelp(ISender sender, ArgumentList args)
        //{
        //    //var commands = Program.commandParser.serverCommands;
        //    //foreach (var plugin in PluginManager.plugins.Values)
        //    //{
        //    //    if (plugin.commands.Count > 0)
        //    //    {
        //    //        commands =
        //    //            commands.Concat(
        //    //                plugin.commands.Where(
        //    //                    kvp => !commands.ContainsKey(kvp.Key)
        //    //                    &&
        //    //                    !kvp.Key.StartsWith(plugin.Name.ToLower() + '.'))
        //    //                )
        //    //                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        //    //    }
        //    //}

        //    //if (args == null || args.Count < 1)
        //    //{
        //    //    for (int i = 0; i < commands.Values.Count; i++)
        //    //    {
        //    //        string Key = commands.Keys.ToArray()[i];
        //    //        CommandInfo cmdInfo = commands.Values.ToArray()[i];
        //    //        if (CommandParser.CheckAccessLevel(cmdInfo, sender) && !Key.StartsWith("."))
        //    //        {
        //    //            string tab = "\t";
        //    //            if (Key.Length < 8)
        //    //            {
        //    //                tab = "\t\t";
        //    //            }
        //    //            string Message = "\t" + Key + tab + "- " + cmdInfo.description;
        //    //            if (sender is Player)
        //    //            {
        //    //                Message = Message.Replace("\t", String.Empty);
        //    //            }
        //    //            sender.Message(Message);
        //    //        }
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    int maxPages = (commands.Values.Count / 5) + 1;
        //    //    if (maxPages > 0 && args.Count > 1 && args[0] != null)
        //    //    {
        //    //        try
        //    //        {
        //    //            int selectingPage = Int32.Parse(args[0].Trim());

        //    //            if (selectingPage < maxPages)
        //    //            {
        //    //                for (int i = 0; i < maxPages; i++)
        //    //                {
        //    //                    if ((selectingPage <= i))
        //    //                    {
        //    //                        selectingPage = i * ((commands.Values.Count / 5) + 1);
        //    //                        break;
        //    //                    }
        //    //                }

        //    //                int toPage = commands.Values.Count;
        //    //                if (selectingPage + 5 < toPage)
        //    //                {
        //    //                    toPage = selectingPage + 5;
        //    //                }

        //    //                for (int i = selectingPage; i < toPage; i++)
        //    //                {
        //    //                    string Key = commands.Keys.ToArray()[i];
        //    //                    CommandInfo cmdInfo = commands.Values.ToArray()[i];
        //    //                    if (CommandParser.CheckAccessLevel(cmdInfo, sender) && !Key.StartsWith("."))
        //    //                    {
        //    //                        string tab = "\t";
        //    //                        if (Key.Length < 8)
        //    //                        {
        //    //                            tab = "\t\t";
        //    //                        }
        //    //                        string Message = "\t" + Key + tab + "- " + cmdInfo.description;
        //    //                        if (sender is Player)
        //    //                        {
        //    //                            Message = Message.Replace("\t", String.Empty);
        //    //                        }
        //    //                        sender.Message(Message);
        //    //                    }
        //    //                }
        //    //            }
        //    //            else
        //    //            {
        //    //                sender.Message(Languages.InvalidPage + ": 0 -> " + (maxPages - 1).ToString());
        //    //            }
        //    //        }
        //    //        catch (Exception)
        //    //        {
        //    //            ShowHelp(sender, null);
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        ShowHelp(sender, null);
        //    //    }
        //    //}
        //}

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
            double Time;
            if (args.TryParseOne<Double>("-set", out Time))
            {
                World.SetTime(Time, true);
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
                    case "-now":
                        {
                            string AP = "AM";
                            double time = Terraria.Main.time;
                            if (!Main.dayTime)
                            {
                                time += 54000.0;
                            }
                            time = (time / 86400.0 * 24.0) - 19.5;
                            if (time < 0.0)
                            {
                                time += 24.0;
                            }
                            if (time >= 12.0)
                            {
                                AP = "PM";
                            }

                            int Hours = (int)time;
                            double Minutes = time - (double)Hours;
                            string MinuteString = (Minutes * 60.0).ToString();
                            if (Minutes < 10.0)
                            {
                                MinuteString = "0" + MinuteString;
                            }
                            if (Hours > 12)
                            {
                                Hours -= 12;
                            }
                            if (Hours == 0)
                            {
                                Hours = 12;
                            }
                            if (MinuteString.Length > 2)
                            {
                                MinuteString = MinuteString.Substring(0, 2);
                            }

                            sender.Message("Current time: " + Hours + ":" + MinuteString + " " + AP);
                            return;
                        }
                    default:
                        {
                            sender.Message("Please review your command");
                            return;
                        }
                }
            }
            NewNetMessage.SendData((int)Packet.WORLD_DATA); //Update Data
            Tools.NotifyAllPlayers("Time set to " + Terraria.Main.time.ToString() + " by " + sender.SenderName, Color.Green);
        }

        /// <summary>
        /// Gives specified item to the specified player.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void Give(ISender sender, ArgumentList args)
        {
            // /give <player> <stack> <name> 
            Player receiver = args.GetOnlinePlayer(0);
            int stack = args.GetInt(1);
            string name = args.GetString(2);

            var max = Tools.AvailableItemSlots; //Perhaps remove a few incase of new drops
            if (stack > max) {
				stack = max; // Set to Tools.AvailableItemSlots because number given was larger than this.
			}
            var results = DefinitionManager.FindItem(name);
            if (results != null && results.Length > 0)
            {
                if (results.Length > 1)
                    throw new CommandError(String.Format("More than 1 item found, total is: {0}", results.Length));

                var item = results[0];

                var index = receiver.GiveItem(item.Id, stack, sender, item.NetId, true, item.Prefix);

                if (item.NetId < 0)
                    Main.item[index].netDefaults(item.NetId);

                Main.item[index].Prefix(item.Prefix);
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
            if (args.Count > 3)
                throw new CommandError("Too many arguments");
            else if (sender is ConsoleSender && args.Count <= 2)
            {
                if (!Netplay.anyClients || !Tools.TryGetFirstOnlinePlayer(out player))
                    throw new CommandError("No players online.");
            }
            else if (args.Count == 3)
                player = args.GetOnlinePlayer(2);

            var npcName = args.GetString(1).ToLower().Trim();

            // Get the class id of the npc
            var npcs = DefinitionManager.FindNPC(npcName);
            if (npcs.Length == 0) throw new CommandError("No npc exists by the name {0}", npcName);
            else if (npcs.Length > 1) throw new CommandError("Too many results for {0}, total count {1}", npcName, npcs.Length);

            var npc = npcs[0];
            if (npc.Boss.HasValue && npc.Boss == true) throw new CommandError("This NPC can only be summoned by the SPAWNBOSS command.");
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
            if (amount > max) throw new CommandError("Cannot spawn that many, available slots: {0}", max);

            string realNPCName = String.Empty;
            for (int i = 0; i < amount; i++)
            {
                Vector2 location = World.GetRandomClearTile(((int)player.position.X / 16), ((int)player.position.Y / 16), 100, true, 100, 50);
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
                    /*if (*/
                    subject.Teleport(Main.spawnTileX, Main.spawnTileY);//)
                    {
                        Tools.NotifyAllOps(String.Format("{0} has teleported to spawn", subject.Name), true);
                    }
                    //else
                    //    sender.Message(Languages.TeleportFailed);
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

                    Tools.NotifyAllOps(string.Concat("Teleported ", subject.Name, " to ",
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

                    /*if (*/
                    subject.Teleport(x, y); //)
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

                var slot = tdsm.api.Callbacks.NetplayCallback.slots[s];

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
                NewNetMessage.SendData(25, -1, -1, player.Name + " has been kicked by " + sender.SenderName + ".", 255);
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

        //        lock (tdsm.api.Callbacks.Main.updatingItems)
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

        /// <summary>
        /// Summon a Boss
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void SummonBoss(ISender sender, ArgumentList args)
        {
            bool EoW = args.TryPop("eater");
            bool EyeOC = args.TryPop("eye");
            bool Skeletron = args.TryPop("skeletron");
            bool KingSlime = args.TryPop("kingslime");
            bool Twins = args.TryPop("twins");
            bool Wof = args.TryPop("wof");
            bool Destroyer = args.TryPop("destroyer");
            bool Prime = args.TryPop("prime");
            bool Golem = args.TryPop("golem");
            bool Plantera = args.TryPop("plantera");
            bool Retinazer = args.TryPop("retinazer");
            bool Spazmatism = args.TryPop("spazmatism");
            bool All = args.TryPop("-all");
            bool NightOverride = args.TryPop("-night") || All;

            Player player = null;
            if (sender is Player) player = sender as Player;
            else if (Netplay.anyClients)
            {
                string PlayerName;
                if (args.TryParseOne<String>("-player", out PlayerName))
                    player = Tools.GetPlayerByName(PlayerName);
                else
                {
                    if (Main.rand == null) Main.rand = new Random((new Random()).Next());
                    //Find Random
                    int plr = Main.rand.Next(0, ServerCore.ClientConnection.All.Count - 1); //Get Random PLayer
                    player = Main.player[plr];
                }

                if (player == null)
                    throw new CommandError("Cannot find player");
            }
            else
                throw new CommandError("No online players to spawn near.");

            List<Int32> bosses = new List<Int32>();

            if (EyeOC || Twins || Spazmatism || Retinazer || All || Prime || Skeletron | Destroyer)
            {
                if (Main.dayTime && !NightOverride)
                    throw new CommandError("The specified boss requires it to be night.");
            }

            var wofSummoned = Tools.IsNPCSummoned(113);
            if (Wof && wofSummoned)
                sender.Message("Wall of Flesh already summoned, Ignoring.");

            if (EyeOC || All) bosses.Add(4);
            if (Skeletron || All) bosses.Add(35);
            if (KingSlime || All) bosses.Add(50);
            if (EoW || All) bosses.Add(13);
            if (Twins || All) { bosses.Add(125); bosses.Add(126); }
			if (Retinazer) bosses.Add(125);
			if (Spazmatism) bosses.Add(126);
            if ((Wof) && !wofSummoned) bosses.Add(113);
            if (Destroyer || All) bosses.Add(134);
            if (Prime || All) bosses.Add(127);
            if (Golem) bosses.Add(245);
            if (Plantera) bosses.Add(262);

            if (bosses.Count > 0)
            {
                if (NightOverride)
                {
                    World.SetTime(16200.0, false);
                    //NewNetMessage.SendData((int)Packet.WORLD_DATA); //Update Data
                }

                foreach (int bossId in bosses)
                {
                    //Vector2 location = World.GetRandomClearTile(((int)player.Position.X / 16), ((int)player.Position.Y / 16), 100, true, 100, 50);
                    //int BossSlot = NPC.NewNPC(((int)location.X * 16), ((int)location.Y * 16), BossId);

                    //var npc = Main.npc[BossSlot];
                    //var name = npc.Name;

                    //if (!String.IsNullOrEmpty(npc.DisplayName))
                    //    name = npc.DisplayName;

                    //npc.TargetClosest(true);

                    var name = String.Empty;
                    switch (bossId)
                    {
                        case 4:
                            name = "Eye of Cthulu was";
                            break;
                        case 13:
                            name = "Eater of Worlds was";
                            break;
                        case 35:
                            name = "Skeletron was";
                            break;
                        case 50:
                            name = "King Slime was";
                            break;
                        case 113:
                            name = "Wall of Flesh was";
                            break;
                        case 125:
                            name = "The Twins were";
                            break;
                        case 127:
                            name = "Skeletron Prime was";
                            break;
                        case 134:
                            name = "The Destroyer was";
                            break;
                        case 245:
                            name = "Golem was";
                            break;
                        case 242:
                            name = "Plantera was";
                            break;

                    }
                    if (!String.IsNullOrEmpty(name)) Tools.NotifyAllPlayers(name + " summoned by " + sender.SenderName, Color.Purple, true);

                    //if (!(sender is ConsoleSender))
                    //    ProgramLog.Log("{0} summoned boss {1} at slot {2}.", sender.SenderName, name, BossSlot);

                    NPC.SpawnOnPlayer(player.whoAmi, bossId);
                }
            }
            else
            {
                throw new CommandError("Boss not specified");
            }
        }

        ///// <summary>
        ///// Disallow an item from the server
        ///// </summary>
        ///// <param name="sender">Sending player</param>
        ///// <param name="args">Arguments sent with command</param>
        //public static void ItemRejection(ISender sender, ArgumentList args)
        //{
        //    string exception;
        //    if (args.TryParseOne<String>(Languages.Add, out exception))
        //    {
        //        if (!Server.RejectedItems.Contains(exception))
        //        {
        //            Server.RejectedItems.Add(exception);
        //            sender.Message(exception + Languages.ItemRejection_Added);
        //        }
        //        else
        //        {
        //            throw new CommandError(Languages.ItemRejection_ItemExists);
        //        }
        //    }
        //    else if (args.TryParseOne<String>(Languages.Remove, out exception))
        //    {
        //        if (Server.RejectedItems.Contains(exception))
        //        {
        //            Server.RejectedItems.Remove(exception);
        //            sender.Message(exception + Languages.ItemRejection_Removed);
        //        }
        //        else
        //        {
        //            throw new CommandError(Languages.ItemRejection_ItemDoesntExist);
        //        }
        //    }
        //    else if (args.TryPop(Languages.Clear))
        //    {
        //        Server.RejectedItems.Clear();
        //        sender.Message(Languages.ItemRejection_Removed);
        //    }
        //    else
        //    {
        //        throw new CommandError(Languages.NoItemIDNameProvided);
        //    }
        //    Program.properties.RejectedItems = String.Join(",", Server.RejectedItems);
        //    Program.properties.Save(false);
        //}

        ///// <summary>
        ///// Toggle whether the server allows explosions
        ///// </summary>
        ///// <param name="sender">Sending player</param>
        ///// <param name="args">Arguments sent with command</param>
        //public static void Explosions(ISender sender, ArgumentList args)
        //{
        //    args.ParseNone();

        //    Server.AllowExplosions = !Server.AllowExplosions;
        //    Program.properties.AllowExplosions = Server.AllowExplosions;
        //    Program.properties.Save();

        //    sender.Message(Languages.ExplosionsAreNow + (Server.AllowExplosions ? "allowed" : "disabled") + "!");
        //}

        ///// <summary>
        ///// Refreshes a players area
        ///// </summary>
        ///// <param name="sender">Sending player</param>
        ///// <param name="args">Arguments sent with command</param>
        //public static void Refresh(ISender sender, ArgumentList args)
        //{
        //    args.ParseNone();

        //    var player = sender as Player;

        //    if (player == null)
        //    {
        //        sender.Message(255, Languages.ThisIsPlayerCommand);
        //        return;
        //    }

        //    if (player.whoAmi < 0) return;

        //    if (!player.Op)
        //    {
        //        var diff = DateTime.Now - player.LastCostlyCommand;

        //        if (diff < TimeSpan.FromSeconds(30))
        //        {
        //            sender.Message(255, Languages.YouMustWaitBeforeAnotherCommand, 30.0 - diff.TotalSeconds);
        //            return;
        //        }

        //        player.LastCostlyCommand = DateTime.Now;
        //    }

        //    NewNetMessage.SendTileSquare(player.whoAmi, (int)(player.Position.X / 16), (int)(player.Position.Y / 16), 32);
        //}

        ///// <summary>
        ///// Toggles whether the server allows RPG.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="args"></param>
        //public static void ToggleRPGClients(ISender sender, ArgumentList args)
        //{
        //    Server.AllowTDCMRPG = !Server.AllowTDCMRPG;
        //    Program.properties.AllowTDCMRPG = Server.AllowTDCMRPG;

        //    foreach (Player player in Main.player)
        //    {
        //        if (player.HasClientMod)
        //            NewNetMessage.SendData(Packet.CLIENT_MOD, player.whoAmi);
        //    }

        //    if (!Server.OpList.Save())
        //    {
        //        Tools.NotifyAllOps(Languages.OPlistFailedSave + " [" + sender.SenderName + "]", true);
        //        return;
        //    }

        //    string message = (Server.AllowTDCMRPG) ? Languages.RPGMode_Allowed : Languages.RPGMode_Refused;
        //    Tools.NotifyAllOps(message);
        //}

        ///// <summary>
        ///// Spawns the TDCM Quest Giver
        ///// </summary>
        ///// <param name="sender">Sending player</param>
        ///// <param name="args">Arguments sent with command</param>
        //public static void SpawnQuestGiver(ISender sender, ArgumentList args)
        //{
        //    if (!Server.AllowTDCMRPG)
        //        throw new CommandError(Languages.CannotQuestGiverWithoutTDCM);

        //    int npcId;
        //    if (NPC.TryFindNPCByName(Statics.TDCM_QUEST_GIVER, out npcId))
        //        throw new CommandError(Languages.QuestGiverAlreadySpawned);

        //    NPC.SpawnTDCMQuestGiver();
        //}

        ///// <summary>
        ///// Enables hardmode
        ///// </summary>
        ///// <param name="sender">Sending player</param>
        ///// <param name="args">Arguments sent with command</param>
        //public static void HardMode(ISender sender, ArgumentList args)
        //{
        //    args.ParseNone();

        //    if (Main.hardMode)
        //        throw new CommandError(Languages.HardModeAlreadyEnabled);

        //    sender.Message(Languages.StartingHardMode);
        //    WorldModify.StartHardMode();
        //}

        ///// <summary>
        ///// Reloads the language definitions
        ///// </summary>
        ///// <param name="sender">Sending player</param>
        ///// <param name="args">Arguments sent with command</param>
        //public static void LanguageReload(ISender sender, ArgumentList args)
        //{
        //    args.ParseNone();

        //    sender.Message("Reloading Language File...");
        //    sender.Message("Reloading " + (Languages.LoadClass(Collections.Registries.LANGUAGE_FILE) ? "Succeeded" : "Failed"));
        //}

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
                if (!(args.Plugin as Entry).UseTimeLock) { sender.Message("Time lock is already disabled", 255, 255, 0, 0); return; }

                (args.Plugin as Entry).UseTimeLock = false;
                sender.Message("Time lock has been disabled.", 255, 0, 255, 0);
                return;
            }
            else if (setNow) (args.Plugin as Entry).UseTimeLock = true;
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
                    (args.Plugin as Entry).TimelockTime = time;
                    (args.Plugin as Entry).UseTimeLock = true;
                }
                else throw new CommandError("Double expected.");
            }
            else throw new CommandError("Certain arguments expected.");

            if ((args.Plugin as Entry).UseTimeLock)
            {
                if (!setNow) NewNetMessage.SendData(Packet.WORLD_DATA);

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
            var password = args.GetString(1);

            Tools.NotifyAllOps("Opping " + playerName + " [" + sender.SenderName + "]", true);
            Server.Ops.Add(playerName, password);

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
            }

            if (!Server.Ops.Save())
            {
                Tools.NotifyAllOps("Failed to save op list [" + sender.SenderName + "]", true);
                return;
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

            if (Server.Ops.Contains(playerName))
            {
                var player = Tools.GetPlayerByName(playerName);
                if (player != null)
                {
                    player.SendMessage("Your server operator privledges have been revoked.", Color.Green);
                    player.Op = false;
                }

                Tools.NotifyAllOps("De-Opping " + playerName + " [" + sender.SenderName + "]", true);
                Server.Ops.Remove(playerName, true);

                if (!Server.Ops.Save())
                {
                    Tools.NotifyAllOps("Failed to save op list [" + sender.SenderName + "]", true);
                }
            }
            else sender.SendMessage("No user found by " + playerName);
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
                if (Server.Ops.Contains(player.name, password))
                {
                    Tools.NotifyAllOps(
                        String.Format("{0} successfully logged in.", player.Name)
                    );
                    player.Op = true;
                    player.SendMessage("Successfully logged in.", Color.DarkGreen);
                }
                else
                {
                    player.SendMessage("Login failed", Color.DarkRed);
                }
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
    }
}
