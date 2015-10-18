using System;
using OTA.Plugin;
using System.Collections.Generic;
using TDSM.Core.Command;
using System.Threading;
using OTA;
using OTA.Logging;
using System.Linq;

namespace TDSM.Core
{
    internal class PluginCommand
    {
        public int runningCommands;
        public int pausedCommands;
        public ManualResetEvent commandPauseSignal;

        public Dictionary<string, CommandInfo> commands;

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

    public static class CommandManager
    {
        internal static Dictionary<BasePlugin, PluginCommand> commands = new Dictionary<BasePlugin,PluginCommand>();

        internal static PluginCommand GetPluginCommandDefinitions(this BasePlugin plugin)
        {
            if (commands.ContainsKey(plugin)) return commands[plugin];
            return null;
        }

        internal static PluginCommand GetOrCreatePluginCommandDefinitions(this BasePlugin plugin)
        {
            if (commands.ContainsKey(plugin)) return commands[plugin];

            var def = new PluginCommand()
            {
                commands = new Dictionary<string, CommandInfo>()
            };
            commands.Add(plugin, def);

            return def;
        }

        public static bool FindStringCommand(string prefix, out CommandInfo info)
        {
            info = null;

            prefix = prefix.ToLower();
            foreach (var def in commands)
            {
                var plugin = def.Key;
                var cmd = def.Value;

                lock (cmd.commands)
                {
                    if (plugin.IsEnabled && cmd.commands.TryGetValue(prefix, out info))
                    {
                        return info.stringCallback != null;
                    }
                }
            }
            
            if (Entry.CommandParser.serverCommands.TryGetValue(prefix, out info) && info.stringCallback != null)
                return true;

            return false;
        }

        public static bool FindTokenCommand(string prefix, out CommandInfo info)
        {
            info = null;

            prefix = prefix.ToLower();
            foreach (var def in commands)
            {
                var plugin = def.Key;
                var cmd = def.Value;

                lock (cmd.commands)
                {
                    if (plugin.IsEnabled && cmd.commands.TryGetValue(prefix, out info) && (info.tokenCallback != null || info.LuaCallback != null))
                        return true;
                }
            }
            
            if (Entry.CommandParser.serverCommands.TryGetValue(prefix, out info) && info.tokenCallback != null)
                return true;

            return false;
        }
    }

    public static class PluginExtensions
    {
        public static Dictionary<string, CommandInfo> GetPluginCommands(this BasePlugin plugin)
        {
            if (CommandManager.commands.ContainsKey(plugin)) return CommandManager.commands[plugin].commands;
            return null;
        }

        public static void Initialise(Entry plugin)
        {
            plugin.Hook(HookPoints.PluginDisposed, OnDisposed);
            plugin.Hook(HookPoints.PluginReplacing, OnPluginReplacing);
            plugin.Hook(HookPoints.PluginPauseComplete, OnPluginPauseComplete);
            plugin.Hook(HookPoints.PluginPausing, OnPluginPausing);
        }

        public static void OnPluginReplacing(ref HookContext ctx, ref HookArgs.PluginReplacing args)
        {
            var def = args.OldPlugin.GetPluginCommandDefinitions();
            if (null == def) return;

            // use command objects from the old plugin, because command invocations
            // may be paused inside them, this way when they unpause
            // they run the new plugin's methods
            lock (def.commands)
            {
                ProgramLog.Debug.Log("Replacing commands...");

                var newDef = args.NewPlugin.GetPluginCommandDefinitions();

                string[] prefixes = newDef.commands.Keys.ToArray();

                var done = new HashSet<CommandInfo>();

                foreach (var prefix in prefixes)
                {
                    CommandInfo oldCmd;
                    if (def.commands.TryGetValue(prefix, out oldCmd))
                    {
                        //                              Tools.WriteLine ("Replacing command {0}.", prefix);
                        var newCmd = newDef.commands[prefix];

                        newDef.commands[prefix] = oldCmd;
                        def.commands.Remove(prefix);

                        if (done.Contains(oldCmd))
                            continue;

                        oldCmd.InitFrom(newCmd);
                        done.Add(oldCmd);

                        oldCmd.AfterEvent += newDef.NotifyAfterCommand;
                        oldCmd.BeforeEvent += newDef.NotifyBeforeCommand;

                        // garbage
                        newCmd.ClearCallbacks();
                        newCmd.ClearEvents();
                    }
                }

                foreach (var kv in def.commands)
                {
                    var cmd = kv.Value;
                    ProgramLog.Debug.Log("Clearing command {0}.", kv.Key);
                    cmd.ClearCallbacks();
                }
                def.commands.Clear();
            }
        }

        public static bool HasRunningCommands(this BasePlugin plugin)
        {
            var def = plugin.GetPluginCommandDefinitions();
            if (null == def) return false;

            return (def.runningCommands - def.pausedCommands - PluginCommand.threadInCommand) > 0;
        }

        public static void OnDisposed(ref HookContext ctx, ref HookArgs.PluginDisposed args)
        {
            var def = args.Plugin.GetPluginCommandDefinitions();
            if (null != def)
                def.commands.Clear();
        }

        public static void OnPluginPauseComplete(ref HookContext ctx, ref HookArgs.PluginPauseComplete args)
        {
            var def = args.Plugin.GetPluginCommandDefinitions();
            if (null != def)
                def.commandPauseSignal = null;
        }

        public static void OnPluginPausing(ref HookContext ctx, ref HookArgs.PluginPausing args)
        {
            var def = args.Plugin.GetPluginCommandDefinitions();
            if (null != def)
            {
                //commands or hooks that begin running after this get paused
                def.commandPauseSignal = args.Signal;

                // wait for commands that may have already been running to finish
                while (args.Plugin.HasRunningCommands())
                {
                    Thread.Sleep(10);
                }
            }
        }

        /// <summary>
        /// Adds a new command to the server's command list
        /// </summary>
        /// <param name="prefix">Command text</param>
        /// <returns>New Command</returns>
        public static CommandInfo AddPluginCommand(BasePlugin plugin, string prefix)
        {
            var def = CommandManager.GetOrCreatePluginCommandDefinitions(plugin);

            if (def.commands.ContainsKey(prefix))
                throw new ApplicationException("AddCommand: duplicate command: " + prefix);

            var cmd = new CommandInfo(prefix);
            cmd.BeforeEvent += def.NotifyBeforeCommand;
            cmd.AfterEvent += def.NotifyAfterCommand;

            lock (def.commands)
            {
                def.commands[prefix] = cmd;
                def.commands[string.Concat(plugin.Name.ToLower(), ".", prefix)] = cmd;
            }

            return cmd;
        }

        public static CommandInfo AddCommand(this BasePlugin plugin, string prefix)
        {
            return AddPluginCommand(plugin, prefix);
        }
    }

    public static class ISenderExtensions
    {
        public static void Message(this OTA.Command.ISender sender, int sentFrom, string message, byte r = 255, byte g = 255, byte b = 255)
        {
            sender.SendMessage(message, sentFrom, r, g, b);
        }
    }
}

