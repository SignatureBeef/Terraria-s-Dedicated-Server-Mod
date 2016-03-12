using System;

namespace TDSM.Utility.Info
{
    [Serializable]
    public class DefinitionFile<T> where T : class
    {
        public int Version { get; set; }

        public T[] Data { get; set; }
    }
}
