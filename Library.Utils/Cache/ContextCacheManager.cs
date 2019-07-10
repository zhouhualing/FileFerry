using System;
using System.Web;
using System.Text;
using System.Collections.Generic;
using WD.Library.Core;

namespace WD.Library.Caching
{
    /// <summary>
    /// �ڵ����������л����Cache����������Cache�������ڣ������ڵ�ǰ�̣߳�WinForm����һ��Http��Web��)���������
    /// </summary>
    public static class ContextCacheManager
    {
        [ThreadStatic]
        private static Dictionary<System.Type, ContextCacheQueueBase> cacheDictionary;

        /// <summary>
        /// ��ȡCache���е�ʵ��������ö���û�б����棬���Զ�̬����һ��ʵ��
        /// </summary>
        /// <typeparam name="T">Cache���е�����</typeparam>
        /// <returns>Cache���е�ʵ��</returns>
        public static T GetInstance<T>() where T : ContextCacheQueueBase
        {
            System.Type type = typeof(T);

            ContextCacheQueueBase instance;
            Dictionary<System.Type, ContextCacheQueueBase> cacheDict = GetCacheDictionary();

            if (cacheDict.TryGetValue(type, out instance) == false)
            {
                instance = (ContextCacheQueueBase)Activator.CreateInstance(typeof(T), true);
                cacheDict.Add(type, instance);
            }

            return (T)instance;
        }

        private static Dictionary<System.Type, ContextCacheQueueBase> GetCacheDictionary()
        {
            Dictionary<System.Type, ContextCacheQueueBase> cacheDict;

            if (EnvironmentHelper.Mode == InstanceMode.Web)
                cacheDict = (Dictionary<System.Type, ContextCacheQueueBase>)HttpContext.Current.Items["ContextCacheDictionary"];
            else
                cacheDict = ContextCacheManager.cacheDictionary;

            if (cacheDict == null)
                cacheDict = new Dictionary<Type, ContextCacheQueueBase>();

            if (EnvironmentHelper.Mode == InstanceMode.Web)
                HttpContext.Current.Items["ContextCacheDictionary"] = cacheDict;
            else
                ContextCacheManager.cacheDictionary = cacheDict;

            return cacheDict;
        }
    }
}
