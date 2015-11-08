using System;
using OTA.Plugin;
using TDSM.Core.Misc;
using OTA;
using Terraria;
using System.Collections.Generic;
using OTA.Command;
using OTA.Misc;
using OTA.Extensions;

namespace TDSM.Core.Command.Commands
{
    public class InvasionCommand : CoreCommand
    {
        public override void Initialise()
        {
            Core.AddCommand("invasion")
                .WithDescription("Begins an invasion")
                .WithAccessLevel(AccessLevel.OP)
                .WithHelpText("goblin|frost|pirate|martian")
                .WithHelpText("-custom <npc id or name> <npc id or name> ...")
                .WithHelpText("stop|end|cancel")
                .WithPermissionNode("tdsm.invasion")
                .Calls(this.Invasion);
        }

        [Hook(HookOrder.NORMAL)]
        void OnInvasionNPCSpawn(ref HookContext ctx, ref HookArgs.InvasionNpcSpawn args)
        {
            if (Main.rand == null)
                Main.rand = new Random();

            if (Main.invasionType == Core._assignedInvasionType && Core._invasion != null)
            {
                lock (Core._invasion)
                {
                    if (Core._invasion.Count > 0)
                    {
                        var npc = Core._invasion.Random();

                        if (npc.Key != 0 && Main.rand.Next(Main.rand.Next(9)) == 0)
                        {
                            var id = NPC.NewNPC(args.X, args.Y, npc.Key);
                            if (npc.Value < 0)
                            {
                                Terraria.Main.npc[id].netDefaults(npc.Value);
                            }
                        }
                    }
                }

                //                ctx.SetResult(HookResult.ERASE);
            }
        }

        [Hook]
        void OnNPCKilled(ref HookContext ctx, ref HookArgs.NpcKilled args)
        {
            if (Main.invasionType == Core._assignedInvasionType && Core._invasion != null)
            {
                lock (Core._invasion)
                {
                    if (Core._invasion.ContainsKey(args.Type))
                    {
                        Main.invasionSize--;
                        if (Main.invasionSize <= 0)
                        {
                            lock (Core._invasion)
                                Core._invasion.Clear();
                            Main.invasionSize = 0;
                            Main.invasionType = 0;
                            NetMessage.SendData(25, -1, -1, "The invasion was defeated!", 255, 175f, 75f, 255f);
                        }
                    }
                }
            }

            if (Core._likeABoss)
            {
                //NPC death message
                //Check if all bosses are inactive
            }
        }

        [Hook]
        void OnInvasionWarning(ref HookContext ctx, ref HookArgs.InvasionWarning args)
        {
            if (Main.invasionType == Core._assignedInvasionType && Core._invasion != null)
            {
                if (Main.invasionSize > 0)
                {
                    string message = null;
                    if (Main.invasionX < (double)Main.spawnTileX)
                    {
                        //West
                        if (!Core._notfInbound)
                        {
                            Core._notfInbound = true;
                            message = "An invasion is approaching from the west!";
                        }
                    }
                    else if (Main.invasionX > (double)Main.spawnTileX)
                    {
                        //East
                        if (!Core._notfInbound)
                        {
                            Core._notfInbound = true;
                            message = "An invasion is approaching from the east!";
                        }
                    }
                    else
                    {
                        //Arrived
                        message = "The invasion has arrived!";
                    }

                    if (null != message)
                        NetMessage.SendData(25, -1, -1, message, 255, 175f, 75f, 255f);
                }

                ctx.SetResult(HookResult.IGNORE);
            }
        }

        public void Invasion(ISender sender, ArgumentList args)
        {
            if (Main.rand == null)
                Main.rand = new Random();

            if (Main.invasionType == 0)
            {
                var custom = args.TryPop("-custom");
                var first = args.TryPop("-f");
                if (custom)
                {
                    if (args.Count > 0)
                    {
                        if (Core._invasion == null)
                            Core._invasion = new Dictionary<int, int>();

                        var npcIds = new List<Int32>();
                        var c = 0;
                        while (c < args.Count)
                        {
                            int npcType;
                            string npc;

                            if (args.TryGetInt(c, out npcType))
                            {
                                Core._invasion.Add(npcType, 0);
                            }
                            else if (args.TryGetString(c, out npc))
                            {
                                var match = Definitions.DefinitionManager.FindNPC(npc);
                                if (match.Length == 1 || first)
                                {
                                    Core._invasion.Add(match[0].Id, match[0].NetId);
                                }
                                else if (match.Length == 0)
                                {
                                    sender.Message("Cannot find a NPC by `{0}`", npc);
                                    return;
                                }
                                else
                                {
                                    sender.Message("Found too many NPC's containing `{0}`", npc);
                                    return;
                                }
                            }
                            else
                            {
                                throw new CommandError("Expected a NPC id or name.");
                            }
                            c++;
                        }

                        //Schedule...
                        //if (!_customInvasion.IsEmpty()) _customInvasion.Enabled = false;
                        //_customInvasion = new Task()
                        //{
                        //    Trigger = 10,
                        //    Data = npcIds,
                        //    Method = (task) =>
                        //    {
                        //        var ids = task.Data as List<Int32>;
                        //        for (var x = 0; x < 5; x++)
                        //        {
                        //            var ix = Main.rand.Next(0, ids.Count - 1);
                        //            var pos = FindPositionOutOfSight();

                        //            if (pos.HasValue)
                        //            {

                        //            }
                        //        }
                        //    }
                        //};


                        //                        if (_invasion != null)
                        //                            lock (_invasion)
                        //                                _invasion = npcIds;
                        //                        else
                        //                            _invasion = npcIds;

                        Core._notfInbound = false;
                        Main.StartInvasion(Core._assignedInvasionType);
                        sender.Message("Invasion started");
                    }
                    else
                        throw new CommandError("Expected npc type or name");
                }
                else
                {
                    var txt = args.GetString(0).ToLower();
                    var vals = Enum.GetValues(typeof(InvasionType));
                    foreach (InvasionType it in vals)
                    {
                        if (it.ToString().ToLower() == txt)
                        {
                            Main.StartInvasion((int)it);
                            sender.Message("A {0} invasion has begun.", it);
                            return;
                        }
                    }

                    sender.Message("No such invasion of type {0}", txt);
                }
            }
            else
            {
                if (args.TryPop("end") || args.TryPop("stop") || args.TryPop("cancel"))
                {
                    //if (!_customInvasion.IsEmpty()) _customInvasion.Enabled = false;
                    Main.invasionType = 0;
                    if (Core._invasion != null)
                        lock (Core._invasion)
                            Core._invasion.Clear();
                    sender.Message("The invasion has now been stopped.");
                }
                else
                    sender.Message("An invasion is already under way.");
            }
        }
    }
}

