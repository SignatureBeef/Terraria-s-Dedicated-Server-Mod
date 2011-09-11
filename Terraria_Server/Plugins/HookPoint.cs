using System;

using Terraria_Server.Misc;
using Terraria_Server.Logging;
using Terraria_Server.Commands;

namespace Terraria_Server.Plugins
{
	public delegate bool HookAction<T> (ref HookContext context, ref T argument);
	
	public struct HookContext
	{
		public Networking.ClientConnection Connection  { get; internal set; }
		public ISender                     Sender      { get; set; }
		public Player                      Player      { get; set; }
		
		public object                      ResultParam { get; private set; }
		public HookResult                  Result      { get; private set; }
		
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
		
		public void SetResult (HookResult result)
		{
			Result = result;
			ResultParam = null;
		}
		
		public void SetKick (string reason)
		{
			Result = HookResult.KICK;
			ResultParam = reason;
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
		
		public abstract void HookBase (BasePlugin plugin, Delegate callback, HookOrder order = HookOrder.NORMAL);
		
		public void HookBase (Delegate callback, HookOrder order = HookOrder.NORMAL)
		{
			var plugin = callback.Target as BasePlugin;
			
			if (plugin == null) throw new ArgumentException ("Callback doesn't point to an instance method of class BasePlugin", "callback");
			
			HookBase (plugin, callback, order);
		}
		
		public abstract void Unhook (BasePlugin plugin);
		
		static PropertiesFile hookprop = new PropertiesFile ("hooks.properties");
		
		static HookPoint ()
		{
			hookprop.Load ();
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
		
		object editLock = new object();
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
		
		public void Hook (BasePlugin plugin, HookAction<T> callback, HookOrder order = HookOrder.NORMAL)
		{
			lock (editLock)
			{
				var count = entries.Length;
				var copy = new Entry [count + 1];
				Array.Copy (entries, copy, count);
				
				copy[count] = new Entry { plugin = plugin, callback = callback, order = order };
				
				Array.Sort (copy, (Entry x, Entry y) => x.order.CompareTo (y.order));
				
				entries = copy;
				
				lock (plugin.hooks)
				{
					plugin.hooks.Add (this);
				}
			}
		}
		
		public void Hook (HookAction<T> callback, HookOrder order = HookOrder.NORMAL)
		{
			var plugin = callback.Target as BasePlugin;
			
			if (plugin == null) throw new ArgumentException ("Callback doesn't point to an instance method of class BasePlugin", "callback");
			
			Hook (plugin, callback, order);
		}
		
		public override void HookBase (BasePlugin plugin, Delegate callback, HookOrder order = HookOrder.NORMAL)
		{
			var cb = callback as HookAction<T>;
			
			if (cb == null) throw new ArgumentException (string.Format ("A callback of type HookAction<{0}> expected.", typeof(T).Name), "callback");
			
			Hook (plugin, cb, order);
		}
		
		public override void Unhook (BasePlugin plugin)
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
				
				lock (plugin.hooks)
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
		
		public void Invoke (ref HookContext context, ref T arg)
		{
			var hooks = entries;
			for (int i = 0; i < hooks.Length; i++)
			{
				if (hooks[i].plugin.Enabled)
				{
					try
					{
						if (hooks[i].callback (ref context, ref arg))
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
		
		static void SortEntries (ref Entry[] array)
		{
			//var count = new int[5];
			
			//TODO: implement configurable sorting
		}
	}
}

