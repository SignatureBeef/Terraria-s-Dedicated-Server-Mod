using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Terraria_Server.Messages
{
	public abstract class SpammableMessage<T, V> : SlotMessageHandler
	{
		public Dictionary<T, V> Register = new Dictionary<T, V>();

		public abstract IEnumerable<T> GetRemovable { get; }

		public void AddOrUpdate(T key, V val)
		{
			lock (Register)
			{
				if (Register.ContainsKey(key))
					Register.Remove(key);

				Register.Add(key, val);
			}
		}

		public void Purge()
		{
			lock (Register)
			{
				var removable = (T[])GetRemovable.ToArray().Clone();
				foreach (var id in removable)
					Register.Remove(id);
			}
		}
	}
}
