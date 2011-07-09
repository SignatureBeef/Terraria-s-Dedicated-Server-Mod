using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Terraria_Server.Collections
{
    public class Registry<T> where T : IRegisterableEntity
    {
        protected Dictionary<int, T> typeLookup = new Dictionary<int, T>();
        protected Dictionary<string, T> nameLookup = new Dictionary<string, T>();

        private readonly T defaultValue;

        public Registry(String filePath, T defaultValue)
        {
            this.defaultValue = defaultValue;

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

        public T Default
        {
            get
            {
                return CloneAndInit(defaultValue);
            }
        }

        public T Create(int type)
        {
            T t;
            if (typeLookup.TryGetValue(type, out t))
            {
                return CloneAndInit(t);
            }
            return CloneAndInit(defaultValue);
        }

        public T Create(String name)
        {
            T t;
            if (nameLookup.TryGetValue(name, out t))
            {
                return CloneAndInit(t);
            }
            return CloneAndInit(defaultValue);
        }

        private static T CloneAndInit(T t)
        {
            T cloned = (T) t.Clone();
            if (cloned.Type != 0)
            {
                cloned.Active = true;
            }
            return cloned;
        }
    }
}
