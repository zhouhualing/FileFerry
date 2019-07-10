using System;
using System.Web;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace WD.Library.Caching
{
	/// <summary>
	/// ����WebӦ������Cache��HttpModule
	/// </summary>
	public sealed class CacheModule : IHttpModule
	{
		private static DateTime lastScavengeTime = DateTime.Now;
		private static object syncObject = new object();

		#region IHttpModule ��Ա
		/// <summary>
		/// �ͷŶ���
		/// </summary>
		public void Dispose()
		{
			return;
		}

		/// <summary>
		/// ��ʼ��
		/// </summary>
		/// <param name="context">HttpApplication����</param>
		public void Init(HttpApplication context)
		{
			context.BeginRequest += new EventHandler(context_BeginRequest);
		}

		#endregion

		#region ˽�з���
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
