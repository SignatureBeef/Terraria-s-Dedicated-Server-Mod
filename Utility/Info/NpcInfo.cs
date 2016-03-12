using System;

namespace TDSM.Utility.Info
{
    [Serializable]
    public class NpcInfo
    {
        public int Id { get; set; }

        public int NetId { get; set; }

        public string Name { get; set; }

        public bool Boss { get; set; }
        //public SpawnTime? SpawnTime { get; set; }
        //Maybe a spawn biome? (to test)
    }
}
