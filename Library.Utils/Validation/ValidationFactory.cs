using System;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Validation
{
	/// <summary>
	/// У�����Ĺ����࣬�û�ͨ�������������У����
	/// </summary>
	/// <remarks>
	/// У�����Ĺ����࣬�û�ͨ�����������������ȡĿ��������Ԥ�����У����
	/// </remarks>
	public static class ValidationFactory
	{
		/// <summary>
		/// ��ȡָ�������϶����У����
		/// </summary>
		/// <param name="targetType">Ŀ������</param>
		/// <returns>У����</returns>
		/// <remarks>
		/// <code>
		/// 
		/// </code>
		/// </remarks>
		public static Validator CreateValidator(Type targetType)
		{
			return CreateValidator(targetType, string.Empty);
		}

		/// <summary>
		/// ��ȡָ�������϶����У����
		/// </summary>
		/// <param name="targetType">Ŀ������</param>
		/// <param name="unValidates">���Ե����Լ���</param>
		/// <returns></returns>
		public static Validator CreateValidator(Type targetType, List<string> unValidates)
		{
			return CreateValidator(targetType, string.Empty, unValidates);
		}

		/// <summary>
		/// ��ȡָ�������϶���Ĳ�����ָ�����򼯺ϵ�У����
		/// </summary>
		/// <param name="targetType">Ŀ������</param>
		/// <param name="ruleset">У���������Ĺ��򼯺�</param>
		/// <returns>У����</returns>
		/// <remarks>
		/// </remarks>
		public static Validator CreateValidator(Type targetType, string ruleset)
		{
			MetadataValidatorBuilder builder = new MetadataValidatorBuilder();

			return builder.CreateValidator(targetType, ruleset, null);
		}


		/// <summary>
		/// ��ȡָ�������϶����У����
		/// </summary>
		/// <param name="targetType">Ŀ������</param>
		/// <param name="ruleset">У���������Ĺ��򼯺�</param>
		/// <param name="unValidates">���Ե����Լ���</param>
		/// <returns></returns>
		public static Validator CreateValidator(Type targetType, string ruleset, List<string> unValidates)
		{
			MetadataValidatorBuilder builder = new MetadataValidatorBuilder();

			return builder.CreateValidator(targetType, ruleset, unValidates);
		}
	}
}
