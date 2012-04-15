using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using Terraria_Server.Logging;

namespace Terraria_Server.Networking
{
	public static class LiquidUpdateBuffer
	{
		struct UpdateRecord
		{
			public short x;
			public short y;
			public short next;
		}
		
		
		//static Dictionary<int, short> tiles = new Dictionary<int, short> ();
//		static HashSet<int> tiles = new HashSet<int> ();
		static int maxUpdates = Liquid.maxLiquid * 3;
		static short freeList = -1;
		static UpdateRecord[] updates = new UpdateRecord [maxUpdates];
		static short[,] sections;
		static HashSet<int> queuedSections = new HashSet<int> ();
//		static int numUpdates = 0;
		//static byte[,] lastVolume;
		
		static short TakeFromPool ()
		{
			if (freeList >= 0)
			{
				short head = freeList;
				freeList = updates[freeList].next;
				return head;
			}
			return -1;
		}
		
		static void Free (short i)
		{
			updates[i].next = freeList;
			freeList = i;
		}
		
		public static void Initialize (int sectionsX, int sectionsY)
		{
			sections = new short [sectionsX, sectionsY];
			
			for (int x = 0; x < sectionsX; x++)
			{
				for (int y = 0; y < sectionsY; y++)
				{
					sections[x, y] = -1;
				}
			}
			
			for (short i = 0; i < updates.Length - 1; i++)
			{
				updates[i].next = (short) (i + 1);
			}
			
			updates[updates.Length - 1].next = -1;
			
			freeList = 0;
			
			queuedSections.Clear ();
			sectionRecords.Clear ();
			
//			lastVolume = new byte [Main.maxTilesX, Main.maxTilesY];
			
//			for (int i = 0; i < Main.maxTilesX; i++)
//			{
//				for (int j = 0; j < Main.maxTilesY; j++)
//				{
//					lastVolume[i, j] = Main.tile.At(i, j).Liquid;
//				}
//			}
			
//			numUpdates = 0;
		}
		
//		static int done;
//		static int skipped;
		
		public static void QueueUpdate (int x, int y)
		{
//			int key = x | (y << 16);
//			if (tiles.Contains (key))
//			{
//				skipped += 1;
//				return;
//			}
//			done += 1;
			
			short u = TakeFromPool ();
			
			if (u == -1) ProgramLog.Error.Log ("LiquidUpdateBuffer: Ran out of records.");
			else
			{
				int sx = x / 200;
				int sy = y / 150;

//				tiles.Add (key);
				
				updates[u].x = (short) x;
				updates[u].y = (short) y;
				updates[u].next = sections[sx, sy];
				sections[sx, sy] = u;
				queuedSections.Add (sx | (sy << 16));
				
//				numUpdates += 1;
			}
		}
		
		struct SectionRecord
		{
			public byte[] bytes;
			public short x;
			public short y;
		}
		
		static List<SectionRecord> sectionRecords = new List<SectionRecord> ();
		static NetMessage updateBuilder = new NetMessage (15 * maxUpdates);
		
//		static long Total;
//		static int ctr;
		public static void FlushQueue ()
		{
//			ctr ++;
//			if (ctr < 30) return;
//			ctr = 0;
//			tiles.Clear ();
			
			var msg = updateBuilder;
//			int total = 0;
			
			foreach (int section in queuedSections)
			{
				short x = (short) (section & 0xffff);
				short y = (short) (section >> 16);
				
				short u = sections[x, y];
				sections[x, y] = -1;
				
				while (u != -1)
				{
//					var tile = Main.tile.At (updates[u].x, updates[u].y);
//					if (Math.Abs ((int)tile.Liquid - (int)lastVolume[updates[u].x, updates[u].y]) >= 32)
					{
//						lastVolume[updates[u].x, updates[u].y] = tile.Liquid;
						msg.FlowLiquid (updates[u].x, updates[u].y);
					}
					var v = u;
					u = updates[u].next;
					Free (v);
				}
				
				if (msg.Written > 0)
				{
					var bytes = msg.Output;
//					total += bytes.Length;
					//ProgramLog.Debug.Log ("Sending liquid update for section {0},{1} of {2} bytes", x, y, bytes.Length);
					sectionRecords.Add (new SectionRecord { x = x, y = y, bytes = bytes });
					msg.Clear ();
				}
			}
			
			queuedSections.Clear ();
			
			for (int i = 0; i < 255; i++)
			{
				var slot = NetPlay.slots[i];
				
				if (slot.state >= SlotState.PLAYING && slot.Connected)
				{
					var has = slot.tileSection;
					
					foreach (var section in sectionRecords)
					{
						if (has[section.x, section.y])
							slot.Send (section.bytes);
					}
				}
			}
			
//			Total += total;
//			
//			if (total > 0)
//				ProgramLog.Debug.Log ("Liquid updates this frame: {0} bytes, Total: {1:0.0}KB (done: {2}, skipped: {3})", total, Total/1024.0, done, skipped);
			
			sectionRecords.Clear ();
		}
		
		public static void ClearQueue ()
		{
			foreach (int section in queuedSections)
			{
				short x = (short) (section & 0xffff);
				short y = (short) (section >> 16);
				
				short u = sections[x, y];
				sections[x, y] = -1;
				
				while (u != -1)
				{
					var v = u;
					u = updates[u].next;
					Free (v);
				}
			}
			
			queuedSections.Clear ();
			sectionRecords.Clear ();
		}

	}
}

