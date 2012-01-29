using System;
using System.Collections.Generic;
using Terraria_Server.Misc;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;
using Terraria_Server.Logging;

namespace Terraria_Server.Collections
{
    public class ItemRegistry : Registry<Item>
    {
        private const string ITEMS_BY_NAME = "ItemsByName.xml";
        private const string ITEM_FILE = "Items.xml";

        public void Load () 
        { 
            base.Load (ITEM_FILE);
            /*
             * We need to load additional items into the name lookup and only the name
             * lookup dictionary. This is at least until the item list can be fixed so
             * each item has a unique type or other identifier.
             */
            StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(Registries.DEFINITIONS + ITEMS_BY_NAME));
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
                ProgramLog.Log (e);
            }
        }
        
        public override Item Create(int type, int stack = 1)
        {
            Item item = CloneAndInit(base.Create(type), stack);
            return item;
        }

        public Item Create(string name, int stack = 1)
        {
            return CloneAndInit(base.Create(name), stack);
        }

        private static Item CloneAndInit(Item item, int stack)
        {
            Item cloned = (Item) item.Clone();
            if (cloned.Active)
            {
                cloned.Stack = stack;
            }

            //Specialized handling for familiar armor until a better method is created.
            //switch(item.Type)
			//{
			//    case 269:
			//        item.Color = Main.players[Main.myPlayer].shirtColor;
			//        break;
			//    case 270:
			//        item.Color = Main.players[Main.myPlayer].pantsColor;
			//        break;
			//    case 271:
			//        item.Color = Main.players[Main.myPlayer].hairColor;
			//        break;
			//}
            return cloned;
        }
    }
}
