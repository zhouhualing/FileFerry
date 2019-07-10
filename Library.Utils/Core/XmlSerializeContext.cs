using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace WD.Library.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class XmlSerializeContext
	{
		private Dictionary<object, int> _ObjectContext = new Dictionary<object, int>();
		private Dictionary<Type, int> _TypeContext = new Dictionary<Type, int>();

		private int _CurrentID = 0;
		private int _CurrentTypeID = 0;

		internal XElement RootElement
		{
			get;
			set;
		}

		internal int CurrentID
		{
			get { return _CurrentID; }
			set { _CurrentID = value; }
		}

		internal Dictionary<object, int> ObjectContext
		{
			get { return _ObjectContext; }
			set { _ObjectContext = value; }
		}

		internal Dictionary<Type, int> TypeContext
		{
			get { return _TypeContext; }
			set { _TypeContext = value; }
		}

		internal int GetTypeID(System.Type type)
		{
			int result = -1;

			if (TypeContext.TryGetValue(type, out result) == false)
			{
				result = _CurrentTypeID++;
				TypeContext.Add(type, result);
			}

			return result;
		}

		internal void SerializeTypeInfo(XElement parent)
		{
			XElement typesNode = parent.AddChildElement("Types");

			foreach (KeyValuePair<Type, int> kp in TypeContext)
			{
				XElement typeNode = typesNode.AddChildElement("Type");

				typeNode.SetAttributeValue("id", kp.Value);
				typeNode.SetAttributeValue("name", kp.Key.AssemblyQualifiedName);
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class XmlDeserializeContext
	{
		internal XElement RootElement
		{
			get;
			set;
		}

		private Dictionary<int, object> _ObjectContext = new Dictionary<int, object>();
		private Dictionary<int, Type> _TypeContext = new Dictionary<int, Type>();

		internal Dictionary<int, object> ObjectContext
		{
			get { return _ObjectContext; }
			set { _ObjectContext = value; }
		}

		internal Dictionary<int, Type> TypeContext
		{
			get { return _TypeContext; }
			set { _TypeContext = value; }
		}

		internal Type GetTypeInfo(int id)
		{
			Type result = null;

			TypeContext.TryGetValue(id, out result).FalseThrow("不能找到ID为{0}的Type", id);

			return result;
		}

		internal Type GetTypeInfo(string strID)
		{
			int id = -1;

			int.TryParse(strID, out id).FalseThrow("不能将{0}转换为整数", strID);

			return GetTypeInfo(id);
		}

		internal void DeserializeTypeInfo(XElement parent)
		{
			var types = from typeNodes in parent.Descendants("Types").Descendants("Type")
						select new
						{
							ID = typeNodes.Attribute("id", -1),
							TypeDesp = typeNodes.Attribute("name", string.Empty),
						};

			types.ForEach(t => TypeContext[t.ID] = TypeCreator.GetTypeInfo(t.TypeDesp));
		}
	}
}
