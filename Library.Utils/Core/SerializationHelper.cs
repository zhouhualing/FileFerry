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
	/// ö�����ͣ������Soap��Binary���л����뷽ʽ
	/// </summary>
	/// <remarks>SerializationFormatterTypeö���Ͱ���Soap��Binary��Soap��SOAP��Ϣ��ʽ���룬Binary�Ƕ�������Ϣ��ʽ���롣</remarks>
	public enum SerializationFormatterType
	{
		/// <summary>
		/// SOAP��Ϣ��ʽ����
		/// </summary>
		Soap,

		/// <summary>
		/// ��������Ϣ��ʽ����
		/// </summary>
		Binary
	}


	/// <summary>
	/// ��������ʵ�����л��ͷ����л���
	/// </summary>
	/// <remarks>�������л��ǰѶ������л�ת��Ϊstring���ͣ��������л��ǰѶ����string���ͷ����л�ת��Ϊ��Դ���͡�
	/// </remarks>
	public static class SerializationHelper
	{
		#region Helper method
		/// <summary>
		/// ���մ��л��ı���Ҫ�����ɶ�Ӧ�ı�������
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
		/// �Ѷ������л�ת��Ϊ�ַ���
		/// </summary>
		/// <param name="graph">�ɴ��л�����ʵ��</param>
		/// <param name="formatterType">��Ϣ��ʽ�������ͣ�Soap��Binary�ͣ�</param>
		/// <returns>���л�ת�����</returns>
		/// <remarks>����BinaryFormatter��SoapFormatter��Serialize����ʵ����Ҫת�����̡�
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
		/// �������л�Ϊ�ַ������͵Ķ������л�Ϊָ��������
		/// </summary>
		/// <param name="serializedGraph">�����л�Ϊ�ַ������͵Ķ���</param>
		/// <param name="formatterType">��Ϣ��ʽ�������ͣ�Soap��Binary�ͣ�</param>
		/// <typeparam name="T">����ת���������</typeparam>
		/// <returns>���л�ת�����</returns>
		/// <remarks>����BinaryFormatter��SoapFormatter��Deserialize����ʵ����Ҫת�����̡�
		/// </remarks>
		public static T DeserializeStringToObject<T>(string serializedGraph, SerializationFormatterType formatterType)
		{
			return (T)DeserializeStringToObject(serializedGraph, formatterType);
		}

		/// <summary>
		/// �������л�Ϊ�ַ������͵Ķ������л�Ϊָ��������
		/// </summary>
		/// <typeparam name="T">����ת���������</typeparam>
		/// <param name="serializedGraph">�����л�Ϊ�ַ������͵Ķ���</param>
		/// <param name="formatterType">��Ϣ��ʽ�������ͣ�Soap��Binary�ͣ�</param>
		/// <param name="binder">�����л�ʱ������ת����</param>
		/// <returns>���л�ת����</returns>
		/// <remarks>����BinaryFormatter��SoapFormatter��Deserialize����ʵ����Ҫת�����̡�</remarks>
		public static T DeserializeStringToObject<T>(string serializedGraph, SerializationFormatterType formatterType, SerializationBinder binder)
		{
			return (T)DeserializeStringToObject(serializedGraph, formatterType, binder);
		}

		/// <summary>
		/// �������л�Ϊ�ַ������͵Ķ������л�Ϊָ��������
		/// </summary>
		/// <param name="serializedGraph">�����л�Ϊ�ַ������͵Ķ���</param>
		/// <param name="formatterType">��Ϣ��ʽ�������ͣ�Soap��Binary�ͣ�</param>
		/// <returns>���л�ת�����</returns>
		/// <remarks>����BinaryFormatter��SoapFormatter��Deserialize����ʵ����Ҫת�����̡�
		public static object DeserializeStringToObject(string serializedGraph, SerializationFormatterType formatterType)
		{
			return DeserializeStringToObject(serializedGraph, formatterType, null);
		}

		/// <summary>
		/// �������л�Ϊ�ַ������͵Ķ������л�Ϊָ��������
		/// </summary>
		/// <param name="serializedGraph">�����л�Ϊ�ַ������͵Ķ���</param>
		/// <param name="formatterType">��Ϣ��ʽ�������ͣ�Soap��Binary�ͣ�</param>
		/// <param name="binder"></param>
		/// <returns>���л�ת�����</returns>
		/// <remarks>����BinaryFormatter��SoapFormatter��Deserialize����ʵ����Ҫת�����̡�</remarks>
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
		/// ͨ�����л����ƶ���
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
	/// ��汾�޹ص�Binder�࣬���ڷ����л�����
	/// </summary>
	public class VersionStrategyBinder : SerializationBinder
	{
		private const string VersionMatchTemplate = @"Version=([0-9.]{1,})(,)";

		private VersionStrategyBinder()
		{
		}

		/// <summary>
		/// ��һʵ��
		/// </summary>
		public static readonly VersionStrategyBinder Instance = new VersionStrategyBinder();

		/// <summary>
		/// ������������������ת����ȥ��Version��Ϣ�����������Ͷ���
		/// </summary>
		/// <param name="assemblyName">AssemblyName</param>
		/// <param name="typeName">���͵�����</param>
		/// <returns>���͵�ʵ��</returns>
		public override Type BindToType(string assemblyName, string typeName)
		{
			typeName = Regex.Replace(typeName, VersionMatchTemplate, string.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);

			Assembly assembly = Assembly.Load(assemblyName);

			return assembly.GetType(typeName);
		}
	}
}
