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
        private String[] ignoreFields;
        private bool skipName;

        public DiffSerializer(Type baseType, String[] ignoreFields, bool skipName = false)
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
                writer.WriteEndElement();
            }
        }

        public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
        {
            foreach (PropertyInfo property in baseType.GetProperties())
            {
                
            }

            foreach(FieldInfo field in baseType.GetFields())
            {
                bool found = false;
                if (ignoreFields != null)
                {
                    foreach (String ignoreField in ignoreFields)
                    {
                        if (field.Name.Equals(ignoreField))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found)
                    {
                        continue;
                    }
                }

                object nodeValue = field.GetValue(graph);
                if (nodeValue != null && !nodeValue.Equals(field.GetValue(baseObject)))
                {
                    writer.WriteStartElement(field.Name);
                    if (field.FieldType.IsPrimitive || field.FieldType.Equals(typeof(String)))
                    {
                        writer.WriteString(nodeValue.ToString());
                    }
                    else
                    {
                        DiffSerializer innerSerializer = new DiffSerializer(field.FieldType, null, true);
                        innerSerializer.WriteObject(writer, nodeValue);
                    }
                    writer.WriteEndElement();
                }
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
                writer.WriteStartElement(baseType.Name);
            }
        }
    }
}
