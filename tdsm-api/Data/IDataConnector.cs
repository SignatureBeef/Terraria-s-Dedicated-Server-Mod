using System;

namespace tdsm.api.Data
{
    public interface IDataConnector
    {
        QueryBuilder GetBuilder();

        void Open();
    }

    public abstract class QueryBuilder : IDisposable
    {
        void IDisposable.Dispose()
        {

        }
    }

    public static class Storage
    {
        private static readonly object _sync = new object();
        private static IDataConnector _connector;

        public static bool IsAvailable
        {
            get
            { return _connector != null; }
        }

        public static void SetConnector(IDataConnector connector, bool throwWhenSet = true)
        {
            lock (_sync)
            {
                if (_connector != null && throwWhenSet)
                {
                    throw new InvalidOperationException(String.Format("Attempted to load '{0}' when a '{1}' was already loaded", connector.ToString(), _connector.ToString()));
                }
                _connector = connector;
            }
        }
    }
}

