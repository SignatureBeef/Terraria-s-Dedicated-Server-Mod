using System;
using System.Collections;
using System.Collections.Generic;

namespace Terraria_Server.Misc
{
	class ArrayDeque<T> : IEnumerable<T>, IEnumerable
	{
		internal protected T[] array;
		internal protected int head;
		internal protected int count;
		
		public ArrayDeque (int size = 16)
		{
			array = new T [size];
		}
		
		public int Count { get { return count; } }
		
		public int Capacity { get { return array.Length; } }
		
		public T this [int index]
		{
			get
			{
				if (index < 0 || index > count) throw new IndexOutOfRangeException ("index");
				return array[(head + index) % array.Length];
			}
			
			set
			{
				if (index < 0 || index > count) throw new IndexOutOfRangeException ("index");
				array[(head + index) % array.Length] = value;
			}
		}
		
		void Grow ()
		{
			int len = array.Length;
			
			var a = new T [len * 3 / 2 + 16];
			
			for (int i = 0; i < count; i++)
			{
				int k = (head + i) % len;
				a[i] = array[k];
			}
			
			head = 0;
			
			array = a;
		}
		
		public void PushBack (T item)
		{
			if (count == array.Length) Grow ();
			
			int tail = (head + count) % array.Length;
			array[tail] = item;
			count += 1;
		}
		
		public void PushFront (T item)
		{
			if (count == array.Length) Grow ();
			
			head -= 1;
			if (head < 0) head += array.Length;
			
			array[head] = item;
			count += 1;
		}
		
		public T PopFront ()
		{
			if (count == 0) throw new InvalidOperationException ("Deque is empty.");
			
			return PopFrontInternal ();
		}
		
		public T PopFront (T defaultValue)
		{
			if (count == 0) return defaultValue;
			
			return PopFrontInternal ();
		}
		
		public T PopBack ()
		{
			if (count == 0) throw new InvalidOperationException ("Deque is empty.");
			
			return PopBackInternal ();
		}
		
		public T PopBack (T defaultValue)
		{
			if (count == 0) return defaultValue;
			
			return PopBackInternal ();
		}
		
		public T PeekFront ()
		{
			if (count == 0) throw new InvalidOperationException ("Deque is empty.");
			
			return array[head];
		}
		
		public T PeekFront (T defaultValue)
		{
			if (count == 0) return defaultValue;
			
			return array[head];
		}

		public T PeekBack ()
		{
			if (count == 0) throw new InvalidOperationException ("Deque is empty.");
			
			return array[(head + count - 1) % array.Length];
		}
		
		public T PeekBack (T defaultValue)
		{
			if (count == 0) return defaultValue;
			
			return array[(head + count - 1) % array.Length];
		}
		
		public void ReplaceBack (T item)
		{
			if (count == 0) throw new InvalidOperationException ("Deque is empty.");
			
			array[(head + count - 1) % array.Length] = item;
		}
		
		public void ReplaceFront (T item)
		{
			if (count == 0) throw new InvalidOperationException ("Deque is empty.");
			
			array[head] = item;
		}
		
		public void Clear ()
		{
			Array.Clear (array, 0, array.Length);
			
			count = 0;
			head = 0;
		}
		
		T PopFrontInternal ()
		{
			var item = array[head];
			array[head] = default(T);
			head = (head + 1) % array.Length;
			count -= 1;
			
			return item;
		}
		
		T PopBackInternal ()
		{
			var tail = (head + count - 1) % array.Length;
			var item = array[tail];
			array[tail] = default(T);
			count -= 1;
			
			return item;
		}
		
		public DequeEnumerator GetEnumerator ()
		{
			return new DequeEnumerator() { deque = this, index = -1 };
		}
		
		IEnumerator<T> IEnumerable<T>.GetEnumerator ()
		{
			return new DequeEnumerator() { deque = this, index = -1 };
		}
		
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return new DequeEnumerator() { deque = this, index = -1 };
		}
		
		public struct DequeEnumerator : IDisposable, IEnumerator<T>, IEnumerator
		{
			internal ArrayDeque<T> deque;
			internal int           index;
			
			public T Current
			{
				get { return deque[index]; }
			}
			
			object IEnumerator.Current
			{
				get { return deque[index]; }
			}
			
			public bool MoveNext ()
			{
				return (++index) < deque.count;
			}
			
			public void Reset ()
			{
				index = -1;
			}
			
			public void Dispose ()
			{
			}
		}
	}
}