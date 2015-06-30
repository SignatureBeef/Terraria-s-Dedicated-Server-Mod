using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace tdsm.utility
{
    class Program
    {
        static void Main(string[] args)
        {
            Terraria.Main.dedServ = true;
            DumpNPCs(539);
            DumpItems(3596);
        }

        static void DumpNPCs(int maxNPCs)
        {
            var list = new List<NPCInfo>();

            for (var npcId = -65; npcId <= maxNPCs; npcId++)
            {
                var npc = Activator.CreateInstance(typeof(Terraria.NPC)) as Terraria.NPC;
                npc.netDefaults(npcId);

                if (npc.name == String.Empty)
                    continue;
                if (npc.boss)
                {
                    list.Add(new NPCInfo()
                    {
                        Name = npc.name.Trim(),
                        NetId = npc.netID,
                        Id = npc.type,
                        Boss = true
                    });
                }
                else
                    list.Add(new NPCInfo()
                    {
                        Name = npc.name.Trim(),
                        NetId = npc.netID,
                        Id = npc.type
                    });
            }

            var writable = list
                .Distinct()
                .OrderBy(x => x.Name)
                .ToArray();

            //var bf = new BinaryFormatter();
            var bf = new XmlSerializer(typeof(DefinitionFile<NPCInfo>));
            var info = new System.IO.FileInfo("npc.xml");
            if (info.Exists) info.Delete();

            using (var fs = info.OpenWrite())
            {
                bf.Serialize(fs, new DefinitionFile<NPCInfo>()
                {
                    Version = 2,
                    Data = writable
                });
                fs.Flush();
            }

            bf = null;
        }

        static void DumpItems(int maxItems)
        {
            var list = new List<ItemInfo>();

            Terraria.Main.player = new Terraria.Player[1]
            {
                new Terraria.Player()
                {
                    shirtColor = Microsoft.Xna.Framework.Color.Red
                }
            };

            for (var prefix = 0; prefix <= 51; prefix++)
                for (var itemId = -48; itemId <= maxItems; itemId++)
                {
                    var item = Activator.CreateInstance(typeof(Terraria.Item)) as Terraria.Item;
                    item.Prefix(prefix);
                    item.netDefaults(itemId);

                    if (item.name == String.Empty)
                        continue;

                    if (!list.Exists(x => x.Name == item.name))
                        list.Add(new ItemInfo()
                        {
                            Name = item.name.Trim(),
                            NetId = item.netID,
                            Prefix = prefix,
                            Affix = item.AffixName().Trim(),
                            Id = itemId
                        });
                }

            var writable = list
                .OrderBy(x => x.Name)
                .ToArray();

            var bf = new XmlSerializer(typeof(DefinitionFile<ItemInfo>));
            var info = new System.IO.FileInfo("item.xml");
            if (info.Exists) info.Delete();

            using (var fs = info.OpenWrite())
            {
                bf.Serialize(fs, new DefinitionFile<ItemInfo>()
                {
                    Version = 2,
                    Data = writable
                });
                fs.Flush();
            }

            bf = null;
        }
    }

    //[Flags]
    //public enum SpawnTime : byte
    //{
    //    Dawn = 1,
    //    Midday,
    //    Dusk,
    //    Midnight
    //}

    [Serializable]
    public class NPCInfo
    {
        public int Id { get; set; }
        public int NetId { get; set; }
        public string Name { get; set; }

        public bool Boss { get; set; }
        //public SpawnTime? SpawnTime { get; set; }
        //Maybe a spawn biome? (to test)
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
