using System;
using System.Collections.Generic;

using Terraria_Server.Misc;
using Terraria_Server.Logging;
using Terraria_Server.Commands;

namespace Terraria_Server.Networking
{
	public static class SlotManager
	{
		static Queue[] queues = new Queue [4];
		static int maxSlots;
		static int overlimitSlots;
		static int otherSlotsInUse;
		static Stack<int> freeSlots = new Stack<int> (); // those configured by Main.maxNetplayers
		static Stack<int> otherFreeSlots = new Stack<int> (); // those left over
		static object syncRoot = new object();
		
		public static void Initialize (int maxSlots, int overlimitSlots = 0)
		{
			lock (syncRoot)
			{
				for (int i = maxSlots - 1; i >= 0; i--)
				{
					freeSlots.Push (i);
				}
				
				for (int i = 253; i >= maxSlots; i--)
				{
					otherFreeSlots.Push (i);
				}
			}
			
			for (int q = 0; q < 4; q++) queues[q] = new Queue ();
			
			otherSlotsInUse = 0;
			SlotManager.maxSlots = maxSlots;
			SlotManager.overlimitSlots = overlimitSlots;
		}
		
		public static int Schedule (ClientConnection conn, int priority)
		{
			int id;
			
			lock (syncRoot)
			{
				if (priority > 2 || priority < 0)
				{
					// bypass queue
					id = AssignSlotOrQueue (conn, 3);
				}
				else
					id = AssignSlotOrQueue (conn, priority);
			}
			
			// we send here, so as to prevent a deadlock between our lock
			// and the connection's sendqueue lock
			if (id == -1)
			{
				var msg = NetMessage.PrepareThreadInstance ();
				msg.SendTileLoading (1, WaitingMessage (conn));
				conn.Send (msg.Output);
			}
			else if (id >= 0)
			{
				var msg = NetMessage.PrepareThreadInstance ();
				msg.ConnectionResponse (id);
				conn.Send (msg.Output);
			}
			
			return id;
		}
		
		static int AssignSlotOrQueue (ClientConnection conn, int queue)
		{
			int id = -1;

			if (queue == 3)
			{
				if (otherFreeSlots.Count > 0)
				{
					// use other slots for highest priority connections first
					id = otherFreeSlots.Pop ();
					otherSlotsInUse += 1;
				}
				else if (freeSlots.Count > 0)
					id = freeSlots.Pop ();
			}
			else if (freeSlots.Count > 0 && freeSlots.Count > (otherSlotsInUse - overlimitSlots))
				id = freeSlots.Pop ();
			
			if (id == -1)
			{
				conn.State = SlotState.QUEUED;
				conn.ResetTimeout ();
				
				queues[queue].Enqueue (conn);
				
				ProgramLog.Debug.Log ("Connection {0} queued with priority {1}.", conn.RemoteAddress, queue);
				
				return -1;
			}
			
			if (AssignSlot (conn, id))
				return id;
			
			PushSlot (id);
			return -2;
		}
		
		public static bool AssignSlot (ClientConnection conn, int id)
		{
			if (! conn.AssignSlot (id))
				return false;

			conn.State = SlotState.ASSIGNING_SLOT;
			conn.ResetTimeout ();
			
			var slot = Netplay.slots[id];
			slot.remoteAddress = conn.RemoteAddress;
			slot.conn = conn;
			
			conn.Player.whoAmi = id;
			Main.players[id] = conn.Player;
			
			var age = conn.Age;
			if (age > TimeSpan.FromMilliseconds (500))
				ProgramLog.Debug.Log ("Slot {1} assigned to {0} after {2}.", slot.remoteAddress, id, age);
			else
				ProgramLog.Debug.Log ("Slot {1} assigned to {0}.", slot.remoteAddress, id);
			
			return true;
		}
		
		static ClientConnection FindForSlot (int id)
		{
			if (id >= maxSlots)
			{
				while (queues[3].Count > 0) // a loop against race conds, if connection already died, try another
				{
					var conn = queues[3].Dequeue ();
					if (conn != null && AssignSlot (conn, id))
					{
						return conn;
					}
				}
			}
			else
			{
				for (int q = 3; q >= 0; q--)
				{
					while (queues[q].Count > 0)
					{
						var conn = queues[q].Dequeue ();
						if (conn != null && AssignSlot (conn, id))
						{
							return conn;
						}
					}
				}
			}
			
			return null;
		}
		
		static void PushSlot (int id)
		{
			if (id >= maxSlots)
			{
				otherFreeSlots.Push (id);
				otherSlotsInUse = Math.Max (0, otherSlotsInUse - 1);
			}
			else
				freeSlots.Push (id);
		}
		
		public static void FreeSlot (int id)
		{
			ClientConnection assignedTo = null;
			
			var slot = Netplay.slots[id];
			slot.Reset ();
			
			lock (syncRoot)
			{
				assignedTo = FindForSlot (id);
				
				if (assignedTo == null)
				{
					PushSlot (id);
					ProgramLog.Debug.Log ("Freed slot {0}", id);
					
					// this is for when a privileged slot is freed, but no privileged clients are waiting
					// so we check if we have an unprivileged slot to use
					if (id >= maxSlots && (freeSlots.Count > 0 && freeSlots.Count > (otherSlotsInUse - overlimitSlots)))
					{
						id = freeSlots.Pop ();
						assignedTo = FindForSlot (id);
						
						if (assignedTo == null)
						{
							freeSlots.Push (id);
							return;
						}
					}
					else
						return;
				}
			}
			
			var msg = NetMessage.PrepareThreadInstance ();
			msg.ConnectionResponse (id);
			assignedTo.Send (msg.Output);
		}
		
		public static void RemoveFromQueues (ClientConnection conn)
		{
			lock (syncRoot)
			{
				for (int q = 3; q >= 0; q--)
				{
					queues[q].Remove (conn);
				}
			}
		}
		
		const string waitingMessagePrefix = "Waiting for free slot... (You are";
		const string waitingMessageSuffix = "in line)\n";
		
		internal static string WaitingMessage (ClientConnection conn)
		{
			int i = 0;
			for (int q = 3; q > conn.Queue; q--)
			{
				i += queues[q].Count;
			}
			
			i += Math.Min (queues[conn.Queue].Count, Math.Max (0, conn.IndexInQueue - queues[conn.Queue].TotalDequeued));
			
			if (i <= 0) i = 1;
			
			switch (i % 100)
			{
				case 11:
				case 12:
				case 13:
					return string.Format ("{1} {0}th {2}", i, waitingMessagePrefix, waitingMessageSuffix);
			}
			
			switch (i % 10)
			{
				case 1:
					return string.Format ("{1} {0}st {2}", i, waitingMessagePrefix, waitingMessageSuffix);
				
				case 2:
					return string.Format ("{1} {0}nd {2}", i, waitingMessagePrefix, waitingMessageSuffix);
				
				case 3:
					return string.Format ("{1} {0}rd {2}", i, waitingMessagePrefix, waitingMessageSuffix);
			}
			
			return string.Format ("{1} {0}th {2}", i, waitingMessagePrefix, waitingMessageSuffix);
		}
		
		internal static void QCommand (Server server, ISender sender, ArgumentList args)
		{
			args.ParseNone ();
			
			lock (syncRoot)
			{
				int c = 1;
				
				for (int q = 3; q >= 0; q--)
				{
					var queue = queues[q];
					
					if (queue.Count > 0)
					{
						sender.Message (255, ChatColour.Snow, "Queued with priority {0}:", q);
						
						for (int i = 0; i < queue.count; i++)
						{
							int k = (queue.head + i) % queue.array.Length;
							var conn = queue.array[k];
							
							if (conn != null)
							{
								sender.Message (255, ChatColour.Beige, "  {0}. {1} from {2}", c, conn.Player.Name, conn.RemoteAddress);
								c += 1;
							}
						}
					}
				}
			}
		}
	}
	
	class Queue
	{
		internal ClientConnection[] array;
		internal int head;
		internal int count;
		internal int realCount;
		
		public Queue ()
		{
			array = new ClientConnection [16];
		}
		
		public int Count { get { return realCount; } }
		
		public int TotalEnqueued { get; private set; }
		public int TotalDequeued { get; private set; }
		
		void Grow ()
		{
			int len = array.Length;
			
			var a = new ClientConnection [len * 3 / 2 + 16];
			
			for (int i = 0; i < count; i++)
			{
				int k = (head + i) % len;
				a[i] = array[k];
			}
			
			head = 0;
			
			array = a;
		}
		
		public void Enqueue (ClientConnection c)
		{
			if (count == array.Length) Grow ();
			
			int tail = (head + count) % array.Length;
			array[tail] = c;
			count += 1;
			realCount += 1;
			
			TotalEnqueued += 1;
			c.IndexInQueue = TotalEnqueued;
		}
		
		public ClientConnection Dequeue ()
		{
			if (count == 0) return null;
			
			ClientConnection c = null;
			while (c == null && count > 0)
			{
				c = array[head];
				array[head] = null;
				head = (head + 1) % array.Length;
				count -= 1;
				TotalDequeued += 1;
			}
			
			if (c != null) realCount -= 1;
			
			return c;
		}
		
		public void Remove (ClientConnection c)
		{
			var len = array.Length;
			
			for (int i = 0; i < count; i++)
			{
				int k = (head + i) % len;
				if (array[k] == c)
				{
					realCount -= 1;
					array[k] = null;
				}
			}
			
			while (array[head] == null && count > 0)
			{
				head = (head + 1) % array.Length;
				count -= 1;
				TotalDequeued += 1;
			}
			
			while (count > 0 && array[(head + count - 1) % array.Length] == null)
			{
				count -= 1;
				TotalDequeued += 1;
			}
		}
		
		public void Clear ()
		{
			var len = array.Length;
			
			for (int i = 0; i < count; i++)
			{
				int k = (head + i) % len;
				array[k] = null;
			}
			
			count = 0;
			realCount = 0;
			head = 0;
		}
	}
}

