using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using WD.Library.Core;
using WD.Library.Caching;

namespace WD.Library.Core
{
	/// <summary>
	/// 操作MonitorData的Helper类
	/// </summary>
	public static class PerformanceMonitorHelper
	{
		/// <summary>
		/// 开始执行计数
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static MonitorData StartMonitor(string name)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(name, "name");

			ExceptionHelper.TrueThrow(StopWatchContextCache.Instance.ContainsKey(name),
				"已经启动了名称为{0}的MonitorData", name);

			MonitorData md = new MonitorData();
			StopWatchContextCache.Instance.Add(name, md);

			md.Stopwatch.Start();

			return md;
		}

		/// <summary>
		/// 计数器是否已经存在
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static bool ExistsMonitor(string name)
		{
			return StopWatchContextCache.Instance.ContainsKey(name);
		}

		/// <summary>
		/// 移除计数器
		/// </summary>
		/// <param name="name"></param>
		public static void RemoveMonitor(string name)
		{
			MonitorData md = null;

			ExceptionHelper.FalseThrow(StopWatchContextCache.Instance.TryGetValue(name, out md),
				"不能找到名称为{0}的MonitorData");

			md.Stopwatch.Stop();
			StopWatchContextCache.Instance.Remove(name);
		}

		/// <summary>
		/// 得到指定名称的监控数据
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static MonitorData GetMonitor(string name)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(name, "name");

			MonitorData md = null;

			ExceptionHelper.FalseThrow(StopWatchContextCache.Instance.TryGetValue(name, out md),
				"不能找到名称为{0}的MonitorData");

			return md;
		}
	}

	internal class StopWatchContextCache : ContextCacheQueueBase<string, MonitorData>
	{
		public static StopWatchContextCache Instance
		{
			get
			{
				return ContextCacheManager.GetInstance<StopWatchContextCache>();
			}
		}
	}
}
