using System;
using System.Collections.Generic;
using System.Linq;
using TDSM.Utility.Info;

namespace TDSM.Utility.Extractors
{
    class Npc : Extractor
    {
        const int MaxNpcs = 539;
        const int MinNpcs = -65;

        public override void Extract()
        {
            var list = new List<NpcInfo>();

            var total = (MinNpcs * -1) + MaxNpcs + 1;
            Min(0);
            Max(total);

            for (var npcId = MinNpcs; npcId <= MaxNpcs; npcId++)
            {
                var npc = Activator.CreateInstance(typeof(Terraria.NPC)) as Terraria.NPC;
                npc.netDefaults(npcId);

                if (npc.name != String.Empty)
                {
                    if (npc.boss)
                    {
                        list.Add(new NpcInfo()
                        {
                            Name = npc.name.Trim(),
                            NetId = npc.netID,
                            Id = npc.type,
                            Boss = true
                        });
                    }
                    else {
                        list.Add(new NpcInfo()
                        {
                            Name = npc.name.Trim(),
                            NetId = npc.netID,
                            Id = npc.type
                        });
                    }
                }

                Increment();
            }

            var writable = list
                .Distinct()
                .OrderBy(x => x.Name)
                .ToArray();

            Write(new DefinitionFile<NpcInfo>()
            {
                Version = 2,
                Data = writable
            }, "npc.xml");
        }
    }
}
