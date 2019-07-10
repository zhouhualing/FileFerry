using System;
using System.Web;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace WD.Library.Caching
{
	/// <summary>
	/// 用于Web应用清理Cache的HttpModule
	/// </summary>
	public sealed class CacheModule : IHttpModule
	{
		private static DateTime lastScavengeTime = DateTime.Now;
		private static object syncObject = new object();

		#region IHttpModule 成员
		/// <summary>
		/// 释放对象
		/// </summary>
		public void Dispose()
		{
			return;
		}

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="context">HttpApplication对象</param>
		public void Init(HttpApplication context)
		{
			context.BeginRequest += new EventHandler(context_BeginRequest);
		}

		#endregion

		#region 私有方法
		private void context_BeginRequest(object sender, EventArgs e)
		{
			CheckAndExecuteScavenge();
		}

		private void CheckAndExecuteScavenge()
		{
			TimeSpan interval = CacheSettingsSection.GetConfig().ScanvageInterval;

			if (DateTime.Now.Subtract(CacheModule.lastScavengeTime) > interval && CacheManager.InScavengeThread == false)
			{
				lock (syncObject)
				{
					if (CacheManager.InScavengeThread == false &&
						DateTime.Now.Subtract(CacheModule.lastScavengeTime) > interval)
					{
						CacheManager.StartScavengeThread();
						CacheModule.lastScavengeTime = DateTime.Now;
					}
				}
			}
		}
		#endregion
	}
}
