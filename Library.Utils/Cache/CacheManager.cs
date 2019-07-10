using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Configuration;
using WD.Library.Core;

namespace WD.Library.Caching
{
    /// <summary>
    /// 全局静态类，通过注册方式统一管理应用域内所有CacheQueue
    /// </summary>
    public static class CacheManager
    {
        //保存应用域内所有CacheQueue字典，用来注册
		private static Dictionary<Type, CacheQueueBase> innerCacheQueues = null;
                
        [ThreadStatic]
        private static bool inScavengeThread = false;

        private static CachingPerformanceCounters totalCounters = null;

		static CacheManager()
		{
            CacheManager.innerCacheQueues = new Dictionary<Type, CacheQueueBase>();
			CacheManager.totalCounters = new CachingPerformanceCounters("_Total_");

			//后台清理线程，定期清理整个应用域中每一个CacheQueue中的每一个Cache项
			//此线程在系统启动时自动启动，不受客户端代码控制
			//InitScavengingThread();
		}

        /// <summary>
        /// 表示当前是否处于后台清理状态，此属性主要用于在Cache队列内部判断当前是清理线程的工作状态中。
        /// 在CacheQueue或Dependency的内部，可以通过此状态判断调用者是否是清理线程
        /// </summary>
        public static bool InScavengeThread
        {
            get
            {
                return CacheManager.inScavengeThread;
            }
            internal set
            {
                CacheManager.inScavengeThread = value;
            }
        }

        public static T GetInstance<T>() where T : CacheQueueBase
        {
            //保证线程安全性
			lock (((IDictionary)CacheManager.innerCacheQueues).SyncRoot)
            {
                CacheQueueBase result;

                System.Type type = typeof(T);

				if (CacheManager.innerCacheQueues.TryGetValue(type, out result) == false)
                {
                    result = (T)Activator.CreateInstance(type, true);
                    CacheManager.innerCacheQueues.Add(type, result);
                }

                return (T)result;
            }
        }

		/// <summary>
		/// 启动一个清理线程，完成一次清理工作
		/// </summary>
		public static void StartScavengeThread()
		{
			Thread t = new Thread(new ThreadStart(InternalScavenge));

			t.IsBackground = true;
			t.Priority = ThreadPriority.Lowest;
			t.Start();
		}

        /// <summary>
        /// 初始化并启动后台清理线程
        /// </summary>
        /// <returns></returns>
        private static Thread InitScavengingThread()
        {
            Thread t = null;

            if (EnvironmentHelper.Mode == InstanceMode.Windows)
            {
				//仅在Windows程序中清理Cache的线程
                t = new Thread(new ThreadStart(ScavengeCache));

                t.IsBackground = true;
                t.Priority = ThreadPriority.Lowest;
                t.Start();
            }

            return t;
        }

        /// <summary>
        /// CacheQueue的清理方法
        /// </summary>
        private static void ScavengeCache()
        {
            while (true)
            {
                //清理周期，在配置文件中进行设置
                Thread.Sleep(CacheSettingsSection.GetConfig().ScanvageInterval);

                InternalScavenge();
            }
        }

        private static void InternalScavenge()
        {
            int totalCount = 0;
            CacheManager.InScavengeThread = true;
            try
            {
                lock (((IDictionary)CacheManager.innerCacheQueues).SyncRoot)
                {
                    //对注册的每一类型的CacheQueue进行扫描清理
                    foreach (KeyValuePair<Type, CacheQueueBase> cache in CacheManager.innerCacheQueues)
                    {
                        if (cache.Value is IScavenge)
                        {
                            IScavenge cacheScavenge = (IScavenge)cache.Value;
                            cacheScavenge.DoScavenging();
                        }

                        if (cache.Value is CacheQueueBase)
                            totalCount += ((CacheQueueBase)cache.Value).Count;
                    }
                }

                CacheManager.totalCounters.EntriesCounter.RawValue = totalCount;
            }
            finally
            {
                CacheManager.InScavengeThread = false;
            }
        }
    }
}
