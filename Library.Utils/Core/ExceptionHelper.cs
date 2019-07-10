using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using WD.Library.Properties;
using System.Web.Services.Protocols;
using System.Runtime.CompilerServices;

namespace WD.Library.Core
{
	/// <summary>
	/// Exception���ߣ��ṩ��TrueThrow��FalseThrow�ȷ���
	/// </summary>
	/// <remarks>Exception���ߣ�TrueThrow�����ж����Ĳ�������ֵ�Ƿ�Ϊtrue���������׳��쳣��FalseThrow�����ж����Ĳ�������ֵ�Ƿ�Ϊfalse���������׳��쳣��
	/// </remarks>
	public static class ExceptionHelper
	{
		/// <summary>
		/// �������Ƿ�Ϊ�գ����Ϊ�գ��׳�ArgumentNullException
		/// </summary>
		/// <param name="data">�����Ķ���</param>
		/// <param name="message">����������</param>
		public static void NullCheck(this object data, string message = "Object could not be null.")
		{
			NullCheck<ArgumentNullException>(data, message);
		}
		
		/// <summary>
		/// �������Ƿ�Ϊ�գ����Ϊ�գ��׳��쳣
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="data"></param>
		/// <param name="message"></param>
		/// <param name="messageParams"></param>
		public static void NullCheck<T>(this object data, string message, params object[] messageParams) where T : System.Exception
		{
			(data == null).TrueThrow<T>(message, messageParams);
		}

		/// <summary>
		/// ����������ʽboolExpression�Ľ��ֵΪ��(true)�����׳�strMessageָ���Ĵ�����Ϣ
		/// </summary>
		/// <param name="parseExpressionResult">�������ʽ</param>
		/// <param name="message">������Ϣ</param>
		/// <param name="messageParams">������Ϣ����</param>
		/// <remarks>
		/// ����������ʽboolExpression�Ľ��ֵΪ��(true)�����׳�strMessageָ���Ĵ�����Ϣ
		
		/// <seealso cref="FalseThrow"/>
		/// <seealso cref="WD.Library.Compression.ZipReader"/>
		/// </remarks>
		/// <example>
		/// <code>
		/// ExceptionTools.TrueThrow(name == string.Empty, "�Բ������ֲ���Ϊ�գ�");
		/// </code>
		/// </example>
		public static void TrueThrow(this bool parseExpressionResult, string message, params object[] messageParams)
		{
			TrueThrow<SystemSupportException>(parseExpressionResult, message, messageParams);
		}

		/// <summary>
		/// ����������ʽboolExpression�Ľ��ֵΪ��(true)�����׳�strMessageָ���Ĵ�����Ϣ
		/// </summary>
		/// <param name="parseExpressionResult">�������ʽ</param>
		/// <param name="message">������Ϣ</param>
		/// <param name="messageParams">������Ϣ�Ĳ���</param>
		/// <typeparam name="T">�쳣������</typeparam>
		/// <remarks>
		/// ����������ʽboolExpression�Ľ��ֵΪ��(true)�����׳�messageָ���Ĵ�����Ϣ
		
		/// <seealso cref="FalseThrow"/>
		/// <seealso cref="WD.Library.Logging.LogEntity"/>
		/// </remarks>
		public static void TrueThrow<T>(this bool parseExpressionResult, string message, params object[] messageParams) where T : System.Exception
		{
			Type exceptionType = typeof(T);

			if (parseExpressionResult)
			{
				if (message == null)
					throw new ArgumentNullException("message");

				Object obj = Activator.CreateInstance(exceptionType);

				Type[] types = new Type[1];
				types[0] = typeof(string);

				ConstructorInfo constructorInfoObj = exceptionType.GetConstructor(
					BindingFlags.Instance | BindingFlags.Public, null,
					CallingConventions.HasThis, types, null);

				Object[] args = new Object[1];

				args[0] = string.Format(message, messageParams);

				constructorInfoObj.Invoke(obj, args);

				throw (Exception)obj;
			}
		}

		/// <summary>
		/// ����������ʽboolExpression�Ľ��ֵΪ�٣�false�������׳�strMessageָ���Ĵ�����Ϣ
		/// </summary>
		/// <param name="parseExpressionResult">�������ʽ</param>
		/// <param name="message">������Ϣ</param>
		/// <param name="messageParams">������Ϣ����</param>
		
		/// <seealso cref="TrueThrow"/>
		/// <seealso cref="WD.Library.Logging.LoggerFactory"/>
		/// <remarks>
		/// ����������ʽboolExpression�Ľ��ֵΪ�٣�false�������׳�messageָ���Ĵ�����Ϣ
		/// </remarks>
		/// <example>
		/// <code>
		/// ExceptionTools.FalseThrow(name != string.Empty, "�Բ������ֲ���Ϊ�գ�");
		/// </code>
		/// </example>
		public static void FalseThrow(this bool parseExpressionResult, string message, params object[] messageParams)
		{
			TrueThrow(false == parseExpressionResult, message, messageParams);
		}

		/// <summary>
		/// ����������ʽboolExpression�Ľ��ֵΪ�٣�false�������׳�messageָ���Ĵ�����Ϣ
		/// </summary>
		/// <typeparam name="T">�쳣������</typeparam>
		/// <param name="parseExpressionResult">�������ʽ</param>
		/// <param name="message">������Ϣ</param>
		/// <param name="messageParams">������Ϣ����</param>
		/// <remarks>
		/// ����������ʽboolExpression�Ľ��ֵΪ�٣�false�������׳�strMessageָ���Ĵ�����Ϣ
		
		/// <seealso cref="TrueThrow"/>
		/// <seealso cref="WD.Library.Core.EnumItemDescriptionAttribute"/>
		/// </remarks>
		/// <example>
		/// <code>
		/// ExceptionTools.FalseThrow(name != string.Empty, typeof(ApplicationException), "�Բ������ֲ���Ϊ�գ�");
		/// </code>
		/// </example>
		public static void FalseThrow<T>(this bool parseExpressionResult, string message, params object[] messageParams) where T : System.Exception
		{
			TrueThrow<T>(false == parseExpressionResult, message, messageParams);
		}

		/// <summary>
		/// ����ַ��������Ƿ�ΪNull��մ�������ǣ����׳��쳣
		/// </summary>
		/// <param name="data">�ַ�������ֵ</param>
		/// <param name="paramName">�ַ�������</param>
		/// <remarks>
		/// ���ַ�������ΪNull��մ����׳�ArgumentException�쳣
		
		/// </remarks>
		public static void CheckStringIsNullOrEmpty(this string data, string paramName)
		{
			if (string.IsNullOrEmpty(data))
				throw new ArgumentException(string.Format(Resource.StringParamCanNotBeNullOrEmpty, paramName));
		}

        /// <summary>
        /// ����ַ��������Ƿ�ΪNull,�մ����߿ո�հ��ַ�������ǣ����׳��쳣
        /// </summary>
        /// <param name="data">�ַ�������ֵ</param>
        /// <param name="paramName">�ַ�������</param>
        /// <remarks>
        /// ���ַ�������ΪNull��մ����׳�ArgumentException�쳣
        
        /// </remarks>
        public static void CheckStringIsNullOrWhiteSpace(this string data, string paramName)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentException(string.Format(Resource.StringParamCanNotBeNullOrEmpty, paramName));
        }

        /// <summary>
        /// ��Exception�����У���ȡ������������Ĵ������
        /// </summary>
        /// <param name="ex">Exception����</param>
        /// <returns>������������Ĵ������</returns>
        public static Exception GetRealException(Exception ex)
		{
			System.Exception lastestEx = ex;

			if (ex is SoapException)
			{
				lastestEx = new SystemSupportException(GetSoapExceptionMessage(ex), ex);
			}
			else
			{
				while (ex != null &&
					(ex is System.Web.HttpUnhandledException || ex is System.Web.HttpException || ex is TargetInvocationException))
				{
					if (ex.InnerException != null)
						lastestEx = ex.InnerException;
					else
						lastestEx = ex;

					ex = ex.InnerException;
				}
			}

			return lastestEx;
		}

		/// <summary>
		/// �õ�SoapException�еĴ�����Ϣ
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		public static string GetSoapExceptionMessage(Exception ex)
		{
			string strNewMsg = ex.Message;

			if (ex is SoapException)
			{
				int i = strNewMsg.LastIndexOf("--> ");

				if (i > 0)
				{
					strNewMsg = strNewMsg.Substring(i + 4);
					i = strNewMsg.IndexOf(": ");

					if (i > 0)
					{
						strNewMsg = strNewMsg.Substring(i + 2);

						i = strNewMsg.IndexOf("\n   ");

						strNewMsg = strNewMsg.Substring(0, i);
					}
				}
			}

			return strNewMsg;
		}

        public static int AssertGreaterThan(int value, int expected, string parameterName = "")
        {
            if (value > expected)
            {
                return value;
            }

            throw new ArgumentOutOfRangeException(
                "Argument should be greater than " + expected, parameterName); /* TODO: Make localizable resource. */
        }

        public static double AssertGreaterThan(double value, double expected, string parameterName = "")
        {
            if (value > expected)
            {
                return value;
            }

            throw new ArgumentOutOfRangeException(
                "Argument should be greater than " + expected, parameterName); /* TODO: Make localizable resource. */
        }

        public static int AssertGreaterThanOrEqual(int value, int expected, string parameterName = "")
        {
            if (value >= expected)
            {
                return value;
            }

            throw new ArgumentOutOfRangeException(
                "Argument should be greater than or equal to " + expected, parameterName);
        }

        public static double AssertGreaterThanOrEqual(double value, double expected, string parameterName = "")
        {
            if (value >= expected)
            {
                return value;
            }

            throw new ArgumentOutOfRangeException(
                "Argument should be greater than or equal to " + expected, parameterName); /* TODO: Make localizable resource. */
        }

        public static int AssertLessThan(int value, int expected, string parameterName = "")
        {
            if (value >= expected)
            {
                throw new ArgumentOutOfRangeException(
                    "Argument should be less than " + expected, parameterName); /* TODO: Make localizable resource. */
            }

            return value;
        }

        public static double AssertLessThan(double value, double expected, string parameterName = "")
        {
            if (value >= expected)
            {
                throw new ArgumentOutOfRangeException(
                    "Argument should be less than " + expected, parameterName); /* TODO: Make localizable resource. */
            }

            return value;
        }

        public static int AssertLessThanOrEqual(int value, int expected, string parameterName = "")
        {
            if (value > expected)
            {
                throw new ArgumentOutOfRangeException(
                    "Argument should be less than or equal to " + expected, parameterName); /* TODO: Make localizable resource. */
            }

            return value;
        }

        public static double AssertLessThanOrEqual(double value, double expected, string parameterName = "")
        {
            if (value > expected)
            {
                throw new ArgumentOutOfRangeException(
                    "Argument should be less than or equal to " + expected, parameterName = "");
            }

            return value;
        }

        public static double AssertEqual(double value, double expected, string parameterName)
        {
            if (!(value == expected))
            {
                throw new ArgumentOutOfRangeException(
                    "Argument should be equal to " + expected, parameterName);
            }

            return value;
        }

        public static bool AssertEqual(object value, object expected, string parameterName)
        {
            if (!ReferenceEquals(value, expected))
            {
                throw new ArgumentOutOfRangeException(
                    "Argument should be equal to " + expected, parameterName);
            }

            return true;
        }

        public static double AssertEqual(int value, int expected, string parameterName = "")
        {
            if (!(value == expected))
            {
                throw new ArgumentOutOfRangeException(
                    "Argument should be equal to " + expected, parameterName);
            }

            return value;
        }

        public static T AssertNotNullAndOfType<T>(object value, string parameterName = "") where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            var result = value as T;
            if (result == null)
            {
                throw new ArgumentException(string.Format(
                    "Expected argument of type " + typeof(T) + ", but was " + value.GetType(), typeof(T), value.GetType()),
                    parameterName);
            }
            return result;
        }

        public static bool AssertTrue(bool value, string parameterName = "")
        {
            FalseThrow(value, parameterName);

            return value;
        }

        /// <summary>
        /// Assert of Type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static T AssertOfType<T>(object value, string parameterName)
        {
            T result;
            try
            {
                result = (T)value;
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException(string.Format(
                    "Expected argument of type " + typeof(T) + ", but was " + value.GetType(), typeof(T), value.GetType()),
                    parameterName);
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T AssertNotNull<T>(T value, string parameterName = "") where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string AssertNotNullOrEmpty(string value, string parameterName = "")
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(
                    "Argument should not be an empty string.", parameterName);
            }

            return value;
        }
    }
}
