using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

using Terraria_Server.Logging;
using System.Collections;
using System.Collections.Generic;

namespace Terraria_Server.Plugins
{
	/// <summary>
	/// PluginManager class.  Handles all input/output, loading, enabling, disabling, and hook processing for plugins
	/// [TODO] Reload plugin assembly, Not enable/disable.
	/// </summary>
	public static class PluginManager
	{
		private static string pluginPath = String.Empty;
		private static string libraryPath = String.Empty;
		private static Dictionary<String, BasePlugin> plugins;

		/// <summary>
		/// Server's plugin list
		/// </summary>
		public static Dictionary<String, BasePlugin> Plugins
		{
			get { return plugins; }
		}
		
		public static PluginRecordEnumerator EnumeratePluginsRecords
		{
			get
			{
				Monitor.Enter (plugins);
				return new PluginRecordEnumerator { inner = plugins.GetEnumerator() };
			}
		}
		
		public static PluginEnumerator EnumeratePlugins
		{
			get
			{
				Monitor.Enter (plugins);
				return new PluginEnumerator { inner = plugins.Values.GetEnumerator() };
			}
		}
		
		public struct PluginRecordEnumerator : IDisposable, IEnumerator<KeyValuePair<string, BasePlugin>>, IEnumerator
		{
			internal Dictionary<string, BasePlugin>.Enumerator inner;
			
			public void Dispose ()
			{
				inner.Dispose ();
				Monitor.Exit (plugins);
			}
			
			public KeyValuePair<string, BasePlugin> Current
			{
				get { return inner.Current; }
			}
			
			object IEnumerator.Current
			{
				get { return inner.Current; }
			}
			
			public PluginRecordEnumerator GetEnumerator ()
			{
				return this;
			}
			
			public bool MoveNext ()
			{
				return inner.MoveNext ();
			}
			
			public void Reset ()
			{
			}
		}
		
		public struct PluginEnumerator : IDisposable, IEnumerator<BasePlugin>, IEnumerator
		{
			internal Dictionary<string, BasePlugin>.ValueCollection.Enumerator inner;
			
			public void Dispose ()
			{
				inner.Dispose ();
				Monitor.Exit (plugins);
			}
			
			public BasePlugin Current
			{
				get { return inner.Current; }
			}
			
			object IEnumerator.Current
			{
				get { return inner.Current; }
			}
			
			public PluginEnumerator GetEnumerator ()
			{
				return this;
			}
			
			public bool MoveNext ()
			{
				return inner.MoveNext ();
			}
			
			public void Reset ()
			{
			}
		}
		
		/// <summary>
		/// PluginManager class constructor
		/// </summary>
		/// <param name="pluginPath">Path to plugin directory</param>
		/// <param name="libraryPath">Path to library directory</param>
		public static void Initialize (string _pluginPath, string _libraryPath)
		{
			pluginPath = _pluginPath;
			libraryPath = _libraryPath;

			plugins = new Dictionary<String, BasePlugin>();
		}

		/// <summary>
		/// Initializes Plugin (Loads) and Checks for Out of Date Plugins.
		/// </summary>
		public static void LoadAllPlugins()
		{
			LoadPlugins();

			CheckPlugins();
			
			var ctx = new HookContext
			{
			};
			
			var args = new HookArgs.PluginsLoaded
			{
			};
			
			HookPoints.PluginsLoaded.Invoke (ref ctx, ref args);
		}

		public static void CheckPlugins()
		{
			foreach (var plugin in plugins.Values)
			{
				if (plugin.TDSMBuild != Statics.BUILD)
				{
					ProgramLog.Error.Log("[WARNING] Plugin build incorrect: " + plugin.Name); //Admin's responsibility.
				}
			}
		}
		
		static void SetPluginProperty<T> (BasePlugin plugin, string name, string target)
		{
			try
			{
				var field = plugin.GetType().GetField (name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				
				if (field != null && field.FieldType == typeof(T))
				{
					var prop = typeof(BasePlugin).GetProperty (target);
					
					prop.SetValue (plugin, field.GetValue (null), null);
				}
			}
			catch (Exception e)
			{
				ProgramLog.Log (e);
			}
		}
		
		static BasePlugin LoadPluginAssembly (Assembly assembly)
		{
			foreach (var type in assembly.GetTypes().Where(x => typeof(BasePlugin).IsAssignableFrom(x) && !x.IsAbstract))
			{
				var plugin = CreatePluginInstance (type);
				if (plugin == null)
				{
					throw new Exception("Could not create plugin instance");
				}
				else
				{
					return plugin;
				}
			}
			return null;
		}
		
		static BasePlugin CreatePluginInstance (System.Type type)
		{
			var plugin = (BasePlugin) Activator.CreateInstance (type);
			
			if (plugin == null) return null;
			
			SetPluginProperty<string> (plugin, "NAME", "Name");
			SetPluginProperty<string> (plugin, "AUTHOR", "Author");
			SetPluginProperty<string> (plugin, "DESCRIPTION", "Description");
			SetPluginProperty<string> (plugin, "VERSION", "Version");
			SetPluginProperty<int> (plugin, "BUILD", "TDSMBuild");
			
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
				
				return LoadPluginAssembly (assembly);
			}
			catch (Exception e)
			{
				ProgramLog.Log (e, "Error loading plugin assembly " + PluginPath);
			}

			return null;
		}
		
		static readonly Dictionary<string, string> compilerOptions = new Dictionary<string, string> ()
		{
			{ "CompilerVersion", "v4.0" },
		};
		
		public static BasePlugin LoadSourcePlugin (string path)
		{
			
			var cp = new Microsoft.CSharp.CSharpCodeProvider (compilerOptions);
			var par = new System.CodeDom.Compiler.CompilerParameters ();
			par.GenerateExecutable = false;
			par.GenerateInMemory = true;
			par.IncludeDebugInformation = true;
			//par.CompilerOptions = "/optimize";
			par.TreatWarningsAsErrors = false;
			
			var us = Assembly.GetExecutingAssembly();
			par.ReferencedAssemblies.Add (us.Location);
			
			foreach (var asn in us.GetReferencedAssemblies())
			{
				par.ReferencedAssemblies.Add (asn.Name);
			}
			
			var result = cp.CompileAssemblyFromFile (par, path);
			
			var errors = result.Errors;
			if (errors != null)
			{
				if (errors.HasErrors)
					ProgramLog.Error.Log ("Failed to compile source plugin:");
				foreach (System.CodeDom.Compiler.CompilerError error in errors)
				{
					if (error.IsWarning)
						ProgramLog.BareLog (ProgramLog.Debug, error.ToString());
					else
						ProgramLog.BareLog (ProgramLog.Error, error.ToString());
				}
				if (errors.HasErrors)
					return null;
			}
			
			return LoadPluginAssembly (result.CompiledAssembly);
		}
		
		public static BasePlugin LoadPluginFromPath (string file)
		{
			FileInfo fileInfo = new FileInfo(file);
			var ext = fileInfo.Extension.ToLower();
			BasePlugin plugin = null;
			
			if (ext == ".dll")
			{
				ProgramLog.Plugin.Log ("Loading plugin from {0}.", fileInfo.Name);
				plugin = LoadPluginFromDLL(file);
			}
			else if (ext == ".cs")
			{
				ProgramLog.Plugin.Log ("Compiling and loading plugin from {0}.", fileInfo.Name);
				plugin = LoadSourcePlugin(file);
			}
			
			if (plugin != null)
			{
				plugin.Path = file;
				plugin.PathTimestamp = fileInfo.LastWriteTimeUtc;
				if (plugin.Name == null) plugin.Name = Path.GetFileNameWithoutExtension (file);
			}
			
			return plugin;
		}
		
		public static bool ReplacePlugin (BasePlugin oldPlugin, BasePlugin newPlugin)
		{
			lock (plugins)
			{
				if (oldPlugin.ReplaceWith (newPlugin))
				{
					string oldName = oldPlugin.Name.ToLower().Trim();
					string newName = newPlugin.Name.ToLower().Trim();
					
					if (plugins.ContainsKey (oldName))
						plugins.Remove (oldName);
					
					plugins.Add (newName, newPlugin);
					
					return true;
				}
			}
			
			return false;
		}
		
		public static bool ReloadPlugin (BasePlugin oldPlugin)
		{
			var fi = new FileInfo (oldPlugin.Path);
			
			BasePlugin newPlugin;
			
			if (fi.LastWriteTimeUtc > oldPlugin.PathTimestamp)
			{
				// plugin updated
				ProgramLog.Plugin.Log ("Plugin {0} is being updated from file.", oldPlugin.Name);
				newPlugin = LoadPluginFromPath (oldPlugin.Path);
			}
			else
			{
				ProgramLog.Plugin.Log ("Plugin {0} not updated, reinitializing.", oldPlugin.Name);
				newPlugin = CreatePluginInstance (oldPlugin.GetType());
			}
			
			if (newPlugin == null)
				return false;
			
			return ReplacePlugin (oldPlugin, newPlugin);
		}
		
		public static void LoadPlugins()
		{
			foreach (string file in Directory.GetFiles(pluginPath))
			{
				var plugin = LoadPluginFromPath (file);
				if (plugin != null)
				{
					if (plugin.InitializeAndHookUp ())
						plugins.Add (plugin.Name.ToLower().Trim(), plugin);
				}
			}

			EnablePlugins();
		}

		/// <summary>
		/// Reloads all plugins currently running on the server
		/// </summary>
		public static void ReloadPlugins ()
		{
			lock (plugins)
			{
				var list = plugins.Values.ToArray ();
				
				foreach (var plugin in list)
				{
					ReloadPlugin (plugin);
				}
			}
		}

		/// <summary>
		/// Enables all plugins available to the server
		/// </summary>
		public static void EnablePlugins()
		{
			foreach (var plugin in plugins.Values)
			{
				plugin.Enable();
			}
		}

		/// <summary>
		/// Disables all plugins currently running on the server
		/// </summary>
		public static void DisablePlugins()
		{
			foreach (var plugin in plugins.Values)
			{
				plugin.Disable();
			}

			plugins.Clear();
		}
		
		/// <summary>
		/// Enables a plugin by name. Currently unused in core
		/// </summary>
		/// <param name="name">Plugin name</param>
		/// <returns>Returns true on plugin successfully Enabling</returns>
		public static bool EnablePlugin(string name)
		{
			string cleanedName = name.ToLower().Trim();
			if(plugins.ContainsKey(cleanedName))
			{
				BasePlugin plugin = plugins[cleanedName];
				plugin.Enable();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Disables a plugin by name.  Currently unused in core
		/// </summary>
		/// <param name="name">Name of plugin</param>
		/// <returns>Returns true on plugin successfully Disabling</returns>
		public static bool DisablePlugin (string name)
		{
			string cleanedName = name.ToLower().Trim();
			if(plugins.ContainsKey(cleanedName))
			{
				BasePlugin plugin = plugins[cleanedName];
				plugin.Disable();
				return true;
			}
			return false;
		}

		public static bool DisposeOfPlugin (string name)
		{
			string cleanedName = name.ToLower().Trim();
			if(plugins.ContainsKey(cleanedName))
			{
				BasePlugin plugin = plugins[cleanedName];
				plugin.Dispose();
				plugins.Remove (cleanedName);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets plugin instance by name.
		/// </summary>
		/// <param name="name">Plugin name</param>
		/// <returns>Returns found plugin if successful, otherwise returns null</returns>
		public static BasePlugin GetPlugin (string name)
		{
			string cleanedName = name.ToLower().Trim();
			if(plugins.ContainsKey(cleanedName))
			{
				return plugins[cleanedName];
			}
			return null;
		}
		
		internal static void NotifyWorldLoaded ()
		{
			foreach (var kv in plugins)
			{
				kv.Value.NotifyWorldLoaded ();
			}
		}
	}
}
