using System;
using System.Threading;

namespace Terraria_Server.Misc
{
	public struct ProducerConsumerSignal
	// this struct is meant to reduce overhead on mono, where AutoResetEvent.Set()
	// locks at least two mutexes; should be safe for a single producer and single consumer
	
	// ACTUALLY: it was causing problems, I dunno why I used it with multiple producers too...
	//           so it's just an AutoResetEvent for now
	{
		readonly AutoResetEvent handle;
		//volatile bool signalled;
		
		public ProducerConsumerSignal (bool initial)
		{
			handle = new AutoResetEvent (initial);
			//signalled = false;
		}
		
		public void Signal ()
		{
			//var sig = signalled;
			//signalled = true;
			//if (! sig)
			handle.Set ();
		}
		
		public void WaitForIt ()
		{
			//signalled = false;
			handle.WaitOne ();
		}
		
		public void WaitForIt (int ms)
		{
			//signalled = false;
			handle.WaitOne (ms);
		}

	}
}

