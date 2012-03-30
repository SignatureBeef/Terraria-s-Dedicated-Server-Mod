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
using Terraria_Server.Language;

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
				sender.sendMessage(Languages.PermissionsError, 255, 238, 130, 238);
				return;
			}
			else if (!CommandParser.CheckAccessLevel((AccessLevel)AccessLevel, sender))
			{
				sender.sendMessage(Languages.PermissionsError, 255, 238, 130, 238);
				return;
			}

			args.ParseNone();

			Server.notifyOps(Languages.ExitRequestCommand, false);
			NetPlay.StopServer();
			Statics.Exit = true;

			throw new ExitException(String.Format(sender.Name + Languages.XRequestedShutdown));
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
			Server.notifyOps(Languages.PropertiesReload, true);
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
			var line = String.Concat(Languages.CurrentPlayers, String.Join(", ", players), (players.Count() > 0) ? "." : String.Empty);

			sender.sendMessage(line, 255, 255, 240, 20);
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
				sender.sendMessage(Languages.NoPlayers);
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
			Server.notifyOps(Languages.SavingWorld, true);

			WorldIO.SaveWorld(World.SavePath, false);

			while (WorldModify.saveLock)
				Thread.Sleep(100);

			Server.notifyOps(Languages.SavingData, true);

			Server.BanList.Save();
			Server.WhiteList.Save();
			Server.OpList.Save();

			Server.notifyOps(Languages.SavingComplete, true);
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
							sender.sendMessage(Languages.InvalidPage + ": 0 -> " + (maxPages - 1).ToString());
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
			string Exception, Type = "";
			if (args.TryParseOne<String>(Languages.Add, out Exception))
			{
				Server.WhiteList.addException(Exception);
				Type = Languages.Added;
			}
			else if (args.TryParseOne<String>(Languages.Remove, out Exception))
			{
				Server.WhiteList.removeException(Exception);
				Type = Languages.RemovedFrom;
			}
			else
			{
				sender.sendMessage(Languages.PleaseReview);
				return;
			}

			Server.notifyOps(Exception + " was " + Type + " the Whitelist [" + sender.Name + "]", true);

			if (!Server.WhiteList.Save())
			{
				Server.notifyOps(Languages.WhilelistFailedSave + sender.Name + "'s " + Languages.Command, true);
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
				banee.Kick(Languages.Ban_You);
				Server.BanList.addException(NetPlay.slots[banee.whoAmi].
						remoteAddress.Split(':')[0]);
			}
			else if (!args.TryGetString(0, out playerName))
			{
				throw new CommandError(Languages.IPExpected);
			}

			Server.BanList.addException(playerName);

			Server.notifyOps(playerName + Languages.Ban_Banned + " [" + sender.Name + "]", true);
			if (!Server.BanList.Save())
			{
				Server.notifyOps(Languages.Ban_FailedToSave + sender.Name + "'s " + Languages.Command, true);
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
				throw new CommandError(Languages.IPExpected);
			}

			Server.BanList.removeException(playerName);

			Server.notifyOps(playerName + Languages.Ban_UnBanned + " [" + sender.Name + "]", true);

			if (!Server.BanList.Save())
			{
				Server.notifyOps(Languages.Ban_FailedToSave + sender.Name + "'s " + Languages.Command, true);
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
							World.SetTime(0, false, false);
							break;
						}
					case "noon":
						{
							World.SetTime(27000.0);
							break;
						}
					case "night":
						{
							World.SetTime(16200.0, false, false);
							break;
						}
					case "-now":
						{
							string AP = "AM";
							double time = Main.Time;
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

							sender.sendMessage(Languages.CurrentTime + ": " + Hours + ":" + MinuteString + " " + AP);
							return;
						}
					default:
						{
							sender.sendMessage(Languages.PleaseReview);
							return;
						}
				}
			}
			NetMessage.SendData((int)Packet.WORLD_DATA); //Update Data
			Server.notifyAll(Languages.TimeSet + Main.Time.ToString() + " by " + sender.Name);
		}

		/// <summary>
		/// Gives specified item to the specified player.
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void Give(ISender sender, ArgumentList args)
		{
			// /give <player> <stack> <name> 

			string _prefix;
			args.TryPopAny("-prefix", out _prefix);

			byte prefix;
			if (!Byte.TryParse(_prefix, out prefix))
			{
				Affix affix;
				if (!AffixExtensions.Parse(_prefix ?? String.Empty, out affix, true)) prefix = 0;
				else prefix = (byte)affix;
			}

			Player receiver = args.GetOnlinePlayer(0);
			int stack = args.GetInt(1);
			string NameOrId = args.GetString(2);

			List<ItemInfo> itemlist;
			if (Server.TryFindItemByName(NameOrId, out itemlist) && itemlist.Count > 0)
			{
				if (itemlist.Count > 1)
					throw new CommandError(String.Format(Languages.MoreThanOneItemFoundNameId, itemlist.Count));

				var item = itemlist[0];

				var index = receiver.GiveItem(item.Type, stack, sender, item.NetID, true, prefix);

				if (item.NetID < 0)
					Main.item[index] = Item.netDefaults(item.NetID);

				Main.item[index].Prefix = prefix;
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
					throw new CommandError(String.Format(Languages.MoreThanOneItemFoundNameId, itemlist.Count));
				}

				if (Server.TryFindItemByType(Id, out itemlist) && itemlist.Count > 0)
				{
					if (itemlist.Count > 1)
						throw new CommandError(String.Format(Languages.MoreThanOneItemFoundType, itemlist.Count));

					//receiver.GiveItem(itemlist[0].Type, stack, sender);
					var item = itemlist[0];

					var index = receiver.GiveItem(item.Type, stack, sender, item.NetID, true, prefix);

					if (item.NetID < 0)
						Main.item[index] = Item.netDefaults(item.NetID);

					Main.item[index].Prefix = prefix;
				}
				else
				{
					throw new CommandError(String.Format(Languages.MoreThanOneItemFoundNameId, "no"));
				}
			}
		}

		/// <summary>
		/// Spawns specified NPC type.
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		/// <remarks>This function also allows NPC custom health.</remarks>
		public static void SpawnNPC(ISender sender, ArgumentList args)
		{
			if (Main.stopSpawns && !Program.properties.NPCSpawnsOverride)
				throw new CommandError("NPC Spawing is disabled.");

			var health = -1;
			var customHealth = args.TryPopAny<Int32>("-health", out health);

			Player player = sender as Player;
			int NPCAmount;
			if (args.Count > 3)
				throw new CommandError(Languages.TooManyArguments);
			else if (sender is ConsoleSender && args.Count <= 2)
			{
				if (!NetPlay.anyClients || !Server.TryGetFirstOnlinePlayer(out player))
					throw new CommandError(Languages.NobodyOnline);
			}
			else if (args.Count == 3)
				player = args.GetOnlinePlayer(2);

			var npcName = args.GetString(1).ToLower().Trim();

			// Get the class id of the npc
			int realNPCId = 0;
			NPC fclass = Registries.NPC.FindClass(npcName);
			if (fclass.Name != String.Empty)
				realNPCId = fclass.Type;
			else
				throw new CommandError(Languages.NPCDoesntExist);

			try
			{
				NPCAmount = args.GetInt(0);
				if (NPCAmount > Program.properties.SpawnNPCMax && sender is Player)
				{
					(sender as Player).Kick(Languages.DontSpawnThatMany);
					return;
				}
			}
			catch
			{
				throw new CommandError(Languages.ExpectedSpawnInteger);
			}

			string realNPCName = String.Empty;
			for (int i = 0; i < NPCAmount; i++)
			{
				Vector2 location = World.GetRandomClearTile(((int)player.Position.X / 16), ((int)player.Position.Y / 16), 100, true, 100, 50);
				int npcIndex = NPC.NewNPC(((int)location.X * 16), ((int)location.Y * 16), fclass.Name, 0, Main.SpawnsOverride);

				if (customHealth)
				{
					Main.npcs[npcIndex].life = health;
					Main.npcs[npcIndex].lifeMax = health;
				}

				realNPCName = Main.npcs[npcIndex].Name;
			}
			Server.notifyOps("Spawned " + NPCAmount.ToString() + " of " +
					realNPCName + " [" + player.Name + "]", true);
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

			if (!args.TryPopOne(out subject))
			{
				subject = sender as Player;
				if (subject == null)
				{
					sender.sendMessage(Languages.NeedTeleportTarget);
					return;
				}

				if (args.Count == 0)
				{
					if (subject.Teleport(Main.spawnTileX, Main.spawnTileY))
					{
						Server.notifyOps(String.Format(Languages.TeleportedToSpawn, subject.Name), true);
					}
					else
						sender.sendMessage(Languages.TeleportFailed);
					return;
				}
			}
			else if (args.Count == 0)
			{
				target = subject;

				subject = sender as Player;
				if (subject == null)
				{
					sender.sendMessage(Languages.NeedTeleportTarget);
					return;
				}

				if (subject.Teleport(target))
				{

					Server.notifyOps(string.Concat("Teleported ", subject.Name, " to ",
						target.Name, ". [", sender.Name, "]"), true);
				}
				else
					sender.sendMessage(Languages.TeleportFailed);
				return;
			}

			int x;
			int y;

			if (args.Count == 1)
			{
				if (args.TryParseOne(out target))
				{
					if (subject.Teleport(target))
					{
						Server.notifyOps(string.Concat("Teleported ", subject.Name, " to ",
							target.Name, ". [", sender.Name, "]"), true);
					}
					else
						sender.sendMessage(Languages.TeleportFailed);
				}
				else
					sender.sendMessage(Languages.PlayerNotFound);
				return;
			}
			else if (args.Count == 2)
			{
				if (args.TryParseTwo(out x, out y))
				{
					if (x < 0 || x >= Main.maxTilesX || y < 0 || y >= Main.maxTilesY)
					{
						sender.sendMessage(String.Format("Coordinates out of range of (0, {0}); (0, {1}).", Main.maxTilesX - 1, Main.maxTilesY - 1));
						return;
					}

					if (subject.Teleport(x, y))
					{
						Server.notifyOps(string.Concat("Teleported ", subject.Name, " to ",
							x, ":", y, ". [", sender.Name, "]"), true);
					}
					else
						sender.sendMessage(Languages.TeleportFailed);
				}
				else
					throw new CommandError(Languages.InvalidCoords);
				return;
			}

			throw new CommandError("");
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
						sender.sendMessage(Languages.CouldNotFindPlayer);
						return;
					}

					subject.Teleport(player);

					Server.notifyOps("Teleported " + subject.Name + " to " +
						player.Name + " [" + sender.Name + "]", true);
				}
			}
			else
			{
				throw new CommandError(Languages.OnlyPlayerCanUseCommand);
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
				sender.sendMessage(Languages.SettlingLiquids);
				Liquid.StartPanic();
				sender.sendMessage(Languages.Complete);
			}
			else
			{
				sender.sendMessage(Languages.LiquidsAlreadySettled);
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

				player.sendMessage(Languages.YouAreNowOP, ChatColor.Green);
				player.Op = true;
				if (player.HasClientMod)
				{
					NetMessage.SendData(Packet.CLIENT_MOD, player.whoAmi);
				}
			}

			Server.notifyOps("Opping " + playerName + " [" + sender.Name + "]", true);
			Server.OpList.addException(playerName + ":" + password, true, playerName.Length + 1);

			if (!Server.OpList.Save())
			{
				Server.notifyOps(Languages.OPlistFailedSave + " [" + sender.Name + "]", true);
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
					player.sendMessage(Languages.YouAreNowDeop, ChatColor.Green);
				}

				player.Op = false;
				if (player.HasClientMod)
				{
					NetMessage.SendData(Packet.CLIENT_MOD, player.whoAmi);
				}
			}

			if (Player.isInOpList(playerName))
			{
				Server.notifyOps("De-Opping " + playerName + " [" + sender.Name + "]", true);
				Server.OpList.removeException(playerName + ":" + Player.GetPlayerPassword(playerName));
			}

			if (!Server.OpList.Save())
			{
				Server.notifyOps(Languages.OPlistFailedSave + " [" + sender.Name + "]", true);
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
						Server.notifyOps(
							String.Format("{0} " + Languages.SuccessfullyLoggedInOP, player.Name)
						);
						player.Op = true;
						player.sendMessage(Languages.SuccessfullyLoggedInOP, ChatColor.DarkGreen);

						if (player.HasClientMod)
						{
							NetMessage.SendData(Packet.CLIENT_MOD, player.whoAmi);
						}
					}
					else
					{
						Server.notifyOps(
							String.Format("{0} " + Languages.FailedLoginWrongPassword, player.Name)
						);
						player.sendMessage(Languages.IncorrectOPPassword, ChatColor.DarkRed);
					}
				}
				else
				{
					player.sendMessage(Languages.YouNeedPrivileges, ChatColor.DarkRed);
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
					player.sendMessage(Languages.SuccessfullyLoggedOutOP, ChatColor.DarkRed);

					Server.notifyOps(
						String.Format("{0} " + Languages.SuccessfullyLoggedOutOP, player.Name)
					);

					if (player.HasClientMod)
					{
						NetMessage.SendData(Packet.CLIENT_MOD, player.whoAmi);
					}
				}
				else
				{
					player.sendMessage(Languages.YouNeedPrivileges, ChatColor.DarkRed);
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
			Main.stopSpawns = !Main.stopSpawns;
			sender.sendMessage(Languages.NPCSpawningIsNow + ((Main.stopSpawns) ? "off" : "on") + "!");

			Program.properties.StopNPCSpawning = Main.stopSpawns;
			Program.properties.Save(false);

			if (args.TryPop("-clear"))
				Purge(sender, new ArgumentList() { "all" });
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
					slot.Kick(Languages.YouHaveBeenKickedBy + sender.Name + ".");

					var player = Main.players[s];
					if (player != null && player.Name != null)
						NetMessage.SendData(25, -1, -1, player.Name + Languages.HasBeenKickedBy + sender.Name + ".", 255);
				}
				else
				{
					sender.sendMessage(Languages.KickSlotIsEmpty);
				}
			}
			else
			{
				Player player;
				args.ParseOne<Player>(out player);

				if (player.Name == null)
				{
					sender.sendMessage(Languages.KickPlayerNameNull);
					return;
				}

				player.Kick(Languages.YouHaveBeenKickedBy + sender.Name + ".");
				NetMessage.SendData(25, -1, -1, player.Name + Languages.HasBeenKickedBy + sender.Name + ".", 255);
			}
		}

		/// <summary>
		/// Restarts the server
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void Restart(ISender sender, ArgumentList args)
		{
			Program.Restarting = true;
			Server.notifyOps(Languages.RestartingServer + " [" + sender.Name + "]", true);
			//Statics.keepRunning = true;

			NetPlay.StopServer();
			while (NetPlay.ServerUp) { Thread.Sleep(10); }

			ProgramLog.Log(Languages.StartingServer);
			Main.Initialize();

			Program.LoadPlugins();

			WorldIO.LoadWorld(null, null, World.SavePath);
			Program.updateThread = new ProgramThread("Updt", Program.UpdateLoop);
			NetPlay.StartServer();

			while (!NetPlay.ServerUp) { Thread.Sleep(100); }

			ProgramLog.Console.Print(Languages.Startup_YouCanNowInsertCommands);
			Program.Restarting = false;
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

				ProgramLog.Admin.Log(Languages.PurgingProjectiles);

				var msg = NetMessage.PrepareThreadInstance();

				msg.PlayerChat(255, "<Server> " + Languages.PurgingProjectiles, 255, 180, 100);

				lock (Main.updatingProjectiles)
				{
					foreach (var projectile in Main.projectile)
					{
						projectile.Active = false;
						projectile.type = ProjectileType.N0_UNKNOWN;

						msg.Projectile(projectile);
					}

					msg.Broadcast();
				}
			}

			if (all || args.TryPop("npc") || args.TryPop("npcs"))
			{
				something = true;

				ProgramLog.Admin.Log(Languages.PurgingNPC);

				var msg = NetMessage.PrepareThreadInstance();

				msg.PlayerChat(255, "<Server> " + Languages.PurgingNPC, 255, 180, 100);

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

				ProgramLog.Admin.Log(Languages.PurgingItems);

				var msg = NetMessage.PrepareThreadInstance();

				msg.PlayerChat(255, "<Server> " + Languages.PurgingItems, 255, 180, 100);

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
				sender.sendMessage(Languages.LoadedPlugins + plugins + ".");
			}
			else
			{
				sender.sendMessage(Languages.NoPluginsLoaded);
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
			bool Twins = args.TryPop("twins");
			bool Wof = args.TryPop("wof");
			bool Destroyer = args.TryPop("destroyer");
			bool Prime = args.TryPop("prime");
			bool All = args.TryPop("-all");
			bool NightOverride = args.TryPop("-night");

			Player player = null;
			if (sender is Player) player = sender as Player;
			else if (NetPlay.anyClients)
			{
				string PlayerName;
				if (args.TryParseOne<String>("-player", out PlayerName))
					player = Server.GetPlayerByName(PlayerName);
				else
				{
					//Find Random
					int plr = Main.rand.Next(0, Networking.ClientConnection.All.Count - 1); //Get Random PLayer
					player = Main.players[plr];
				}

				if (player == null)
					throw new CommandError(Languages.IssueFindingPlayer);
			}
			else
				throw new CommandError(Languages.NoOnlinePlayersToSpawnNear);

			List<Int32> bosses = new List<Int32>();

			if (EyeOC || Twins || All || Prime || Skeletron | Destroyer)
			{
				if (Main.dayTime && !NightOverride)
					throw new CommandError(Languages.NeedsToBeNightTime);
			}

			var wofSummoned = NPC.IsNPCSummoned((int)NPCType.N113_WALL_OF_FLESH);
			if (Wof && wofSummoned)
				sender.sendMessage("Wall of Flesh already summoned, Ignoring.");

			if (EyeOC || All) bosses.Add((int)NPCType.N04_EYE_OF_CTHULHU);
			if (Skeletron || All) bosses.Add((int)NPCType.N35_SKELETRON_HEAD);
			if (KingSlime || All) bosses.Add((int)NPCType.N50_KING_SLIME);
			if (EoW || All) bosses.Add((int)NPCType.N13_EATER_OF_WORLDS_HEAD);
			if (Twins || All) { bosses.Add((int)NPCType.N125_RETINAZER); bosses.Add((int)NPCType.N126_SPAZMATISM); }
			if ((Wof || All) && !wofSummoned) bosses.Add((int)NPCType.N113_WALL_OF_FLESH);
			if (Destroyer || All) bosses.Add((int)NPCType.N134_THE_DESTROYER);
			if (Prime || All) bosses.Add((int)NPCType.N127_SKELETRON_PRIME);

			if (bosses.Count > 0)
			{
				if (NightOverride)
				{
					World.SetTime(16200.0, false, false);
					NetMessage.SendData((int)Packet.WORLD_DATA); //Update Data
				}

				foreach (int bossId in bosses)
				{
					//Vector2 location = World.GetRandomClearTile(((int)player.Position.X / 16), ((int)player.Position.Y / 16), 100, true, 100, 50);
					//int BossSlot = NPC.NewNPC(((int)location.X * 16), ((int)location.Y * 16), BossId);

					//var npc = Main.npcs[BossSlot];
					//var name = npc.Name;

					//if (!String.IsNullOrEmpty(npc.DisplayName))
					//    name = npc.DisplayName;

					//npc.TargetClosest(true);

					//Server.notifyAll(name + Languages.BossSummonedBy + sender.Name, ChatColor.Purple, true);

					//if (!(sender is ConsoleSender))
					//    ProgramLog.Log("{0} summoned boss {1} at slot {2}.", sender.Name, name, BossSlot);

					NPC.SpawnOnPlayer(player.whoAmi, bossId, Main.SpawnsOverride);
				}
			}
			else
			{
				throw new CommandError(Languages.BossNotSpecified);
			}
		}

		/// <summary>
		/// Disallow an item from the server
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void ItemRejection(ISender sender, ArgumentList args)
		{
			string exception;
			if (args.TryParseOne<String>(Languages.Add, out exception))
			{
				if (!Server.RejectedItems.Contains(exception))
				{
					Server.RejectedItems.Add(exception);
					sender.sendMessage(exception + Languages.ItemRejection_Added);
				}
				else
				{
					throw new CommandError(Languages.ItemRejection_ItemExists);
				}
			}
			else if (args.TryParseOne<String>(Languages.Remove, out exception))
			{
				if (Server.RejectedItems.Contains(exception))
				{
					Server.RejectedItems.Remove(exception);
					sender.sendMessage(exception + Languages.ItemRejection_Removed);
				}
				else
				{
					throw new CommandError(Languages.ItemRejection_ItemDoesntExist);
				}
			}
			else if (args.TryPop(Languages.Clear))
			{
				Server.RejectedItems.Clear();
				sender.sendMessage(Languages.ItemRejection_Removed);
			}
			else
			{
				throw new CommandError(Languages.NoItemIDNameProvided);
			}
			Program.properties.RejectedItems = String.Join(",", Server.RejectedItems);
			Program.properties.Save(false);
		}

		/// <summary>
		/// Toggle whether the server allows explosions
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void Explosions(ISender sender, ArgumentList args)
		{
			args.ParseNone();

			Server.AllowExplosions = !Server.AllowExplosions;
			Program.properties.AllowExplosions = Server.AllowExplosions;
			Program.properties.Save();

			sender.sendMessage(Languages.ExplosionsAreNow + (Server.AllowExplosions ? "allowed" : "disabled") + "!");
		}

		/// <summary>
		/// Refreshes a players area
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void Refresh(ISender sender, ArgumentList args)
		{
			args.ParseNone();

			var player = sender as Player;

			if (player == null)
			{
				sender.Message(255, Languages.ThisIsPlayerCommand);
				return;
			}

			if (player.whoAmi < 0) return;

			if (!player.Op)
			{
				var diff = DateTime.Now - player.LastCostlyCommand;

				if (diff < TimeSpan.FromSeconds(30))
				{
					sender.Message(255, Languages.YouMustWaitBeforeAnotherCommand, 30.0 - diff.TotalSeconds);
					return;
				}

				player.LastCostlyCommand = DateTime.Now;
			}

			NetMessage.SendTileSquare(player.whoAmi, (int)(player.Position.X / 16), (int)(player.Position.Y / 16), 32);
		}

		/// <summary>
		/// Toggles whether the server allows RPG.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
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
				Server.notifyOps(Languages.OPlistFailedSave + " [" + sender.Name + "]", true);
				return;
			}

			string message = (Server.AllowTDCMRPG) ? Languages.RPGMode_Allowed : Languages.RPGMode_Refused;
			Server.notifyOps(message);
		}

		/// <summary>
		/// Spawns the TDCM Quest Giver
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void SpawnQuestGiver(ISender sender, ArgumentList args)
		{
			if (!Server.AllowTDCMRPG)
				throw new CommandError(Languages.CannotQuestGiverWithoutTDCM);

			int npcId;
			if (NPC.TryFindNPCByName(Statics.TDCM_QUEST_GIVER, out npcId))
				throw new CommandError(Languages.QuestGiverAlreadySpawned);

			NPC.SpawnTDCMQuestGiver();
		}

		/// <summary>
		/// Enables hardmode
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void HardMode(ISender sender, ArgumentList args)
		{
			args.ParseNone();

			if (Main.hardMode)
				throw new CommandError(Languages.HardModeAlreadyEnabled);

			sender.sendMessage(Languages.StartingHardMode);
			WorldModify.StartHardMode();
		}

		/// <summary>
		/// Reloads the language definitions
		/// </summary>
		/// <param name="sender">Sending player</param>
		/// <param name="args">Arguments sent with command</param>
		public static void LanguageReload(ISender sender, ArgumentList args)
		{
			args.ParseNone();

			sender.sendMessage("Reloading Language File...");
			sender.sendMessage("Reloading " + (Languages.LoadClass(Collections.Registries.LANGUAGE_FILE) ? "Succeeded" : "Failed"));
		}

		/// <summary>
		/// Allows a user to take backups and purge old data
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public static void Backups(ISender sender, ArgumentList args)
		{
			var perform = args.TryPop("now");
			var purge = args.TryPop("purge");

			if (perform)
				BackupManager.PerformBackup();
			else if (purge)
			{
				int minutes;
				if (args.TryParseOne<Int32>(out minutes))
				{
					var backups = BackupManager.GetBackupsBefore(Main.worldName, DateTime.Now.AddMinutes(-minutes));

					var failCount = 0;
					foreach (var backup in backups)
						try
						{
							File.Delete(backup);
						}
						catch { failCount++; }

					if (failCount > 0)
						sender.sendMessage(
							String.Format("Failed to deleted {0} backup(s).", failCount)
						);
					else
						sender.sendMessage(
							String.Format("Deleted {0} backup(s).", backups.Length - failCount)
						);
				}
				else
					throw new CommandError("Please specify a time frame.");
			}
			else
				throw new CommandError("Argument expected.");
		}

		/// <summary>
		/// Allows an OP to force the time to dtay at a certain point.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public static void Timelock(ISender sender, ArgumentList args)
		{
			var disable = args.TryPop("disable");
			var setNow = args.TryPop("now");
			var setMode = args.TryPop("set");
			var setAt = args.TryPop("setat");

			if (disable)
			{
				if (!Main.UseTimeLock) { sender.sendMessage("Time lock is already disabled", 255, 255, 0, 0); return; }

				Main.UseTimeLock = false;
				sender.sendMessage("Time lock has been disabled.", 255, 0, 255, 0);
				return;
			}
			else if (setNow) Main.UseTimeLock = true;
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
							World.SetTime(0, false, false);
							break;
						}
					case "noon":
						{
							World.SetTime(27000.0);
							break;
						}
					case "night":
						{
							World.SetTime(16200.0, false, false);
							break;
						}
					default:
						{
							sender.sendMessage(Languages.PleaseReview, 255, 255, 0, 0);
							return;
						}
				}
				Main.UseTimeLock = true;
			}
			else if (setAt)
			{
				double time;
				if (args.TryParseOne<Double>(out time))
				{
					Main.Time = time;
					Main.UseTimeLock = true;
				}
				else throw new CommandError("Double expected.");
			}
			else throw new CommandError("Certain arguments expected.");

			if (Main.UseTimeLock)
			{
				if (!setNow) NetMessage.SendData(Packet.WORLD_DATA);

				sender.sendMessage(
					String.Format("Time lock has set at {0}.", Main.Time),
					255, 0, 255, 0
				);
			}
		}
	}
}
