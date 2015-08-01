using System;
using System.Collections;
using System.Collections.Generic;
using TDSM.API.Logging;

namespace TDSM.API.Plugin
{
    public class LUAPlugin : BasePlugin
    {
        private NLua.Lua _ctx;

        internal bool IsValid;

        protected override void Initialized(object state)
        {
            base.Initialized(state);

            if (_ctx != null)
            {
                _ctx.Dispose();
            }

            _ctx = new NLua.Lua();
            _ctx.LoadCLRPackage();

            _ctx.RegisterFunction("AddCommand", this, this.GetType().GetMethod("AddCommand"));
            _ctx.RegisterFunction("ArraySort", this, this.GetType().GetMethod("ArraySort"));
            //_ctx.RegisterFunction("HookBase", this, this.GetType().GetMethods()
            //    .Where(x => x.Name == "HookBase" && x.GetParameters().Last().ParameterType == typeof(NLua.LuaFunction))
            //    .First());

            //Access level enum
            _ctx.NewTable("AccessLevel");
            var ac = _ctx.GetTable("AccessLevel");
            foreach (var val in Enum.GetValues(typeof(TDSM.API.Command.AccessLevel)))
            {
                var al = (TDSM.API.Command.AccessLevel)val;
                ac[al.ToString()] = (int)val;
            }

            _ctx.DoFile(Path);

            CallSelf("Initialized");

            var plg = _ctx["export"] as NLua.LuaTable;

            if (plg != null)
            {
                this.TDSMBuild = (int)(double)plg["TDSMBuild"];
                this.Author = plg["Author"] as String;
                this.Description = plg["Description"] as String;
                this.Name = plg["Name"] as String;
                this.Version = plg["Version"] as String;

                IsValid = true;

                var hooks = plg["Hooks"] as NLua.LuaTable;
                if (hooks != null)
                    foreach (KeyValuePair<Object, Object> hookTarget in hooks)
                    {
                        var hook = hookTarget.Key as String;
                        var hookPoint = PluginManager.GetHookPoint(hook);

                        if (hookPoint != null)
                        {
                            var details = hookTarget.Value as NLua.LuaTable;
                            var priority = (HookOrder)(details["Priority"] ?? HookOrder.NORMAL);
                            //var priority = FindOrder(details["Priority"] as String);

                            var del = _ctx.GetFunction(hookPoint.DelegateType, "export.Hooks." + hook + ".Call");
                            HookBase(hookPoint, priority, del); //TODO maybe fake an actual delegate as this is only used for registering or make a TDSM event info class 
                        }
                    }
            }
        }

        public void InternalCall<T>(ref HookContext ctx, ref T args)
        {
            //callback.Call(ctx, args);
        }

        public void HookBase(HookPoint hookPoint, HookOrder order, NLua.LuaFunction callback)
        {
            var targetType = hookPoint.DelegateType.GetGenericArguments()[0];
            var call = this.GetType().GetMethod("InternalCall");//.MakeGenericMethod(targetType);
            var act = typeof(HookAction<>).MakeGenericType(targetType);

            var inst = Activator.CreateInstance(act, call);

            //if (initialized)
            //hookPoint.HookBase(this, new HookAction<Object>((ref HookContext ctx, ref Object args) =>
            //{
            //    callback.Call(ctx, args);
            //}));
            //else
            //{
            //    lock (desiredHooks)
            //        desiredHooks.Add(new HookEntry
            //        {
            //            hookPoint = hookPoint,
            //            callback = new HookAction<Object>((ref HookContext ctx, ref Object args) =>
            //            {
            //                callback.Call(ctx, args);
            //            }),
            //            order = order
            //        });
            //}
        }

        public new Command.CommandInfo AddCommand(string prefix)
        {
            return base.AddCommand(prefix);
        }

        public void Call(ref HookContext ctx, ref object args, string function)
        {
            if (_ctx != null)
            {
                var fnc = _ctx.GetFunction("export.Hooks." + function + ".Call");
                if (fnc != null)
                {
                    var res = fnc.Call(this, ctx, args);
                    if (res != null && res.Length > 0)
                        foreach (var x in res)
                        {
                            if (x is HookContext)
                                ctx = (HookContext)x;
                        }
                }
            }
        }

        static HookOrder FindOrder(string text)
        {
            var vals = Enum.GetValues(typeof(HookOrder));
            foreach (var val in vals)
            {
                var ho = (HookOrder)val;
                if (ho.ToString() == text /*|| val.ToString() == text*/)
                    return ho;
            }
            return HookOrder.NORMAL;
        }

        protected override void Disabled()
        {
            base.Disabled();
            CallSelf("Disabled");
        }

        protected override void Enabled()
        {
            base.Enabled();
            CallSelf("Enabled");
        }

        protected override void Resumed(object state)
        {
            base.Resumed(state);
            CallSelf("Resumed");
        }

        protected override object SuspendedAndSaved()
        {
            //return base.SuspendedAndSaved();
            return CallSelf("SuspendedAndSaved") ?? base.SuspendedAndSaved();
        }

        protected override void WorldLoaded()
        {
            base.WorldLoaded();
            CallSelf("WorldLoaded");
        }

        private object[] CallSelf(string function, params object[] args)
        {
            try
            {
                if (_ctx != null)
                {
                    var fnc = _ctx.GetFunction("export." + function);
                    if (fnc != null)
                    {
                        return fnc.Call(this, args);
                    }
                }
            }
            catch (NLua.Exceptions.LuaScriptException e)
            {
                try
                {
                    if (e.IsNetException && e.InnerException != null)
                    {
                        ProgramLog.Log(e.InnerException, String.Format("Plugin {0} crashed in hook {1}", this.Name, function));
                    }
                    else
                    {
                        ProgramLog.Log(e, String.Format("Plugin {0} crashed in hook {1}", this.Name, function));
                    }
                }
                catch
                {
                }
            }
            return null;
        }

        protected override void Disposed(object state)
        {
            if (_ctx != null)
            {
                CallSelf("Disposed");
                //_ctx.Dispose();
                _ctx = null;
            }

            base.Disposed(state);
        }

        class LuaSorter : IComparer
        {
            private NLua.LuaFunction _callback;

            public LuaSorter(NLua.LuaFunction callback)
            {
                _callback = callback;
            }

            public int Compare(Object x, Object y)
            {
                var res = _callback.Call(x, y);
                return (int)(double)res[0];
            }
        }

        public Array ArraySort(Array arr, NLua.LuaFunction callback)
        {
            var comparer = new LuaSorter(callback);
            Array.Sort(arr, comparer);

            return arr;
        }
    }

    //public class LuaCallback
    //{
    //    public string _callback;

    //    public LuaCallback(string function)
    //    {
    //        _callback = function;
    //    }
    //}
}
