using System;
using System.Collections.Generic;
using Terraria_Server.Misc;
using System.IO;
using System.Xml.Serialization;

namespace Terraria_Server.Collections
{
    public class ItemRegistry : Registry<Item>
    {
        private const String ITEMS_BY_NAME = "ItemsByName.xml";

        public ItemRegistry() : base("Items.xml", new Item()) 
        { 
            /**
             * We need to load additional items into the name lookup and only the name
             * lookup dictionary. This is at least until the item list can be fixed so
             * each item has a unique type or other identifier.
             */
            StreamReader reader = new StreamReader(ITEMS_BY_NAME);
            XmlSerializer serializer = new XmlSerializer(typeof(Item[]));
            try
            {
                Item[] deserialized = (Item[])serializer.Deserialize(reader);
                foreach (Item item in deserialized)
                {
                    nameLookup.Add(item.Name, item);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public Item Create(int type, int stack = 1)
        {
            return CloneAndInit(base.Create(type), stack);
        }

        public Item Create(String name, int stack = 1)
        {
            return CloneAndInit(base.Create(name), stack);
        }

        private static Item CloneAndInit(Item item, int stack)
        {
            Item cloned = (Item) item.Clone();
            if (cloned.Active)
            {
                item.Stack = stack;
            }
            return cloned;
        }
    }
}
