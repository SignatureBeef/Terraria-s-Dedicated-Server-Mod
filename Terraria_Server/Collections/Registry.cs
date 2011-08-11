using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;
using Terraria_Server.Logging;
using Terraria_Server.Misc;

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
                catch (Exception e)
                {
                    ProgramLog.Log (e, "Error adding element: " + errored.ToString());
                }
            }
            catch (Exception e)
            {
                ProgramLog.Log (e, "Error deserializing: " + filePath);
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

        public void Alter (T subject, String name)
        {
            List<T> values;
            if (typeLookup.TryGetValue(subject.Type, out values))
            {
                foreach (T value in values)
                {
                    if (value.Name == name)
                    {
                        subject.Name = value.Name;
                        subject.aiStyle = value.aiStyle;
                        subject.damage = value.damage;
                        subject.defense = value.defense;
                        subject.life = value.lifeMax;
                        subject.lifeMax = value.lifeMax;
                        subject.scale = value.scale;
                        subject.knockBackResist = value.knockBackResist;
                        subject.slots = value.slots;
                        return;
                    }
                }
            }
            throw new ApplicationException ("Unknown NPC '" + name + "'");
        }
        
        [Obsolete("Not obsolete, but probably needs tweaking to work properly")]
        public void SetDefaults (T obj, int type)
        {
            List<T> values;
            if (typeLookup.TryGetValue(type, out values))
            {
                if (values.Count == 1)
                {
                    obj.CopyFieldsFrom (values[0]);
                }
                else
                    throw new ApplicationException ("Registry.SetDefaults(T, int) called with a non-unique type.");
            }
        }
        
        [Obsolete("Not obsolete, but probably needs tweaking to work properly")]
        public void SetDefaults (T obj, string name)
        {
            T value;
            if (nameLookup.TryGetValue (name, out value))
            {
                obj.CopyFieldsFrom (value);
            }
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
