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
using OTA.Extensions;
using OTA.Data;

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

#if ENTITY_FRAMEWORK_7
            using (var dbCtx = new TContext())
#elif DAPPER
            using (var dbCtx = DatabaseFactory.CreateConnection())
#endif
            {
                using (var txn = dbCtx.BeginTransaction())
                {
                    if (CharacterManager.Mode == CharacterMode.UUID)
                    {
                        CharacterManager.LoadForAuthenticated(dbCtx, txn, ctx.Player, !Config.SSC_AllowGuestInfo);
                    }
                    else if (CharacterManager.Mode == CharacterMode.AUTH)
                    {
                        if (!String.IsNullOrEmpty(ctx.Player.GetAuthenticatedAs()))
                        {
                            CharacterManager.LoadForAuthenticated(dbCtx, txn, ctx.Player, !Config.SSC_AllowGuestInfo);
                        }
                        else
                        {
                            if (!Config.SSC_AllowGuestInfo)
                            {
                                CharacterManager.LoadForGuest(ctx.Player);
                            }
                        }
                    }
                    txn.Commit();
                }
            }
        }

        [Hook(HookOrder.LATE)] //This is required so this can be called last, in order for us to know if it's been cancelled or not
        void OnPlayerAuthenticated(ref HookContext ctx, ref TDSMHookArgs.PlayerAuthenticationChanged args)
        {
            if (ctx.Client.State >= 4 && CharacterManager.Mode == CharacterMode.AUTH)
            {
#if ENTITY_FRAMEWORK_7
                using (var dbCtx = new TContext())
#elif DAPPER
                using (var dbCtx = DatabaseFactory.CreateConnection())
#endif
                {
                    using (var txn = dbCtx.BeginTransaction())
                    {
                        CharacterManager.LoadForAuthenticated(dbCtx, txn, ctx.Player, !Config.SSC_AllowGuestInfo);
                        txn.Commit();
                    }
                }

                if (Config.SSC_AllowGuestInfo)
                {
                    ctx.Player.SetSSCReadyForSave(true); //Since we aren't issuing out data, and accepting it, we can save it.
                }
            }
        }

        [Hook(HookOrder.NORMAL)]
        void OnPlayerDisconnected(ref HookContext ctx, ref HookArgs.PlayerLeftGame args)
        {
            if (Terraria.Main.ServerSideCharacter)
            {
#if ENTITY_FRAMEWORK_7
                using (var dbCtx = new TContext())
#elif DAPPER
                using (var dbCtx = DatabaseFactory.CreateConnection())
#endif
                {
                    using (var txn = dbCtx.BeginTransaction())
                    {
                        CharacterManager.SavePlayerData(dbCtx, txn, true, ctx.Player);
                        txn.Commit();
                    }
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
                Loggers.Death.Log(args.DeathText);
            }
            else
            {
                //Standard death log
                Loggers.Death.Log(ctx.Player.name + args.DeathText);
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
                RunComponent(ComponentEvent.ServerInitialising);
                if (BackupManager.BackupsEnabled && BackupManager.BackupIntervalMinutes < SaveManager.SaveIntervalMinutes)
                {
                    ProgramLog.Admin.Log("[Warning] Backup interval is smaller than the save interval.");
                }
            }
            else if (args.ServerChangeState == ServerState.Starting)
            {
                RunComponent(ComponentEvent.ServerStarting);
            }
            else if (args.ServerChangeState == ServerState.Stopping)
            {
                RunComponent(ComponentEvent.ServerStopping);
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

            if (Config.WhitelistEnabled)
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

        [Hook]
        void OnLoadConfiguration(ref HookContext ctx, ref HookArgs.LoadConfigurationFile args)
        {
            ctx.SetResult(HookResult.IGNORE);
        }
    }
}

