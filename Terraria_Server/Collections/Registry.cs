using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;

namespace Terraria_Server.Collections
{
    public class Registry<T> where T : IRegisterableEntity
    {
        protected Dictionary<int, List<T>> typeLookup = new Dictionary<int, List<T>>();
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

                        if (typeLookup.ContainsKey(t.Type))
                        {
                            List<T> values;
                            if (typeLookup.TryGetValue(t.Type, out values))
                            {
                                values.Add(t);
                            }
                        }
                        else
                        {
                            List<T> values = new List<T>();
                            values.Add(t);
                            typeLookup.Add(t.Type, values);
                        }

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

        public virtual T Create(int type, int index = 0)
        {
            List<T> values;
            if (typeLookup.TryGetValue(type, out values))
            {
                if (values.Count > 0)
                {
                    return CloneAndInit(values[index]);
                }
            }
            return CloneAndInit(defaultValue);
        }

        public T Alter(T coneable, String name)
        {
            List<T> values;
            if (typeLookup.TryGetValue(coneable.Type, out values))
            {
                foreach (T value in values)
                {
                    if (value.Name == name)
                    {
                        //T newClone = (T)coneable.Clone();
                        T cloned = CloneAndInit(value);
                        coneable.Name = cloned.Name;
                        coneable.aiStyle = cloned.aiStyle;
                        coneable.damage = cloned.damage;
                        coneable.defense = cloned.defense;
                        coneable.life = cloned.life;
                        coneable.lifeMax = cloned.lifeMax;
                        coneable.scale = cloned.scale;
                        coneable.knockBackResist = cloned.knockBackResist;
                        NPC.npcSlots = cloned.slots;

                        return coneable;
                    }
                }
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


        public T FindClass(String name)
        {
            List<T> values;
            foreach (int type in typeLookup.Keys)
            {
                if (typeLookup.TryGetValue(type, out values))
                {
                    foreach (T value in values)
                    {
                        if (value.Name.ToLower().Replace(" ", "").Trim() == name.ToLower().Replace(" ", "").Trim()) //Exact :3
                        {
                            //CloneAndInit(values[i]);
                            return value;
                        }
                    }
                }
            }
            return Default;
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
