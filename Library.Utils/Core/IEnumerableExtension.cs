using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WD.Library.Core
{
	/// <summary>
	/// IEnumerable的接口扩展
	/// </summary>
	public static class IEnumerableExtension
	{
		/// <summary>
		/// 枚举处理IEnumerable的内容
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="data"></param>
		/// <param name="action"></param>
		public static void ForEach<T>(this IEnumerable<T> data, Action<T> action)
		{
			foreach (T item in data)
			{
				action(item);
			}
		}
	}
}
