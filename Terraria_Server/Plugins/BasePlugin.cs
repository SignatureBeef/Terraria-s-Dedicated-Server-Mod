using System;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;

using Terraria_Server.Events;
using Terraria_Server.Commands;
using Terraria_Server.Logging;

namespace Terraria_Server.Plugins
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
		/// Whether this plugin is enabled or not
		/// </summary>
		public bool Enabled
		{
			get { return enabled == 1; }
		}
		
		internal int enabled;
		
		protected BasePlugin ()
		{
			Description = "";
			Author = "";
			Version = "";
		}
		
		/// <summary>
		/// Load routines, typically setting up plugin instances, initial values, etc; called before Enable() in startup
		/// </summary>
		protected virtual void OnInitialize () { }
		
		protected virtual void OnDispose () { }
		
		//public void abstract UnLoad(); //I have high hopes :3
		
		/// <summary>
		/// Enable routines, usually no more than enabled announcement and registering hooks
		/// </summary>
		protected virtual void OnEnable () { }
		
		/// <summary>
		/// Disabling the plugin, usually announcement
		/// </summary>
		protected virtual void OnDisable () { }
		
		/// <summary>
		/// Plugin's internal command list
		/// </summary>
		internal Dictionary<string, CommandInfo> commands = new Dictionary<string, CommandInfo> ();
		
		internal HashSet<HookPoint> hooks = new HashSet<HookPoint> ();
		
		/// <summary>
		/// Adds new command to the server's command list
		/// </summary>
		/// <param name="prefix">Command text</param>
		/// <returns>New Command</returns>
		protected CommandInfo AddCommand (string prefix)
		{
			if (commands.ContainsKey (prefix)) throw new ApplicationException ("AddCommand: duplicate command: " + prefix);
			
			var cmd = new CommandInfo ();
			commands[prefix] = cmd;
			commands[string.Concat (Name.ToLower(), ".", prefix)] = cmd;
			
			return cmd;
		}
		
		internal bool Enable ()
		{
			if (Interlocked.CompareExchange (ref this.enabled, 1, 0) == 0)
			{
				try
				{
					OnEnable ();
				}
				catch (Exception e)
				{
					ProgramLog.Log (e, "Exception while enabling plugin " + Name);
					return false;
				}
			}
			return true;
		}
		
		internal bool Disable ()
		{
			if (Interlocked.CompareExchange (ref this.enabled, 0, 1) == 1)
			{
				try
				{
					OnDisable ();
				}
				catch (Exception e)
				{
					ProgramLog.Log (e, "Exception while disabling plugin " + Name);
					return false;
				}
			}
			return true;
		}
		
		internal bool Initialize ()
		{
			var methods = this.GetType().GetMethods (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
			
			foreach (var method in methods)
			{
				var attr = method.GetCustomAttributes (typeof(HookAttribute), true);
				if (attr.Length > 0)
				{
					var ha = attr[0] as HookAttribute;
					var hpName = method.GetParameters()[1].ParameterType.GetElementType().Name;
					
					var hookPoint = typeof(HookPoints).GetField(hpName).GetValue(null) as HookPoint;
					
					Delegate callback;
					if (method.IsStatic)
						callback = Delegate.CreateDelegate (hookPoint.DelegateType, method);
					else
						callback = Delegate.CreateDelegate (hookPoint.DelegateType, this, method);
					
					hookPoint.HookBase (this, callback, ha.order);
					//ha.hookPoint.Hook (new Hook
				}
			}
			
			try
			{
				OnInitialize ();
			}
			catch (Exception e)
			{
				ProgramLog.Log (e, "Exception in initialization handler of plugin " + Name);
				return false;
			}
			
			return true;
		}
		
		internal bool Dispose ()
		{
			var result = Disable ();
			
			try
			{
				OnDispose ();
			}
			catch (Exception e)
			{
				ProgramLog.Log (e, "Exception in disposal handler of plugin " + Name);
				result = false;
			}
			
			commands.Clear ();
			
			var hooks = new HookPoint [this.hooks.Count];
			this.hooks.CopyTo (hooks, 0, hooks.Length);
			
			foreach (var hook in hooks)
			{
				hook.Unhook (this);
			}
			
			if (this.hooks.Count > 0)
			{
				ProgramLog.Error.Log ("Warning: failed to clean up {0} hooks of plugin {1}.", this.hooks.Count, Name);
				this.hooks.Clear ();
			}
			
			return result;
		}
	}
}
