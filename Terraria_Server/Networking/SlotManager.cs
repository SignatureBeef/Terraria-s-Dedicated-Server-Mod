using System;
using System.Collections.Generic;

using Terraria_Server.Misc;
using Terraria_Server.Logging;
using Terraria_Server.Commands;

namespace Terraria_Server.Networking
{
	public static class SlotManager
	{
		const int MAX_SLOTS = 254;
		
		static Queue[] queues = new Queue [4];
		
		static int maxSlots;
		static int overlimitSlots;
		static int privSlotsInUse;
		
		static bool[] isPrivileged = new bool [MAX_SLOTS + 1];
		
		static Stack<int> freeSlots = new Stack<int> (); // those configured by Main.maxNetplayers
		static Stack<int> privFreeSlots = new Stack<int> (); // those left over
		
		static ClientConnection[] handovers = new ClientConnection [MAX_SLOTS + 1];
		
		static object syncRoot = new object();
		
		public static int MaxSlots
		{
			get { return maxSlots; }
		}
		
		public static int OverlimitSlots
		{
			get { return overlimitSlots; }
		}
		
		public static bool IsPrivileged (int id)
		{
			return isPrivileged[id];
		}
		
		public static void Initialize (int maxSlots, int overlimitSlots = 0)
		{
			if (maxSlots < 1) maxSlots = 1;
			else if (maxSlots > MAX_SLOTS) maxSlots = MAX_SLOTS;
			
			if (overlimitSlots < 0) overlimitSlots = 0;
			else if (overlimitSlots > maxSlots) overlimitSlots = maxSlots;

			ProgramLog.Log("{2} {0}+{1} {3}.", maxSlots, overlimitSlots, Language.Languages.InitializingSlotManagerFor, Language.Languages.Players);
			
			lock (syncRoot)
			{
				freeSlots.Clear ();
				
				for (int i = maxSlots - 1; i >= 0; i--)
				{
					isPrivileged[i] = false;
					freeSlots.Push (i);
				}
				
				privFreeSlots.Clear ();
				
				for (int i = MAX_SLOTS; i >= maxSlots; i--)
				{
					isPrivileged[i] = true;
					privFreeSlots.Push (i);
				}
			
				for (int q = 0; q < 4; q++) queues[q] = new Queue ();
				
				privSlotsInUse = 0;
				SlotManager.maxSlots = maxSlots;
				SlotManager.overlimitSlots = overlimitSlots;
			}
		}
		
		public static int ChangeLimits (int newMaxSlots, int newOverlimitSlots)
		{
			if (newMaxSlots < 1) newMaxSlots = 1;
			else if (newMaxSlots > MAX_SLOTS) newMaxSlots = MAX_SLOTS;
			
			if (newOverlimitSlots < 0) newOverlimitSlots = 0;
			
			var newSlots = new Stack<int> ();
			
			lock (syncRoot)
			{
				int diff = newMaxSlots - maxSlots;
				maxSlots = newMaxSlots;
				
				if (diff > 0)
				{
					for (int i = 0; i < diff; i++)
					{
						if (privFreeSlots.Count > 0)
						{
							var id = privFreeSlots.Pop ();
							//freeSlots.Push (id);
							newSlots.Push (id);
							isPrivileged[id] = false;
						}
						else
						{
							maxSlots -= diff - i;
						}
					}
				}
				else if (diff < 0)
				{
					for (int i = 0; i < -diff; i++) // remove some free slots
					{
						if (freeSlots.Count > 0)
						{
							var id = freeSlots.Pop ();
							//privFreeSlots.Push (id);
							newSlots.Push (id);
							isPrivileged[id] = true;
							privSlotsInUse += 1; // this gets decremented from FreeSlot
							diff += 1;
							
						}
						else
							break;
					}
					
					if (diff < 0) // mark some used slots privileged
					{
						for (int i = 0; i < MAX_SLOTS; i++)
						{
							int id = (MAX_SLOTS + maxSlots - i) % MAX_SLOTS;
							
							if (! isPrivileged[id])
							{
								isPrivileged[id] = true;
								privSlotsInUse += 1;
								diff += 1;
								if (diff == 0) break;
							}
						}
					}
					
					if (diff < 0)
					{
						maxSlots -= diff;
					}
				}
				
				overlimitSlots = Math.Min (maxSlots, newOverlimitSlots);
			}
			
			while (newSlots.Count > 0)
			{
				var id = newSlots.Pop ();
				FreeSlot (id);
			}
			
			return maxSlots;
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
		
		public static bool HandoverSlot (ClientConnection src, ClientConnection dst)
		{
			int id = src.SlotIndex;
			
			if (id < 0 || id > MAX_SLOTS) return false;
			
			lock (syncRoot)
			{
				handovers[id] = dst;
				
				// in case the connection dies during the operation
				if (src.SlotIndex != id)
				{
					handovers[id] = null;
					return false;
				}
			}
			
			return true;
		}
		
		static int AssignSlotOrQueue (ClientConnection conn, int queue)
		{
			int id = -1;
			
			conn.Queue = queue;
			
			if (queue == 3)
			{
				if (privFreeSlots.Count > 0)
				{
					// use other slots for highest priority connections first
					id = privFreeSlots.Pop ();
					privSlotsInUse += 1;
				}
				else if (freeSlots.Count > 0)
					id = freeSlots.Pop ();
			}
			else if (freeSlots.Count > 0 && freeSlots.Count > (privSlotsInUse - overlimitSlots))
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
		
		static bool AssignSlot (ClientConnection conn, int id)
		{
			if (! conn.AssignSlot (id))
				return false;

			conn.State = SlotState.ASSIGNING_SLOT;
			conn.ResetTimeout ();
			
			var slot = NetPlay.slots[id];
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
			if (isPrivileged[id])
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
			if (isPrivileged[id])
			{
				privFreeSlots.Push (id);
				privSlotsInUse = Math.Max (0, privSlotsInUse - 1);
			}
			else
				freeSlots.Push (id);
		}
		
		public static void FreeSlot (int id)
		{
			ClientConnection assignedTo = null;
			
			var slot = NetPlay.slots[id];
			slot.Reset ();
			
			lock (syncRoot)
			{
				if (handovers[id] != null)
				{
					assignedTo = handovers[id];
					handovers[id] = null;
					
					if (! AssignSlot (assignedTo, id))
					{
						assignedTo = null;
						ProgramLog.Debug.Log ("Slot {0} handover failed.", id);
					}
					else
						ProgramLog.Debug.Log ("Slot {0} handover successful.", id);
				}
				
				if (assignedTo == null)
				{
					assignedTo = FindForSlot (id);
				}
					
				if (assignedTo == null)
				{
					PushSlot (id);
					ProgramLog.Debug.Log ("Freed slot {0}", id);
					
					// this is for when a privileged slot is freed, but no privileged clients are waiting
					// so we check if we have an unprivileged slot to use
					if (isPrivileged[id] && (freeSlots.Count > 0 && freeSlots.Count > (privSlotsInUse - overlimitSlots)))
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
		
		// TODO: optimize
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
		
		const string waitingMessage = "Waiting for free slot... (You are {0}{1} in line)\n";
		
		internal static string WaitingMessage (ClientConnection conn)
		{
			int i = 0;
			for (int q = 3; q > conn.Queue; q--)
			{
				i += queues[q].Count;
			}
			
			i += Math.Min (queues[conn.Queue].Count, Math.Max (0, conn.IndexInQueue - queues[conn.Queue].TotalDequeued));
			
			if (i <= 0) i = 1;
			
			string suffix = "th";
			
			switch (i % 10)
			{
				case 1:
					suffix = "st";
					break;
				
				case 2:
					suffix = "nd";
					break;
					
				case 3:
					suffix = "rd";
					break;
			}
			
			switch (i % 100)
			{
				case 11:
				case 12:
				case 13:
					suffix = "th";
					break;
			}
			
			return String.Format (waitingMessage, i, suffix);
		}

		public static bool MaxPlayersDisabled = false;
		internal static void MaxPlayersCommand (ISender sender, ArgumentList args)
		{
			if (MaxPlayersDisabled)
			{
				sender.Message (255, "This command has been disabled.");
				return;
			}

			int maxp = -1;
			int overl = -1;
			
			if (args.TryGetInt (0, out maxp) && (maxp < 1 || maxp > MAX_SLOTS))
			{
				sender.Message (255, "Max numbers of players must be in range 1 .. {0}", MAX_SLOTS);
				return;
			}
			
			if (args.Count > 1)
			{
				overl = args.GetInt (1);
				if (overl < 0 || overl > maxp)
				{
					sender.Message (255, "Number of overlimit slots must be in range 0 .. <max player count>");
					return;
				}
			}
			
			int oldmax = maxSlots;
			int oldover = overlimitSlots;
			
			int result = maxSlots;
			if (maxp >= 0 || overl >= 0)
			{
				result = ChangeLimits (maxp < 0 ? maxSlots : maxp, overl < 0 ? overlimitSlots : overl);
				Server.notifyOps(
					String.Format("Max player slots changed to {0}+{1}. [{2}]", result, overlimitSlots, sender.Name)
				);
			}
			
			sender.Message (255, ChatColor.SteelBlue, "Max player slots: {0}, overlimit slots: {1}", result, overlimitSlots);
			
			if (oldmax != maxSlots || oldover != overlimitSlots)
			{
				Program.properties.MaxPlayers = maxSlots;
				Program.properties.OverlimitSlots = overlimitSlots;
				Program.properties.Save (true);
			}
		}
		
		internal static void QCommand (ISender sender, ArgumentList args)
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
						sender.Message (255, ChatColor.Snow, "Queued with priority {0}:", q);
						
						for (int i = 0; i < queue.count; i++)
						{
							int k = (queue.head + i) % queue.array.Length;
							var conn = queue.array[k];
							
							if (conn != null)
							{
								sender.Message (255, ChatColor.Beige, "  {0}. {1} from {2}", c, conn.Player.Name, conn.RemoteAddress);
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

