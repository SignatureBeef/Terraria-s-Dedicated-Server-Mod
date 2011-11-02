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
using Terraria_Server.Plugins;
using Terraria_Server.Networking;
using System.IO;

namespace Terraria_Server.Commands
{
	public class Commands
	{
		/// <summary>
		/// Closes the Server all connections.
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void Exit(ISender sender, ArgumentList args)
		{
            int AccessLevel = Program.properties.ExitAccessLevel;
            if (AccessLevel == -1 && sender is Player)
            {
                    sender.sendMessage("You cannot perform that action.", 255, 238, 130, 238);
                    return;
            }
            else if (!CommandParser.CheckAccessLevel((AccessLevel)AccessLevel, sender))
            {
                sender.sendMessage("You cannot perform that action.", 255, 238, 130, 238);
                return;
            }
            
			args.ParseNone();

			Server.notifyOps("Exiting on request.", false);
			NetPlay.StopServer();
			Statics.Exit = true;

            throw new ExitException(String.Format("{0} requested that TDSM is to shutdown.", sender.Name));
		}

		/// <summary>
		/// Outputs statistics of the servers performance.
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void Status(ISender sender, ArgumentList args)
		{
			args.ParseNone();

			var process = System.Diagnostics.Process.GetCurrentProcess();
			sender.sendMessage(String.Format("Virtual memory:  {0:0.0}/{1:0.0}MB",
				process.VirtualMemorySize64 / 1024.0 / 1024.0,
				process.PeakVirtualMemorySize64 / 1024.0 / 1024.0));
			sender.sendMessage(String.Format("Physical memory: {0:0.0}/{1:0.0}MB",
				process.WorkingSet64 / 1024.0 / 1024.0,
				process.PeakWorkingSet64 / 1024.0 / 1024.0));
			var time = process.TotalProcessorTime;
			sender.sendMessage(String.Format("Total cpu usage:        {0:0.00}% ({1})",
				100.0 * time.TotalMilliseconds / (DateTime.Now - process.StartTime).TotalMilliseconds, time));

			if (LoadMonitor.LoadLastSecond >= 0)
				sender.sendMessage(String.Format("Cpu usage last second:  {0:0.00}%", LoadMonitor.LoadLastSecond));

			if (LoadMonitor.LoadLastMinute >= 0)
				sender.sendMessage(String.Format("Cpu usage last minute:  {0:0.00}%", LoadMonitor.LoadLastMinute));

			sender.sendMessage(String.Format("Last world update took: {0:0.000}ms (plr: {1:0.0}ms, npc: {2:0.0}ms, proj: {3:0.0}ms, item: {4:0.0}ms, world: {5:0.0}ms, time: {6:0.0}ms, inva: {7:0.0}ms, serv: {8:0.0}ms)",
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
				if (!npc.Active) continue;
				npcs += 1;
				if (!npc.netUpdate) continue;
				unpcs += 1;
			}

			foreach (var proj in Main.projectile)
			{
				if (!proj.Active) continue;
				projs += 1;
				if (!proj.netUpdate) continue;
				uprojs += 1;
			}

			foreach (var item in Main.item)
			{
				if (!item.Active) continue;
				items += 1;
			}
			
			sender.sendMessage(String.Format("NPCs: {0}a/{1}u, projectiles: {2}a/{3}u, items: {4}", npcs, unpcs, projs, uprojs, items));
			//long diff = Connection.TotalOutgoingBytesUnbuffered - Connection.TotalOutgoingBytes;
			//sender.sendMessage(String.Format("NPCs: {0}a/{1}u, projectiles: {2}a/{3}u, items: {4}, bytes saved: {5:0.0}K ({6:0.0}%)", npcs, unpcs, projs, uprojs, items, diff, diff * 100.0 / Connection.TotalOutgoingBytesUnbuffered));

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
//			sender.sendMessage (sb.ToString());
//#endif
		}

		/// <summary>
		/// Reloads Plugins.
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void Reload(ISender sender, ArgumentList args)
		{
			Server.notifyOps("Reloading server.properties.", true);
			Program.properties.Load();
		}

		/// <summary>
		/// Prints a Playerlist.
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void OldList(ISender sender, ArgumentList args)
		{
			args.ParseNone();

            var players = from p in Main.players where p.Active select p.Name;
			sender.sendMessage(string.Concat("Current players: ", String.Join(", ", players), "."), 255, 255, 240, 20);
		}

		/// <summary>
		/// Prints a player list, Possibly readable by bots.
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void List(ISender sender, ArgumentList args)
		{
			args.ParseNone();

            var players = from p in Main.players where p.Active && !p.Op select p.Name;
            var ops = from p in Main.players where p.Active && p.Op select p.Name;

			var pn = players.Count();
			var on = ops.Count();

			if (on + pn == 0)
			{
				sender.sendMessage("No players online.");
				return;
			}

			string ps = "";
			string os = "";

			if (pn > 0)
				ps = (on > 0 ? " | Players: " : "Players: ") + String.Join(", ", players);

			if (on > 0)
				os = "Ops: " + String.Join(", ", ops);

			sender.sendMessage(string.Concat(os, ps, " (", on + pn, "/", SlotManager.MaxSlots, ")"), 255, 255, 240, 20);
		}

		/// <summary>
		/// 3rd person talking.
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="message">Message to send</param>
		public static void Action(ISender sender, string message)
		{
			ProgramLog.Chat.Log("* " + sender.Name + " " + message);
			if (sender is Player)
				NetMessage.SendData(25, -1, -1, "* " + sender.Name + " " + message, 255, 200, 100, 0);
			else
				NetMessage.SendData(25, -1, -1, "* " + sender.Name + " " + message, 255, 238, 130, 238);
		}

		/// <summary>
		/// Sends a Server Message to all online Players.
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="message">Message to send</param>
		public static void Say(ISender sender, string message)
		{
			/*ProgramLog.Chat.Log("<" + sender.Name + "> " + message);
			if (sender is Player)
				NetMessage.SendData(25, -1, -1, "<" + sender.Name + "> " + message, 255, 255, 255, 255);
			else
				NetMessage.SendData(25, -1, -1, "<" + sender.Name + "> " + message, 255, 238, 180, 238);*/

            // 'Say' should be used for Server Messages, OP's only. This is used on many large servers to notify
            // Users for a quick restart (example), So the OP will most likely be in game, unless it's major.
            ProgramLog.Chat.Log("<" + sender.Name + "> " + ((sender is ConsoleSender) ? "" : "SERVER: ") + message);
            NetMessage.SendData(25, -1, -1, "SERVER: " + message, 255, 238, 130, 238);
		}

		/// <summary>
		/// Executes the world data save routine.
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void SaveAll(ISender sender, ArgumentList args)
		{
			Server.notifyOps("Saving World...", true);

			WorldIO.saveWorld(Server.World.SavePath, false);
			while (WorldModify.saveLock)
			{
                Thread.Sleep(100);
			}

			Server.notifyOps("Saving Data...", true);

			Server.BanList.Save();
			Server.WhiteList.Save();
            Server.OpList.Save();

			Server.notifyOps("Saving Complete.", true);
		}

		/// <summary>
		/// Sends the help list to the requesting player's chat.
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void ShowHelp(ISender sender, ArgumentList args)
		{
			if (args == null || args.Count < 1)
			{
				for (int i = 0; i < Program.commandParser.serverCommands.Values.Count; i++)
				{
					string Key = Program.commandParser.serverCommands.Keys.ToArray()[i];
					CommandInfo cmdInfo = Program.commandParser.serverCommands.Values.ToArray()[i];
					if (CommandParser.CheckAccessLevel(cmdInfo, sender) && !Key.StartsWith("."))
					{
                        string tab = "\t";
						if (Key.Length < 8)
						{
							tab = "\t\t";
						}
                        string Message = "\t" + Key + tab + "- " + cmdInfo.description;
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
                                string Key = Program.commandParser.serverCommands.Keys.ToArray()[i];
								CommandInfo cmdInfo = Program.commandParser.serverCommands.Values.ToArray()[i];
								if (CommandParser.CheckAccessLevel(cmdInfo, sender) && !Key.StartsWith("."))
								{
                                    string tab = "\t";
									if (Key.Length < 8)
									{
										tab = "\t\t";
									}
                                    string Message = "\t" + Key + tab + "- " + cmdInfo.description;
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
						ShowHelp(sender, null);
					}
				}
				else
				{
					ShowHelp(sender, null);
				}
			}
		}

		/// <summary>
		/// Adds or removes specified player to/from the white list.
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void WhiteList(ISender sender, ArgumentList args)
		{
			// /whitelist <add:remove> <player>
            string Exception, Type = "removed from";
			if (args.TryParseOne<String>("-add", out Exception))
			{
				Server.WhiteList.addException(Exception);
				Type = "added to";
			}
			else if (args.TryParseOne<String>("-remove", out Exception))
			{

				Server.WhiteList.removeException(Exception);
			}
			else
			{
				sender.sendMessage("Please review that command");
				return;
			}

			Server.notifyOps(Exception + " was " + Type + " the Whitelist {" + sender.Name + "}", true);

			if (!Server.WhiteList.Save())
			{
				Server.notifyOps("WhiteList Failed to Save due to " + sender.Name + "'s command", true);
			}
		}

		/// <summary>
		/// Adds a player or ip (Exception) to the ban list.
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void Ban(ISender sender, ArgumentList args)
		{
            Player banee;
            string playerName = null;

            if (args.TryGetOnlinePlayer(0, out banee))
            {
                playerName = banee.Name;
                banee.Kick("You have been banned from this Server.");
                Server.BanList.addException(NetPlay.slots[banee.whoAmi].
                        remoteAddress.Split(':')[0]);
            }
            else if(!args.TryGetString(0, out playerName))
            {
                throw new CommandError("A player or IP was expected.");
            }

            Server.BanList.addException(playerName);

            Server.notifyOps(playerName + " has been banned {" + sender.Name + "}", true);
            if (!Server.BanList.Save())
            {
                Server.notifyOps("BanList Failed to Save due to " + sender.Name + "'s command", true);
            }
		}

		/// <summary>
		/// Removes an exception from the ban list.
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void UnBan(ISender sender, ArgumentList args)
		{
            string playerName;
            if (!args.TryGetString(0, out playerName))
            {
                throw new CommandError("A player or IP was expected.");
            }

            Server.BanList.removeException(playerName);

            Server.notifyOps(playerName + " has been unbanned {" + sender.Name + "}", true);

            if (!Server.BanList.Save())
            {
                Server.notifyOps("BanList Failed to Save due to " + sender.Name + "'s command", true);
            }
		}

		/// <summary>
		/// Sets the time in the game.
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void Time(ISender sender, ArgumentList args)
		{
			double Time;
			if (args.TryParseOne<Double>("-set", out Time))
			{
                Server.World.setTime(Time, true);
			}
			else
			{
                string caseType = args.GetString(0);
				switch (caseType)
				{
					case "day":
						{
                            Server.World.setTime(13500.0);
							break;
						}
					case "dawn":
						{
                            Server.World.setTime(0);
							break;
						}
					case "dusk":
						{
                            Server.World.setTime(0, false, false);
							break;
						}
					case "noon":
						{
                            Server.World.setTime(27000.0);
							break;
						}
					case "night":
						{
                            Server.World.setTime(16200.0, false, false);
							break;
						}
					case "-now":
						{
                            string AP = "AM";
							double time = Main.time;
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
            Server.notifyAll("Time set to " + Main.time.ToString() + " by " + sender.Name);
		}

		/// <summary>
		/// Gives specified item to the specified player.
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
        public static void Give(ISender sender, ArgumentList args)
		{
			// /give <player> <stack> <name> 

            Player receiver = args.GetOnlinePlayer(0);
            int stack = args.GetInt(1);
            string NameOrId = args.GetString(2);

            List<Int32> itemlist;
            if (Server.TryFindItemByName(NameOrId, out itemlist) && itemlist.Count > 0)
            {
                if (itemlist.Count > 1)
                    throw new CommandError("There were {0} Items found regarding the specified name", itemlist.Count);

                foreach (int id in itemlist)
                    receiver.GiveItem(id, stack, sender);
            }
            else
            {
                int Id = -1;
                try
                {
                    Id = Int32.Parse(NameOrId);
                }
                catch
                {
                    throw new CommandError("There were {0} Items found regarding the specified Item Id/Name", itemlist.Count);
                }

                if (Server.TryFindItemByType(Id, out itemlist) && itemlist.Count > 0)
                {
                    if (itemlist.Count > 1)
                        throw new CommandError("There were {0} Items found regarding the specified Type Id", itemlist.Count);

                    foreach (int id in itemlist)
                        receiver.GiveItem(id, stack, sender);
                }
                else
                {
                    throw new CommandError("There were no Items found regarding the specified Item Id/Name");
                }
            }            
		}

		/// <summary>
		/// Spawns specified NPC type.
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void SpawnNPC(ISender sender, ArgumentList args)
		{
			Player player = sender as Player;
			if (args.Count > 3)
			{
				throw new CommandError("Too many arguments. NPC and player names with spaces require quotes.");
			}
            else if (sender is ConsoleSender && args.Count <= 2)
            {
                throw new CommandError("As console you need to specify the player to spawn near.");
            }
            else if (args.Count == 3)
            {
                player = args.GetOnlinePlayer(2);
            }

            string npcName = args.GetString(1).ToLower().Trim();

			// Get the class id of the npc
			int realNPCId = 0;
			NPC fclass = Registries.NPC.FindClass(npcName);
			if (fclass.type != Registries.NPC.Default.type)
			{
				realNPCId = fclass.Type;
			}
			else
			{
				try
				{
					realNPCId = Int32.Parse(npcName);
				}
				catch
				{
					throw new CommandError("Specified NPC does not exist");
				}
			}

			int NPCAmount = 0;

			try
			{
				NPCAmount = Int32.Parse(args[0]);
				if (NPCAmount > Program.properties.SpawnNPCMax && sender is Player)
				{
					(sender as Player).Kick ("Don't spawn that many.");
					return;
				}
			}
			catch
			{
				throw new CommandError("Expected integer for number to spawn.");
			}

            string realNPCName = "";
			for (int i = 0; i < NPCAmount; i++)
			{
				Vector2 location = World.GetRandomClearTile(((int)player.Position.X / 16), ((int)player.Position.Y / 16), 100, true, 100, 50);
				int npcIndex = NPC.NewNPC(((int)location.X * 16), ((int)location.Y * 16), fclass.Name);
				//Registries.NPC.Alter(Main.npcs[npcIndex], fclass.Name);
				realNPCName = Main.npcs[npcIndex].Name;
			}
			Server.notifyOps("Spawned " + NPCAmount.ToString() + " of " +
					realNPCName + " {" + player.Name + "}", true);
		}

		/// <summary>
		/// Teleports player1 to a second specified player's location.
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void Teleport(ISender sender, ArgumentList args)
		{
			Player subject;
			Player target;
			
			if (! args.TryPopOne (out subject))
			{
				subject = sender as Player;
				if (subject == null)
				{
					sender.sendMessage ("Need specify who to teleport.");
					return;
				}
				
				if (args.Count == 0)
				{
					if (subject.Teleport (Main.spawnTileX, Main.spawnTileY))
					{
						Server.notifyOps (string.Concat ("Teleported ", subject.Name, " to spawn."), true);
					}
					else
						sender.sendMessage ("Teleportation failed.");
					return;
				}
			}
			else if (args.Count == 0)
			{
				target = subject;
				
				subject = sender as Player;
				if (subject == null)
				{
					sender.sendMessage ("Need specify who to teleport.");
					return;
				}

				if (subject.Teleport (target))
				{

					Server.notifyOps (string.Concat ("Teleported ", subject.Name, " to ",
						target.Name, ". {", sender.Name, "}"), true);
				}
				else
					sender.sendMessage ("Teleportation failed.");
				return;
			}
			
			int x;
			int y;
			
			if (args.Count == 1)
			{
				if (args.TryParseOne (out target))
				{
					if (subject.Teleport (target))
					{
						Server.notifyOps (string.Concat ("Teleported ", subject.Name, " to ",
							target.Name, ". {", sender.Name, "}"), true);
					}
					else
						sender.sendMessage ("Teleportation failed.");
				}
				else
					sender.sendMessage ("Target player not found.");
				return;
			}
			else if (args.Count == 2)
			{
				if (args.TryParseTwo (out x, out y))
				{
					if (x < 0 || x >= Main.maxTilesX || y < 0 || y >= Main.maxTilesY)
					{
						sender.sendMessage (String.Format ("Coordinates out of range of (0, {0}); (0, {1}).", Main.maxTilesX - 1, Main.maxTilesY - 1));
						return;
					}
					
					if (subject.Teleport (x, y))
					{
						Server.notifyOps (string.Concat ("Teleported ", subject.Name, " to ",
							x, ":", y, ". {", sender.Name, "}"), true);
					}
					else
						sender.sendMessage ("Teleportation failed.");
				}
				else
					throw new CommandError ("Invalid coordinates.");
				return;
			}
			
			throw new CommandError ("");
		}

		/// <summary>
		/// Teleports specified player to sending player's location.
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void TeleportHere(ISender sender, ArgumentList args)
		{
			if (sender is Player)
			{
				Player player = ((Player)sender);
                Player subject;

                if (args.TryPopOne(out subject))
                {
                    if (subject == null)
                    {
                        sender.sendMessage("Could not find a Player on the Server");
                        return;
                    }

                    subject.Teleport(player);

                    Server.notifyOps("Teleported " + subject.Name + " to " +
                        player.Name + " {" + sender.Name + "}", true);
                }
			}
			else
			{
                throw new CommandError("Only a player can call this command!");
			}
		}

		/// <summary>
		/// Settles water like in the startup routine.
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void SettleWater(ISender sender, ArgumentList args)
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
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void OpPlayer(ISender sender, ArgumentList args)
		{
            var playerName = args.GetString(0);
            var password = args.GetString(1);
            Player player;
            if (args.TryGetOnlinePlayer(0, out player))
            {
                playerName = player.Name;

                player.sendMessage("You are now OP!", ChatColor.Green);
                player.Op = true;
                if (player.HasClientMod)
                {
                    NetMessage.SendData(Packet.CLIENT_MOD, player.whoAmi);
                }
            }

            Server.notifyOps("Opping " + playerName + " {" + sender.Name + "}", true);
            Server.OpList.addException(playerName + ":" + password, true, playerName.Length + 1);

            if (!Server.OpList.Save())
            {
                Server.notifyOps("OpList Failed to Save due. {" + sender.Name + "}", true);
                return;
            }
		}

		/// <summary>
		/// De-OPs a given Player.
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void DeopPlayer(ISender sender, ArgumentList args)
		{
            var playerName = args.GetString(0);
            Player player;
            if (args.TryGetOnlinePlayer(0, out player))
            {
                playerName = player.Name;

                if (Player.isInOpList(playerName))
                {
                    player.sendMessage("You have been De-Opped!.", ChatColor.Green);
                }

                player.Op = false;
                if (player.HasClientMod)
                {
                    NetMessage.SendData(Packet.CLIENT_MOD, player.whoAmi);
                }
            }

            if (Player.isInOpList(playerName))
            {
                Server.notifyOps("De-Opping " + playerName + " {" + sender.Name + "}", true);
                Server.OpList.removeException(playerName + ":" + Player.GetPlayerPassword(playerName));
            }

            if (!Server.OpList.Save())
            {
                Server.notifyOps("OpList Failed to Save due. {" + sender.Name + "}", true);
                return;
            }
		}

		/// <summary>
		/// Allows Operators to login.
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void OpLogin(ISender sender, ArgumentList args)
		{
			if (sender is Player)
			{
				Player player = sender as Player;
                string Password = String.Join(" ", args).Trim();
				if (player.isInOpList())
				{
					if (player.Password.Equals(Password))
					{
                        Server.notifyOps("{0} Logged in as OP.", true, player.Name);
						player.Op = true;
						player.sendMessage("Successfully Logged in as OP.", ChatColor.DarkGreen);
                        
                        if (player.HasClientMod)
                        {
                            NetMessage.SendData(Packet.CLIENT_MOD, player.whoAmi);
                        }
					}
					else
                    {
                        Server.notifyOps("{0} Failed to log in as OP due to incorrect password.", true, player.Name);
						player.sendMessage("Incorrect OP Password.", ChatColor.DarkRed);
					}
				}
				else
				{
					player.sendMessage("You need to be Assigned OP Privledges.", ChatColor.DarkRed);
				}
			}
		}

		/// <summary>
		/// Allows Operators to logout.
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void OpLogout(ISender sender, ArgumentList args)
		{
			if (sender is Player)
			{
                var player = sender as Player;
				if (sender.Op)
				{
                    player.Op = false;
                    player.sendMessage("Successfully Logged Out.", ChatColor.DarkRed);

                    Server.notifyOps("{0} logged out.", true, player.Name);

                    if (player.HasClientMod)
                    {
                        NetMessage.SendData(Packet.CLIENT_MOD, player.whoAmi);
                    }
				}
				else
				{
                    player.sendMessage("You need to be Assigned OP Privledges.", ChatColor.DarkRed);
				}
			}
		}

		/// <summary>
		/// Enables or disables NPC spawning
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void NPCSpawns(ISender sender, ArgumentList args)
        {
            args.ParseNone();

			Main.stopSpawns = !Main.stopSpawns;
            sender.sendMessage("NPC Spawning is now " + ((Main.stopSpawns) ? "off" : "on") + "!");
		}

		/// <summary>
		/// Kicks a given Player from the server
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void Kick(ISender sender, ArgumentList args)
		{
			if (args.TryPop("-s"))
			{
				int s;
				args.ParseOne(out s);

				var slot = NetPlay.slots[s];

				if (slot.state != SlotState.VACANT)
				{
					slot.Kick("You have been kicked by " + sender.Name + ".");

					var player = Main.players[s];
					if (player != null && player.Name != null)
						NetMessage.SendData(25, -1, -1, player.Name + " has been kicked by " + sender.Name + ".", 255);
				}
				else
				{
					sender.sendMessage("kick: Slot is vacant.");
				}
			}
			else
			{
				Player player;
				args.ParseOne<Player>(out player);

				if (player.Name == null)
				{
					sender.sendMessage("kick: Error, player has null name.");
					return;
				}

				player.Kick("You have been kicked by " + sender.Name + ".");
				NetMessage.SendData(25, -1, -1, player.Name + " has been kicked by " + sender.Name + ".", 255);
			}
		}

		/// <summary>
		/// Restarts the server
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void Restart(ISender sender, ArgumentList args)
		{
            Server.notifyOps("Restarting the Server {" + sender.Name + "}", true);
			//Statics.keepRunning = true;

			NetPlay.StopServer();
			while (NetPlay.ServerUp) { Thread.Sleep(10); }

			ProgramLog.Log("Starting the Server");
			Main.Initialize();
			WorldIO.LoadWorld(Server.World.SavePath);
			Program.updateThread = new ProgramThread ("Updt", Program.UpdateLoop);
            NetPlay.StartServer();
			//Statics.keepRunning = false;
		}

		/// <summary>
		/// Checks the state of a slot.
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void Slots(ISender sender, ArgumentList args)
		{
			bool dinfo = args.Contains("-d") || args.Contains("-dp") || args.Contains("-pd");
			bool pinfo = args.Contains("-p") || args.Contains("-dp") || args.Contains("-pd");

			int k = 0;
			for (int i = 0; i < 255; i++)
			{
				var slot = NetPlay.slots[i];
				var player = Main.players[i];

				if (slot.state != SlotState.VACANT)
				{
					k += 1;

					var name = "";
					if (player != null)
					{
						name = string.Concat(", ", player.Op ? "Op. " : "", "\"", (player.Name ?? "<null>"), "\"");
						if (player.AuthenticatedAs != null)
						{
							if (player.Name == player.AuthenticatedAs)
								name = name + " (auth'd)";
							else
								name = name + " (auth'd as " + player.AuthenticatedAs + ")";
						}
					}

					var addr = "<secret>";
					if (!(sender is Player && player.Op))
						addr = slot.remoteAddress;

					var msg = String.Format("slot {4}{0}: {1}, {2}{3}", i, slot.state, addr, name, SlotManager.IsPrivileged(i) ? "*" : "");

					if (pinfo && player != null)
					{
						msg += String.Format(", {0}/{1}hp", player.statLife, player.statLifeMax);
					}

					if (dinfo)
					{
						msg += String.Format(", {0}{1}{2}, tx:{3:0.0}K, rx:{4:0.0}K, q:{5}, qm:{6:0.0}KB",
							slot.conn.kicking ? "+" : "-", slot.conn.sending ? "+" : "-", slot.conn.receiving ? "+" : "-",
							slot.conn.BytesSent / 1024.0, slot.conn.BytesReceived / 1024.0,
							slot.conn.QueueLength,
							slot.conn.QueueSize / 1024.0);
					}

					sender.sendMessage(msg);
				}
			}
			sender.sendMessage(String.Format("{0}/{1} slots occupied.", k, SlotManager.MaxSlots));
		}

		/// <summary>
		/// Purge Server data
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void Purge(ISender sender, ArgumentList args)
		{
			var all = args.TryPop("all");
			var something = false;

			if (all || args.TryPop("proj") || args.TryPop("projectiles"))
			{
				something = true;

				ProgramLog.Admin.Log("Purging all projectiles.");

				var msg = NetMessage.PrepareThreadInstance();

				msg.PlayerChat(255, "<Server> Purging all projectiles.", 255, 180, 100);

				lock (Main.updatingProjectiles)
				{
					foreach (var projectile in Main.projectile)
					{
						projectile.Active = false;
						projectile.type = ProjectileType.UNKNOWN;

						msg.Projectile(projectile);
					}

					msg.Broadcast();
				}
			}

			if (all || args.TryPop("npc") || args.TryPop("npcs"))
			{
				something = true;

				ProgramLog.Admin.Log("Purging all NPCs.");

				var msg = NetMessage.PrepareThreadInstance();

				msg.PlayerChat(255, "<Server> Purging all NPCs.", 255, 180, 100);

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

							msg.NPCInfo(npc.whoAmI);
						}
					}

					msg.Broadcast();
				}
			}

			if (all || args.TryPop("item") || args.TryPop("items"))
			{
				something = true;

				ProgramLog.Admin.Log("Purging all items.");

				var msg = NetMessage.PrepareThreadInstance();

				msg.PlayerChat(255, "<Server> Purging all items.", 255, 180, 100);

				lock (Main.updatingItems)
				{
					for (int i = 0; i < 200; i++)
					{
						var item = Main.item[i];
						if (item.Active)
						{
							Main.item[i] = new Item(); // this is what Main does when ignoreErrors is on *shrug*
							msg.ItemInfo(i);
							msg.ItemOwnerInfo(i);
						}
					}

					msg.Broadcast();
				}
			}

			if (!something)
				throw new CommandError("");
		}

		/// <summary>
		/// Lists currently enabled plugins.
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void ListPlugins(ISender sender, ArgumentList args)
		{
            if (PluginManager.PluginCount > 0)
			{
                string plugins = "";

                foreach (var plugin in PluginManager.EnumeratePlugins)
				{
					if (!plugin.IsEnabled || plugin.Name.Trim().Length > 0)
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
        /// Summon a Boss
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public static void SummonBoss(ISender sender, ArgumentList args)
        {
            //Come to think of it now, It may be 1 boss at a time -_-
            bool EoW = args.TryPop("eater");
            bool EyeOC = args.TryPop("eye");
            bool Skeletron = args.TryPop("skeletron");
            bool KingSlime = args.TryPop("kingslime");
            bool NightOverride = args.TryPop("-night");

            Player player = null;
            if (sender is Player)
            {
                player = sender as Player;
            }
            else
            {
                if (NetPlay.anyClients)
                {
                    string PlayerName;
                    if (args.TryParseOne<String>("-player", out PlayerName))
                    {
                        player = Server.GetPlayerByName(PlayerName);
                    }
                    else
                    {
                        //Find Random
                        int plr = Main.rand.Next(0, Networking.ClientConnection.All.Count - 1); //Get Random PLayer
                        player = Main.players[plr];
                    }
                    if (player == null)
                    {
                        throw new CommandError("There was an issue finding the player.");
                    }
                }
                else
                {
                    throw new CommandError("There is no Online Players to spawn near.");
                }
            }

            List<Int32> Bosses = new List<Int32>();
            if (EoW)
            {
                Bosses.Add((int)NPCType.N13_EATER_OF_WORLDS_HEAD);
            }
            if (EyeOC)
            {
                if (Main.dayTime && !NightOverride)
                    throw new CommandError("This boss needs to be summoned in night time, Please override with -night");

                Bosses.Add((int)NPCType.N04_EYE_OF_CTHULU);
            }
            if (Skeletron)
            {
                Bosses.Add((int)NPCType.N35_SKELETRON_HEAD);
            }
            if (KingSlime)
            {
                Bosses.Add((int)NPCType.N50_KING_SLIME);
            }

            if (Bosses.Count > 0)
            {
                if (NightOverride) //Mainly for eye
                {
                    Server.World.setTime(16200.0, false, false);
                    NetMessage.SendData((int)Packet.WORLD_DATA); //Update Data
                }

                foreach (int BossId in Bosses)
                {
                    Vector2 location = World.GetRandomClearTile(((int)player.Position.X / 16), ((int)player.Position.Y / 16), 100, true, 100, 50);
                    int BossSlot = NPC.NewNPC(((int)location.X * 16), ((int)location.Y * 16), BossId);
                    Server.notifyAll(Main.npcs[BossSlot].Name + " has been been summoned by " + sender.Name, ChatColor.Purple, true);
                    if (!(sender is ConsoleSender))
                        ProgramLog.Log("{0} summoned boss {1} at slot {2}.", sender.Name, Main.npcs[BossSlot].Name, BossSlot);
                }
            }
            else
            {
                throw new CommandError("You have not specified a Boss.");
            }            
        }

        public static void ItemRejection(ISender sender, ArgumentList args)
        {
            string exception;
            if (args.TryParseOne<String>("-add", out exception))
            {
                if (!Server.RejectedItems.Contains(exception))
                {
                    Server.RejectedItems.Add(exception);
                    sender.sendMessage(exception + " was added to the Item Rejection list!");
                }
                else
                {
                    throw new CommandError("That item already exists in the list.");
                }
            }
            else if (args.TryParseOne<String>("-remove", out exception))
            {
                if (Server.RejectedItems.Contains(exception))
                {
                    Server.RejectedItems.Remove(exception);
                    sender.sendMessage(exception + " was removed from the Item Rejection list!");
                }
                else
                {
                    throw new CommandError("That item already does not exist in the list.");
                }
            }
            else if (args.TryPop("-clear"))
            {
                Server.RejectedItems.Clear();
                sender.sendMessage("Item Rejection list has been cleared!");
            }
            else
            {
                throw new CommandError("No item/id provided with your command");
            }
            Program.properties.RejectedItems = String.Join(",", Server.RejectedItems);
            Program.properties.Save(false);
        }

        public static void Explosions(ISender sender, ArgumentList args)
        {
            args.ParseNone();

            Server.AllowExplosions = !Server.AllowExplosions;
            Program.properties.AllowExplosions = Server.AllowExplosions;
            Program.properties.Save();

            sender.sendMessage("Explosions are now " + ((Server.AllowExplosions) ? "allowed" : "disabled") + "!");
        }
        
		public static void Refresh (ISender sender, ArgumentList args)
		{
			args.ParseNone ();
			
			var player = sender as Player;
			
			if (player == null)
			{
				sender.Message (255, "This is a player command.");
				return;
			}
			
			if (player.whoAmi < 0) return;
			
			if (! player.Op)
			{
				var diff = DateTime.Now - player.LastCostlyCommand;
				
				if (diff < TimeSpan.FromSeconds (30))
				{
					sender.Message (255, "You must wait {0:0} more seconds before using this command.", 30.0 - diff.TotalSeconds);
					return;
				}
				
				player.LastCostlyCommand = DateTime.Now;
			}
			
			NetMessage.SendTileSquare (player.whoAmi, (int) (player.Position.X/16), (int) (player.Position.Y/16), 32);
		}

        public static void ToggleRPGClients(ISender sender, ArgumentList args)
        {
            Server.AllowTDCMRPG = !Server.AllowTDCMRPG;
            Program.properties.AllowTDCMRPG = Server.AllowTDCMRPG;

            foreach (Player player in Main.players)
            {
                if (player.HasClientMod)
                    NetMessage.SendData(Packet.CLIENT_MOD, player.whoAmi);
            }

            if (!Server.OpList.Save())
            {
                Server.notifyOps("OpList Failed to Save due. {" + sender.Name + "}", true);
                return;
            }

            string message = String.Format("RPG Mode is now {0} on this server:", (Server.AllowTDCMRPG) ? "allowed" : "refused");
            Server.notifyOps(message);
        }
    }
}
