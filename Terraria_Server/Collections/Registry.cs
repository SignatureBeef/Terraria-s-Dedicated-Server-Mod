using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using Terraria_Server.Logging;
using Terraria_Server.Misc;
using Terraria_Server.Language;
using System.Linq;

namespace Terraria_Server.Collections
{
    public class Registry<T> where T : IRegisterableEntity
    {
        protected Dictionary<Int32, List<T>> typeLookup = new Dictionary<Int32, List<T>>();
        protected Dictionary<String, T> nameLookup = new Dictionary<String, T>();

        private readonly T defaultValue;

        public Dictionary<Int32, List<T>> TypesById
        {
            get
            {
                return typeLookup;
            }
        }

        public Dictionary<String, T> TypesByName
        {
            get
            {
                return nameLookup;
            }
        }

        public Registry ()
        {
            this.defaultValue = Activator.CreateInstance<T>();
        }
        
		public void Load (string filePath)
		{
			var document = new XmlDocument ();
			document.Load (Assembly.GetExecutingAssembly().GetManifestResourceStream(Registries.DEFINITIONS + filePath));
			var nodes = document.SelectNodes ("/*/*");
			var ser = new XmlSerializer (typeof(T));
			
			foreach (XmlNode node in nodes)
			{
				try
				{
					var rdr = new XmlNodeReader (node);
					var t = (T) ser.Deserialize (rdr);
					
					//ProgramLog.Debug.Log ("Created entity {0}, {1}", t.Type, t.Name);
					
					t.Name = String.Intern (t.Name);
					if (t.NetID == 0) t.NetID = (short)t.Type;
					//Networking.StringCache.Add (System.Text.Encoding.ASCII.GetBytes (t.Name), t.Name);
					Networking.StringCache.Add(t.Name);

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
				catch (Exception e)
				{
					ProgramLog.Log (e, "Error adding element");
					ProgramLog.Error.Log ("Element was:\n" + node.ToString());
				}				
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

        public void SetDefaults (T obj, int type)
        {
            List<T> values;
            if (typeLookup.TryGetValue(type, out values))
            {
                if (values.Count > 0)
                {
                    obj.CopyFieldsFrom (values[0]);
                    return;
                }
                else
                    throw new ApplicationException ("Registry.SetDefaults(T, int): type " + type + " not found.");
            }
            throw new ApplicationException ("Registry.SetDefaults(T, int): type " + type + " not found.");
        }
        
        public void SetDefaults (T obj, string name)
        {
            T value;
            if (nameLookup.TryGetValue (name, out value))
            {
                obj.CopyFieldsFrom (value);
                return;
            }
            throw new ApplicationException ("Registry.SetDefaults(T, string): type '" + name + "' not found.");
        }

        public T Create(string name)
        {
            T t;
            if (nameLookup.TryGetValue(name, out t))
            {
                return CloneAndInit(t);
            }
            return CloneAndInit(defaultValue);
        }
		
        public T FindClass(string nameOrId)
        {
			int id = 0;
			var parsed = Int32.TryParse(nameOrId, out id);
            List<T> values;

            foreach (int type in typeLookup.Keys)
            {
                if (typeLookup.TryGetValue(type, out values))
                {
                    foreach (T value in values)
                    {
						var trimmed = value.Name.ToLower().Replace(" ", "").Trim() == nameOrId.ToLower().Replace(" ", "").Trim();

						if(trimmed || parsed && value.Type == id)
							return value;
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
        
		public T GetTemplate (int type)
		{
			List<T> values;
			if (typeLookup.TryGetValue (type, out values))
			{
				return values[0];
			}
			return default(T);
		}
    }
}
