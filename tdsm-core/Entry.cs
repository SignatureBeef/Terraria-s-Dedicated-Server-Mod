//#define WebInterface
//#define TDSMServer

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using TDSM.API;
using TDSM.API.Callbacks;
using TDSM.API.Command;
using TDSM.API.Plugin;
using TDSM.Core.Definitions;
using TDSM.API.Logging;
using TDSM.Core.Misc;
using TDSM.API.Misc;
using TDSM.Core.RemoteConsole;
using TDSM.Core.ServerCharacters;
using Terraria;
using Terraria.Social;

namespace TDSM.Core
{
    //    public enum SSCResetLevel : byte
    //    {
    //        GUEST = 1,
    //        AUTH = 2,
    //        OP = 4,
    //
    //        ALL = GUEST | AUTH | OP
    //    }

    public partial class Entry : BasePlugin
    {
        public const Int32 CoreBuild = 5;

        private bool _useTimeLock;

        public bool UseTimeLock
        {
            get
            { return _useTimeLock; }
            set
            {

                _useTimeLock = value;
                TimelockTime = Terraria.Main.time;
            }
        }

        public double TimelockTime { get; set; }

        public bool TimelockRain { get; set; }

        public bool TimelockSlimeRain { get; set; }

        public static string RConHashNonce { get; set; }

        public static string RConBindAddress { get; set; }

        public static bool EnableCheatProtection { get; set; }

        private bool VanillaOnly
        {
            get
            {
                return !IsEnabled;
            }
            set
            {
                if (value)
                {
                    if (IsEnabled)
                        PluginManager.DisablePlugin(this);
                }
                else
                {
                    if (!IsEnabled)
                        PluginManager.EnablePlugin(this);
                }
            }
        }

        public bool StopNPCSpawning { get; set; }

        public bool TimeFastForwarding { get; set; }

        public bool RunServerCore { get; set; }

        private string _webServerAddress { get; set; }

        private string _webServerProvider { get; set; }

        public bool RestartWhenNoPlayers { get; set; }

        public static DataRegister Ops { get; private set; }

        public int ExitAccessLevel { get; set; }

        public bool EnableHeartbeat { get; set; }

        public bool AllowSSCGuestInfo { get; set; }

        public Entry()
        {
            this.TDSMBuild = CoreBuild;
            this.Author = "TDSM";
            this.Description = "TDSM Core";
            this.Name = "TDSM Core Module";
            this.Version = CoreBuild + Globals.PhaseToSuffix(ReleasePhase.LiveRelease);
        }

        protected override void Enabled()
        {
            base.Enabled();

            TDSM.API.Callbacks.MainCallback.StatusTextChange = OnStatusTextChanged;
            TDSM.API.Callbacks.MainCallback.UpdateServer = OnUpdateServer;

            TDSM.API.Command.CommandParser.ExtCheckAccessLevel = (acc, sender) =>
            {
                if (sender is RConSender)
                    return acc <= AccessLevel.REMOTE_CONSOLE;

                return false;
            };

            EnableCheatProtection = true;
            RunServerCore = true;
            ExitAccessLevel = -1;
        }

        protected override void Initialized(object state)
        {
            //            if (/*!Globals.IsPatching &&*/ !ProgramLog.IsOpen)
            //            {
            //                var logFile = Globals.DataPath + System.IO.Path.DirectorySeparatorChar + "server.log";
            //                ProgramLog.OpenLogFile(logFile);
            //
            ProgramLog.Log("TDSM Rebind core build {0}", this.Version);
            //
            //                Tools.SetWriteLineMethod(ProgramLog.Log, OnLogFinished);
            //                ConsoleSender.DefaultColour = ConsoleColor.Gray;
            //                //ConsoleSender.SetMethod((msg, r, g, b) =>
            //                //{
            //                //    Console.ForegroundColor = FromColor((byte)r, (byte)g, (byte)b);
            //                //    Console.WriteLine(msg);
            //                //});
            //            }

            Ops = new DataRegister(System.IO.Path.Combine(Globals.DataPath, "ops.txt"));
#if WebInterface
            WebInterface.WebPermissions.Load();
#endif
            AddCommand("platform")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Show what type of server is running TDSM")
                .SetDefaultUsage()
                .WithPermissionNode("tdsm.platform")
                .Calls(this.OperatingSystem);

            AddCommand("exit")
                .WithDescription("Stops the server")
                .WithAccessLevel(AccessLevel.CONSOLE)
                .SetDefaultUsage()
                .WithPermissionNode("tdsm.admin")
                .Calls(this.Exit);

            AddCommand("stop")
                .WithDescription("Stops the server")
                .WithAccessLevel(AccessLevel.CONSOLE)
                .SetDefaultUsage()
                .WithPermissionNode("tdsm.admin")
                .Calls(this.Exit);

            AddCommand("time")
                .WithDescription("Change the time of day")
                .WithAccessLevel(AccessLevel.OP)
                .WithHelpText("set <numeric time>")
                .WithHelpText("set 5:10am")
                .WithHelpText("now|?")
                .WithHelpText("day|dawn|dusk|noon|night")
                .WithPermissionNode("tdsm.time")
                .Calls(this.Time);

            AddCommand("give")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Give a player items")
                .WithHelpText("<amount> <itemname:itemid> [prefix] [player]")
                .WithPermissionNode("tdsm.give")
                .Calls(this.Give);

            AddCommand("spawnnpc")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Spawns NPCs")
                .WithHelpText("<amount> \"<name:id>\" \"<player>\"")
                .WithPermissionNode("tdsm.spawnnpc")
                .Calls(this.SpawnNPC);

            AddCommand("tp")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Teleport a player to another player")
                .WithHelpText("<player> <toplayer> - another player")
                .WithHelpText("<player> <x> <y>")
                .WithHelpText("<toplayer>          - yourself")
                .WithHelpText("<x> <y>")
                .WithHelpText("                    - yourself to spawn")
                .WithPermissionNode("tdsm.tp")
                .Calls(this.Teleport);

            AddCommand("tphere")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Teleport a player to yourself")
                .WithHelpText("<player>")
                .WithPermissionNode("tdsm.tphere")
                .Calls(this.TeleportHere);

            AddCommand("save")
                .WithDescription("Save world and configuration data")
                .WithAccessLevel(AccessLevel.OP)
                .SetDefaultUsage()
                .WithPermissionNode("tdsm.admin")
                .Calls(this.SaveAll);

            AddCommand("save-all")
                .WithDescription("Save world and configuration data")
                .WithAccessLevel(AccessLevel.OP)
                .SetDefaultUsage()
                .WithPermissionNode("tdsm.admin")
                .Calls(this.SaveAll);

            //AddCommand("reload")
            //    .WithDescription(Languages.CommandDescription_ReloadConfig)
            //    .WithAccessLevel(AccessLevel.REMOTE_CONSOLE)
            //    .WithPermissionNode("tdsm.admin")
            //    .Calls(this.Reload);

            AddCommand("itemrej")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Manage item rejections")
                .WithHelpText("-add|-remove <id:name>")
                .WithHelpText("-clear")
                .WithPermissionNode("tdsm.itemrej")
                .Calls(this.ItemRejection);

            AddCommand("refresh")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Redownload the area around you from the server")
                .WithHelpText("Usage:    refresh")
                .WithPermissionNode("tdsm.refresh")
                .Calls(this.Refresh);

            AddCommand("list")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Lists online players")
                .SetDefaultUsage()
                .WithPermissionNode("tdsm.who")
                .Calls(this.List);

            AddCommand("who")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Lists online players")
                .SetDefaultUsage()
                .WithPermissionNode("tdsm.who")
                .Calls(this.List);

            AddCommand("players")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Lists online players")
                .SetDefaultUsage()
                .WithPermissionNode("tdsm.who")
                .Calls(this.OldList);

            // this is what the server crawler expects
            AddCommand("playing")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Lists online players")
                .SetDefaultUsage()
                .WithPermissionNode("tdsm.who")
                .Calls(this.OldList);

            AddCommand("online")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Lists online players")
                .SetDefaultUsage()
                .WithPermissionNode("tdsm.who")
                .Calls(this.List);

            AddCommand("me")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("3rd person talk")
                .WithHelpText("<message> - Message to display in third person.")
            //.SetDefaultUsage() //This was causing an additional "me" to be displayed in the help commmand syntax.
                .WithPermissionNode("tdsm.me")
                .Calls(this.Action);

            AddCommand("say")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Say a message from the server")
                .WithHelpText("<message>")
                .WithPermissionNode("tdsm.say")
                .Calls(this.Say);

            AddCommand("status")
                .WithDescription("Server status")
                .SetDefaultUsage()
                .WithPermissionNode("tdsm.status")
                .Calls(this.ServerStatus);

            AddCommand("kick")
                .WithDescription("Kicks a player from the server")
                .WithHelpText("<player> - Kicks the player specified.")
                .WithPermissionNode("tdsm.kick")
                .Calls(this.Kick);

            AddCommand("op")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Allows a player server operator status")
                .WithHelpText("<player> <password> - Sets the player as an operator on the server and sets the OP password for that player.")
                .WithPermissionNode("tdsm.op")
                .Calls(this.OpPlayer);

            AddCommand("deop")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Removes server operator status from a player")
                .WithHelpText("<player> - Removes server operator status from the specified player.")
                .WithPermissionNode("tdsm.deop")
                .Calls(this.DeopPlayer);

            AddCommand("oplogin")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Allows an operator to log in")
                .WithHelpText("<password> - Logs into the server as an OP.")
                .WithPermissionNode("tdsm.oplogin")
                .Calls(this.OpLogin);

            AddCommand("oplogout")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Logs out a signed in operator.")
                .SetDefaultUsage()
                .WithPermissionNode("tdsm.oplogout")
                .Calls(this.OpLogout);

            AddCommand("spawnboss")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Spawn a boss")
                .WithHelpText("<amount> <boss> <player>")
                .WithHelpText("(If no player is entered it will be a random online player)")
                .WithPermissionNode("tdsm.spawnboss")
                .Calls(this.SummonBoss);

            AddCommand("timelock")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Forces the time to stay at a certain point.")
                .WithHelpText("now")
                .WithHelpText("set day|dawn|dusk|noon|night")
                .WithHelpText("setat <time>")
                .WithHelpText("disable")
                .WithPermissionNode("tdsm.timelock")
                .Calls(this.Timelock);

            AddCommand("fastforwardtime")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Fast forwards time until disabled.")
                .WithPermissionNode("tdsm.fastforwardtime")
                .Calls(this.FastForwardTime);

            AddCommand("heal")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Heals one or all players.")
                .WithHelpText("<player>")
                .WithHelpText("-all")
                .WithPermissionNode("tdsm.heal")
                .Calls(this.Heal);

            AddCommand("hardmode")
                .WithAccessLevel(AccessLevel.OP)
                .SetDefaultUsage()
                .WithDescription("Enables hard mode.")
                .WithPermissionNode("tdsm.hardmode")
                .Calls(this.HardMode);

            AddCommand("rcon")
                .WithDescription("Manage remote console access.")
                .WithAccessLevel(AccessLevel.REMOTE_CONSOLE)
                .WithHelpText("load       - reload login database")
                .WithHelpText("list       - list rcon connections")
                .WithHelpText("cut <name> - cut off rcon connections")
                .WithHelpText("ban <name> - cut off rcon connections and revoke access")
                .WithHelpText("add <name> <password> - add an rcon user")
                .WithPermissionNode("tdsm.rcon")
                .Calls(RConServer.RConCommand);

            AddCommand("npcspawning")
                .WithDescription("Turn NPC spawning on or off.")
                .WithAccessLevel(AccessLevel.OP)
                .WithHelpText("<true|false>")
                .WithPermissionNode("tdsm.npcspawning")
                .Calls(this.NPCSpawning);

            AddCommand("invasion")
                .WithDescription("Begins an invasion")
                .WithAccessLevel(AccessLevel.OP)
                .WithHelpText("goblin|frost|pirate|martian")
                .WithHelpText("-custom <npc id or name> <npc id or name> ...")
                .WithHelpText("stop|end|cancel")
                .WithPermissionNode("tdsm.invasion")
                .Calls(this.Invasion);

            AddCommand("serverlist")
                .WithDescription("Manages the heartbeat and server list")
                .WithAccessLevel(AccessLevel.OP)
                .WithHelpText("print|?              - Displays the current details")
                .WithHelpText("enable|disable       - Enable/disable the heartbeat")
                .WithHelpText("public true|false    - Allows public viewing")
                .WithHelpText("desc|name|domain     - Displays the current")
                .WithHelpText("desc <description>")
                .WithHelpText("name <name>")
                .WithHelpText("domain <domain>")
                .WithPermissionNode("tdsm.serverlist")
                .Calls(this.ServerList);

            AddCommand("var")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Experimental variable manipulation")
                .WithHelpText("<field|exec|prop> <namespace.classname> <fieldname|methodname>")
                .WithHelpText("field Terraria.Main eclipse          #Get the value")
                .WithHelpText("field Terraria.Main eclipse false    #Set the value")
                .WithPermissionNode("tdsm.var")
                .Calls(this.VariableMan);

            AddCommand("worldevent")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Start or stop an event")
                .WithHelpText("eclipse|bloodmoon|pumpkinmoon|snowmoon|slimerain")
                .WithPermissionNode("tdsm.worldevent")
                .Calls(this.WorldEvent);
#if TDSMServer
            AddCommand("maxplayers")
                .WithAccessLevel(AccessLevel.REMOTE_CONSOLE)
                .WithDescription("Set the maximum number of player slots.")
                .WithHelpText("<num> - set the max number of slots")
                .WithHelpText("<num> <num> - also set the number of overlimit slots")
                .WithPermissionNode("tdsm.maxplayers")
                .Calls(SlotManager.MaxPlayersCommand);

            AddCommand("q")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("List connections waiting in queues.")
                .WithHelpText("q")
                .WithPermissionNode("tdsm.q")
                .Calls(SlotManager.QCommand);

            AddCommand("conn")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Accept new connections.")
                .WithPermissionNode("tdsm.conn")
                .SetDefaultUsage()
                .Calls(Server.Command_AcceptConnections);
#endif
            //            AddCommand("restart")
            //                .WithAccessLevel(AccessLevel.OP)
            //                .WithDescription("Restart the server.")
            //                .WithHelpText("<no parameters>    - Restart immediately.")
            //                .WithHelpText("--wait             - Wait for users to disconnect and then restart.")
            //                .WithPermissionNode("tdsm.restart")
            //                .Calls(this.Restart);

#if DEBUG
            AddCommand("repo")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Install or update plugins.")
                .WithHelpText("<status|update|install> <plugin name>")
                .WithHelpText("status -all")
                .WithHelpText("update -all")
                .WithHelpText("update \"TDSM Core Module\"")
                .WithPermissionNode("tdsm.repo")
                .Calls(this.RepositoryCommand);

            //Template for when we have more plugins
            //AddCommand("repo")
            //    .WithDescription("The tdsm update repository")
            //    .WithAccessLevel(AccessLevel.OP)
            //    .WithHelpText("status       - Displays plugins out of date")
            //    .WithHelpText("definitions  - Update NPC and Item definitions")
            //    .WithHelpText("updatetest   - Tests if your plugins are compatible with the latest TDSM")
            //    .WithHelpText("search <search params> - Browse the repository")
            //    .WithHelpText("update <plugin name>|-all  - Update a particular plugin or all")
            //    .WithHelpText("install <plugin name>  - Installs a plugin")
            //    .WithPermissionNode("tdsm.repo")
            //    .Calls(this.Repository);

            AddCommand("group")
                .WithAccessLevel(AccessLevel.OP)
                .WithPermissionNode("tdsm.group")
                .WithDescription("Manage groups and their permissions")
                .WithHelpText("add <group> <ApplyToGuests> <Parent> <R> <G> <B> <Prefix> <Suffix>")
                .WithHelpText("remove <group>")
                .WithHelpText("addnode <group> <node> [deny]")
                .WithHelpText("removenode <group> <node>")
                .WithHelpText("list")
                .WithHelpText("listnodes <group>")
                .Calls(this.GroupPermission);

            AddCommand("user")
                .WithAccessLevel(AccessLevel.OP)
                .WithPermissionNode("tdsm.user")
                .WithDescription("Manage user permissions")
                .WithHelpText("add <user> <password> [op]")
                .WithHelpText("add <group>")
                .WithHelpText("remove <user> ")
                .WithHelpText("update <user> <password> [op]")
                .WithHelpText("addnode <username> <node> [deny]")
                .WithHelpText("removenode <username> <node> [deny]")
                .WithHelpText("listgroups")
                .WithHelpText("listnodes")
                .Calls(this.UserPermission);

            AddCommand("killnpc")
                .WithAccessLevel(AccessLevel.OP)
                .WithPermissionNode("tdsm.killnpc")
                .WithDescription("Kill all non town NPC's")
                .Calls(this.KillNPC);

            AddCommand("auth")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithPermissionNode("tdsm.auth")
                .WithDescription("Sign in")
                .Calls(this.Auth);

            AddCommand("grow")
                .WithAccessLevel(AccessLevel.OP)
                .WithPermissionNode("tdsm.grow")
                .Calls((ISender sender, ArgumentList args) =>
                {
                    if (null == WorldGen.genRand) WorldGen.genRand = new Random();
                    var ply = sender as Player;
                    int tileX = (int)(ply.position.X / 16f), tileY = (int)((ply.position.Y + ply.height) / 16f);

                    if (args.TryPop("-alch")) WorldGen.GrowAlch(tileX, tileY);
                    //                    else if (args.TryPop("-cactus")) WorldGen.GrowCactus(tileX, tileY);
                    else if (args.TryPop("-epictree")) WorldGen.GrowEpicTree(tileX, tileY);
                    //                    else if (args.TryPop("-livingtree")) WorldGen.GrowLivingTree(tileX, tileY);
                    else if (args.TryPop("-palmtree")) WorldGen.GrowPalmTree(tileX, tileY);
                    //                    else if (args.TryPop("-pumpkin")) WorldGen.GrowPumpkin(tileX, tileY);
                    else if (args.TryPop("-shroom")) WorldGen.GrowShroom(tileX, tileY);
                    //                    else if (args.TryPop("-spike")) WorldGen.GrowSpike(tileX, tileY);
                    else if (args.TryPop("-tree") || args.TryPop("-t")) WorldGen.GrowTree(tileX, tileY);
                    else if (args.TryPop("-tree")) WorldGen.GrowTree(tileX, tileY);
                    else if (args.TryPop("-grass") || args.TryPop("-g")) WorldGen.SpreadGrass(tileX, tileY);
                    else if (args.TryPop("-undergroundtree")) WorldGen.GrowUndergroundTree(tileX, tileY);
                    else throw new CommandError("Element not supported");

                    NetMessage.SendTileSquare(ply.whoAmI, (int)(ply.position.X / 16), (int)(ply.position.Y / 16), 32);
                });

            AddCommand("abuff")
                .WithAccessLevel(AccessLevel.OP)
                .WithPermissionNode("tdsm.abuff")
                .Calls((ISender sender, ArgumentList args) =>
                {
                    var time = args.GetInt(0);

                    (sender as Player).AddBuff(21, time);

                    NetMessage.SendData(55, -1, -1, "", (sender as Player).whoAmI, 21, time, 0, 0, 0, 0);
                    NetMessage.SendData(55, (sender as Player).whoAmI, -1, "", (sender as Player).whoAmI, 21, time);
                });
#endif

            if (!DefinitionManager.Initialise())
                ProgramLog.Log("Failed to initialise definitions.");

            ProgramLog.Log("TDSM Rebind core enabled");
        }

        void ProcessPIDFile(string pidPath)
        {
            var PIDFile = pidPath.Trim();
            if (PIDFile.Length > 0)
            {
                string ProcessUID = Process.GetCurrentProcess().Id.ToString();
                bool fail = false;
                if (File.Exists(PIDFile))
                {
                    try
                    {
                        File.Delete(PIDFile);
                    }
                    catch (Exception e)
                    {
                        ProgramLog.Log(e);
                        ProgramLog.Console.Print("Issue deleting PID file, Continue? [Y/n]: ");
                        if (Console.ReadLine().ToLower() == "n")
                        {
                            ProgramLog.Console.Print("Press any key to exit...");
                            Console.ReadKey(true);
                            return;
                        }
                        fail = true;
                    }
                }
                if (!fail)
                {
                    try
                    {
                        File.WriteAllText(PIDFile, ProcessUID);
                    }
                    catch (Exception e)
                    {
                        ProgramLog.Log(e);
                        ProgramLog.Console.Print("Issue writing PID file, Continue? [Y/n]: ");
                        if (Console.ReadLine().ToLower() == "n")
                        {
                            ProgramLog.Console.Print("Press any key to exit...");
                            Console.ReadKey(true);
                            return;
                        }
                    }
                    ProgramLog.Log("PID file successfully created with: " + ProcessUID);
                }
            }
        }

        [Hook(HookOrder.NORMAL)]
        void OnInventoryItemReceived(ref HookContext ctx, ref HookArgs.InventoryItemReceived args)
        {
#if TDSMSever
            if (Server.ItemRejections.Count > 0)
            {
                if (args.Item != null)
                {
                    if (Server.ItemRejections.Contains(args.Item.name) || Server.ItemRejections.Contains(args.Item.type.ToString()))
                    {
                        if (!String.IsNullOrEmpty(args.Item.name))
                        {
                            ctx.SetKick(args.Item.name + " is not allowed on this server.");
                        }
                        else
                        {
                            ctx.SetKick("Item type " + args.Item.type.ToString() + " is not allowed on this server.");
                        }
                    }
                }
            }
#endif
        }

        [Hook(HookOrder.NORMAL)]
        void OnPlayerJoin(ref HookContext ctx, ref HookArgs.PlayerEnteredGame args)
        {
            //The player may randomly disconnect at any time, and if it's before the point of saving then the data may be lost.
            //So we must ensure the data is saved.
            //ServerCharacters.CharacterManager.EnsureSave = true;

            if (CharacterManager.Mode == CharacterMode.UUID)
            {
                CharacterManager.LoadForAuthenticated(ctx.Player, !AllowSSCGuestInfo);
            }
            else if (CharacterManager.Mode == CharacterMode.AUTH)
            {
                if (!String.IsNullOrEmpty(ctx.Player.AuthenticatedAs))
                {
                    CharacterManager.LoadForAuthenticated(ctx.Player, !AllowSSCGuestInfo);
                }
            }
        }

        [Hook(HookOrder.NORMAL)]
        void OnPlayerAuthenticated(ref HookContext ctx, ref HookArgs.PlayerAuthenticationChanged args)
        {
            if (ctx.Client.State >= 4 && CharacterManager.Mode == CharacterMode.AUTH)
            {
                CharacterManager.LoadForAuthenticated(ctx.Player, !AllowSSCGuestInfo);
            }
        }


        [Hook(HookOrder.NORMAL)]
        void OnStartCommandProcessing(ref HookContext ctx, ref HookArgs.StartCommandProcessing args)
        {
            ctx.SetResult(HookResult.IGNORE);

            (new TDSM.API.Misc.ProgramThread("Command", ListenForCommands)).Start();
        }

        [Hook(HookOrder.NORMAL)]
        void OnPlayerDisconnected(ref HookContext ctx, ref HookArgs.PlayerLeftGame args)
        {
            if (Terraria.Main.ServerSideCharacter)
            {
                CharacterManager.SavePlayerData(ctx.Player);
            }
#if TDSMServer
            if (RestartWhenNoPlayers && ClientConnection.All.Count - 1 == 0)
            {
                PerformRestart();
            }
#endif
        }

        //[Hook(HookOrder.NORMAL)]
        //void OnBanAddRequired(ref HookContext ctx, ref HookArgs.AddBan args)
        //{
        //    ctx.SetResult(HookResult.IGNORE);

        //    //Server.Bans.Add(args.RemoteAddress); //TODO see if port needs removing
        //}

        void ListenForCommands()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            ProgramLog.Log("Ready for commands.");
            while (!Terraria.Netplay.disconnect /*|| Server.RestartInProgress*/)
            {
                try
                {
                    var ln = Console.ReadLine();
                    if (!String.IsNullOrEmpty(ln))
                        UserInput.CommandParser.ParseConsoleCommand(ln);
                    else if (null == ln)
                    {
                        ProgramLog.Log("No console input available");
                        break;
                    }
                }
                catch (ExitException)
                {
                }
                catch (Exception e)
                {
                    ProgramLog.Log("Exception from command");
                    ProgramLog.Log(e);
                }
            }
        }

        protected override void Disabled()
        {
            ProgramLog.Log(base.Name + " disabled.");
        }

        public static void Log(string fmt, params object[] args)
        {
            ProgramLog.Log("[TDSM] " + fmt, args);
        }

        [Hook(HookOrder.NORMAL)]
        void OnChat(ref HookContext ctx, ref HookArgs.PlayerChat args)
        {
            if (args.Message.Length > 0 && args.Message.Substring(0, 1).Equals("/"))
            {
                ProgramLog.Log(ctx.Player.Name + " sent command: " + args.Message);
                ctx.SetResult(HookResult.IGNORE);

                UserInput.CommandParser.ParsePlayerCommand(ctx.Player, args.Message);
            }
        }

        //[Hook(HookOrder.NORMAL)]
        void OnUpdateServer(/*ref HookContext ctx, ref HookArgs.UpdateServer args*/)
        {
            //for (var i = 0; i < Terraria.Main.player.Length; i++)
            //{
            //    var player = Terraria.Main.player[i];
            //    //if (player.active)
            //    {
            //        var conn = (player.Connection as ClientConnection);
            //        if (conn != null)
            //            conn.Flush();
            //    }
            //    //if (Terraria.Main.player[i].active)
            //    //{
            //    //    Server.CheckSection(i, Terraria.Main.player[i].position);

            //    //    //TODO SpamUpdate
            //    //    //if(TDSM.API.Callbacks.Netplay.slots[i].conn != null && TDSM.API.Callbacks.Netplay.slots[i].conn.Active )
            //    //    //    TDSM.API.Callbacks.Netplay.slots[i].conn.s
            //    //}
            //}

            if (_useTimeLock)
            {
                Terraria.Main.time = TimelockTime;
                Terraria.Main.raining = TimelockRain;
                Terraria.Main.slimeRain = TimelockSlimeRain;
                //TODO: verify
            }

            if (TimeFastForwarding) Terraria.Main.fastForwardTime = true;

            if (Terraria.Main.ServerSideCharacter)
            {
                ServerCharacters.CharacterManager.SaveAll();
            }
        }

        //[Hook(HookOrder.NORMAL)]
        //void OnPlayerAuthenticationChanged(ref HookContext ctx, ref HookArgs.PlayerAuthenticationChanged args)
        //{
        //    if (Terraria.Main.ServerSideCharacter)
        //    {
        //        var cfg = ServerCharacters.CharacterManager.LoadPlayerData(ctx.Player, true);
        //        if (cfg != null)
        //        {
        //            cfg.ApplyToPlayer(ctx.Player);
        //        }
        //    }
        //}

        void OnLogFinished()
        {
            Thread.Sleep(500);
            ProgramLog.Log("Log end.");
            ProgramLog.Close();
        }

        [Hook(HookOrder.NORMAL)]
        void OnNPCSpawned(ref HookContext ctx, ref HookArgs.NPCSpawn args)
        {
            if (StopNPCSpawning)
                ctx.SetResult(HookResult.IGNORE);
        }

        /////Avoid using this as much as possible (this goes for plugin developers too).
        /////The idea is to be able to add/remove a plugin from the installation without issues.
        /////You'll find that generally your implementation should end up in the API
        //[Hook(HookOrder.NORMAL)]
        //void OnServerPatching(ref HookContext ctx, ref HookArgs.PatchServer args)
        //{
        //    if (args.IsServer)
        //    {
        //        var _self = AssemblyDefinition.ReadAssembly("Plugins/TDSM.Core.dll");

        //        Console.WriteLine("Routing socket implementations...");
        //        var serverClass = _self.MainModule.Types.Where(x => x.Name == "Server").First();
        //        var sockClass = _self.MainModule.Types.Where(x => x.Name == "ServerSlot").First();
        //        var targetArray = serverClass.Fields.Where(x => x.Name == "slots").First();
        //        var targetField = sockClass.Fields.Where(x => x.Name == "tileSection").First();

        //        var msa = new MemoryStream(args.Terraria);
        //        msa.Seek(0, SeekOrigin.Begin);
        //        var terraria = AssemblyDefinition.ReadAssembly(msa);
        //        //var terraria = args.Terraria as AssemblyDefinition;

        //        //Replace Terraria.Netplay.Clients references with TDSM.Core.TDSM.API.Callbacks.Netplay.slots
        //        var instructions = terraria.MainModule.Types
        //            .SelectMany(x => x.Methods
        //                .Where(y => y.HasBody && y.Body.Instructions != null)
        //            )
        //            .SelectMany(x => x.Body.Instructions)
        //            .Where(x => x.OpCode == Mono.Cecil.Cil.OpCodes.Ldsfld
        //                && x.Operand is FieldReference
        //                && (x.Operand as FieldReference).FieldType.FullName == "Terraria.ServerSock[]"
        //                && x.Next.Next.Next.OpCode == Mono.Cecil.Cil.OpCodes.Ldfld
        //                && x.Next.Next.Next.Operand is FieldReference
        //                && (x.Next.Next.Next.Operand as FieldReference).Name == "tileSection"
        //            )
        //            .ToArray();

        //        for (var x = 0; x < instructions.Length; x++)
        //        {
        //            instructions[x].Operand = terraria.MainModule.Import(targetArray);
        //            instructions[x].Next.Next.Next.Operand = terraria.MainModule.Import(targetField);
        //        }

        //        //Replace SendAnglerQuest()
        //        //Replace sendWater()
        //        //Replace syncPlayers()
        //        //Replace AddBan()
        //        var ourClass = _self.MainModule.Types.Where(x => x.Name == "Net").First();
        //        foreach (var rep in new string[] { "SendAnglerQuest", "sendWater", "syncPlayers", "AddBan" })
        //        {
        //            var toBeReplaced = terraria.MainModule.Types
        //                .SelectMany(x => x.Methods
        //                    .Where(y => y.HasBody)
        //                )
        //                .SelectMany(x => x.Body.Instructions)
        //                .Where(x => x.OpCode == Mono.Cecil.Cil.OpCodes.Call
        //                    && x.Operand is MethodReference
        //                    && (x.Operand as MethodReference).Name == rep
        //                )
        //                .ToArray();

        //            var replacement = ourClass.Methods.Where(x => x.Name == rep).First();
        //            for (var x = 0; x < toBeReplaced.Length; x++)
        //            {
        //                toBeReplaced[x].Operand = terraria.MainModule.Import(replacement);
        //            }
        //        }

        //        using (var ms = new MemoryStream())
        //        {
        //            terraria.Write(ms);
        //            args.Terraria = ms.ToArray();
        //        }
        //    }
        //}

#if TDSMServer
        [Hook(HookOrder.NORMAL)]
        void OnDefaultServerStart(ref HookContext ctx, ref HookArgs.StartDefaultServer args)
        {
            if (RunServerCore)
            {
                ProgramLog.Log("Starting TDSM's slot server...");
                ctx.SetResult(HookResult.IGNORE);

                ServerCore.Server.StartServer();
            } else
                ProgramLog.Log("Vanilla only specified, continuing on with Re-Logic code...");
        }
#endif

        [Hook(HookOrder.NORMAL)]
        void OnConfigLineRead(ref HookContext ctx, ref HookArgs.ConfigurationLine args)
        {
            switch (args.Key)
            {
#if TDSMServer
                case "usewhitelist":
                    bool usewhitelist;
                    if (Boolean.TryParse(args.Value, out usewhitelist))
                    {
                        Server.WhitelistEnabled = usewhitelist;
                    }
                    break;
#endif
                case "vanilla-linux":
                    bool vanillaOnly;
                    if (Boolean.TryParse(args.Value, out vanillaOnly))
                    {
                        VanillaOnly = vanillaOnly;
                    }
                    break;
                case "heartbeat":
                    bool hb;
                    if (Boolean.TryParse(args.Value, out hb) && hb)
                    {
                        EnableHeartbeat = true;
                    }
                    break;
                case "server-list":
                    bool serverList;
                    if (Boolean.TryParse(args.Value, out serverList))
                    {
                        Heartbeat.PublishToList = serverList;
                    }
                    break;
                case "server-list-name":
                    Heartbeat.ServerName = args.Value;
                    break;
                case "server-list-desc":
                    Heartbeat.ServerDescription = args.Value;
                    break;
                case "server-list-domain":
                    Heartbeat.ServerDomain = args.Value;
                    break;
                case "rcon-hash-nonce":
                    RConHashNonce = args.Value;
                    break;
                case "rcon-bind-address":
                    RConBindAddress = args.Value;
                    break;
                case "web-server-bind-address":
                    //                    _webServerAddress = args.Value;
                    TDSM.API.WebServer.Start(args.Value);
                    break;
                case "web-server-provider":
                    _webServerProvider = args.Value;
                    break;
                case "web-server-serve-files":
                    bool serveFiles;
                    if (Boolean.TryParse(args.Value, out serveFiles))
                    {
                        //WebInterface.WebServer.ServeWebFiles = serveFiles;
                    }
                    break;
#if TDSMServer
                case "send-queue-quota":
                    int sendQueueQuota;
                    if (Int32.TryParse(args.Value, out sendQueueQuota))
                    {
                        Connection.SendQueueQuota = sendQueueQuota;
                    }
                    break;
                case "overlimit-slots":
                    int overlimitSlots;
                    if (Int32.TryParse(args.Value, out overlimitSlots))
                    {
                        Server.OverlimitSlots = overlimitSlots;
                    }
                    break;
#endif
                case "pid-file":
                    ProcessPIDFile(args.Value);
                    break;
                case "cheat-detection":
                    bool cheatDetection;
                    if (Boolean.TryParse(args.Value, out cheatDetection))
                    {
                        EnableCheatProtection = cheatDetection;
                    }
                    break;
                case "log-rotation":
                    bool logRotation;
                    if (Boolean.TryParse(args.Value, out logRotation))
                    {
                        ProgramLog.LogRotation = logRotation;
                    }
                    break;
                case "server-side-characters":
                    CharacterMode characterMode;
                    if (CharacterMode.TryParse(args.Value, out characterMode))
                    {
                        Terraria.Main.ServerSideCharacter = characterMode != CharacterMode.NONE;
                        CharacterManager.Mode = characterMode;
                        ProgramLog.Admin.Log("SSC are enabled with mode " + characterMode);

                        Hook(HookPoints.ReceiveNetMessage, OnNetMessageReceived);
                        //                        Hook(HookPoints.PlayerDataReceived, OnPlayerDataReceived);
                    }
                    else
                        ProgramLog.Error.Log("Failed to parse line server-side-characters. No SSC will be used.");
                    break;
                case "tdsm-server-core":
                    bool runServerCore;
                    if (Boolean.TryParse(args.Value, out runServerCore))
                    {
                        RunServerCore = runServerCore;
                    }
                    break;
                case "exitaccesslevel":
                    int accessLevel;
                    if (Int32.TryParse(args.Value, out accessLevel))
                    {
                        ExitAccessLevel = accessLevel;
                    }
                    break;
                case "ssc-allow-guest-info":
                    bool guestInfo;
                    if (Boolean.TryParse(args.Value, out guestInfo))
                    {
                        AllowSSCGuestInfo = guestInfo;
                    }
                    break;
            }
        }

        [Hook(HookOrder.NORMAL)]
        void OnGreetPlayer(ref HookContext ctx, ref HookArgs.PlayerPreGreeting args)
        {
            ctx.SetResult(HookResult.IGNORE);
            var lines = args.Motd.Split(new string[] { "\\0" }, StringSplitOptions.None);
            foreach (var line in lines)
                ctx.Player.SendMessage(line, 255, 0, 0, 255);

            string list = "";
            for (int i = 0; i < 255; i++)
            {
                if (Main.player[i].active)
                {
                    if (list == "")
                        list += Main.player[i].name;
                    else
                        list = list + ", " + Main.player[i].Name;
                }
            }

            ctx.Player.SendMessage("Current players: " + list + ".", 255, 255, 240, 20);
        }

        [Hook(HookOrder.NORMAL)]
        void OnServerStateChange(ref HookContext ctx, ref HookArgs.ServerStateChange args)
        {
            ProgramLog.Log("Server state changed to: " + args.ServerChangeState.ToString());

            if (args.ServerChangeState == ServerState.Initialising)
            {
                if (!String.IsNullOrEmpty(RConBindAddress))
                {
                    ProgramLog.Log("Starting RCON Server");
                    RemoteConsole.RConServer.Start(Path.Combine(Globals.DataPath, "rcon_logins.properties"));
                }

                if (Terraria.Main.ServerSideCharacter)
                {
                    CharacterManager.Init();
                }
            }
            if (args.ServerChangeState == ServerState.Stopping)
            {
                RemoteConsole.RConServer.Stop();
            }
            if (args.ServerChangeState == ServerState.Starting)
            {
                Heartbeat.Begin(CoreBuild);
            }

            //if (args.ServerChangeState == ServerState.Initialising)
#if TDSMServer
            if (!Server.IsInitialised)
            {
                Server.Init();

                if (!String.IsNullOrEmpty(RConBindAddress))
                {
                    ProgramLog.Log("Starting RCON Server");
                    RemoteConsole.RConServer.Start(Path.Combine(Globals.DataPath, "rcon_logins.properties"));
                }

                if (!String.IsNullOrEmpty(_webServerAddress))
                {
                    ProgramLog.Log("Starting Web Server");
                    WebInterface.WebServer.Begin(_webServerAddress, _webServerProvider);

                    AddCommand("webauth")
                        .WithAccessLevel(AccessLevel.OP)
                        .Calls(WebInterface.WebServer.WebAuthCommand);
                }
            }

            if (args.ServerChangeState == ServerState.Stopping)
            {
                RemoteConsole.RConServer.Stop();
                WebInterface.WebServer.End();
                //if (properties != null && File.Exists(properties.PIDFile.Trim()))
                //File.Delete(properties.PIDFile.Trim());
            }

            ctx.SetResult(HookResult.IGNORE); //Don't continue on with vanilla code
#endif
        }

        //[Hook(HookOrder.NORMAL)]
        //void OnNetMessageSendData(ref HookContext ctx, ref HookArgs.SendNetData args)
        //{
        //    ctx.SetResult(HookResult.IGNORE);
        //    NewNetMessage.SendData(args.MsgType, args.RemoteClient, args.IgnoreClient, args.Text, args.Number,
        //        args.Number2, args.Number3, args.Number4, args.Number5);
        //}

        //        string GetProgressKey(string input, out int length, out string progress)
        //        {
        //            length = 0;
        //            string key = null;
        //
        //            //Determine format
        //            int progTypeStart = input.IndexOf('-');
        //            int progTypeEnd = input.LastIndexOf('-');
        //
        //            if (progTypeStart > -1 && progTypeEnd > -1)
        //            {
        //                progTypeStart++;
        //                progTypeEnd--;
        //
        //                key = input.Substring(progTypeStart, progTypeEnd - 1);
        //
        //                //                length = input.Length - (progTypeEnd + 1);
        //                //This format does need
        //            }
        //            else
        //            {
        //
        //            }
        //
        //            return key;
        //        }

        static readonly System.Text.RegularExpressions.Regex _fmtGeneration = new System.Text.RegularExpressions.Regex(".* - (.*) - (.*)%");
        static readonly System.Text.RegularExpressions.Regex _fmtSemi = new System.Text.RegularExpressions.Regex("(.*): (.*)%");
        static readonly System.Text.RegularExpressions.Regex _fmtDefault = new System.Text.RegularExpressions.Regex("(.*) (.*)%");

        string GetProgressKey(string input, out string progress)
        {
            progress = String.Empty;

            var gen = _fmtGeneration.Matches(input);
            if (gen != null && gen.Count == 1 && gen[0].Groups.Count == 3)
            {
                progress = gen[0].Groups[2].Value + '%';
                return gen[0].Groups[1].Value;
            }
            gen = _fmtSemi.Matches(input);
            if (gen != null && gen.Count == 1 && gen[0].Groups.Count == 3)
            {
                progress = gen[0].Groups[2].Value + '%';
                return gen[0].Groups[1].Value;
            }
            gen = _fmtDefault.Matches(input);
            if (gen != null && gen.Count == 1 && gen[0].Groups.Count == 3)
            {
                progress = gen[0].Groups[2].Value + '%';
                return gen[0].Groups[1].Value;
            }

            return input;
        }

        //        [Hook]
        void OnNetMessageReceived(ref HookContext ctx, ref HookArgs.ReceiveNetMessage args)
        {
            if (Terraria.Main.ServerSideCharacter)
            {
                switch ((Packet)args.PacketId)
                {
                    case Packet.INVENTORY_DATA:
                        if (!AllowSSCGuestInfo && !ctx.Player.IsAuthenticated)
                            ctx.SetResult(HookResult.IGNORE);
                        break;

                    case Packet.PLAYER_MANA_UPDATE:
                        if (!AllowSSCGuestInfo && !ctx.Player.IsAuthenticated)
                            ctx.SetResult(HookResult.IGNORE);
                        break;

                    case Packet.PLAYER_HEALTH_UPDATE:
                        if (!AllowSSCGuestInfo && !ctx.Player.IsAuthenticated)
                            ctx.SetResult(HookResult.IGNORE);
                        break;
                }
            }
        }

        //        void OnPlayerDataReceived(ref HookContext ctx, ref HookArgs.PlayerDataReceived args)
        //        {
        //            //If the player is not authenticated, then ensure they are reset
        //            if (!AllowSSCGuestInfo && !ctx.Player.IsAuthenticated)
        //            {
        //
        //            }
        //        }

        [Hook]
        void OnNetMessageSend(ref HookContext ctx, ref HookArgs.SendNetMessage args)
        {
            if (Terraria.Main.ServerSideCharacter)
            {
                switch ((Packet)args.MsgType)
                {
                    case Packet.WORLD_DATA:
                        ctx.SetResult(HookResult.IGNORE);

                        var writer = NetMessage.buffer[args.BufferId].writer;

                        writer.Write((int)Main.time);

                        byte value;

                        value = 0;
                        if (Main.dayTime) value += 1;
                        if (Main.bloodMoon) value += 2;
                        if (Main.bloodMoon) value += 4;
                        writer.Write(value);

                        writer.Write((byte)Main.moonPhase);
                        writer.Write((short)Main.maxTilesX);
                        writer.Write((short)Main.maxTilesY);
                        writer.Write((short)Main.spawnTileX);
                        writer.Write((short)Main.spawnTileY);
                        writer.Write((short)Main.worldSurface);
                        writer.Write((short)Main.rockLayer);
                        writer.Write(Main.worldID);
                        writer.Write(Main.worldName);
                        writer.Write((byte)Main.moonType);
                        writer.Write((byte)WorldGen.treeBG);
                        writer.Write((byte)WorldGen.corruptBG);
                        writer.Write((byte)WorldGen.jungleBG);
                        writer.Write((byte)WorldGen.snowBG);
                        writer.Write((byte)WorldGen.hallowBG);
                        writer.Write((byte)WorldGen.crimsonBG);
                        writer.Write((byte)WorldGen.desertBG);
                        writer.Write((byte)WorldGen.oceanBG);
                        writer.Write((byte)Main.iceBackStyle);
                        writer.Write((byte)Main.jungleBackStyle);
                        writer.Write((byte)Main.hellBackStyle);
                        writer.Write(Main.windSpeedSet);
                        writer.Write((byte)Main.numClouds);

                        for (int k = 0; k < 3; k++) writer.Write(Main.treeX[k]);
                        for (int l = 0; l < 4; l++) writer.Write((byte)Main.treeStyle[l]);
                        for (int m = 0; m < 3; m++) writer.Write(Main.caveBackX[m]);
                        for (int n = 0; n < 4; n++) writer.Write((byte)Main.caveBackStyle[n]);

                        if (!Main.raining) Main.maxRaining = 0;
                        writer.Write(Main.maxRaining);

                        value = 0;
                        if (WorldGen.shadowOrbSmashed) value += 1;
                        if (NPC.downedBoss1) value += 2;
                        if (NPC.downedBoss2) value += 4;
                        if (NPC.downedBoss3) value += 8;
                        if (Main.hardMode) value += 16;
                        if (NPC.downedClown) value += 32;
                        if (Main.ServerSideCharacter) value += 64;
                        if (NPC.downedPlantBoss) value += 128;
                        writer.Write(value);

                        value = 0;
                        if (NPC.downedMechBoss1) value += 1;
                        if (NPC.downedMechBoss2) value += 2;
                        if (NPC.downedMechBoss3) value += 4;
                        if (NPC.downedMechBossAny) value += 8;
                        if (Main.cloudBGActive >= 1) value += 16;
                        if (WorldGen.crimson) value += 32;
                        if (Main.pumpkinMoon) value += 64;
                        if (Main.snowMoon) value += 128;
                        writer.Write(value);

                        value = 0;
                        if (Main.expertMode) value += 1;
                        if (Main.fastForwardTime) value += 2;
                        if (Main.slimeRain) value += 4;
                        if (NPC.downedSlimeKing) value += 8;
                        if (NPC.downedQueenBee) value += 16;
                        if (NPC.downedFishron) value += 32;
                        if (NPC.downedMartians) value += 64;
                        if (NPC.downedAncientCultist) value += 128;
                        writer.Write(value);

                        value = 0;
                        if (NPC.downedMoonlord) value += 1;
                        if (NPC.downedHalloweenKing) value += 2;
                        if (NPC.downedHalloweenTree) value += 4;
                        if (NPC.downedChristmasIceQueen) value += 8;
                        if (NPC.downedChristmasSantank) value += 16;
                        if (NPC.downedChristmasTree) value += 32;
                        if (NPC.downedGolemBoss) value += 64;
                        writer.Write(value);

                        writer.Write((sbyte)Main.invasionType);

                        if (SocialAPI.Network != null)
                            writer.Write(SocialAPI.Network.GetLobbyId());
                        else writer.Write(0);
                        break;
                }
            }
        }

        private int lastWritten = 0;
        //[Hook(HookOrder.NORMAL)]
        void OnStatusTextChanged() //ref HookContext ctx, ref HookArgs.StatusTextChanged args)
        {
            //ctx.SetResult(HookResult.IGNORE);
            //There's no locking and two seperate threads, so we must use local variables incase of changes
            var statusText = Terraria.Main.statusText;
            var oldStatusText = Terraria.Main.oldStatusText;

            if (oldStatusText != statusText)
            {
                if (!String.IsNullOrEmpty(statusText))
                {
                    string previousProgress, currentProgress;

                    string keyA = GetProgressKey(oldStatusText, out previousProgress);
                    string keyB = GetProgressKey(statusText, out currentProgress);

                    if (keyA != null && keyB != null)
                    {
                        keyA = keyA.Trim();
                        keyB = keyB.Trim();
                        if (keyA.Length > 0 && keyB.Length > 0)
                        {
                            if (keyA == keyB)
                            {
                                if (lastWritten > 0)
                                {
                                    for (var x = 0; x < lastWritten; x++)
                                        Console.Write("\b");
                                }

                                Console.Write(currentProgress);
                                lastWritten += currentProgress.Length - lastWritten;
                            }
                            else
                            {
                                Console.WriteLine();
                                lastWritten = 0;
                                Console.Write(statusText);

                                lastWritten += currentProgress.Length;

                                if (currentProgress.Length == 0)
                                    Console.WriteLine();
                            }
                        }
                        else
                        {
                            if (lastWritten > 0)//!String.IsNullOrEmpty(oldStatusText)) //There was existing text
                            {
                                Console.WriteLine();
                                lastWritten = 0;
                            }

                            Console.Write(statusText);
                            lastWritten += currentProgress.Length;
                        }
                    }
                    else if (keyA == null && keyB != null)
                    {
                        Console.Write(statusText);
                        lastWritten += currentProgress.Length;
                    }
                }
                else
                {
                    if (lastWritten > 0)//!String.IsNullOrEmpty(oldStatusText)) //There was existing text
                    {
                        Console.WriteLine();
                        lastWritten = 0;
                    }
                }
            }
            else if (statusText == String.Empty)
            {
                if (lastWritten > 0)//!String.IsNullOrEmpty(Terraria.Main.oldStatusText)) //There was existing text
                {
                    Console.WriteLine();
                    lastWritten = 0;
                }
            }
            Terraria.Main.oldStatusText = statusText;
        }

        public void PerformRestart()
        {
#if TDSMServer
            Server.PerformRestart();
            RestartWhenNoPlayers = false;
            if (_tskWaitForPlayers != null) _tskWaitForPlayers.Enabled = false;
            Server.AcceptNewConnections = _waitFPState.HasValue ? _waitFPState.Value : true;
#endif
        }
    }
}
