using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Terraria_Server.Definitions;

namespace Terraria_Server.Misc
{
    public class Reflection
    {
        public static List<Type> typeList = new List<Type>();
        public static List<String> variableList = new List<String>();

        public static void ResetFeild(ref FieldInfo fInfo, ref Object FromType)
        {
            if (!fInfo.IsLiteral)
            {
                Type type = fInfo.FieldType;
                object data = fInfo.GetValue(FromType);
                object defaultValue = GetDefaultValue(ref type, ref data);
                fInfo.SetValue(FromType, defaultValue);
            }
        }

        public static object GetDefaultValue(ref Type type, ref object CurrentValue)
        {
            if (type == typeof(Boolean))
            {
                return false;
            }
            else if (type == typeof(Int32))
            {
                return 0;
            }
            else if (type == typeof(String))
            {
                return "";
            }
            else if (type == typeof(float))
            {
                return 0f;
            }
            else if (type == typeof(ProjectileType))
            {
                return 0;
            }
            else if (type == typeof(Vector2))
            {
                return default(Vector2);
            }
            else if (type == typeof(Byte))
            {
                return (byte)0;
            }
            else if (type == typeof(Boolean[]))
            {
                bool[] array = (Boolean[])CurrentValue;
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = false;
                }
                return array;
            }
            else if (type == typeof(String[]))
            {
                string[] array = (string[])CurrentValue;
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = "";
                }
                return array;
            }
            else if (type == typeof(float[]))
            {
                float[] array = (float[])CurrentValue;
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = 0f;
                }
                return array;
            }
            else if (type == typeof(Int32[]))
            {
                int[] array = (int[])CurrentValue;
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = 0;
                }
                return array;
            }
            else if (type == typeof(Vector2))
            {
                Vector2[] array = (Vector2[])CurrentValue;
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = default(Vector2);
                }
                return array;
            }
            else if (type == typeof(Byte))
            {
                byte[] array = (byte[])CurrentValue;
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = 0;
                }
                return array;
            }
            return null;
        }

        public static void ResetVariable(ref PropertyInfo fInfo, ref Object FromType)
        {
            Type type = fInfo.PropertyType;
            object data = fInfo.GetValue(FromType, null);
            object defaultValue = GetDefaultValue(ref type, ref data);
            fInfo.SetValue(FromType, defaultValue, null);
        }

        public static void FindVariables(ref Type Searchtype, ref Object typeClass)
        {
            if (Searchtype == null)
                return;

            foreach (FieldInfo t in Searchtype.GetFields())
            {
                FieldInfo type = t;
                ResetFeild(ref type, ref typeClass);
            }

            foreach (FieldInfo t in Searchtype.BaseType.GetFields())
            {
                FieldInfo type = t;
                ResetFeild(ref type, ref typeClass);
            }

            foreach (Type interf in Searchtype.BaseType.GetInterfaces())
            {
                foreach (PropertyInfo member in interf.GetProperties())
                {
                    PropertyInfo mem = member;
                    ResetVariable(ref mem, ref typeClass);
                }
            }

            foreach (Type fType in typeList)
            {
                Type type = fType;
                if (type != Searchtype)
                    FindVariables(ref type, ref typeClass);
            }
        }
    }
}
