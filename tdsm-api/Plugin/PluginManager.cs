﻿using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using TDSM.API.Command;
using TDSM.API.Plugin;

namespace TDSM.API
{
    /// <summary>
    /// PluginManager class.  Handles all input/output, loading, enabling, disabling, and hook processing for plugins
    /// [TODO] Reload plugin assembly, Not enable/disable.
    /// </summary>
    public static class PluginManager
    {
        static PluginManager()
        {
            //Resolves external plugin hook assemblies. So there is no need to place the DLL beside tdsm.exe
            AppDomain.CurrentDomain.AssemblyResolve += (s, a) =>
            {
                if (a.Name == "Terraria") return Assembly.GetEntryAssembly();
                var items = _plugins.Values
                    .Where(x => x != null && x.Assembly != null && x.Assembly.FullName == a.Name)
                    .Select(x => x.Assembly)
                    .FirstOrDefault();
                //if (items == null)
                //{
                //    Tools.WriteLine("[Fatal] Unable to load {0}, was this plugin removed or do you need to repatch?", a.Name);
                //}

                //Look in libraries - assembly name must match filename
                if (items == null)
                {
                    var ix = a.Name.IndexOf(',');
                    if (ix > -1)
                    {
                        var loc = Path.Combine(Globals.LibrariesPath, a.Name.Substring(0, ix) + ".dll");
                        if (File.Exists(loc))
                        {
                            using (var ms = new MemoryStream())
                            {
                                var buff = new byte[256];
                                using (var fs = File.OpenRead(loc))
                                {
                                    while (fs.Position < fs.Length)
                                    {
                                        var read = fs.Read(buff, 0, buff.Length);
                                        ms.Write(buff, 0, read);
                                    }
                                }

                                return Assembly.Load(ms.ToArray());
                            }
                        }
                    }
                }

                return items;
            };
        }

        private static string _pluginPath = String.Empty;
        //private static string _libraryPath = String.Empty;

        public static Dictionary<String, BasePlugin> _plugins;
        public static int PluginCount { get { return _plugins.Count; } }

        private static Type _hookPointSource;

        public static void SetHookSource(Type hpt)
        {
            _hookPointSource = hpt;
        }

        public static HookPoint GetHookPoint(string name)
        {
            var fld = _hookPointSource.GetField(name);
            if (fld != null) return fld.GetValue(null) as HookPoint;

            return null;
        }

        /// <summary>
        /// Server's plugin list
        /// </summary>
        public static PluginRecordEnumerator EnumeratePluginsRecords
        {
            get
            {
                Monitor.Enter(_plugins);
                return new PluginRecordEnumerator { inner = _plugins.GetEnumerator() };
            }
        }

        public static PluginEnumerator EnumeratePlugins
        {
            get
            {
                Monitor.Enter(_plugins);
                return new PluginEnumerator { inner = _plugins.Values.GetEnumerator() };
            }
        }

        public struct PluginRecordEnumerator : IDisposable, IEnumerator<KeyValuePair<string, BasePlugin>>, IEnumerator
        {
            internal Dictionary<string, BasePlugin>.Enumerator inner;

            public void Dispose()
            {
                inner.Dispose();
                Monitor.Exit(_plugins);
            }

            public KeyValuePair<string, BasePlugin> Current
            {
                get { return inner.Current; }
            }

            object IEnumerator.Current
            {
                get { return inner.Current; }
            }

            public PluginRecordEnumerator GetEnumerator()
            {
                return this;
            }

            public bool MoveNext()
            {
                return inner.MoveNext();
            }

            public void Reset()
            {
            }
        }

        public struct PluginEnumerator : IDisposable, IEnumerator<BasePlugin>, IEnumerator
        {
            internal Dictionary<string, BasePlugin>.ValueCollection.Enumerator inner;

            public void Dispose()
            {
                inner.Dispose();
                Monitor.Exit(_plugins);
            }

            public BasePlugin Current
            {
                get { return inner.Current; }
            }

            object IEnumerator.Current
            {
                get { return inner.Current; }
            }

            public PluginEnumerator GetEnumerator()
            {
                return this;
            }

            public bool MoveNext()
            {
                return inner.MoveNext();
            }

            public void Reset() { }
        }

        private static bool _enableLUA;

        /// <summary>
        /// PluginManager class constructor
        /// </summary>
        /// <param name="_pluginPath">Path to plugin directory</param>
        public static void Initialize(string pluginPath)
        {
            _pluginPath = pluginPath;
            //_libraryPath = libraryPath;

            _plugins = new Dictionary<String, BasePlugin>();
        }

        /// <summary>
        /// Initializes Plugin (Loads) and Checks for Out of Date Plugins.
        /// </summary>
        public static void LoadPlugins()
        {
            lock (_plugins)
            {
                LoadPluginsInternal();

                foreach (var kv in _plugins)
                {
                    var plugin = kv.Value;
                    if (plugin.TDSMBuild != Globals.Build)
                    {
                        Tools.WriteLine("[WARNING] Plugin build incorrect: " + plugin.Name);
                    }
                }
            }

            var ctx = new HookContext
            {
            };

            var args = new HookArgs.PluginsLoaded
            {
            };

            HookPoints.PluginsLoaded.Invoke(ref ctx, ref args);
        }

        public static void CheckPlugins()
        {
        }

        static void SetPluginProperty<T>(BasePlugin plugin, string name, string target)
        {
            try
            {
                var field = plugin.GetType().GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                if (field != null && field.FieldType == typeof(T))
                {
                    var prop = typeof(BasePlugin).GetProperty(target);

                    prop.SetValue(plugin, field.GetValue(null), null);
                }
            }
            catch (Exception e)
            {
                Tools.WriteLine(e);
            }
        }

        static BasePlugin LoadPluginAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes().Where(x => typeof(BasePlugin).IsAssignableFrom(x) && !x.IsAbstract))
            {
                var plugin = CreatePluginInstance(type);
                if (plugin == null)
                {
                    throw new Exception("Could not create plugin instance");
                }
                else
                {
                    plugin.Assembly = assembly;
                    return plugin;
                }
            }
            return null;
        }

        static BasePlugin CreatePluginInstance(System.Type type)
        {
            var plugin = (BasePlugin)Activator.CreateInstance(type);

            if (plugin == null) return null;

            SetPluginProperty<string>(plugin, "NAME", "Name");
            SetPluginProperty<string>(plugin, "AUTHOR", "Author");
            SetPluginProperty<string>(plugin, "DESCRIPTION", "Description");
            SetPluginProperty<string>(plugin, "VERSION", "Version");
            SetPluginProperty<int>(plugin, "BUILD", "TDSMBuild");

            return plugin;
        }

        /// <summary>
        /// Load the plugin located at the specified path.
        /// This only loads one plugin.
        /// </summary>
        /// <param name="PluginPath">Path to plugin</param>
        /// <returns>Instance of the successfully loaded plugin, otherwise null</returns>
        public static BasePlugin LoadPluginFromDLL(string PluginPath)
        {
            try
            {
                Assembly assembly = null;
                Type type = typeof(BasePlugin);

                using (FileStream fs = File.Open(PluginPath, FileMode.Open))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        byte[] buffer = new byte[1024];

                        int read = 0;

                        while ((read = fs.Read(buffer, 0, 1024)) > 0)
                            ms.Write(buffer, 0, read);

                        assembly = Assembly.Load(ms.ToArray());
                    }
                }

                return LoadPluginAssembly(assembly);
            }
            catch (Exception e)
            {
                Tools.WriteLine("Error loading plugin assembly " + PluginPath);
                Tools.WriteLine(e);
            }

            return null;
        }

        static readonly Dictionary<string, string> compilerOptions = new Dictionary<string, string>()
		{
			{ "CompilerVersion", "v4.0" },
		};

        public static BasePlugin LoadSourcePlugin(string path)
        {
            var cp = new Microsoft.CSharp.CSharpCodeProvider(compilerOptions);
            var par = new System.CodeDom.Compiler.CompilerParameters();
            par.GenerateExecutable = false;
            par.GenerateInMemory = true;
            par.IncludeDebugInformation = true;
            //par.CompilerOptions = "/optimize";
            par.TreatWarningsAsErrors = false;

            var us = Assembly.GetExecutingAssembly();
            par.ReferencedAssemblies.Add(us.Location);

            foreach (var asn in us.GetReferencedAssemblies())
            {
                par.ReferencedAssemblies.Add(asn.Name);
            }

            var result = cp.CompileAssemblyFromFile(par, path);

            var errors = result.Errors;
            if (errors != null)
            {
                if (errors.HasErrors)
                    Tools.WriteLine("Failed to compile source plugin:");
                foreach (System.CodeDom.Compiler.CompilerError error in errors)
                {
                    if (error.IsWarning)
                        Tools.WriteLine(error.ToString());
                    else
                        Tools.WriteLine(error.ToString());
                }
                if (errors.HasErrors)
                    return null;
            }

            return LoadPluginAssembly(result.CompiledAssembly);
        }

        public static BasePlugin LoadPluginFromPath(string file)
        {
            FileInfo fileInfo = new FileInfo(file);
            var ext = fileInfo.Extension.ToLower();
            BasePlugin plugin = null;

            var ctx = new HookContext
            {
            };

            var args = new HookArgs.PluginLoadRequest
            {
                Path = file,
            };

            HookPoints.PluginLoadRequest.Invoke(ref ctx, ref args);

            if (ctx.Result == HookResult.IGNORE)
                return null;

            if (args.LoadedPlugin == null)
            {
                if (ext == ".dll")
                {
                    Tools.WriteLine("Loading plugin from {0}.", fileInfo.Name);
                    plugin = LoadPluginFromDLL(file);
                }
                else if (ext == ".cs")
                {
                    Tools.WriteLine("Compiling and loading plugin from {0}.", fileInfo.Name);
                    plugin = LoadSourcePlugin(file);
                }
                else if (ext == ".lua")
                {
                    if (!_enableLUA) _enableLUA = true;
                    Tools.WriteLine("Loading plugin from {0}.", fileInfo.Name);
                    plugin = new LUAPlugin();
                }
            }

            if (plugin != null)
            {
                plugin.Path = file;
                plugin.PathTimestamp = fileInfo.LastWriteTimeUtc;
                if (plugin.Name == null) plugin.Name = Path.GetFileNameWithoutExtension(file);
            }

            return plugin;
        }

        public static bool ReplacePlugin(BasePlugin oldPlugin, BasePlugin newPlugin, bool saveState = true)
        {
            lock (_plugins)
            {
                string oldName = oldPlugin.Name.ToLower().Trim();

                if (oldPlugin.ReplaceWith(newPlugin, saveState))
                {
                    string newName = newPlugin.Name.ToLower().Trim();

                    if (_plugins.ContainsKey(oldName))
                        _plugins.Remove(oldName);

                    _plugins.Add(newName, newPlugin);

                    return true;
                }
                else if (oldPlugin.IsDisposed)
                {
                    if (_plugins.ContainsKey(oldName))
                        _plugins.Remove(oldName);
                }
            }

            return false;
        }

        public static BasePlugin ReloadPlugin(BasePlugin oldPlugin, bool saveState = true)
        {
            var fi = new FileInfo(oldPlugin.Path);

            BasePlugin newPlugin;

            if (fi.LastWriteTimeUtc > oldPlugin.PathTimestamp)
            {
                // plugin updated
                Tools.WriteLine("Plugin {0} is being updated from file.", oldPlugin.Name);
                newPlugin = LoadPluginFromPath(oldPlugin.Path);
            }
            else
            {
                Tools.WriteLine("Plugin {0} not updated, reinitializing.", oldPlugin.Name);
                newPlugin = CreatePluginInstance(oldPlugin.GetType());
                newPlugin.Path = oldPlugin.Path;
                newPlugin.PathTimestamp = oldPlugin.PathTimestamp;
                newPlugin.Name = oldPlugin.Name;
            }

            if (newPlugin == null)
                return oldPlugin;

            if (ReplacePlugin(oldPlugin, newPlugin, saveState))
            {
                return newPlugin;
            }
            else if (oldPlugin.IsDisposed)
            {
                return null;
            }

            return oldPlugin;
        }

        internal static void LoadPluginsInternal()
        {
            var files = Directory.GetFiles(_pluginPath);
            Array.Sort(files);

            foreach (string file in files)
            {
                var plugin = LoadPluginFromPath(file);
                if (plugin != null)
                {
                    if (plugin.InitializeAndHookUp())
                    {
                        _plugins.Add(plugin.Name.ToLower().Trim(), plugin);

                        if (plugin.EnableEarly)
                            plugin.Enable();
                    }
                }
            }

            EnablePlugins();
        }

        /// <summary>
        /// Reloads all plugins currently running on the server
        /// </summary>
        public static void ReloadPlugins(bool saveState = true)
        {
            lock (_plugins)
            {
                var list = _plugins.Values.ToArray();

                foreach (var plugin in list)
                {
                    ReloadPlugin(plugin, saveState);
                }
            }
        }

        /// <summary>
        /// Enables all plugins available to the server
        /// </summary>
        public static void EnablePlugins()
        {
            foreach (var plugin in EnumeratePlugins)
            {
                plugin.Enable();
            }
        }

        /// <summary>
        /// Disables all plugins currently running on the server
        /// </summary>
        public static void DisablePlugins()
        {
            foreach (var plugin in EnumeratePlugins)
            {
                plugin.Disable();
            }
        }

        /// <summary>
        /// Enables a plugin by name. Currently unused in core
        /// </summary>
        /// <param name="name">Plugin name</param>
        /// <returns>Returns true on plugin successfully Enabling</returns>
        public static bool EnablePlugin(string name)
        {
            lock (_plugins)
            {
                string cleanedName = name.ToLower().Trim();
                if (_plugins.ContainsKey(cleanedName))
                {
                    BasePlugin plugin = _plugins[cleanedName];
                    plugin.Enable();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Enables a plugin.
        /// </summary>
        /// <param name="name">Plugin name</param>
        /// <returns>Returns true on plugin successfully Enabling</returns>
        public static bool EnablePlugin(BasePlugin plugin)
        {
            return plugin.Enable();
        }

        /// <summary>
        /// Enables a plugin.
        /// </summary>
        /// <param name="name">Plugin name</param>
        /// <returns>Returns true on plugin successfully Enabling</returns>
        public static bool DisablePlugin(BasePlugin plugin)
        {
            return plugin.Disable();
        }

        /// <summary>
        /// Disables a plugin by name.  Currently unused in core
        /// </summary>
        /// <param name="name">Name of plugin</param>
        /// <returns>Returns true on plugin successfully Disabling</returns>
        public static bool DisablePlugin(string name)
        {
            lock (_plugins)
            {
                string cleanedName = name.ToLower().Trim();
                if (_plugins.ContainsKey(cleanedName))
                {
                    BasePlugin plugin = _plugins[cleanedName];
                    plugin.Disable();
                    return true;
                }
                return false;
            }
        }

        public static bool UnloadPlugin(BasePlugin plugin)
        {
            lock (_plugins)
            {
                bool result = plugin.Dispose();

                string cleanedName = plugin.Name.ToLower().Trim();
                if (_plugins.ContainsKey(cleanedName))
                    _plugins.Remove(cleanedName);

                return result;
            }
        }

        /// <summary>
        /// Gets plugin instance by name.
        /// </summary>
        /// <param name="name">Plugin name</param>
        /// <returns>Returns found plugin if successful, otherwise returns null</returns>
        public static BasePlugin GetPlugin(string name)
        {
            lock (_plugins)
            {
                string cleanedName = name.ToLower().Trim();
                if (_plugins.ContainsKey(cleanedName))
                {
                    return _plugins[cleanedName];
                }
                return null;
            }
        }

        internal static void NotifyWorldLoaded()
        {
            foreach (var plugin in EnumeratePlugins)
            {
                plugin.NotifyWorldLoaded();
            }
        }

        public static void PluginCommand(ISender sender, ArgumentList args)
        {
            /*
             * Commands:
             *      list    - shows all plugins
             *      info    - shows a plugin's author & description etc
             *      disable - disables a plugin
             *      enable  - enables a plugin
             *      reload
             *      unload
             *      status
             *      load
             */

            if (args.Count == 0) throw new CommandError("Subcommand expected.");

            string command = args[0];
            args.RemoveAt(0); //Allow the commands to use any additional arguments without also getting the command

            lock (_plugins)
                switch (command)
                {
                    case "-l":
                    case "ls":
                    case "list":
                        {
                            if (PluginCount == 0)
                            {
                                sender.Message(255, "No plugins loaded.");
                                return;
                            }

                            var msg = new StringBuilder();
                            msg.Append("Plugins: ");

                            int i = 0;
                            foreach (var plugin in EnumeratePlugins)
                            {
                                if (i > 0)
                                    msg.Append(", ");
                                msg.Append(plugin.Name);

                                if (!String.IsNullOrEmpty(plugin.Version))
                                {
                                    msg.Append(" (");
                                    msg.Append(plugin.Version);
                                    msg.Append(")");
                                }

                                if (!plugin.IsEnabled)
                                    msg.Append("[OFF]");
                                i++;
                            }
                            msg.Append(".");

                            sender.Message(255, Color.DodgerBlue, msg.ToString());

                            break;
                        }

                    case "-s":
                    case "stat":
                    case "status":
                        {
                            if (PluginCount == 0)
                            {
                                sender.Message(255, "No plugins loaded.");
                                return;
                            }

                            var msg = new StringBuilder();

                            foreach (var plugin in EnumeratePlugins)
                            {
                                msg.Clear();
                                msg.Append(plugin.IsDisposed ? "[DISPOSED] " : (plugin.IsEnabled ? "[ON]  " : "[OFF] "));
                                msg.Append(plugin.Name);
                                msg.Append(" ");
                                msg.Append(plugin.Version);
                                if (plugin.Status != null && plugin.Status.Length > 0)
                                {
                                    msg.Append(" : ");
                                    msg.Append(plugin.Status);
                                }
                                sender.Message(255, Color.DodgerBlue, msg.ToString());
                            }

                            break;
                        }

                    case "-i":
                    case "info":
                        {
                            string name;
                            args.ParseOne(out name);

                            var fplugin = GetPlugin(name);
                            if (fplugin != null)
                            {
                                var path = Path.GetFileName(fplugin.Path);
                                sender.Message(255, Color.DodgerBlue, fplugin.Name);
                                sender.Message(255, Color.DodgerBlue, "Filename: " + path);
                                sender.Message(255, Color.DodgerBlue, "Version:  " + fplugin.Version);
                                sender.Message(255, Color.DodgerBlue, "Author:   " + fplugin.Author);
                                if (fplugin.Description != null && fplugin.Description.Length > 0)
                                    sender.Message(255, Color.DodgerBlue, fplugin.Description);
                                sender.Message(255, Color.DodgerBlue, "Status:   " + (fplugin.IsEnabled ? "[ON] " : "[OFF] ") + fplugin.Status);
                            }
                            else
                            {
                                sender.SendMessage("The plugin \"" + args[1] + "\" was not found.");
                            }

                            break;
                        }

                    case "-d":
                    case "disable":
                        {
                            string name;
                            args.ParseOne(out name);

                            var fplugin = GetPlugin(name);
                            if (fplugin != null)
                            {
                                if (fplugin.Disable())
                                    sender.Message(255, Color.DodgerBlue, fplugin.Name + " was disabled.");
                                else
                                    sender.Message(255, Color.DodgerBlue, fplugin.Name + " was disabled, errors occured during the process.");
                            }
                            else
                                sender.Message(255, "The plugin \"" + name + "\" could not be found.");

                            break;
                        }

                    case "-e":
                    case "enable":
                        {
                            string name;
                            args.ParseOne(out name);

                            var fplugin = GetPlugin(name);
                            if (fplugin != null)
                            {
                                if (fplugin.Enable())
                                    sender.Message(255, Color.DodgerBlue, fplugin.Name + " was enabled.");
                                else
                                    sender.Message(255, Color.DodgerBlue, fplugin.Name + " was enabled, errors occured during the process.");
                            }
                            else
                                sender.Message(255, "The plugin \"" + name + "\" could not be found.");

                            break;
                        }

                    case "-u":
                    case "-ua":
                    case "unload":
                        {
                            string name;

                            if (command == "-ua" || command == "-uca")
                                name = "all";
                            else
                                args.ParseOne(out name);

                            BasePlugin[] plugs;
                            if (name == "all" || name == "-a")
                            {
                                plugs = _plugins.Values.ToArray();
                            }
                            else
                            {
                                var splugin = PluginManager.GetPlugin(name);

                                if (splugin == null)
                                {
                                    sender.Message(255, "The plugin \"" + name + "\" could not be found.");
                                    return;
                                }

                                plugs = new BasePlugin[] { splugin };
                            }

                            foreach (var fplugin in plugs)
                            {
                                if (UnloadPlugin(fplugin))
                                    sender.Message(255, Color.DodgerBlue, fplugin.Name + " was unloaded.");
                                else
                                    sender.Message(255, Color.DodgerBlue, fplugin.Name + " was unloaded, errors occured during the process.");
                            }

                            break;
                        }

                    case "-r":
                    case "-rc":
                    case "-ra":
                    case "-rca":
                    case "reload":
                        {
                            bool save = true;
                            if (command == "-rc" || command == "-rca" || args.TryPop("-c") || args.TryPop("-clean"))
                                save = false;

                            string name;

                            if (command == "-ra" || command == "-rca")
                                name = "all";
                            else
                                args.ParseOne(out name);

                            BasePlugin[] plugs;
                            if (name == "all" || name == "-a")
                            {
                                plugs = _plugins.Values.ToArray();
                            }
                            else
                            {
                                var splugin = PluginManager.GetPlugin(name);

                                if (splugin == null)
                                {
                                    sender.Message(255, "The plugin \"" + name + "\" could not be found.");
                                    return;
                                }

                                plugs = new BasePlugin[] { splugin };
                            }

                            foreach (var fplugin in plugs)
                            {
                                var nplugin = PluginManager.ReloadPlugin(fplugin, save);
                                if (nplugin == fplugin)
                                {
                                    sender.Message(255, Color.DodgerBlue, "Errors occured while reloading plugin " + fplugin.Name + ", old instance kept.");
                                }
                                else if (nplugin == null)
                                {
                                    sender.Message(255, Color.DodgerBlue, "Errors occured while reloading plugin " + fplugin.Name + ", it has been unloaded.");
                                }
                            }

                            break;
                        }

                    case "-L":
                    case "-LR":
                    case "load":
                        {
                            bool replace = command == "-LR" || args.TryPop("-R") || args.TryPop("-replace");
                            bool save = command != "-LRc" && !args.TryPop("-c") && !args.TryPop("-clean");

                            var fname = string.Join(" ", args);
                            string path;

                            if (fname == "") throw new CommandError("File name expected");

                            if (Path.IsPathRooted(fname))
                                path = Path.GetFullPath(fname);
                            else
                                path = Path.Combine(_pluginPath, fname);

                            var fi = new FileInfo(path);

                            if (!fi.Exists)
                            {
                                sender.Message(255, "Specified file doesn't exist.");
                                return;
                            }

                            var newPlugin = LoadPluginFromPath(path);

                            if (newPlugin == null)
                            {
                                sender.Message(255, "Unable to load plugin.");
                                return;
                            }

                            var oldPlugin = GetPlugin(newPlugin.Name);
                            if (oldPlugin != null)
                            {
                                if (!replace)
                                {
                                    sender.Message(255, "A plugin named {0} is already loaded, use -replace to replace it.", oldPlugin.Name);
                                    return;
                                }

                                if (ReplacePlugin(oldPlugin, newPlugin, save))
                                {
                                    sender.Message(255, Color.DodgerBlue, "Plugin {0} has been replaced.", oldPlugin.Name);
                                }
                                else if (oldPlugin.IsDisposed)
                                {
                                    sender.Message(255, Color.DodgerBlue, "Replacement of plugin {0} failed, it has been unloaded.", oldPlugin.Name);
                                }
                                else
                                {
                                    sender.Message(255, Color.DodgerBlue, "Replacement of plugin {0} failed, old instance kept.", oldPlugin.Name);
                                }

                                return;
                            }

                            if (!newPlugin.InitializeAndHookUp())
                            {
                                sender.Message(255, Color.DodgerBlue, "Failed to initialize new plugin instance.");
                            }

                            _plugins.Add(newPlugin.Name.ToLower().Trim(), newPlugin);

                            if (!newPlugin.Enable())
                            {
                                sender.Message(255, Color.DodgerBlue, "Failed to enable new plugin instance.");
                            }

                            break;
                        }

                    default:
                        {
                            throw new CommandError("Subcommand not recognized.");
                        }
                }
        }

        public static void RegisterPlugin(BasePlugin plugin)
        {
            if (!plugin.InitializeAndHookUp())
            {
                Tools.WriteLine("Failed to initialize new plugin instance.", Color.DodgerBlue);
            }

            _plugins.Add(plugin.Name.ToLower().Trim(), plugin);

            if (!plugin.Enable())
            {
                Tools.WriteLine("Failed to enable new plugin instance.", Color.DodgerBlue);
            }
        }

        public static PluginLoadStatus LoadAndInitPlugin(string Path)
        {
            var rPlg = LoadPluginFromDLL(Path);

            if (rPlg == null)
            {
                Tools.WriteLine("Plugin failed to load!");
                return PluginLoadStatus.FAIL_LOAD;
            }

            if (!rPlg.InitializeAndHookUp())
            {
                Tools.WriteLine("Failed to initialize plugin.");
                return PluginLoadStatus.FAIL_INIT;
            }

            _plugins.Add(rPlg.Name.ToLower().Trim(), rPlg);

            if (!rPlg.Enable())
            {
                Tools.WriteLine("Failed to enable plugin.");
                return PluginLoadStatus.FAIL_ENABLE;
            }

            return PluginLoadStatus.SUCCESS;
        }
    }

    public enum PluginLoadStatus : int
    {
        FAIL_ENABLE,
        FAIL_INIT,
        FAIL_LOAD,
        SUCCESS
    }
}