using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Terraria_Server.Messages
{
	public struct Record<T, V>
	{
		public T Key;
		public V Val;
	}

	public abstract class SpammableMessage<T, V> : SlotMessageHandler
	{
		//public Dictionary<T, V> Register = new Dictionary<T, V>();
		public List<Record<T, V>> Register = new List<Record<T, V>>();

		/// <summary>
		/// This should return the items to be removed when Purge is called.
		/// </summary>
		public abstract IEnumerable<Record<T,V>> GetRemovable { get; }

		/// <summary>
		/// Adds a new record to me monitored
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		public void Add(T key, V val)
		{
			lock (Register)
			{
				//if (Register.ContainsKey(key))
				//    Register.Remove(key);

				//Register.Add(key, val);
				Register.Add(new Record<T, V>()
				{
					Key = key,
					Val = val
				});
			}
		}

		/// <summary>
		/// Purges out data based on GetRemovables as an array of records
		/// </summary>
		public void Purge()
		{
			lock (Register)
			{
				var removable = (Record<T, V>[])GetRemovable.ToArray().Clone();
				foreach (var id in removable)
					Register.Remove(id);
			}
		}
	}

	public static class EnumerableExtensions
	{
		public static bool GetAllResults<T,V>(this IEnumerable<Record<T,V>> list, T key, out V[] val)
		{
			val = (from x in list where x.Key.Equals(key) select x.Val).ToArray();
			return val.Count() > 0;
		}
	}
}
