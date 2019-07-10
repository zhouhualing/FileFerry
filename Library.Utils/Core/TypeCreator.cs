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
	/// ������󶨷�ʽ��̬����ʵ��
	/// </summary>
	/// <remarks>������󶨷�ʽ��̬����ʵ����
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
		/// ���ú�󶨷�ʽ��̬�Ĵ���һ��ʵ����
		/// </summary>
		/// <param name="typeDescription">����ʵ����������������</param>
		/// <param name="constructorParams">����ʵ���ĳ�ʼ������</param>
		/// <returns>ʵ������</returns>
		/// <remarks>������󶨷�ʽ��̬����һ��ʵ��
		public static object CreateInstance(string typeDescription, params object[] constructorParams)
		{
			Type type = GetTypeInfo(typeDescription);

			ExceptionHelper.FalseThrow<TypeLoadException>(type != null, Resource.TypeLoadException, typeDescription);

			return CreateInstance(type, constructorParams);
		}

		/// <summary>
		/// ����������Ϣ�������󣬸ö���ʹû�й��еĹ��췽����Ҳ���Դ���ʵ��
		/// </summary>
		/// <param name="type">��������ʱ��������Ϣ</param>
		/// <param name="constructorParams">����ʵ���ĳ�ʼ������</param>
		/// <returns>ʵ������</returns>
		/// <remarks>������󶨷�ʽ��̬����һ��ʵ��</remarks>
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
		/// �������������õ����Ͷ���
		/// </summary>
		/// <param name="typeDescription">��������������Ӧ����Namespace.ClassName, AssemblyName</param>
		/// <returns>���Ͷ���</returns>
		public static Type GetTypeInfo(string typeDescription)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(typeDescription, "typeDescription");

			Type result = Type.GetType(typeDescription);

			if (result == null)
			{
				TypeInfo ti = GenerateTypeInfo(typeDescription);

				AssemblyName aName = new AssemblyName(ti.AssemblyName);

				AssemblyMappingConfigurationElement element = AssemblyMappingSettings.GetConfig().Mapping[aName.Name];

				ExceptionHelper.TrueThrow(element == null, "�����ҵ�����{0}", typeDescription);

				ti.AssemblyName = element.MapTo;

				result = Type.GetType(ti.ToString());

				ExceptionHelper.FalseThrow(result != null, "���ܵõ�������Ϣ{0}", ti.ToString());
			}

			return result;
		}

		/// <summary>
		/// �õ�ĳ���������͵�ȱʡֵ
		/// </summary>
		/// <param name="type">����</param>
		/// <returns>�����͵�ȱʡֵ</returns>
		/// <remarks>���������Ϊ�������ͣ��򷵻�null�����򷵻�ֵ���͵�ȱʡֵ����Int32����0��DateTime����DateTime.MinValue</remarks>
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
