using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

public class DeepCloneAttribute : Attribute
{
}

public class DontCloneAttribute : Attribute
{
}

public static class CloningExtensions
{
	public static void CopyFieldsFrom<T> (this T target, T template)
	{
		CopyoverCache<T>.Copyover (template, target);
	}
	 
	public static T FieldwiseClone<T> (this T template) where T : new()
	{
		var target = new T();
		CopyoverCache<T>.Copyover (template, target);
		return target;
	}
	 
	static class CopyoverCache<T>
	{
		private static readonly Action<T, T> copyover;
		
		static CopyoverCache()
		{
			ParameterExpression template = Expression.Parameter(typeof(T), "in");
			ParameterExpression target = Expression.Parameter(typeof(T), "in");
			
			// finds all non-readonly fields without a DontClone attribute and creates a list of
			// assignment statements for copying them over
			// fields with DeepClone are assumed to implement ICloneable
			var type = typeof(T);
			var bindings = new List<BinaryExpression> ();
			
			while (type != typeof(object))
			{
				bindings.AddRange (
					from field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					where !field.IsInitOnly && Attribute.GetCustomAttribute (field, typeof(DontCloneAttribute)) == null
					select Expression.Assign(Expression.Field(target, field),
						(field.FieldType.IsValueType || Attribute.GetCustomAttribute (field, typeof(DeepCloneAttribute)) == null)
						? Expression.Field(template, field)
						: (field.FieldType.GetInterfaces().Contains (typeof(ICloneable))
							? Expression.Convert (Expression.Call (Expression.Field(template, field), typeof(ICloneable).GetMethod("Clone")), field.FieldType)
							: Throw (field) ) ));
				
				type = type.BaseType;
			}
			
			copyover = Expression.Lambda<Action<T,T>>(Expression.Block (bindings), new ParameterExpression[] {template, target}).Compile();
		}
		
		static Expression Throw (FieldInfo field)
		{
			throw new ApplicationException ("Cannot perform a deep cloning of class '" + field.DeclaringType.Name + "' field '" + field.Name + "' because its type '" + field.FieldType.Name + "' doesn't implement ICloneable");
		}
		 
		public static void Copyover (T from, T to)
		{
			copyover (from, to);
		}

	} 
}
