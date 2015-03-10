using System;
using System.Collections.Generic;
using tdsm.api.Command;
using tdsm.api.Misc;
using tdsm.api.Plugin;
using Terraria;

namespace tdsm.core
{
    public partial class Entry
    {
        //private Task _customInvasion;
        private List<Int32> _invasion;
        private int _assignedInvasionType = tdsm.api.Callbacks.NPCCallback.AssignInvasionType();

        [Hook(HookOrder.NORMAL)]
        void OnInvasionNPCSpawn(ref HookContext ctx, ref HookArgs.InvasionNPCSpawn args)
        {
            if (Main.invasionType == _assignedInvasionType && _invasion != null)
            {
                int npcId = 0;
                lock (_invasion)
                {
                    if (_invasion.Count > 0)
                    {
                        var ix = Main.rand.Next(0, _invasion.Count - 1);
                        npcId = _invasion[ix];
                    }
                }

                if (npcId != 0 && Main.rand.Next(Main.rand.Next(9)) == 0)
                {
                    NPC.NewNPC(args.X, args.Y, npcId);
                }
            }
        }

        public void Invasion(ISender sender, ArgumentList args)
        {
            if (Main.invasionType == 0)
            {
                var custom = args.TryPop("-custom");
                if (!custom)
                {
                    if (args.Count > 0)
                    {
                        var npcIds = new List<Int32>();
                        while (args.Count > 0)
                        {
                            int npcType;
                            string npc;

                            if (args.TryGetInt(0, out npcType))
                            {
                                npcIds.Add(npcType);
                            }
                            else if (args.TryGetString(0, out npc))
                            {
                                var match = Definitions.DefinitionManager.FindNPC(npc);
                                if (match.Length == 1)
                                {
                                    npcIds.Add(match[0].Id);
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


                        if (_invasion != null) lock (_invasion) _invasion = npcIds;
                        else _invasion = npcIds;

                        Main.StartInvasion(_assignedInvasionType);
                    }
                    else throw new CommandError("Expected npc type or name");
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
                    if (_invasion != null) lock (_invasion) _invasion.Clear();
                    sender.Message("The invasion has now been stopped.");
                }
                else sender.Message("An invasion is already under way.");
            }
        }
    }
}
