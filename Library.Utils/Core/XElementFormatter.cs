using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Reflection;
using WD.Library.Properties;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using WD.Library.Caching;
using System.Diagnostics;

namespace WD.Library.Core
{
	/// <summary>
	/// 
	/// </summary>
	public enum XElementFormattingStatus
	{
		/// <summary>
		/// 
		/// </summary>
		None,

		/// <summary>
		/// 
		/// </summary>
		Serializing,
		
		/// <summary>
		/// 
		/// </summary>
		Deserializing
	}

	/// <summary>
	/// 对象序列化成XElement
	/// </summary>
	public class XElementFormatter
	{
		/// <summary>
		/// 处理状态
		/// </summary>
		public static XElementFormattingStatus FormattingStatus
		{
			get
			{
				XElementFormattingStatus status = XElementFormattingStatus.None;

				status = (XElementFormattingStatus)ObjectContextCache.Instance.GetOrAddNewValue("XElementFormattingStatus", (cache, key) =>
				{
					cache.Add(key, XElementFormattingStatus.None);
					return XElementFormattingStatus.None;
				});

				return status;
			}
			set
			{
				ObjectContextCache.Instance["XElementFormattingStatus"] = value;
			}
		}

		#region 序列化
		/// <summary>
		/// 对象序列化为XElement
		/// </summary>
		/// <param name="graph"></param>
		/// <returns></returns>
		public XElement Serialize(object graph)
		{
			XElementFormatter.FormattingStatus = XElementFormattingStatus.Serializing;

			try
			{
				XElement root = XElement.Parse(string.Format("<Root/>"));

				XmlSerializeContext context = new XmlSerializeContext() { RootElement = root };

				int objID = context.CurrentID;
				context.ObjectContext.Add(graph, objID);

				XElement graphElement = SerializeObjectToNode(root, graph, context);

				root.SetAttributeValue("value", objID);
				root.SetAttributeValue("isRef", true);

				context.SerializeTypeInfo(root);

				return root;
			}
			finally
			{
				XElementFormatter.FormattingStatus = XElementFormattingStatus.None;
			}
		}

		private XElement SerializeObjectToNode(XElement parent, object graph, XmlSerializeContext context)
		{
			System.Type type = graph.GetType();

			type.IsSerializable.FalseThrow<SerializationException>("类型{0}不可序列化", type.AssemblyQualifiedName);

			XElement node = context.RootElement.AddChildElement("Object");
			int objID = context.CurrentID++;

			node.SetAttributeValue("id", objID);
			node.SetAttributeValue("shortType", type.Name);
			node.SetAttributeValue("typeID", context.GetTypeID(type));

			if (TypeFields.GetTypeFields(type).XElementSerializable)
			{
				if (graph is IEnumerable)
					SerializeIEnumerableObject(node, (IEnumerable)graph, context);

				SerializePropertiesToNodes(node, graph, context);
			}
			else
			{
				node.AddChildElement("Value", SerializationHelper.SerializeObjectToString(graph, SerializationFormatterType.Binary));
			}

			return node;
		}

		private void SerializeIEnumerableObject(XElement parent, IEnumerable graph, XmlSerializeContext context)
		{
			//此处对一维数组与list<类型>的处理,其它的都以base64string处理
			Type objectType = graph.GetType();

			if (objectType.IsArray)
			{
				(objectType.GetArrayRank() == 1).FalseThrow<InvalidOperationException>("{0}不是一维数组，我们只支持一维数组", objectType.FullName);

				int rank = GetArrayRank(objectType);
				if (rank == 1)
				{
					parent.SetAttributeValue("dimLength", GetArrayDimensionsLength(objectType.GetArrayRank(), (Array)graph));
					SerializeIEnumerableOjbectToNode(graph, context, parent);
				}
				else
				{
					//嵌套数组
					parent.AddChildElement("Value", SerializationHelper.SerializeObjectToString(graph, SerializationFormatterType.Binary));
				}
			}
			else
			{
				SerializeIEnumerableOjbectToNode(graph, context, parent);
			}
		}

		private void SerializeIEnumerableOjbectToNode(IEnumerable graph, XmlSerializeContext context, XElement xElement)
		{
			XElement itemsNode = xElement.AddChildElement("Items");

			foreach (object data in graph)
			{
				if (data != null)
				{
					XElement itemNode = itemsNode.AddChildElement("Item");

					if (Type.GetTypeCode(data.GetType()) != TypeCode.Object)
					{
						itemNode.SetAttributeValue("value", data);
						itemNode.SetAttributeValue("shortType", data.GetType().Name);
						itemNode.SetAttributeValue("typeID", context.GetTypeID(data.GetType()));
					}
					else
					{
						int objID = 0;

						if (context.ObjectContext.TryGetValue(data, out objID) == false)
						{
							objID = context.CurrentID;
							context.ObjectContext.Add(data, objID);
							SerializeObjectToNode(xElement, data, context);
						}

						itemNode.SetAttributeValue("value", objID);
						itemNode.SetAttributeValue("shortType", data.GetType().Name);
						itemNode.SetAttributeValue("typeID", context.GetTypeID(data.GetType()));
						itemNode.SetAttributeValue("isRef", true);
					}
				}
			}
		}

		private string GetArrayDimensionsLength(int dimensions, Array array)
		{
			StringBuilder strB = new StringBuilder();

			for (int i = 0; i < dimensions; i++)
			{
				if (strB.Length > 0)
					strB.Append(",");

				strB.Append(array.GetLength(i));
			}
			return strB.ToString();
		}

		private void SerializePropertiesToNodes(XElement parent, object graph, XmlSerializeContext context)
		{
			TypeFields tf = TypeFields.GetTypeFields(graph.GetType());

			foreach (KeyValuePair<TypeFieldInfo, ExtendedFieldInfo> kp in tf.Fields)
			{
				ExtendedFieldInfo efi = kp.Value;

				if (efi.IsNotSerialized == false)
				{
					object data = GetValueFromObject(efi.FieldInfo, graph);

					if ((data == null || data == DBNull.Value || (data != null && data.Equals(TypeCreator.GetTypeDefaultValue(data.GetType())))) == false)
					{
						if (Type.GetTypeCode(data.GetType()) == TypeCode.Object)
						{
							int objID = 0;

							if (context.ObjectContext.TryGetValue(data, out objID) == false)
							{
								objID = context.CurrentID;
								context.ObjectContext.Add(data, objID);

								SerializeObjectToNode(parent, data, context);
							}

							XElement propertyElem = parent.AddChildElement("Field");

							propertyElem.SetAttributeValue("name", kp.Key.ObjectType.FullName + "." + efi.FieldInfo.Name);
							propertyElem.SetAttributeValue("value", objID);
							propertyElem.SetAttributeValue("isRef", true);
						}
						else
						{
							XElement propertyElem = parent.AddChildElement("Field");

							propertyElem.SetAttributeValue("name", kp.Key.ObjectType.FullName + "." + efi.FieldInfo.Name);
							propertyElem.SetAttributeValue("value", data);
						}
					}
				}
			}
		}

		private void SerializableBinaryToNode(XElement parent, XmlObjectMappingItem item, object graph, XmlSerializeContext context)
		{
			//此处对没有标记为序列化的需要做一个异常处理,还是?
			XElement xElement = context.RootElement.AddChildElement("object");
			xElement.SetAttributeValue("id", context.CurrentID++);
			xElement.SetAttributeValue("shortType", graph.GetType().Name);
			xElement.SetAttributeValue("typeID", context.GetTypeID(graph.GetType()));

			XElement itemNode = xElement.AddChildElement("value");

			byte[] buffer = null;
			using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
			{
				BinaryFormatter bf = new BinaryFormatter();
				bf.Serialize(ms, graph);
				buffer = ms.GetBuffer();
			}
			string imageBase64String = Convert.ToBase64String(buffer);

			itemNode.Add(imageBase64String);
		}
		#endregion

		#region 反序列化
		/// <summary>
		/// 反序列化对象
		/// </summary>
		/// <param name="root"></param>
		public object Deserialize(XElement root)
		{
			(root != null).FalseThrow<ArgumentNullException>("root");

			XElementFormatter.FormattingStatus = XElementFormattingStatus.Deserializing;
			try
			{
				XmlDeserializeContext context = new XmlDeserializeContext();

				context.RootElement = root;

				context.DeserializeTypeInfo(root);

				var objectProperty = GetObjectElementByID(root, root.Attribute("value", 0));

				object graph = DeserializeNodeToObject(objectProperty, context);

				return graph;
			}
			finally
			{
				XElementFormatter.FormattingStatus = XElementFormattingStatus.None;
			}
		}

		private static XElement GetObjectElementByID(XElement parent, int id)
		{
			XElement objectProperty = (from property in parent.Descendants("Object")
									   where property.Attribute("id", 0) == id
									   select property).FirstOrDefault();

			return objectProperty;
		}

		private object DeserializeNodeToObject(XElement objectNode, XmlDeserializeContext context)
		{
			object data = null;

			int objID = objectNode.Attribute("id", 0);

			if (context.ObjectContext.TryGetValue(objID, out data) == false)
			{
				Type type = context.GetTypeInfo(objectNode.AttributeValue("typeID"));

				data = CreateSerializableInstance(type);

				context.ObjectContext.Add(objID, data);

				string objectValue = objectNode.Element("Value", string.Empty);

				if (objectValue.IsNotEmpty())
				{
					data = SerializationHelper.DeserializeStringToObject(objectValue, SerializationFormatterType.Binary);
				}
				else
				{
					DeserializeNodesToProperties(objectNode, data, context);
				}
			}

			return data;
		}

		private static int[] CreateDimensionLengthArray(string dimLength)
		{
			string[] lengthDesps = dimLength.Split(',');

			int[] result = new int[lengthDesps.Length];

			for (int i = 0; i < lengthDesps.Length; i++)
			{
				result[i] = int.Parse(lengthDesps[i]);
			}

			return result;
		}

		private static object ConvertData(FieldInfo fi, object data)
		{
			try
			{
				System.Type realType = fi.FieldType;

				return DataConverter.ChangeType(data, realType);
			}
			catch (System.Exception ex)
			{
				throw new SystemSupportException(
					string.Format(Resource.ConvertXmlNodeToPropertyError,
						fi.Name, fi.Name, ex.Message),
						ex
					);
			}
		}

		private void DeserializeNodesToProperties(XElement parent, object graph, XmlDeserializeContext context)
		{
			TypeFields tf = TypeFields.GetTypeFields(graph.GetType());

			Debug.WriteLine(string.Format("Deserializing: {0}", graph.GetType().FullName));

			foreach (KeyValuePair<TypeFieldInfo, ExtendedFieldInfo> kp in tf.Fields)
			{
				ExtendedFieldInfo efi = kp.Value;

				if (efi.IsNotSerialized == false)
				{
					Debug.WriteLine(string.Format("Deserializing field: {0}.{1}", kp.Key.ObjectType.FullName, efi.FieldInfo.Name));

					System.Type realType = efi.FieldInfo.FieldType;

					object data = null;

					var propertiesElement = from property in parent.Descendants("Field")
											where property.Attribute("name", string.Empty) == kp.Key.ObjectType.FullName + "." + efi.FieldInfo.Name
											select property;

					XElement propertyElement = propertiesElement.FirstOrDefault();

					if (propertyElement != null)
					{
						if (propertyElement.Attribute("isRef", false) == true)
						{
							XElement objectElement = GetObjectElementByID(context.RootElement, propertyElement.Attribute("value", 0));

							if (objectElement != null)
							{
								data = DeserializeNodeToObject(objectElement, context);

								SetValueToObject(efi.FieldInfo, graph, data);
							}
						}
						else
						{
							data = propertyElement.Attribute("value", TypeCreator.GetTypeDefaultValue(realType));

							if (Convertible(realType, data))
								SetValueToObject(efi.FieldInfo, graph, ConvertData(efi.FieldInfo, data));
						}
					}
				}
			}

			DeserializeXmlSerilizableList(parent, graph, context);
		}

		private void DeserializeXmlSerilizableList(XElement parent, object graph, XmlDeserializeContext context)
		{
			if (graph is IXmlSerilizableList)
			{
				var valueNodes = from vNodes in parent.Descendants("Items").Descendants("Item")
								 select vNodes;

				IXmlSerilizableList list = (IXmlSerilizableList)graph;

				list.Clear();

				DeserializeNodeToCollection(valueNodes, context, (i, itemData) => list.Add(itemData));
			}
		}

		private delegate void DeserializeCollectionAction(int index, object itemDate);

		private void DeserializeNodeToCollection(IEnumerable<XElement> valueNodes, XmlDeserializeContext context, DeserializeCollectionAction action)
		{
			int i = 0;

			foreach (XElement item in valueNodes)
			{
				object itemData = null;

				if (item.Attribute("isRef", false) == true)
				{
					XElement objectElement = GetObjectElementByID(context.RootElement, item.Attribute("value", 0));
					itemData = DeserializeNodeToObject(objectElement, context);
				}
				else
				{
					Type type = context.GetTypeInfo(item.Attribute("typeID", string.Empty));

					itemData = item.Attribute("value", TypeCreator.GetTypeDefaultValue(type));

					itemData = DataConverter.ChangeType(itemData, type);
				}

				action(i, itemData);

				i++;
			}
		}

		/// <summary>
		/// 对数据进行最后的修饰，例如对日期类型的属性加工
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private static object DecorateDate(object data)
		{
			object result = data;

			if (data is DateTime)
			{
				DateTime dt = (DateTime)data;

				if (dt.Kind == DateTimeKind.Unspecified)
					result = DateTime.SpecifyKind(dt, DateTimeKind.Local);
			}

			return result;
		}

		private static void SetValueToObject(FieldInfo fi, object graph, object data)
		{
			data = DecorateDate(data);

			fi.SetValue(graph, data);
		}

		private static bool Convertible(System.Type targetType, object data)
		{
			bool result = true;

			if (data == null && targetType.IsValueType)
				result = false;
			else
			{
				if (data == DBNull.Value)
				{
					if (targetType != typeof(DBNull) && targetType != typeof(string))
						result = false;
				}
			}

			return result;
		}
		#endregion 反序列化

		#region 获取属性
		private static object GetValueFromObject(FieldInfo fi, object graph)
		{
			object data = null;

			if (graph != null)
			{
				data = fi.GetValue(graph);

				if (data != null)
				{
					System.Type dataType = data.GetType();
					if (dataType.IsEnum)
					{
						data = data.ToString();
					}
					else
						if (dataType == typeof(TimeSpan))
							data = ((TimeSpan)data).TotalSeconds;
				}
			}

			return data;
		}

		private static object CreateSerializableInstance(string typeDescription, params object[] constructorParams)
		{
			Type type = TypeCreator.GetTypeInfo(typeDescription);

			ExceptionHelper.FalseThrow<TypeLoadException>(type != null, Resource.TypeLoadException, typeDescription);

			return CreateSerializableInstance(type, constructorParams);
		}

		/// <summary>
		/// 根据类型信息创建对象，该对象即使没有公有的构造方法，也可以创建实例
		/// </summary>
		/// <param name="type">创建类型时的类型信息</param>
		/// <param name="constructorParams">创建实例的初始化参数</param>
		/// <returns>实例对象</returns>
		/// <remarks>运用晚绑定方式动态创建一个实例</remarks>
		public static object CreateSerializableInstance(System.Type type, params object[] constructorParams)
		{
			type.IsSerializable.FalseThrow<SerializationException>("类型{0}不可序列化", type.AssemblyQualifiedName);

			return TypeCreator.CreateInstance(type, constructorParams);
		}
		#endregion 获取属性

		#region

		private static int GetArrayRank(Type type)
		{
			Regex rgx = new Regex(@"(\[\])", RegexOptions.IgnorePatternWhitespace);
			string s = type.ToString();
			MatchCollection mchs = rgx.Matches(s);
			return mchs.Count;
		}

		private bool ContainDictionary(Type objectType)
		{
			Regex rgx = new Regex(@"(\System.Collections.Generic.Dictionary)", RegexOptions.IgnorePatternWhitespace);
			string s = objectType.ToString();
			return rgx.IsMatch(s);
		}

		#endregion
	}
}