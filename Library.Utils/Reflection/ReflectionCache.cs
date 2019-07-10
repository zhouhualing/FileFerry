using System;
using System.Collections.Generic;
using System.Reflection;

namespace WD.Library.Reflection
{
	static class ReflectionCache
	{
		static readonly Dictionary<PropertyInfo, Func<object, object>> getterDictionary
			= new Dictionary<PropertyInfo, Func<object, object>>();

		static readonly Dictionary<PropertyInfo, Action<object, object>> setterDictionary
			= new Dictionary<PropertyInfo, Action<object, object>>();

		static readonly Dictionary<MethodInfo, Action<object, object[]>> voidMethodDictionary
			= new Dictionary<MethodInfo, Action<object, object[]>>();

		internal static Func<object, object> RetrieveGetter(PropertyInfo propertyInfo)
		{
			Func<object, object> getter;

			if (!getterDictionary.TryGetValue(propertyInfo, out getter))
			{
				getter = ReflectionCompiler.CreateGetter(propertyInfo);
				getterDictionary[propertyInfo] = getter;
			}

			return getter;
		}

		internal static Action<object, object[]> RetrieveAction(MethodInfo targetMethod)
		{
			Action<object, object[]> action;
			if (!voidMethodDictionary.TryGetValue(targetMethod, out action))
			{
				action = ReflectionCompiler.CreateAction(targetMethod);
				voidMethodDictionary[targetMethod] = action;
			}

			return action;
		}

		internal static Action<object, object> RetrieveSetter(PropertyInfo propertyInfo)
		{
			Action<object, object> setter;
			if (!setterDictionary.TryGetValue(propertyInfo, out setter))
			{
				setter = ReflectionCompiler.CreateSetter(propertyInfo);
				setterDictionary[propertyInfo] = setter;
			}

			return setter;
		}
	}
}
