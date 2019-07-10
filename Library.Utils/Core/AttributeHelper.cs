using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

namespace WD.Library.Core
{
	/// <summary>
	/// Attribute����Ŀ�����
	/// </summary>
	/// <remarks>
	/// ͨ�����䣬��ȡ�ࡢ���ԡ������ȵ�Attribute����
	/// </remarks>
	public static class AttributeHelper
	{
		private struct AttrDictEntry
		{
			public object MemberInfo;
			public System.Type AttributeType;
			public bool Inherited;
		}

		private static Dictionary<AttrDictEntry, Attribute> dictionary = new Dictionary<AttrDictEntry, Attribute>();

		/// <summary>
		/// ��ȡ�ࡢ���Ի򷽷��ϵ�Attribute����
		/// </summary>
		/// <typeparam name="T">����ȡ���ࡢ���Ի򷽷�������</typeparam>
		/// <param name="element">�ࡢ���Ի򷽷����͵�ʵ��</param>
		/// <returns>�ࡢ���Ի򷽷��ȵ�Attributeʵ��</returns>
		/// <remarks>
		/// ��ȡ�ࡢ���Ի򷽷��ϵ�Attribute���塣���÷������ʵ�֡�
		
		/// <seealso cref="System.ComponentModel.DescriptionAttribute" />
		/// <seealso cref="WD.Library.Core.EnumItemDescriptionAttribute"/>
		/// </remarks>
		public static T GetCustomAttribute<T>(MemberInfo element) where T : Attribute
		{
			T result = default(T);
			System.Type attrType = typeof(T);

			AttrDictEntry key = CalculateKey(element, attrType, true);

			lock (AttributeHelper.dictionary)
			{
				if (AttributeHelper.dictionary.ContainsKey(key))
					result = (T)AttributeHelper.dictionary[key];
				else
				{
					result = (T)Attribute.GetCustomAttribute(element, attrType);
					AttributeHelper.dictionary[key] = result;
				}
			}

			return result;
		}

		private static AttrDictEntry CalculateKey(object element, System.Type attrType, bool inherited)
		{
			AttrDictEntry key;

			key.MemberInfo = element;
			key.AttributeType = attrType;
			key.Inherited = inherited;

			return key;
		}
	}
}
