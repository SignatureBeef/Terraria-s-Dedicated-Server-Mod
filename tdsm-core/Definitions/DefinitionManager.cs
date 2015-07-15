using System;
using System.IO;
using System.Linq;
using TDSM.API;
using TDSM.API.Logging;

namespace TDSM.Core.Definitions
{
    public static class DefinitionManager
    {
        private static DefinitionFile<NPCInfo> _npc;
        private static DefinitionFile<ItemInfo> _item;

        internal static int NPCVersion
        {
            get
            { return _npc.Version; }
        }

        internal static int ItemVersion
        {
            get
            { return _item.Version; }
        }

        public static bool Initialise()
        {
            var npc = new FileInfo(Path.Combine(Globals.DataPath, "npc.xml"));
            var item = new FileInfo(Path.Combine(Globals.DataPath, "item.xml"));

            //Always keep these in the data path, as we may use the update mechanism to update these definitions periodically
            var intNPC = LoadDefinition<NPCInfo>("npc.xml");
            var intItem = LoadDefinition<ItemInfo>("item.xml");

            //If they already exist we must compare with the internal versions. Use the latest.
            if (npc.Exists)
            {
                var current = LoadDefinition<NPCInfo>(npc);
                if (current.Version < intNPC.Version)
                {
                    //Save new
                    Save<NPCInfo>(npc, intNPC);
                    ProgramLog.Log("NPC definitions were updated to v{0}", intNPC.Version);
                    _npc = intNPC;
                }
                else _npc = current;
            }
            else
            {
                Save<NPCInfo>(npc, intNPC);
                _npc = intNPC;
            }
            if (item.Exists)
            {
                var current = LoadDefinition<ItemInfo>(item);
                if (current.Version < intItem.Version)
                {
                    //Save new
                    Save<ItemInfo>(item, intItem);
                    ProgramLog.Log("Item definitions were updated to v{0}", intItem.Version);
                    _item = intItem;
                }
                else _item = current;
            }
            else
            {
                Save<ItemInfo>(item, intItem);
                _item = intItem;
            }

            return true;
        }

        public static NPCInfo[] FindNPC(string name)
        {
            var lowered = name.ToLower();

            var singular = _npc.Data
                .Where(x => x.Name.ToLower() == lowered)
                .ToArray();
            if (singular.Length == 1) return new NPCInfo[] { singular[0] };

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
            if (singular.Length == 1) return new ItemInfo[] { singular[0] };

            return _item.Data
                .Where(x => x.Name.ToLower().StartsWith(name))
                .ToArray();
        }

        #region "IO"
        static void Save<T>(FileInfo info, DefinitionFile<T> definition) where T : class
        {
            if (info.Exists) info.Delete();
            var bf = new System.Xml.Serialization.XmlSerializer(typeof(DefinitionFile<T>));

            using (var fs = info.OpenWrite())
            {
                bf.Serialize(fs, definition);
                fs.Flush();
            }

            bf = null;
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
            DefinitionFile<T> obj = null;
            var bf = new System.Xml.Serialization.XmlSerializer(typeof(DefinitionFile<T>));
            obj = (DefinitionFile<T>)bf.Deserialize(input);
            bf = null;
            return obj;
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
    }

    [Serializable]
    public class DefinitionFile<T> where T : class
    {
        public int Version { get; set; }
        public T[] Data { get; set; }
    }
}
