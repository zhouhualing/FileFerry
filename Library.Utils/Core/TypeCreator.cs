using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using WD.Library.Properties;
using WD.Library.Caching;
using WD.Library.Configuration;

namespace WD.Library.Core
{
	/// <summary>
	/// 运用晚绑定方式动态生成实例
	/// </summary>
	/// <remarks>运用晚绑定方式动态生成实例。
	/// </remarks>
	public static class TypeCreator
	{
		private struct TypeInfo
		{
			public string AssemblyName;
			public string TypeName;

			public override string ToString()
			{
				return TypeName + ", " + AssemblyName;
			}
		}

		/// <summary>
		/// 运用后绑定方式动态的创建一个实例。
		/// </summary>
		/// <param name="typeDescription">创建实例的完整类型名称</param>
		/// <param name="constructorParams">创建实例的初始化参数</param>
		/// <returns>实例对象</returns>
		/// <remarks>运用晚绑定方式动态创建一个实例
		public static object CreateInstance(string typeDescription, params object[] constructorParams)
		{
			Type type = GetTypeInfo(typeDescription);

			ExceptionHelper.FalseThrow<TypeLoadException>(type != null, Resource.TypeLoadException, typeDescription);

			return CreateInstance(type, constructorParams);
		}

		/// <summary>
		/// 根据类型信息创建对象，该对象即使没有公有的构造方法，也可以创建实例
		/// </summary>
		/// <param name="type">创建类型时的类型信息</param>
		/// <param name="constructorParams">创建实例的初始化参数</param>
		/// <returns>实例对象</returns>
		/// <remarks>运用晚绑定方式动态创建一个实例</remarks>
		public static object CreateInstance(System.Type type, params object[] constructorParams)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(type != null, "type");
			ExceptionHelper.FalseThrow<ArgumentNullException>(constructorParams != null, "constructorParams");

			BindingFlags bf= BindingFlags.Instance | BindingFlags.Public;
			if (constructorParams.Length > 0)
			{
				Type[] types = new Type[constructorParams.Length];

				for (int i = 0; i < types.Length; i++)
					types[i] = constructorParams[i].GetType();
				ConstructorInfo ci = type.GetConstructor(bf, null, CallingConventions.HasThis, types, null);

				if (ci != null)
					return ci.Invoke(constructorParams);
			}
			else
			{
				return Activator.CreateInstance(type, true);
			}

			return null;
		}

		/// <summary>
		/// 根据类型描述得到类型对象
		/// </summary>
		/// <param name="typeDescription">完整类型描述，应该是Namespace.ClassName, AssemblyName</param>
		/// <returns>类型对象</returns>
		public static Type GetTypeInfo(string typeDescription)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(typeDescription, "typeDescription");

			Type result = Type.GetType(typeDescription);

			if (result == null)
			{
				TypeInfo ti = GenerateTypeInfo(typeDescription);

				AssemblyName aName = new AssemblyName(ti.AssemblyName);

				AssemblyMappingConfigurationElement element = AssemblyMappingSettings.GetConfig().Mapping[aName.Name];

				ExceptionHelper.TrueThrow(element == null, "不能找到类型{0}", typeDescription);

				ti.AssemblyName = element.MapTo;

				result = Type.GetType(ti.ToString());

				ExceptionHelper.FalseThrow(result != null, "不能得到类型信息{0}", ti.ToString());
			}

			return result;
		}

		/// <summary>
		/// 得到某个数据类型的缺省值
		/// </summary>
		/// <param name="type">类型</param>
		/// <returns>该类型的缺省值</returns>
		/// <remarks>如果该类型为引用类型，则返回null，否则返回值类型的缺省值。如Int32返回0，DateTime返回DateTime.MinValue</remarks>
		public static object GetTypeDefaultValue(System.Type type)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(type != null, "type");

			object result = null;

			if (type.IsValueType)
			{
				if (TypeDefaultValueCacheQueue.Instance.TryGetValue(type, out result) == false)
				{
					result = TypeCreator.CreateInstance(type);

					TypeDefaultValueCacheQueue.Instance.Add(type, result);
				}
			}
			else
				result = null;

			return result;
		}

		private static TypeInfo GenerateTypeInfo(string typeDescription)
		{
			TypeInfo info = new TypeInfo();

			string[] typeParts = typeDescription.Split(',');

			info.TypeName = typeParts[0].Trim();

			StringBuilder strB = new StringBuilder(256);

			for (int i = 1; i < typeParts.Length; i++)
			{
				if (strB.Length > 0)
					strB.Append(", ");

				strB.Append(typeParts[i]);
			}

			info.AssemblyName = strB.ToString().Trim();

			return info;
		}
	}

	internal class TypeDefaultValueCacheQueue : CacheQueue<Type, object>
	{
		public static readonly TypeDefaultValueCacheQueue Instance = CacheManager.GetInstance<TypeDefaultValueCacheQueue>();
	}
}
