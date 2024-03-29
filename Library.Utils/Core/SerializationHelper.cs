using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization.Formatters.Binary;

namespace WD.Library.Core
{
	/// <summary>
	/// 枚举类型，其包括Soap和Binary串行化编码方式
	/// </summary>
	/// <remarks>SerializationFormatterType枚举型包括Soap和Binary，Soap是SOAP消息格式编码，Binary是二进制消息格式编码。</remarks>
	public enum SerializationFormatterType
	{
		/// <summary>
		/// SOAP消息格式编码
		/// </summary>
		Soap,

		/// <summary>
		/// 二进制消息格式编码
		/// </summary>
		Binary
	}


	/// <summary>
	/// 帮助对象实现序列化和反序列化。
	/// </summary>
	/// <remarks>对象序列化是把对象序列化转化为string类型；对象反序列化是把对象从string类型反序列化转化为其源类型。
	/// </remarks>
	public static class SerializationHelper
	{
		#region Helper method
		/// <summary>
		/// 按照串行化的编码要求，生成对应的编码器。
		/// </summary>
		/// <param name="formatterType"></param>
		/// <returns></returns>
		private static IRemotingFormatter GetFormatter(SerializationFormatterType formatterType)
		{
			switch (formatterType)
			{
				case SerializationFormatterType.Binary:
					return new BinaryFormatter();
				case SerializationFormatterType.Soap:
					return new SoapFormatter();
				default:
					throw new NotSupportedException();
			}
		}
		#endregion

		/// <summary>
		/// 把对象序列化转换为字符串
		/// </summary>
		/// <param name="graph">可串行化对象实例</param>
		/// <param name="formatterType">消息格式编码类型（Soap或Binary型）</param>
		/// <returns>串行化转化结果</returns>
		/// <remarks>调用BinaryFormatter或SoapFormatter的Serialize方法实现主要转换过程。
		public static string SerializeObjectToString(object graph, SerializationFormatterType formatterType)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");

			using (MemoryStream memoryStream = new MemoryStream())
			{
				IRemotingFormatter formatter = GetFormatter(formatterType);
				formatter.Serialize(memoryStream, graph);
				Byte[] arrGraph = memoryStream.ToArray();
				return Convert.ToBase64String(arrGraph);
			}
		}

		/// <summary>
		/// 把已序列化为字符串类型的对象反序列化为指定的类型
		/// </summary>
		/// <param name="serializedGraph">已序列化为字符串类型的对象</param>
		/// <param name="formatterType">消息格式编码类型（Soap或Binary型）</param>
		/// <typeparam name="T">对象转换后的类型</typeparam>
		/// <returns>串行化转化结果</returns>
		/// <remarks>调用BinaryFormatter或SoapFormatter的Deserialize方法实现主要转换过程。
		/// </remarks>
		public static T DeserializeStringToObject<T>(string serializedGraph, SerializationFormatterType formatterType)
		{
			return (T)DeserializeStringToObject(serializedGraph, formatterType);
		}

		/// <summary>
		/// 把已序列化为字符串类型的对象反序列化为指定的类型
		/// </summary>
		/// <typeparam name="T">对象转换后的类型</typeparam>
		/// <param name="serializedGraph">已序列化为字符串类型的对象</param>
		/// <param name="formatterType">消息格式编码类型（Soap或Binary型）</param>
		/// <param name="binder">反序列化时的类型转换器</param>
		/// <returns>串行化转化结</returns>
		/// <remarks>调用BinaryFormatter或SoapFormatter的Deserialize方法实现主要转换过程。</remarks>
		public static T DeserializeStringToObject<T>(string serializedGraph, SerializationFormatterType formatterType, SerializationBinder binder)
		{
			return (T)DeserializeStringToObject(serializedGraph, formatterType, binder);
		}

		/// <summary>
		/// 把已序列化为字符串类型的对象反序列化为指定的类型
		/// </summary>
		/// <param name="serializedGraph">已序列化为字符串类型的对象</param>
		/// <param name="formatterType">消息格式编码类型（Soap或Binary型）</param>
		/// <returns>串行化转化结果</returns>
		/// <remarks>调用BinaryFormatter或SoapFormatter的Deserialize方法实现主要转换过程。
		public static object DeserializeStringToObject(string serializedGraph, SerializationFormatterType formatterType)
		{
			return DeserializeStringToObject(serializedGraph, formatterType, null);
		}

		/// <summary>
		/// 把已序列化为字符串类型的对象反序列化为指定的类型
		/// </summary>
		/// <param name="serializedGraph">已序列化为字符串类型的对象</param>
		/// <param name="formatterType">消息格式编码类型（Soap或Binary型）</param>
		/// <param name="binder"></param>
		/// <returns>串行化转化结果</returns>
		/// <remarks>调用BinaryFormatter或SoapFormatter的Deserialize方法实现主要转换过程。</remarks>
		public static object DeserializeStringToObject(string serializedGraph, SerializationFormatterType formatterType, SerializationBinder binder)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(serializedGraph, "serializedGraph");

			Byte[] arrGraph = Convert.FromBase64String(serializedGraph);
			using (MemoryStream memoryStream = new MemoryStream(arrGraph))
			{
				IRemotingFormatter formatter = GetFormatter(formatterType);
				formatter.Binder = binder;

				return formatter.Deserialize(memoryStream);
			}
		}

		/// <summary>
		/// 通过序列化复制对象
		/// </summary>
		/// <param name="graph"></param>
		/// <returns></returns>
		public static object CloneObject(object graph)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");

			using (MemoryStream memoryStream = new MemoryStream(1024))
			{
				BinaryFormatter formatter = new BinaryFormatter();

				formatter.Serialize(memoryStream, graph);

				memoryStream.Position = 0;

				return formatter.Deserialize(memoryStream);
			}
		}
	}

	/// <summary>
	/// 与版本无关的Binder类，用于反序列化对象
	/// </summary>
	public class VersionStrategyBinder : SerializationBinder
	{
		private const string VersionMatchTemplate = @"Version=([0-9.]{1,})(,)";

		private VersionStrategyBinder()
		{
		}

		/// <summary>
		/// 单一实例
		/// </summary>
		public static readonly VersionStrategyBinder Instance = new VersionStrategyBinder();

		/// <summary>
		/// 根据类型描述，经过转换（去处Version信息），返回类型对象
		/// </summary>
		/// <param name="assemblyName">AssemblyName</param>
		/// <param name="typeName">类型的描述</param>
		/// <returns>类型的实例</returns>
		public override Type BindToType(string assemblyName, string typeName)
		{
			typeName = Regex.Replace(typeName, VersionMatchTemplate, string.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);

			Assembly assembly = Assembly.Load(assemblyName);

			return assembly.GetType(typeName);
		}
	}
}
