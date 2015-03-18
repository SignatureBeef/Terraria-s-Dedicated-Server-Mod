using System;
using System.Collections.Generic;

namespace Terraria_Server.Networking
{
	public static class StringCache
	{
		static HashSet<string> test = new HashSet<string> ();

#if FALSE
		// This is half a trie implementation, I bet it's fast
		// but it takes 630KB for 461 entries, which is pointless wastefulness
		
		static List<string> leaves = new List<string> ();
		static List<ushort> trie = new List<ushort> ();
		const int LEVEL_SIZE = 126 - 31;
		
		static StringCache ()
		{
			for (int i = 0; i < 126 - 31; i++)
				trie.Add (0);
			
			var foo = (new System.Threading.Timer (Test, null, 5000, 0));
		}
		
		public static bool Add (byte[] bytes, string str)
		{
			return Add (new ArraySegment<byte> (bytes), str);
		}
		
		public static bool Add (ArraySegment<byte> bytes, string str)
		{
			test.Add (str);
			
			for (int i = bytes.Offset; i < bytes.Offset + bytes.Count; i++)
			{
				if (bytes.Array[i] < 32 || bytes.Array[i] > 126) return false;
			}
			
			int k = 0;
			for (int i = 0; i < bytes.Count; i++)
			{
				byte b = bytes.Array[i + bytes.Offset];
				
				int node = k + b - 31;
				
				if (trie[node] == 0)
				{
					trie[node] = (ushort) (trie.Count / LEVEL_SIZE);
					
					for (int j = 0; j < 126 - 32 + 1; j++)
						trie.Add (0);
				}
				
				k = trie[node] * LEVEL_SIZE;
			}
			
			if (trie[k] == 0)
			{
				trie[k] = (ushort) (leaves.Count + 1);
				leaves.Add (str);
			}
			
			Logging.ProgramLog.Debug.Log ("trie size: {0} ({3}), leaves: {1} ({2}), {4}", trie.Count * 2, leaves.Count, test.Count, trie.Count / LEVEL_SIZE, str);
			
			return true;
		}
		
		public static string Find (ArraySegment<byte> bytes)
		{
			int k = 0;
			for (int i = 0; i < bytes.Count; i++)
			{
				byte b = bytes.Array[i + bytes.Offset];
				
				int node = k + b - 31;
				
				if (trie[node] == 0)
				{
					return null;
				}
				
				k = trie[node] * LEVEL_SIZE;
			}
			
			if (trie[k] > 0)
				return leaves[trie[k] - 1];
			
			return null;
		}
		
		static void Test (object o)
		{
			int c = 0;
			foreach (var str in test)
			{
				if (str == Find (new ArraySegment<byte> (System.Text.Encoding.ASCII.GetBytes (str))))
					c += 1;
			}
			Logging.ProgramLog.Debug.Log ("Correct: {0}", c);
		}
#else
		
		const uint BUCKETS = 97;
		static ushort[] buckets = new ushort [BUCKETS];
		static List<ushort> next = new List<ushort> () { 0 };
		static List<string> leaves = new List<string> () { null };
		
		static StringCache ()
		{
			for (int i = 0; i < 30; i++)
				Add (String.Format ("Terraria{0}", i));
		}
		
		public static bool Add (string s)
		{
			uint hash = 0;
			
			int last = 0;
			for (int i = 0; i < s.Length - 4; i += 4)
			{
				char a = s[i];
				char b = s[i + 1];
				char c = s[i + 2];
				char d = s[i + 3];
				
				if (a > 255 || b > 255 || c > 255 || d > 255) return false;
				
				hash ^= d | (uint)(a << 8) | (uint)(b << 16) | (uint)(c << 24);
				
				last = i + 3;
			}
			
			for (int i = last + 1; i < s.Length; i++)
			{
				char a = s[i];
				
				if (a > 255) return false;
				
				hash ^= (uint) (a << (7 * (i % 3)));
			}
			
			uint B = hash % BUCKETS;
			var bucket = buckets[B];
			
			if (bucket == 0)
			{
				buckets[B] = (ushort) leaves.Count;
				leaves.Add (s);
				next.Add (0);
			}
			else
			{
				ushort prev = 0;
				
//				int i = 0;
				
				while (bucket != 0)
				{
//					i ++;
					if (leaves[bucket] == s)
					{
						return true;
					}
					
					prev = bucket;
					bucket = next[bucket];
				}
				
//				chain = Math.Max (chain, i);
				
				next[prev] = (ushort) leaves.Count;
				leaves.Add (s);
				next.Add (0);
			}
			
//			Logging.ProgramLog.Debug.Log ("buckets: {0}, bytes: {1}, chain: {2}, result: {3}", leaves.Count, leaves.Count * 10 + BUCKETS * 2, chain, Find (new ArraySegment<byte> (System.Text.Encoding.ASCII.GetBytes (s))));
			
			return true;
		}
		
//		static int chain = 0;
		
		static bool Compare (string s, ArraySegment<byte> b)
		{
			if (s.Length != b.Count) return false;
			
			for (int i = 0; i < s.Length; i++)
				if (s[i] != (char)b.Array[b.Offset + i]) return false;
			
			return true;
		}
		
		public static string Find (ArraySegment<byte> s)
		{
			uint hash = 0;
			
			int last = 0;
			for (int i = s.Offset; i < s.Offset + s.Count - 4; i += 4)
			{
				var a = s.Array[i];
				var b = s.Array[i + 1];
				var c = s.Array[i + 2];
				var d = s.Array[i + 3];
				
				hash ^= d | (uint)(a << 8) | (uint)(b << 16) | (uint)(c << 24);
				
				last = i + 3;
			}
			
			for (int i = last + 1; i < s.Offset + s.Count; i++)
			{
				var a = s.Array[i];
				
				hash ^= (uint) (a << (7 * (i % 3)));
			}
			
			var bucket = buckets[hash % BUCKETS];
			
			while (bucket != 0)
			{
				if (Compare (leaves[bucket], s))
					return leaves[bucket];
				
				bucket = next[bucket];
			}
			
			return null;
		}
		
		public static string FindOrMake (ArraySegment<byte> b)
		{
			var s = Find (b);
			
			if (s != null) return s;
			
			return System.Text.Encoding.ASCII.GetString (b.Array, b.Offset, b.Count);
		}
#endif
	}
}

