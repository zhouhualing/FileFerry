using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Web.UI;

namespace WD.Library.Core
{
	/// <summary>
	/// 为一般集合类所做的扩展
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// 字符串不是Null且Empty
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static bool IsNotEmpty(this string data)
		{
			bool result = false;

			if (data != null)
				result = (string.IsNullOrEmpty(data) == false);

			return result;
		}

		/// <summary>
		/// 字符串不是Null、Empty和WhiteSpace
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static bool IsNotWhiteSpace(this string data)
		{
			bool result = false;

			if (data != null)
				result = (string.IsNullOrEmpty(data.Trim()) == false);

			return result;
		}

		/// <summary>
		/// NameValueCollection的扩展，获取参数的值，并且转换为目标类型。如果不存在，则返回defaultValue
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection"></param>
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static T GetValue<T>(this NameValueCollection collection, string name, T defaultValue)
		{
			name.CheckStringIsNullOrEmpty("name");

			T result = defaultValue;
			
			if (collection != null)
			{
				string data = collection[name];
				
				if (data.IsNotEmpty())
					result = (T)DataConverter.ChangeType(data, typeof(T));
			}

			return result;
		}
	}
}
