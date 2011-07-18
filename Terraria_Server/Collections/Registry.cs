using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;

namespace Terraria_Server.Collections
{
    public class Registry<T> where T : IRegisterableEntity
    {
        protected Dictionary<int, T> typeLookup = new Dictionary<int, T>();
        protected Dictionary<string, T> nameLookup = new Dictionary<string, T>();

        protected String DEFINITIONS = "Terraria_Server.Definitions.";

        private readonly T defaultValue;

        public Registry(String filePath)
        {
            this.defaultValue = Activator.CreateInstance<T>();
            StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(DEFINITIONS + filePath));
            XmlSerializer serializer = new XmlSerializer(typeof(T[]));
            T[] deserialized;
            try
            {
                deserialized  = (T[]) serializer.Deserialize(reader);
                T errored = deserialized[0];
                try
                {
                    foreach (T t in deserialized)
                    {
                        errored = t;
                        typeLookup.Add(t.Type, t);
                        if (!nameLookup.ContainsKey(t.Name))
                        {
                            nameLookup.Add(t.Name, t);
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Error adding element: " + errored.ToString());
                }
            }
            catch(Exception)
            {
                Console.WriteLine("Error deserializing: " + filePath);
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
            if (t.Inherits != 0)
            {
                cloned.Type = t.Inherits;
            }
            if (cloned.Type != 0)
            {
                cloned.Active = true;
            }
            return cloned;
        }
    }
}
