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
    /// ȫ�־�̬�࣬ͨ��ע�᷽ʽͳһ����Ӧ����������CacheQueue
    /// </summary>
    public static class CacheManager
    {
        //����Ӧ����������CacheQueue�ֵ䣬����ע��
		private static Dictionary<Type, CacheQueueBase> innerCacheQueues = null;
                
        [ThreadStatic]
        private static bool inScavengeThread = false;

        private static CachingPerformanceCounters totalCounters = null;

		static CacheManager()
		{
            CacheManager.innerCacheQueues = new Dictionary<Type, CacheQueueBase>();
			CacheManager.totalCounters = new CachingPerformanceCounters("_Total_");

			//��̨�����̣߳�������������Ӧ������ÿһ��CacheQueue�е�ÿһ��Cache��
			//���߳���ϵͳ����ʱ�Զ����������ܿͻ��˴������
			//InitScavengingThread();
		}

        /// <summary>
        /// ��ʾ��ǰ�Ƿ��ں�̨����״̬����������Ҫ������Cache�����ڲ��жϵ�ǰ�������̵߳Ĺ���״̬�С�
        /// ��CacheQueue��Dependency���ڲ�������ͨ����״̬�жϵ������Ƿ��������߳�
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
            //��֤�̰߳�ȫ��
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
		/// ����һ�������̣߳����һ��������
		/// </summary>
		public static void StartScavengeThread()
		{
			Thread t = new Thread(new ThreadStart(InternalScavenge));

			t.IsBackground = true;
			t.Priority = ThreadPriority.Lowest;
			t.Start();
		}

        /// <summary>
        /// ��ʼ����������̨�����߳�
        /// </summary>
        /// <returns></returns>
        private static Thread InitScavengingThread()
        {
            Thread t = null;

            if (EnvironmentHelper.Mode == InstanceMode.Windows)
            {
				//����Windows����������Cache���߳�
                t = new Thread(new ThreadStart(ScavengeCache));

                t.IsBackground = true;
                t.Priority = ThreadPriority.Lowest;
                t.Start();
            }

            return t;
        }

        /// <summary>
        /// CacheQueue��������
        /// </summary>
        private static void ScavengeCache()
        {
            while (true)
            {
                //�������ڣ��������ļ��н�������
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
                    //��ע���ÿһ���͵�CacheQueue����ɨ������
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
