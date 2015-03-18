using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using tdsm.api.Command;
using tdsm.api.Plugin;

namespace tdsm.api
{
    /// <summary>
    /// Plugin class, used as base for plugin extensions
    /// </summary>
    public abstract class BasePlugin
    {
        /// <summary>
        /// Name of the plugin
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Plugin description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Plugin author
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// Plugin version
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// Latest compatible TDSM build
        /// </summary>
        public int TDSMBuild { get; set; }
        /// <summary>
        /// Whether to enable the plugin right after loading, so it could intercept the PluginLoadRequest hook for other plugins
        /// </summary>
        public bool EnableEarly { get; set; }
        /// <summary>
        /// Status text displayed by some of the /plugin commands
        /// </summary>
        public string Status { get; set; }

        internal string Path { get; set; }
        internal DateTime PathTimestamp { get; set; }

        internal Assembly Assembly { get; set; }
        internal AppDomain _domain;

        /// <summary>
        /// Whether this plugin is enabled or not
        /// </summary>
        public bool IsEnabled
        {
            get { return enabled == 1; }
        }

        public bool IsDisposed
        {
            get { return disposed == 1; }
        }

        internal bool HasRunningCommands
        {
            get { return (runningCommands - pausedCommands - threadInCommand) > 0; }
        }

        internal volatile bool initialized;
        internal int disposed;
        internal int enabled;
        internal int informedOfWorld;
        internal int runningCommands;
        internal int pausedCommands;
        internal ManualResetEvent commandPauseSignal;

        internal Dictionary<string, CommandInfo> commands = new Dictionary<string, CommandInfo>();
        internal HashSet<HookPoint> hooks = new HashSet<HookPoint>();

        internal struct HookEntry
        {
            public HookPoint hookPoint;
            public Delegate callback;
            public HookOrder order;
        }

        internal List<HookEntry> desiredHooks = new List<HookEntry>();

        protected BasePlugin()
        {
            Description = "";
            Author = "";
            Version = "";
        }

        /// <summary>
        /// A callback for initializing the plugin's resources and subscribing to hooks.
        /// <param name='state'>
        /// A state object returned from OnDispose by a previous instance of the plugin, or null otherwise.
        /// </param>
        /// </summary>
        protected virtual void Initialized(object state) { }

        /// <summary>
        /// A callback for disposing of any resources held by the plugin.
        /// </summary>
        /// <param name='state'>
        /// A state object previously returned from SaveState to be disposed of as well.
        /// </param>
        protected virtual void Disposed(object state) { }

        protected virtual object SuspendedAndSaved()
        {
            return null;
        }

        protected virtual void Resumed(object state) { }

        /// <summary>
        /// Enable routines, usually no more than enabled announcement and registering hooks
        /// </summary>
        protected virtual void Enabled() { }

        /// <summary>
        /// Disabling the plugin, usually announcement
        /// </summary>
        protected virtual void Disabled() { }

        protected virtual void WorldLoaded() { }

        public void Hook<T>(HookPoint<T> hookPoint, HookAction<T> callback)
        {
            Hook<T>(hookPoint, HookOrder.NORMAL, callback);
        }

        public void Hook<T>(HookPoint<T> hookPoint, HookOrder order, HookAction<T> callback)
        {
            HookBase(hookPoint, order, callback);
        }

        public void HookBase(HookPoint hookPoint, HookOrder order, Delegate callback)
        {
            if (initialized)
                hookPoint.HookBase(this, callback);
            else
            {
                lock (desiredHooks)
                    desiredHooks.Add(new HookEntry { hookPoint = hookPoint, callback = callback, order = order });
            }
        }

        //public void HookBase(HookPoint hookPoint, HookOrder order, NLua.LuaFunction callback)
        //{

        //    if (initialized)
        //        hookPoint.HookBase(this, new HookAction<Object>((ref HookContext ctx, ref Object args) =>
        //        {
        //            callback.Call(ctx, args);
        //        }));
        //    else
        //    {
        //        lock (desiredHooks)
        //            desiredHooks.Add(new HookEntry
        //            {
        //                hookPoint = hookPoint,
        //                callback = new HookAction<Object>((ref HookContext ctx, ref Object args) =>
        //                {
        //                    callback.Call(ctx, args);
        //                }),
        //                order = order
        //            });
        //    }
        //}

        public void Unhook(HookPoint hookPoint)
        {
            if (initialized)
                hookPoint.Unhook(this);
            else
            {
                lock (desiredHooks)
                {
                    int i = 0;
                    foreach (var h in desiredHooks)
                    {
                        if (h.hookPoint == hookPoint)
                        {
                            desiredHooks.RemoveAt(i);
                            break;
                        }
                        i++;
                    }
                }
            }
        }

        /// <summary>
        /// Adds new command to the server's command list
        /// </summary>
        /// <param name="prefix">Command text</param>
        /// <returns>New Command</returns>
        protected CommandInfo AddCommand(string prefix)
        {
            if (commands.ContainsKey(prefix)) throw new ApplicationException("AddCommand: duplicate command: " + prefix);

            var cmd = new CommandInfo(prefix);
            cmd.BeforeEvent += NotifyBeforeCommand;
            cmd.AfterEvent += NotifyAfterCommand;

            lock (commands)
            {
                commands[prefix] = cmd;
                commands[string.Concat(Name.ToLower(), ".", prefix)] = cmd;
            }

            return cmd;
        }

        internal bool Enable()
        {
            if (Interlocked.CompareExchange(ref this.enabled, 1, 0) == 0)
            {
                try
                {
                    Enabled();
                }
                catch (Exception e)
                {
                    Tools.WriteLine("Exception while enabling plugin " + Name);
                    Tools.WriteLine(e);
                    return false;
                }
            }
            return true;
        }

        internal bool Disable()
        {
            if (Interlocked.CompareExchange(ref this.enabled, 0, 1) == 1)
            {
                try
                {
                    Disabled();
                }
                catch (Exception e)
                {
                    Tools.WriteLine("Exception while disabling plugin " + Name);
                    Tools.WriteLine(e);
                    return false;
                }
            }
            return true;
        }

        internal bool InitializeAndHookUp(object state = null)
        {
            if (!Initialize(state)) return false;

            foreach (var h in desiredHooks)
            {
                h.hookPoint.HookBase(this, h.callback, h.order);
            }

            return true;
        }

        internal bool Initialize(object state = null)
        {
            if (initialized)
            {
                Tools.WriteLine("Double initialize of plugin {0}.", Name);
                return true;
            }

            try
            {
                Initialized(state);
            }
            catch (Exception e)
            {
                Tools.WriteLine("Exception in initialization handler of plugin " + Name);
                Tools.WriteLine(e);
                return false;
            }

            if (!NotifyWorldLoaded())
                return false;

            var methods = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttributes(typeof(HookAttribute), true);
                if (attr.Length > 0)
                {
                    var ha = attr[0] as HookAttribute;
                    var hpName = method.GetParameters()[1].ParameterType.GetElementType().Name;

                    var hookPoint = PluginManager.GetHookPoint(hpName);
                    //var hookPoint = typeof(HookPoints).GetField(hpName).GetValue(null) as HookPoint;

                    if (hookPoint != null)
                    {
                        Delegate callback;
                        if (method.IsStatic) // TODO: exception handling
                            callback = Delegate.CreateDelegate(hookPoint.DelegateType, method);
                        else
                            callback = Delegate.CreateDelegate(hookPoint.DelegateType, this, method);

                        HookBase(hookPoint, ha.order, callback);
                    }
                }
            }

            initialized = true;

            return true;
        }

        internal object Suspend()
        {
            try
            {
                return SuspendedAndSaved();
            }
            catch (Exception e)
            {
                Tools.WriteLine("Exception while saving plugin state of " + Name);
                Tools.WriteLine(e);
                return null;
            }
        }

        internal bool Resume(object state)
        {
            try
            {
                Resumed(state);
                return true;
            }
            catch (Exception e)
            {
                Tools.WriteLine("Exception while saving plugin state of " + Name);
                Tools.WriteLine(e);
                return false;
            }
        }

        internal bool Dispose(object state = null)
        {
            if (Interlocked.CompareExchange(ref disposed, 1, 0) == 1) return true;

            var result = Disable();

            try
            {
                Disposed(state);
            }
            catch (Exception e)
            {
                Tools.WriteLine("Exception in disposal handler of plugin " + Name);
                Tools.WriteLine(e);
                result = false;
            }

            commands.Clear();

            var hooks = new HookPoint[this.hooks.Count];
            this.hooks.CopyTo(hooks, 0, hooks.Length);

            foreach (var hook in hooks)
            {
                hook.Unhook(this);
            }

            if (this.hooks.Count > 0)
            {
                Tools.WriteLine("Warning: failed to clean up {0} hooks of plugin {1}.", this.hooks.Count, Name);
                this.hooks.Clear();
            }

            return result;
        }

        // newPlugin should not have been initialized at this point!
        internal bool ReplaceWith(BasePlugin newPlugin, bool saveState = true)
        {
            var result = false;
            var noreturn = false;
            var paused = new LinkedList<HookPoint>();
            object savedState = null;

            lock (HookPoint.editLock)
            {
                Tools.NotifyAllPlayers("<Server> Reloading plugin " + Name + ", you may experience lag...", Color.White, true);

                var signal = new ManualResetEvent(false);

                lock (HookPoint.editLock) try
                    {
                        using (this.Pause())
                        {
                            // initialize new instance with saved state
                            if (saveState)
                                savedState = Suspend();

                            Tools.WriteLine("Initializing new plugin instance...");
                            if (!newPlugin.Initialize(savedState))
                            {
                                if (saveState)
                                    Resume(savedState);
                                return false;
                            }

                            // point of no return, if the new plugin fails now,
                            // blame the author
                            // because it's time to dispose the old plugin
                            noreturn = true;

                            // use command objects from the old plugin, because command invocations
                            // may be paused inside them, this way when they unpause
                            // they run the new plugin's methods
                            lock (commands)
                            {
                                Tools.WriteLine("Replacing commands...");

                                var prefixes = newPlugin.commands.Keys.ToArray();

                                var done = new HashSet<CommandInfo>();

                                foreach (var prefix in prefixes)
                                {
                                    CommandInfo oldCmd;
                                    if (commands.TryGetValue(prefix, out oldCmd))
                                    {
                                        //								Tools.WriteLine ("Replacing command {0}.", prefix);
                                        var newCmd = newPlugin.commands[prefix];

                                        newPlugin.commands[prefix] = oldCmd;
                                        commands.Remove(prefix);

                                        if (done.Contains(oldCmd)) continue;

                                        oldCmd.InitFrom(newCmd);
                                        done.Add(oldCmd);

                                        oldCmd.AfterEvent += newPlugin.NotifyAfterCommand;
                                        oldCmd.BeforeEvent += newPlugin.NotifyBeforeCommand;

                                        // garbage
                                        newCmd.ClearCallbacks();
                                        newCmd.ClearEvents();
                                    }
                                }

                                foreach (var kv in commands)
                                {
                                    var cmd = kv.Value;
                                    Tools.WriteLine("Clearing command {0}.", kv.Key);
                                    cmd.ClearCallbacks();
                                }
                                commands.Clear();
                            }

                            // replace hook subscriptions from the old plugin with new ones
                            // in the exact same spots in the invocation chains
                            lock (newPlugin.desiredHooks)
                            {
                                Tools.WriteLine("Replacing hooks...");

                                foreach (var h in newPlugin.desiredHooks)
                                {
                                    if (hooks.Contains(h.hookPoint))
                                    {
                                        h.hookPoint.Replace(this, newPlugin, h.callback, h.order);
                                        hooks.Remove(h.hookPoint);
                                        newPlugin.hooks.Add(h.hookPoint);
                                    }
                                    else
                                    {
                                        // this adds the hook to newPlugin.hooks
                                        h.hookPoint.HookBase(newPlugin, h.callback, h.order);
                                    }
                                }
                            }

                            Tools.WriteLine("Disabling old plugin instance...");
                            Disable();

                            Tools.WriteLine("Enabling new plugin instance...");
                            if (newPlugin.Enable())
                            {
                                result = true;
                            }
                        }
                    }
                    finally
                    {
                        Tools.NotifyAllPlayers("<Server> Done.", Color.White, true);

                        // clean up remaining hooks
                        if (noreturn)
                        {
                            Tools.WriteLine("Disposing of old plugin instance...");
                            Dispose();
                        }
                    }
            }

            return result;
        }

        /// <summary>
        /// Pause all hook and command invocations of this plugin.
        /// This method should only be called by the plugin itself and should always be used
        /// in a using statement.
        /// <example>
        ///     using (this.Pause ())
        ///     {
        ///         // code
        ///     }
        /// </example>
        /// </summary>
        protected PauseContext Pause()
        {
            return new PauseContext(this);
        }

        protected class PauseContext : IDisposable
        {
            BasePlugin plugin;
            ManualResetEvent signal;
            LinkedList<HookPoint> paused;

            internal PauseContext(BasePlugin plugin)
            {
                this.plugin = plugin;

                signal = new ManualResetEvent(false);
                paused = new LinkedList<HookPoint>();

                Monitor.Enter(HookPoint.editLock);

                // commands or hooks that begin running after this get paused
                plugin.commandPauseSignal = signal;

                foreach (var hook in plugin.hooks)
                {
                    hook.Pause(signal);
                }

                // wait for commands that may have already been running to finish
                while (plugin.HasRunningCommands)
                {
                    Thread.Sleep(10);
                }

                Tools.WriteLine("Plugin {0} commands paused...", plugin.Name ?? "???");

                // wait for hooks that may have already been running to finish
                var pausing = new LinkedList<HookPoint>(plugin.hooks);

                // pausing hooks is more disruptive than pausing commands,
                // so we spinwait instead of sleeping
                var wait = new SpinWait();
                var min = HookPoint.threadInHook ? 1 : 0;
                while (pausing.Count > min)
                {
                    wait.SpinOnce();
                    var link = pausing.First;
                    while (link != null)
                    {
                        if (link.Value.AllPaused)
                        {
                            var x = link;
                            link = link.Next;
                            pausing.Remove(x);
                            paused.AddFirst(x);
                        }
                        else
                        {
                            link = link.Next;
                        }
                    }
                }

                Tools.WriteLine("Plugin {0} hooks paused...", plugin.Name ?? "???");
            }

            public void Dispose()
            {
                Tools.WriteLine("Unpausing everything related to plugin {0}...", plugin.Name ?? "???");

                plugin.commandPauseSignal = null;

                foreach (var hook in paused)
                {
                    hook.CancelPause();
                }

                signal.Set();

                Monitor.Exit(HookPoint.editLock);
            }
        }

        internal bool NotifyWorldLoaded()
        {
            //if (!Statics.WorldLoaded) return true;

            if (Interlocked.CompareExchange(ref informedOfWorld, 1, 0) != 0) return true;

            try
            {
                WorldLoaded();
            }
            catch (Exception e)
            {
                Tools.WriteLine("Exception in world load handler of plugin " + Name);
                Tools.WriteLine(e);
                return false;
            }

            return true;
        }

        [ThreadStatic]
        internal static int threadInCommand;

        internal void NotifyBeforeCommand(CommandInfo cmd)
        {
            Interlocked.Increment(ref runningCommands);

            var signal = commandPauseSignal;
            if (signal != null)
            {
                Interlocked.Increment(ref pausedCommands);
                signal.WaitOne();
                Interlocked.Decrement(ref pausedCommands);
            }

            threadInCommand = 1;
        }

        internal void NotifyAfterCommand(CommandInfo cmd)
        {
            Interlocked.Decrement(ref runningCommands);
            threadInCommand = 0;
        }
    }
}