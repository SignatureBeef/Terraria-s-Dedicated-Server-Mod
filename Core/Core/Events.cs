using System;
using OTA.Plugin;
using TDSM.Core.ServerCharacters;
using TDSM.Core.Plugin.Hooks;
using OTA.Command;
using OTA.Logging;
using Terraria;
using Terraria.Initializers;
using OTA;
using TDSM.Core.Data.Management;
using System.IO;
using TDSM.Core.Command.Commands;
using TDSM.Core.Net.Web;
using TDSM.Core.Data;

namespace TDSM.Core
{
    public partial class Entry
    {
        [Hook(HookOrder.LATE)]
        private void Command(ref HookContext ctx, ref TDSMHookArgs.ServerCommand args)
        {
            if (args.Prefix == "!") return;

            //Perhaps here we can use the player's PluginData, and simply store a string for the console
            if (ctx.Sender is Player)
            {
                if (CommandDictionary.ContainsKey(ctx.Player.name))
                    CommandDictionary[ctx.Player.name] = "/" + args.Prefix + " " + args.ArgumentString;
                else
                    CommandDictionary.Add(ctx.Player.name, "/" + args.Prefix + " " + args.ArgumentString);
            }
            else if (ctx.Sender is ConsoleSender)
            {
                if (CommandDictionary.ContainsKey("CONSOLE"))
                    CommandDictionary["CONSOLE"] = args.Prefix + " " + args.ArgumentString;
                else
                    CommandDictionary.Add("CONSOLE", args.Prefix + " " + args.ArgumentString);
            }
        }

        //        [Hook(HookOrder.NORMAL)]
        //        void OnInventoryItemReceived(ref HookContext ctx, ref HookArgs.InventoryItemReceived args)
        //        {
        //#if TDSMSever
        //            if (Server.ItemRejections.Count > 0)
        //            {
        //                if (args.Item != null)
        //                {
        //                    if (Server.ItemRejections.Contains(args.Item.name) || Server.ItemRejections.Contains(args.Item.type.ToString()))
        //                    {
        //                        if (!String.IsNullOrEmpty(args.Item.name))
        //                        {
        //                            ctx.SetKick(args.Item.name + " is not allowed on this server.");
        //                        }
        //                        else
        //                        {
        //                            ctx.SetKick("Item type " + args.Item.type.ToString() + " is not allowed on this server.");
        //                        }
        //                    }
        //                }
        //            }
        //#endif
        //        }

        [Hook(HookOrder.NORMAL)]
        void OnPlayerJoin(ref HookContext ctx, ref HookArgs.PlayerEnteredGame args)
        {
            //The player may randomly disconnect at any time, and if it's before the point of saving then the data may be lost.
            //So we must ensure the data is saved.
            //ServerCharacters.CharacterManager.EnsureSave = true;

            using (var dbCtx = new TContext())
            {
                if (CharacterManager.Mode == CharacterMode.UUID)
                {
                    CharacterManager.LoadForAuthenticated(dbCtx, ctx.Player, !AllowSSCGuestInfo);
                }
                else if (CharacterManager.Mode == CharacterMode.AUTH)
                {
                    if (!String.IsNullOrEmpty(ctx.Player.GetAuthenticatedAs()))
                    {
                        CharacterManager.LoadForAuthenticated(dbCtx, ctx.Player, !AllowSSCGuestInfo);
                    }
                    else
                    {
                        if (!AllowSSCGuestInfo)
                        {
                            //                        ProgramLog.Debug.Log("This fella, yeah him; clear his inventory");
                            CharacterManager.LoadForGuest(ctx.Player);
                        }
                    }
                }
            }
        }

        [Hook(HookOrder.LATE)] //This is required so this can be called last, in order for us to know if it's been cancelled or not
        void OnPlayerAuthenticated(ref HookContext ctx, ref TDSMHookArgs.PlayerAuthenticationChanged args)
        {
            if (ctx.Client.State >= 4 && CharacterManager.Mode == CharacterMode.AUTH)
            {
                using (var dbCtx = new TContext())
                {
                    CharacterManager.LoadForAuthenticated(dbCtx, ctx.Player, !AllowSSCGuestInfo);
                }

                if (AllowSSCGuestInfo)
                {
                    ctx.Player.SetSSCReadyForSave(true); //Since we aren't issuing out data, and accepting it, we can save it.
                }
            }
        }

        [Hook(HookOrder.NORMAL)]
        void OnStartCommandProcessing(ref HookContext ctx, ref HookArgs.StartCommandProcessing args)
        {
            ctx.SetResult(HookResult.IGNORE);

            if (Console.IsInputRedirected)
            {
                ProgramLog.Admin.Log("Console input redirection has been detected.");
                return;
            }

            (new OTA.Misc.ProgramThread("Command", ListenForCommands)).Start();
        }

        [Hook(HookOrder.NORMAL)]
        void OnPlayerDisconnected(ref HookContext ctx, ref HookArgs.PlayerLeftGame args)
        {
            if (Terraria.Main.ServerSideCharacter)
            {
                using (var dbCtx = new TContext())
                {
                    CharacterManager.SavePlayerData(dbCtx, true, ctx.Player);
                }
            }
            if (ctx.Player != null)
            {
                if (CommandDictionary.ContainsKey(ctx.Player.name))
                {
                    CommandDictionary.Remove(ctx.Player.name);
                }
                //ProgramLog.Log("{0}", ctx.Player.name); //, args.Prefix + " " + args.ArgumentString);
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

        [Hook(HookOrder.NORMAL)]
        void OnChat(ref HookContext ctx, ref TDSMHookArgs.PlayerChat args)
        {
            if (args.Message.Length > 0 && args.Message.Substring(0, 1).Equals("/"))
            {
                ProgramLog.Log(ctx.Player.name + " sent command: " + args.Message);
                ctx.SetResult(HookResult.IGNORE);

                CommandParser.ParsePlayerCommand(ctx.Player, args.Message);
            }
        }

        [Hook(HookOrder.NORMAL)]
        void OnServerTick(ref HookContext ctx, ref HookArgs.ServerTick args)
        {
            if (Terraria.Main.ServerSideCharacter)
            {
                ServerCharacters.CharacterManager.SaveAll();
            }

            TDSM.Core.Data.Management.SaveManager.OnUpdate();
            TDSM.Core.Data.Management.BackupManager.OnUpdate();
        }

        [Hook(HookOrder.NORMAL)]
        void OnUpdateServer(ref HookContext ctx, ref HookArgs.ServerUpdate args)
        {
            if (args.State == MethodState.End)
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
                //    //    //if(OTA.Callbacks.Netplay.slots[i].conn != null && OTA.Callbacks.Netplay.slots[i].conn.Active )
                //    //    //    OTA.Callbacks.Netplay.slots[i].conn.s
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

        [Hook(HookOrder.NORMAL)]
        void OnNPCSpawned(ref HookContext ctx, ref HookArgs.NpcSpawn args)
        {
            if (StopNPCSpawning)
                ctx.SetResult(HookResult.IGNORE);
        }

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
        void OnConfigLineRead(ref HookContext ctx, ref HookArgs.ConfigurationFileLineRead args)
        {
            //Ensure command line argument supersede config options - hosting providers can use this 
            if (LaunchInitializer.HasParameter(new string[] { "-" + args.Key }))
            {
                //                ProgramLog.Log("Ignoring overridden config property " + args.Key);
                return;
            }
            switch (args.Key)
            {
                case "whitelist":
                    bool usewhitelist;
                    if (Boolean.TryParse(args.Value, out usewhitelist))
                    {
                        WhitelistEnabled = usewhitelist;
                    }
                    break;
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
                    _webServerAddress = args.Value;
                    OTA.Web.WebServer.Start(args.Value);
                    break;
                case "web-server-provider":
                    _webServerProvider = args.Value;
                    break;
            //                case "web-server-serve-files":
            //                    bool serveFiles;
            //                    if (Boolean.TryParse(args.Value, out serveFiles))
            //                    {
            //                        //WebInterface.WebServer.ServeWebFiles = serveFiles;
            //                    }
            //                    break;
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
                        ProgramLog.Admin.Log("SSC mode is: " + characterMode);

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
                case "api-showplugins":
                    bool apiShowPlugins;
                    if (Boolean.TryParse(args.Value, out apiShowPlugins))
                    {
                        OTA.Web.API.PublicController.ShowPlugins = apiShowPlugins;
                    }
                    break;
                case "ssc-save-interval":
                    int sscSaveInterval;
                    if (Int32.TryParse(args.Value, out sscSaveInterval))
                    {
                        CharacterManager.SaveInterval = sscSaveInterval;
                    }
                    break;

                case "logs-to-keep":
                    int logsToKeep;
                    if (Int32.TryParse(args.Value, out logsToKeep))
                    {
                        TDSM.Core.Data.Management.LogManagement.LogsToLeave = logsToKeep;
                    }
                    break;
                case "backup-interval-min":
                    int backupInterval;
                    if (Int32.TryParse(args.Value, out backupInterval))
                    {
                        TDSM.Core.Data.Management.BackupManager.BackupIntervalMinutes = backupInterval;
                    }
                    break;
                case "backup-expiry-min":
                    int backupExpiry;
                    if (Int32.TryParse(args.Value, out backupExpiry))
                    {
                        TDSM.Core.Data.Management.BackupManager.BackupExpiryMinutes = backupExpiry;
                    }
                    break;
                case "compress-backups":
                    bool compressBackups;
                    if (Boolean.TryParse(args.Value, out compressBackups))
                    {
                        TDSM.Core.Data.Management.BackupManager.CompressBackups = compressBackups;
                    }
                    break;
                case "copy-backups":
                    bool copyBackups;
                    if (Boolean.TryParse(args.Value, out copyBackups))
                    {
                        TDSM.Core.Data.Management.BackupManager.CopyBackups = copyBackups;
                    }
                    break;
                case "save-interval-min":
                    int saveInterval;
                    if (Int32.TryParse(args.Value, out saveInterval))
                    {
                        const Int32 MinSaveInterval = 1;
                        if (saveInterval < MinSaveInterval)
                        {
                            saveInterval = MinSaveInterval;
                            ProgramLog.Admin.Log("The save interval cannot be disabled and is now set to {0} minute", MinSaveInterval);
                        }

                        TDSM.Core.Data.Management.SaveManager.SaveIntervalMinutes = saveInterval;
                    }
                    break;
            }
        }

        [Hook]
        void OnCMDArgsReady(ref HookContext ctx, ref HookArgs.ParseCommandLineArguments args)
        {
            string tmp;
            bool tmp1;

            //            if (!String.IsNullOrEmpty(tmp = LaunchInitializer.TryParameter(new string[] { "-world" }))
            //                && !File.Exists(tmp))
            //            {
            //                ProgramLog.Error.Log("Command line world file not found at: {0}\nPress the [Y] key to continue", tmp);
            //                if (Console.ReadKey().Key != ConsoleKey.Y)
            //                {
            //                    
            //                }
            //            }

            if (!String.IsNullOrEmpty(tmp = LaunchInitializer.TryParameter(new string[] { "-whitelist" }))
                && Boolean.TryParse(tmp, out tmp1))
                WhitelistEnabled = tmp1;

            if (!String.IsNullOrEmpty(tmp = LaunchInitializer.TryParameter(new string[] { "-heartbeat" }))
                && Boolean.TryParse(tmp, out tmp1))
                EnableHeartbeat = tmp1;

            if (!String.IsNullOrEmpty(tmp = LaunchInitializer.TryParameter(new string[] { "-server-list" }))
                && Boolean.TryParse(tmp, out tmp1))
                Heartbeat.PublishToList = tmp1;

            if (!String.IsNullOrEmpty(tmp = LaunchInitializer.TryParameter(new string[] { "-server-list-name" })))
                Heartbeat.ServerName = tmp;

            if (!String.IsNullOrEmpty(tmp = LaunchInitializer.TryParameter(new string[] { "-server-list-desc" })))
                Heartbeat.ServerDescription = tmp;

            if (!String.IsNullOrEmpty(tmp = LaunchInitializer.TryParameter(new string[] { "-server-list-domain" })))
                Heartbeat.ServerDomain = tmp;

            if (!String.IsNullOrEmpty(tmp = LaunchInitializer.TryParameter(new string[] { "-rcon-hash-nonce" })))
                RConHashNonce = tmp;

            if (!String.IsNullOrEmpty(tmp = LaunchInitializer.TryParameter(new string[] { "-rcon-bind-address" })))
                RConBindAddress = tmp;

            if (!String.IsNullOrEmpty(tmp = LaunchInitializer.TryParameter(new string[] { "-web-server-bind-address" })))
                OTA.Web.WebServer.Start(tmp);

            if (!String.IsNullOrEmpty(tmp = LaunchInitializer.TryParameter(new string[] { "-web-server-provider" })))
                _webServerProvider = tmp;

            if (!String.IsNullOrEmpty(tmp = LaunchInitializer.TryParameter(new string[] { "-pid-file" })))
                ProcessPIDFile(tmp);

            if (!String.IsNullOrEmpty(tmp = LaunchInitializer.TryParameter(new string[] { "-cheat-detection" }))
                && Boolean.TryParse(tmp, out tmp1))
                EnableCheatProtection = tmp1;

            if (!String.IsNullOrEmpty(tmp = LaunchInitializer.TryParameter(new string[] { "-log-rotation" }))
                && Boolean.TryParse(tmp, out tmp1))
                ProgramLog.LogRotation = tmp1;

            CharacterMode characterMode;
            if (!String.IsNullOrEmpty(tmp = LaunchInitializer.TryParameter(new string[] { "-server-side-characters" }))
                && CharacterMode.TryParse(tmp, out characterMode))
            {
                Terraria.Main.ServerSideCharacter = characterMode != CharacterMode.NONE;
                CharacterManager.Mode = characterMode;
                ProgramLog.Admin.Log("SSC are enabled with mode " + characterMode);

                Hook(HookPoints.ReceiveNetMessage, OnNetMessageReceived);
            }

            int accessLevel;
            if (!String.IsNullOrEmpty(tmp = LaunchInitializer.TryParameter(new string[] { "-exitaccesslevel" }))
                && Int32.TryParse(tmp, out accessLevel))
                ExitAccessLevel = accessLevel;

            if (!String.IsNullOrEmpty(tmp = LaunchInitializer.TryParameter(new string[] { "-ssc-allow-guest-info" }))
                && Boolean.TryParse(tmp, out tmp1))
                AllowSSCGuestInfo = tmp1;
        }

        [Hook(HookOrder.LATE)]
        //Late so other plugins can perform alterations
        void OnPlayerKilled(ref HookContext ctx, ref HookArgs.PlayerKilled args)
        {
            if (_likeABoss)
            {
                if (Terraria.Main.rand == null) Terraria.Main.rand = new Random();

                if (ctx.Player.talkNPC > -1 && Terraria.Main.rand.Next(_labDeathMessages.Count - 1) == 1)
                {
                    args.DeathText = " was too busy talking";
                }
                //                else if (Terraria.Main.rand.Next(_labDeathMessages.Count - 1) == 1)
                //                {
                //
                //                }
                // forgot [NPC]'s birthday
                // tried to hit on [NPC]

                else
                {
                    args.DeathText = _labDeathMessages.Next();
                }
                args.DeathText = ctx.Player.name + args.DeathText;
                ctx.SetResult(HookResult.CONTINUE);
                ProgramLog.Death.Log(args.DeathText);
            }
            else
            {
                //Standard death log
                ProgramLog.Death.Log(ctx.Player.name + args.DeathText);
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
                        list = list + ", " + Main.player[i].name;
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

                if (BackupManager.BackupsEnabled && BackupManager.BackupIntervalMinutes < SaveManager.SaveIntervalMinutes)
                {
                    ProgramLog.Admin.Log("[Warning] Backup interval is smaller than the save interval.");
                }
            }
            if (args.ServerChangeState == ServerState.Stopping)
            {
                RunComponent(ComponentEvent.ServerStopping);
                if (!String.IsNullOrEmpty(RConBindAddress))
                {
                    ProgramLog.Log("Stopping RCON Server");
                    RemoteConsole.RConServer.Stop();
                }
            }
            if (args.ServerChangeState == ServerState.Starting)
            {
                RunComponent(ComponentEvent.ServerStarting);
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

            this.AddCommand("webauth")
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



        //        [Hook]
        void OnNetMessageReceived(ref HookContext ctx, ref HookArgs.ReceiveNetMessage args)
        {
            if (Terraria.Main.ServerSideCharacter)
            {
                switch ((Packet)args.PacketId)
                {
                    case Packet.INVENTORY_DATA:
                        if (!AllowSSCGuestInfo && !ctx.Player.IsAuthenticated())
                            ctx.SetResult(HookResult.IGNORE);
                        break;

                    case Packet.PLAYER_MANA_UPDATE:
                        if (!AllowSSCGuestInfo && !ctx.Player.IsAuthenticated())
                            ctx.SetResult(HookResult.IGNORE);
                        break;

                    case Packet.PLAYER_HEALTH_UPDATE:
                        if (!AllowSSCGuestInfo && !ctx.Player.IsAuthenticated())
                            ctx.SetResult(HookResult.IGNORE);
                        break;
                }
            }
        }

        [Hook(HookOrder.FIRST)]
        void OnPlayerDataReceived(ref HookContext ctx, ref TDSMHookArgs.PlayerDataReceived args)
        {
            //            //If the player is not authenticated, then ensure they are reset
            //            if (!AllowSSCGuestInfo && !ctx.Player.IsAuthenticated)
            //            {
            //        
            //            }

            if (WhitelistEnabled)
            {
                var name = WhiteListCommand.Prefix_WhitelistName + args.Name;
                var ip = WhiteListCommand.Prefix_WhitelistIp + ctx.Client.RemoteIPAddress();

                if (!Whitelist.Contains(name) && !Whitelist.Contains(ip))
                {
                    ctx.SetKick(args.Name + ", You are not whitelisted");
                }
            }
        }

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

                        /*if (SocialAPI.Network != null)
                            writer.Write(SocialAPI.Network.GetLobbyId());
                        else*/
                        writer.Write(0);

                        break;
                }
            }
        }
    }
}

