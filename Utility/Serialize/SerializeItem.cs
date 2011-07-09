using System;
using System.IO;
using Terraria_Server;
using System.Xml;

namespace Terraria_Utilities.Serialize
{
    public class SerializeItem
    {
        public static void Start()
        {
            String[] ignoreFields = new String[] {"Active", "Stack", };
            Serializer.Serialize(typeof(Item), ignoreFields);
        }
    }
}
