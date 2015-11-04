using System;
using System.Collections.Generic;
using System.Linq;

namespace TDSM.Core.Misc
{
    public class CyclicQueue<T>
    {
        private T[] _source;
        private int _index;

        public int Count
        {
            get
            {
                return _source.Length;
            }
        }

        public CyclicQueue(T[] source)
        {
            this._source = source;
            this._index = -1;
        }

        public CyclicQueue(IEnumerable<T> source)
        {
            this._source = source.ToArray();
            this._index = -1;
        }

        public T Next()
        {
            var ix = System.Threading.Interlocked.Increment(ref _index);
            System.Threading.Interlocked.CompareExchange(ref _index, -1, _source.Length - 1);
            return this._source[ix];
        }
    }
}

