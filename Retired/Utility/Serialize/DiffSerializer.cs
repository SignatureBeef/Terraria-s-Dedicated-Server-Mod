using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.Runtime.Serialization;
using System.Reflection;

namespace Terraria_Utilities.Serialize
{
    /**
     * This serializer should be used only to write differences between the default object 
     * instantiation and individual object instances to a serialized form. This cannot be
     * used to read objects as it is beyond the scope of why it was created.
     **/
    public class DiffSerializer : XmlObjectSerializer
    {
        private Type baseType;
        private object baseObject;
        private string[] ignoreFields;
        private bool skipName;

        public DiffSerializer(Type baseType, string[] ignoreFields = null, bool skipName = false)
        {
            this.baseType = baseType;
            baseObject = Activator.CreateInstance(baseType);
            this.ignoreFields = ignoreFields;
            this.skipName = skipName;
        }

        public override bool IsStartObject(XmlDictionaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName)
        {
            throw new NotImplementedException();
        }

        public override void WriteEndObject(XmlDictionaryWriter writer)
        {
            if (!skipName)
            {
				writer.WriteString("\n\t");
                writer.WriteEndElement();
            }
        }

        private bool IsIgnored(MemberInfo member)
        {
            if (ignoreFields != null)
            {
                foreach (string ignoreField in ignoreFields)
                {
                    if (member.Name.Equals(ignoreField))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void Process(XmlDictionaryWriter writer, object nodeValue, object baseValue, string name, Type type)
        {
            if (nodeValue != null && !nodeValue.Equals(baseValue))
            {
				if (!name.Equals("value__"))
				{
					writer.WriteString("\n\t\t");
					writer.WriteStartElement(name);
				}
				if (type.Equals(typeof(bool)))
				{
					if ((bool)nodeValue)
					{
						writer.WriteString("true");
					}
					else
					{
						writer.WriteString("false");
					}
                }
                else if (type.IsPrimitive || type.Equals(typeof(String)))
                {
                    writer.WriteString(nodeValue.ToString());
                }
				else if(!type.IsArray)
				{
					DiffSerializer innerSerializer = new DiffSerializer(type, null, true);
					innerSerializer.WriteObject(writer, nodeValue);
				}
				if (!name.Equals("value__"))
				{
					writer.WriteEndElement();
				}
            }
        }

        public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
        {
            foreach (PropertyInfo property in baseType.GetProperties())
            {
                if (IsIgnored(property))
                {
                    continue;
                }

                Process(writer, property.GetValue(graph, null),  property.GetValue(baseObject, null), property.Name, property.PropertyType);
            }

            foreach(FieldInfo field in baseType.GetFields())
            {
                if (IsIgnored(field))
                {
                    continue;
                }

                Process(writer, field.GetValue(graph), field.GetValue(baseObject), field.Name, field.FieldType);
            }
        }

        public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
        {
            if (!graph.GetType().Equals(baseType))
            {
                throw new Exception("Object is not of the correct type.");
            }

            if (!skipName)
            {
				writer.WriteString("\n\t");
                writer.WriteStartElement(baseType.Name);
            }
        }
    }
}
