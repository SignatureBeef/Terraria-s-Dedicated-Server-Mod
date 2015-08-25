using System;
using System.Collections.Generic;
using TDSM.API.Command;
using TDSM.API.Misc;
using TDSM.API.Plugin;
using Terraria;
using TDSM.API;
using System.Linq;

namespace TDSM.Core
{
    public partial class Entry
    {
        //private Task _customInvasion;
        private Dictionary<Int32,Int32> _invasion;
        private int _assignedInvasionType = TDSM.API.Callbacks.NPCCallback.AssignInvasionType();
        private bool _notfInbound;

        [Hook(HookOrder.NORMAL)]
        void OnInvasionNPCSpawn(ref HookContext ctx, ref HookArgs.InvasionNPCSpawn args)
        {
            if (Main.rand == null)
                Main.rand = new Random();
            
            if (Main.invasionType == _assignedInvasionType && _invasion != null)
            {
                lock (_invasion)
                {
                    if (_invasion.Count > 0)
                    {
                        var npc = _invasion.Random();

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

        class CyclicQueue<T>
        {
            private T[] _source;
            private int _index;

            public int Count
            {
                get
                {
                    return _source.Length;
                }
            }

            public CyclicQueue(T[] source)
            {
                this._source = source;
                this._index = -1;
            }

            public CyclicQueue(IEnumerable<T> source)
            {
                this._source = source.ToArray();
                this._index = -1;
            }

            public T Next()
            {
                var ix = System.Threading.Interlocked.Increment(ref _index);
                System.Threading.Interlocked.CompareExchange(ref _index, -1, _source.Length - 1);
                return this._source[ix];
            }
        }

        private bool _likeABoss;
        private static CyclicQueue<String> _labDeathMessages = new CyclicQueue<String>((new string[]
            {
                " jumped out a window",
                " had to approve memo's",
                " failed to promote synergy",
                " cried deeply",
                " was too busy eating chicken strips",
                " forgot to approve memo's",
                " crashed into the sun",
                " blacked out in a sewer",
                " swallowed sadness"
            }).Shuffle());

        [Hook]
        void OnNPCKilled(ref HookContext ctx, ref HookArgs.NPCKilled args)
        {
            if (Main.invasionType == _assignedInvasionType && _invasion != null)
            {
                lock (_invasion)
                {
                    if (_invasion.ContainsKey(args.Type))
                    {
                        Main.invasionSize--;
                        if (Main.invasionSize <= 0)
                        {
                            lock (_invasion)
                                _invasion.Clear();
                            Main.invasionSize = 0;
                            Main.invasionType = 0;
                            NetMessage.SendData(25, -1, -1, "The invasion was defeated!", 255, 175f, 75f, 255f);
                        }
                    }
                }
            }

            if (_likeABoss)
            {
                //NPC death message
                //Check if all bosses are inactive
            }
        }

        [Hook]
        void OnInvasionWarning(ref HookContext ctx, ref HookArgs.InvasionWarning args)
        {
            if (Main.invasionType == _assignedInvasionType && _invasion != null)
            {
                if (Main.invasionSize > 0)
                {
                    string message = null;
                    if (Main.invasionX < (double)Main.spawnTileX)
                    {
                        //West
                        if (!_notfInbound)
                        {
                            _notfInbound = true;
                            message = "An invasion is approaching from the west!";
                        }
                    }
                    else if (Main.invasionX > (double)Main.spawnTileX)
                    {
                        //East
                        if (!_notfInbound)
                        {
                            _notfInbound = true;
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
                        if (_invasion == null)
                            _invasion = new Dictionary<int, int>();
                        
                        var npcIds = new List<Int32>();
                        var c = 0;
                        while (c < args.Count)
                        {
                            int npcType;
                            string npc;

                            if (args.TryGetInt(c, out npcType))
                            {
                                _invasion.Add(npcType, 0);
                            }
                            else if (args.TryGetString(c, out npc))
                            {
                                var match = Definitions.DefinitionManager.FindNPC(npc);
                                if (match.Length == 1 || first)
                                {
                                    _invasion.Add(match[0].Id, match[0].NetId);
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

                        _notfInbound = false;
                        Main.StartInvasion(_assignedInvasionType);
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
                    if (_invasion != null)
                        lock (_invasion)
                            _invasion.Clear();
                    sender.Message("The invasion has now been stopped.");
                }
                else
                    sender.Message("An invasion is already under way.");
            }
        }
    }
}
