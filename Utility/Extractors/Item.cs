using System;
using System.Collections.Generic;
using System.Linq;
using TDSM.Utility.Info;

namespace TDSM.Utility.Extractors
{
    class Item : Extractor
    {
        const int MaxItems = 3596;

        const int MaxPrefix = 51;
        const int MinItems = -48;

        public override void Extract()
        {
            var list = new List<ItemInfo>();

            Terraria.Main.player = new Terraria.Player[1]
            {
                new Terraria.Player()
                {
                    shirtColor = Microsoft.Xna.Framework.Color.Red
                }
            };

            var total = ((MinItems * -1) + MaxItems + 1) * (MaxPrefix + 1);
            Min(0);
            Max(total);

            for (var prefix = 0; prefix <= MaxPrefix; prefix++)
            {
                for (var itemId = MinItems; itemId <= MaxItems; itemId++)
                {
                    var item = Activator.CreateInstance(typeof(Terraria.Item)) as Terraria.Item;
                    item.netDefaults(itemId);
                    item.Prefix(prefix);

                    if (item.name != String.Empty && !list.Exists(x => x.Name == item.name))
                    {
                        list.Add(new ItemInfo()
                        {
                            Name = item.name.Trim(),
                            NetId = item.netID,
                            Prefix = prefix,
                            Affix = item.AffixName().Trim(),
                            Id = itemId,
                            MaxStack = item.maxStack
                        });
                    }

                    Increment();
                }
            }

            var writable = list
                .OrderBy(x => x.Name)
                .ToArray();

            Write(new DefinitionFile<ItemInfo>()
            {
                Version = 3,
                Data = writable
            }, "item.xml");
        }
    }
}
