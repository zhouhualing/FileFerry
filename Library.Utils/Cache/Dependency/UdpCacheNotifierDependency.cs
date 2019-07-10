using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using WD.Library.Core;

namespace WD.Library.Caching
{
	/// <summary>
	/// 基于Udp通知的CacheDepenedency
	/// </summary>
	public class UdpNotifierCacheDependency : DependencyBase
	{
		private bool changed = false;

		#region CacheNotifyKey
		/// <summary>
		/// 是否改变
		/// </summary>
		public override bool HasChanged
		{
			get
			{
				return this.changed;
			}
		}

		private struct CacheNotifyKey
		{
			private Type cacheQueueType;
			private object cacheKey;

			public Type CacheQueueType
			{
				get
				{
					return this.cacheQueueType;
				}
				set
				{
					this.cacheQueueType = value;
				}
			}

			public object CacheKey
			{
				get
				{
					return this.cacheKey;
				}
				set
				{
					this.cacheKey = value;
				}
			}
		}
		#endregion CacheNotifyKey

		private static Dictionary<CacheNotifyKey, UdpNotifierCacheDependency> cacheItemEntryDict = new Dictionary<CacheNotifyKey, UdpNotifierCacheDependency>();
		private static Thread monitorNotifyThread = null;

		/// <summary>
		/// 构造方法
		/// </summary>
		public UdpNotifierCacheDependency()
		{
			
		}

		/// <summary>
		/// 当CacheItem加入到Cache队列中时
		/// </summary>
		protected internal override void CacheItemBinded()
		{
			CacheNotifyKey key = GetNotifyKey();

			lock (UdpNotifierCacheDependency.cacheItemEntryDict)
			{
				UdpNotifierCacheDependency.cacheItemEntryDict[key] = this;
			}

			InitMonitorNotifyThread();
		}

		/// <summary>
		/// 释放资源
		/// </summary>
		public override void Dispose()
		{
			CacheNotifyKey key = GetNotifyKey();

			lock (UdpNotifierCacheDependency.cacheItemEntryDict)
			{
				if (UdpNotifierCacheDependency.cacheItemEntryDict.ContainsKey(key))
					UdpNotifierCacheDependency.cacheItemEntryDict.Remove(key);
			}

			base.Dispose();
		}

		/// <summary>
		/// 
		/// </summary>
		internal protected override void SetChanged()
		{
			this.changed = true;
		}

		private CacheNotifyKey GetNotifyKey()
		{
			CacheNotifyKey key = new CacheNotifyKey();
			KeyValuePair<object, object> kp = this.CacheItem.GetKeyValue();

			key.CacheQueueType = this.CacheItem.Queue.GetType();
			key.CacheKey = kp.Key;

			return key;
		}

		private static void InitMonitorNotifyThread()
		{
			lock (typeof(UdpNotifierCacheDependency))
			{
				if (UdpNotifierCacheDependency.monitorNotifyThread == null)
				{
					Thread thread = new Thread(new ThreadStart(MonitorUdpCacheNotifyThread));
					thread.IsBackground = true;

					UdpNotifierCacheDependency.monitorNotifyThread = thread;

					thread.Start();
				}
			}
		}

		private static void MonitorUdpCacheNotifyThread()
		{
			using (UdpClient udp = new UdpClient())
			{
				IPEndPoint bindEndPoint = BindToEndPoint(udp.Client);

				while (true)
				{
					try
					{
						byte[] recBuffer = udp.Receive(ref bindEndPoint);

						BinaryFormatter formatter = new BinaryFormatter();

						formatter.Binder = VersionStrategyBinder.Instance;

						using (MemoryStream ms = new MemoryStream(recBuffer))
						{
							CacheNotifyData data = (CacheNotifyData)formatter.Deserialize(ms);

							DoCacheChanged(data);
						}
					}
					catch (System.Exception)
					{
					}
				}
			}
		}

		private static void DoCacheChanged(CacheNotifyData data)
		{
			CacheNotifyKey key = new CacheNotifyKey();

			key.CacheKey = data.CacheKey;
			key.CacheQueueType = data.CacheQueueType;

			UdpNotifierCacheDependency dependency;

			if (data.NotifyType == CacheNotifyType.Clear)
			{
				CacheQueueBase needToClearQueue = null;

				lock (UdpNotifierCacheDependency.cacheItemEntryDict)
				{
					foreach (KeyValuePair<CacheNotifyKey, UdpNotifierCacheDependency> kp in UdpNotifierCacheDependency.cacheItemEntryDict)
					{
						if (kp.Key.CacheQueueType == data.CacheQueueType)
						{
							needToClearQueue = kp.Value.CacheItem.Queue;
							break;
						}
					}
				}

				if (needToClearQueue != null)
					needToClearQueue.Clear();
					//needToClearQueue.SetChanged();
			}
			else
			{
				lock (UdpNotifierCacheDependency.cacheItemEntryDict)
				{
					if (UdpNotifierCacheDependency.cacheItemEntryDict.TryGetValue(key, out dependency))
					{
						switch (data.NotifyType)
						{
							case CacheNotifyType.Invalid:
								//dependency.CacheItem.RemoveCacheItem();	//不自动删除，等待回收
								dependency.changed = true;
								UdpNotifierCacheDependency.cacheItemEntryDict.Remove(key);
								break;
							case CacheNotifyType.Update:
								dependency.CacheItem.SetValue(data.CacheData);
								break;
						}
					}
				}
			}
		}

		private static IPEndPoint BindToEndPoint(Socket socket)
		{
			IPEndPoint bindEndPoint = null;

			UdpCacheClientSettings settings = UdpCacheClientSettings.GetConfig();

			int[] ports = settings.GetPorts();

			for (int i = 0; i < ports.Length; i++)
			{
				try
				{
					bindEndPoint = new IPEndPoint(settings.Address, ports[i]);
					socket.Bind(bindEndPoint);
					socket.ReceiveTimeout = 1000;

					break;
				}
				catch (SocketException ex)
				{
					if (ex.SocketErrorCode != SocketError.AddressAlreadyInUse)
						throw ex;
				}
			}

			ExceptionHelper.FalseThrow(socket.IsBound, "Cache监听线程不能找到可以监听的端口");

			return bindEndPoint;
		}
	}
}
