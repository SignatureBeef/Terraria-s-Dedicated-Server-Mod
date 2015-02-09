using System.Linq;

namespace tdsm.core.Misc
{
    /// <summary>
    /// Low use data storage
    /// </summary>
    public class DataRegister
    {
        private string _path;
        private string[] _data;

        const char PrefixKey = '=';

        public DataRegister(string path, bool autoLoad = true)
        {
            _path = path;
            _data = new string[0];

            if (autoLoad) Load();
        }

        public void Load()
        {
            lock (_data)
            {
                if (System.IO.File.Exists(_path))
                    _data = System.IO.File.ReadAllLines(_path)
                        .Select(x => x.ToLower().Trim())
                        .Distinct()
                        .ToArray();
                else System.IO.File.WriteAllText(_path, System.String.Empty);
            }
        }

        public bool Save()
        {
            try
            {
                lock (_data)
                {
                    System.IO.File.WriteAllLines(_path,
                        _data.Distinct().ToArray()
                    );
                }
                return true;
            }
            catch (System.Exception e)
            {
                Logging.ProgramLog.Log(e, "Failure saving data list.");
                return false;
            }
        }

        public bool Add(string item, bool autoSave = true)
        {
            lock (_data)
            {
                System.Array.Resize(ref _data, _data.Length + 1);
                _data[_data.Length - 1] = item.ToLower().Trim();
            }

            if (autoSave) return Save();
            return true;
        }

        public bool Add(string key, string value, bool autoSave = true)
        {
            lock (_data)
            {
                System.Array.Resize(ref _data, _data.Length + 1);
                _data[_data.Length - 1] = key.ToLower().Trim() + PrefixKey + value;
            }

            if (autoSave) return Save();
            return true;
        }

        public bool Remove(string item, bool byKey = false, bool autoSave = true)
        {
            var cleaned = item.ToLower().Trim();
            lock (_data)
            {
                if (byKey)
                    _data = _data.Where(x => !x.StartsWith(cleaned + PrefixKey)).ToArray();
                else
                    _data = _data.Where(x => x != cleaned).ToArray();
            }

            if (autoSave) return Save();
            return true;
        }

        public bool Contains(string item, bool byKey = false)
        {
            var cleaned = item.ToLower().Trim();
            lock (_data)
            {
                if (byKey)
                    return _data.Where(x => x.StartsWith(cleaned + PrefixKey)).Count() > 0;
                else
                    return _data.Where(x => x == item).Count() > 0;
            }
        }

        public bool Contains(string key, string value)
        {
            var cleaned = key.ToLower().Trim();
            lock (_data)
            {
                return _data.Where(x => x == (cleaned + PrefixKey + value)).Count() > 0;
            }
        }
    }
}
