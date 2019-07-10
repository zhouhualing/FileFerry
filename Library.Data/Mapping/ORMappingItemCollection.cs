using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using WD.Library.Core;
using System.Reflection;

namespace WD.Library.Data.Mapping
{
	/// <summary>
	/// ӳ���ϵ������
	/// </summary>
	/// <remarks>
	/// ӳ���ϵ������
	/// <seealso cref="WD.Library.Data.Mapping.ORMappingItem"/>
	/// </remarks>
	public class ORMappingItemCollection : KeyedCollection<string, ORMappingItem>
	{
		private string tableName = string.Empty;

		/// <summary>
		/// ORMappingItemCollection��Ĺ��캯��
		/// </summary>
		/// <remarks>
		/// ORMappingItemCollection�࣬һ��ORMappingItem�ļ����࣬�����ִ�Сд
		/// </remarks>
		public ORMappingItemCollection()
			: base(StringComparer.OrdinalIgnoreCase)
		{
		}

		/// <summary>
		/// ����
		/// </summary>
		public string TableName
		{
			get { return this.tableName; }
			set { this.tableName = value; }
		}

		/// <summary>
		/// д�뵽XmlWriter
		/// </summary>
		/// <param name="writer">Xml��д������</param>
		public void WriteToXml(XmlWriter writer)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(writer != null, "writer");

			writer.WriteStartElement("ORMapping");
			writer.WriteAttributeString("tableName", this.tableName);

			foreach (ORMappingItem item in this)
			{
				writer.WriteStartElement("Item");
				item.WriteToXml(writer);
				writer.WriteEndElement();
			}

			writer.WriteEndElement();
		}

		/// <summary>
		/// ��XmlReader�ж�ȡ
		/// </summary>
		/// <param name="reader">Xml�Ķ�������</param>
		/// <param name="type">��������</param>
		public void ReadFromXml(XmlReader reader, System.Type type)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(reader != null, "reader");
			ExceptionHelper.FalseThrow<ArgumentNullException>(type != null, "type");

			this.Clear();
			Dictionary<string, MemberInfo> miDict = GetMemberInfoDict(type);

			while (reader.EOF == false)
			{
				reader.Read();

				if (reader.IsStartElement("ORMapping"))
				{
					this.tableName = XmlHelper.GetAttributeValue(reader, "tableName", string.Empty);
					reader.ReadToDescendant("Item");
				}

				if (reader.IsStartElement("Item"))
				{
					string propName = reader.GetAttribute("propertyName");
					string subClassPropertyName = reader.GetAttribute("subClassPropertyName");
					string subClassTypeDescription = reader.GetAttribute("subClassTypeDescription");

					MemberInfo mi = null;

					if (miDict.TryGetValue(propName, out mi))
					{
						if (string.IsNullOrEmpty(subClassPropertyName) == false)
							if (string.IsNullOrEmpty(subClassTypeDescription) == false)
								mi = ORMapping.GetSubClassMemberInfoByName(subClassPropertyName,
									TypeCreator.GetTypeInfo(subClassTypeDescription));
							else
								mi = ORMapping.GetSubClassMemberInfoByName(subClassPropertyName, mi);

						if (mi != null)
							ReadItemFromXml(reader, mi);
					}
				}
			}
		}

		/// <summary>
		/// ����Mapping�ļ���
		/// </summary>
		/// <returns></returns>
		public ORMappingItemCollection Clone()
		{
			ORMappingItemCollection items = new ORMappingItemCollection();

			items.tableName = this.tableName;

			foreach (ORMappingItem item in this)
			{
				items.Add(item.Clone());
			}

			return items;
		}

		private Dictionary<string, MemberInfo> GetMemberInfoDict(System.Type type)
		{
			MemberInfo[] mis = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

			Dictionary<string, MemberInfo> dict = new Dictionary<string, MemberInfo>();

			foreach (MemberInfo mi in mis)
			{
				if (mi.MemberType == MemberTypes.Property || mi.MemberType == MemberTypes.Field)
					dict[mi.Name] = mi;
			}

			return dict;
		}

		private void ReadItemFromXml(XmlReader reader, MemberInfo mi)
		{
			ORMappingItem item = new ORMappingItem();

			item.ReadFromXml(reader, mi);

			this.Add(item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected override string GetKeyForItem(ORMappingItem item)
		{
			return item.DataFieldName;
		}
	}
}