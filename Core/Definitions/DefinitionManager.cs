using System;
using System.IO;
using System.Linq;
using OTA;
using OTA.Logging;

namespace TDSM.Core.Definitions
{
    public static class DefinitionManager
    {
        private static DefinitionFile<NPCInfo> _npc;
        private static DefinitionFile<ItemInfo> _item;

        internal static int NPCVersion
        {
            get
            {
                if (_npc == null) return 0;
                return _npc.Version;
            }
        }

        internal static int ItemVersion
        {
            get
            {
                if (_item == null) return 0;
                return _item.Version;
            }
        }

        [TDSMComponent(ComponentEvent.Initialise)]
        public static void Initialise(Entry plugin)
        {
            if (!Initialise())
                ProgramLog.Log("Failed to initialise definitions.");
        }

        public static bool Initialise()
        {
            var npc = new FileInfo(Path.Combine(Globals.DataPath, "npc.json"));
            var item = new FileInfo(Path.Combine(Globals.DataPath, "item.json"));

            //Always keep these in the data path, as we may use the update mechanism to update these definitions periodically
            var intNPC = LoadDefinition<NPCInfo>("npc.json");
            var intItem = LoadDefinition<ItemInfo>("item.json");

            //If they already exist we must compare with the internal versions. Use the latest.
            if (npc.Exists)
            {
                var current = LoadDefinition<NPCInfo>(npc);
                if (current.Version < intNPC.Version)
                {
                    //Save new
                    Save<NPCInfo>(npc.FullName, intNPC);
                    ProgramLog.Log("NPC definitions were updated to v{0}", intNPC.Version);
                    _npc = intNPC;
                }
                else _npc = current;
            }
            else
            {
                Save<NPCInfo>(npc.FullName, intNPC);
                _npc = intNPC;
            }
            if (item.Exists)
            {
                var current = LoadDefinition<ItemInfo>(item);
                if (current.Version < intItem.Version)
                {
                    //Save new
                    Save<ItemInfo>(item.FullName, intItem);
                    ProgramLog.Log("Item definitions were updated to v{0}", intItem.Version);
                    _item = intItem;
                }
                else _item = current;
            }
            else
            {
                Save<ItemInfo>(item.FullName, intItem);
                _item = intItem;
            }

            return true;
        }

        public static NPCInfo[] FindNPC(int type)
        {
            return _npc.Data
                .Where(x => x.Id == type)
                .ToArray();
        }

        public static NPCInfo[] FindNPC(string name)
        {
            var lowered = name.ToLower();

            var singular = _npc.Data
                .Where(x => x.Name.ToLower() == lowered)
                .ToArray();

            if (singular.Length == 1) return singular;

            return _npc.Data
                .Where(x => x.Name.ToLower().StartsWith(name))
                .ToArray();
        }

        public static ItemInfo[] FindItem(string name)
        {
            var lowered = name.ToLower();

            var singular = _item.Data
                .Where(x => x.Name.ToLower() == lowered)
                .ToArray();
            if (singular.Length == 1) return singular;

            return _item.Data
                .Where(x => x.Name.ToLower().StartsWith(name))
                .ToArray();
        }

        public static ItemInfo[] FindItem(int id)
        {
            return _item.Data
                .Where(x => x.Id == id)
                .ToArray();
        }

        #region "IO"

        static void Save<T>(string filename, DefinitionFile<T> definition) where T : class
        {
            if (System.IO.File.Exists(filename)) System.IO.File.Delete(filename);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(definition);
            System.IO.File.WriteAllText(filename, json);
        }

        static DefinitionFile<T> LoadDefinition<T>(string internalFile) where T : class
        {
            using (var srm = GetInternalDefinition(internalFile))
                return LoadDefinition<T>(srm);
        }

        static DefinitionFile<T> LoadDefinition<T>(FileInfo local) where T : class
        {
            using (var srm = local.OpenRead())
                return LoadDefinition<T>(srm);
        }

        static DefinitionFile<T> LoadDefinition<T>(Stream input) where T : class
        {
            using (var st = new StreamReader(input))
            {
                var json = st.ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<DefinitionFile<T>>(json);
            }
        }

        static Stream GetInternalDefinition(string file)
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("TDSM.Core.Definitions." + file);
        }

        #endregion
    }

    [Serializable]
    public class NPCInfo
    {
        public int Id { get; set; }

        public int NetId { get; set; }

        public string Name { get; set; }

        public bool? Boss { get; set; }
    }

    [Serializable]
    public class ItemInfo
    {
        public int Id { get; set; }

        public int NetId { get; set; }

        public string Affix { get; set; }

        public int Prefix { get; set; }

        public string Name { get; set; }

        public int MaxStack { get; set; }
    }

    [Serializable]
    public class DefinitionFile<T> where T : class
    {
        public int Version { get; set; }

        public T[] Data { get; set; }
    }
}
