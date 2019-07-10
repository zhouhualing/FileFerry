using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace WD.Library.Reflection
{
	public static class PropertyUtility
	{
		public static Func<TProperty> CreateGetter<TProperty>(
			this PropertyInfo propertyInfo, object owner)
		{
            Debug.Assert(propertyInfo != null && owner != null);

			Type getterType = System.Linq.Expressions.Expression.GetFuncType(
								new[] { propertyInfo.PropertyType });


			object getter = Delegate.CreateDelegate(
								getterType, owner, propertyInfo.GetGetMethod());
			return (Func<TProperty>)getter;
		}

        public static dynamic CreateGetter(
            this PropertyInfo propertyInfo, object owner)
        {
            Debug.Assert(propertyInfo != null && owner != null);

            Type getterType = System.Linq.Expressions.Expression.GetFuncType(
                                new[] { propertyInfo.PropertyType });
            //Type getterType = typeof(Func<>).MakeGenericType(propertyInfo.PropertyType);

            object getter = Delegate.CreateDelegate(
                                getterType, owner, propertyInfo.GetGetMethod());
            return getter;
        }


        public static TDelegate CreateDelegate<TDelegate>(
			this PropertyInfo propertyInfo,
			object owner)
		{
            Debug.Assert(propertyInfo != null && owner != null);

            object getter = Delegate.CreateDelegate(
								typeof(TDelegate), owner, propertyInfo.GetGetMethod());
			return (TDelegate)getter;
		}

		public static Action<TProperty> CreateSetter<TProperty>(
			this PropertyInfo propertyInfo, object owner)
		{
            Debug.Assert(propertyInfo != null && owner != null);

            var propertyType = propertyInfo.PropertyType;
			var setterType = System.Linq.Expressions.Expression.GetActionType(new[] { propertyType });

			Delegate setter = Delegate.CreateDelegate(
						setterType, owner, propertyInfo.GetSetMethod());
			return (Action<TProperty>)setter;
		}

        public static dynamic CreateSetter(
            this PropertyInfo propertyInfo, object owner)
        {
            Debug.Assert(propertyInfo != null && owner != null);

            var propertyType = propertyInfo.PropertyType;
            var setterType = System.Linq.Expressions.Expression.GetActionType(new[] { propertyType });

            Delegate setter = Delegate.CreateDelegate(
                        setterType, owner, propertyInfo.GetSetMethod());
            return setter;
        }

        public static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            return type.GetProperty(propertyName);
        }

        public static PropertyInfo GetPropertyInfo<T>(Expression<Func<T>> expression)
		{
			var memberExpression = expression.Body as MemberExpression;
			if (memberExpression == null)
			{
				throw new ArgumentException(
					"MemberExpression expected.", "expression");
			}

			if (memberExpression.Member == null)
			{
				throw new ArgumentException("Member should not be null.");
			}
			if (memberExpression.Member.MemberType != MemberTypes.Property)
			{
				throw new ArgumentException("Property expected.", "expression");
			}
			PropertyInfo propertyInfo = (PropertyInfo)memberExpression.Member;
			return propertyInfo;
		}

        public static PropertyInfo GetProperty(Type type, string propertyName, bool throwIfInvalid = true)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException($"propertyName:{propertyName} does not exist.");
            }

            PropertyInfo propertyInfo = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

            if (!IsValidProperty(propertyInfo))
            {
                if (throwIfInvalid)
                {
                    throw new ArgumentException(
                        string.Format(
                            System.Globalization.CultureInfo.CurrentCulture,
                            "Invalid Property",
                            propertyName,
                            type.FullName));
                }
                else
                {
                    return null;
                }
            }

            return propertyInfo;
        }
        public static bool IsValidProperty(PropertyInfo propertyInfo)
        {
            return null != propertyInfo                // exists
                    && propertyInfo.CanRead            // and it's readable
                    && propertyInfo.GetIndexParameters().Length == 0;    // and it's not an indexer
        }

        public static Hashtable GetProperties(object obj)
        {
            Hashtable table = new Hashtable();
            foreach (var prop in obj.GetType().GetProperties())
            {
                if (IsValidProperty(prop))
                {
                    table.Add(prop.Name, ReflectionCompiler.CreateGetter(prop)(obj));
                }
            }

            return table;
        }

    }
}
