using System;
using System.Threading;
using System.Runtime.InteropServices;

namespace Terraria_Server
{
	public class ProgramThread
	{
		Thread thread;
		
		public ProgramThread (string name, Action entrypoint)
		{
			Name = name;
			thread = new Thread ((object arg) => 
			{
				SetOSThreadName (name);
				entrypoint();
			});
			thread.Name = name;
			thread.IsBackground = true;
		}
		
		public string Name { get; private set; }
		
		public bool IsBackground
		{
			get { return thread.IsBackground; }
			set { thread.IsBackground = true; }
		}
		
		public bool IsAlive
		{
			get { return thread.IsAlive; }
		}
		
		public void Start ()
		{
			thread.Start ();
		}
		
		private static volatile bool have_prctl = true;
		
		[DllImport("libc.so.6")]
		private static extern int prctl (int op, string str, int x, int y, int z);
		
		private static void SetOSThreadName (string name)
		{
			if (have_prctl)
			{
				try
				{
					prctl (15, "TDSM: " + name, 0, 0, 0);
				}
				catch
				{
					have_prctl = false;
				}
			}
		}

	}
}

