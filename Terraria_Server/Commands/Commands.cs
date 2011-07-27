using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Terraria_Server;
using System.Threading;
using Terraria_Server.Collections;
using Terraria_Server.Misc;
using Terraria_Server.Logging;
using Terraria_Server.RemoteConsole;
using Terraria_Server.WorldMod;
using Terraria_Server.Definitions;
using Terraria_Server.Plugin;

namespace Terraria_Server.Commands
{
    public class Commands
    {
        /// <summary>
        /// Closes the Server all connections.
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void Exit (Server server, ISender sender, ArgumentList args)
        {
            if (sender is Player || sender is RConSender)
            {
                sender.sendMessage ("You cannot perform that action.", 255, 238, 130, 238);
                return;
            }
            
            args.ParseNone ();
            
            server.notifyOps ("Exiting on request.", true);
            server.StopServer ();
            
            return;
        }

        /// <summary>
        /// Outputs statistics of the servers performance.
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void Status (Server server, ISender sender, ArgumentList args)
        {
            args.ParseNone ();
            
            var process = System.Diagnostics.Process.GetCurrentProcess();
            sender.sendMessage (string.Format ("Virtual memory:  {0:0.0}/{1:0.0}MB",
                process.VirtualMemorySize64 / 1024.0 / 1024.0,
                process.PeakVirtualMemorySize64 / 1024.0 / 1024.0));
            sender.sendMessage (string.Format ("Physical memory: {0:0.0}/{1:0.0}MB",
                process.WorkingSet64 / 1024.0 / 1024.0,
                process.PeakWorkingSet64 / 1024.0 / 1024.0));
            var time = process.TotalProcessorTime;
            sender.sendMessage (string.Format ("Total cpu usage:        {0:0.00}% ({1})",
                100.0 * time.TotalMilliseconds / (DateTime.Now - process.StartTime).TotalMilliseconds, time));
            
            if (LoadMonitor.LoadLastSecond >= 0)
                sender.sendMessage (string.Format ("Cpu usage last second:  {0:0.00}%", LoadMonitor.LoadLastSecond));
                
            if (LoadMonitor.LoadLastMinute >= 0)
                sender.sendMessage (string.Format ("Cpu usage last minute:  {0:0.00}%", LoadMonitor.LoadLastMinute));
            
            sender.sendMessage (string.Format ("Last world update took: {0:0.000}ms (plr: {1:0.0}ms, npc: {2:0.0}ms, proj: {3:0.0}ms, item: {4:0.0}ms, world: {5:0.0}ms, time: {6:0.0}ms, inva: {7:0.0}ms, serv: {8:0.0}ms)",
                Program.LastUpdateTime.TotalMilliseconds,
                Main.LastPlayerUpdateTime.TotalMilliseconds,
                Main.LastNPCUpdateTime.TotalMilliseconds,
                Main.LastProjectileUpdateTime.TotalMilliseconds,
                Main.LastItemUpdateTime.TotalMilliseconds,
                Main.LastWorldUpdateTime.TotalMilliseconds,
                Main.LastTimeUpdateTime.TotalMilliseconds,
                Main.LastInvasionUpdateTime.TotalMilliseconds,
                Main.LastServerUpdateTime.TotalMilliseconds
                ));
            
            var projs = 0; var uprojs = 0;
            var npcs = 0; var unpcs = 0;
            var items = 0;
            
            foreach (var npc in Main.npcs)
            {
                if (! npc.Active) continue;
                npcs += 1;
                if (! npc.netUpdate) continue;
                unpcs += 1;
            }

            foreach (var proj in Main.projectile)
            {
                if (! proj.Active) continue;
                projs += 1;
                if (! proj.netUpdate) continue;
                uprojs += 1;
            }
            
            foreach (var item in Main.item)
            {
                if (! item.Active) continue;
                items += 1;
            }
            
            sender.sendMessage (string.Format ("NPCs: {0}a/{1}u, projectiles: {2}a/{3}u, items: {4}", npcs, unpcs, projs, uprojs, items));
        }

        /// <summary>
        /// Reloads Plugins.
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void Reload (Server server, ISender sender, ArgumentList args)
        {
            args.ParseNone ();
            
            server.notifyOps ("Reloading plugins.", true);
            server.PluginManager.ReloadPlugins ();
            
            return;
        }

        /// <summary>
        /// Prints a Playerlist.
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void OldList (Server server, ISender sender, ArgumentList args)
        {
            args.ParseNone ();
            
            var players = from p in Server.players where p.Active select p.Name;
            sender.sendMessage (string.Concat ("Current players: ", string.Join (", ", players), "."), 255, 255, 240, 20);
        }

        /// <summary>
        /// Prints a player list, Possibly readable by bots.
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void List (Server server, ISender sender, ArgumentList args)
        {
            args.ParseNone ();
            
            var players = from p in Server.players where p.Active && !p.Op select p.Name;
            var ops = from p in Server.players where p.Active && p.Op select p.Name;
            
            var pn = players.Count();
            var on = ops.Count();
            
            if (on + pn == 0)
            {
                sender.sendMessage ("No players online.");
                return;
            }
            
            string ps = "";
            string os = "";
            
            if (pn > 0)
                ps = (on > 0 ? " | Players: " : "Players: ") + string.Join (", ", players);
            
            if (on > 0)
                os = "Ops: " + string.Join (", ", ops);
            
            sender.sendMessage (string.Concat (os, ps, " (", on + pn, "/", Main.maxNetplayers, ")"), 255, 255, 240, 20);
        }

        /// <summary>
        /// 3rd person talking.
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="message">Message to send</param>
        public static void Action (Server server, ISender sender, string message)
        {
            ProgramLog.Chat.Log ("* " + sender.Name + " " + message);
            if (sender is Player)
                NetMessage.SendData (25, -1, -1, "* " + sender.Name + " " + message, 255, 200, 100, 0);
            else
                NetMessage.SendData (25, -1, -1, "* " + sender.Name + " " + message, 255, 238, 130, 238);
        }

        /// <summary>
        /// Sends a Server Message to all online Players.
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="message">Message to send</param>
        public static void Say (Server server, ISender sender, string message)
        {
            ProgramLog.Chat.Log ("<" + sender.Name + "> " + message);
            if (sender is Player)
                NetMessage.SendData (25, -1, -1, "<" + sender.Name + "> " + message, 255, 255, 255, 255);
            else
                NetMessage.SendData (25, -1, -1, "<" + sender.Name + "> " + message, 255, 238, 180, 238);
        }

        /// <summary>
        /// Executes the world data save routine.
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void SaveAll(Server server, ISender sender, ArgumentList args)
        {
            Program.server.notifyOps("Saving World...", true);

            WorldIO.saveWorld(Program.server.World.SavePath, false);
            while (WorldModify.saveLock)
            {
            }

            Program.server.notifyOps("Saving Data...", true);

            Program.server.BanList.Save();
            Program.server.WhiteList.Save();

            Program.server.notifyOps("Saving Complete.", true);
        }
        
        /// <summary>
        /// Sends the help list to the requesting player's chat.
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void ShowHelp(Server server, ISender sender, ArgumentList args)
        {
            if (args == null || args.Count < 1)
            {
                for (int i = 0; i < Program.commandParser.serverCommands.Values.Count; i++)
                {
                    String Key = Program.commandParser.serverCommands.Keys.ToArray()[i];
                    CommandInfo cmdInfo = Program.commandParser.serverCommands.Values.ToArray()[i];
                    if (CommandParser.CheckAccessLevel(cmdInfo, sender) && !Key.StartsWith("."))
                    {
                        String tab = "\t";
                        if (Key.Length < 8)
                        {
                            tab = "\t\t";
                        }
                        String Message = "\t" + Key + tab + "- " + cmdInfo.description;
                        if (sender is Player)
                        {
                            Message = Message.Replace("\t", "");
                        }
                        sender.sendMessage(Message);
                    }
                }
            }
            else
            {
                int maxPages = (Program.commandParser.serverCommands.Values.Count / 5) + 1;
                if (maxPages > 0 && args.Count > 1 && args[0] != null)
                {
                    try
                    {
                        int selectingPage = Int32.Parse(args[0].Trim());

                        if (selectingPage < maxPages)
                        {
                            for (int i = 0; i < maxPages; i++)
                            {
                                if ((selectingPage <= i))
                                {
                                    selectingPage = i * ((Program.commandParser.serverCommands.Values.Count / 5) + 1);
                                    break;
                                }
                            }

                            int toPage = Program.commandParser.serverCommands.Values.Count;
                            if (selectingPage + 5 < toPage)
                            {
                                toPage = selectingPage + 5;
                            }

                            for (int i = selectingPage; i < toPage; i++)
                            {
                                String Key = Program.commandParser.serverCommands.Keys.ToArray()[i];
                                CommandInfo cmdInfo = Program.commandParser.serverCommands.Values.ToArray()[i];
                                if (CommandParser.CheckAccessLevel(cmdInfo, sender) && !Key.StartsWith("."))
                                {
                                    String tab = "\t";
                                    if (Key.Length < 8)
                                    {
                                        tab = "\t\t";
                                    }
                                    String Message = "\t" + Key + tab + "- " + cmdInfo.description;
                                    if (sender is Player)
                                    {
                                        Message = Message.Replace("\t", "");
                                    }
                                    sender.sendMessage(Message);
                                }
                            }
                        }
                        else
                        {
                            sender.sendMessage("Invalid page! Use: 0 -> " + (maxPages - 1).ToString());
                        }
                    }
                    catch (Exception)
                    {
                        ShowHelp(server, sender, null);
                    }
                }
                else
                {
                    ShowHelp(server, sender, null);
                }
            }
        }

        /// <summary>
        /// Adds or removes specified player to/from the white list.
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void WhiteList(Server server, ISender sender, ArgumentList args)
        {
            // /whitelist <add:remove> <player>
            String Exception, Type = "removed from";
            if (args.TryParseOne<String>("-add", out Exception))
            {
                Program.server.WhiteList.addException(Exception);
                Type = "added to";
            }
            else if (args.TryParseOne<String>("-remove", out Exception))
            {

                Program.server.WhiteList.removeException(Exception);
            }
            else
            {
                sender.sendMessage("Please review that command");
                return;
            }

            Program.server.notifyOps(Exception + " was " + Type + " the Whitelist {" + sender.Name + "}", true);

            if (!Program.server.WhiteList.Save())
            {
                Program.server.notifyOps("WhiteList Failed to Save due to " + sender.Name + "'s command", true);
            }
        }

        /// <summary>
        /// Adds a player or ip (Exception) to the ban list.
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void Ban(Server server, ISender sender, ArgumentList args)
        {
            if (args != null && args.Count > 0)
            {
                //We now should check to make sure they are off the server...
                Player banee = Program.server.GetPlayerByName(args[0]);

                if (banee == null)
                {
                    foreach (Player player in Program.server.PlayerList)
                    {
                        var ip = Netplay.slots[player.whoAmi].remoteAddress.Split(':')[0];
                        if (ip == args[0])
                        {
                            banee = player;
                        }
                    }
                }

                Program.server.BanList.addException(args[0]);

                if (banee != null)
                {
                    banee.Kick("You have been banned from this Server.");
                    Program.server.BanList.addException(Netplay.slots[banee.whoAmi].
                        remoteAddress.Split(':')[0]);
                }


                Program.server.notifyOps(args[0] + " has been banned {" + sender.Name + "}", true);
                if (!Program.server.BanList.Save())
                {
                    Program.server.notifyOps("BanList Failed to Save due to " + sender.Name + "'s command", true);
                }
            }
            else
            {
                sender.sendMessage("Please review that command");
            }
        }

        /// <summary>
        /// Removes an exception from the ban list.
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void UnBan(Server server, ISender sender, ArgumentList args)
        {
            if (args != null && args.Count > 0)
            {
                Program.server.BanList.removeException(args[0]);

                Program.server.notifyOps(args[0] + " has been unbanned {" + sender.Name + "}", true);

                if (!Program.server.BanList.Save())
                {
                    Program.server.notifyOps("BanList Failed to Save due to " + sender.Name + "'s command", true);
                }
            }
            else
            {
                sender.sendMessage("Please review that command");
            }
        }

        /// <summary>
        /// Sets the time in the game.
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void Time(Server server, ISender sender, ArgumentList args)
        {
            Double Time;
            if (args.TryParseOne<Double>("-set", out Time))
            {
                server.World.setTime(Time, true);
            }
            else
            {
                String caseType = args.GetString (0);
                switch (caseType)
                {
                    case "day":
                        {
                            server.World.setTime(13500.0);
                            break;
                        }
                    case "dawn":
                        {
                            server.World.setTime(0);
                            break;
                        }
                    case "dusk":
                        {
                            server.World.setTime(0, false, false);
                            break;
                        }
                    case "noon":
                        {
                            server.World.setTime(27000.0);
                            break;
                        }
                    case "night":
                        {
                            server.World.setTime(16200.0, false, false);
                            break;
                        }
                    case "-now":
                        {
                            String AP = "AM";
                            double time = Main.time;
                            if (!Main.dayTime)
                            {
                                time += 54000.0;
                            }
                            time = time / 86400.0 * 24.0;
                            double num2 = 7.5; //stuffs me at this stage
                            time = time - num2 - 12.0;
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
                            String MinuteString = (Minutes * 60.0).ToString();
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
                            sender.sendMessage("Current Time: " + Hours + ":" + MinuteString + " " + AP);
                            return;
                        }
                    default:
                        {
                            sender.sendMessage("Please review that command.");
                            return;
                        }
                }
            }
            NetMessage.SendData((int)Packet.WORLD_DATA); //Update Data
            server.notifyAll("Time set to " + Server.time.ToString() + " by " + sender.Name);
        }

        /// <summary>
        /// Gives specified item to the specified player.
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void Give(Server server, ISender sender, ArgumentList args)
        {
            // /give <player> <stack> <name> 
            if (args.Count > 2 && args[0] != null && args[1] != null && args[2] != null &&
                args[0].Trim().Length > 0 && args[1].Trim().Length > 0 && args[2].Trim().Length > 0)
            {
                String playerName = args[0].Trim();
                String itemName = string.Join(" ", args);
                itemName = itemName.Remove(0, itemName.IndexOf(" " + args[2]));

                Player player = Program.server.GetPlayerByName(playerName);
                if (player != null)
                {
                    Item[] items = new Item[Main.maxItemTypes];
                    for (int i = 0; i < Main.maxItemTypes; i++)
                    {
                        items[i] = Registries.Item.Create(i);
                    }

                    Item item = null;
                    itemName = itemName.Replace(" ", "").ToLower();
                    for (int i = 0; i < Main.maxItemTypes; i++)
                    {
                        if (items[i].Name != null)
                        {
                            String genItemName = items[i].Name.Replace(" ", "").Trim().ToLower();
                            if (genItemName == itemName)
                            {
                                item = items[i];
                            }
                        }
                    }

                    int itemType = -1;
                    bool assumed = false;
                    if (item != null)
                    {
                        itemType = item.Type;
                    }
                    else
                    {
                        int assumedItem;
                        try
                        {
                            assumedItem = Int32.Parse(itemName);
                        }
                        catch (Exception)
                        {
                            sender.sendMessage("Item '" + itemName + "' not found!");
                            return;
                        }

                        for (int i = 0; i < Main.maxItemTypes; i++)
                        {
                            if (items[i].Type == assumedItem)
                            {
                                itemType = items[i].Type;
                                assumed = true;
                                break;
                            }
                        }

                        if (!assumed)
                        {
                            sender.sendMessage("Item '" + itemName + "' not found!");
                            return;
                        }
                    }

                    //Clear Data
                    for (int i = 0; i < Main.maxItemTypes; i++)
                    {
                        items[i] = null;
                    }
                    items = null;

                    if (itemType != -1)
                    {

                        int stackSize;
                        try
                        {
                            stackSize = Int32.Parse(args[1]);
                        }
                        catch (Exception)
                        {
                            stackSize = 1;
                        }

                        Item.NewItem((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height, itemType, stackSize, false);

                        Program.server.notifyOps("Giving " + player.Name + " some " + itemType.ToString() + " {" + sender.Name + "}", true);

                        return;
                    }
                }
                else
                {
                    sender.sendMessage("Player '" + playerName + "' not found!");
                    return;
                }
            }
            else
            {
                goto ERROR;
            }

        ERROR:
            sender.sendMessage("Command Error!");
        }

        /// <summary>
        /// Spawns specified NPC type.
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void SpawnNPC(Server server, ISender sender, ArgumentList args)
        {
			// /spawnnpc <amount> <name:id>
			try
			{
				Player player = ((Player)sender);
                if (args.Count >= 3)
                {
                    player = Program.server.GetPlayerByName(args[1]);
                    if (null == player)
                    {
                        sender.sendMessage("Player not found.", 255, 238f, 130f, 238f);
                        return;
                    }
                }

                String npcName = args[1].Replace(" ", "").ToLower();

				// Get the class id of the npc
				Int32 realNPCId = 0;
				NPC fclass = Registries.NPC.FindClass(npcName);
				if (fclass.type != Registries.NPC.Default.type)
				{
					realNPCId = fclass.Type;
				}
				else
				{
					realNPCId = Int32.Parse(npcName);
				}

                int NPCAmount = Int32.Parse(args[0]);

				String realNPCName = "";
				for (int i = 0; i < NPCAmount; i++)
				{
					Vector2 location = World.GetRandomClearTile(((int)player.Position.X / 16), ((int)player.Position.Y / 16), 100, true, 100, 50);
                    int npcIndex = NPC.NewNPC(((int)location.X * 16), ((int)location.Y * 16), realNPCId);
                    Main.npcs[npcIndex] = Registries.NPC.Alter(Main.npcs[npcIndex], fclass.Name);
					realNPCName = Main.npcs[npcIndex].Name;
				}
				Program.server.notifyOps("Spawned " + NPCAmount.ToString() + " of " +
						realNPCName + " {" + player.Name + "}", true);
			}
			catch
			{
				sender.sendMessage("Command Error!");
			}
        }

        /// <summary>
        /// Teleports player1 to a second specified player's location.
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void Teleport(Server server, ISender sender, ArgumentList args)
        {
            // /tp <player> <toplayer>
            if (args.Count > 1 && args[0] != null && args[1] != null && args[0].Trim().Length > 0 && args[1].Trim().Length > 0)
            {
                Player player = Program.server.GetPlayerByName(args[0].Trim());
                Player toplayer = Program.server.GetPlayerByName(args[1].Trim());

                if (player == null || toplayer == null)
                {
                    sender.sendMessage("Could not find a Player on the Server");
                    return;
                }

                player.teleportTo(toplayer);

                Program.server.notifyOps("Teleported " + player.Name + " to " +
                    toplayer.Name + " {" + sender.Name + "}", true);

                return;
            }
            else
            {
                goto ERROR;
            }

        ERROR:
            sender.sendMessage("Command Error!");
        }

        /// <summary>
        /// Teleports specified player to sending player's location.
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void TeleportHere(Server server, ISender sender, ArgumentList args)
        {
            if (sender is Player)
            {
                Player player = ((Player)sender);

                // /tp <player> <toplayer>
                if (args.Count > 0 && args[0] != null && args[0].Trim().Length > 0)
                {
                    Player toplayer = Program.server.GetPlayerByName(args[0].Trim());

                    if (toplayer == null)
                    {
                        sender.sendMessage("Could not find a Player on the Server");
                        return;
                    }

                    toplayer.teleportTo(player);

                    Program.server.notifyOps("Teleported " + toplayer.Name + " to " +
                        player.Name + " {" + sender.Name + "}", true);

                    return;
                }
            }
            else
            {
                goto ERROR;
            }

        ERROR:
            sender.sendMessage("Command Error!");
        }

        /// <summary>
        /// Settles water like in the startup routine.
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void SettleWater(Server server, ISender sender, ArgumentList args)
        {
            if (!Liquid.panicMode)
            {
                sender.sendMessage("Settling Liquids...");
                Liquid.StartPanic();
                sender.sendMessage("Complete.");
            }
            else
            {
                sender.sendMessage("Liquids are already settling");
            }
        }

        /// <summary>
        /// Sets OP status to a given Player.
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void OpPlayer(Server server, ISender sender, ArgumentList args)
        {
            if (args.Count > 1)
            {
                String Password = args[0];
                String player = string.Join(" ", args);
                player = player.Remove(0, player.IndexOf(Password) + Password.Length).Trim().ToLower();

                server.notifyOps("Opping " + player + " {" + sender.Name + "}", true);
                server.OpList.addException(player + ":" + Password, true, player.Length);


                if (!server.OpList.Save())
                {
                    server.notifyOps("OpList Failed to Save due. {" + sender.Name + "}", true);
                    return;
                } 

                Player playerInstance = server.GetPlayerByName(player);
                if (playerInstance != null)
                {
                    playerInstance.sendMessage("You are now OP!", ChatColour.Green);
                }
            }
            else
            {
                sender.sendMessage("Please review that command");
            }
        }

        /// <summary>
        /// De-OPs a given Player.
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void DeopPlayer(Server server, ISender sender, ArgumentList args)
        {
            if (args.Count > 0)
            {
                String player = string.Join(" ", args).Trim();

                server.notifyOps("De-Opping " + player + " {" + sender.Name + "}", true);

                if (Player.isInOpList(player, server))
                {
                    Program.server.OpList.removeException(player + ":" + Player.GetPlayerPassword(player, server));
                }

                if (!server.OpList.Save())
                {
                    server.notifyOps("OpList Failed to Save due. {" + sender.Name + "}", true);
                    return;
                }  

                Player playerInstance = server.GetPlayerByName(player);
                if (playerInstance != null)
                {
                    playerInstance.Op = false;
                    playerInstance.sendMessage("You have been De-Opped!.", ChatColour.Green);
                }
            }
            else
            {
                sender.sendMessage("Please review that command");
            }
        }

        /// <summary>
        /// Allows Operators to login.
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void OpLogin(Server server, ISender sender, ArgumentList args)
        {
            if (sender is Player)
            {
                Player player = sender as Player;
                String Password = string.Join(" ", args).Trim();
                if (player.isInOpList())
                {
                    if (player.Password.Equals(Password))
                    {
                        player.Op = true;
                        player.sendMessage("Successfully Logged in as OP.", ChatColour.DarkGreen);
                    }
                    else
                    {
                        player.sendMessage("Incorrect OP Password.", ChatColour.DarkRed);
                    }
                }
                else
                {
                    player.sendMessage("You need to be Assiged OP Privledges.", ChatColour.DarkRed);
                }
            }
        }

        /// <summary>
        /// Allows Operators to logout.
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void OpLogout(Server server, ISender sender, ArgumentList args)
        {
            if (sender is Player)
            {
                if (sender.Op)
                {
                    sender.Op = false;
                    ((Player)sender).sendMessage("Successfully Logged Out.", ChatColour.DarkRed);
                }
                else
                {
                    ((Player)sender).sendMessage("You need to be Assiged OP Privledges.", ChatColour.DarkRed);
                }
            }
        }
        
        /// <summary>
        /// Enables or disables NPC spawning
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void NPCSpawns(Server server, ISender sender, ArgumentList args)
        {
            Main.stopSpawns = !Main.stopSpawns;

            if (Main.stopSpawns)
            {
                sender.sendMessage("NPC Spawning is now off!");
            }
            else
            {
                sender.sendMessage("NPC Spawning is now on!");
            }
        }

        /// <summary>
        /// Kicks a given Player from the server
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void Kick (Server server, ISender sender, ArgumentList args)
        {
            if (args.TryPop ("-s"))
            {
                int s;
                args.ParseOne (out s);
                
                var slot = Netplay.slots[s];
                
                if (slot.state != SlotState.VACANT)
                {
                    slot.Kick ("You have been kicked by " + sender.Name + ".");
                    
                    var player = Main.players[s];
                    if (player != null && player.Name != null)
                        NetMessage.SendData (25, -1, -1, player.Name + " has been kicked by " + sender.Name + ".", 255);
                }
                else
                {
                    sender.sendMessage ("kick: Slot is vacant.");
                }
            }
            else
            {
                Player player;
                args.ParseOne<Player> (out player);
            
                if (player.Name == null)
                {
                    sender.sendMessage("kick: Error, player has null name.");
                    return;
                }
            
                player.Kick ("You have been kicked by " + sender.Name + ".");
                NetMessage.SendData (25, -1, -1, player.Name + " has been kicked by " + sender.Name + ".", 255);
            }
        }
                
        /// <summary>
        /// Restarts the server
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void Restart(Server server, ISender sender, ArgumentList args)
        {
            server.notifyOps("Restarting the Server {" + sender.Name + "}", true);
            Statics.keepRunning = true;
            server.StopServer();
            while (Statics.serverStarted) { Thread.Sleep (10); }
            ProgramLog.Log ("Starting the Server");
            server.Initialize();
            WorldIO.loadWorld();
            Program.updateThread = new Thread(Program.UpdateLoop);
            server.StartServer();
            Statics.keepRunning = false;
        }

        /// <summary>
        /// Checks the state of a slot.
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
		public static void Slots (Server server, ISender sender, ArgumentList args)
		{
			bool dinfo = args.Contains ("-d") || args.Contains ("-dp") || args.Contains ("-pd");
			bool pinfo = args.Contains ("-p") || args.Contains ("-dp") || args.Contains ("-pd");
			
			int k = 0;
			for (int i = 0; i < 255; i++)
			{
				var slot = Netplay.slots[i];
				var player = Main.players[i];
				
				if (slot.state != SlotState.VACANT)
				{
					k += 1;
					
					var name = "";
					if (player != null)
					{
						name = string.Concat (", ", player.Op ? "Op. " : "", "\"", (player.Name ?? "<null>"), "\"");
						if (player.AuthenticatedAs != null)
						{
							if (player.Name == player.AuthenticatedAs)
								name = name + " (auth'd)";
							else
								name = name + " (auth'd as " + player.AuthenticatedAs + ")";
						}
					}
					
					var addr = "<secret>";
					if (! (sender is Player && player.Op))
						addr = slot.remoteAddress;
					
					var msg = string.Format ("slot {0}: {1}, {2}{3}", i, slot.state, addr, name);
					
					if (pinfo && player != null)
					{
						msg += string.Format (", {0}/{1}hp", player.statLife, player.statLifeMax);
					}
					
					if (dinfo)
					{
						msg += string.Format (", recv:{0} side:{1}", NetMessage.buffer[i].totalData, NetMessage.buffer[i].sideBufferBytes);
					}
					
					sender.sendMessage (msg);
				}
			}
			sender.sendMessage (string.Format ("{0}/{1} slots occupied.", k, Main.maxNetplayers));
		}

        /// <summary>
        /// Purge Server data
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
		public static void Purge (Server server, ISender sender, ArgumentList args)
		{
			var all = args.TryPop ("all");
			var something = false;
			
			if (all || args.TryPop ("proj") || args.TryPop ("projectiles"))
			{
				something = true;
				
				ProgramLog.Admin.Log ("Purging all projectiles.");
				
				var msg = NetMessage.PrepareThreadInstance ();
				
				msg.PlayerChat (255, "<Server> Purging all projectiles.", 255, 180, 100);
				
				lock (Main.updatingProjectiles)
				{
					foreach (var projectile in Main.projectile)
					{
						projectile.Active = false;
						projectile.type = ProjectileType.UNKNOWN;
						
						msg.Projectile (projectile);
					}
					
					msg.Broadcast ();
				}
			}
			
			if (all || args.TryPop ("npc") || args.TryPop ("npcs"))
			{
				something = true;
				
				ProgramLog.Admin.Log ("Purging all NPCs.");
				
				var msg = NetMessage.PrepareThreadInstance ();
				
				msg.PlayerChat (255, "<Server> Purging all NPCs.", 255, 180, 100);
				
				lock (Main.updatingNPCs)
				{
					foreach (var npc in Main.npcs)
					{
						if (npc.Active)
						{
							npc.Active = false;
							npc.life = 0;
							npc.netUpdate = false;
							npc.Name = "";
							
							msg.NPCInfo (npc.whoAmI);
						}
					}
					
					msg.Broadcast ();
				}
			}
			
			if (all || args.TryPop ("item") || args.TryPop ("items"))
			{
				something = true;
				
				ProgramLog.Admin.Log ("Purging all items.");
				
				var msg = NetMessage.PrepareThreadInstance ();
				
				msg.PlayerChat (255, "<Server> Purging all items.", 255, 180, 100);
				
				lock (Main.updatingItems)
				{
					for (int i = 0; i < 200; i++)
					{
						var item = Main.item[i];
						if (item.Active)
						{
							Main.item[i] = new Item(); // this is what Main does when ignoreErrors is on *shrug*
							msg.ItemInfo (i);
							msg.ItemOwnerInfo (i);
						}
					}
					
					msg.Broadcast ();
				}
			}
			
			if (! something)
				throw new CommandError ("");
		}

        /// <summary>
        /// Lists currently enabled plugins.
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void ListPlugins(Server server, ISender sender, ArgumentList args)
        {
                if (server.PluginManager.PluginList.Count > 0)
                {
                    String plugins = "";

                    foreach (Plugin.Plugin plugin in server.PluginManager.PluginList.Values)
                    {
                        if (!plugin.Enabled || plugin.Name.Trim().Length > 0)
                        {
                            plugins += ", " + plugin.Name.Trim();
                        }
                    }
                    if (plugins.StartsWith(","))
                    {
                        plugins = plugins.Remove(0, 1).Trim(); //Remove the ', ' from the start and trim the ends
                    }
                    sender.sendMessage("Loaded Plugins: " + plugins + ".");
                }
                else
                {
                    sender.sendMessage("There are no loaded plugins.");
                }
        }

        /// <summary>
        /// Enable/disable and get details about specific plugins.
        /// </summary>
        /// <param name="server">Current Server instance</param>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void ManagePlugins(Server server, ISender sender, ArgumentList args)
        {
            /*
             * Commands:
             *      list    - shows all plugins
             *      info    - shows a plugin's author & description etc
             *      disable - disables a plugin
             *      enable  - enables a plugin
             */
            if (args.Count > 0 && args[0] != null && args[0].Trim().Length > 0)
            {
                String command = args[0].Trim();
                args.RemoveAt(0); //Allow the commands to use any additional arguments without also getting the command
                switch (command)
                {
                    case "list":
                        {
                            if (server.PluginManager.PluginList.Count > 0)
                            {
                                String plugins = "";

                                foreach (Plugin.Plugin plugin in server.PluginManager.PluginList.Values)
                                {
                                    if (plugin.Name.Trim().Length > 0)
                                    {
                                        plugins += ", " + plugin.Name.Trim() + ((!plugin.Enabled) ? "[DISABLED] " : " ");
                                    }
                                }
                                if (plugins.StartsWith(","))
                                {
                                    plugins = plugins.Remove(0, 1).Trim(); //Remove the ', ' from the start and trim the ends
                                }
                                sender.sendMessage("Plugins: " + plugins + ".");
                            }
                            else
                            {
                                sender.sendMessage("There are no installed plugins.");
                            }
                            break;
                        }
                    case "info":
                        {
                            if (!(args.Count > 1 && args[1] != null && args[0].Trim().Length > 0))
                            {
                                sender.sendMessage("Please review your argument count.");
                            }

                            String pluginName = string.Join(" ", args);

                            if (server.PluginManager.PluginList.Count > 0)
                            {
                                Plugin.Plugin fplugin = server.PluginManager.GetPlugin(pluginName);
                                if (fplugin != null)
                                {
                                    sender.sendMessage("Plugin Name: " + fplugin.Name);
                                    sender.sendMessage("Plugin Author: " + fplugin.Author);
                                    sender.sendMessage("Plugin Description: " + fplugin.Description);
                                    sender.sendMessage("Plugin Enabled: " + fplugin.Enabled.ToString());
                                }
                                else
                                {
                                    sender.sendMessage("The plugin \"" + args[1] + "\" was not found.");
                                }
                            }
                            else
                            {
                                sender.sendMessage("There are no plugins loaded.");
                            }
                            break;
                        }
                    case "disable":
                        {
                            if (!(args.Count > 1 && args[1] != null && args[1].Trim().Length > 0))
                            {
                                sender.sendMessage("Please review your argument count.");
                            }

                            String pluginName = string.Join(" ", args);

                            if (server.PluginManager.PluginList.Count > 0)
                            {
                                Plugin.Plugin fplugin = server.PluginManager.GetPlugin(pluginName);
                                if (fplugin != null)
                                {
                                    if (fplugin.Enabled)
                                    {
                                        if (server.PluginManager.DisablePlugin(fplugin.Name))
                                        {
                                            sender.sendMessage(pluginName + " was disabled!");
                                        }
                                        else
                                        {
                                            sender.sendMessage("There was an issue disabling plugin \"" + pluginName + "\".");
                                        }
                                    }
                                    else
                                    {
                                        sender.sendMessage("The plugin \"" + pluginName + "\" is already disabled.");
                                    }
                                }
                                else
                                {
                                    sender.sendMessage("The plugin \"" + pluginName + "\" could not be found.");
                                }
                            }
                            else
                            {
                                sender.sendMessage("There are no plugins loaded.");
                            }
                            break;
                        }
                    case "enable":
                        {
                            if (!(args.Count > 1 && args[1] != null && args[0].Trim().Length > 0))
                            {
                                sender.sendMessage("Please review your argument count.");
                            }

                            String pluginName = string.Join(" ", args);

                            if (server.PluginManager.PluginList.Count > 0)
                            {
                                Plugin.Plugin fplugin = server.PluginManager.GetPlugin(pluginName);
                                if (fplugin != null)
                                {
                                    if (!fplugin.Enabled)
                                    {
                                        if (server.PluginManager.EnablePlugin(fplugin.Name))
                                        {
                                            sender.sendMessage(args[1] + " was enabled!");
                                        }
                                        else
                                        {
                                            sender.sendMessage("There was an issue enabling plugin \"" + pluginName + "\".");
                                        }
                                    }
                                    else
                                    {
                                        sender.sendMessage("The plugin \"" + pluginName + "\" is already enabled.");
                                    }
                                }
                                else
                                {
                                    sender.sendMessage("The plugin \"" + pluginName + "\" could not be found.");
                                }
                            }
                            else
                            {
                                sender.sendMessage("There are no plugins loaded.");
                            }
                            break;
                        }
                    default:
                        {
                            sender.sendMessage("Please review your argument count");
                            break;
                        }
                }
            }
        }
    }
}
