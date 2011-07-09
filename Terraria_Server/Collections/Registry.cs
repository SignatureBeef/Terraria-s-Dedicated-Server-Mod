using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Terraria_Server.Collections
{
    public class Registry<T> where T : IRegisterableEntity, ICloneable
    {
        private Dictionary<int, T> typeLookup = new Dictionary<int, T>();
        private Dictionary<string, T> nameLookup = new Dictionary<string, T>();

        public Registry(String filePath, T defaultValue)
        {
            Default = defaultValue;

            StreamReader reader = new StreamReader(filePath);
            XmlSerializer serializer = new XmlSerializer(typeof(T[]));
            try
            {
                T[] deserialized  = (T[]) serializer.Deserialize(reader);
                foreach (T t in deserialized)
                {
                    typeLookup.Add(t.Type, t);
                    nameLookup.Add(t.Name, t);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public T Default { get; private set; }

        public T this[int type]
        {
            get
            {
                T t;
                if (typeLookup.TryGetValue(type, out t))
                {
                    return t;
                }
                return Default;
            }
        }

        public T this[String name]
        {
            get
            {
                T t;
                if (nameLookup.TryGetValue(name, out t))
                {
                    return t;
                }
                return Default;
            }
        }
    }
}
