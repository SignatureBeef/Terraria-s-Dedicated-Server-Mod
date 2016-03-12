using System;

namespace TDSM.Utility.Info
{
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
}
