using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using tdsm.api;
using tdsm.api.Callbacks;
using tdsm.api.Command;
using tdsm.api.Misc;
using tdsm.api.Plugin;
using tdsm.core.Definitions;
using tdsm.core.Logging;
using tdsm.core.Messages.Out;
using tdsm.core.Misc;
using tdsm.core.ServerCore;

namespace tdsm.core
{
    public partial class Entry : BasePlugin
    {
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
                    if (IsEnabled) PluginManager.DisablePlugin(this);
                }
                else
                {
                    if (!IsEnabled) PluginManager.EnablePlugin(this);
                }
            }
        }

        public Entry()
        {
            this.TDSMBuild = 1;
            this.Author = "TDSM";
            this.Description = "TDSM Core";
            this.Name = "TDSM Core Module";
            this.Version = "1";
        }

        protected override void Initialized(object state)
        {
            if (!patcher.Program.IsPatching && !ProgramLog.IsOpen)
            {
                var logFile = Globals.DataPath + System.IO.Path.DirectorySeparatorChar + "server.log";
                ProgramLog.OpenLogFile(logFile);

                ProgramLog.Log("TDSM Rebind core build {0}", this.Version);

                Tools.SetWriteLineMethod(ProgramLog.Log);
            }

            //Register Hooks            
            Hook(HookPoints.PlayerWorldAlteration, OnPlayerWorldAlteration);
            Hook(HookPoints.ProjectileReceived, HookOrder.FIRST, OnReceiveProjectile);

            //Add this
            AddCommand("platform")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Show what type of server is running TDSM")
                .WithHelpText("Usage:   /platform") //TODO replace with .SetDefaultUsage()
                .Calls(this.OperatingSystem);

            AddCommand("exit")
                .WithDescription("Stops the server")
                .WithAccessLevel(AccessLevel.CONSOLE)
                .WithPermissionNode("tdsm.admin")
                .Calls(this.Exit);

            AddCommand("stop")
                .WithDescription("Stops the server")
                .WithAccessLevel(AccessLevel.CONSOLE)
                .WithPermissionNode("tdsm.admin")
                .Calls(this.Exit);

            AddCommand("time")
                .WithDescription("Change the time of day")
                .WithAccessLevel(AccessLevel.OP)
                .WithHelpText("Usage:   time -set <time>")
                .WithHelpText("         time -now")
                .WithHelpText("         time day|dawn|dusk|noon|night")
                .WithPermissionNode("tdsm.time")
                .Calls(this.Time);

            AddCommand("give")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Give a player items")
                .WithHelpText("Usage:   give <player> <amount> <itemname:itemid> [-prefix]")
                .WithPermissionNode("tdsm.give")
                .Calls(this.Give);

            AddCommand("spawnnpc")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Spawns NPCs")
                .WithHelpText("Usage:   spawnnpc <amount> \"<name:id>\" \"<player>\"")
                .WithPermissionNode("tdsm.spawnnpc")
                .Calls(this.SpawnNPC);

            AddCommand("tp")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Teleport a player to another player")
                .WithHelpText("Usage:   tp <player> <toplayer> - another player")
                .WithHelpText("         tp <player> <x> <y>")
                .WithHelpText("         tp <toplayer>          - yourself")
                .WithHelpText("         tp <x> <y>")
                .WithHelpText("         tp                     - yourself to spawn")
                .WithPermissionNode("tdsm.tp")
                .Calls(this.Teleport);

            AddCommand("tphere")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Teleport a player to yourself")
                .WithHelpText("Usage:   tphere <player>")
                .WithPermissionNode("tdsm.tphere")
                .Calls(this.TeleportHere);

            AddCommand("save-all")
                .WithDescription("Save world and configuration data")
                .WithAccessLevel(AccessLevel.OP)
                .WithPermissionNode("tdsm.admin")
                .Calls(this.SaveAll);

            //AddCommand("reload")
            //    .WithDescription(Languages.CommandDescription_ReloadConfig)
            //    .WithAccessLevel(AccessLevel.REMOTE_CONSOLE)
            //    .WithPermissionNode("tdsm.admin")
            //    .Calls(this.Reload);

            AddCommand("list")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Lists online players")
                .WithPermissionNode("tdsm.who")
                .Calls(this.List);

            AddCommand("who")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Lists online players")
                .WithPermissionNode("tdsm.who")
                .Calls(this.List);

            AddCommand("players")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Lists online players")
                .WithPermissionNode("tdsm.who")
                .Calls(this.OldList);

            // this is what the server crawler expects
            AddCommand("playing")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Lists online players")
                .WithPermissionNode("tdsm.who")
                .Calls(this.OldList);

            AddCommand("online")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Lists online players")
                .WithPermissionNode("tdsm.who")
                .Calls(this.List);

            AddCommand("me")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("3rd person talk")
                .WithPermissionNode("tdsm.me")
                .Calls(this.Action);

            AddCommand("say")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Say a message from the server")
                .WithPermissionNode("tdsm.say")
                .Calls(this.Say);

            AddCommand("status")
                .WithDescription("Server status")
                .WithHelpText("Usage:   status")
                .WithPermissionNode("tdsm.status")
                .Calls(this.Status);

            AddCommand("kick")
                .WithDescription("Kicks a player from the server")
                .WithHelpText("Usage:   kick <player>")
                .WithPermissionNode("tdsm.kick")
                .Calls(this.Kick);

            AddCommand("op")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Allows a player server operator status")
                .WithHelpText("Usage:   op <player> <password>")
                .WithPermissionNode("tdsm.op")
                .Calls(this.OpPlayer);

            AddCommand("deop")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Removes server operator status from a player")
                .WithHelpText("Usage:   deop <player>")
                .WithPermissionNode("tdsm.deop")
                .Calls(this.DeopPlayer);

            AddCommand("oplogin")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Allows an operator to log in")
                .WithHelpText("Usage:   oplogin <password>")
                .WithPermissionNode("tdsm.oplogin")
                .Calls(this.OpLogin);

            AddCommand("oplogout")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("Logs out a signed in operator.")
                .WithHelpText("Usage:   oplogout")
                .WithPermissionNode("tdsm.oplogout")
                .Calls(this.OpLogout);

            AddCommand("spawnboss")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Spawn a boss")
                .WithHelpText("Usage:    spawnboss eye skeletron eater kingslime prime twins destroyer wof")
                .WithHelpText("          spawnboss eye twins -night")
                .WithHelpText("          spawnboss -all")
                .WithHelpText("          spawnboss <boss> -player <name>")
                .WithHelpText("(If no player is entered it will be a random online player)")
                .WithPermissionNode("tdsm.spawnboss")
                .Calls(this.SummonBoss);

            AddCommand("timelock")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Forces the time to stay at a certain point.")
                .WithHelpText("Usage:    timelock now")
                .WithHelpText("          timelock set day|dawn|dusk|noon|night")
                .WithHelpText("          timelock setat <time>")
                .WithHelpText("          timelock disable")
                .WithPermissionNode("tdsm.timelock")
                .Calls(this.Timelock);

            AddCommand("heal")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Heals one or all players.")
                .WithHelpText("Usage:    heal <player>")
                .WithHelpText("          heal -all")
                .WithPermissionNode("tdsm.heal")
                .Calls(this.Heal);

            //Heartbeat.Begin(this.TDSMBuild);
            if (!DefinitionManager.Initialise()) ProgramLog.Log("Failed to initialise definitions.");
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
        void OnStartCommandProcessing(ref HookContext ctx, ref HookArgs.StartCommandProcessing args)
        {
            ctx.SetResult(HookResult.IGNORE);

            (new ProgramThread("Command", ListenForCommands)).Start();
        }

        static void ListenForCommands()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            ProgramLog.Log("Ready for commands.");
            while (!Terraria.Netplay.disconnect)
            {
                try
                {
                    var ln = Console.ReadLine();
                    UserInput.CommandParser.ParseConsoleCommand(ln);
                }
                catch (ExitException) { }
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

        bool tileBreakageAllowed;
        bool explosivesAllowed;
        void OnPlayerWorldAlteration(ref HookContext ctx, ref HookArgs.PlayerWorldAlteration args)
        {
            if (!tileBreakageAllowed) return;
            Log("Cancelled tile change of Player: " + ctx.Player.Name);
            ctx.SetResult(HookResult.RECTIFY);
        }

        void OnReceiveProjectile(ref HookContext ctx, ref HookArgs.ProjectileReceived args)
        {
            if (!explosivesAllowed && args.Current.IsExplosive())
            {
                Log("Cancelled explosive usage of Player: " + ctx.Player.Name);
                ctx.SetResult(HookResult.ERASE);
            }
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

        [Hook(HookOrder.NORMAL)]
        void OnUpdateServer(ref HookContext ctx, ref HookArgs.UpdateServer args)
        {
            for (var i = 0; i < Terraria.Main.player.Length; i++)
            {
                var player = Terraria.Main.player[i];
                //if (player.active)
                {
                    var conn = player.Connection;
                    if (conn != null)
                        conn.Flush();
                }
                if (Terraria.Main.player[i].active)
                {
                    Server.CheckSection(i, Terraria.Main.player[i].position);

                    //TODO SpamUpdate
                    //if(Server.slots[i].conn != null && Server.slots[i].conn.Active )
                    //    Server.slots[i].conn.s
                }
            }

            if (_useTimeLock) Terraria.Main.time = TimelockTime;
        }

        ///Avoid using this as much as possible (this goes for plugin developers too).
        ///The idea is to be able to add/remove a plugin from the installation without issues.
        ///You'll find that generally your implementation should end up in the API
        [Hook(HookOrder.NORMAL)]
        void OnServerPatching(ref HookContext ctx, ref tdsm.patcher.HookArgs.PatchServer args)
        {
            if (args.IsServer)
            {
                var _self = AssemblyDefinition.ReadAssembly("Plugins/tdsm.core.dll");

                Console.WriteLine("Routing socket implementations...");
                var serverClass = _self.MainModule.Types.Where(x => x.Name == "Server").First();
                var sockClass = _self.MainModule.Types.Where(x => x.Name == "ServerSlot").First();
                var targetArray = serverClass.Fields.Where(x => x.Name == "slots").First();
                var targetField = sockClass.Fields.Where(x => x.Name == "tileSection").First();

                //Replace Terraria.Netplay.serverSock references with tdsm.core.Server.slots
                var instructions = args.Default.Terraria.MainModule.Types
                    .SelectMany(x => x.Methods
                        .Where(y => y.HasBody)
                    )
                    .SelectMany(x => x.Body.Instructions)
                    .Where(x => x.OpCode == Mono.Cecil.Cil.OpCodes.Ldsfld
                        && x.Operand is FieldReference
                        && (x.Operand as FieldReference).FieldType.FullName == "Terraria.ServerSock[]"
                        && x.Next.Next.Next.OpCode == Mono.Cecil.Cil.OpCodes.Ldfld
                        && x.Next.Next.Next.Operand is FieldReference
                        && (x.Next.Next.Next.Operand as FieldReference).Name == "tileSection"
                    )
                    .ToArray();

                for (var x = 0; x < instructions.Length; x++)
                {
                    instructions[x].Operand = args.Default.Terraria.MainModule.Import(targetArray);
                    instructions[x].Next.Next.Next.Operand = args.Default.Terraria.MainModule.Import(targetField);
                }

                //Replace SendAnglerQuest()
                //Replace sendWater()
                //Replace syncPlayers()
                //Replace AddBan()
                var ourClass = _self.MainModule.Types.Where(x => x.Name == "Net").First();
                foreach (var rep in new string[] { "SendAnglerQuest", "sendWater", "syncPlayers", "AddBan" })
                {
                    var toBeReplaced = args.Default.Terraria.MainModule.Types
                        .SelectMany(x => x.Methods
                            .Where(y => y.HasBody)
                        )
                        .SelectMany(x => x.Body.Instructions)
                        .Where(x => x.OpCode == Mono.Cecil.Cil.OpCodes.Call
                            && x.Operand is MethodReference
                            && (x.Operand as MethodReference).Name == rep
                        )
                        .ToArray();

                    var replacement = ourClass.Methods.Where(x => x.Name == rep).First();
                    for (var x = 0; x < toBeReplaced.Length; x++)
                    {
                        toBeReplaced[x].Operand = args.Default.Terraria.MainModule.Import(replacement);
                    }
                }
            }
        }

        [Hook(HookOrder.NORMAL)]
        void OnDefaultServerStart(ref HookContext ctx, ref HookArgs.StartDefaultServer args)
        {
            ProgramLog.Log("Starting TDSM's slot server...");
            ctx.SetResult(HookResult.IGNORE);

            ServerCore.Server.StartServer();
        }

        [Hook(HookOrder.NORMAL)]
        void OnConfigLineRead(ref HookContext ctx, ref HookArgs.ConfigurationLine args)
        {
            switch (args.Key)
            {
                case "usewhitelist":
                    bool usewhitelist;
                    if (Boolean.TryParse(args.Value, out usewhitelist))
                    {
                        Server.WhitelistEnabled = usewhitelist;
                    }
                    break;
                case "vanilla-linux":
                    bool vanillaOnly;
                    if (Boolean.TryParse(args.Value, out vanillaOnly))
                    {
                        VanillaOnly = vanillaOnly;
                    }
                    break;
            }
        }

        [Hook(HookOrder.NORMAL)]
        void OnServerStateChange(ref HookContext ctx, ref HookArgs.ServerStateChange args)
        {
            //ProgramLog.Log("Server state changed to: " + args.ServerChangeState.ToString());

            if (args.ServerChangeState == ServerState.Initialising)
            {
                Server.Init();
            }
        }

        [Hook(HookOrder.NORMAL)]
        void OnNetMessageSendData(ref HookContext ctx, ref HookArgs.SendNetData args)
        {
            ctx.SetResult(HookResult.IGNORE);
            NewNetMessage.SendData(args.MsgType, args.RemoteClient, args.IgnoreClient, args.Text, args.Number,
                args.Number2, args.Number3, args.Number4, args.Number5);
        }

        private static int lastWritten = 0;
        [Hook(HookOrder.NORMAL)]
        void OnStatusTextChanged(ref HookContext ctx, ref HookArgs.StatusTextChanged args)
        {
            //There's no locking and two seperate threads, so we must use local variables incase of changes
            var statusText = Terraria.Main.statusText;
            var oldStatusText = Terraria.Main.oldStatusText;

            if (oldStatusText != statusText)
            {
                if (!String.IsNullOrEmpty(statusText))
                {
                    var ixA = oldStatusText.LastIndexOf(":");
                    var ixB = statusText.LastIndexOf(":");
                    if (ixA > -1 && ixB > -1)
                    {
                        var keyA = oldStatusText.Substring(0, ixA);
                        var keyB = statusText.Substring(0, ixB);
                        if (keyA == keyB)
                        {
                            if (lastWritten > 0)
                            {
                                var moveBack = oldStatusText.Length - ixA;
                                for (var x = 0; x < moveBack; x++)
                                    Console.Write("\b");
                            }

                            var len = statusText.Length - ixB;
                            Console.Write(statusText.Substring(ixB, len));
                            lastWritten += len;
                        }
                        else
                        {
                            Console.WriteLine();
                            lastWritten = 0;
                            Console.Write(statusText);

                            lastWritten += statusText.Length;
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
                        lastWritten += statusText.Length;
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
            ctx.SetResult(HookResult.IGNORE);
        }
    }
}