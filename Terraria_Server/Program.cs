using System.Threading;
using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

using Terraria_Server.Commands;
using Terraria_Server.Definitions;
using Terraria_Server.Logging;
using Terraria_Server.WorldMod;
using System.Security.Policy;

namespace Terraria_Server
{
	public class Program
	{
        public const string VERSION_NUMBER = "v1.0.6.1";

		public static ProgramThread updateThread = null;
		public static ServerProperties properties = null;
		public static CommandParser commandParser = null;
		public static TConsole tConsole = null;

		public static Server server;

        public static void Main(string[] args)
		{
			Thread.CurrentThread.Name = "Main";
            try
            {
                string MODInfo = "Terraria's Dedicated Server Mod. (" + VERSION_NUMBER + " {" + Statics.CURRENT_TERRARIA_RELEASE + "}) #"
					+ Statics.BUILD;
				try
				{
					Console.Title = MODInfo;
				}
				catch
				{

				}

				var lis = new Logging.LogTraceListener ();
				System.Diagnostics.Trace.Listeners.Clear ();
				System.Diagnostics.Trace.Listeners.Add (lis);
				System.Diagnostics.Debug.Listeners.Clear ();
				System.Diagnostics.Debug.Listeners.Add (lis);
				
				ProgramLog.Log ("Initializing " + MODInfo);

				ProgramLog.Log ("Setting up Paths.");
				if (!SetupPaths())
				{
					return;
				}
				
				Platform.InitPlatform();
				tConsole = new TConsole (null, Platform.Type); //dummy

				ProgramLog.Log ("Setting up Properties.");
				bool propertiesExist = File.Exists("server.properties");
				SetupProperties();

				if (!propertiesExist)
				{
					ProgramLog.Console.Print ("New properties file created. Would you like to exit for editing? [Y/n]: ");
					if (Console.ReadLine().ToLower() == "y")
					{
						ProgramLog.Console.Print ("Complete, Press any Key to Exit...");
						Console.ReadKey(true);
						return;
					}
				}
				
				var logFile = Statics.DataPath + Path.DirectorySeparatorChar + "server.log";
				ProgramLog.OpenLogFile (logFile);

                string PIDFile = properties.PIDFile.Trim();
				if (PIDFile.Length > 0)
				{
                    string ProcessUID = Process.GetCurrentProcess().Id.ToString();
					bool Issue = false;
					if (File.Exists(PIDFile))
					{
						try
						{
							File.Delete(PIDFile);
						}
						catch (Exception)
						{
							ProgramLog.Console.Print ("Issue deleting PID file, Continue? [Y/n]: ");
							if (Console.ReadLine().ToLower() == "n")
							{
								ProgramLog.Console.Print ("Press any Key to Exit...");
								Console.ReadKey(true);
								return;
							}
							Issue = true;
						}
					}
					if (!Issue)
					{
						try
						{
							File.WriteAllText(PIDFile, ProcessUID);
						}
						catch (Exception)
						{
							ProgramLog.Console.Print ("Issue creating PID file, Continue? [Y/n]: ");
							if (Console.ReadLine().ToLower() == "n")
							{
								ProgramLog.Console.Print ("Press any Key to Exit...");
								Console.ReadKey(true);
								return;
							}
						}
						ProgramLog.Log ("PID File Created, Process ID: " + ProcessUID);
					}
				}

				ParseArgs(args);

				try
				{
					if (UpdateManager.performProcess())
					{
						ProgramLog.Log ("Restarting into new update!");
						return;
					}
				}
				catch (UpdateCompleted)
				{
					throw;
				}
				catch (Exception e)
				{
					ProgramLog.Log (e, "Error updating");
				}
				
				LoadMonitor.Start ();
				
				ProgramLog.Log ("Starting remote console server");
				RemoteConsole.RConServer.Start ("rcon_logins.properties");
				
				ProgramLog.Log ("Preparing Server Data...");
				
				using (var prog = new ProgressLogger (1, "Loading item definitions"))
					Collections.Registries.Item.Load ();
				using (var prog = new ProgressLogger (1, "Loading NPC definitions"))
					Collections.Registries.NPC.Load (Collections.Registries.NPC_FILE);
				using (var prog = new ProgressLogger (1, "Loading projectile definitions"))
					Collections.Registries.Projectile.Load (Collections.Registries.PROJECTILE_FILE);

                string worldFile = properties.WorldPath;
				FileInfo file = new FileInfo(worldFile);

				if (!file.Exists)
				{
					try
					{
						file.Directory.Create();
					}
					catch (Exception exception)
					{
						ProgramLog.Log (exception);
						ProgramLog.Console.Print ("Press any key to continue...");
						Console.ReadKey(true);
						return;
					}
					ProgramLog.Log ("Generating World '{0}'", worldFile);

                    string seed = properties.Seed;
					if (seed == "-1")
					{
						seed = new Random().Next(100).ToString();
						ProgramLog.Log ("Generated seed: {0}", seed);
					}

					int worldX = properties.getMapSizes()[0];
					int worldY = properties.getMapSizes()[1];
					if (properties.UseCustomTiles)
					{
						int X = properties.MaxTilesX;
						int Y = properties.MaxTilesY;
						if (X > 0 && Y > 0)
						{
							worldX = X;
							worldY = Y;
						}

						if (worldX < (int)World.MAP_SIZE.SMALL_X || worldY < (int)World.MAP_SIZE.SMALL_Y)
						{
							ProgramLog.Log ("World dimensions need to be equal to or larger than {0} by {1}; using built-in 'small'", (int)World.MAP_SIZE.SMALL_X, (int)World.MAP_SIZE.SMALL_Y);
							worldX = (int)((int)World.MAP_SIZE.SMALL_Y * 3.5);
							worldY = (int)World.MAP_SIZE.SMALL_Y;
						}

						ProgramLog.Log ("Generating world with custom map size: {0}x{1}", worldX, worldY);
					}

					Server.maxTilesX = worldX;
					Server.maxTilesY = worldY;

					WorldIO.clearWorld();
					(new Server()).Initialize();
					if (properties.UseCustomGenOpts)
					{
						WorldGen.numDungeons = properties.DungeonAmount;
						WorldModify.ficount = properties.FloatingIslandAmount;
					}
					else
					{
						WorldGen.numDungeons = 1;
						WorldModify.ficount = (int)((double)Server.maxTilesX * 0.0008); //The Statics one was generating with default values, We want it to use the actual tileX for the world
					}
                    WorldGen.GenerateWorld(seed);
					WorldIO.saveWorld(worldFile, true);
				}
				
				// TODO: read map size from world file instead of config
				int worldXtiles = properties.getMapSizes()[0];
				int worldYtiles = properties.getMapSizes()[1];

				if (properties.UseCustomTiles)
				{
					int X = properties.MaxTilesX;
					int Y = properties.MaxTilesY;
					if (X > 0 && Y > 0)
					{
						worldXtiles = X;
						worldYtiles = Y;
					}

					if (worldXtiles < (int)World.MAP_SIZE.SMALL_X || worldYtiles < (int)World.MAP_SIZE.SMALL_Y)
					{
						ProgramLog.Log ("World dimensions need to be equal to or larger than {0} by {1}; using built-in 'small'", (int)World.MAP_SIZE.SMALL_X, (int)World.MAP_SIZE.SMALL_Y);
						worldXtiles = (int)((int)World.MAP_SIZE.SMALL_Y * 3.5);
						worldYtiles = (int)World.MAP_SIZE.SMALL_Y;
					}

					ProgramLog.Log ("Using world with custom map size: {0}x{1}", worldXtiles, worldYtiles);
				}

				World world = new World(worldXtiles, worldYtiles);
				world.SavePath = worldFile;

				server = new Server(world, properties.MaxPlayers,
					Statics.DataPath + Path.DirectorySeparatorChar + "whitelist.txt",
					Statics.DataPath + Path.DirectorySeparatorChar + "banlist.txt",
					Statics.DataPath + Path.DirectorySeparatorChar + "oplist.txt");
				server.OpPassword = properties.Password;
				server.Port = properties.Port;
				server.ServerIP = properties.ServerIP;
				server.Initialize();
				
				Server.maxTilesX = worldXtiles;
				Server.maxTilesY = worldYtiles;
				Server.maxSectionsX = worldXtiles / 200;
				Server.maxSectionsY = worldYtiles / 150;

                WorldIO.loadWorld();

				updateThread = new ProgramThread ("Updt", Program.UpdateLoop);

				ProgramLog.Log ("Starting the Server");
				server.StartServer();
				
				Statics.IsActive = true;
				while (!Statics.serverStarted) { }

                commandParser = new CommandParser(server);
				ProgramLog.Console.Print ("You can now insert Commands.");
				
				while (Statics.IsActive)
				{
					try
					{
                        string line = Console.ReadLine().Trim();
						if (line.Length > 0)
						{
							commandParser.ParseConsoleCommand(line, server);
						}
					}
					catch (Exception e)
					{
						ProgramLog.Log (e, "Issue parsing console command");
					}
				}
				while (Statics.serverStarted) { Thread.Sleep(10); }
				ProgramLog.Log ("Exiting...");
				Program.tConsole.Close();
            }
            catch (UpdateCompleted)
            {
            }
            catch (Exception e)
            {
                try
                {
                    using (StreamWriter streamWriter = new StreamWriter(Statics.DataPath + Path.DirectorySeparatorChar + "crashlog.txt", true))
                    {
                        streamWriter.WriteLine(DateTime.Now);
                        streamWriter.WriteLine("Crash Log Generated by TDSM #" + Statics.BUILD + " for " + //+ " r" + Statics.revision + " for " +
                            VERSION_NUMBER + " {" + Statics.CURRENT_TERRARIA_RELEASE + "}");
                        streamWriter.WriteLine(e);
                        streamWriter.WriteLine("");
                    }
                    ProgramLog.Log(e, "Program crash");
                    ProgramLog.Log("Please send crashlog.txt to http://tdsm.org/");
                }
                catch
                {
                }
            }

            if (File.Exists(properties.PIDFile.Trim()))
            {
                File.Delete(properties.PIDFile.Trim());
            }

			if (Program.tConsole != null)
			{
				Program.tConsole.Close();
			}
			ProgramLog.Log ("Log end.");
			ProgramLog.Close();
			
			RemoteConsole.RConServer.Stop ();
		}

		private static bool SetupPaths()
		{
            try
            {
                CreateDirectory(Statics.WorldPath);
                CreateDirectory(Statics.PluginPath);
                CreateDirectory(Statics.DataPath);
                CreateDirectory(Statics.LibrariesPath);

                AppDomain.CurrentDomain.AppendPrivatePath(Statics.LibrariesPath); //For Mono, The config setting doesn't fucking work.
            }
            catch (Exception exception)
            {
                ProgramLog.Log(exception);
                ProgramLog.Log("Press any key to continue...");
                Console.ReadKey(true);
                return false;
            }

			CreateFile(Statics.DataPath + Path.DirectorySeparatorChar + "whitelist.txt");
			CreateFile(Statics.DataPath + Path.DirectorySeparatorChar + "banlist.txt");
			CreateFile(Statics.DataPath + Path.DirectorySeparatorChar + "oplist.txt");
			CreateFile(Statics.DataPath + Path.DirectorySeparatorChar + "server.log");
			return true;
		}

        private static void CreateDirectory(string dirPath)
		{
			if (!Directory.Exists(dirPath))
			{
				Directory.CreateDirectory(dirPath);
			}
		}

        private static bool CreateFile(string filePath)
		{
			if (!File.Exists(filePath))
			{
				try
				{
					File.Create(filePath).Close();
				}
				catch (Exception exception)
				{
					ProgramLog.Log (exception);
					ProgramLog.Log ("Press any key to continue...");
					Console.ReadKey(true);
					return false;
				}
			}
			return true;
		}

		private static void SetupProperties()
		{
			properties = new ServerProperties("server.properties");
			properties.Load();
			properties.pushData();
			properties.Save();
		}

        public static void ParseArgs(string[] args)
		{
			if (args != null && args.Length > 0)
			{
				for (int i = 0; i < args.Length; i++)
				{
					if (i == (args.Length - 1)) { break; }
                    string commandMessage = args[i].ToLower().Trim();
					// 0 for Ops
					if (commandMessage.Equals("-ignoremessages:0"))
					{
						Statics.cmdMessages = false;
					}
					else if (commandMessage.Equals("-maxplayers"))
					{
						try
						{
							properties.MaxPlayers = Convert.ToInt32(args[i + 1]);
						}
						catch
						{

						}
					}
					else if (commandMessage.Equals("-ip"))
					{
						properties.ServerIP = args[i + 1];
					}
					else if (commandMessage.Equals("-port"))
					{
						try
						{
							properties.Port = Convert.ToInt32(args[i + 1]);
						}
						catch
						{

						}
					}
					else if (commandMessage.Equals("-greeting"))
					{
						properties.Greeting = args[i + 1];
					}
					else if (commandMessage.Equals("-worldpath"))
					{
						properties.WorldPath = args[i + 1];
					}
					else if (commandMessage.Equals("-password"))
					{
						properties.Password = args[i + 1];
					}
					else if (commandMessage.Equals("-allowupdates"))
					{
						try
						{
							properties.AutomaticUpdates = Convert.ToBoolean(args[i + 1]);
						}
						catch
						{

						}
					}
					else if (commandMessage.Equals("-npcdoorcancel"))
					{
						try
						{
							properties.NPCDoorOpenCancel = Convert.ToBoolean(args[i + 1]);
						}
						catch
						{

						}
					}
					else if (commandMessage.Equals("-seed"))
					{
						try
						{
							properties.Seed = args[i + 1];
						}
						catch (Exception)
						{

						}
					}
					else if (commandMessage.Equals("-mapsize"))
					{
						properties.MapSize = args[i + 1];
					}
					else if (commandMessage.Equals("-usecustomtiles"))
					{
						try
						{
							properties.UseCustomTiles = Convert.ToBoolean(args[i + 1]);
						}
						catch
						{

						}
					}
					else if (commandMessage.Equals("-maxtilesx"))
					{
						try
						{
							properties.MaxTilesX = Convert.ToInt32(args[i + 1]);
						}
						catch
						{

						}
					}
					else if (commandMessage.Equals("-maxtilesy"))
					{
						try
						{
							properties.MaxTilesY = Convert.ToInt32(args[i + 1]);
						}
						catch
						{

						}
					}
					else if (commandMessage.Equals("-numdungeons"))
					{
						try
						{
							properties.DungeonAmount = Convert.ToInt32(args[i + 1]);
						}
						catch
						{

						}
					}
					else if (commandMessage.Equals("-customworldgen"))
					{
						try
						{
							properties.UseCustomGenOpts = Convert.ToBoolean(args[i + 1]);
						}
						catch
						{

						}
					}
					else if (commandMessage.Equals("-numislands"))
					{
						try
						{
							properties.FloatingIslandAmount = Convert.ToInt32(args[i + 1]);
						}
						catch
						{

						}
					}
					else if (commandMessage.Equals("-whitelist"))
					{
						try
						{
							properties.UseWhiteList = Convert.ToBoolean(args[i + 1]);
						}
						catch
						{

						}
					}
					else if (commandMessage.Equals("-pidfile"))
					{
						try
						{
							properties.PIDFile = args[i + 1];
						}
						catch
						{

						}
					}
					else if (commandMessage.Equals("-simpleloop"))
					{
						try
						{
							properties.SimpleLoop = Convert.ToBoolean(args[i + 1]);
						}
						catch
						{

						}
					}
					else if (commandMessage.Equals("-windowsoutput"))
					{
						Platform.Type = Platform.PlatformType.WINDOWS;
						try
						{
							bool windows = Convert.ToBoolean(args[i + 1]);
							if (!windows)
							{
								Platform.Type = Platform.PlatformType.LINUX;
							}
						}
						catch
						{

						}
					}
					else if (commandMessage.Equals("-hackeddata"))
					{
						try
						{
							properties.HackedData = Convert.ToBoolean(args[i + 1]);
						}
						catch
						{

						}
					}
					else if (commandMessage.Equals("-rconip"))
					{
						try
						{
							properties.RConBindAddress = args[i + 1];
						}
						catch
						{

						}
					}
					else if (commandMessage.Equals("-rconsalt"))
					{
						try
						{
							properties.RConHashNonce = args[i + 1];
						}
						catch
						{

						}
					}
					else if (commandMessage.Equals("-rotatelog"))
					{
						try
						{
							properties.LogRotation = Convert.ToBoolean(args[i + 1]);
						}
						catch
						{

						}
					}
					else if (commandMessage.Equals("-spawnnpcmax"))
					{
						try
						{
							properties.SpawnNPCMax = Convert.ToInt16(args[i + 1]);
						}
						catch
						{

						}
					}
				}

				properties.Save();
			}
		}
		
		public static TimeSpan LastUpdateTime { get; private set; }
		
		public static void UpdateLoop()
		{
			try
			{
				if (server == null)
				{
					ProgramLog.Log ("Issue in UpdateLoop thread!");
					return;
				}
	
				if (Server.rand == null)
				{
					Server.rand = new Random((int)DateTime.Now.Ticks);
				}
				
				bool hibernate = properties.StopUpdatesWhenEmpty;
	
				if (properties.SimpleLoop)
				{
					Stopwatch s = new Stopwatch();
					s.Start();
	
					double updateTime = 16.66666666666667;
					double nextUpdate = s.ElapsedMilliseconds + updateTime;
	
					while (!Netplay.disconnect)
					{
						double now = s.ElapsedMilliseconds;
						double left = nextUpdate - now;
	
						if (left >= 0)
						{
							while (left > 1)
							{
								Thread.Sleep((int)left);
								left = nextUpdate - s.ElapsedMilliseconds;
							}
							nextUpdate += updateTime;
						}
						else
							nextUpdate = now + updateTime;
	
						if (Netplay.anyClients || (hibernate == false))
						{
							var start = s.Elapsed;
							server.Update (s);
							LastUpdateTime = s.Elapsed - start;
						}
					}
	
					return;
				}
	
				double serverProcessAverage = 16.666666666666668;
				double leftOver = 0.0;
	
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
	
				while (!Netplay.disconnect)
				{
					double elapsed = (double)stopwatch.ElapsedMilliseconds;
					if (elapsed + leftOver >= serverProcessAverage)
					{
						leftOver += elapsed - serverProcessAverage;
						stopwatch.Reset();
						stopwatch.Start();
	
						if (leftOver > 1000.0)
						{
							leftOver = 1000.0;
						}
						if (Netplay.anyClients || (hibernate == false))
						{
							server.Update (stopwatch);
						}
						double num9 = (double)stopwatch.ElapsedMilliseconds + leftOver;
						if (num9 < serverProcessAverage)
						{
							int num10 = (int)(serverProcessAverage - num9) - 1;
							if (num10 > 1)
							{
								Thread.Sleep(num10);
								if (hibernate && !Netplay.anyClients)
								{
									leftOver = 0.0;
									Thread.Sleep(10);
								}
							}
						}
					}
					Thread.Sleep(0);
				}
			}
			catch (Exception e)
			{
				ProgramLog.Log (e, "World update thread crashed");
			}
		}

	}
}
