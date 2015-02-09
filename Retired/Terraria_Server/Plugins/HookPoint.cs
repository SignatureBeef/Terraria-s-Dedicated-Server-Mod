using System;
using System.Threading;

using Terraria_Server.Misc;
using Terraria_Server.Logging;
using Terraria_Server.Commands;

namespace Terraria_Server.Plugins
{
	public delegate void HookAction<T> (ref HookContext context, ref T argument);
	
	public struct HookContext
	{
		public Networking.ClientConnection Connection  { get; internal set; }
		public ISender                     Sender      { get; set; }
		public Player                      Player      { get; set; }
		
		public object                      ResultParam { get; private set; }
		public HookResult                  Result      { get; private set; }
		public bool                        Conclude    { get; set; }
		
		public bool CheckForKick ()
		{
			if (Connection != null)
			{
				if (Result == HookResult.KICK)
				{
					var reason = ResultParam as string;
					Connection.Kick (reason ?? "Connection closed by plugin.");
					return true;
				}
				else if (Connection.State.DisconnectInProgress())
				{
					return true;
				}
			}
			
			return false;
		}
		
		public void SetResult (HookResult result, bool conclude = true)
		{
			Result = result;
			ResultParam = null;
			Conclude = conclude;
		}
		
		public void SetKick (string reason, bool conclude = true)
		{
			Result = HookResult.KICK;
			ResultParam = reason;
			Conclude = conclude;
		}
	}
	
	public enum HookResult
	{
		DEFAULT,
		CONTINUE,
		KICK,
		ASK_PASS,
		IGNORE,
		RECTIFY,
		ERASE
	}
	
	public enum HookOrder
	{
		FIRST,
		EARLY,
		NORMAL,
		LATE,
		TERMINAL
	}
	
	public abstract class HookPoint
	{
		public string Name { get; private set; }
		
		internal abstract Type DelegateType { get; }
		
		public HookPoint (string name)
		{
			Name = name;
		}
		
		public abstract int Count { get; }
		
		internal protected abstract void HookBase (BasePlugin plugin, Delegate callback, HookOrder order = HookOrder.NORMAL);
		
		internal protected void HookBase (Delegate callback, HookOrder order = HookOrder.NORMAL)
		{
			var plugin = callback.Target as BasePlugin;
			
			if (plugin == null) throw new ArgumentException ("Callback doesn't point to an instance method of class BasePlugin", "callback");
			
			HookBase (plugin, callback, order);
		}
		
		internal protected abstract void Unhook (BasePlugin plugin);
		
		internal abstract void Replace (BasePlugin oldPlugin, BasePlugin newPlugin, Delegate callback, HookOrder order);
		
		static PropertiesFile hookprop = new PropertiesFile ("hooks.properties");
		
		static HookPoint ()
		{
			hookprop.Load ();
		}
		
		internal int currentlyExecuting;
		internal int currentlyPaused;
		internal ManualResetEvent pauseSignal;
		
		[ThreadStatic]
		internal static bool threadInHook;
		
		internal protected static object editLock = new object(); //we use it recursively
		
		internal void Pause (ManualResetEvent signal) //.Set() the signal to unpause
		{
			lock (editLock)
			{
				if (pauseSignal != null)
				{
					throw new ApplicationException ("Attempt to pause hook invocation twice.");
				}
				
				pauseSignal = signal;
			}
		}
		
		internal void CancelPause ()
		{
			pauseSignal = null;
		}
		
		internal bool AllPaused
		{
			get
			{
				var num = currentlyExecuting - currentlyPaused;
				if (num < 0)
					ProgramLog.Debug.Log ("Oops, currentlyExecuting < currentlyPaused!?");
				return num <= 0;
			}
		}
	}
	
	public class HookPoint<T> : HookPoint
	{
		struct Entry
		{
			public HookOrder         order;
			public BasePlugin    plugin;
			public HookAction<T> callback;
		}
		
		Entry[] entries = new Entry[0];
		
		public override int Count
		{
			get { return entries.Length; }
		}
		
		internal override Type DelegateType
		{
			get { return typeof(HookAction<T>); }
		}
		
		public HookPoint (string name) : base(name)
		{
		}
		
		internal protected void Hook (BasePlugin plugin, HookAction<T> callback, HookOrder order = HookOrder.NORMAL)
		{
			lock (editLock)
			{
				var count = entries.Length;
				var copy = new Entry [count + 1];
				Array.Copy (entries, copy, count);
				
				copy[count] = new Entry { plugin = plugin, callback = callback, order = order };
				
				Array.Sort (copy, (Entry x, Entry y) => x.order.CompareTo (y.order));
				
				entries = copy;
				
//				lock (plugin.hooks) //disabled as long as editLock is static
				{
					plugin.hooks.Add (this);
				}
			}
		}
		
		internal protected void Hook (HookAction<T> callback, HookOrder order = HookOrder.NORMAL)
		{
			var plugin = callback.Target as BasePlugin;
			
			if (plugin == null) throw new ArgumentException ("Callback doesn't point to an instance method of class BasePlugin", "callback");
			
			Hook (plugin, callback, order);
		}
		
		internal protected override void HookBase (BasePlugin plugin, Delegate callback, HookOrder order = HookOrder.NORMAL)
		{
			var cb = callback as HookAction<T>;
			
			if (cb == null) throw new ArgumentException (string.Format ("A callback of type HookAction<{0}> expected.", typeof(T).Name), "callback");
			
			Hook (plugin, cb, order);
		}
		
		internal protected override void Unhook (BasePlugin plugin)
		{
			lock (editLock)
			{
				var count = entries.Length;
				
				int k = 0;
				for (int i = 0; i < count; i++)
				{
					if (entries[i].plugin != plugin)
					{
						k++;
					}
				}
				
				var copy = new Entry [k];
				
				k = 0;
				for (int i = 0; i < count; i++)
				{
					if (entries[i].plugin != plugin)
					{
						copy[k++] = entries[i];
					}
				}
				
				entries = copy;
				
//				lock (plugin.hooks) //disabled as long as editLock is static
				{
					try
					{
						plugin.hooks.Remove (this);
					}
					catch (Exception e)
					{
						ProgramLog.Log (e, "Exception removing hook from plugin's hook list");
					}
				}
			}
		}
		
		internal override void Replace (BasePlugin oldPlugin, BasePlugin newPlugin, Delegate callback, HookOrder order)
		{
			lock (editLock)
			{
				for (int i = 0; i < entries.Length; i++)
				{
					if (entries[i].plugin == oldPlugin)
					{
						entries[i] = new Entry { plugin = newPlugin, callback = (HookAction<T>) callback, order = order };
						break;
					}
				}
			}
		}
		
		public void Invoke (ref HookContext context, ref T arg)
		{
			var hooks = entries;
			var len = hooks.Length;
			bool locked;
			
			if (len > 0)
			{
				locked = true;
				Interlocked.Increment (ref currentlyExecuting);
			}
			else
				locked = false;
			
			var signal = pauseSignal;
			if (signal != null)
			{
				pauseSignal = null;
				ProgramLog.Debug.Log ("Paused hook point {0}.", Name);
				//Interlocked.Decrement (ref currentlyExecuting);
				Interlocked.Increment (ref currentlyPaused);
				signal.WaitOne ();
				Interlocked.Decrement (ref currentlyPaused);
				//Interlocked.Increment (ref currentlyExecuting);
				ProgramLog.Debug.Log ("Unpaused hook point {0}.", Name);
			}
			
			try
			{
				threadInHook = true;
				
				for (int i = 0; i < len; i++)
				{
					if (hooks[i].plugin.IsEnabled)
					{
						try
						{
							hooks[i].callback (ref context, ref arg);
							
							if (context.Conclude)
							{
								return;
							}
						}
						catch (Exception e)
						{
							ProgramLog.Log (e, string.Format ("Plugin {0} crashed in hook {1}", hooks[i].plugin.Name, Name));
						}
					}
				}
			}
			finally
			{
				threadInHook = false;
				
				if (locked)
					Interlocked.Decrement (ref currentlyExecuting);
			}
		}
		
		static void SortEntries (ref Entry[] array)
		{
			//var count = new int[5];
			
			//TODO: implement configurable sorting
		}
	}
}

